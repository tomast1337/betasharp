using BetaSharp.Blocks;
using BetaSharp.Inventorys;
using BetaSharp.Items;
using java.util;

namespace BetaSharp.Recipes;

public class CraftingManager
{
    private static readonly CraftingManager instance = new CraftingManager();
    private List recipes = new ArrayList();

    public static CraftingManager getInstance()
    {
        return instance;
    }

    private CraftingManager()
    {
        new RecipesTools().addRecipes(this);
        new RecipesWeapons().addRecipes(this);
        new RecipesIngots().addRecipes(this);
        new RecipesFood().addRecipes(this);
        new RecipesCrafting().addRecipes(this);
        new RecipesArmor().addRecipes(this);
        new RecipesDyes().addRecipes(this);
        addRecipe(new ItemStack(Item.PAPER, 3), ["###", java.lang.Character.valueOf('#'), Item.SUGAR_CANE]);
        addRecipe(new ItemStack(Item.BOOK, 1), ["#", "#", "#", java.lang.Character.valueOf('#'), Item.PAPER]);
        addRecipe(new ItemStack(Block.FENCE, 2), ["###", "###", java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.JUKEBOX, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.DIAMOND]);
        addRecipe(new ItemStack(Block.NOTE_BLOCK, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Block.BOOKSHELF, 1), ["###", "XXX", "###", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.BOOK]);
        addRecipe(new ItemStack(Block.SNOW_BLOCK, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.SNOWBALL]);
        addRecipe(new ItemStack(Block.CLAY, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.CLAY]);
        addRecipe(new ItemStack(Block.BRICKS, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.BRICK]);
        addRecipe(new ItemStack(Block.GLOWSTONE, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.GLOWSTONE_DUST]);
        addRecipe(new ItemStack(Block.WOOL, 1), ["##", "##", java.lang.Character.valueOf('#'), Item.STRING]);
        addRecipe(new ItemStack(Block.TNT, 1), ["X#X", "#X#", "X#X", java.lang.Character.valueOf('X'), Item.GUNPOWDER, java.lang.Character.valueOf('#'), Block.SAND]);
        addRecipe(new ItemStack(Block.SLAB, 3, 3), ["###", java.lang.Character.valueOf('#'), Block.COBBLESTONE]);
        addRecipe(new ItemStack(Block.SLAB, 3, 0), ["###", java.lang.Character.valueOf('#'), Block.STONE]);
        addRecipe(new ItemStack(Block.SLAB, 3, 1), ["###", java.lang.Character.valueOf('#'), Block.SANDSTONE]);
        addRecipe(new ItemStack(Block.SLAB, 3, 2), ["###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Block.LADDER, 2), ["# #", "###", "# #", java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Item.WOODEN_DOOR, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Block.TRAPDOOR, 2), ["###", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Item.IRON_DOOR, 1), ["##", "##", "##", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        addRecipe(new ItemStack(Item.SIGN, 1), ["###", "###", " X ", java.lang.Character.valueOf('#'), Block.PLANKS, java.lang.Character.valueOf('X'), Item.STICK]);
        addRecipe(new ItemStack(Item.CAKE, 1), ["AAA", "BEB", "CCC", java.lang.Character.valueOf('A'), Item.MILK_BUCKET, java.lang.Character.valueOf('B'), Item.SUGAR, java.lang.Character.valueOf('C'), Item.WHEAT, java.lang.Character.valueOf('E'), Item.EGG]);
        addRecipe(new ItemStack(Item.SUGAR, 1), ["#", java.lang.Character.valueOf('#'), Item.SUGAR_CANE]);
        addRecipe(new ItemStack(Block.PLANKS, 4), ["#", java.lang.Character.valueOf('#'), Block.LOG]);
        addRecipe(new ItemStack(Item.STICK, 4), ["#", "#", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", java.lang.Character.valueOf('X'), Item.COAL, java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.TORCH, 4), ["X", "#", java.lang.Character.valueOf('X'), new ItemStack(Item.COAL, 1, 1), java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Item.BOWL, 4), ["# #", " # ", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Block.RAIL, 16), ["X X", "X#X", "X X", java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.POWERED_RAIL, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.GOLD_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('#'), Item.STICK]);
        addRecipe(new ItemStack(Block.DETECTOR_RAIL, 6), ["X X", "X#X", "XRX", java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('#'), Block.STONE_PRESSURE_PLATE]);
        addRecipe(new ItemStack(Item.MINECART, 1), ["# #", "###", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        addRecipe(new ItemStack(Block.JACK_O_LANTERN, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.PUMPKIN, java.lang.Character.valueOf('B'), Block.TORCH]);
        addRecipe(new ItemStack(Item.CHEST_MINECART, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.CHEST, java.lang.Character.valueOf('B'), Item.MINECART]);
        addRecipe(new ItemStack(Item.FURNACE_MINECART, 1), ["A", "B", java.lang.Character.valueOf('A'), Block.FURNACE, java.lang.Character.valueOf('B'), Item.MINECART]);
        addRecipe(new ItemStack(Item.BOAT, 1), ["# #", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Item.BUCKET, 1), ["# #", " # ", java.lang.Character.valueOf('#'), Item.IRON_INGOT]);
        addRecipe(new ItemStack(Item.FLINT_AND_STEEL, 1), ["A ", " B", java.lang.Character.valueOf('A'), Item.IRON_INGOT, java.lang.Character.valueOf('B'), Item.FLINT]);
        addRecipe(new ItemStack(Item.BREAD, 1), ["###", java.lang.Character.valueOf('#'), Item.WHEAT]);
        addRecipe(new ItemStack(Block.WOODEN_STAIRS, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Item.FISHING_ROD, 1), ["  #", " #X", "# X", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Item.STRING]);
        addRecipe(new ItemStack(Block.COBBLESTONE_STAIRS, 4), ["#  ", "## ", "###", java.lang.Character.valueOf('#'), Block.COBBLESTONE]);
        addRecipe(new ItemStack(Item.PAINTING, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Block.WOOL]);
        addRecipe(new ItemStack(Item.GOLDEN_APPLE, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Block.GOLD_BLOCK, java.lang.Character.valueOf('X'), Item.APPLE]);
        addRecipe(new ItemStack(Block.LEVER, 1), ["X", "#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.STICK]);
        addRecipe(new ItemStack(Block.LIT_REDSTONE_TORCH, 1), ["X", "#", java.lang.Character.valueOf('#'), Item.STICK, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Item.REPEATER, 1), ["#X#", "III", java.lang.Character.valueOf('#'), Block.LIT_REDSTONE_TORCH, java.lang.Character.valueOf('X'), Item.REDSTONE, java.lang.Character.valueOf('I'), Block.STONE]);
        addRecipe(new ItemStack(Item.CLOCK, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.GOLD_INGOT, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Item.COMPASS, 1), [" # ", "#X#", " # ", java.lang.Character.valueOf('#'), Item.IRON_INGOT, java.lang.Character.valueOf('X'), Item.REDSTONE]);
        addRecipe(new ItemStack(Item.MAP, 1), ["###", "#X#", "###", java.lang.Character.valueOf('#'), Item.PAPER, java.lang.Character.valueOf('X'), Item.COMPASS]);
        addRecipe(new ItemStack(Block.BUTTON, 1), ["#", "#", java.lang.Character.valueOf('#'), Block.STONE]);
        addRecipe(new ItemStack(Block.STONE_PRESSURE_PLATE, 1), ["##", java.lang.Character.valueOf('#'), Block.STONE]);
        addRecipe(new ItemStack(Block.WOODEN_PRESSURE_PLATE, 1), ["##", java.lang.Character.valueOf('#'), Block.PLANKS]);
        addRecipe(new ItemStack(Block.DISPENSER, 1), ["###", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.BOW, java.lang.Character.valueOf('R'), Item.REDSTONE]);
        addRecipe(new ItemStack(Block.PISTON, 1), ["TTT", "#X#", "#R#", java.lang.Character.valueOf('#'), Block.COBBLESTONE, java.lang.Character.valueOf('X'), Item.IRON_INGOT, java.lang.Character.valueOf('R'), Item.REDSTONE, java.lang.Character.valueOf('T'), Block.PLANKS]);
        addRecipe(new ItemStack(Block.STICKY_PISTON, 1), ["S", "P", java.lang.Character.valueOf('S'), Item.SLIMEBALL, java.lang.Character.valueOf('P'), Block.PISTON]);
        addRecipe(new ItemStack(Item.BED, 1), ["###", "XXX", java.lang.Character.valueOf('#'), Block.WOOL, java.lang.Character.valueOf('X'), Block.PLANKS]);
        Collections.sort(recipes, new RecipeSorter());
        java.lang.System.@out.println($"{recipes.size()} recipes");
    }

    public void addRecipe(ItemStack result, params object[] pattern)
    {
        string patternString = "";
        int index = 0;
        int width = 0;
        int height = 0;
        if (pattern[index] is string[])
        {
            string[] rows = (string[])pattern[index++];

            for (int i = 0; i < rows.Length; ++i)
            {
                string row = rows[i];
                ++height;
                width = row.Length;
                patternString = patternString + row;
            }
        }
        else
        {
            while (pattern[index] is string)
            {
                string row = (string)pattern[index++];
                ++height;
                width = row.Length;
                patternString = patternString + row;
            }
        }

        HashMap ingredient;
        for (ingredient = new HashMap(); index < pattern.Length; index += 2)
        {
            java.lang.Character key = (java.lang.Character)pattern[index];
            ItemStack value = null;
            if (pattern[index + 1] is Item)
            {
                value = new ItemStack((Item)pattern[index + 1]);
            }
            else if (pattern[index + 1] is Block)
            {
                value = new ItemStack((Block)pattern[index + 1], 1, -1);
            }
            else if (pattern[index + 1] is ItemStack)
            {
                value = (ItemStack)pattern[index + 1];
            }

            ingredient.put(key, value);
        }

        ItemStack[] ingredientGrid = new ItemStack[width * height];

        for (int i = 0; i < width * height; ++i)
        {
            char c = patternString[i];
            if (ingredient.containsKey(java.lang.Character.valueOf(c)))
            {
                ingredientGrid[i] = ((ItemStack)ingredient.get(java.lang.Character.valueOf(c))).copy();
            }
            else
            {
                ingredientGrid[i] = null;
            }
        }

        recipes.add(new ShapedRecipes(width, height, ingredientGrid, result));
    }

    public void addShapelessRecipe(ItemStack result, params object[] pattern)
    {
        ArrayList stacks = new ArrayList();
        int length = pattern.Length;

        for (int i = 0; i < length; ++i)
        {
            object var7 = pattern[i];
            if (var7 is ItemStack)
            {
                stacks.add(((ItemStack)var7).copy());
            }
            else if (var7 is Item)
            {
                stacks.add(new ItemStack((Item)var7));
            }
            else
            {
                if (!(var7 is Block))
                {
                    throw new java.lang.RuntimeException("Invalid shapeless recipy!");
                }
                stacks.add(new ItemStack((Block)var7));
            }
        }

        recipes.add(new ShapelessRecipes(result, stacks));
    }

    public ItemStack findMatchingRecipe(InventoryCrafting craftingInventory)
    {
        for (int i = 0; i < recipes.size(); ++i)
        {
            IRecipe recipe = (IRecipe)recipes.get(i);
            if (recipe.matches(craftingInventory))

                return recipe.getCraftingResult(craftingInventory);
        }

        return null;
    }

    public List getRecipeList()
    {
        return recipes;
    }
}