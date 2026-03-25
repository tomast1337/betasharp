using BetaSharp.Entities;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using BetaSharp.NBT;

namespace BetaSharp.Blocks.Entities;

internal class BlockEntityChest : BlockEntity, IInventory
{
    private ItemStack?[] _inventory = new ItemStack[36];
    public override BlockEntityType Type => Chest;

    public int size() => 27;

    public ItemStack? getStack(int stackIndex) => _inventory[stackIndex];

    public ItemStack? removeStack(int slot, int amount)
    {
        if (_inventory[slot] == null)
        {
            return null;
        }

        ItemStack? itemStack;
        if (_inventory[slot]!.count <= amount)
        {
            itemStack = _inventory[slot];
            _inventory[slot] = null;
            markDirty();
            return itemStack;
        }

        itemStack = _inventory[slot]!.split(amount);
        if (_inventory[slot]!.count == 0)
        {
            _inventory[slot] = null;
        }

        markDirty();
        return itemStack;
    }

    public void setStack(int slot, ItemStack? stack)
    {
        _inventory[slot] = stack;
        if (stack != null && stack.count > getMaxCountPerStack())
        {
            stack.count = getMaxCountPerStack();
        }

        markDirty();
    }

    public string getName() => "Chest";

    public int getMaxCountPerStack() => 64;

    public bool canPlayerUse(EntityPlayer player) => World.Entities.GetBlockEntity<BlockEntityChest>(X, Y, Z) == this && player.getSquaredDistance(X + 0.5D, Y + 0.5D, Z + 0.5D) <= 64.0D;

    public override void ReadNbt(NBTTagCompound nbt)
    {
        base.ReadNbt(nbt);
        NBTTagList itemList = nbt.GetTagList("Items");
        _inventory = new ItemStack[size()];

        for (int itemIndex = 0; itemIndex < itemList.TagCount(); ++itemIndex)
        {
            NBTTagCompound itemsTag = (NBTTagCompound)itemList.TagAt(itemIndex);
            int slot = itemsTag.GetByte("Slot") & 255;
            if (slot >= 0 && slot < _inventory.Length)
            {
                _inventory[slot] = new ItemStack(itemsTag);
            }
        }
    }

    public override void WriteNbt(NBTTagCompound nbt)
    {
        base.WriteNbt(nbt);
        NBTTagList itemList = new();

        for (int slotIndex = 0; slotIndex < _inventory.Length; ++slotIndex)
        {
            if (_inventory[slotIndex] != null)
            {
                NBTTagCompound itemsTag = new();
                itemsTag.SetByte("Slot", (sbyte)slotIndex);
                _inventory[slotIndex]!.writeToNBT(itemsTag);
                itemList.SetTag(itemsTag);
            }
        }

        nbt.SetTag("Items", itemList);
    }
}
