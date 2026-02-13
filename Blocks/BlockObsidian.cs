namespace betareborn.Blocks
{
    public class BlockObsidian : BlockStone
    {
        public BlockObsidian(int id, int textureId) : base(id, textureId)
        {
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return 1;
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Block.OBSIDIAN.id;
        }
    }

}