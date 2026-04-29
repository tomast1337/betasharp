using System.Diagnostics.CodeAnalysis;
using BetaSharp.Blocks;

namespace BetaSharp.Registries;

/// <summary>
/// Vanilla block ids are still authoritative in <see cref="Block.Blocks"/> and on the wire.
/// This bootstrap registers stable <c>betasharp:&lt;path&gt;</c> keys mapped to those ids via <see cref="IndexedRegistry{T}"/>.
/// </summary>
/// <remarks>
/// Id 0 (air) has no <see cref="Block"/> instance — use <see cref="BlockIds.AirKey"/> and
/// <see cref="BlockIds.TryGetNumericId"/> / <see cref="BlockIds.TryGetKey"/> for air.
/// Paths are lower_snake_case and disambiguate duplicates (e.g. flowing vs stationary water).
/// </remarks>
public static class BlockRegistryBootstrap
{
    private static ResourceLocation B(string path) => new(Namespace.BetaSharp, path);

    public static void RegisterVanillaBlocks(IRegistry<Block> registry)
    {
        _ = Block.Stone;

        registry.Register(Block.Stone.id, B("stone"), Block.Stone);
        registry.Register(Block.GrassBlock.id, B("grass_block"), Block.GrassBlock);
        registry.Register(Block.Dirt.id, B("dirt"), Block.Dirt);
        registry.Register(Block.Cobblestone.id, B("cobblestone"), Block.Cobblestone);
        registry.Register(Block.Planks.id, B("planks"), Block.Planks);
        registry.Register(Block.Sapling.id, B("sapling"), Block.Sapling);
        registry.Register(Block.Bedrock.id, B("bedrock"), Block.Bedrock);
        registry.Register(Block.FlowingWater.id, B("flowing_water"), Block.FlowingWater);
        registry.Register(Block.Water.id, B("water"), Block.Water);
        registry.Register(Block.FlowingLava.id, B("flowing_lava"), Block.FlowingLava);
        registry.Register(Block.Lava.id, B("lava"), Block.Lava);
        registry.Register(Block.Sand.id, B("sand"), Block.Sand);
        registry.Register(Block.Gravel.id, B("gravel"), Block.Gravel);
        registry.Register(Block.GoldOre.id, B("gold_ore"), Block.GoldOre);
        registry.Register(Block.IronOre.id, B("iron_ore"), Block.IronOre);
        registry.Register(Block.CoalOre.id, B("coal_ore"), Block.CoalOre);
        registry.Register(Block.Log.id, B("log"), Block.Log);
        registry.Register(Block.Leaves.id, B("leaves"), Block.Leaves);
        registry.Register(Block.Sponge.id, B("sponge"), Block.Sponge);
        registry.Register(Block.Glass.id, B("glass"), Block.Glass);
        registry.Register(Block.LapisOre.id, B("lapis_ore"), Block.LapisOre);
        registry.Register(Block.LapisBlock.id, B("lapis_block"), Block.LapisBlock);
        registry.Register(Block.Dispenser.id, B("dispenser"), Block.Dispenser);
        registry.Register(Block.Sandstone.id, B("sandstone"), Block.Sandstone);
        registry.Register(Block.Noteblock.id, B("noteblock"), Block.Noteblock);
        registry.Register(Block.Bed.id, B("bed"), Block.Bed);
        registry.Register(Block.PoweredRail.id, B("powered_rail"), Block.PoweredRail);
        registry.Register(Block.DetectorRail.id, B("detector_rail"), Block.DetectorRail);
        registry.Register(Block.StickyPiston.id, B("sticky_piston"), Block.StickyPiston);
        registry.Register(Block.Cobweb.id, B("cobweb"), Block.Cobweb);
        registry.Register(Block.Grass.id, B("tall_grass"), Block.Grass);
        registry.Register(Block.DeadBush.id, B("dead_bush"), Block.DeadBush);
        registry.Register(Block.Piston.id, B("piston"), Block.Piston);
        registry.Register(Block.PistonHead.id, B("piston_head"), Block.PistonHead);
        registry.Register(Block.Wool.id, B("wool"), Block.Wool);
        registry.Register(Block.MovingPiston.id, B("moving_piston"), Block.MovingPiston);
        registry.Register(Block.Dandelion.id, B("dandelion"), Block.Dandelion);
        registry.Register(Block.Rose.id, B("rose"), Block.Rose);
        registry.Register(Block.BrownMushroom.id, B("brown_mushroom"), Block.BrownMushroom);
        registry.Register(Block.RedMushroom.id, B("red_mushroom"), Block.RedMushroom);
        registry.Register(Block.GoldBlock.id, B("gold_block"), Block.GoldBlock);
        registry.Register(Block.IronBlock.id, B("iron_block"), Block.IronBlock);
        registry.Register(Block.DoubleSlab.id, B("double_slab"), Block.DoubleSlab);
        registry.Register(Block.Slab.id, B("slab"), Block.Slab);
        registry.Register(Block.Bricks.id, B("bricks"), Block.Bricks);
        registry.Register(Block.TNT.id, B("tnt"), Block.TNT);
        registry.Register(Block.Bookshelf.id, B("bookshelf"), Block.Bookshelf);
        registry.Register(Block.MossyCobblestone.id, B("mossy_cobblestone"), Block.MossyCobblestone);
        registry.Register(Block.Obsidian.id, B("obsidian"), Block.Obsidian);
        registry.Register(Block.Torch.id, B("torch"), Block.Torch);
        registry.Register(Block.Fire.id, B("fire"), Block.Fire);
        registry.Register(Block.Spawner.id, B("spawner"), Block.Spawner);
        registry.Register(Block.WoodenStairs.id, B("wooden_stairs"), Block.WoodenStairs);
        registry.Register(Block.Chest.id, B("chest"), Block.Chest);
        registry.Register(Block.RedstoneWire.id, B("redstone_wire"), Block.RedstoneWire);
        registry.Register(Block.DiamondOre.id, B("diamond_ore"), Block.DiamondOre);
        registry.Register(Block.DiamondBlock.id, B("diamond_block"), Block.DiamondBlock);
        registry.Register(Block.CraftingTable.id, B("crafting_table"), Block.CraftingTable);
        registry.Register(Block.Wheat.id, B("wheat"), Block.Wheat);
        registry.Register(Block.Farmland.id, B("farmland"), Block.Farmland);
        registry.Register(Block.Furnace.id, B("furnace"), Block.Furnace);
        registry.Register(Block.LitFurnace.id, B("lit_furnace"), Block.LitFurnace);
        registry.Register(Block.Sign.id, B("sign"), Block.Sign);
        registry.Register(Block.Door.id, B("oak_door"), Block.Door);
        registry.Register(Block.Ladder.id, B("ladder"), Block.Ladder);
        registry.Register(Block.Rail.id, B("rail"), Block.Rail);
        registry.Register(Block.CobblestoneStairs.id, B("cobblestone_stairs"), Block.CobblestoneStairs);
        registry.Register(Block.WallSign.id, B("wall_sign"), Block.WallSign);
        registry.Register(Block.Lever.id, B("lever"), Block.Lever);
        registry.Register(Block.StonePressurePlate.id, B("stone_pressure_plate"), Block.StonePressurePlate);
        registry.Register(Block.IronDoor.id, B("iron_door"), Block.IronDoor);
        registry.Register(Block.WoodenPressurePlate.id, B("wooden_pressure_plate"), Block.WoodenPressurePlate);
        registry.Register(Block.RedstoneOre.id, B("redstone_ore"), Block.RedstoneOre);
        registry.Register(Block.LitRedstoneOre.id, B("lit_redstone_ore"), Block.LitRedstoneOre);
        registry.Register(Block.RedstoneTorch.id, B("redstone_torch"), Block.RedstoneTorch);
        registry.Register(Block.LitRedstoneTorch.id, B("lit_redstone_torch"), Block.LitRedstoneTorch);
        registry.Register(Block.Button.id, B("button"), Block.Button);
        registry.Register(Block.Snow.id, B("snow"), Block.Snow);
        registry.Register(Block.Ice.id, B("ice"), Block.Ice);
        registry.Register(Block.SnowBlock.id, B("snow_block"), Block.SnowBlock);
        registry.Register(Block.Cactus.id, B("cactus"), Block.Cactus);
        registry.Register(Block.Clay.id, B("clay"), Block.Clay);
        registry.Register(Block.SugarCane.id, B("sugar_cane"), Block.SugarCane);
        registry.Register(Block.Jukebox.id, B("jukebox"), Block.Jukebox);
        registry.Register(Block.Fence.id, B("fence"), Block.Fence);
        registry.Register(Block.Pumpkin.id, B("pumpkin"), Block.Pumpkin);
        registry.Register(Block.Netherrack.id, B("netherrack"), Block.Netherrack);
        registry.Register(Block.Soulsand.id, B("soul_sand"), Block.Soulsand);
        registry.Register(Block.Glowstone.id, B("glowstone"), Block.Glowstone);
        registry.Register(Block.NetherPortal.id, B("nether_portal"), Block.NetherPortal);
        registry.Register(Block.JackLantern.id, B("jack_o_lantern"), Block.JackLantern);
        registry.Register(Block.Cake.id, B("cake"), Block.Cake);
        registry.Register(Block.Repeater.id, B("repeater"), Block.Repeater);
        registry.Register(Block.PoweredRepeater.id, B("powered_repeater"), Block.PoweredRepeater);
        registry.Register(Block.Trapdoor.id, B("trapdoor"), Block.Trapdoor);
    }
}

