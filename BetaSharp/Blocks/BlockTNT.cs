using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockTNT : Block
{
    public BlockTNT(int id, int textureId) : base(id, textureId, Material.Tnt)
    {
    }

    public override int getTexture(int side) => side == 0 ? textureId + 2 : side == 1 ? textureId + 1 : textureId;

    public override void onPlaced(OnPlacedEvt ctx)
    {
        base.onPlaced(ctx);
        if (ctx.Redstone.IsPowered(ctx.X, ctx.Y, ctx.Z))
        {
            // TODO: Implement this
            // onMetadataChange(new OnMetadataChangeEvt(ctx.WorldRead, ctx.WorldWrite, ctx.Redstone, ctx.Entities, ctx.Broadcaster, ctx.IsRemote, ctx.X, ctx.Y, ctx.Z));
            // ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }
    }

    public override void neighborUpdate(OnTickEvt ctx)
    {
        if (ctx.BlockId > 0 && Blocks[ctx.BlockId].canEmitRedstonePower() && ctx.Redstone.IsPowered(ctx.X, ctx.Y, ctx.Z))
        {
            // TODO: Implement this
            // onMetadataChange(ctx);
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, 0);
        }
    }

    public override int getDroppedItemCount() => 0;

    public override void onDestroyedByExplosion(OnDestroyedByExplosionEvt ctx)
    {
        // TODO: Implement this
        // EntityTNTPrimed entityTNTPrimed = new(ctx.World, ctx.X + 0.5F, ctx.Y + 0.5F, ctx.Z + 0.5F);
        // entityTNTPrimed.fuse = ctx.Random.NextInt(entityTNTPrimed.fuse / 4) + entityTNTPrimed.fuse / 8;
        // ctx.Entities.SpawnEntity(entityTNTPrimed);
    }

    public override void onMetadataChange(OnMetadataChangeEvt ctx)
    {
        if (!ctx.IsRemote)
        {
            if ((ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z) & 1) == 0)
            {
                // TODO: Implement this
                // dropStack(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, new ItemStack(TNT.id, 1, 0));
            }
            else
            {
                // TODO: Implement this
                // EntityTNTPrimed entityTNTPrimed = new EntityTNTPrimed(ctx.WorldRead, (double)(ctx.X + 0.5F), (double)(ctx.Y + 0.5F), (double)(ctx.Z + 0.5F));
                // ctx.Entities.SpawnEntity(entityTNTPrimed);
                // ctx.Broadcaster.PlaySoundAtPos(ctx.X + 0.5F, ctx.Y + 0.5F, ctx.Z + 0.5F, "random.fuse", 1.0F, 1.0F);
            }
        }
    }

    public override void onBlockBreakStart(OnBlockBreakStartEvt ctx)
    {
        if (ctx.Player.getHand() != null && ctx.Player.getHand().itemId == Item.FlintAndSteel.id)
        {
            ctx.WorldWrite.SetBlockMetaWithoutNotifyingNeighbors(ctx.X, ctx.Y, ctx.Z, 1);
        }

        base.onBlockBreakStart(ctx);
    }

    public override bool onUse(OnUseEvt ctx) => base.onUse(ctx);
}
