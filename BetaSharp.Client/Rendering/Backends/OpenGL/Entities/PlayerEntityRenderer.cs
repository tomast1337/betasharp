using BetaSharp.Blocks;
using BetaSharp.Client.Entities;
using BetaSharp.Client.Guis;
using BetaSharp.Client.Rendering.Blocks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Entities.Models;
using BetaSharp.Client.Rendering.Legacy;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Util.Maths;

namespace BetaSharp.Client.Rendering.Entities;

public class PlayerEntityRenderer : LivingEntityRenderer
{
    private readonly ModelBiped _modelBipedMain;
    private readonly ModelBiped _modelArmorChestplate = new(1.0F);
    private readonly ModelBiped _modelArmor = new(0.5F);
    private static readonly string[] s_armorFilenamePrefix = ["cloth", "chain", "iron", "diamond", "gold"];

    public PlayerEntityRenderer() : base(new ModelBiped(0.0F), 0.5F)
    {
        _modelBipedMain = (ModelBiped)mainModel;
    }

    protected bool SetArmorModel(EntityPlayer player, int armorSlot, float tickDelta)
    {
        ItemStack armorStack = player.Inventory.ArmorItemBySlot(3 - armorSlot);
        if (armorStack != null)
        {
            Item armorItem = armorStack.getItem();
            if (armorItem is ItemArmor itemArmor)
            {
                loadTexture("/armor/" + s_armorFilenamePrefix[itemArmor.renderIndex] + "_" + (armorSlot == 2 ? 2 : 1) +
                            ".png");
                ModelBiped armorModel = armorSlot == 2 ? _modelArmor : _modelArmorChestplate;
                armorModel.bipedHead.visible = armorSlot == 0;
                armorModel.bipedHeadwear.visible = armorSlot == 0;
                armorModel.bipedBody.visible = armorSlot == 1 || armorSlot == 2;
                armorModel.bipedRightArm.visible = armorSlot == 1;
                armorModel.bipedLeftArm.visible = armorSlot == 1;
                armorModel.bipedRightLeg.visible = armorSlot == 2 || armorSlot == 3;
                armorModel.bipedLeftLeg.visible = armorSlot == 2 || armorSlot == 3;
                setRenderPassModel(armorModel);
                return true;
            }
        }

        return false;
    }

    public void RenderPlayer(EntityPlayer player, double x, double y, double z, float yaw, float tickDelta)
    {
        ItemStack heldItem = player.Inventory.ItemInHand;
        _modelArmorChestplate.field_1278_i = _modelArmor.field_1278_i = _modelBipedMain.field_1278_i = heldItem != null;
        _modelArmorChestplate.isSneak = _modelArmor.isSneak = _modelBipedMain.isSneak = player.IsSneaking();
        double renderY = y - player.StandingEyeHeight;
        if (player.IsSneaking() && player is not ClientPlayerEntity)
        {
            renderY -= 0.125D;
        }

        base.DoRenderLiving(player, x, renderY, z, yaw, tickDelta);
        _modelArmorChestplate.isSneak = _modelArmor.isSneak = _modelBipedMain.isSneak = false;
        _modelArmorChestplate.field_1278_i = _modelArmor.field_1278_i = _modelBipedMain.field_1278_i = false;
    }

