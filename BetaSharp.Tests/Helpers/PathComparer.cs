using BetaSharp.Entities;
using BetaSharp.Util.Maths;
using OldPathEntity = BetaSharp.PathFindingOLd.PathEntity;
using NewPathEntity = BetaSharp.PathFinding.PathEntity;

namespace BetaSharp.Tests.Helpers;

internal static class PathComparer
{
    /// <summary>
    /// Traverses the old PathEntity and returns the sequence of waypoints as Vec3D values,
    /// or null if pathEntity is null (no path found).
    /// </summary>
    public static List<Vec3D>? ExtractOld(OldPathEntity? pathEntity, Entity entity)
    {
        if (pathEntity == null)
            return null;

        var waypoints = new List<Vec3D>();
        while (!pathEntity.isFinished())
        {
            waypoints.Add(pathEntity.getPosition(entity));
            pathEntity.incrementPathIndex();
        }
        return waypoints;
    }

    /// <summary>
    /// Traverses the new PathEntity and returns the sequence of waypoints as Vec3D values,
    /// or null if pathEntity is null (no path found).
    /// </summary>
    public static List<Vec3D>? ExtractNew(NewPathEntity? pathEntity, Entity entity)
    {
        if (pathEntity == null)
            return null;

        var waypoints = new List<Vec3D>();
        while (!pathEntity.IsFinished)
        {
            waypoints.Add(pathEntity.GetPosition(entity));
            pathEntity.IncrementPathIndex();
        }
        return waypoints;
    }
}
