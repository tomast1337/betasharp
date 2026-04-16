using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Legacy;
using BetaSharp.Entities;

namespace BetaSharp.Client.Rendering.Entities;

public class TntEntityRenderer : EntityRenderer
{
    public TntEntityRenderer()
    {
        ShadowRadius = 0.5F;
    }

    public void render(EntityTntPrimed tntEntity, double x, double y, double z, float yaw, float tickDelta)
    {
        Scene.PushMatrix();
        Scene.Translate((float)x, (float)y, (float)z);
        float flashProgress;
        if (tntEntity.Fuse - tickDelta + 1.0F < 10.0F)
        {
            flashProgress = 1.0F - (tntEntity.Fuse - tickDelta + 1.0F) / 10.0F;
            if (flashProgress < 0.0F)
            {
                flashProgress = 0.0F;
            }

            if (flashProgress > 1.0F)
            {
                flashProgress = 1.0F;
            }

            flashProgress *= flashProgress;
            flashProgress *= flashProgress;
            float scale = 1.0F + flashProgress * 0.3F;
            Scene.Scale(scale, scale, scale);
        }

        flashProgress = (1.0F - (tntEntity.Fuse - tickDelta + 1.0F) / 100.0F) * 0.8F;
        loadTexture("/terrain.png");
        BlockRenderer.RenderBlockOnInventory(Block.TNT, 0, tntEntity.GetBrightnessAtEyes(tickDelta), Tessellator.instance);
        if (tntEntity.Fuse / 5 % 2 == 0)
        {
            Scene.Disable(SceneRenderCapability.Texture2D);
            Scene.Disable(SceneRenderCapability.Lighting);
            Scene.Enable(SceneRenderCapability.Blend);
            Scene.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.DstAlpha);
            Scene.SetColor(1.0F, 1.0F, 1.0F, flashProgress);
            BlockRenderer.RenderBlockOnInventory(Block.TNT, 0, 1.0F, Tessellator.instance);
            Scene.SetColor(1.0F, 1.0F, 1.0F, 1.0F);
            Scene.Disable(SceneRenderCapability.Blend);
            Scene.Enable(SceneRenderCapability.Lighting);
            Scene.Enable(SceneRenderCapability.Texture2D);
        }

        Scene.PopMatrix();
    }

    public override void Render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        render((EntityTntPrimed)target, x, y, z, yaw, tickDelta);
    }
}
