namespace BetaSharp.Client.Rendering.Presentation;

public interface IRenderPresentation
{
    PresentationViewportImage ViewportImage { get; }
    int FramebufferWidth { get; }
    int FramebufferHeight { get; }
    bool SkipBlit { get; set; }

    void Begin();
    void End();
    void Resize(int width, int height);
}
