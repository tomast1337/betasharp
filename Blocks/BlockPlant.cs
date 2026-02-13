using betareborn.Blocks.Materials;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockPlant : Block
    {
        public BlockPlant(int id, int textureId) : base(id, Material.PLANT)
        {
            base.textureId = textureId;
            SetTickRandomly(true);
            float halfSize = 0.2F;
            setBoundingBox(0.5F - halfSize, 0.0F, 0.5F - halfSize, 0.5F + halfSize, halfSize * 3.0F, 0.5F + halfSize);
        }

        public override bool CanPlaceAt(World world, int x, int y, int z)
        {
            return base.CanPlaceAt(world, x, y, z) && canPlantOnTop(world.getBlockId(x, y - 1, z));
        }

        protected virtual bool canPlantOnTop(int id)
        {
            return id == Block.GRASS_BLOCK.id || id == Block.DIRT.id || id == Block.FARMLAND.id;
        }

        public override void NeighborUpdate(World world, int x, int y, int z, int id)
        {
            base.NeighborUpdate(world, x, y, z, id);
            breakIfCannotGrow(world, x, y, z);
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            breakIfCannotGrow(world, x, y, z);
        }

        protected void breakIfCannotGrow(World world, int x, int y, int z)
        {
            if (!CanGrow(world, x, y, z))
            {
                DropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
            }

        }

        public override bool CanGrow(World world, int x, int y, int z)
        {
            return (world.getBrightness(x, y, z) >= 8 || world.hasSkyLight(x, y, z)) && canPlantOnTop(world.getBlockId(x, y - 1, z));
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            return null;
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override bool IsFullCube()
        {
            return false;
        }

        public override int GetRenderType()
        {
            return 1;
        }
    }

}