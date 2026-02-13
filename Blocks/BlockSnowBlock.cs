using betareborn.Blocks.Materials;
using betareborn.Items;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSnowBlock : Block
    {

        public BlockSnowBlock(int id, int textureId) : base(id, textureId, Material.SNOW_BLOCK)
        {
            SetTickRandomly(true);
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Item.SNOWBALL.id;
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 4;
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (world.getBrightness(LightType.Block, x, y, z) > 11)
            {
                DropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
            }

        }
    }

}