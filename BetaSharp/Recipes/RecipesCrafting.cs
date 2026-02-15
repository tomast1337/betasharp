using BetaSharp.Blocks;
using BetaSharp.Items;
using java.lang;

namespace BetaSharp.Recipes;

public class RecipesCrafting
{
    public void addRecipes(CraftingManager manager)
    {
        manager.addRecipe(new ItemStack(Block.CHEST), ["###", "# #", "###", Character.valueOf('#'), Block.PLANKS]);
        manager.addRecipe(new ItemStack(Block.FURNACE), ["###", "# #", "###", Character.valueOf('#'), Block.COBBLESTONE]);
        manager.addRecipe(new ItemStack(Block.CRAFTING_TABLE), ["##", "##", Character.valueOf('#'), Block.PLANKS]);
        manager.addRecipe(new ItemStack(Block.SANDSTONE), ["##", "##", Character.valueOf('#'), Block.SAND]);
    }
}