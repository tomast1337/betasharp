using Silk.NET.OpenGL;

namespace BetaSharp.Client.Rendering.Core.Textures;

public sealed class OpenGlTextureResourceFactory : ITextureResourceFactory
{
    public int ActiveTextureCount => GLTexture.ActiveTextureCount;

    public ITextureResource CreateTexture(string source) => CreateTexture(source, TextureTarget.Texture2D);

    public ITextureResource CreateTexture(string source, TextureTarget target) => new GLTexture(source, target);
}
