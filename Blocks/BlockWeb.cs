using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Items;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockWeb : Block
    {
        public BlockWeb(int id, int texturePosition) : base(id, texturePosition, Material.COBWEB)
        {
        }

        public override void OnEntityCollision(World world, int x, int y, int z, Entity entity)
        {
            entity.slowed = true;
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            return null;
        }

        public override int GetRenderType()
        {
            return 1;
        }

        public override bool IsFullCube()
        {
            return false;
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return Item.STRING.id;
        }
    }

}