using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Client.Rendering.Legacy;
using BetaSharp.Entities;

namespace BetaSharp.Client.Rendering.Entities;

public class ProjectileEntityRenderer : EntityRenderer
{
    private readonly int itemIconIndex;

    public ProjectileEntityRenderer(int itemIconIndex)
    {
        this.itemIconIndex = itemIconIndex;
    }

    public override void Render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        Scene.PushMatrix();
        Scene.Translate((float)x, (float)y, (float)z);
        Scene.Enable(SceneRenderCapability.RescaleNormal);
        Scene.Scale(0.5F, 0.5F, 0.5F);
        loadTexture("/gui/items.png");
        Tessellator tessellator = Tessellator.instance;
        int tileSize = Dispatcher.TextureManager?.GetAtlasTileSize("/gui/items.png") ?? 16;
        float atlasSize = tileSize * 16.0F;
        int texU = (itemIconIndex & 15) * tileSize;
        int texV = (itemIconIndex >> 4) * tileSize;
        float minU = texU / atlasSize;
        float maxU = (texU + tileSize) / atlasSize;
        float minV = texV / atlasSize;
        float maxV = (texV + tileSize) / atlasSize;
        float quadWidth = 1.0F;
        float xOffset = 0.5F;
        float yOffset = 0.25F;
        Scene.Rotate(180.0F - Dispatcher.PlayerViewY, 0.0F, 1.0F, 0.0F);
        Scene.Rotate(-Dispatcher.PlayerViewX, 1.0F, 0.0F, 0.0F);
        tessellator.startDrawingQuads();
        tessellator.setNormal(0.0F, 1.0F, 0.0F);
        tessellator.addVertexWithUV((double)(0.0F - xOffset), (double)(0.0F - yOffset), 0.0D, (double)minU, (double)maxV);
        tessellator.addVertexWithUV((double)(quadWidth - xOffset), (double)(0.0F - yOffset), 0.0D, (double)maxU, (double)maxV);
        tessellator.addVertexWithUV((double)(quadWidth - xOffset), (double)(1.0F - yOffset), 0.0D, (double)maxU, (double)minV);
        tessellator.addVertexWithUV((double)(0.0F - xOffset), (double)(1.0F - yOffset), 0.0D, (double)minU, (double)minV);
        tessellator.draw();
        Scene.Disable(SceneRenderCapability.RescaleNormal);
        Scene.PopMatrix();
    }
}
