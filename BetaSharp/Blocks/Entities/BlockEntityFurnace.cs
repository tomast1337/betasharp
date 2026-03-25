using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using BetaSharp.NBT;
using BetaSharp.Recipes;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityFurnace : BlockEntity, IInventory
{
    public int BurnTime;
    public int CookTime;
    public int FuelTime;
    private ItemStack?[] _itemStacks = new ItemStack[3];
    public override BlockEntityType Type => Furnace;

    public int size() => _itemStacks.Length;

    public ItemStack? getStack(int slot) => _itemStacks[slot];

    public ItemStack? removeStack(int slot, int stack)
    {
        if (_itemStacks[slot] != null)
        {
            ItemStack? removedStack;
            if (_itemStacks[slot]!.count <= stack)
            {
                removedStack = _itemStacks[slot];
                _itemStacks[slot] = null;
                return removedStack;
            }

            removedStack = _itemStacks[slot]!.split(stack);
            if (_itemStacks[slot]!.count == 0)
            {
                _itemStacks[slot] = null;
            }

            return removedStack;
        }

        return null;
    }

    public void setStack(int slot, ItemStack? stack)
    {
        _itemStacks[slot] = stack;
        if (stack != null && stack.count > getMaxCountPerStack())
        {
            stack.count = getMaxCountPerStack();
        }
    }

    public string getName() => "Furnace";

    public int getMaxCountPerStack() => 64;

    public bool canPlayerUse(EntityPlayer player) => World.Entities.GetBlockEntity<BlockEntityFurnace>(X, Y, Z) == this && player.getSquaredDistance(X + 0.5D, Y + 0.5D, Z + 0.5D) <= 64.0D;

    public override void ReadNbt(NBTTagCompound nbt)
    {
        base.ReadNbt(nbt);
        NBTTagList itemList = nbt.GetTagList("Items");
        _itemStacks = new ItemStack[size()];

        for (int itemIndex = 0; itemIndex < itemList.TagCount(); ++itemIndex)
        {
            NBTTagCompound itemTag = (NBTTagCompound)itemList.TagAt(itemIndex);
            sbyte slot = itemTag.GetByte("Slot");
            if (slot >= 0 && slot < _itemStacks.Length)
            {
                _itemStacks[slot] = new ItemStack(itemTag);
            }
        }

        BurnTime = nbt.GetShort("BurnTime");
        CookTime = nbt.GetShort("CookTime");
        FuelTime = GetFuelTime(_itemStacks[1]);
    }

    public override void WriteNbt(NBTTagCompound nbt)
    {
        base.WriteNbt(nbt);
        nbt.SetShort("BurnTime", (short)BurnTime);
        nbt.SetShort("CookTime", (short)CookTime);
        NBTTagList itemList = new();

        for (int slotIndex = 0; slotIndex < _itemStacks.Length; ++slotIndex)
        {
            if (_itemStacks[slotIndex] == null)
            {
                continue;
            }

            NBTTagCompound slotTag = new();
            slotTag.SetByte("Slot", (sbyte)slotIndex);
            _itemStacks[slotIndex]!.writeToNBT(slotTag);
            itemList.SetTag(slotTag);
        }

        nbt.SetTag("Items", itemList);
    }

    public int getCookTimeDelta(int multiplier) => CookTime * multiplier / 200;

    public int getFuelTimeDelta(int multiplier)
    {
        if (FuelTime == 0)
        {
            FuelTime = 200;
        }

        return BurnTime * multiplier / FuelTime;
    }

    public bool isBurning() => BurnTime > 0;

    public override void Tick(EntityManager entities)
    {
        bool wasBurning = BurnTime > 0;
        bool stateChanged = false;
        if (BurnTime > 0)
        {
            --BurnTime;
        }

        if (!World.IsRemote)
        {
            if (BurnTime == 0 && canAcceptRecipeOutput())
            {
                FuelTime = BurnTime = GetFuelTime(_itemStacks[1]);
                if (BurnTime > 0)
                {
                    stateChanged = true;
                    if (_itemStacks[1] != null)
                    {
                        --_itemStacks[1]!.count;
                        if (_itemStacks[1]!.count == 0)
                        {
                            _itemStacks[1] = null;
                        }
                    }
                }
            }

            if (isBurning() && canAcceptRecipeOutput())
            {
                ++CookTime;
                if (CookTime == 200)
                {
                    CookTime = 0;
                    craftRecipe();
                    stateChanged = true;
                }
            }
            else
            {
                CookTime = 0;
            }

            if (wasBurning != BurnTime > 0)
            {
                stateChanged = true;
                BlockFurnace.UpdateLitState(BurnTime > 0, World, X, Y, Z);
            }
        }

        if (stateChanged)
        {
            markDirty();
        }
    }

    private bool canAcceptRecipeOutput()
    {
        if (_itemStacks[0] == null)
        {
            return false;
        }

        ItemStack? outputStack = SmeltingRecipeManager.getInstance().Craft(_itemStacks[0]!.getItem()!.id);
        return outputStack != null && (_itemStacks[2] == null || (_itemStacks[2]!.isItemEqual(outputStack) &&
                                                                  ((_itemStacks[2]!.count < getMaxCountPerStack() &&
                                                                    _itemStacks[2]!.count < _itemStacks[2]!.getMaxCount()) || _itemStacks[2]!.count < outputStack.getMaxCount())));
    }

    public void craftRecipe()
    {
        if (!canAcceptRecipeOutput())
        {
            return;
        }

        ItemStack outputStack = SmeltingRecipeManager.getInstance().Craft(_itemStacks[0].getItem().id);
        if (_itemStacks[2] == null)
        {
            _itemStacks[2] = outputStack.copy();
        }
        else if (_itemStacks[2].itemId == outputStack.itemId)
        {
            ++_itemStacks[2].count;
        }

        --_itemStacks[0].count;
        if (_itemStacks[0].count <= 0)
        {
            _itemStacks[0] = null;
        }
    }

    private static int GetFuelTime(ItemStack? itemStack)
    {
        if (itemStack == null)
        {
            return 0;
        }

        int itemId = itemStack.getItem().id;
        return itemId < 256 && Block.Blocks[itemId]!.Material == Material.Wood ? 300 : itemId == Item.Stick.id ? 100 : itemId == Item.Coal.id ? 1600 : itemId == Item.LavaBucket.id ? 20000 : itemId == Block.Sapling.Id ? 100 : 0;
    }
}
