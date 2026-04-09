using BetaSharp.Worlds;

namespace BetaSharp.Blocks.Entities;

/// <summary>
/// Type of a block entity, containing a factory for creating instances of the block entity and the string ID used in NBT data.
/// </summary>
public class BlockEntityType(Func<BlockEntity> factory, string id)
{
    private readonly Func<BlockEntity> _factory = factory;

    /// <summary>
    /// String ID of the block entity type, used in NBT data. For example, "Furnace" for a furnace block entity.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Creates a new instance of the block entity type using the factory function provided in the constructor.
    /// </summary>
    public BlockEntity Create() => _factory();
}
