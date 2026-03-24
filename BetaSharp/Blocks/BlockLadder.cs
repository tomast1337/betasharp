using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockLadder(int id, int textureId) : Block(id, textureId, Material.PistonBreakable)
{
    public override Box? GetCollisionShape(IBlockReader world, EntityManager entities, int x, int y, int z)
    {
        const float thickness = 2.0F / 16.0F;
        int meta = world.GetBlockMeta(x, y, z);
        switch (meta)
        {
            case 2:
                SetBoundingBox(0.0F, 0.0F, 1.0F - thickness, 1.0F, 1.0F, 1.0F);
                break;
            case 3:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, thickness);
                break;
            case 4:
                SetBoundingBox(1.0F - thickness, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                break;
            case 5:
                SetBoundingBox(0.0F, 0.0F, 0.0F, thickness, 1.0F, 1.0F);
                break;
        }

        return base.GetCollisionShape(world, entities, x, y, z);
    }

    public override Box GetBoundingBox(IBlockReader world, EntityManager entities, int x, int y, int z)
    {
        const float thickness = 2.0F / 16.0F;
        int meta = world.GetBlockMeta(x, y, z);
        switch (meta)
        {
            case 2:
                SetBoundingBox(0.0F, 0.0F, 1.0F - thickness, 1.0F, 1.0F, 1.0F);
                break;
            case 3:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, thickness);
                break;
            case 4:
                SetBoundingBox(1.0F - thickness, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                break;
            case 5:
                SetBoundingBox(0.0F, 0.0F, 0.0F, thickness, 1.0F, 1.0F);
                break;
        }

        return base.GetBoundingBox(world, entities, x, y, z);
    }

    public override bool IsOpaque() => false;

    public override bool IsFullCube() => false;

    public override BlockRendererType GetRenderType() => BlockRendererType.Ladder;

    public override bool CanPlaceAt(CanPlaceAtContext context) =>
        context.World.Reader.ShouldSuffocate(context.X - 1, context.Y, context.Z) ||
        context.World.Reader.ShouldSuffocate(context.X + 1, context.Y, context.Z) ||
        context.World.Reader.ShouldSuffocate(context.X, context.Y, context.Z - 1) ||
        context.World.Reader.ShouldSuffocate(context.X, context.Y, context.Z + 1);

    public override void OnPlaced(OnPlacedEvent ctx)
    {
        int meta = ctx.World.Reader.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
        if ((meta == 0 || ctx.Direction == 2) && ctx.World.Reader.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z + 1))
        {
            meta = 2;
        }

        if ((meta == 0 || ctx.Direction == 3) && ctx.World.Reader.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z - 1))
        {
            meta = 3;
        }

        if ((meta == 0 || ctx.Direction == 4) && ctx.World.Reader.ShouldSuffocate(ctx.X + 1, ctx.Y, ctx.Z))
        {
            meta = 4;
        }

        if ((meta == 0 || ctx.Direction == 5) && ctx.World.Reader.ShouldSuffocate(ctx.X - 1, ctx.Y, ctx.Z))
        {
            meta = 5;
        }

        ctx.World.Writer.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, meta);
    }

    public override void NeighborUpdate(OnTickEvent ctx)
    {
        int meta = ctx.World.Reader.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
        bool hasSupport = (meta == 2 && ctx.World.Reader.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z + 1)) ||
                          (meta == 3 && ctx.World.Reader.ShouldSuffocate(ctx.X, ctx.Y, ctx.Z - 1)) ||
                          (meta == 4 && ctx.World.Reader.ShouldSuffocate(ctx.X + 1, ctx.Y, ctx.Z)) ||
                          (meta == 5 && ctx.World.Reader.ShouldSuffocate(ctx.X - 1, ctx.Y, ctx.Z));

        if (!hasSupport)
        {
            DropStacks(new OnDropEvent(ctx.World, ctx.X, ctx.Y, ctx.Z, meta));
            ctx.World.Writer.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }

        base.NeighborUpdate(ctx);
    }

    public override int GetDroppedItemCount() => 1;
}
