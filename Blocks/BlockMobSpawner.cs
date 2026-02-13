using betareborn.Blocks.Entities;
using betareborn.Blocks.Materials;

namespace betareborn.Blocks
{
    public class BlockMobSpawner : BlockWithEntity
    {

        public BlockMobSpawner(int id, int textureId) : base(id, textureId, Material.STONE)
        {
        }

        protected override BlockEntity getBlockEntity()
        {
            return new BlockEntityMobSpawner();
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return 0;
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 0;
        }

        public override bool IsOpaque()
        {
            return false;
        }
    }

}