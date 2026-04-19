using BetaSharp.Blocks.Materials;
using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntityFish : Entity
{
    public EntityPlayer angler;
    public Entity bobber;
    private double clientVelocityX;
    private double clientVelocityY;
    private double clientVelocityZ;
    private bool inGround;
    private int inTile;
    private int positionUpdateTicks;
    public int shake;
    private double targetPitch;
    private double targetX;
    private double targetY;
    private double targetYaw;
    private double targetZ;
    private int ticksCatchable;
    private int ticksInAir;
    private int ticksInGround;
    private int xTile;
    private int yTile;
    private int zTile;

    public EntityFish(IWorldContext world) : base(world)
    {
        xTile = -1;
        yTile = -1;
        zTile = -1;
        inTile = 0;
        inGround = false;
        shake = 0;
        ticksInAir = 0;
        ticksCatchable = 0;
        bobber = null;
        setBoundingBoxSpacing(0.25F, 0.25F);
        ignoreFrustumCheck = true;
    }

    public EntityFish(IWorldContext world, double x, double y, double z) : this(world)
    {
        setPosition(x, y, z);
        ignoreFrustumCheck = true;
    }

    public EntityFish(IWorldContext world, EntityPlayer player) : base(world)
    {
        xTile = -1;
        yTile = -1;
        zTile = -1;
        inTile = 0;
        inGround = false;
        shake = 0;
        ticksInAir = 0;
        ticksCatchable = 0;
        bobber = null;
        ignoreFrustumCheck = true;
        angler = player;
        angler.fishHook = this;
        setBoundingBoxSpacing(0.25F, 0.25F);
        setPositionAndAnglesKeepPrevAngles(player.x, player.y + 1.62D - player.standingEyeHeight, player.z, player.yaw, player.pitch);
        x -= MathHelper.Cos(yaw / 180.0F * (float)Math.PI) * 0.16F;
        y -= 0.1F;
        z -= MathHelper.Sin(yaw / 180.0F * (float)Math.PI) * 0.16F;
        setPosition(x, y, z);
        standingEyeHeight = 0.0F;
        float speed = 0.4F;
        velocityX = -MathHelper.Sin(yaw / 180.0F * (float)Math.PI) * MathHelper.Cos(pitch / 180.0F * (float)Math.PI) * speed;
        velocityZ = MathHelper.Cos(yaw / 180.0F * (float)Math.PI) * MathHelper.Cos(pitch / 180.0F * (float)Math.PI) * speed;
        velocityY = -MathHelper.Sin(pitch / 180.0F * (float)Math.PI) * speed;
        setHeading(velocityX, velocityY, velocityZ, 1.5F, 1.0F);
    }

    public override EntityType Type => EntityRegistry.FishHook;


    public override bool shouldRender(double distanceSquared)
    {
        double renderDistance = boundingBox.AverageEdgeLength * 4.0D;
        renderDistance *= 64.0D;
        return distanceSquared < renderDistance * renderDistance;
    }

    public void setHeading(double dirX, double dirY, double dirZ, float speed, float spread)
    {
        float length = MathHelper.Sqrt(dirX * dirX + dirY * dirY + dirZ * dirZ);
        dirX /= length;
        dirY /= length;
        dirZ /= length;
        dirX += random.NextGaussian() * 0.0075F * spread;
        dirY += random.NextGaussian() * 0.0075F * spread;
        dirZ += random.NextGaussian() * 0.0075F * spread;
        dirX *= speed;
        dirY *= speed;
        dirZ *= speed;
        velocityX = dirX;
        velocityY = dirY;
        velocityZ = dirZ;
        float horizontalLength = MathHelper.Sqrt(dirX * dirX + dirZ * dirZ);
        prevYaw = yaw = (float)(Math.Atan2(dirX, dirZ) * 180.0D / (float)Math.PI);
        prevPitch = pitch = (float)(Math.Atan2(dirY, horizontalLength) * 180.0D / (float)Math.PI);
        ticksInGround = 0;
    }

    public override void setPositionAndAnglesAvoidEntities(double newX, double newY, double newZ, float newYaw, float newPitch, int interpolationSteps)
    {
        targetX = newX;
        targetY = newY;
        targetZ = newZ;
        targetYaw = newYaw;
        targetPitch = newPitch;
        positionUpdateTicks = interpolationSteps;
        velocityX = clientVelocityX;
        velocityY = clientVelocityY;
        velocityZ = clientVelocityZ;
    }

    public override void setVelocityClient(double motionX, double motionY, double motionZ)
    {
        clientVelocityX = velocityX = motionX;
        clientVelocityY = velocityY = motionY;
        clientVelocityZ = velocityZ = motionZ;
    }

    public override void tick()
    {
        base.tick();
        if (positionUpdateTicks > 0)
        {
            double interpX = x + (targetX - x) / positionUpdateTicks;
            double interpY = y + (targetY - y) / positionUpdateTicks;
            double interpZ = z + (targetZ - z) / positionUpdateTicks;

            double yawDelta;
            for (yawDelta = targetYaw - yaw; yawDelta < -180.0D; yawDelta += 360.0D)
            {
            }

            while (yawDelta >= 180.0D)
            {
                yawDelta -= 360.0D;
            }

            yaw = (float)(yaw + yawDelta / positionUpdateTicks);
            pitch = (float)(pitch + (targetPitch - pitch) / positionUpdateTicks);
            --positionUpdateTicks;
            setPosition(interpX, interpY, interpZ);
            setRotation(yaw, pitch);
        }
        else
        {
            if (!world.IsRemote)
            {
                ItemStack heldItem = angler.getHand();
                if (angler.dead || !angler.isAlive() || heldItem == null || heldItem.getItem() != Item.FishingRod || getSquaredDistance(angler) > 1024.0D)
                {
                    markDead();
                    angler.fishHook = null;
                    return;
                }

                if (bobber != null)
                {
                    if (!bobber.dead)
                    {
                        x = bobber.x;
                        y = bobber.boundingBox.MinY + bobber.height * 0.8D;
                        z = bobber.z;
                        return;
                    }

                    bobber = null;
                }
            }

            if (shake > 0)
            {
                --shake;
            }

            if (inGround)
            {
                int blockId = world.Reader.GetBlockId(xTile, yTile, zTile);
                if (blockId == inTile)
                {
                    ++ticksInGround;
                    if (ticksInGround == 1200)
                    {
                        markDead();
                    }

                    return;
                }

                inGround = false;
                velocityX *= random.NextFloat() * 0.2F;
                velocityY *= random.NextFloat() * 0.2F;
                velocityZ *= random.NextFloat() * 0.2F;
                ticksInGround = 0;
                ticksInAir = 0;
            }
            else
            {
                ++ticksInAir;
            }

            Vec3D rayStart = new(x, y, z);
            Vec3D rayEnd = new(x + velocityX, y + velocityY, z + velocityZ);
            HitResult hit = world.Reader.Raycast(rayStart, rayEnd);
            rayStart = new Vec3D(x, y, z);
            rayEnd = new Vec3D(x + velocityX, y + velocityY, z + velocityZ);
            if (hit.Type != HitResultType.MISS)
            {
                rayEnd = new Vec3D(hit.Pos.x, hit.Pos.y, hit.Pos.z);
            }

            Entity hitEntity = null;
            List<Entity> entities = world.Entities.GetEntities(this, boundingBox.Stretch(velocityX, velocityY, velocityZ).Expand(1.0D, 1.0D, 1.0D));
            double minHitDistance = 0.0D;

            double buoyancy;
            for (int i = 0; i < entities.Count; ++i)
            {
                Entity entity = entities[i];
                if (entity.isCollidable() && (entity != angler || ticksInAir >= 5))
                {
                    float expandAmount = 0.3F;
                    Box expandedBox = entity.boundingBox.Expand(expandAmount, expandAmount, expandAmount);
                    HitResult entityHit = expandedBox.Raycast(rayStart, rayEnd);
                    if (entityHit.Type != HitResultType.MISS)
                    {
                        buoyancy = rayStart.distanceTo(entityHit.Pos);
                        if (buoyancy < minHitDistance || minHitDistance == 0.0D)
                        {
                            hitEntity = entity;
                            minHitDistance = buoyancy;
                        }
                    }
                }
            }

            if (hitEntity != null)
            {
                hit = new HitResult(hitEntity);
            }

            if (hit.Type != HitResultType.MISS)
            {
                if (hit.Entity != null)
                {
                    if (hit.Entity.damage(angler, 0))
                    {
                        bobber = hit.Entity;
                    }
                }
                else
                {
                    inGround = true;
                }
            }

            if (!inGround)
            {
                base.move(velocityX, velocityY, velocityZ);
                float horizontalSpeed = MathHelper.Sqrt(velocityX * velocityX + velocityZ * velocityZ);
                yaw = (float)(Math.Atan2(velocityX, velocityZ) * 180.0D / (float)Math.PI);

                for (pitch = (float)(Math.Atan2(velocityY, horizontalSpeed) * 180.0D / (float)Math.PI); pitch - prevPitch < -180.0F; prevPitch -= 360.0F)
                {
                }

                while (pitch - prevPitch >= 180.0F)
                {
                    prevPitch += 360.0F;
                }

                while (yaw - prevYaw < -180.0F)
                {
                    prevYaw -= 360.0F;
                }

                while (yaw - prevYaw >= 180.0F)
                {
                    prevYaw += 360.0F;
                }

                pitch = prevPitch + (pitch - prevPitch) * 0.2F;
                yaw = prevYaw + (yaw - prevYaw) * 0.2F;
                float drag = 0.92F;
                if (onGround || horizontalCollison)
                {
                    drag = 0.5F;
                }

                byte waterCheckSegments = 5;
                double waterSubmersion = 0.0D;

                for (int segment = 0; segment < waterCheckSegments; ++segment)
                {
                    double segmentBottom = boundingBox.MinY + (boundingBox.MaxY - boundingBox.MinY) * (segment + 0) / waterCheckSegments - 0.125D + 0.125D;
                    double segmentTop = boundingBox.MinY + (boundingBox.MaxY - boundingBox.MinY) * (segment + 1) / waterCheckSegments - 0.125D + 0.125D;
                    Box segmentBox = new(boundingBox.MinX, segmentBottom, boundingBox.MinZ, boundingBox.MaxX, segmentTop, boundingBox.MaxZ);
                    if (world.Reader.IsMaterialInBox(segmentBox, m => m == Material.Water))
                    {
                        waterSubmersion += 1.0D / waterCheckSegments;
                    }
                }

                if (waterSubmersion > 0.0D)
                {
                    if (ticksCatchable > 0)
                    {
                        --ticksCatchable;
                    }
                    else
                    {
                        short catchDelay = 500;
                        if (world.Environment.IsRainingAt(MathHelper.Floor(x), MathHelper.Floor(y) + 1, MathHelper.Floor(z)))
                        {
                            catchDelay = 300;
                        }

                        if (random.NextInt(catchDelay) == 0)
                        {
                            ticksCatchable = random.NextInt(30) + 10;
                            velocityY -= 0.2F;
                            world.Broadcaster.PlaySoundAtEntity(this, "random.splash", 0.25F, 1.0F + (random.NextFloat() - random.NextFloat()) * 0.4F);
                            float waterSurface = MathHelper.Floor(boundingBox.MinY);

                            int particle;
                            float offsetX;
                            float offsetZ;
                            for (particle = 0; particle < 1.0F + width * 20.0F; ++particle)
                            {
                                offsetX = (random.NextFloat() * 2.0F - 1.0F) * width;
                                offsetZ = (random.NextFloat() * 2.0F - 1.0F) * width;
                                world.Broadcaster.AddParticle("bubble", x + offsetX, waterSurface + 1.0F, z + offsetZ, velocityX, velocityY - random.NextFloat() * 0.2F, velocityZ);
                            }

                            for (particle = 0; particle < 1.0F + width * 20.0F; ++particle)
                            {
                                offsetX = (random.NextFloat() * 2.0F - 1.0F) * width;
                                offsetZ = (random.NextFloat() * 2.0F - 1.0F) * width;
                                world.Broadcaster.AddParticle("splash", x + offsetX, waterSurface + 1.0F, z + offsetZ, velocityX, velocityY, velocityZ);
                            }
                        }
                    }
                }

                if (ticksCatchable > 0)
                {
                    velocityY -= random.NextFloat() * random.NextFloat() * random.NextFloat() * 0.2D;
                }

                buoyancy = waterSubmersion * 2.0D - 1.0D;
                velocityY += 0.04F * buoyancy;
                if (waterSubmersion > 0.0D)
                {
                    drag = (float)(drag * 0.9D);
                    velocityY *= 0.8D;
                }

                velocityX *= drag;
                velocityY *= drag;
                velocityZ *= drag;
                setPosition(x, y, z);
            }
        }
    }

    public override void writeNbt(NBTTagCompound nbt)
    {
        nbt.SetShort("xTile", (short)xTile);
        nbt.SetShort("yTile", (short)yTile);
        nbt.SetShort("zTile", (short)zTile);
        nbt.SetByte("inTile", (sbyte)inTile);
        nbt.SetByte("shake", (sbyte)shake);
        nbt.SetByte("inGround", (sbyte)(inGround ? 1 : 0));
    }

    public override void readNbt(NBTTagCompound nbt)
    {
        xTile = nbt.GetShort("xTile");
        yTile = nbt.GetShort("yTile");
        zTile = nbt.GetShort("zTile");
        inTile = nbt.GetByte("inTile") & 255;
        shake = nbt.GetByte("shake") & 255;
        inGround = nbt.GetByte("inGround") == 1;
    }

    public override float getShadowRadius() => 0.0F;

    public int catchFish()
    {
        byte result = 0;
        if (bobber != null)
        {
            double deltaX = angler.x - x;
            double deltaY = angler.y - y;
            double deltaZ = angler.z - z;
            double distance = MathHelper.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            double pullStrength = 0.1D;
            bobber.velocityX += deltaX * pullStrength;
            bobber.velocityY += deltaY * pullStrength + MathHelper.Sqrt(distance) * 0.08D;
            bobber.velocityZ += deltaZ * pullStrength;
            result = 3;
        }
        else if (ticksCatchable > 0)
        {
            EntityItem fishItem = new(world, x, y, z, new ItemStack(Item.RawFish));
            double deltaX = angler.x - x;
            double deltaY = angler.y - y;
            double deltaZ = angler.z - z;
            double distance = MathHelper.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            double pullStrength = 0.1D;
            fishItem.velocityX = deltaX * pullStrength;
            fishItem.velocityY = deltaY * pullStrength + MathHelper.Sqrt(distance) * 0.08D;
            fishItem.velocityZ = deltaZ * pullStrength;
            world.SpawnEntity(fishItem);
            angler.increaseStat(Stats.Stats.FishCaughtStat, 1);
            result = 1;
        }

        if (inGround)
        {
            result = 2;
        }

        markDead();
        angler.fishHook = null;
        return result;
    }
}
