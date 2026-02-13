using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockCactus : Block
    {

        public BlockCactus(int id, int textureId) : base(id, textureId, Material.CACTUS)
        {
            SetTickRandomly(true);
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (world.isAir(x, y + 1, z))
            {
                int heightBelow;
                for (heightBelow = 1; world.getBlockId(x, y - heightBelow, z) == id; ++heightBelow)
                {
                }

                if (heightBelow < 3)
                {
                    int growthStage = world.getBlockMeta(x, y, z);
                    if (growthStage == 15)
                    {
                        world.setBlock(x, y + 1, z, id);
                        world.setBlockMeta(x, y, z, 0);
                    }
                    else
                    {
                        world.setBlockMeta(x, y, z, growthStage + 1);
                    }
                }
            }

        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            float edgeInset = 1.0F / 16.0F;
            return new Box((double)((float)x + edgeInset), (double)y, (double)((float)z + edgeInset), (double)((float)(x + 1) - edgeInset), (double)((float)(y + 1) - edgeInset), (double)((float)(z + 1) - edgeInset));
        }

        public override Box GetBoundingBox(World world, int x, int y, int z)
        {
            float edgeInset = 1.0F / 16.0F;
            return new Box((double)((float)x + edgeInset), (double)y, (double)((float)z + edgeInset), (double)((float)(x + 1) - edgeInset), (double)(y + 1), (double)((float)(z + 1) - edgeInset));
        }

        public override int GetTexture(int side)
        {
            return side == 1 ? textureId - 1 : (side == 0 ? textureId + 1 : textureId);
        }

        public override bool IsFullCube()
        {
            return false;
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override int GetRenderType()
        {
            return 13;
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
            if (world.getMaterial(x - 1, y, z).isSolid())
            {
                return false;
            }
            else if (world.getMaterial(x + 1, y, z).isSolid())
            {
                return false;
            }
            else if (world.getMaterial(x, y, z - 1).isSolid())
            {
                return false;
            }
            else if (world.getMaterial(x, y, z + 1).isSolid())
            {
                return false;
            }
            else
            {
                int blockBelowId = world.getBlockId(x, y - 1, z);
                return blockBelowId == Block.CACTUS.id || blockBelowId == Block.SAND.id;
            }
        }

        public override void OnEntityCollision(World world, int x, int y, int z, Entity entity)
        {
            entity.damage((Entity)null, 1);
        }
    }

}