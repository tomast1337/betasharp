using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockCake : Block
    {

        public BlockCake(int id, int textureId) : base(id, textureId, Material.CAKE)
        {
            SetTickRandomly(true);
        }

        public override void UpdateBoundingBox(BlockView blockView, int x, int y, int z)
        {
            int slicesEaten = blockView.getBlockMeta(x, y, z);
            float edgeInset = 1.0F / 16.0F;
            float minX = (float)(1 + slicesEaten * 2) / 16.0F;
            float height = 0.5F;
            setBoundingBox(minX, 0.0F, edgeInset, 1.0F - edgeInset, height, 1.0F - edgeInset);
        }

        public override void SetupRenderBoundingBox()
        {
            float edgeInset = 1.0F / 16.0F;
            float height = 0.5F;
            setBoundingBox(edgeInset, 0.0F, edgeInset, 1.0F - edgeInset, height, 1.0F - edgeInset);
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            int slicesEaten = world.getBlockMeta(x, y, z);
            float edgeInset = 1.0F / 16.0F;
            float minX = (float)(1 + slicesEaten * 2) / 16.0F;
            float height = 0.5F;
            return new Box((double)((float)x + minX), (double)y, (double)((float)z + edgeInset), (double)((float)(x + 1) - edgeInset), (double)((float)y + height - edgeInset), (double)((float)(z + 1) - edgeInset));
        }

        public override Box GetBoundingBox(World world, int x, int y, int z)
        {
            int slicesEaten = world.getBlockMeta(x, y, z);
            float edgeInset = 1.0F / 16.0F;
            float minX = (float)(1 + slicesEaten * 2) / 16.0F;
            float height = 0.5F;
            return new Box((double)((float)x + minX), (double)y, (double)((float)z + edgeInset), (double)((float)(x + 1) - edgeInset), (double)((float)y + height), (double)((float)(z + 1) - edgeInset));
        }

        public override int GetTexture(int side, int meta)
        {
            return side == 1 ? textureId : (side == 0 ? textureId + 3 : (meta > 0 && side == 4 ? textureId + 2 : textureId + 1));
        }

        public override int GetTexture(int side)
        {
            return side == 1 ? textureId : (side == 0 ? textureId + 3 : textureId + 1);
        }

        public override bool IsFullCube()
        {
            return false;
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override bool OnUse(World world, int x, int y, int z, EntityPlayer player)
        {
            tryEat(world, x, y, z, player);
            return true;
        }

        public override void OnBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
        {
            tryEat(world, x, y, z, player);
        }

        private void tryEat(World world, int x, int y, int z, EntityPlayer player)
        {
            if (player.health < 20)
            {
                player.heal(3);
                int var6 = world.getBlockMeta(x, y, z) + 1;
                if (var6 >= 6)
                {
                    world.setBlock(x, y, z, 0);
                }
                else
                {
                    world.setBlockMeta(x, y, z, var6);
                    world.setBlocksDirty(x, y, z);
                }
            }

        }

        public override bool CanPlaceAt(World world, int x, int y, int z)
        {
            return !base.CanPlaceAt(world, x, y, z) ? false : CanGrow(world, x, y, z);
        }

        public override void NeighborUpdate(World world, int x, int y, int z, int id)
        {
            if (!CanGrow(world, x, y, z))
            {
                DropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
            }

        }

        public override bool CanGrow(World world, int x, int y, int z)
        {
            return world.getMaterial(x, y - 1, z).isSolid();
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 0;
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return 0;
        }
    }

}