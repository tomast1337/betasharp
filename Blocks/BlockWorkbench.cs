using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockWorkbench : Block
    {

        public BlockWorkbench(int id) : base(id, Material.WOOD)
        {
            textureId = 59;
        }

        public override int GetTexture(int side)
        {
            return side == 1 ? textureId - 16 : (side == 0 ? Block.PLANKS.GetTexture(0) : (side != 2 && side != 4 ? textureId : textureId + 1));
        }

        public override bool OnUse(World world, int x, int y, int z, EntityPlayer player)
        {
            if (world.isRemote)
            {
                return true;
            }
            else
            {
                player.openCraftingScreen(x, y, z);
                return true;
            }
        }
    }

}