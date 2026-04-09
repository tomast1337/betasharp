using BetaSharp.Client.Diagnostics.GuiBackends;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Presentation;

namespace BetaSharp.Client.Rendering.Backends;

internal sealed class VulkanRenderBackendRuntime : IRenderBackendRuntime
{
    public RendererBackendKind Kind => RendererBackendKind.Vulkan;

    public IRenderPresentation CreatePresentation(int width, int height, GameOptions options)
    {
        return RenderPresentationFactory.Create(Kind, width, height, options);
    }

    public IImGuiRendererBackend CreateImGuiRendererBackend()
    {
        return ImGuiRendererBackendFactory.Create(Kind);
    }
}
