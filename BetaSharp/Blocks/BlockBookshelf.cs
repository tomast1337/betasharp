using BetaSharp.Blocks.Materials;

namespace BetaSharp.Blocks;

internal class BlockBookshelf(int id, int textureId) : Block(id, textureId, Material.Wood)
{
    public override int getTexture(Side side) => side <= Side.Up ? 4 : TextureId;

    public override int getDroppedItemCount() => 0;
}
