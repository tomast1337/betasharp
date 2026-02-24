using BetaSharp.Blocks;
using BetaSharp.Tests.Helpers;
using BetaSharp.Util.Maths;
using OldPathFinder = BetaSharp.PathFindingOLd.PathFinder;
using NewPathFinder = BetaSharp.PathFinding.PathFinder;

namespace BetaSharp.Tests.PathFinding;

/// <summary>
/// Blackbox tests that run each scenario through the legacy PathFinder and the refactored one,
/// then assert that both produce identical waypoint sequences.
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
        {
            await Assert.That(actual[i]).IsEqualTo(expected[i]);
        }
    }

    // ── scenario 1 ───────────────────────────────────────────────────────────

    /// <summary>
    /// Straight open path: no obstacles between entity and target.
    /// Expected: both finders produce an identical direct path along the X axis.
    /// </summary>
    [Test]
    public async Task Scenario01_StraightPath_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 8, -1, 2);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 5, 1, 0, 20f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 5, 1, 0, 20f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 2 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A two-block-tall wall at X=3 forces a detour via Z±1.
    /// Both finders must agree on the detour route taken.
    /// </summary>
    [Test]
    public async Task Scenario02_WallForcesDetour_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        // Two-block-tall wall (entity size is 2 blocks tall so both rows must be solid)
        world.SetSolid(3, 1, 0);
        world.SetSolid(3, 2, 0);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 6, 1, 0, 25f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 6, 1, 0, 25f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 3 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A one-block-high step at X=3 leads to a raised platform.
    /// Both finders should step up identically.
    /// </summary>
    [Test]
    public async Task Scenario03_StepUp_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 2);
        // One-block-high step at X=3 and raised platform beyond it (floor at Y=1)
        for (int x = 3; x <= 8; x++)
            world.SetSolid(x, 1, 0);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);

        // Target sits on the raised platform at Y=2 (entity feet on top of Y=1 block)
        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 5, 2, 0, 25f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 5, 2, 0, 25f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 4 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A gap at X=3 drops 4+ blocks (fallDistance ≥ 4 → prevented).
    /// Both finders must avoid X=3 and agree on the detour.
    ///
    /// Entity placed at Y=5, floor at Y=4 everywhere except the gap column (X=3).
    /// With 4 consecutive air blocks below Y=5 at X=3, getSafePoint returns null.
    /// </summary>
    [Test]
    public async Task Scenario04_FallPrevented_ReturnsMatchingWaypoints()
    {
        var world = new FakeBlockView();
        // Floor at Y=4 for all X/Z except the gap column at X=3
        for (int x = -1; x <= 9; x++)
            for (int z = -1; z <= 2; z++)
                if (x != 3)
                    world.SetSolid(x, 4, z);

        var entity = new TestEntity();
        entity.Place(0.5, 5, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 6, 5, 0, 30f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 6, 5, 0, 30f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 5 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A two-block drop from a higher platform to a lower one (fallDistance = 2, allowed).
    /// Both finders should produce the same path that descends to the lower level.
    ///
    /// Layout (X axis):
    ///   X 0..3: floor at Y=2 (entity walks at Y=3)
    ///   X 4..9: floor at Y=0 (entity walks at Y=1)
    ///   Transition at X=4: entity falls from Y=3 to Y=1 (2-block safe fall)
    /// </summary>
    [Test]
    public async Task Scenario05_SafeFall_ReturnsMatchingWaypoints()
    {
        var world = new FakeBlockView();
        // Higher platform (Y=2 floor)
        world.SetFloorRange(-1, 4, -1, 2, y: 2);
        // Lower platform (Y=0 floor)
        world.SetFloorRange(4, 9, -1, 2, y: 0);

        var entity = new TestEntity();
        entity.Place(0.5, 3, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 7, 1, 0, 30f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 7, 1, 0, 30f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 6 ───────────────────────────────────────────────────────────

    /// <summary>
    /// The entity is completely enclosed by solid walls; the target is unreachable.
    /// Both finders must return null (closestPoint == start).
    /// </summary>
    [Test]
    public async Task Scenario06_TargetUnreachable_BothReturnNull()
    {
        var world = new FakeBlockView();
        // Floor
        world.SetFloorRange(0, 4, 0, 4, y: 0);
        // Four walls (two blocks tall each side) sealing position (2,1,2)
        world.SetSolid(1, 1, 2); world.SetSolid(1, 2, 2); // west
        world.SetSolid(3, 1, 2); world.SetSolid(3, 2, 2); // east
        world.SetSolid(2, 1, 1); world.SetSolid(2, 2, 1); // north
        world.SetSolid(2, 1, 3); world.SetSolid(2, 2, 3); // south

        var entity = new TestEntity();
        // Place entity so boundingBox.minX/Y/Z floor to (2,1,2)
        entity.Place(2.5, 1, 2.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 10, 1, 10, 50f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 10, 1, 10, 50f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 7 ───────────────────────────────────────────────────────────

    /// <summary>
    /// A closed wooden door at (3,1,0) blocks the direct path.
    /// meta = 0 → BlockDoor.isOpen(0) = false → treated as solid → detour required.
    /// Both finders must agree on the detour.
    /// </summary>
    [Test]
    public async Task Scenario07_ClosedDoor_BlocksPath_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        // Closed wooden door (meta=0 → isOpen=false) at entity level
        world.SetBlock(3, 1, 0, Block.Door.id, meta: 0);
        world.SetBlock(3, 2, 0, Block.Door.id, meta: 8); // upper half of door

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 6, 1, 0, 25f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 6, 1, 0, 25f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 8 ───────────────────────────────────────────────────────────

    /// <summary>
    /// An open wooden door at (3,1,0) allows the direct path through.
    /// meta = 4 → BlockDoor.isOpen(4) = true → treated as passable air.
    /// Both finders must agree on the direct path.
    /// </summary>
    [Test]
    public async Task Scenario08_OpenDoor_AllowsPath_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 2);
        // Open wooden door (meta bit 2 set → isOpen=true)
        world.SetBlock(3, 1, 0, Block.Door.id, meta: 4);
        world.SetBlock(3, 2, 0, Block.Door.id, meta: 12); // upper half, open

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 6, 1, 0, 25f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 6, 1, 0, 25f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 9 ───────────────────────────────────────────────────────────

    /// <summary>
    /// Lava at Y=0 (the floor cell) at X=3 causes the fall-detection loop to return -2,
    /// which makes getSafePoint return null for any position above lava.
    /// Both finders must avoid that column and agree on the detour.
    /// </summary>
    [Test]
    public async Task Scenario09_LavaFloor_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        // Replace the floor tile at X=3 with stationary lava (material = Material.Lava)
        world.SetBlock(3, 0, 0, Block.Lava.id);

        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 6, 1, 0, 25f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 6, 1, 0, 25f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }

    // ── scenario 10 ──────────────────────────────────────────────────────────

    /// <summary>
    /// Target is immediately adjacent to the entity (one block east).
    /// Both finders must return a minimal path of length 1 (just the target point).
    /// </summary>
    [Test]
    public async Task Scenario10_AdjacentTarget_ReturnsMatchingWaypoints()
    {
        var world = FlatFloor(-1, 4, -1, 2);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);

        var oldPath = PathComparer.ExtractOld(new OldPathFinder(world).createEntityPathTo(entity, 1, 1, 0, 10f), entity);
        var newPath = PathComparer.ExtractNew(new NewPathFinder(world).CreateEntityPathTo(entity, 1, 1, 0, 10f), entity);

        await AssertPathsEqual(oldPath, newPath);
    }
}
