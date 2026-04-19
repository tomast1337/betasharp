using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntitySnowball : Entity
{
    private readonly EntityLiving thrower;
    private bool inGround;
    private int inTile;
    public int shake;
    private int ticksInAir;
    private int ticksInGround;
    private int xTile = -1;
    private int yTile = -1;
    private int zTile = -1;

    public EntitySnowball(IWorldContext world) : base(world) => setBoundingBoxSpacing(0.25F, 0.25F);

    public EntitySnowball(IWorldContext world, EntityLiving owner) : base(world)
    {
        thrower = owner;
        setBoundingBoxSpacing(0.25F, 0.25F);
        setPositionAndAnglesKeepPrevAngles(owner.x, owner.y + owner.getEyeHeight(), owner.z, owner.yaw, owner.pitch);
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

    public EntitySnowball(IWorldContext world, double x, double y, double z) : base(world)
    {
        ticksInGround = 0;
        setBoundingBoxSpacing(0.25F, 0.25F);
        setPosition(x, y, z);
        standingEyeHeight = 0.0F;
    }

    public override EntityType Type => EntityRegistry.Snowball;


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

    public override void setVelocityClient(double motionX, double motionY, double motionZ)
    {
        velocityX = motionX;
        velocityY = motionY;
        velocityZ = motionZ;
        if (prevPitch == 0.0F && prevYaw == 0.0F)
        {
            float horizontalLength = MathHelper.Sqrt(motionX * motionX + motionZ * motionZ);
            prevYaw = yaw = (float)(Math.Atan2(motionX, motionZ) * 180.0D / (float)Math.PI);
            prevPitch = pitch = (float)(Math.Atan2(motionY, horizontalLength) * 180.0D / (float)Math.PI);
        }
    }

    public override void tick()
    {
        lastTickX = x;
        lastTickY = y;
        lastTickZ = z;
        base.tick();
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

        if (!world.IsRemote)
        {
            Entity hitEntity = null;
            List<Entity> entities = world.Entities.GetEntities(this, boundingBox.Stretch(velocityX, velocityY, velocityZ).Expand(1.0D, 1.0D, 1.0D));
            double minHitDistance = 0.0D;

            for (int i = 0; i < entities.Count; ++i)
            {
                Entity entity = entities[i];
                if (entity.isCollidable() && (entity != thrower || ticksInAir >= 5))
                {
                    float expandAmount = 0.3F;
                    Box expandedBox = entity.boundingBox.Expand(expandAmount, expandAmount, expandAmount);
                    HitResult entityHit = expandedBox.Raycast(rayStart, rayEnd);
                    if (entityHit.Type != HitResultType.MISS)
                    {
                        double distance = rayStart.distanceTo(entityHit.Pos);
                        if (distance < minHitDistance || minHitDistance == 0.0D)
                        {
                            hitEntity = entity;
                            minHitDistance = distance;
                        }
                    }
                }
            }

            if (hitEntity != null)
            {
                hit = new HitResult(hitEntity);
            }
        }

        if (hit.Type != HitResultType.MISS)
        {
            if (hit.Entity != null && hit.Entity.damage(thrower, 0))
            {
            }

            for (int i = 0; i < 8; ++i)
            {
                world.Broadcaster.AddParticle("snowballpoof", x, y, z, 0.0D, 0.0D, 0.0D);
            }

            markDead();
        }

        x += velocityX;
        y += velocityY;
        z += velocityZ;
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
        float drag = 0.99F;
        float gravity = 0.03F;
        if (isInWater())
        {
            for (int i = 0; i < 4; ++i)
            {
                float trailOffset = 0.25F;
                world.Broadcaster.AddParticle("bubble", x - velocityX * trailOffset, y - velocityY * trailOffset, z - velocityZ * trailOffset, velocityX, velocityY, velocityZ);
            }

            drag = 0.8F;
        }

        velocityX *= drag;
        velocityY *= drag;
        velocityZ *= drag;
        velocityY -= gravity;
        setPosition(x, y, z);
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

    public override void onPlayerInteraction(EntityPlayer player)
    {
        if (inGround && thrower == player && shake <= 0 && player.inventory.AddItemStackToInventory(new ItemStack(Item.ARROW, 1)))
        {
            world.Broadcaster.PlaySoundAtEntity(this, "random.pop", 0.2F, ((random.NextFloat() - random.NextFloat()) * 0.7F + 1.0F) * 2.0F);
            player.sendPickup(this, 1);
            markDead();
        }
    }

    public override float getShadowRadius() => 0.0F;
}
