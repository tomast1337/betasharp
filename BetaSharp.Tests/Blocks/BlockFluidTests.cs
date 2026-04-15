using BetaSharp.Blocks;

namespace BetaSharp.Tests.Blocks;

public sealed class BlockFluidTests
{
    [Fact]
    public void LavaNeighborUpdate_WithMetaZeroAndAdjacentWater_HardensToObsidian()
    {
        FakeWorldContext world = new();
        world.ReaderWriter.SetInitial(0, 64, 0, Block.Lava.id, 0);
        world.ReaderWriter.SetInitial(1, 64, 0, Block.Water.id, 0);

        Block.Lava.neighborUpdate(new OnTickEvent(world, 0, 64, 0, 0, Block.Water.id));

        Assert.Equal(Block.Obsidian.id, world.Reader.GetBlockId(0, 64, 0));
    }

    [Fact]
    public void FlowingLavaNeighborUpdate_WithMetaBetweenOneAndFourAndAdjacentWater_HardensToCobblestone()
    {
        FakeWorldContext world = new();
        world.ReaderWriter.SetInitial(0, 64, 0, Block.FlowingLava.id, 3);
        world.ReaderWriter.SetInitial(1, 64, 0, Block.Water.id, 0);

        Block.FlowingLava.neighborUpdate(new OnTickEvent(world, 0, 64, 0, 3, Block.Water.id));

        Assert.Equal(Block.Cobblestone.id, world.Reader.GetBlockId(0, 64, 0));
    }

    [Fact]
    public void FlowingLavaNeighborUpdate_WithMetaAboveFourAndAdjacentWater_DoesNotHarden()
    {
        FakeWorldContext world = new();
        world.ReaderWriter.SetInitial(0, 64, 0, Block.FlowingLava.id, 5);
        world.ReaderWriter.SetInitial(1, 64, 0, Block.Water.id, 0);

        Block.FlowingLava.neighborUpdate(new OnTickEvent(world, 0, 64, 0, 5, Block.Water.id));

        Assert.Equal(Block.FlowingLava.id, world.Reader.GetBlockId(0, 64, 0));
        Assert.Equal(5, world.Reader.GetBlockMeta(0, 64, 0));
    }
}
