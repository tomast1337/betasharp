using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockTNT : Block
    {
        public BlockTNT(int id, int textureId) : base(id, textureId, Material.TNT)
        {
        }

        public override int GetTexture(int side)
        {
            return side == 0 ? textureId + 2 : (side == 1 ? textureId + 1 : textureId);
        }

        public override void OnPlaced(World world, int x, int y, int z)
        {
            base.OnPlaced(world, x, y, z);
            if (world.isPowered(x, y, z))
            {
                OnMetadataChange(world, x, y, z, 1);
                world.setBlock(x, y, z, 0);
            }

        }

        public override void NeighborUpdate(World world, int x, int y, int z, int id)
        {
            if (id > 0 && Block.BLOCKS[id].CanEmitRedstonePower() && world.isPowered(x, y, z))
            {
                OnMetadataChange(world, x, y, z, 1);
                world.setBlock(x, y, z, 0);
            }

        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 0;
        }

        public override void OnDestroyedByExplosion(World world, int x, int y, int z)
        {
            EntityTNTPrimed entityTNTPrimed = new EntityTNTPrimed(world, (double)((float)x + 0.5F), (double)((float)y + 0.5F), (double)((float)z + 0.5F));
            entityTNTPrimed.fuse = world.random.nextInt(entityTNTPrimed.fuse / 4) + entityTNTPrimed.fuse / 8;
            world.spawnEntity(entityTNTPrimed);
        }

        public override void OnMetadataChange(World world, int x, int y, int z, int meta)
        {
            if (!world.isRemote)
            {
                if ((meta & 1) == 0)
                {
                    DropStack(world, x, y, z, new ItemStack(Block.TNT.id, 1, 0));
                }
                else
                {
                    EntityTNTPrimed entityTNTPrimed = new EntityTNTPrimed(world, (double)((float)x + 0.5F), (double)((float)y + 0.5F), (double)((float)z + 0.5F));
                    world.spawnEntity(entityTNTPrimed);
                    world.playSound(entityTNTPrimed, "random.fuse", 1.0F, 1.0F);
                }

            }
        }

        public override void OnBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
        {
            if (player.getHand() != null && player.getHand().itemId == Item.FLINT_AND_STEEL.id)
            {
                world.setBlockMetaWithoutNotifyingNeighbors(x, y, z, 1);
            }

            base.OnBlockBreakStart(world, x, y, z, player);
        }

        public override bool OnUse(World world, int x, int y, int z, EntityPlayer player)
        {
            return base.OnUse(world, x, y, z, player);
        }
    }

}