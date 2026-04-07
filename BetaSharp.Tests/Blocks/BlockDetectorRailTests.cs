using BetaSharp.Blocks;

namespace BetaSharp.Tests.Blocks;

public sealed class BlockDetectorRailTests
{
    private static OnTickEvent Tick(FakeWorldContext world, int x = 0, int y = 64, int z = 0) => new(world, x, y, z, world.Reader.GetBlockMeta(x, y, z), world.Reader.GetBlockId(x, y, z));

    [Fact]
    public void OnTick_UnpoweredDetectorRail_DoesNothing()
    {
        FakeWorldContext world = new();
        world.ReaderWriter.SetInitial(0, 63, 0, Block.Stone.id);
        world.ReaderWriter.SetInitial(0, 64, 0, Block.DetectorRail.id);

        Block.DetectorRail.onTick(Tick(world));

        Assert.Equal(Block.DetectorRail.id, world.Reader.GetBlockId(0, 64, 0));
        Assert.Equal(0, world.Reader.GetBlockMeta(0, 64, 0));
        Assert.Empty(world.TickSchedulerSpy.ScheduledTicks);
    }
}