    protected void RenderName(EntityPlayer player, double x, double y, double z)
    {
        if (Dispatcher.Options.HideGUI && player != Dispatcher.CameraEntity)
        {
            float nameplateBaseScale = 1.6F;
            float nameplateScale = (float)(1.0D / 60.0D) * nameplateBaseScale;
            float distanceToCamera = player.GetDistance(Dispatcher.CameraEntity);
            float maxNameDistance = player.IsSneaking() ? 32.0F : 64.0F;
            if (distanceToCamera < maxNameDistance)
            {
                string playerName = player.Name;
                if (!player.IsSneaking())
                {
                    if (player.IsSleeping)
                    {
                        renderLivingLabel(player, playerName, x, y - 1.5D, z, 64);
                    }
                    else
                    {
                        renderLivingLabel(player, playerName, x, y, z, 64);
                    }
                }
                else
                {
                    ITextRenderer textRenderer = TextRenderer;
                    Scene.PushMatrix();
                    Scene.Translate((float)x + 0.0F, (float)y + 2.3F, (float)z);
                    Scene.SetNormal(0.0F, 1.0F, 0.0F);
                    Scene.Rotate(-Dispatcher.PlayerViewY, 0.0F, 1.0F, 0.0F);
                    Scene.Rotate(Dispatcher.PlayerViewX, 1.0F, 0.0F, 0.0F);
                    Scene.Scale(-nameplateScale, -nameplateScale, nameplateScale);
                    Scene.Disable(SceneRenderCapability.Lighting);
                    Scene.Translate(0.0F, 0.25F / nameplateScale, 0.0F);
                    Scene.SetDepthMask(false);
                    Scene.Enable(SceneRenderCapability.Blend);
                    Scene.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.OneMinusSrcAlpha);
                    Tessellator tessellator = Tessellator.instance;
                    Scene.Disable(SceneRenderCapability.Texture2D);
                    tessellator.startDrawingQuads();
                    int halfNameWidth = textRenderer.GetStringWidth(playerName) / 2;
                    tessellator.setColorRGBA_F(0.0F, 0.0F, 0.0F, 0.25F);
                    tessellator.addVertex(-halfNameWidth - 1, -1.0D, 0.0D);
                    tessellator.addVertex(-halfNameWidth - 1, 8.0D, 0.0D);
                    tessellator.addVertex(halfNameWidth + 1, 8.0D, 0.0D);
                    tessellator.addVertex(halfNameWidth + 1, -1.0D, 0.0D);
                    tessellator.draw();
                    Scene.Enable(SceneRenderCapability.Texture2D);
                    Scene.SetDepthMask(true);
                    textRenderer.DrawString(playerName, -textRenderer.GetStringWidth(playerName) / 2, 0,
                        Color.WhiteAlpha20);
                    Scene.Enable(SceneRenderCapability.Lighting);
                    Scene.Disable(SceneRenderCapability.Blend);
                    Scene.SetColor(1.0F, 1.0F, 1.0F, 1.0F);
                    Scene.PopMatrix();
                }
            }
        }
    }

    protected void RenderSpecials(EntityPlayer player, float tickDelta)
    {
        ItemStack helmetStack = player.Inventory.ArmorItemBySlot(3);
        if (helmetStack != null && helmetStack.getItem().id < 256)
        {
            Scene.PushMatrix();
            _modelBipedMain.bipedHead.transform(Scene, 1.0F / 16.0F);
            if (BlockRenderer.IsSideLit(Block.Blocks[helmetStack.ItemId].getRenderType()))
            {
                float helmetScale = 10.0F / 16.0F;
                Scene.Translate(0.0F, -0.25F, 0.0F);
                Scene.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
                Scene.Scale(helmetScale, -helmetScale, helmetScale);
            }

            Dispatcher.HeldItemRenderer.renderItem(player, helmetStack);
            Scene.PopMatrix();
        }

        float heldItemScale;
        if (player.Name.Equals("deadmau5") && LoadDownloadableImageTexture(player.Name, null))
        {
            for (int earIndex = 0; earIndex < 2; ++earIndex)
            {
                float earYawOffset = player.PrevYaw + (player.Yaw - player.PrevYaw) * tickDelta
                    - (player.LastBodyYaw + (player.BodyYaw - player.LastBodyYaw) * tickDelta);
                float earPitch = player.PrevPitch + (player.Pitch - player.PrevPitch) * tickDelta;
                Scene.PushMatrix();
                Scene.Rotate(earYawOffset, 0.0F, 1.0F, 0.0F);
                Scene.Rotate(earPitch, 1.0F, 0.0F, 0.0F);
                Scene.Translate(6.0F / 16.0F * (earIndex * 2 - 1), 0.0F, 0.0F);
                Scene.Translate(0.0F, -(6.0F / 16.0F), 0.0F);
                Scene.Rotate(-earPitch, 1.0F, 0.0F, 0.0F);
                Scene.Rotate(-earYawOffset, 0.0F, 1.0F, 0.0F);
                float earScale = 4.0F / 3.0F;
                Scene.Scale(earScale, earScale, earScale);
                _modelBipedMain.renderEars(Scene, 1.0F / 16.0F);
                Scene.PopMatrix();
            }
        }

        if (LoadDownloadableImageTexture(player.PlayerCloakUrl, null))
        {
            Scene.PushMatrix();
            Scene.Translate(0.0F, 0.0F, 2.0F / 16.0F);
            double capeDeltaX = player.PrevCapePos.x + (player.CapePos.x - player.PrevCapePos.x) * tickDelta
                - (player.LastTickX + (player.X - player.LastTickX) * tickDelta);
            double capeDeltaY = player.PrevCapePos.y + (player.CapePos.y - player.PrevCapePos.y) * tickDelta
                - (player.LastTickY + (player.Y - player.LastTickY) * tickDelta);
            double capeDeltaZ = player.PrevCapePos.z + (player.CapePos.z - player.PrevCapePos.z) * tickDelta
                - (player.LastTickZ + (player.Z - player.LastTickZ) * tickDelta);
            float bodyYaw = player.LastBodyYaw + (player.BodyYaw - player.LastBodyYaw) * tickDelta;
            double bodyYawSin = MathHelper.Sin(bodyYaw * (float)Math.PI / 180.0F);
            double bodyYawCos = -MathHelper.Cos(bodyYaw * (float)Math.PI / 180.0F);
            float capeLift = (float)capeDeltaY * 10.0F;
            if (capeLift < -6.0F)
            {
                capeLift = -6.0F;
            }

            if (capeLift > 32.0F)
            {
                capeLift = 32.0F;
            }

            float capeForwardSwing = (float)(capeDeltaX * bodyYawSin + capeDeltaZ * bodyYawCos) * 100.0F;
            float capeSideSwing = (float)(capeDeltaX * bodyYawCos - capeDeltaZ * bodyYawSin) * 100.0F;
            if (capeForwardSwing < 0.0F)
            {
                capeForwardSwing = 0.0F;
            }

            float stepBobbing = player.PrevStepBobbingAmount + (player.StepBobbingAmount - player.PrevStepBobbingAmount) * tickDelta;
            capeLift += MathHelper.Sin((player.PrevHorizontalSpeed + (player.HorizontalSpeed - player.PrevHorizontalSpeed) * tickDelta) * 6.0F) * 32.0F * stepBobbing;
            if (player.IsSneaking())
            {
                capeLift += 25.0F;
            }

            Scene.Rotate(6.0F + capeForwardSwing / 2.0F + capeLift, 1.0F, 0.0F, 0.0F);
            Scene.Rotate(capeSideSwing / 2.0F, 0.0F, 0.0F, 1.0F);
            Scene.Rotate(-capeSideSwing / 2.0F, 0.0F, 1.0F, 0.0F);
            Scene.Rotate(180.0F, 0.0F, 1.0F, 0.0F);
            _modelBipedMain.renderCloak(Scene, 1.0F / 16.0F);
            Scene.PopMatrix();
        }

        ItemStack heldStack = player.Inventory.ItemInHand;
        if (heldStack != null)
        {
            Scene.PushMatrix();
            _modelBipedMain.bipedRightArm.transform(Scene, 1.0F / 16.0F);
            Scene.Translate(-(1.0F / 16.0F), 7.0F / 16.0F, 1.0F / 16.0F);
            if (player.FishHook != null)
            {
                heldStack = new ItemStack(Item.Stick);
            }

            if (heldStack.ItemId < 256 && BlockRenderer.IsSideLit(Block.Blocks[heldStack.ItemId].getRenderType()))
            {
                heldItemScale = 0.5F;
                Scene.Translate(0.0F, 3.0F / 16.0F, -(5.0F / 16.0F));
                heldItemScale *= 12.0F / 16.0F;
                Scene.Rotate(20.0F, 1.0F, 0.0F, 0.0F);
                Scene.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
                Scene.Scale(heldItemScale, -heldItemScale, heldItemScale);
            }
            else if (Item.ITEMS[heldStack.ItemId].isHandheld())
            {
                heldItemScale = 10.0F / 16.0F;
                if (Item.ITEMS[heldStack.ItemId].isHandheldRod())
                {
                    Scene.Rotate(180.0F, 0.0F, 0.0F, 1.0F);
                    Scene.Translate(0.0F, -(2.0F / 16.0F), 0.0F);
                }

                Scene.Translate(0.0F, 3.0F / 16.0F, 0.0F);
                Scene.Scale(heldItemScale, -heldItemScale, heldItemScale);
                Scene.Rotate(-100.0F, 1.0F, 0.0F, 0.0F);
                Scene.Rotate(45.0F, 0.0F, 1.0F, 0.0F);
            }
            else
            {
                heldItemScale = 6.0F / 16.0F;
                Scene.Translate(0.25F, 3.0F / 16.0F, -(3.0F / 16.0F));
                Scene.Scale(heldItemScale, heldItemScale, heldItemScale);
                Scene.Rotate(60.0F, 0.0F, 0.0F, 1.0F);
                Scene.Rotate(-90.0F, 1.0F, 0.0F, 0.0F);
                Scene.Rotate(20.0F, 0.0F, 0.0F, 1.0F);
            }

            Dispatcher.HeldItemRenderer.renderItem(player, heldStack);
            Scene.PopMatrix();
        }
    }

    protected void func_186_b(EntityPlayer player, float tickDelta)
    {
        float playerScale = 15.0F / 16.0F;
        Scene.Scale(playerScale, playerScale, playerScale);
    }

    public void DrawFirstPersonHand()
    {
        _modelBipedMain.onGround = 0.0F;
        _modelBipedMain.setRotationAngles(0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F / 16.0F);
        _modelBipedMain.bipedRightArm.render(Scene, 1.0F / 16.0F);
    }

    protected void func_22016_b(EntityPlayer player, double x, double y, double z)
    {
        if (player.IsAlive && player.IsSleeping)
        {
            base.Func_22012_b(player, x + player.SleepOffsetX, y + player.SleepOffsetY, z + player.SleepOffsetZ);
        }
        else
        {
            base.Func_22012_b(player, x, y, z);
        }
    }

    protected void func_22017_a(EntityPlayer player, float animationProgress, float bodyYaw, float tickDelta)
    {
        if (player is { IsAlive: true, IsSleeping: true })
        {
            Scene.Rotate(player.GetSleepingRotation(), 0.0F, 1.0F, 0.0F);
            Scene.Rotate(getDeathMaxRotation(player), 0.0F, 0.0F, 1.0F);
            Scene.Rotate(270.0F, 0.0F, 1.0F, 0.0F);
        }
        else
        {
            base.RotateCorpse(player, animationProgress, bodyYaw, tickDelta);
        }
    }

    protected override void PassSpecialRender(EntityLiving entity, double x, double y, double z)
    {
        RenderName((EntityPlayer)entity, x, y, z);
    }

    protected override void PreRenderCallback(EntityLiving entity, float tickDelta)
    {
        func_186_b((EntityPlayer)entity, tickDelta);
    }

    protected override bool ShouldRenderPass(EntityLiving entity, int renderPass, float tickDelta)
    {
        return SetArmorModel((EntityPlayer)entity, renderPass, tickDelta);
    }

    protected override void RenderMore(EntityLiving entity, float tickDelta)
    {
        RenderSpecials((EntityPlayer)entity, tickDelta);
    }

    protected override void RotateCorpse(EntityLiving entity, float animationProgress, float bodyYaw, float tickDelta)
    {
        func_22017_a((EntityPlayer)entity, animationProgress, bodyYaw, tickDelta);
    }

    protected override void Func_22012_b(EntityLiving entity, double x, double y, double z)
    {
        func_22016_b((EntityPlayer)entity, x, y, z);
    }

    public override void DoRenderLiving(EntityLiving entity, double x, double y, double z, float yaw, float tickDelta)
    {
        RenderPlayer((EntityPlayer)entity, x, y, z, yaw, tickDelta);
    }

    public override void Render(Entity target, double x, double y, double z, float yaw, float tickDelta)
    {
        RenderPlayer((EntityPlayer)target, x, y, z, yaw, tickDelta);
    }
}
