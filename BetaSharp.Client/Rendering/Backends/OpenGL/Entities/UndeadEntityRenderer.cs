using BetaSharp.Blocks;
using BetaSharp.Client.Rendering.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities.Models;
using BetaSharp.Client.Rendering.Legacy;
using BetaSharp.Entities;
using BetaSharp.Items;

namespace BetaSharp.Client.Rendering.Entities;

public class UndeadEntityRenderer : LivingEntityRenderer
{
    protected ModelBiped modelBipedMain;

    public UndeadEntityRenderer(ModelBiped mainModel, float shadowRadius) : base(mainModel, shadowRadius)
    {
        modelBipedMain = mainModel;
    }

    protected override void RenderMore(EntityLiving entity, float tickDelta)
    {
        ItemStack heldItem = entity.HeldItem;
        if (heldItem != null)
        {
            Scene.PushMatrix();
            modelBipedMain.bipedRightArm.transform(Scene, 1.0F / 16.0F);
            Scene.Translate(-(1.0F / 16.0F), 7.0F / 16.0F, 1.0F / 16.0F);
            float itemScale;
            if (heldItem.ItemId < 256 && BlockRenderer.IsSideLit(Block.Blocks[heldItem.ItemId].getRenderType()))
            {
                itemScale = 0.5F;
                Scene.Translate(0.0F, 3.0F / 16.0F, -(5.0F / 16.0F));
                itemScale *= 12.0F / 16.0F;
                Scene.Rotate(20.0F, 1.0F, 0.0F, 0.0F);
                Scene.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
                Scene.Scale(itemScale, -itemScale, itemScale);
            }
            else if (Item.ITEMS[heldItem.ItemId].isHandheld())
            {
                itemScale = 10.0F / 16.0F;
                Scene.Translate(0.0F, 3.0F / 16.0F, 0.0F);
                Scene.Scale(itemScale, -itemScale, itemScale);
                Scene.Rotate(-100.0F, 1.0F, 0.0F, 0.0F);
                Scene.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
            }
            else
            {
                itemScale = 6.0F / 16.0F;
                Scene.Translate(0.25F, 3.0F / 16.0F, -(3.0F / 16.0F));
                Scene.Scale(itemScale, itemScale, itemScale);
                Scene.Rotate(60.0F, 0.0F, 0.0F, 1.0F);
                Scene.Rotate(-90.0F, 1.0F, 0.0F, 0.0F);
                Scene.Rotate(20.0F, 0.0F, 0.0F, 1.0F);
            }

            Dispatcher.HeldItemRenderer.renderItem(entity, heldItem);
            Scene.PopMatrix();
        }
    }
}
