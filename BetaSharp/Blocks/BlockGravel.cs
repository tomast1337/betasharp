using BetaSharp.Items;

namespace BetaSharp.Blocks;

internal class BlockGravel(int id, int textureId) : BlockSand(id, textureId)
{
    public override int GetDroppedItemId(int blockMeta) => Random.Shared.Next(10) == 0 ? Item.Flint.id : ID;
}
