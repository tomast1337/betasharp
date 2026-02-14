using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityMobSpawner : BlockEntity
{
    public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(BlockEntityMobSpawner).TypeHandle);

    public int spawnDelay = -1;
    private string spawnedEntityId = "Pig";
    public double rotation;
    public double lastRotation = 0.0D;

    public BlockEntityMobSpawner()
    {
        spawnDelay = 20;
    }

    public string getSpawnedEntityId()
    {
        return spawnedEntityId;
    }

    public void setSpawnedEntityId(string spawnedEntityId)
    {
        this.spawnedEntityId = spawnedEntityId;
    }

    public bool isPlayerInRange()
    {
        return World.getClosestPlayer(X + 0.5D, Y + 0.5D, Z + 0.5D, 16.0D) != null;
    }

    public override void Tick()
    {
        lastRotation = rotation;
        if (isPlayerInRange())
        {
            double particleX = (double)(X + World.random.nextFloat());
            double particleY = (double)(Y + World.random.nextFloat());
            double particleZ = (double)(Z + World.random.nextFloat());
            World.addParticle("smoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
            World.addParticle("flame", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);

            for (rotation += 1000.0F / (spawnDelay + 200.0F); rotation > 360.0D; lastRotation -= 360.0D)
            {
                rotation -= 360.0D;
            }

            if (!World.isRemote)
            {
                if (spawnDelay == -1)
                {
                    resetDelay();
                }

                if (spawnDelay > 0)
                {
                    --spawnDelay;
                    return;
                }

                byte max = 4;

                for (int spawnAttempt = 0; spawnAttempt < max; ++spawnAttempt)
                {
                    EntityLiving entityLiving = (EntityLiving)EntityRegistry.create(spawnedEntityId, World);
                    if (entityLiving == null)
                    {
                        return;
                    }

                    int count = World.collectEntitiesByClass(entityLiving.getClass(), new Box(X, Y, Z, X + 1, Y + 1, Z + 1).expand(8.0D, 4.0D, 8.0D)).Count;
                    if (count >= 6)
                    {
                        resetDelay();
                        return;
                    }

                    if (entityLiving != null)
                    {
                        double posX = X + (World.random.nextDouble() - World.random.nextDouble()) * 4.0D;
                        double posY = Y + World.random.nextInt(3) - 1;
                        double posZ = Z + (World.random.nextDouble() - World.random.nextDouble()) * 4.0D;
                        entityLiving.setPositionAndAnglesKeepPrevAngles(posX, posY, posZ, World.random.nextFloat() * 360.0F, 0.0F);
                        if (entityLiving.canSpawn())
                        {
                            World.spawnEntity(entityLiving);

                            for (int particleIndex = 0; particleIndex < 20; ++particleIndex)
                            {
                                particleX = X + 0.5D + ((double)World.random.nextFloat() - 0.5D) * 2.0D;
                                particleY = Y + 0.5D + ((double)World.random.nextFloat() - 0.5D) * 2.0D;
                                particleZ = Z + 0.5D + ((double)World.random.nextFloat() - 0.5D) * 2.0D;
                                World.addParticle("smoke", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                                World.addParticle("flame", particleX, particleY, particleZ, 0.0D, 0.0D, 0.0D);
                            }

                            entityLiving.animateSpawn();
                            resetDelay();
                        }
                    }
                }
            }

            base.Tick();
        }
    }

    private void resetDelay()
    {
        spawnDelay = 200 + World.random.nextInt(600);
    }

    public override void ReadNbt(NBTTagCompound nbt)
    {
        base.ReadNbt(nbt);
        spawnedEntityId = nbt.GetString("EntityId");
        spawnDelay = nbt.GetShort("Delay");
    }

    public override void WriteNbt(NBTTagCompound nbt)
    {
        base.WriteNbt(nbt);
        nbt.SetString("EntityId", spawnedEntityId);
        nbt.SetShort("Delay", (short)spawnDelay);
    }
}