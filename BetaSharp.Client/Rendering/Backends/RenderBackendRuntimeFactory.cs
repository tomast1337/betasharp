namespace BetaSharp.Client.Rendering.Backends;

internal static class RenderBackendRuntimeFactory
{
    public static IRenderBackendRuntime Create(RendererBackendKind backend)
    {
        return backend switch
        {
            RendererBackendKind.OpenGL => new OpenGlRenderBackendRuntime(),
            RendererBackendKind.Vulkan => new VulkanRenderBackendRuntime(),
            _ => throw new NotSupportedException($"Unsupported renderer backend: {backend}")
        };
    }
}
