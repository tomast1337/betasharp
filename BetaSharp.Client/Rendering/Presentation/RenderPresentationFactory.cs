using BetaSharp.Client.Options;

namespace BetaSharp.Client.Rendering.Presentation;

public static class RenderPresentationFactory
{
    public static IRenderPresentation Create(
        RendererBackendKind backend,
        int width,
        int height,
        GameOptions options)
    {
        return backend switch
        {
            RendererBackendKind.OpenGL => new OpenGlRenderPresentation(width, height, options),
            RendererBackendKind.Vulkan => throw new NotSupportedException(
                "Vulkan presentation path is not implemented yet."),
            _ => throw new NotSupportedException(
                $"Unsupported renderer backend for presentation: {backend}")
        };
    }
}
