using BetaSharp.Blocks;
using BetaSharp.PathFinding;
using BetaSharp.Tests.Helpers;
using BetaSharp.Util.Maths;

namespace BetaSharp.Tests.PathFinding;

/// <summary>
/// Blackbox snapshot tests for <see cref="PathFinder"/>.
///
/// On the <b>first run</b> each test auto-seeds its snapshot JSON file (written to
/// <c>BetaSharp.Tests/Snapshots/</c>).  Commit those files; all subsequent runs
/// compare the live PathFinder output against the committed golden values, catching
/// regressions automatically.
///
/// World coordinate conventions used throughout:
///   Y = 0  → solid floor level
///   Y = 1  → entity walking level (feet)
///   Y = 2  → entity head level
///
/// Entity defaults: width = 0.6, height = 1.8
///   → sizePoint = (Floor(0.6+1), Floor(1.8+1), Floor(0.6+1)) = (1, 2, 1)
///   → bounding box volume checked per candidate cell: 1×2×1 blocks
///
/// Entity.Place(0.5, 1, 0.5) sets boundingBox.minX/Y/Z to (0.2, 1, 0.2)
///   → Floor values → startPoint = (0, 1, 0)
/// </summary>
public class PathFinderTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static FakeBlockView FlatFloor(int xMin, int xMax, int zMin, int zMax, int y = 0)
    {
        var world = new FakeBlockView();
        world.SetFloorRange(xMin, xMax, zMin, zMax, y);
        return world;
    }

    /// <summary>
    /// Runs the PathFinder for the given scenario, compares the result against the
    /// stored snapshot (seeding it on first run), and asserts equality.
    /// </summary>
    private static async Task RunScenario(
        string snapshotName,
        FakeBlockView world,
        TestEntity entity,
        int targetX, int targetY, int targetZ,
        float maxDistance)
    {
        var actual = PathComparer.ExtractNew(
            new PathFinder(world).CreateEntityPathTo(entity, targetX, targetY, targetZ, maxDistance),
            entity);

        var expected = SnapshotStore.LoadOrSeed(snapshotName, () => actual);
        await AssertPathsEqual(expected, actual);
    }

    private static async Task AssertPathsEqual(List<Vec3D>? expected, List<Vec3D>? actual)
    {
        if (expected == null)
        {
            await Assert.That(actual).IsNull();
            return;
        }

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.Count).IsEqualTo(expected.Count);
        for (int i = 0; i < expected.Count; i++)
            await Assert.That(actual[i]).IsEqualTo(expected[i]);
    }

    // ── scenario 1 ───────────────────────────────────────────────────────────

    /// <summary>
    /// Straight open path: no obstacles between entity and target.
    /// Expected: direct path along the X axis.
    /// </summary>
    [Test]
    public async Task Scenario01_StraightPath_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 8, -1, 2);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        await RunScenario("Scenario01", world, entity, 5, 1, 0, 20f);
    }

    // ── scenario 2 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A two-block-tall wall at X=3 forces a detour via Z±1.
    /// </summary>
    [Test]
    public async Task Scenario02_WallForcesDetour_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        world.SetSolid(3, 1, 0);
        world.SetSolid(3, 2, 0);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        await RunScenario("Scenario02", world, entity, 6, 1, 0, 25f);
    }

    // ── scenario 3 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A one-block-high step at X=3 leads to a raised platform.
    /// PathFinder should step up identically.
    /// </summary>
    [Test]
    public async Task Scenario03_StepUp_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 2);
        for (int x = 3; x <= 8; x++)
            world.SetSolid(x, 1, 0);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        await RunScenario("Scenario03", world, entity, 5, 2, 0, 25f);
    }

    // ── scenario 4 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A gap at X=3 drops 4+ blocks (fallDistance ≥ 4 → prevented).
    /// PathFinder must avoid X=3 and detour around the gap.
    /// </summary>
    [Test]
    public async Task Scenario04_FallPrevented_ReturnsMatchingWaypoints()
    {
        var world = new FakeBlockView();
        for (int x = -1; x <= 9; x++)
            for (int z = -1; z <= 2; z++)
                if (x != 3)
                    world.SetSolid(x, 4, z);

        var entity = new TestEntity();
        entity.Place(0.5, 5, 0.5);
        await RunScenario("Scenario04", world, entity, 6, 5, 0, 30f);
    }

    // ── scenario 5 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A two-block drop from a higher platform to a lower one (safe fall, allowed).
    /// PathFinder should descend to the lower level.
    /// </summary>
    [Test]
    public async Task Scenario05_SafeFall_ReturnsMatchingWaypoints()
    {
        var world = new FakeBlockView();
        world.SetFloorRange(-1, 4, -1, 2, y: 2);
        world.SetFloorRange(4, 9, -1, 2, y: 0);

        var entity = new TestEntity();
        entity.Place(0.5, 3, 0.5);
        await RunScenario("Scenario05", world, entity, 7, 1, 0, 30f);
    }

    // ── scenario 6 ───────────────────────────────────────────────────────────

    /// <summary>
    /// The entity is completely enclosed by solid walls; the target is unreachable.
    /// PathFinder must return null (closestPoint == start).
    /// </summary>
    [Test]
    public async Task Scenario06_TargetUnreachable_BothReturnNull()
    {
        var world = new FakeBlockView();
        world.SetFloorRange(0, 4, 0, 4, y: 0);
        world.SetSolid(1, 1, 2); world.SetSolid(1, 2, 2);
        world.SetSolid(3, 1, 2); world.SetSolid(3, 2, 2);
        world.SetSolid(2, 1, 1); world.SetSolid(2, 2, 1);
        world.SetSolid(2, 1, 3); world.SetSolid(2, 2, 3);

        var entity = new TestEntity();
        entity.Place(2.5, 1, 2.5);
        await RunScenario("Scenario06", world, entity, 10, 1, 10, 50f);
    }

    // ── scenario 7 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A closed wooden door at (3,1,0) blocks the direct path.
    /// meta = 0 → BlockDoor.isOpen(0) = false → treated as solid → detour required.
    /// </summary>
    [Test]
    public async Task Scenario07_ClosedDoor_BlocksPath_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        world.SetBlock(3, 1, 0, Block.Door.id, meta: 0);
        world.SetBlock(3, 2, 0, Block.Door.id, meta: 8);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        await RunScenario("Scenario07", world, entity, 6, 1, 0, 25f);
    }

    // ── scenario 8 ───────────────────────────────────────────────────────────

    /// <summary>
    /// An open wooden door at (3,1,0) allows the direct path through.
    /// meta = 4 → BlockDoor.isOpen(4) = true → treated as passable.
    /// </summary>
    [Test]
    public async Task Scenario08_OpenDoor_AllowsPath_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 2);
        world.SetBlock(3, 1, 0, Block.Door.id, meta: 4);
        world.SetBlock(3, 2, 0, Block.Door.id, meta: 12);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        await RunScenario("Scenario08", world, entity, 6, 1, 0, 25f);
    }

    // ── scenario 9 ───────────────────────────────────────────────────────────

    /// <summary>
    /// Lava at Y=0 at X=3 causes getSafePoint to return null for that column.
    /// PathFinder must avoid that column and detour around it.
    /// </summary>
    [Test]
    public async Task Scenario09_LavaFloor_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        world.SetBlock(3, 0, 0, Block.Lava.id);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        await RunScenario("Scenario09", world, entity, 6, 1, 0, 25f);
    }

    // ── scenario 10 ──────────────────────────────────────────────────────────

    /// <summary>
    /// Target is immediately adjacent to the entity (one block east).
    /// PathFinder must return a minimal one-point path.
    /// </summary>
    [Test]
    public async Task Scenario10_AdjacentTarget_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 4, -1, 2);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        await RunScenario("Scenario10", world, entity, 1, 1, 0, 10f);
    }
}
