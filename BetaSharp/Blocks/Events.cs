using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

public struct OnTickEvt(IWorldContext level, int x, int y, int z, int meta, int blockId)
{
    public IWorldContext Level = level;
    public int X = x;
    public int Y = y;
    public int Z = z;
    public int Meta = meta;
    public int BlockId = blockId;
}

public struct OnPlacedEvt(IWorldContext level, EntityLiving? placer, int direction, int side, int x, int y, int z)
{
    public IWorldContext Level = level;
    public EntityLiving? Placer = placer;
    public int Direction = direction;
    public int Side = side;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct CanPlaceAtCtx(IWorldContext level, int direction, int x, int y, int z)
{
    public IWorldContext Level = level;
    public int Direction = direction;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnUseEvt(IWorldContext level, EntityPlayer player, int x, int y, int z)
{
    public IWorldContext Level = level;
    public EntityPlayer Player = player;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnBreakEvt(IWorldContext level, Entity? entity, int x, int y, int z)
{
    public IWorldContext Level = level;
    public Entity? Entity = entity;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnBlockBreakStartEvt(IWorldContext level, EntityPlayer player, int x, int y, int z)
{
    public IWorldContext Level = level;
    public EntityPlayer Player = player;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnDropEvt(IWorldContext level, int x, int y, int z, int meta, float luck = 1.0F)
{
    public IWorldContext Level = level;
    public int X = x;
    public int Y = y;
    public int Z = z;
    public int Meta = meta;
    public float Luck = luck;
}

public struct OnMetadataChangeEvt(IWorldContext level, int x, int y, int z, int meta)
{
    public IWorldContext Level = level;
    public int X = x;
    public int Y = y;
    public int Z = z;
    public int Meta = meta;
}

public struct OnEntityStepEvt(IWorldContext level, Entity entity, int x, int y, int z)
{
    public IWorldContext Level = level;
    public Entity Entity = entity;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnEntityCollisionEvt(IWorldContext level, Entity entity, int x, int y, int z)
{
    public IWorldContext Level = level;
    public Entity Entity = entity;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnApplyVelocityEvt(IWorldContext level, Entity entity, Vec3D velocity, int x, int y, int z)
{
    public IWorldContext Level = level;
    public Entity Entity = entity;
    public Vec3D Velocity = velocity;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnDestroyedByExplosionEvt(IWorldContext level, int x, int y, int z)
{
    public IWorldContext Level = level;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnAfterBreakEvt(IWorldContext level, EntityPlayer player, int meta, int x, int y, int z)
{
    public IWorldContext Level = level;
    public EntityPlayer Player = player;
    public int Meta = meta;
    public int X = x;
    public int Y = y;
    public int Z = z;
}

public struct OnBlockActionEvt(IWorldContext level, int data1, int data2, int x, int y, int z)
{
    public IWorldContext Level = level;
    public int Data1 = data1;
    public int Data2 = data2;
    public int X = x;
    public int Y = y;
    public int Z = z;
}
