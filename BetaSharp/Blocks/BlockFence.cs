using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockFence : Block
{
    public BlockFence(int id, int texture) : base(id, texture, Material.Wood)
    {
    }

    public override bool canPlaceAt(CanPlaceAtCtx ctx)
    {
        return ctx.Level.Reader.GetBlockId(ctx.X, ctx.Y - 1, ctx.Z) == id ? true : !ctx.Level.Reader.GetMaterial(ctx.X, ctx.Y - 1, ctx.Z).IsSolid ? false : base.canPlaceAt(ctx);
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        return new Box(x, y, z, x + 1, y + 1.5F, z + 1);
    }

    public override bool isOpaque()
    {
        return false;
    }

    public override bool isFullCube()
    {
        return false;
    }

    public override BlockRendererType getRenderType()
    {
        return BlockRendererType.Fence;
    }
}
