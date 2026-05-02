using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Rendering.Core.Textures;

/// <summary>
/// Splits a square legacy terrain atlas (N×N pixels, N divisible by 16) into 256 tile layers for a 2D texture array.
/// </summary>
public sealed class TextureArrayBuilder
{
    private readonly int _tileSize;
    private readonly int _layerCount;
    private readonly byte[][] _layers;

    public TextureArrayBuilder(Image<Rgba32> atlas)
    {
        if (atlas.Width != atlas.Height)
        {
            throw new ArgumentException("Terrain atlas must be square.", nameof(atlas));
        }

        if (atlas.Width % 16 != 0)
        {
            throw new ArgumentException("Terrain atlas width must be divisible by 16.", nameof(atlas));
        }

        _tileSize = atlas.Width / 16;
        _layerCount = 256;
        int bytesPerTile = _tileSize * _tileSize * 4;
        _layers = new byte[_layerCount][];

        for (int i = 0; i < _layerCount; i++)
        {
            _layers[i] = new byte[bytesPerTile];
        }

        atlas.ProcessPixelRows(accessor =>
        {
            for (int row = 0; row < 16; row++)
            {
                for (int col = 0; col < 16; col++)
                {
                    int layer = col + row * 16;
                    byte[] dst = _layers[layer];
                    int baseX = col * _tileSize;
                    int baseY = row * _tileSize;

                    for (int ty = 0; ty < _tileSize; ty++)
                    {
                        Span<Rgba32> srcRow = accessor.GetRowSpan(baseY + ty);
                        int dstOfs = ty * _tileSize * 4;
                        for (int tx = 0; tx < _tileSize; tx++)
                        {
                            Rgba32 p = srcRow[baseX + tx];
                            int o = dstOfs + tx * 4;
                            dst[o] = p.R;
                            dst[o + 1] = p.G;
                            dst[o + 2] = p.B;
                            dst[o + 3] = p.A;
                        }
                    }
                }
            }
        });
    }

    public int TileSize => _tileSize;

    public int LayerCount => _layerCount;

    /// <summary>Vanilla mapping: legacy sprite index equals layer index (0–255).</summary>
    public static int LegacyIdToLayer(int legacyId) => legacyId & 255;

    public byte[] BuildContiguousBuffer()
    {
        int layerBytes = _tileSize * _tileSize * 4;
        byte[] result = new byte[checked(_layerCount * layerBytes)];
        for (int layer = 0; layer < _layerCount; layer++)
        {
            Buffer.BlockCopy(_layers[layer], 0, result, layer * layerBytes, layerBytes);
        }

        return result;
    }
}
