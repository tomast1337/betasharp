using BetaSharp;
using BetaSharp.Blocks;
using BetaSharp.Registries;

namespace BetaSharp.Tests;

public sealed class BlockRegistryTests
{
    private static IReadableRegistry<Block> CreateFrozenVanillaRegistry()
    {
        var registry = new IndexedRegistry<Block>(ResourceLocation.Parse("blocks"));
        BlockRegistryBootstrap.RegisterVanillaBlocks(registry);
        registry.Freeze();
        return registry;
    }

    [Fact]
    public void VanillaRegistry_EveryNonNullBlockSlot_HasMatchingIdAndLookup()
    {
        IReadableRegistry<Block> registry = CreateFrozenVanillaRegistry();
        int registeredFromArray = 0;

        for (int i = 1; i < 256; i++)
        {
            Block? expected = Block.Blocks[i];
            if (expected is null)
            {
                continue;
            }

            registeredFromArray++;
            Assert.Same(expected, registry.Get(i));
            Assert.Equal(i, registry.GetId(expected));
            ResourceLocation? key = registry.GetKey(expected);
            Assert.NotNull(key);
            Assert.Same(expected, registry.GetValue(key));
        }

        Assert.Equal(registeredFromArray, registry.Keys.Count());
    }

    [Fact]
    public void VanillaRegistry_Slot95_IsUnused()
    {
        IReadableRegistry<Block> registry = CreateFrozenVanillaRegistry();
        Assert.Null(Block.Blocks[95]);
        Assert.Null(registry.Get(95));
    }

    [Fact]
    public void BlockIds_Air_IsNotInRegistryButResolves()
    {
        IReadableRegistry<Block> registry = CreateFrozenVanillaRegistry();
        Assert.Null(registry.Get(0));

        Assert.True(BlockIds.TryGetNumericId(registry, BlockIds.AirKey, out int airId));
        Assert.Equal(0, airId);

        Assert.True(BlockIds.TryGetKey(registry, 0, out ResourceLocation? key));
        Assert.Same(BlockIds.AirKey, key);
    }

    [Fact]
    public void VanillaRegistry_KeysAreUnique()
    {
        IReadableRegistry<Block> registry = CreateFrozenVanillaRegistry();
        HashSet<ResourceLocation> seen = [];
        foreach (ResourceLocation k in registry.Keys)
        {
            Assert.True(seen.Add(k));
        }
    }
}
