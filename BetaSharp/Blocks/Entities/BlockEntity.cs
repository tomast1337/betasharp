using BetaSharp.NBT;
using BetaSharp.Network.Packets;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks.Entities;

public class BlockEntity
{
    public static readonly Type Type = typeof(BlockEntity);
    private static readonly Dictionary<string, Type> idToClass = new();
    private static readonly Dictionary<Type, string> classToId = new();
    public World world;
    public int x;
    public int y;
    public int z;
    protected bool removed;

    private static void create(Type blockEntityClass, string id)
    {
        if (classToId.ContainsKey(blockEntityClass))
        {
            throw new InvalidOperationException("Duplicate id: " + id);
        }
        else
        {
            idToClass.Add(id, blockEntityClass);
            classToId.Add(blockEntityClass, id);
        }
    }

    public virtual void readNbt(NBTTagCompound nbt)
    {
        x = nbt.GetInteger("x");
        y = nbt.GetInteger("y");
        z = nbt.GetInteger("z");
    }

    public virtual void writeNbt(NBTTagCompound nbt)
    {
        string entityId = classToId.TryGetValue(GetType(), out string? value) ? value : null;
        if (entityId == null)
        {
            throw new InvalidOperationException(GetType() + " is missing a mapping! This is a bug!");
        }
        else
        {
            nbt.SetString("id", entityId);
            nbt.SetInteger("x", x);
            nbt.SetInteger("y", y);
            nbt.SetInteger("z", z);
        }
    }

    public virtual void tick()
    {
    }

    public static BlockEntity createFromNbt(NBTTagCompound nbt)
    {
        BlockEntity blockEntity = null;

        try
        {
            Type blockEntityClass = idToClass.TryGetValue(nbt.GetString("id"), out Type? value) ? value : null;
            if (blockEntityClass != null)
            {
                blockEntity = (BlockEntity)Activator.CreateInstance(blockEntityClass)!;
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception);
        }

        if (blockEntity != null)
        {
            blockEntity.readNbt(nbt);
        }
        else
        {
            Log.Info("Skipping TileEntity with id " + nbt.GetString("id"));
        }

        return blockEntity;
    }

    public virtual int getPushedBlockData()
    {
        return world.getBlockMeta(x, y, z);
    }

    public void markDirty()
    {
        if (world != null)
        {
            world.updateBlockEntity(x, y, z, this);
        }

    }

    public double distanceFrom(double x, double y, double z)
    {
        double dx = this.x + 0.5D - x;
        double dy = this.y + 0.5D - y;
        double dz = this.z + 0.5D - z;
        return dx * dx + dy * dy + dz * dz;
    }

    public Block getBlock()
    {
        return Block.Blocks[world.getBlockId(x, y, z)];
    }

    public virtual Packet createUpdatePacket()
    {
        return null;
    }

    public bool isRemoved()
    {
        return removed;
    }

    public void markRemoved()
    {
        removed = true;
    }

    public void cancelRemoval()
    {
        removed = false;
    }

    static BlockEntity()
    {
        create(typeof(BlockEntityFurnace), "Furnace");
        create(typeof(BlockEntityChest), "Chest");
        create(typeof(BlockEntityRecordPlayer), "RecordPlayer");
        create(typeof(BlockEntityDispenser), "Trap");
        create(typeof(BlockEntitySign), "Sign");
        create(typeof(BlockEntityMobSpawner), "MobSpawner");
        create(typeof(BlockEntityNote), "Music");
        create(typeof(BlockEntityPiston), "Piston");
    }
}
