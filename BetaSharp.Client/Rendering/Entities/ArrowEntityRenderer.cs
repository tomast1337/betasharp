using BetaSharp.Client.Rendering.Core;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Entities;

public class ArrowEntityRenderer : EntityRenderer
{

    public void renderArrow(EntityArrow arrowEntity, double x, double y, double z, float yaw, float tickDelta)
    {
        if (arrowEntity.PrevYaw != 0.0F || arrowEntity.PrevPitch != 0.0F)
        {
            loadTexture("/item/arrows.png");
            Scene.PushMatrix();
            Scene.Translate((float)x, (float)y, (float)z);
            Scene.Rotate(arrowEntity.PrevYaw + (arrowEntity.Yaw - arrowEntity.PrevYaw) * tickDelta - 90.0F, 0.0F, 1.0F, 0.0F);
            Scene.Rotate(arrowEntity.PrevPitch + (arrowEntity.Pitch - arrowEntity.PrevPitch) * tickDelta, 0.0F, 0.0F, 1.0F);
            Tessellator tessellator = Tessellator.instance;
            byte arrowType = 0;
            float shaftMinU = 0.0F;
            float shaftMaxU = 0.5F;
            float featherMinV = (0 + arrowType * 10) / 32.0F;
            float featherMaxV = (5 + arrowType * 10) / 32.0F;
            float sideMinU = 0.0F;
            float sideMaxU = 0.15625F;
            float sideMinV = (5 + arrowType * 10) / 32.0F;
            float sideMaxV = (10 + arrowType * 10) / 32.0F;
            float modelScale = 0.05625F;
            Scene.Enable(SceneRenderCapability.RescaleNormal);
            float shakeTime = arrowEntity.ArrowShake - tickDelta;
            if (shakeTime > 0.0F)
            {
                float shakeRotation = -MathHelper.Sin(shakeTime * 3.0F) * shakeTime;
                Scene.Rotate(shakeRotation, 0.0F, 0.0F, 1.0F);
            }

            Scene.Rotate(45.0F, 1.0F, 0.0F, 0.0F);
            Scene.Scale(modelScale, modelScale, modelScale);
            Scene.Translate(-4.0F, 0.0F, 0.0F);
            Scene.SetNormal(modelScale, 0.0F, 0.0F);
            tessellator.startDrawingQuads();
            tessellator.addVertexWithUV(-7.0D, -2.0D, -2.0D, (double)sideMinU, (double)sideMinV);
            tessellator.addVertexWithUV(-7.0D, -2.0D, 2.0D, (double)sideMaxU, (double)sideMinV);
            tessellator.addVertexWithUV(-7.0D, 2.0D, 2.0D, (double)sideMaxU, (double)sideMaxV);
            tessellator.addVertexWithUV(-7.0D, 2.0D, -2.0D, (double)sideMinU, (double)sideMaxV);
            tessellator.draw();
            Scene.SetNormal(-modelScale, 0.0F, 0.0F);
            tessellator.startDrawingQuads();
            tessellator.addVertexWithUV(-7.0D, 2.0D, -2.0D, (double)sideMinU, (double)sideMinV);
            tessellator.addVertexWithUV(-7.0D, 2.0D, 2.0D, (double)sideMaxU, (double)sideMinV);
            tessellator.addVertexWithUV(-7.0D, -2.0D, 2.0D, (double)sideMaxU, (double)sideMaxV);
            tessellator.addVertexWithUV(-7.0D, -2.0D, -2.0D, (double)sideMinU, (double)sideMaxV);
            tessellator.draw();

            for (int quadIndex = 0; quadIndex < 4; ++quadIndex)
            {
                Scene.Rotate(90.0F, 1.0F, 0.0F, 0.0F);
                Scene.SetNormal(0.0F, 0.0F, modelScale);
                tessellator.startDrawingQuads();
                tessellator.addVertexWithUV(-8.0D, -2.0D, 0.0D, (double)shaftMinU, (double)featherMinV);
                tessellator.addVertexWithUV(8.0D, -2.0D, 0.0D, (double)shaftMaxU, (double)featherMinV);
                tessellator.addVertexWithUV(8.0D, 2.0D, 0.0D, (double)shaftMaxU, (double)featherMaxV);
                tessellator.addVertexWithUV(-8.0D, 2.0D, 0.0D, (double)shaftMinU, (double)featherMaxV);
                tessellator.draw();
            }

            Scene.Disable(SceneRenderCapability.RescaleNormal);
            Scene.PopMatrix();
        }
    }

    public override void Render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        renderArrow((EntityArrow)target, x, y, z, yaw, tickDelta);
    }
}