/// <summary>
/// Helpers for numeric id 0 (air), which is not stored in <see cref="IReadableRegistry{Block}"/>.
/// </summary>
public static class BlockIds
{
    public const string AirPath = "air";

    public static readonly ResourceLocation AirKey = new(Namespace.BetaSharp, AirPath);

    public static bool IsAir(int blockId) => blockId == 0;

    /// <summary>
    /// Resolves a registry key to the wire/save block id, including <see cref="AirKey"/> for 0.
    /// </summary>
    public static bool TryGetNumericId(IReadableRegistry<Block> registry, ResourceLocation key, out int id)
    {
        if (key.Namespace == Namespace.BetaSharp && key.Path == AirPath)
        {
            id = 0;
            return true;
        }

        if (registry.TryGet(key, out Block? block))
        {
            id = block.id;
            return true;
        }

        id = -1;
        return false;
    }

    /// <summary>
    /// Returns the stable key for a vanilla block id, or <see cref="AirKey"/> for 0.
    /// </summary>
    public static bool TryGetKey(IReadableRegistry<Block> registry, int blockId, [NotNullWhen(true)] out ResourceLocation? key)
    {
        if (blockId == 0)
        {
            key = AirKey;
            return true;
        }

        Block? block = registry.Get(blockId);
        if (block is null)
        {
            key = null;
            return false;
        }

        key = registry.GetKey(block);
        return key is not null;
    }
}
