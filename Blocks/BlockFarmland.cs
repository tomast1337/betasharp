using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFarmland : Block
    {

        public BlockFarmland(int id) : base(id, Material.SOIL)
        {
            textureId = 87;
            SetTickRandomly(true);
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 15.0F / 16.0F, 1.0F);
            SetOpacity(255);
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            return new Box((double)(x + 0), (double)(y + 0), (double)(z + 0), (double)(x + 1), (double)(y + 1), (double)(z + 1));
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override bool IsFullCube()
        {
            return false;
        }

        public override int GetTexture(int side, int meta)
        {
            return side == 1 && meta > 0 ? textureId - 1 : (side == 1 ? textureId : 2);
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (random.nextInt(5) == 0)
            {
                if (!isWaterNearby(world, x, y, z) && !world.isRaining(x, y + 1, z))
                {
                    int meta = world.getBlockMeta(x, y, z);
                    if (meta > 0)
                    {
                        world.setBlockMeta(x, y, z, meta - 1);
                    }
                    else if (!hasCrop(world, x, y, z))
                    {
                        world.setBlock(x, y, z, Block.DIRT.id);
                    }
                }
                else
                {
                    world.setBlockMeta(x, y, z, 7);
                }
            }

        }

        public override void OnSteppedOn(World world, int x, int y, int z, Entity entity)
        {
            if (world.random.nextInt(4) == 0)
            {
                world.setBlock(x, y, z, Block.DIRT.id);
            }

        }

        private static bool hasCrop(World world, int x, int y, int z)
        {
            sbyte cropRadius = 0;

            for (int var6 = x - cropRadius; var6 <= x + cropRadius; ++var6)
            {
                for (int var7 = z - cropRadius; var7 <= z + cropRadius; ++var7)
                {
                    if (world.getBlockId(var6, y + 1, var7) == Block.WHEAT.id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool isWaterNearby(World world, int x, int y, int z)
        {
            for (int checkX = x - 4; checkX <= x + 4; ++checkX)
            {
                for (int checkY = y; checkY <= y + 1; ++checkY)
                {
                    for (int checkZ = z - 4; checkZ <= z + 4; ++checkZ)
                    {
                        if (world.getMaterial(checkX, checkY, checkZ) == Material.WATER)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void NeighborUpdate(World world, int x, int y, int z, int id)
        {
            base.NeighborUpdate(world, x, y, z, id);
            Material material = world.getMaterial(x, y + 1, z);
            if (material.isSolid())
            {
                world.setBlock(x, y, z, Block.DIRT.id);
            }

        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Block.DIRT.GetDroppedItemId(0, random);
        }
    }

}