using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities.Models;
using BetaSharp.Client.Rendering.Legacy;
using BetaSharp.Entities;

namespace BetaSharp.Client.Rendering.Entities;

public class SpiderEntityRenderer : LivingEntityRenderer
{
    public SpiderEntityRenderer() : base(new ModelSpider(), 1.0F)
    {
        setRenderPassModel(new ModelSpider());
    }

    protected float setSpiderDeathMaxRotation(EntitySpider spiderEntity)
    {
        return 180.0F;
    }

    protected bool setSpiderEyeBrightness(EntitySpider spiderEntity, int renderPass, float tickDelta)
    {
        // Note: During renaming there was a double if (same condition here)
        // check if is there any missing render pass for spider eyes entities
        if (renderPass != 0)
        {
            return false;
        }
        else
        {
            loadTexture("/mob/spider_eyes.png");
            float alpha = (1.0F - spiderEntity.GetBrightnessAtEyes(1.0F)) * 0.5F;
            Scene.Enable(SceneRenderCapability.Blend);
            Scene.Disable(SceneRenderCapability.AlphaTest);
            Scene.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.OneMinusSrcAlpha);
            Scene.SetColor(1.0F, 1.0F, 1.0F, alpha);
            return true;
        }
    }

    protected override float getDeathMaxRotation(EntityLiving entity)
    {
        return setSpiderDeathMaxRotation((EntitySpider)entity);
    }

    protected override bool ShouldRenderPass(EntityLiving entity, int renderPass, float tickDelta)
    {
        return setSpiderEyeBrightness((EntitySpider)entity, renderPass, tickDelta);
    }
}
