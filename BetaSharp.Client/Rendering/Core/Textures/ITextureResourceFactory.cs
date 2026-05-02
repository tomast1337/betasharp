using Silk.NET.OpenGL;

namespace BetaSharp.Client.Rendering.Core.Textures;

public interface ITextureResourceFactory
{
    int ActiveTextureCount { get; }
    ITextureResource CreateTexture(string source);

    ITextureResource CreateTexture(string source, TextureTarget target);
}
