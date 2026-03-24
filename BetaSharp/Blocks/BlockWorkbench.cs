using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockWorkbench : Block
{
    public BlockWorkbench(int id) : base(id, Material.Wood) => TextureId = 59;

    public override int GetTexture(int side) => side == 1 ? TextureId - 16 : side == 0 ? Planks.GetTexture(0) : side != 2 && side != 4 ? TextureId : TextureId + 1;

    public override bool OnUse(OnUseEvent ctx)
    {
        if (ctx.World.IsRemote)
        {
            return true;
        }

        ctx.Player.openCraftingScreen(ctx.X, ctx.Y, ctx.Z);
        return true;
    }
}
