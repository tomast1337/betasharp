using BetaSharp.NBT;
using BetaSharp.Network.Packets;
using BetaSharp.Worlds;
using java.lang;
using java.util;

namespace BetaSharp.Blocks.Entities;

public class BlockEntity
{
    public static readonly Type Class = typeof(BlockEntity);
    private static readonly Dictionary<string, Type> IdToType = [];
    private static readonly Dictionary<Type, string> TypeToId = [];
    public World World { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public bool IsRemoved { get; protected set; }

    private static void Register(Type blockEntityType, string id)
    {
        if (TypeToId.ContainsKey(blockEntityType))
        {
            throw new ArgumentException($"Duplicate id: {id}");
        }
        else
        {
            IdToType[id] = blockEntityType;
            TypeToId[blockEntityType] = id;
        }
    }

    public virtual void ReadNbt(NBTTagCompound nbt)
    {
        X = nbt.GetInteger("x");
        Y = nbt.GetInteger("y");
        Z = nbt.GetInteger("z");
    }

    public virtual void WriteNbt(NBTTagCompound nbt)
    {
        string entityId = TypeToId[GetType()];
        if (entityId == null)
        {
            throw new RuntimeException($"{GetType()} is missing a mapping! This is a bug!");
        }
        else
        {
            nbt.SetString("id", entityId);
            nbt.SetInteger("x", X);
            nbt.SetInteger("y", Y);
            nbt.SetInteger("z", Z);
        }
    }

    public virtual void Tick() { }

    public static BlockEntity? CreateFromNbt(NBTTagCompound nbt)
    {
        BlockEntity blockEntity = null;

        try
        {
            Type blockEntityClass = IdToType[nbt.GetString("id")];
            if (blockEntityClass != null)
            {
                blockEntity = (BlockEntity)Activator.CreateInstance(blockEntityClass);
            }
        }
        catch (System.Exception ex)
        {
            Console.Error.WriteLine(ex);
        }

        if (blockEntity != null)
        {
            blockEntity.ReadNbt(nbt);
        }
        else
        {
            java.lang.System.@out.println($"Skipping TileEntity with id {nbt.GetString("id")}");
        }

        return blockEntity;
    }

    public virtual int GetPushedBlockData() => World.getBlockMeta(X, Y, Z);


    public void MarkDirty() => World?.updateBlockEntity(X, Y, Z, this);

    public double DistanceFrom(double targetX, double targetY, double targetZ)
    {
        double dx = X + 0.5D - targetX;
        double dy = Y + 0.5D - targetY;
        double dz = Z + 0.5D - targetZ;
        return dx * dx + dy * dy + dz * dz;
    }

    public Block GetBlock() => Block.BLOCKS[World.getBlockId(X, Y, Z)];

    public virtual Packet? CreateUpdatePacket() => null;
    public void MarkRemoved() => IsRemoved = true;

    public void CancelRemoval() => IsRemoved = false;

    static BlockEntity()
    {
        Register(typeof(BlockEntityFurnace), "Furnace");
        Register(typeof(BlockEntityChest), "Chest");
        Register(typeof(BlockEntityRecordPlayer), "RecordPlayer");
        Register(typeof(BlockEntityDispenser), "Trap");
        Register(typeof(BlockEntitySign), "Sign");
        Register(typeof(BlockEntityMobSpawner), "MobSpawner");
        Register(typeof(BlockEntityNote), "Music");
        Register(typeof(BlockEntityPiston), "Piston");
    }
}