using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockIce : BlockBreakable
    {

        public BlockIce(int id, int textureId) : base(id, textureId, Material.ICE, false)
        {
            Slipperiness = 0.98F;
            SetTickRandomly(true);
        }

        public override int GetRenderLayer()
        {
            return 1;
        }

        public override bool IsSideVisible(BlockView blockView, int x, int y, int z, int side)
        {
            return base.IsSideVisible(blockView, x, y, z, 1 - side);
        }

        public override void AfterBreak(World world, EntityPlayer player, int x, int y, int z, int meta)
        {
            base.AfterBreak(world, player, x, y, z, meta);
            Material materialBelow = world.getMaterial(x, y - 1, z);
            if (materialBelow.blocksMovement() || materialBelow.isFluid())
            {
                world.setBlock(x, y, z, Block.FLOWING_WATER.id);
            }

        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 0;
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (world.getBrightness(LightType.Block, x, y, z) > 11 - Block.BLOCK_LIGHT_OPACITY[id])
            {
                DropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, Block.WATER.id);
            }

        }

        public override int GetPistonBehavior()
        {
            return 0;
        }
    }

}