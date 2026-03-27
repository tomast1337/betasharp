namespace BetaSharp.Blocks;

/// <summary>
/// Declares which faces of a block may receive random per-block UV rotation
/// when the "Alternate Blocks" option is enabled. Used by the client renderer;
/// the base-game project only carries the declaration.
/// </summary>
[Flags]
public enum FaceVarianceFlags : byte
{
    /// <summary>No faces are rotated (default for all blocks).</summary>
    None      = 0,

    Top       = 1 << 0,
    Bottom    = 1 << 1,
    Sides     = 1 << 2,

    /// <summary>Top and bottom only — keeps side-face seams clean.</summary>
    TopBottom = Top | Bottom,

    /// <summary>All six faces receive rotation.</summary>
    All       = Top | Bottom | Sides,
}
