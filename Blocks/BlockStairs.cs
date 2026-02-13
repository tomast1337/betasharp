using betareborn.Entities;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockStairs : Block
    {

        private Block baseBlock;

        public BlockStairs(int id, Block block) : base(id, block.textureId, block.Material)
        {
            baseBlock = block;
            SetHardness(block.Hardness);
            SetResistance(block.Resistance / 3.0F);
            SetSoundGroup(block.SoundGroup);
            SetOpacity(255);
        }

        public override void UpdateBoundingBox(BlockView blockView, int x, int y, int z)
        {
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            return base.GetCollisionShape(world, x, y, z);
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
            return 10;
        }

        public override bool IsSideVisible(BlockView blockView, int x, int y, int z, int side)
        {
            return base.IsSideVisible(blockView, x, y, z, side);
        }

        public override void AddIntersectingBoundingBox(World world, int x, int y, int z, Box box, List<Box> boxes)
        {
            int meta = world.getBlockMeta(x, y, z);
            if (meta == 0)
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 0.5F, 1.0F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
            }
            else if (meta == 1)
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 0.5F, 1.0F, 1.0F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(0.5F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
            }
            else if (meta == 2)
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 0.5F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 1.0F, 1.0F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
            }
            else if (meta == 3)
            {
                setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.5F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
                setBoundingBox(0.0F, 0.0F, 0.5F, 1.0F, 0.5F, 1.0F);
                base.AddIntersectingBoundingBox(world, x, y, z, box, boxes);
            }

            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
        }

        public override void RandomDisplayTick(World world, int x, int y, int z, java.util.Random random)
        {
            baseBlock.RandomDisplayTick(world, x, y, z, random);
        }

        public override void OnBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
        {
            baseBlock.OnBlockBreakStart(world, x, y, z, player);
        }

        public override void OnMetadataChange(World world, int x, int y, int z, int meta)
        {
            baseBlock.OnMetadataChange(world, x, y, z, meta);
        }

        public override float getLuminance(BlockView blockView, int x, int y, int z)
        {
            return baseBlock.getLuminance(blockView, x, y, z);
        }

        public override float GetBlastResistance(Entity entity)
        {
            return baseBlock.GetBlastResistance(entity);
        }

        public override int GetRenderLayer()
        {
            return baseBlock.GetRenderLayer();
        }

        public override int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return baseBlock.GetDroppedItemId(blockMeta, random);
        }

        public override int GetDroppedItemCount(java.util.Random random)
        {
            return baseBlock.GetDroppedItemCount(random);
        }

        public override int GetTexture(int side, int meta)
        {
            return baseBlock.GetTexture(side, meta);
        }

        public override int GetTexture(int side)
        {
            return baseBlock.GetTexture(side);
        }

        public override int GetTextureId(BlockView blockView, int x, int y, int z, int side)
        {
            return baseBlock.GetTextureId(blockView, x, y, z, side);
        }

        public override int GetTickRate()
        {
            return baseBlock.GetTickRate();
        }

        public override Box GetBoundingBox(World world, int x, int y, int z)
        {
            return baseBlock.GetBoundingBox(world, x, y, z);
        }

        public override void ApplyVelocity(World world, int x, int y, int z, Entity entity, Vec3D velocity)
        {
            baseBlock.ApplyVelocity(world, x, y, z, entity, velocity);
        }

        public override bool HasCollision()
        {
            return baseBlock.HasCollision();
        }

        public override bool HasCollision(int meta, bool allowLiquids)
        {
            return baseBlock.HasCollision(meta, allowLiquids);
        }

        public override bool CanPlaceAt(World world, int x, int y, int z)
        {
            return baseBlock.CanPlaceAt(world, x, y, z);
        }

        public override void OnPlaced(World world, int x, int y, int z)
        {
            NeighborUpdate(world, x, y, z, 0);
            baseBlock.OnPlaced(world, x, y, z);
        }

        public override void OnBreak(World world, int x, int y, int z)
        {
            baseBlock.OnBreak(world, x, y, z);
        }

        public override void DropStacks(World world, int x, int y, int z, int meta, float luck)
        {
            baseBlock.DropStacks(world, x, y, z, meta, luck);
        }

        public override void OnSteppedOn(World world, int x, int y, int z, Entity entity)
        {
            baseBlock.OnSteppedOn(world, x, y, z, entity);
        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            baseBlock.OnTick(world, x, y, z, random);
        }

        public override bool OnUse(World world, int x, int y, int z, EntityPlayer player)
        {
            return baseBlock.OnUse(world, x, y, z, player);
        }

        public override void OnDestroyedByExplosion(World world, int x, int y, int z)
        {
            baseBlock.OnDestroyedByExplosion(world, x, y, z);
        }

        public override void OnPlaced(World world, int x, int y, int z, EntityLiving placer)
        {
            int facing = MathHelper.floor_double((double)(placer.yaw * 4.0F / 360.0F) + 0.5D) & 3;
            if (facing == 0)
            {
                world.setBlockMeta(x, y, z, 2);
            }

            if (facing == 1)
            {
                world.setBlockMeta(x, y, z, 1);
            }

            if (facing == 2)
            {
                world.setBlockMeta(x, y, z, 3);
            }

            if (facing == 3)
            {
                world.setBlockMeta(x, y, z, 0);
            }

        }
    }

}