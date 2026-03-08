using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Blocks;

internal class BlockIce : BlockBreakable
{
    public BlockIce(int id, int textureId) : base(id, textureId, Material.Ice, false)
    {
        slipperiness = 0.98F;
        setTickRandomly(true);
    }

    public override int getRenderLayer() => 1;

    public override bool isSideVisible(IBlockReader iBlockReader, int x, int y, int z, int side) => base.isSideVisible(iBlockReader, x, y, z, 1 - side);

    public override void afterBreak(OnAfterBreakEvt evt)
    {
        base.afterBreak(evt);
        Material materialBelow = evt.WorldRead.GetMaterial(evt.X, evt.Y - 1, evt.Z);
        if (materialBelow.BlocksMovement || materialBelow.IsFluid)
        {
            evt.WorldWrite.SetBlock(evt.X, evt.Y, evt.Z, FlowingWater.id);
        }
    }

    public override int getDroppedItemCount() => 0;

    public override void onTick(OnTickEvt ctx)
    {
        if (ctx.Lighting.GetBrightness(LightType.Block, ctx.X, ctx.Y, ctx.Z) > 11 - BlockLightOpacity[id])
        {
            // TODO: Implement this
            //dropStacks(ctx.WorldRead, ctx.X, ctx.Y, ctx.Z, ctx.WorldRead.GetBlockMeta(ctx.X, ctx.Y, ctx.Z));
            ctx.WorldWrite.SetBlock(ctx.X, ctx.Y, ctx.Z, Water.id);
        }
    }

    public override int getPistonBehavior() => 0;
}
