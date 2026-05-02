using BetaSharp.Client.Rendering;
using BetaSharp.Client.Rendering.Chunks;
using BetaSharp.Util;
using BetaSharp.Worlds.Core;

namespace BetaSharp.Client.Rendering.Backends.OpenGL;

internal sealed class OpenGlChunkRendererFactory : IChunkRendererFactory
{
    public IChunkRenderer Create(World world, Func<bool> useAlternateBlockModels, FrameContext frameContext)
    {
        ChunkMeshVersion.ClearPool();
        return new ChunkRenderer(world, useAlternateBlockModels, frameContext);
    }
}
