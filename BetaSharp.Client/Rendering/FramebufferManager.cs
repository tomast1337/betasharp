using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Presentation;

namespace BetaSharp.Client.Rendering;

// Transitional compatibility wrapper during renderer migration.
public class FramebufferManager(int w, int h, GameOptions options)
    : OpenGlRenderPresentation(w, h, options);
