using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockSandStone(int id) : Block(id, 192, Material.Stone)
{
    public override int GetTexture(int side) => side switch
    {
        1 => TextureId - 16,
        0 => TextureId + 16,
        _ => TextureId
    };
}
