using BetaSharp.Inventorys;
using BetaSharp.Items;

namespace BetaSharp.Recipes;

public interface IRecipe
{
    bool matches(InventoryCrafting InventoryCrafting);

    ItemStack getCraftingResult(InventoryCrafting InventoryCrafting);

    int getRecipeSize();

    ItemStack getRecipeOutput();
}