using System.Diagnostics.CodeAnalysis;
using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Registries;

namespace BetaSharp.Recipes;

/// <summary>
/// Resolves recipe ingredient/result strings to <see cref="ItemStack"/> / item ids:
/// legacy names via <see cref="ItemLookup"/>, then <see cref="ResourceLocation"/> against <see cref="DefaultRegistries.Blocks"/>
/// (and <see cref="BlockIds"/> for air).
/// </summary>
internal static class RecipeItemRefResolver
{
    /// <summary>
    /// Same first-colon rule as <see cref="ItemLookup"/>: if the substring after the first <c>:</c> parses as an
    /// integer, it is damage/meta and the prefix is the item/block reference; otherwise the whole string is the reference.
    /// </summary>
    private static (string BaseRef, int Damage) SplitBaseAndDamage(string input, int defaultDamage)
    {
        int colon = input.IndexOf(':');
        if (colon < 0)
        {
            return (input, defaultDamage);
        }

        string suffix = input[(colon + 1)..];
        if (int.TryParse(suffix, out int damage))
        {
            return (input[..colon], damage);
        }

        return (input, defaultDamage);
    }

    internal static bool TryResolveItemStack(
        string input,
        int itemCount,
        int defaultDamage,
        [NotNullWhen(true)] out ItemStack? stack)
    {
        ItemLookup.Initialize();

        if (ItemLookup.TryGetItem(input, out stack, itemCount, defaultDamage))
        {
            return true;
        }

        (string baseRef, int damage) = SplitBaseAndDamage(input, defaultDamage);
        if (!ResourceLocation.TryParse(baseRef, out ResourceLocation? location))
        {
            stack = null;
            return false;
        }

        IReadableRegistry<Block> blocks = DefaultRegistries.Blocks;
        if (!BlockIds.TryGetNumericId(blocks, location, out int id) || id < 0 || id >= Item.ITEMS.Length)
        {
            stack = null;
            return false;
        }

        stack = new ItemStack(id, itemCount, damage);
        return true;
    }

    internal static bool TryResolveItemId(string input, out int itemId)
    {
        ItemLookup.Initialize();

        if (ItemLookup.TryGetItemId(input, out itemId))
        {
            return true;
        }

        (string baseRef, _) = SplitBaseAndDamage(input, 0);
        if (!ResourceLocation.TryParse(baseRef, out ResourceLocation? location))
        {
            itemId = -1;
            return false;
        }

        return BlockIds.TryGetNumericId(DefaultRegistries.Blocks, location, out itemId) && itemId >= 0;
    }
}
