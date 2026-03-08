using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockSnow : Block
{
    public BlockSnow(int id, int textureId) : base(id, textureId, Material.SnowLayer)
    {
        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 2.0F / 16.0F, 1.0F);
        setTickRandomly(true);
    }

    public override Box? getCollisionShape(IBlockReader world, int x, int y, int z)
    {
        int meta = world.GetBlockMeta(x, y, z) & 7;
        return meta >= 3 ? new Box(x + BoundingBox.MinX, y + BoundingBox.MinY, z + BoundingBox.MinZ, x + BoundingBox.MaxX, y + 0.5F, z + BoundingBox.MaxZ) : null;
    }

    public override bool isOpaque() => false;

    public override bool isFullCube() => false;

    public override void updateBoundingBox(IBlockReader iBlockReader, int x, int y, int z)
    {
        int meta = iBlockReader.GetBlockMeta(x, y, z) & 7;
        float height = 2 * (1 + meta) / 16.0F;
        setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, height, 1.0F);
    }

    public override bool canPlaceAt(CanPlaceAtCtx ctx)
    {
        int blockBelowId = ctx.WorldRead.GetBlockId(ctx.X, ctx.Y - 1, ctx.Z);
        return blockBelowId != 0 && Blocks[blockBelowId].isOpaque() ? ctx.WorldRead.GetMaterial(ctx.X, ctx.Y - 1, ctx.Z).BlocksMovement : false;
    }

    public override void neighborUpdate(OnTickEvt ctx) => breakIfCannotPlace(ctx);

    private bool breakIfCannotPlace(OnTickEvt ctx)
    {
        if (!canPlaceAt(new CanPlaceAtCtx(ctx.WorldRead, ctx.WorldWrite, 0, ctx.X, ctx.Y, ctx.Z)))
        {
            // TODO: Implement this
            // dropStacks(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z));
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
            return false;
        }

        return true;
    }

    public override void afterBreak(OnAfterBreakEvt evt)
    {
        int snowballId = Item.Snowball.id;
        float spreadFactor = 0.7F;
        double offsetX = Random.Shared.NextSingle() * spreadFactor + (1.0F - spreadFactor) * 0.5D;
        double offsetY = Random.Shared.NextSingle() * spreadFactor + (1.0F - spreadFactor) * 0.5D;
        double offsetZ = Random.Shared.NextSingle() * spreadFactor + (1.0F - spreadFactor) * 0.5D;
        // TODO: Implement this
        // EntityItem entityItem = new(evt.World, evt.X + offsetX, evt.Y + offsetY, evt.Z + offsetZ, new ItemStack(snowballId, 1, 0));
        // entityItem.delayBeforeCanPickup = 10;
        // evt.Entities.SpawnEntity(entityItem);
        // evt.WorldWrite.SetBlock(evt.X, evt.Y, evt.Z, 0);
        evt.Player.increaseStat(Stats.Stats.MineBlockStatArray[id], 1);
    }

    public override int getDroppedItemId(int blockMeta) => Item.Snowball.id;

    public override int getDroppedItemCount() => 0;

    public override void onTick(OnTickEvt ctx)
    {
        if (ctx.Lighting.GetBrightness(LightType.Block, ctx.X, ctx.Y, ctx.Z) > 11)
        {
            // TODO: Implement this
            // dropStacks(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z));
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }
    }

    public override bool isSideVisible(IBlockReader iBlockReader, int x, int y, int z, int side) => side == 1 ? true : base.isSideVisible(iBlockReader, x, y, z, side);
}
