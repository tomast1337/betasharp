using BetaSharp.Client.Diagnostics.GuiBackends;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Presentation;

namespace BetaSharp.Client.Rendering.Backends;

internal interface IRenderBackendRuntime
{
    RendererBackendKind Kind { get; }

    IRenderPresentation CreatePresentation(int width, int height, GameOptions options);
    IImGuiRendererBackend CreateImGuiRendererBackend();
}
