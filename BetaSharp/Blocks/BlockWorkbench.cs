using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockWorkbench : Block
{
    public BlockWorkbench(int id) : base(id, Material.Wood) => TextureId = BlockTextures.CraftingTableSide;

    public override int GetTexture(Side side)
    {
        return side switch
        {
            Side.Up => BlockTextures.CraftingTableTop,
            Side.Down => Planks.GetTexture(Side.Down),
            Side.North or Side.West => BlockTextures.CraftingTableFront,
            _ => TextureId
        };
    }

    public override bool OnUse(OnUseEvent ctx)
    {
        if (ctx.World.IsRemote) return true;

        ctx.Player.openCraftingScreen(ctx.X, ctx.Y, ctx.Z);
        return true;
    }
}
