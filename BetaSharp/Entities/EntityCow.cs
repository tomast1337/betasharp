using BetaSharp.Items;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Entities;

public class EntityCow : EntityAnimal
{
    public EntityCow(IWorldContext world) : base(world)
    {
        texture = "/mob/cow.png";
        setBoundingBoxSpacing(0.9F, 1.3F);
    }

    public override EntityType Type => EntityRegistry.Cow;

    protected override string getLivingSound() => "mob.cow";

    protected override string getHurtSound() => "mob.cowhurt";

    protected override string getDeathSound() => "mob.cowhurt";

    protected override float getSoundVolume() => 0.4F;

    protected override int getDropItemId() => Item.Leather.id;

    public override bool interact(EntityPlayer player)
    {
        ItemStack heldBucket = player.inventory.GetItemInHand();
        if (heldBucket != null && heldBucket.ItemId == Item.Bucket.id)
        {
            player.inventory.SetStack(player.inventory.SelectedSlot, new ItemStack(Item.MilkBucket));
            return true;
        }

        return false;
    }
}
