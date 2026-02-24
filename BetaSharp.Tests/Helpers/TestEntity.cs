using BetaSharp.Entities;
using BetaSharp.NBT;

namespace BetaSharp.Tests.Helpers;

/// <summary>
/// Minimal concrete Entity for use in pathfinding tests.
/// Passes null for World since PathFinder only reads bounding box, width, and height.
/// </summary>
internal class TestEntity : Entity
{
    public TestEntity() : base(null!)
    {
    }

    protected override void initDataTracker() { }

    public override void readNbt(NBTTagCompound nbt) { }

    public override void writeNbt(NBTTagCompound nbt) { }

    /// <summary>
    /// Positions the entity so that boundingBox.minX/Y/Z reflect the given coordinates.
    /// PathFinder uses boundingBox.minX, boundingBox.minY, boundingBox.minZ as the
    /// starting point for path calculation.
    /// </summary>
    public void Place(double x, double y, double z, float entityWidth = 0.6f, float entityHeight = 1.8f)
    {
        width = entityWidth;
        height = entityHeight;
        setPosition(x, y, z);
    }
}
