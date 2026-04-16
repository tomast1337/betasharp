using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities.Models;
using BetaSharp.Client.Rendering.Legacy;
using BetaSharp.Entities;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Entities;

public class BoatEntityRenderer : EntityRenderer
{
    protected ModelBase modelBoat;

    public BoatEntityRenderer()
    {
        ShadowRadius = 0.5F;
        modelBoat = new ModelBoat();
    }

    public void render(EntityBoat boatEntity, double x, double y, double z, float yaw, float tickDelta)
    {
        Scene.PushMatrix();
        Scene.Translate((float)x, (float)y, (float)z);
        Scene.Rotate(180.0F - yaw, 0.0F, 1.0F, 0.0F);
        float timeSinceHit = boatEntity.BoatTimeSinceHit - tickDelta;
        float damageTaken = boatEntity.BoatCurrentDamage - tickDelta;
        if (damageTaken < 0.0F)
        {
            damageTaken = 0.0F;
        }

        if (timeSinceHit > 0.0F)
        {
            Scene.Rotate(MathHelper.Sin(timeSinceHit) * timeSinceHit * damageTaken / 10.0F * boatEntity.BoatRockDirection, 1.0F, 0.0F, 0.0F);
        }

        loadTexture("/terrain.png");
        float modelScale = 12.0F / 16.0F;
        Scene.Scale(modelScale, modelScale, modelScale);
        Scene.Scale(1.0F / modelScale, 1.0F / modelScale, 1.0F / modelScale);
        loadTexture("/item/boat.png");
        Scene.Scale(-1.0F, -1.0F, 1.0F);
        modelBoat.render(Scene, 0.0F, 0.0F, -0.1F, 0.0F, 0.0F, 1.0F / 16.0F);
        Scene.PopMatrix();
    }

    public override void Render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        render((EntityBoat)target, x, y, z, yaw, tickDelta);
    }
}
