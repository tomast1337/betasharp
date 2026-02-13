using betareborn.Blocks.Materials;
using betareborn.Entities;
using betareborn.Util.Maths;
using betareborn.Worlds;

namespace betareborn.Blocks
{
    public class BlockPressurePlate : Block
    {

        private readonly PressurePlateActiviationRule activationRule;

        public BlockPressurePlate(int id, int textureId, PressurePlateActiviationRule rule, Material material) : base(id, textureId, material)
        {
            activationRule = rule;
            SetTickRandomly(true);
            float edgeInset = 1.0F / 16.0F;
            setBoundingBox(edgeInset, 0.0F, edgeInset, 1.0F - edgeInset, 0.03125F, 1.0F - edgeInset);
        }

        public override int GetTickRate()
        {
            return 20;
        }

        public override Box? GetCollisionShape(World world, int x, int y, int z)
        {
            return null;
        }

        public override bool IsOpaque()
        {
            return false;
        }

        public override bool IsFullCube()
        {
            return false;
        }

        public override bool CanPlaceAt(World world, int x, int y, int z)
        {
            return world.shouldSuffocate(x, y - 1, z);
        }

        public override void OnPlaced(World world, int x, int y, int z)
        {
        }

        public override void NeighborUpdate(World world, int x, int y, int z, int id)
        {
            bool shouldBreak = false;
            if (!world.shouldSuffocate(x, y - 1, z))
            {
                shouldBreak = true;
            }

            if (shouldBreak)
            {
                DropStacks(world, x, y, z, world.getBlockMeta(x, y, z));
                world.setBlock(x, y, z, 0);
            }

        }

        public override void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
            if (!world.isRemote)
            {
                if (world.getBlockMeta(x, y, z) != 0)
                {
                    updatePlateState(world, x, y, z);
                }
            }
        }

        public override void OnEntityCollision(World world, int x, int y, int z, Entity entity)
        {
            if (!world.isRemote)
            {
                if (world.getBlockMeta(x, y, z) != 1)
                {
                    updatePlateState(world, x, y, z);
                }
            }
        }

        private void updatePlateState(World world, int x, int y, int z)
        {
            bool wasPressed = world.getBlockMeta(x, y, z) == 1;
            bool shouldBePressed = false;
            float detectionInset = 2.0F / 16.0F;
            List<Entity> entitiesInBox = null;
            if (activationRule == PressurePlateActiviationRule.EVERYTHING)
            {
                entitiesInBox = world.getEntities((Entity)null, new Box((double)((float)x + detectionInset), (double)y, (double)((float)z + detectionInset), (double)((float)(x + 1) - detectionInset), (double)y + 0.25D, (double)((float)(z + 1) - detectionInset)));
            }

            if (activationRule == PressurePlateActiviationRule.MOBS)
            {
                entitiesInBox = world.collectEntitiesByClass(EntityLiving.Class, new Box((double)((float)x + detectionInset), (double)y, (double)((float)z + detectionInset), (double)((float)(x + 1) - detectionInset), (double)y + 0.25D, (double)((float)(z + 1) - detectionInset)));
            }

            if (activationRule == PressurePlateActiviationRule.PLAYERS)
            {
                entitiesInBox = world.collectEntitiesByClass(EntityPlayer.Class, new Box((double)((float)x + detectionInset), (double)y, (double)((float)z + detectionInset), (double)((float)(x + 1) - detectionInset), (double)y + 0.25D, (double)((float)(z + 1) - detectionInset)));
            }

            if (entitiesInBox.Count > 0)
            {
                shouldBePressed = true;
            }

            if (shouldBePressed && !wasPressed)
            {
                world.setBlockMeta(x, y, z, 1);
                world.notifyNeighbors(x, y, z, id);
                world.notifyNeighbors(x, y - 1, z, id);
                world.setBlocksDirty(x, y, z, x, y, z);
                world.playSound((double)x + 0.5D, (double)y + 0.1D, (double)z + 0.5D, "random.click", 0.3F, 0.6F);
            }

            if (!shouldBePressed && wasPressed)
            {
                world.setBlockMeta(x, y, z, 0);
                world.notifyNeighbors(x, y, z, id);
                world.notifyNeighbors(x, y - 1, z, id);
                world.setBlocksDirty(x, y, z, x, y, z);
                world.playSound((double)x + 0.5D, (double)y + 0.1D, (double)z + 0.5D, "random.click", 0.3F, 0.5F);
            }

            if (shouldBePressed)
            {
                world.scheduleBlockUpdate(x, y, z, id, GetTickRate());
            }

        }

        public override void OnBreak(World world, int x, int y, int z)
        {
            int plateState = world.getBlockMeta(x, y, z);
            if (plateState > 0)
            {
                world.notifyNeighbors(x, y, z, id);
                world.notifyNeighbors(x, y - 1, z, id);
            }

            base.OnBreak(world, x, y, z);
        }

        public override void UpdateBoundingBox(BlockView blockView, int x, int y, int z)
        {
            bool isPressed = blockView.getBlockMeta(x, y, z) == 1;
            float edgeInset = 1.0F / 16.0F;
            if (isPressed)
            {
                setBoundingBox(edgeInset, 0.0F, edgeInset, 1.0F - edgeInset, 0.03125F, 1.0F - edgeInset);
            }
            else
            {
                setBoundingBox(edgeInset, 0.0F, edgeInset, 1.0F - edgeInset, 1.0F / 16.0F, 1.0F - edgeInset);
            }

        }

        public override bool IsPoweringSide(BlockView blockView, int x, int y, int z, int side)
        {
            return blockView.getBlockMeta(x, y, z) > 0;
        }

        public override bool IsStrongPoweringSide(World world, int x, int y, int z, int side)
        {
            return world.getBlockMeta(x, y, z) == 0 ? false : side == 1;
        }

        public override bool CanEmitRedstonePower()
        {
            return true;
        }

        public override void SetupRenderBoundingBox()
        {
            float halfWidth = 0.5F;
            float halfHeight = 2.0F / 16.0F;
            float halfDepth = 0.5F;
            setBoundingBox(0.5F - halfWidth, 0.5F - halfHeight, 0.5F - halfDepth, 0.5F + halfWidth, 0.5F + halfHeight, 0.5F + halfDepth);
        }

        public override int GetPistonBehavior()
        {
            return 1;
        }
    }

}