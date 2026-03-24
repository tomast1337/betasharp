using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockBookshelf(int id, int textureId) : Block(id, textureId, Material.Wood)
{
    public override int GetTexture(int side) => side <= 1 ? 4 : TextureId;

    public override int GetDroppedItemCount() => 0;
}
