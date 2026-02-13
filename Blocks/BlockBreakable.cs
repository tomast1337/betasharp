using betareborn.Blocks.Materials;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockBreakable : Block
    {
        private bool hideAdjacentFaces;

        protected BlockBreakable(int id, int textureId, Material material, bool hideAdjacentFaces) : base(id, textureId, material)
        {
            this.hideAdjacentFaces = hideAdjacentFaces;
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override bool IsSideVisible(BlockView blockView, int x, int y, int z, int side)
        {
            int neighborBlockId = blockView.getBlockId(x, y, z);
            return !hideAdjacentFaces && neighborBlockId == id ? false : base.IsSideVisible(blockView, x, y, z, side);
        }
    }

}