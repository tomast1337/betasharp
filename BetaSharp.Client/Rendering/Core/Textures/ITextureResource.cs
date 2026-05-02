using Silk.NET.OpenGL;

namespace BetaSharp.Client.Rendering.Core.Textures;

public interface ITextureResource : IDisposable
{
    uint Id { get; }
    string Source { get; }
    int Width { get; }
    int Height { get; }

    /// <summary>Layer count for <see cref="TextureTarget.Texture2DArray"/>; otherwise 1.</summary>
    int Depth { get; }

    TextureTarget Target { get; }

    void Bind();
    void SetFilter(TextureMinificationFilter min, TextureMagnificationFilter mag);
    void SetWrap(TextureAddressMode s, TextureAddressMode t);
    void SetMaxLevel(int level);

    /// <summary>
    /// Immutable storage for <see cref="TextureTarget.Texture2DArray"/> (GL TexStorage3D). Must be called before uploads.
    /// </summary>
    void AllocateTexture2DArrayStorage(int width, int height, int depth, TextureStorageFormat internalFormat,
        int mipLevels = 1);

    unsafe void Upload(
        int width,
        int height,
        byte* ptr,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba,
        TextureStorageFormat internalFormat = TextureStorageFormat.Rgba);

    unsafe void UploadSubImage(
        int x,
        int y,
        int width,
        int height,
        byte* ptr,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba);

    unsafe void Upload3D(
        int width,
        int height,
        int depth,
        byte* ptr,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba,
        TextureStorageFormat internalFormat = TextureStorageFormat.Rgba8);

    unsafe void UploadSubImage3D(
        int x,
        int y,
        int z,
        int width,
        int height,
        int depth,
        byte* ptr,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba);

    void SetAnisotropicFilter(float level);
}
