using BetaSharp.Client.Rendering;

namespace BetaSharp.Client.Diagnostics.GuiBackends;

internal static class ImGuiRendererBackendFactory
{
    public static IImGuiRendererBackend Create(RendererBackendKind backend)
    {
        return backend switch
        {
            RendererBackendKind.OpenGL => new OpenGlImGuiRendererBackend(),
            RendererBackendKind.Vulkan => throw new NotSupportedException(
                "Vulkan ImGui backend is not implemented yet."),
            _ => throw new NotSupportedException($"Unsupported ImGui backend: {backend}")
        };
    }
}
