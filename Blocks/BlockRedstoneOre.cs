using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockRedstoneOre : Block
    {

        private bool lit;

        public BlockRedstoneOre(int id, int textureId, bool lit) : base(id, textureId, Material.STONE)
        {
            if (lit)
            {
                SetTickRandomly(true);
            }

            this.lit = lit;
        }

        public override int GetTickRate()
        {
            return 30;
        }

        public override void OnBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
        {
            light(world, x, y, z);
            base.OnBlockBreakStart(world, x, y, z, player);
        }

        public override void OnSteppedOn(World world, int x, int y, int z, Entity entity)
        {
            light(world, x, y, z);
            base.OnSteppedOn(world, x, y, z, entity);
        }

        public override bool OnUse(World world, int x, int y, int z, EntityPlayer player)
        {
            light(world, x, y, z);
            return base.OnUse(world, x, y, z, player);
        }

        private void light(World world, int x, int y, int z)
        {
            spawnParticles(world, x, y, z);
            if (id == Block.REDSTONE_ORE.id)
            {
                world.setBlock(x, y, z, Block.LIT_REDSTONE_ORE.id);
            }

        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (id == Block.LIT_REDSTONE_ORE.id)
            {
                world.setBlock(x, y, z, Block.REDSTONE_ORE.id);
            }

        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Item.REDSTONE.id;
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 4 + random.nextInt(2);
        }

        public override void RandomDisplayTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (lit)
            {
                spawnParticles(world, x, y, z);
            }

        }

        private void spawnParticles(World world, int x, int y, int z)
        {
            java.util.Random random = world.random;
            double faceOffset = 1.0D / 16.0D;

            for (int direction = 0; direction < 6; ++direction)
            {
                double particleX = (double)((float)x + random.nextFloat());
                double particleY = (double)((float)y + random.nextFloat());
                double particleZ = (double)((float)z + random.nextFloat());
                if (direction == 0 && !world.isOpaque(x, y + 1, z))
                {
                    particleY = (double)(y + 1) + faceOffset;
                }

                if (direction == 1 && !world.isOpaque(x, y - 1, z))
                {
                    particleY = (double)(y + 0) - faceOffset;
                }

                if (direction == 2 && !world.isOpaque(x, y, z + 1))
                {
                    particleZ = (double)(z + 1) + faceOffset;
                }

                if (direction == 3 && !world.isOpaque(x, y, z - 1))
                {
                    particleZ = (double)(z + 0) - faceOffset;
                }

                if (direction == 4 && !world.isOpaque(x + 1, y, z))
                {
                    particleX = (double)(x + 1) + faceOffset;
                }

                if (direction == 5 && !world.isOpaque(x - 1, y, z))
                {
                    particleX = (double)(x + 0) - faceOffset;
                }

                if (particleX < (double)x || particleX > (double)(x + 1) || particleY < 0.0D || particleY > (double)(y + 1) || particleZ < (double)z || particleZ > (double)(z + 1))
                {
                    world.addParticle("reddust", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                }
            }

        }
    }

}