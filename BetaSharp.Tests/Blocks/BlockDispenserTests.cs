using BetaSharp.Blocks;

namespace BetaSharp.Tests.Blocks;

public sealed class BlockDispenserTests
{
    [Fact]
    public void NeighborUpdate_PoweredByEmitter_SchedulesDispenseTick()
    {
        FakeWorldContext world = new();
        world.ReaderWriter.SetInitial(0, 64, 0, Block.Dispenser.id, 3);
        world.ReaderWriter.SetInitial(0, 63, 0, Block.LitRedstoneTorch.id); // powers dispenser position

        Block.Dispenser.neighborUpdate(new OnTickEvent(world, 0, 64, 0, 3, Block.LitRedstoneTorch.id));

        Assert.Contains(world.TickSchedulerSpy.ScheduledTicks, t =>
            t is { X: 0, Y: 64, Z: 0 } && t.BlockId == Block.Dispenser.id && t.TickRate == 4);
    }

    [Fact]
    public void NeighborUpdate_NonEmitterTrigger_DoesNotScheduleTick()
    {
        FakeWorldContext world = new();
        world.ReaderWriter.SetInitial(0, 64, 0, Block.Dispenser.id, 3);
        world.ReaderWriter.SetInitial(0, 63, 0, Block.Stone.id);

        Block.Dispenser.neighborUpdate(new OnTickEvent(world, 0, 64, 0, 3, Block.Stone.id));

        Assert.Empty(world.TickSchedulerSpy.ScheduledTicks);
    }
}
