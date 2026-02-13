using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockLog : Block
    {
        public BlockLog(int id) : base(id, Material.WOOD)
        {
            textureId = 20;
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 1;
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Block.LOG.id;
        }

        public override void AfterBreak(World world, EntityPlayer player, int x, int y, int z, int meta)
        {
            base.AfterBreak(world, player, x, y, z, meta);
        }

        public override void OnBreak(World world, int x, int y, int z)
        {
            sbyte searchRadius = 4;
            int regionExtent = searchRadius + 1;
            if (world.isRegionLoaded(x - regionExtent, y - regionExtent, z - regionExtent, x + regionExtent, y + regionExtent, z + regionExtent))
            {
                for (int offsetX = -searchRadius; offsetX <= searchRadius; ++offsetX)
                {
                    for (int offsetY = -searchRadius; offsetY <= searchRadius; ++offsetY)
                    {
                        for (int offsetZ = -searchRadius; offsetZ <= searchRadius; ++offsetZ)
                        {
                            int neighborBlockId = world.getBlockId(x + offsetX, y + offsetY, z + offsetZ);
                            if (neighborBlockId == Block.LEAVES.id)
                            {
                                int leavesMeta = world.getBlockMeta(x + offsetX, y + offsetY, z + offsetZ);
                                if ((leavesMeta & 8) == 0)
                                {
                                    world.setBlockMetaWithoutNotifyingNeighbors(x + offsetX, y + offsetY, z + offsetZ, leavesMeta | 8);
                                }
                            }
                        }
                    }
                }
            }

        }

        public override int GetTexture(int side, int meta)
        {
            return side == 1 ? 21 : (side == 0 ? 21 : (meta == 1 ? 116 : (meta == 2 ? 117 : 20)));
        }

        protected override int GetDroppedItemMeta(int blockMeta)
        {
            return blockMeta;
        }
    }

}