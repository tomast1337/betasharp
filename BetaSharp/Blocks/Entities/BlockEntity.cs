using BetaSharp.NBT;
using BetaSharp.Network.Packets;
using BetaSharp.Registries;
using BetaSharp.Worlds.Core.Systems;
using Microsoft.Extensions.Logging;

namespace BetaSharp.Blocks.Entities;

/// <summary>
/// Abstract class to represent a block entity, allowing a block to store inventory or other such
/// stuff apart from the block.
/// </summary>
public abstract class BlockEntity
{
    private static readonly IRegistry<BlockEntityType> s_registry = DefaultRegistries.BlockEntityTypes;
    private static readonly ILogger<BlockEntity> s_logger = Log.Instance.For<BlockEntity>();
    public IWorldContext World;
    protected bool Removed;

    /// <summary>
    /// Type of the block entity.
    /// </summary>
    public abstract BlockEntityType Type { get; }

    /// <summary>
    /// X position of the block entity.
    /// </summary>
    public int X;

    /// <summary>
    /// Y position of the block entity.
    /// </summary>
    public int Y;

    /// <summary>
    /// Z position of the block entity.
    /// </summary>
    public int Z;

    // Block entity registered types
    // No documentation for each as they are self-explanatory and will just fill up the file.
    public static readonly BlockEntityType Furnace = Register(() => new BlockEntityFurnace(), "Furnace");
    public static readonly BlockEntityType Chest = Register(() => new BlockEntityChest(), "Chest");
    public static readonly BlockEntityType RecordPlayer = Register(() => new BlockEntityRecordPlayer(), "RecordPlayer");
    public static readonly BlockEntityType Dispenser = Register(() => new BlockEntityDispenser(), "Trap");
    public static readonly BlockEntityType Sign = Register(() => new BlockEntitySign(), "Sign");
    public static readonly BlockEntityType MobSpawner = Register(() => new BlockEntityMobSpawner(), "MobSpawner");
    public static readonly BlockEntityType Note = Register(() => new BlockEntityNote(), "Music");
    public static readonly BlockEntityType Piston = Register(() => new BlockEntityPiston(), "Piston");

    private static BlockEntityType Register<T>(Func<T> factory, string id) where T : BlockEntity
    {
        var type = new BlockEntityType(() => factory(), id);
        s_registry.Register(ResourceLocation.Parse(id.ToLower()), type);
        return type;
    }

    /// <summary>
    /// Read data for the entity from a NBT tag compound.
    /// Expected to be overridden (with base.ReadNbt(nbt), of course).
    /// </summary>
    /// <param name="nbt"><see cref="NBTTagCompound"/> containing the data to read.</param>
    public virtual void ReadNbt(NBTTagCompound nbt)
    {
        X = nbt.GetInteger("x");
        Y = nbt.GetInteger("y");
        Z = nbt.GetInteger("z");
    }

    /// <summary>
    /// Write data for the entity to a NBT tag compound.
    /// Expected to be overridden (with base.WriteNbt(nbt), of course).
    /// </summary>
    /// <param name="nbt"><see cref="NBTTagCompound"/> to write the data to.</param>

    public virtual void WriteNbt(NBTTagCompound nbt)
    {
        nbt.SetString("id", Type.Id);
        nbt.SetInteger("x", X);
        nbt.SetInteger("y", Y);
        nbt.SetInteger("z", Z);
    }

    /// <summary>
    /// Run a single tick for the block entity.
    /// Expected to be overridden if the block entity needs to do something every tick.
    /// </summary>
    /// <param name="entities"><see cref="EntityManager"/> containing the entities in the world.</param>
    public virtual void Tick(EntityManager entities)
    {
    }

    /// <summary>
    /// Create a BlockEntity from a NBT tag compound.
    /// Uses the "id" tag to determine the type of block entity to create, and then calls ReadNbt,
    /// to, of course, get the data.
    /// </summary>
    /// <param name="nbt"><see cref="NBTTagCompound"/> containing the data to read, expected to have an "id" tag.</param>
    /// <returns>A <see cref="BlockEntity"></see> representing the NBT, or null if invalid.</returns>
    public static BlockEntity? CreateFromNbt(NBTTagCompound nbt)
    {
        string id = nbt.GetString("id");
        if (string.IsNullOrEmpty(id)) return null;

        BlockEntityType? type = s_registry.Get(ResourceLocation.Parse(id.ToLower()));
        if (type == null)
        {
            s_logger.LogInformation($"{id} is missing a mapping!");
            return null;
        }

        try
        {
            BlockEntity blockEntity = type.Create();
            blockEntity.ReadNbt(nbt);
            return blockEntity;
        }
        catch (Exception exception)
        {
            s_logger.LogError(exception, $"Failed to create block entity for id {id}");
            return null;
        }
    }

    /// <summary>
    /// Gets the metadata value associated with the block at the current coordinates.
    /// </summary>
    public int PushedBlockData => World.Reader.GetBlockMeta(X, Y, Z);

    /// <summary>
    /// Mark dirty for updates.
    /// </summary>
    public void MarkDirty()
    {
        if (World == null || World.IsRemote)
        {
            return;
        }

        World.Broadcaster.UpdateBlockEntity(X, Y, Z, this);
    }

    /// <summary>
    /// Gets the (squared!) distance from the center of the block entity to the given coordinates.
    /// </summary>
    /// <param name="x">X pos to check.</param>
    /// <param name="y">Y pos to check.</param>
    /// <param name="z">Z pos to check.</param>
    /// <returns>The squared distance from the center of the block entity to the given coordinates. 
    public double DistanceFrom(double x, double y, double z)
    {
        double dx = X + 0.5D - x;
        double dy = Y + 0.5D - y;
        double dz = Z + 0.5D - z;
        return dx * dx + dy * dy + dz * dz;
    }

    /// <summary>
    /// Get the block class associated with the block at the current coordinates.
    /// </summary>
    public Block GetBlock() => Block.Blocks[World.Reader.GetBlockId(X, Y, Z)];

    public virtual Packet CreateUpdatePacket() => null;

    /// <summary>
    /// Return true if the block entity removed (either by the targeted block not existing,
    /// or Removed being true via <see cref="MarkRemoved"/>).
    /// </summary>
    /// <returns></returns>
    public bool IsRemoved()
    {
        if (Removed)
        {
            return true;
        }

        if (World != null)
        {
            int id = World.Reader.GetBlockId(X, Y, Z);
            if (id == 0 || !Block.BlocksWithEntity[id])
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Mark this block entity as being removed.
    /// </summary>
    public void MarkRemoved() => Removed = true;

    /// <summary>
    /// Cancel this block entity being removed, if it was marked as such by <see cref="MarkRemoved"/>.
    /// </summary>
    public void CancelRemoval() => Removed = false;

    static BlockEntity()
    {
    }
}
