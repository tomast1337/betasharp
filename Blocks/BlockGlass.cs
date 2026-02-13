using betareborn.Blocks.Materials;

namespace betareborn.Blocks
{
    public class BlockGlass : BlockBreakable
    {
        public BlockGlass(int id, int texture, Material material, bool bl) : base(id, texture, material, bl)
        {
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 0;
        }

        public override int GetRenderLayer()
        {
            return 0;
        }
    }

}