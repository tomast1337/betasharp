using Silk.NET.OpenGL;

namespace BetaSharp.Client.Rendering.Core.Textures;

public sealed class NoOpTextureResourceFactory : ITextureResourceFactory
{
    private int _activeTextureCount;
    public int ActiveTextureCount => _activeTextureCount;

    public ITextureResource CreateTexture(string source) => CreateTexture(source, TextureTarget.Texture2D);

    public ITextureResource CreateTexture(string source, TextureTarget target)
    {
        _activeTextureCount++;
        return new NoOpTextureResource(source, () => _activeTextureCount--);
    }
}
