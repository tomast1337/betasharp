// ┌─────────────────────────────────────────────────────────────────────────────┐
// │  DELETE THIS FILE (or keep it with [Skip]) once snapshots have been         │
// │  generated and committed to source control.                                 │
// └─────────────────────────────────────────────────────────────────────────────┘
//
// HOW TO (RE-)GENERATE SNAPSHOTS
// ────────────────────────────────
//   1. Remove the [Skip] attribute below.
//   2. Run: dotnet run --project BetaSharp.Tests/BetaSharp.Tests.csproj
//   3. Copy the produced files from
//        BetaSharp.Tests/bin/Debug/net10.0/Snapshots/
//      into the project source folder
//        BetaSharp.Tests/Snapshots/
//   4. Make sure the .csproj includes: <None Include="Snapshots\*.json" CopyToOutputDirectory="PreserveNewest" />
//   5. Restore the [Skip] attribute.
//
// NOTE: Uses the new PathFinder (which was already verified equivalent to the
//       old one), so PathFindingOld is not required to regenerate snapshots.

using BetaSharp.Blocks;
using BetaSharp.PathFinding;
using BetaSharp.Tests.Helpers;

namespace BetaSharp.Tests.PathFinding;

[Skip("Run manually to regenerate snapshot files — normally PathFinderTests auto-seeds on first run")]
public class SnapshotGenerator
{
    private static FakeBlockView FlatFloor(int xMin, int xMax, int zMin, int zMax, int y = 0)
    {
        var world = new FakeBlockView();
        world.SetFloorRange(xMin, xMax, zMin, zMax, y);
        return world;
    }

    private static void Generate(string name, FakeBlockView world, TestEntity entity,
        int targetX, int targetY, int targetZ, float maxDistance)
    {
        var result = new PathFinder(world).CreateEntityPathTo(entity, targetX, targetY, targetZ, maxDistance);
        SnapshotStore.Save(name, PathComparer.ExtractNew(result, entity));
        Console.WriteLine($"[SnapshotGenerator] Saved {name}.json");
    }

    [Test]
    public void Generate_Scenario01_StraightPath()
    {
        var world = FlatFloor(-1, 8, -1, 2);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        Generate("Scenario01", world, entity, 5, 1, 0, 20f);
    }

    [Test]
    public void Generate_Scenario02_WallForcesDetour()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        world.SetSolid(3, 1, 0);
        world.SetSolid(3, 2, 0);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        Generate("Scenario02", world, entity, 6, 1, 0, 25f);
    }

    [Test]
    public void Generate_Scenario03_StepUp()
    {
        var world = FlatFloor(-1, 9, -1, 2);
        for (int x = 3; x <= 8; x++)
            world.SetSolid(x, 1, 0);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        Generate("Scenario03", world, entity, 5, 2, 0, 25f);
    }

    [Test]
    public void Generate_Scenario04_FallPrevented()
    {
        var world = new FakeBlockView();
        for (int x = -1; x <= 9; x++)
            for (int z = -1; z <= 2; z++)
                if (x != 3)
                    world.SetSolid(x, 4, z);
        var entity = new TestEntity();
        entity.Place(0.5, 5, 0.5);
        Generate("Scenario04", world, entity, 6, 5, 0, 30f);
    }

    [Test]
    public void Generate_Scenario05_SafeFall()
    {
        var world = new FakeBlockView();
        world.SetFloorRange(-1, 4, -1, 2, y: 2);
        world.SetFloorRange(4, 9, -1, 2, y: 0);
        var entity = new TestEntity();
        entity.Place(0.5, 3, 0.5);
        Generate("Scenario05", world, entity, 7, 1, 0, 30f);
    }

    [Test]
    public void Generate_Scenario06_TargetUnreachable()
    {
        var world = new FakeBlockView();
        world.SetFloorRange(0, 4, 0, 4, y: 0);
        world.SetSolid(1, 1, 2); world.SetSolid(1, 2, 2);
        world.SetSolid(3, 1, 2); world.SetSolid(3, 2, 2);
        world.SetSolid(2, 1, 1); world.SetSolid(2, 2, 1);
        world.SetSolid(2, 1, 3); world.SetSolid(2, 2, 3);
        var entity = new TestEntity();
        entity.Place(2.5, 1, 2.5);
        Generate("Scenario06", world, entity, 10, 1, 10, 50f);
    }

    [Test]
    public void Generate_Scenario07_ClosedDoor()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        world.SetBlock(3, 1, 0, Block.Door.id, meta: 0);
        world.SetBlock(3, 2, 0, Block.Door.id, meta: 8);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        Generate("Scenario07", world, entity, 6, 1, 0, 25f);
    }

    [Test]
    public void Generate_Scenario08_OpenDoor()
    {
        var world = FlatFloor(-1, 9, -1, 2);
        world.SetBlock(3, 1, 0, Block.Door.id, meta: 4);
        world.SetBlock(3, 2, 0, Block.Door.id, meta: 12);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        Generate("Scenario08", world, entity, 6, 1, 0, 25f);
    }

    [Test]
    public void Generate_Scenario09_LavaFloor()
    {
        var world = FlatFloor(-1, 9, -1, 3);
        world.SetBlock(3, 0, 0, Block.Lava.id);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        Generate("Scenario09", world, entity, 6, 1, 0, 25f);
    }

    [Test]
    public void Generate_Scenario10_AdjacentTarget()
    {
        var world = FlatFloor(-1, 4, -1, 2);
        var entity = new TestEntity();
        entity.Place(0.5, 1, 0.5);
        Generate("Scenario10", world, entity, 1, 1, 0, 10f);
    }
}
