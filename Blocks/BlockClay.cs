using betareborn.Blocks.Materials;
using betareborn.Items;

namespace betareborn.Blocks
{
    public class BlockClay : Block
    {

        public BlockClay(int id, int textureId) : base(id, textureId, Material.CLAY)
        {
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Item.CLAY.id;
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 4;
        }
    }

}