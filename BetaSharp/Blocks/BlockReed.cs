using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockReed : Block
{
    public BlockReed(int id, int textureId) : base(id, Material.Plant)
    {
        this.textureId = textureId;
        float halfWidth = 6.0F / 16.0F;
        setBoundingBox(0.5F - halfWidth, 0.0F, 0.5F - halfWidth, 0.5F + halfWidth, 1.0F, 0.5F + halfWidth);
        setTickRandomly(true);
    }

    public override void onTick(OnTickEvt ctx)
    {
        if (ctx.WorldRead.IsAir(ctx.X, ctx.Y + 1, ctx.Z))
        {
            int heightBelow;
            for (heightBelow = 1; ctx.WorldRead.GetBlockId(ctx.X, ctx.Y - heightBelow, ctx.Z) == id; ++heightBelow)
            {
            }

            if (heightBelow < 3)
            {
                int meta = ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z);
                if (meta == 15)
                {
                    ctx.WorldWrite.SetBlock(ctx.X, ctx.Y + 1, ctx.Z, id);
                    ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, 0);
                }
                else
                {
                    ctx.WorldWrite.SetBlockMeta(ctx.X, ctx.Y, ctx.Z, meta + 1);
                }
            }
        }
    }

    public override bool canPlaceAt(CanPlaceAtCtx ctx)
    {
        int blockBelowId = ctx.WorldRead.GetBlockId(ctx.X, ctx.Y - 1, ctx.Z);
        return blockBelowId == id ? true :
            blockBelowId != GrassBlock.id && blockBelowId != Dirt.id ? false :
            ctx.WorldRead.GetMaterial(ctx.X - 1, ctx.Y - 1, ctx.Z) == Material.Water ? true :
            ctx.WorldRead.GetMaterial(ctx.X + 1, ctx.Y - 1, ctx.Z) == Material.Water ? true :
            ctx.WorldRead.GetMaterial(ctx.X, ctx.Y - 1, ctx.Z - 1) == Material.Water ? true : ctx.WorldRead.GetMaterial(ctx.X, ctx.Y - 1, ctx.Z + 1) == Material.Water;
    }

    public override void neighborUpdate(OnTickEvt ctx) => breakIfCannotGrow(ctx);

    protected void breakIfCannotGrow(OnTickEvt ctx)
    {
        if (!canGrow(ctx))
        {
            // TODO: Implement this
            // dropStacks(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z));
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }
    }

    public override bool canGrow(OnTickEvt ctx) => canPlaceAt(new CanPlaceAtCtx(ctx.WorldRead, ctx.WorldWrite, 0, ctx.X, ctx.Y, ctx.Z));

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z) => null;

    public override int getDroppedItemId(int blockMeta) => Item.SugarCane.id;

    public override bool isOpaque() => false;

    public override bool isFullCube() => false;

    public override BlockRendererType getRenderType() => BlockRendererType.Reed;
}
