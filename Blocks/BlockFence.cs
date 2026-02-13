using betareborn.Blocks.Materials;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockFence : Block
    {

        public BlockFence(int id, int texture) : base(id, texture, Material.WOOD)
        {
        }

        public override bool CanPlaceAt(World world, int x, int y, int z)
        {
            return world.getBlockId(x, y - 1, z) == id ? true : (!world.getMaterial(x, y - 1, z).isSolid() ? false : base.CanPlaceAt(world, x, y, z));
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            return new Box((double)x, (double)y, (double)z, (double)(x + 1), (double)((float)y + 1.5F), (double)(z + 1));
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
            return 11;
        }
    }

}