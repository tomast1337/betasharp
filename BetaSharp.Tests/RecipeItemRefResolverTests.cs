using System.Text.Json;
using BetaSharp.Blocks;
using BetaSharp.Items;
using BetaSharp.Recipes;

namespace BetaSharp.Tests;

[Collection("RegistryAccess")]
public sealed class RecipeItemRefResolverTests
{
    private static readonly object s_initLock = new();
    private static bool s_bootstrapped;

    public RecipeItemRefResolverTests()
    {
        lock (s_initLock)
        {
            if (!s_bootstrapped)
            {
                Bootstrap.Initialize();
                s_bootstrapped = true;
            }
        }
    }

    [Fact]
    public void TryResolveItemStack_BetasharpBlockPath_UsesBlockRegistry()
    {
        Assert.True(
            RecipeItemRefResolver.TryResolveItemStack("betasharp:jack_o_lantern", 3, 0, out ItemStack? stack));
        Assert.Equal(Block.JackLantern.id, stack!.ItemId);
        Assert.Equal(3, stack.Count);
    }

    [Fact]
    public void TryResolveItemStack_LegacyFieldName_StillResolves()
    {
        Assert.True(RecipeItemRefResolver.TryResolveItemStack("Stone", 1, 0, out ItemStack? stack));
        Assert.Equal(Block.Stone.id, stack!.ItemId);
    }

    [Fact]
    public void TryResolveItemId_BetasharpPath_MatchesBlockId()
    {
        Assert.True(RecipeItemRefResolver.TryResolveItemId("betasharp:jack_o_lantern", out int id));
        Assert.Equal(Block.JackLantern.id, id);
    }

    [Fact]
    public void ResultRef_Json_Id_AsNumber_DeserializesToString()
    {
        ResultRef r = JsonSerializer.Deserialize<ResultRef>("{\"id\":57,\"count\":2}")!;
        Assert.Equal("57", r.Id);
        Assert.Equal(2, r.Count);
    }
}
