using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockSnow : Block
    {

        public BlockSnow(int id, int textureId) : base(id, textureId, Material.SNOW_LAYER)
        {
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 2.0F / 16.0F, 1.0F);
            SetTickRandomly(true);
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            int meta = world.getBlockMeta(x, y, z) & 7;
            return meta >= 3 ?
                new Box((double)x + minX, (double)y + minY, (double)z + minZ, (double)x + maxX, (double)((float)y + 0.5F), (double)z + maxZ) :
                null;
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override bool IsFullCube()
        {
            return false;
        }

        public override void UpdateBoundingBox(BlockView blockView, int x, int y, int z)
        {
            int meta = blockView.getBlockMeta(x, y, z) & 7;
            float height = (float)(2 * (1 + meta)) / 16.0F;
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, height, 1.0F);
        }

        public override bool CanPlaceAt(World world, int x, int y, int z)
        {
            int blockBelowId = world.getBlockId(x, y - 1, z);
            return blockBelowId != 0 && Block.BLOCKS[blockBelowId].IsOpaque() ? world.getMaterial(x, y - 1, z).blocksMovement() : false;
        }

        public override void NeighborUpdate(World world, int x, int y, int z, int id)
        {
            breakIfCannotPlace(world, x, y, z);
        }

        private bool breakIfCannotPlace(World world, int x, int y, int z)
        {
            if (!CanPlaceAt(world, x, y, z))
            {
                DropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AfterBreak(World world, EntityPlayer player, int x, int y, int z, int meta)
        {
            int snowballId = Item.SNOWBALL.id;
            float spreadFactor = 0.7F;
            double offsetX = (double)(world.random.nextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
            double offsetY = (double)(world.random.nextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
            double offsetZ = (double)(world.random.nextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
            EntityItem entityItem = new EntityItem(world, (double)x + offsetX, (double)y + offsetY, (double)z + offsetZ, new ItemStack(snowballId, 1, 0));
            entityItem.delayBeforeCanPickup = 10;
            world.spawnEntity(entityItem);
            world.setBlock(x, y, z, 0);
            player.increaseStat(Stats.Stats.mineBlockStatArray[id], 1);
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Item.SNOWBALL.id;
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 0;
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (world.getBrightness(LightType.Block, x, y, z) > 11)
            {
                DropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
            }

        }

        public override bool IsSideVisible(BlockView blockView, int x, int y, int z, int side)
        {
            return side == 1 ? true : base.IsSideVisible(blockView, x, y, z, side);
        }
    }

}