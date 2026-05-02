namespace BetaSharp.Client.Rendering.Core.Textures;

/// <summary>
/// Immediate texture uploader used by the current OpenGL path.
/// The interface allows swapping this with deferred/staging upload queues per backend later.
/// </summary>
internal sealed class DirectTextureUploadService : ITextureUploadService
{
    public unsafe void Upload(
        ITextureResource texture,
        int width,
        int height,
        ReadOnlySpan<byte> pixelData,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba,
        TextureStorageFormat internalFormat = TextureStorageFormat.Rgba)
    {
        Validate(texture, width, height, pixelData, format);

        fixed (byte* ptr = pixelData)
        {
            texture.Upload(width, height, ptr, level, format, internalFormat);
        }
    }

    public unsafe void UploadSubImage(
        ITextureResource texture,
        int x,
        int y,
        int width,
        int height,
        ReadOnlySpan<byte> pixelData,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba)
    {
        Validate(texture, width, height, pixelData, format);

        fixed (byte* ptr = pixelData)
        {
            texture.UploadSubImage(x, y, width, height, ptr, level, format);
        }
    }

    public unsafe void Upload3D(
        ITextureResource texture,
        int width,
        int height,
        int depth,
        ReadOnlySpan<byte> pixelData,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba,
        TextureStorageFormat internalFormat = TextureStorageFormat.Rgba8)
    {
        Validate3D(texture, width, height, depth, pixelData, format);

        fixed (byte* ptr = pixelData)
        {
            texture.Upload3D(width, height, depth, ptr, level, format, internalFormat);
        }
    }

    public unsafe void UploadSubImage3D(
        ITextureResource texture,
        int x,
        int y,
        int z,
        int width,
        int height,
        int depth,
        ReadOnlySpan<byte> pixelData,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba)
    {
        Validate3D(texture, width, height, depth, pixelData, format);

        fixed (byte* ptr = pixelData)
        {
            texture.UploadSubImage3D(x, y, z, width, height, depth, ptr, level, format);
        }
    }

    private static void Validate(ITextureResource texture, int width, int height, ReadOnlySpan<byte> pixelData,
        TextureDataFormat format)
    {
        ArgumentNullException.ThrowIfNull(texture);

        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Texture upload width must be > 0.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Texture upload height must be > 0.");
        }

        int bytesPerPixel = format switch
        {
            TextureDataFormat.Rgb => 3,
            _ => 4
        };

        int requiredBytes = checked(width * height * bytesPerPixel);
        if (pixelData.Length < requiredBytes)
        {
            throw new ArgumentException(
                $"Texture upload requires {requiredBytes} bytes, but only {pixelData.Length} were provided.",
                nameof(pixelData));
        }
    }

    private static void Validate3D(
        ITextureResource texture,
        int width,
        int height,
        int depth,
        ReadOnlySpan<byte> pixelData,
        TextureDataFormat format)
    {
        ArgumentNullException.ThrowIfNull(texture);

        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "Texture upload width must be > 0.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "Texture upload height must be > 0.");
        }

        if (depth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(depth), depth, "Texture upload depth must be > 0.");
        }

        int bytesPerPixel = format switch
        {
            TextureDataFormat.Rgb => 3,
            _ => 4
        };

        int requiredBytes = checked(width * height * depth * bytesPerPixel);
        if (pixelData.Length < requiredBytes)
        {
            throw new ArgumentException(
                $"Texture upload requires {requiredBytes} bytes, but only {pixelData.Length} were provided.",
                nameof(pixelData));
        }
    }
}
