using BetaSharp.Client.Rendering.Core.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp.Client.Rendering.Backends.NoOp;

/// <summary>
/// Stand-in texture manager used before the render backend creates a real <see cref="TextureManager"/>.
/// </summary>
public sealed class NoOpTextureManager : ITextureManager
{
    public int ActiveTextureCount => 0;

    public void Dispose()
    {
    }

    public int[] GetColors(string path)
    {
        return new int[256];
    }

    public int GetAtlasTileSize(string path) => 16;

    public TextureHandle Load(Image<Rgba32> image) => new(null);

    public TextureHandle GetTextureId(string path) => new(null);

    public void BindTexture(TextureHandle? handle)
    {
    }

    public void Bind(int[] packedARGB, int width, int height, ITextureResource texture)
    {
    }

    public void Delete(TextureHandle handle)
    {
    }

    public void UploadSubImage(
        TextureHandle handle,
        int x,
        int y,
        int width,
        int height,
        ReadOnlySpan<byte> pixelData,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba)
    {
    }

    public void AddDynamicTexture(global::BetaSharp.Client.Rendering.Core.Textures.DynamicTexture texture)
    {
    }

    public void Reload()
    {
    }

    public void Tick()
    {
    }
}
