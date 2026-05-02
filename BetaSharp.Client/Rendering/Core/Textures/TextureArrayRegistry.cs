namespace BetaSharp.Client.Rendering.Core.Textures;

/// <summary>
/// Maps string aliases to GPU <see cref="TextureTarget.Texture2DArray"/> slice indices. Slots 0–255 are reserved for
/// the vanilla 16×16 terrain sprite grid; custom names allocate from 256 upward.
/// </summary>
public sealed class TextureArrayRegistry
{
    public const int LegacySlotCount = 256;

    /// <summary>Total layers allocated for the terrain <c>TEXTURE_2D_ARRAY</c> (includes reserved grid + growth pool).</summary>
    public const int MaxTerrainLayers = 1024;

    public const int FirstAliasLayer = 256;

    private readonly Dictionary<string, int> _aliasToLayer = new(StringComparer.Ordinal);
    private int _nextFreeLayer = FirstAliasLayer;

    public TextureArrayRegistry()
    {
        for (int i = 0; i < LegacySlotCount; i++)
        {
            _aliasToLayer["terrain_" + i] = i;
        }
    }

    /// <summary>Vanilla terrain sprite index to GPU layer (0–255).</summary>
    public static int GetLayer(int legacyId) => legacyId & 255;

    /// <summary>Resolves or allocates a layer for a mod / named texture alias.</summary>
    public int GetOrAdd(string alias)
    {
        ArgumentException.ThrowIfNullOrEmpty(alias);

        if (_aliasToLayer.TryGetValue(alias, out int existing))
        {
            return existing;
        }

        if (_nextFreeLayer >= MaxTerrainLayers)
        {
            throw new InvalidOperationException(
                $"Texture array layer pool exhausted (maximum {MaxTerrainLayers} layers).");
        }

        int layer = _nextFreeLayer++;
        _aliasToLayer[alias] = layer;
        return layer;
    }

    public bool TryGetAlias(string alias, out int layer) => _aliasToLayer.TryGetValue(alias, out layer);
}
