using BetaSharp.Items;

namespace BetaSharp.Recipes;

public class SmeltingCraftingRegistry : ICraftingRegistry
{
    internal const string Name = "smelting";
    string ICraftingRegistry.Name => Name;
    public int Count => RecipesSmelting.Recipes.Count;
    public void Clear() => RecipesSmelting.Recipes.Clear();
    public void BuildRecipe(RecipeDefinition def) => RecipesSmelting.BuildSmeltRecipe(def);
}

public static class RecipesSmelting
{
    public static Dictionary<int, ItemStack> Recipes { get; } = [];

    public static void BuildSmeltRecipe(RecipeDefinition def)
    {
        if (string.IsNullOrEmpty(def.Input))
            throw new InvalidOperationException("Smelting recipe has no input.");

        if (!RecipeItemRefResolver.TryResolveItemId(def.Input, out int inputId))
            throw new InvalidOperationException($"Unknown input '{def.Input}'.");

        if (!RecipeItemRefResolver.TryResolveItemStack(def.Result.Id, def.Result.Count, 0, out ItemStack? output))
            throw new InvalidOperationException($"Unknown result '{def.Result.Id}'.");

        if (!Recipes.TryAdd(inputId, output))
            throw new OverlappingRecipeException(def.Input, SmeltingCraftingRegistry.Name);
    }

    public static ItemStack? Craft(int inputId)
    {
        Recipes.TryGetValue(inputId, out ItemStack? result);
        return result;
    }
}
