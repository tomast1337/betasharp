using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockSandStone(int id) : Block(id, 192, Material.Stone)
{
    public override int getTexture(Side side) =>
        side switch
        {
            Side.Up => TextureId - 16,
            Side.Down => TextureId + 16,
            _ => TextureId
        };
}
