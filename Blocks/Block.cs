using betareborn.Entities;
using betareborn.Items;
using betareborn.Stats;
using betareborn.Worlds;
using java.lang;
using betareborn.Blocks.Materials;
using betareborn.Util.Maths;
using betareborn.Util.Hit;
using betareborn.Blocks.Entities;

namespace betareborn.Blocks
{
    public class Block
    {
        public static readonly BlockSoundGroup SoundPowderFootstep = new("stone", 1.0F, 1.0F);
        public static readonly BlockSoundGroup SoundWoodFootstep = new("wood", 1.0F, 1.0F);
        public static readonly BlockSoundGroup SoundGravelFootstep = new("gravel", 1.0F, 1.0F);
        public static readonly BlockSoundGroup SoundGrassFootstep = new("grass", 1.0F, 1.0F);
        public static readonly BlockSoundGroup SoundStoneFootstep = new("stone", 1.0F, 1.0F);
        public static readonly BlockSoundGroup SoundMetalFootstep = new("stone", 1.0F, 1.5F);
        public static readonly BlockSoundGroup SoundGlassFootstep = new StepSoundStone("stone", 1.0F, 1.0F);
        public static readonly BlockSoundGroup SoundClothFootstep = new("cloth", 1.0F, 1.0F);
        public static readonly BlockSoundGroup SoundSandFootstep = new StepSoundSand("sand", 1.0F, 1.0F);
        public static readonly Block[] BLOCKS = new Block[256];
        public static readonly bool[] BLOCKS_RANDOM_TICK = new bool[256];
        public static readonly bool[] BLOCKS_OPAQUE = new bool[256];
        public static readonly bool[] BLOCKS_WITH_ENTITY = new bool[256];
        public static readonly int[] BLOCK_LIGHT_OPACITY = new int[256];
        public static readonly bool[] BLOCKS_ALLOW_VISION = new bool[256];
        public static readonly int[] BLOCKS_LIGHT_LUMINANCE = new int[256];
        public static readonly bool[] BLOCKS_IGNORE_META_UPDATE = new bool[256];
        public static readonly Block STONE = (new BlockStone(1, 1)).SetHardness(1.5F).SetResistance(10.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("stone");
        public static readonly BlockGrass GRASS_BLOCK = (BlockGrass)(new BlockGrass(2)).SetHardness(0.6F).SetSoundGroup(SoundGrassFootstep).SetBlockName("grass");
        public static readonly Block DIRT = (new BlockDirt(3, 2)).SetHardness(0.5F).SetSoundGroup(SoundGravelFootstep).SetBlockName("dirt");
        public static readonly Block COBBLESTONE = (new Block(4, 16, Material.STONE)).SetHardness(2.0F).SetResistance(10.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("stonebrick");
        public static readonly Block PLANKS = (new Block(5, 4, Material.WOOD)).SetHardness(2.0F).SetResistance(5.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("wood").IgnoreMetaUpdates();
        public static readonly Block SAPLING = (new BlockSapling(6, 15)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("sapling").IgnoreMetaUpdates();
        public static readonly Block BEDROCK = (new Block(7, 17, Material.STONE)).SetUnbreakable().SetResistance(6000000.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("bedrock").DisableStats();
        public static readonly Block FLOWING_WATER = (new BlockFlowing(8, Material.WATER)).SetHardness(100.0F).SetOpacity(3).SetBlockName("water").DisableStats().IgnoreMetaUpdates();
        public static readonly Block WATER = (new BlockStationary(9, Material.WATER)).SetHardness(100.0F).SetOpacity(3).SetBlockName("water").DisableStats().IgnoreMetaUpdates();
        public static readonly Block FLOWING_LAVA = (new BlockFlowing(10, Material.LAVA)).SetHardness(0.0F).SetLuminance(1.0F).SetOpacity(255).SetBlockName("lava").DisableStats().IgnoreMetaUpdates();
        public static readonly Block LAVA = (new BlockStationary(11, Material.LAVA)).SetHardness(100.0F).SetLuminance(1.0F).SetOpacity(255).SetBlockName("lava").DisableStats().IgnoreMetaUpdates();
        public static readonly Block SAND = (new BlockSand(12, 18)).SetHardness(0.5F).SetSoundGroup(SoundSandFootstep).SetBlockName("sand");
        public static readonly Block GRAVEL = (new BlockGravel(13, 19)).SetHardness(0.6F).SetSoundGroup(SoundGravelFootstep).SetBlockName("gravel");
        public static readonly Block GOLD_ORE = (new BlockOre(14, 32)).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("oreGold");
        public static readonly Block IRON_ORE = (new BlockOre(15, 33)).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("oreIron");
        public static readonly Block COAL_ORE = (new BlockOre(16, 34)).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("oreCoal");
        public static readonly Block LOG = (new BlockLog(17)).SetHardness(2.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("log").IgnoreMetaUpdates();
        public static readonly BlockLeaves LEAVES = (BlockLeaves)(new BlockLeaves(18, 52)).SetHardness(0.2F).SetOpacity(1).SetSoundGroup(SoundGrassFootstep).SetBlockName("leaves").DisableStats().IgnoreMetaUpdates();
        public static readonly Block SPONGE = (new BlockSponge(19)).SetHardness(0.6F).SetSoundGroup(SoundGrassFootstep).SetBlockName("sponge");
        public static readonly Block GLASS = (new BlockGlass(20, 49, Material.GLASS, false)).SetHardness(0.3F).SetSoundGroup(SoundGlassFootstep).SetBlockName("glass");
        public static readonly Block LAPIS_ORE = (new BlockOre(21, 160)).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("oreLapis");
        public static readonly Block LAPIS_BLOCK = (new Block(22, 144, Material.STONE)).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("blockLapis");
        public static readonly Block DISPENSER = (new BlockDispenser(23)).SetHardness(3.5F).SetSoundGroup(SoundStoneFootstep).SetBlockName("dispenser").IgnoreMetaUpdates();
        public static readonly Block SANDSTONE = (new BlockSandStone(24)).SetSoundGroup(SoundStoneFootstep).SetHardness(0.8F).SetBlockName("sandStone");
        public static readonly Block NOTE_BLOCK = (new BlockNote(25)).SetHardness(0.8F).SetBlockName("musicBlock").IgnoreMetaUpdates();
        public static readonly Block BED = (new BlockBed(26)).SetHardness(0.2F).SetBlockName("bed").DisableStats().IgnoreMetaUpdates();
        public static readonly Block POWERED_RAIL = (new BlockRail(27, 179, true)).SetHardness(0.7F).SetSoundGroup(SoundMetalFootstep).SetBlockName("goldenRail").IgnoreMetaUpdates();
        public static readonly Block DETECTOR_RAIL = (new BlockDetectorRail(28, 195)).SetHardness(0.7F).SetSoundGroup(SoundMetalFootstep).SetBlockName("detectorRail").IgnoreMetaUpdates();
        public static readonly Block STICKY_PISTON = (new BlockPistonBase(29, 106, true)).SetBlockName("pistonStickyBase").IgnoreMetaUpdates();
        public static readonly Block COBWEB = (new BlockWeb(30, 11)).SetOpacity(1).SetHardness(4.0F).SetBlockName("web");
        public static readonly BlockTallGrass GRASS = (BlockTallGrass)(new BlockTallGrass(31, 39)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("tallgrass");
        public static readonly BlockDeadBush DEAD_BUSH = (BlockDeadBush)(new BlockDeadBush(32, 55)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("deadbush");
        public static readonly Block PISTON = (new BlockPistonBase(33, 107, false)).SetBlockName("pistonBase").IgnoreMetaUpdates();
        public static readonly BlockPistonExtension PISTON_HEAD = (BlockPistonExtension)(new BlockPistonExtension(34, 107)).IgnoreMetaUpdates();
        public static readonly Block WOOL = (new BlockCloth()).SetHardness(0.8F).SetSoundGroup(SoundClothFootstep).SetBlockName("cloth").IgnoreMetaUpdates();
        public static readonly BlockPistonMoving MOVING_PISTON = new BlockPistonMoving(36);
        public static readonly BlockPlant DANDELION = (BlockPlant)(new BlockPlant(37, 13)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("flower");
        public static readonly BlockPlant ROSE = (BlockPlant)(new BlockPlant(38, 12)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("rose");
        public static readonly BlockPlant BROWN_MUSHROOM = (BlockPlant)(new BlockMushroom(39, 29)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetLuminance(2.0F / 16.0F).SetBlockName("mushroom");
        public static readonly BlockPlant RED_MUSHROOM = (BlockPlant)(new BlockMushroom(40, 28)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("mushroom");
        public static readonly Block GOLD_BLOCK = (new BlockOreStorage(41, 23)).SetHardness(3.0F).SetResistance(10.0F).SetSoundGroup(SoundMetalFootstep).SetBlockName("blockGold");
        public static readonly Block IRON_BLOCK = (new BlockOreStorage(42, 22)).SetHardness(5.0F).SetResistance(10.0F).SetSoundGroup(SoundMetalFootstep).SetBlockName("blockIron");
        public static readonly Block DOUBLE_SLAB = (new BlockSlab(43, true)).SetHardness(2.0F).SetResistance(10.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("stoneSlab");
        public static readonly Block SLAB = (new BlockSlab(44, false)).SetHardness(2.0F).SetResistance(10.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("stoneSlab");
        public static readonly Block BRICKS = (new Block(45, 7, Material.STONE)).SetHardness(2.0F).SetResistance(10.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("brick");
        public static readonly Block TNT = (new BlockTNT(46, 8)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("tnt");
        public static readonly Block BOOKSHELF = (new BlockBookshelf(47, 35)).SetHardness(1.5F).SetSoundGroup(SoundWoodFootstep).SetBlockName("bookshelf");
        public static readonly Block MOSSY_COBBLESTONE = (new Block(48, 36, Material.STONE)).SetHardness(2.0F).SetResistance(10.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("stoneMoss");
        public static readonly Block OBSIDIAN = (new BlockObsidian(49, 37)).SetHardness(10.0F).SetResistance(2000.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("obsidian");
        public static readonly Block TORCH = (new BlockTorch(50, 80)).SetHardness(0.0F).SetLuminance(15.0F / 16.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("torch").IgnoreMetaUpdates();
        public static readonly BlockFire FIRE = (BlockFire)(new BlockFire(51, 31)).SetHardness(0.0F).SetLuminance(1.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("fire").DisableStats().IgnoreMetaUpdates();
        public static readonly Block SPAWNER = (new BlockMobSpawner(52, 65)).SetHardness(5.0F).SetSoundGroup(SoundMetalFootstep).SetBlockName("mobSpawner").DisableStats();
        public static readonly Block WOODEN_STAIRS = (new BlockStairs(53, PLANKS)).SetBlockName("stairsWood").IgnoreMetaUpdates();
        public static readonly Block CHEST = (new BlockChest(54)).SetHardness(2.5F).SetSoundGroup(SoundWoodFootstep).SetBlockName("chest").IgnoreMetaUpdates();
        public static readonly Block REDSTONE_WIRE = (new BlockRedstoneWire(55, 164)).SetHardness(0.0F).SetSoundGroup(SoundPowderFootstep).SetBlockName("redstoneDust").DisableStats().IgnoreMetaUpdates();
        public static readonly Block DIAMOND_ORE = (new BlockOre(56, 50)).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("oreDiamond");
        public static readonly Block DIAMOND_BLOCK = (new BlockOreStorage(57, 24)).SetHardness(5.0F).SetResistance(10.0F).SetSoundGroup(SoundMetalFootstep).SetBlockName("blockDiamond");
        public static readonly Block CRAFTING_TABLE = (new BlockWorkbench(58)).SetHardness(2.5F).SetSoundGroup(SoundWoodFootstep).SetBlockName("workbench");
        public static readonly Block WHEAT = (new BlockCrops(59, 88)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("crops").DisableStats().IgnoreMetaUpdates();
        public static readonly Block FARMLAND = (new BlockFarmland(60)).SetHardness(0.6F).SetSoundGroup(SoundGravelFootstep).SetBlockName("farmland");
        public static readonly Block FURNACE = (new BlockFurnace(61, false)).SetHardness(3.5F).SetSoundGroup(SoundStoneFootstep).SetBlockName("furnace").IgnoreMetaUpdates();
        public static readonly Block LIT_FURNACE = (new BlockFurnace(62, true)).SetHardness(3.5F).SetSoundGroup(SoundStoneFootstep).SetLuminance(14.0F / 16.0F).SetBlockName("furnace").IgnoreMetaUpdates();
        public static readonly Block SIGN = (new BlockSign(63, BlockEntitySign.Class, true)).SetHardness(1.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("sign").DisableStats().IgnoreMetaUpdates();
        public static readonly Block DOOR = (new BlockDoor(64, Material.WOOD)).SetHardness(3.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("doorWood").DisableStats().IgnoreMetaUpdates();
        public static readonly Block LADDER = (new BlockLadder(65, 83)).SetHardness(0.4F).SetSoundGroup(SoundWoodFootstep).SetBlockName("ladder").IgnoreMetaUpdates();
        public static readonly Block RAIL = (new BlockRail(66, 128, false)).SetHardness(0.7F).SetSoundGroup(SoundMetalFootstep).SetBlockName("rail").IgnoreMetaUpdates();
        public static readonly Block COBBLESTONE_STAIRS = (new BlockStairs(67, COBBLESTONE)).SetBlockName("stairsStone").IgnoreMetaUpdates();
        public static readonly Block WALL_SIGN = (new BlockSign(68, BlockEntitySign.Class, false)).SetHardness(1.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("sign").DisableStats().IgnoreMetaUpdates();
        public static readonly Block LEVER = (new BlockLever(69, 96)).SetHardness(0.5F).SetSoundGroup(SoundWoodFootstep).SetBlockName("lever").IgnoreMetaUpdates();
        public static readonly Block STONE_PRESSURE_PLATE = (new BlockPressurePlate(70, STONE.textureId, PressurePlateActiviationRule.MOBS, Material.STONE)).SetHardness(0.5F).SetSoundGroup(SoundStoneFootstep).SetBlockName("pressurePlate").IgnoreMetaUpdates();
        public static readonly Block IRON_DOOR = (new BlockDoor(71, Material.METAL)).SetHardness(5.0F).SetSoundGroup(SoundMetalFootstep).SetBlockName("doorIron").DisableStats().IgnoreMetaUpdates();
        public static readonly Block WOODEN_PRESSURE_PLATE = (new BlockPressurePlate(72, PLANKS.textureId, PressurePlateActiviationRule.EVERYTHING, Material.WOOD)).SetHardness(0.5F).SetSoundGroup(SoundWoodFootstep).SetBlockName("pressurePlate").IgnoreMetaUpdates();
        public static readonly Block REDSTONE_ORE = (new BlockRedstoneOre(73, 51, false)).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("oreRedstone").IgnoreMetaUpdates();
        public static readonly Block LIT_REDSTONE_ORE = (new BlockRedstoneOre(74, 51, true)).SetLuminance(10.0F / 16.0F).SetHardness(3.0F).SetResistance(5.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("oreRedstone").IgnoreMetaUpdates();
        public static readonly Block REDSTONE_TORCH = (new BlockRedstoneTorch(75, 115, false)).SetHardness(0.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("notGate").IgnoreMetaUpdates();
        public static readonly Block LIT_REDSTONE_TORCH = (new BlockRedstoneTorch(76, 99, true)).SetHardness(0.0F).SetLuminance(0.5F).SetSoundGroup(SoundWoodFootstep).SetBlockName("notGate").IgnoreMetaUpdates();
        public static readonly Block BUTTON = (new BlockButton(77, STONE.textureId)).SetHardness(0.5F).SetSoundGroup(SoundStoneFootstep).SetBlockName("button").IgnoreMetaUpdates();
        public static readonly Block SNOW = (new BlockSnow(78, 66)).SetHardness(0.1F).SetSoundGroup(SoundClothFootstep).SetBlockName("snow");
        public static readonly Block ICE = (new BlockIce(79, 67)).SetHardness(0.5F).SetOpacity(3).SetSoundGroup(SoundGlassFootstep).SetBlockName("ice");
        public static readonly Block SNOW_BLOCK = (new BlockSnowBlock(80, 66)).SetHardness(0.2F).SetSoundGroup(SoundClothFootstep).SetBlockName("snow");
        public static readonly Block CACTUS = (new BlockCactus(81, 70)).SetHardness(0.4F).SetSoundGroup(SoundClothFootstep).SetBlockName("cactus");
        public static readonly Block CLAY = (new BlockClay(82, 72)).SetHardness(0.6F).SetSoundGroup(SoundGravelFootstep).SetBlockName("clay");
        public static readonly Block SUGAR_CANE = (new BlockReed(83, 73)).SetHardness(0.0F).SetSoundGroup(SoundGrassFootstep).SetBlockName("reeds").DisableStats();
        public static readonly Block JUKEBOX = (new BlockJukeBox(84, 74)).SetHardness(2.0F).SetResistance(10.0F).SetSoundGroup(SoundStoneFootstep).SetBlockName("jukebox").IgnoreMetaUpdates();
        public static readonly Block FENCE = (new BlockFence(85, 4)).SetHardness(2.0F).SetResistance(5.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("fence").IgnoreMetaUpdates();
        public static readonly Block PUMPKIN = (new BlockPumpkin(86, 102, false)).SetHardness(1.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("pumpkin").IgnoreMetaUpdates();
        public static readonly Block NETHERRACK = (new BlockNetherrack(87, 103)).SetHardness(0.4F).SetSoundGroup(SoundStoneFootstep).SetBlockName("hellrock");
        public static readonly Block SOUL_SAND = (new BlockSoulSand(88, 104)).SetHardness(0.5F).SetSoundGroup(SoundSandFootstep).SetBlockName("hellsand");
        public static readonly Block GLOWSTONE = (new BlockGlowStone(89, 105, Material.STONE)).SetHardness(0.3F).SetSoundGroup(SoundGlassFootstep).SetLuminance(1.0F).SetBlockName("lightgem");
        public static readonly BlockPortal NETHER_PORTAL = (BlockPortal)(new BlockPortal(90, 14)).SetHardness(-1.0F).SetSoundGroup(SoundGlassFootstep).SetLuminance(12.0F / 16.0F).SetBlockName("portal");
        public static readonly Block JACK_O_LANTERN = (new BlockPumpkin(91, 102, true)).SetHardness(1.0F).SetSoundGroup(SoundWoodFootstep).SetLuminance(1.0F).SetBlockName("litpumpkin").IgnoreMetaUpdates();
        public static readonly Block CAKE = (new BlockCake(92, 121)).SetHardness(0.5F).SetSoundGroup(SoundClothFootstep).SetBlockName("cake").DisableStats().IgnoreMetaUpdates();
        public static readonly Block REPEATER = (new BlockRedstoneRepeater(93, false)).SetHardness(0.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("diode").DisableStats().IgnoreMetaUpdates();
        public static readonly Block POWERED_REPEATER = (new BlockRedstoneRepeater(94, true)).SetHardness(0.0F).SetLuminance(10.0F / 16.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("diode").DisableStats().IgnoreMetaUpdates();
        public static readonly Block TRAPDOOR = (new BlockTrapDoor(96, Material.WOOD)).SetHardness(3.0F).SetSoundGroup(SoundWoodFootstep).SetBlockName("trapdoor").DisableStats().IgnoreMetaUpdates();
        public int textureId;
        public readonly int id;
        public float Hardness;
        public float Resistance;
        protected bool EnableStats;
        public double minX; // TODO: Just use Box, it's literally just pasted code
        public double minY;
        public double minZ;
        public double maxX;
        public double maxY;
        public double maxZ;
        public BlockSoundGroup SoundGroup;
        public float ParticleFallSpeedModifier;
        public readonly Material Material;
        public float Slipperiness;
        private string BlockName;

        protected Block(int id, Material material)
        {
            EnableStats = true;
            SoundGroup = SoundPowderFootstep;
            ParticleFallSpeedModifier = 1.0F;
            Slipperiness = 0.6F;
            if (BLOCKS[id] != null)
            {
                throw new IllegalArgumentException("Slot " + id + " is already occupied by " + BLOCKS[id] + " when adding " + this);
            }

            this.Material = material;
            BLOCKS[id] = this;
            this.id = id;
            setBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
            BLOCKS_OPAQUE[id] = IsOpaque();
            BLOCK_LIGHT_OPACITY[id] = IsOpaque() ? 255 : 0;
            BLOCKS_ALLOW_VISION[id] = !material.blocksVision();
            BLOCKS_WITH_ENTITY[id] = false;
        }

        protected Block IgnoreMetaUpdates()
        {
            BLOCKS_IGNORE_META_UPDATE[id] = true;
            return this;
        }

        protected virtual void Init()
        {
        }

        protected Block(int id, int textureId, Material material) : this(id, material)
        {
            this.textureId = textureId;
        }

        protected Block SetSoundGroup(BlockSoundGroup soundGroup)
        {
            this.SoundGroup = soundGroup;
            return this;
        }

        protected Block SetOpacity(int opacity)
        {
            BLOCK_LIGHT_OPACITY[id] = opacity;
            return this;
        }

        protected Block SetLuminance(float fractionalValue)
        {
            BLOCKS_LIGHT_LUMINANCE[id] = (int)(15.0F * fractionalValue);
            return this;
        }

        protected Block SetResistance(float resistance)
        {
            this.Resistance = resistance * 3.0F;
            return this;
        }

        public virtual bool IsFullCube()
        {
            return true;
        }

        public virtual int GetRenderType()
        {
            return 0;
        }

        protected Block SetHardness(float hardness)
        {
            this.Hardness = hardness;
            if (Resistance < hardness * 5.0F)
            {
                Resistance = hardness * 5.0F;
            }

            return this;
        }

        protected Block SetUnbreakable()
        {
            SetHardness(-1.0F);
            return this;
        }

        public float GetHardness()
        {
            return Hardness;
        }

        protected Block SetTickRandomly(bool tickRandomly)
        {
            BLOCKS_RANDOM_TICK[id] = tickRandomly;
            return this;
        }

        public void setBoundingBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            this.minX = minX;
            this.minY = minY;
            this.minZ = minZ;
            this.maxX = maxX;
            this.maxY = maxY;
            this.maxZ = maxZ;
        }

        public virtual float getLuminance(BlockView blockView, int x, int y, int z)
        {
            return blockView.getNaturalBrightness(x, y, z, BLOCKS_LIGHT_LUMINANCE[id]);
        }

        public virtual bool IsSideVisible(BlockView blockView, int x, int y, int z, int side)
        {
            return side == 0 && minY > 0.0D ? true : (side == 1 && maxY < 1.0D ? true : (side == 2 && minZ > 0.0D ? true : (side == 3 && maxZ < 1.0D ? true : (side == 4 && minX > 0.0D ? true : (side == 5 && maxX < 1.0D ? true : !blockView.isOpaque(x, y, z))))));
        }

        public virtual bool IsSolidFace(BlockView blockView, int x, int y, int z, int face)
        {
            return blockView.getMaterial(x, y, z).isSolid();
        }

        public virtual int GetTextureId(BlockView blockView, int x, int y, int z, int side)
        {
            return GetTexture(side, blockView.getBlockMeta(x, y, z));
        }

        public virtual int GetTexture(int side, int meta)
        {
            return GetTexture(side);
        }

        public virtual int GetTexture(int side)
        {
            return textureId;
        }

        public virtual Box GetBoundingBox(World world, int x, int y, int z)
        {
            return new Box(x + minX, y + minY, z + minZ, x + maxX, y + maxY, z + maxZ);
        }

        public virtual void AddIntersectingBoundingBox(World world, int x, int y, int z, Box box, List<Box> boxes)
        {
            Box? collisionBox = GetCollisionShape(world, x, y, z);
            if (collisionBox != null && box.intersects(collisionBox.Value))
            {
                boxes.Add(collisionBox.Value);
            }
        }

        public virtual Box? GetCollisionShape(World world, int x, int y, int z)
        {
            return new Box(x + minX, y + minY, z + minZ, x + maxX, y + maxY, z + maxZ);
        }

        public virtual bool IsOpaque()
        {
            return true;
        }

        public virtual bool HasCollision(int meta, bool allowLiquids)
        {
            return HasCollision();
        }

        public virtual bool HasCollision()
        {
            return true;
        }

        public virtual void OnTick(World world, int x, int y, int z, java.util.Random random)
        {
        }

        public virtual void RandomDisplayTick(World world, int x, int y, int z, java.util.Random random)
        {
        }

        public virtual void OnMetadataChange(World world, int x, int y, int z, int meta)
        {
        }

        public virtual void NeighborUpdate(World world, int x, int y, int z, int id)
        {
        }

        public virtual int GetTickRate()
        {
            return 10;
        }

        public virtual void OnPlaced(World world, int x, int y, int z)
        {
        }

        public virtual void OnBreak(World world, int x, int y, int z)
        {
        }

        public virtual int GetDroppedItemCount(java.util.Random random)
        {
            return 1;
        }

        public virtual int GetDroppedItemId(int blockMeta, java.util.Random random)
        {
            return id;
        }

        public float GetHardness(EntityPlayer player)
        {
            return Hardness < 0.0F ? 0.0F : (!player.canHarvest(this) ? 1.0F / Hardness / 100.0F : player.getBlockBreakingSpeed(this) / Hardness / 30.0F);
        }

        public void DropStacks(World world, int x, int y, int z, int meta)
        {
            DropStacks(world, x, y, z, meta, 1.0F);
        }

        public virtual void DropStacks(World world, int x, int y, int z, int meta, float luck)
        {
            if (!world.isRemote)
            {
                int dropCount = GetDroppedItemCount(world.random);

                for (int attempt = 0; attempt < dropCount; ++attempt)
                {
                    if (world.random.nextFloat() <= luck)
                    {
                        int itemId = GetDroppedItemId(meta, world.random);
                        if (itemId > 0)
                        {
                            DropStack(world, x, y, z, new ItemStack(itemId, 1, GetDroppedItemMeta(meta)));
                        }
                    }
                }

            }
        }

        protected void DropStack(World world, int x, int y, int z, ItemStack itemStack)
        {
            if (!world.isRemote)
            {
                float spreadFactor = 0.7F;
                double offsetX = (double)(world.random.nextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
                double offsetY = (double)(world.random.nextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
                double offsetZ = (double)(world.random.nextFloat() * spreadFactor) + (double)(1.0F - spreadFactor) * 0.5D;
                EntityItem droppedItem = new EntityItem(world, (double)x + offsetX, (double)y + offsetY, (double)z + offsetZ, itemStack);
                droppedItem.delayBeforeCanPickup = 10;
                world.spawnEntity(droppedItem);
            }
        }

        protected virtual int GetDroppedItemMeta(int blockMeta)
        {
            return 0;
        }

        public virtual float GetBlastResistance(Entity entity)
        {
            return Resistance / 5.0F;
        }

        public virtual HitResult Raycast(World world, int x, int y, int z, Vec3D startPos, Vec3D endPos)
        {
            UpdateBoundingBox(world, x, y, z);
            Vec3D pos = new Vec3D(x, y, z);
            HitResult res = new Box(minX, minY, minZ, maxX, maxY, maxZ).raycast(startPos - pos, endPos - pos);
            if (res == null) return null;
            res.blockX = x;
            res.blockY = y;
            res.blockZ = z;
            res.pos += pos;
            return res;
        }

        public virtual void OnDestroyedByExplosion(World world, int x, int y, int z)
        {
        }

        public virtual int GetRenderLayer()
        {
            return 0;
        }

        public virtual bool CanPlaceAt(World world, int x, int y, int z, int side)
        {
            return CanPlaceAt(world, x, y, z);
        }

        public virtual bool CanPlaceAt(World world, int x, int y, int z)
        {
            int blockId = world.getBlockId(x, y, z);
            return blockId == 0 || BLOCKS[blockId].Material.isReplaceable();
        }

        public virtual bool OnUse(World world, int x, int y, int z, EntityPlayer player)
        {
            return false;
        }

        public virtual void OnSteppedOn(World world, int x, int y, int z, Entity entity)
        {
        }

        public virtual void OnPlaced(World world, int x, int y, int z, int direction)
        {
        }

        public virtual void OnBlockBreakStart(World world, int x, int y, int z, EntityPlayer player)
        {
        }

        public virtual void ApplyVelocity(World world, int x, int y, int z, Entity entity, Vec3D velocity)
        {
        }

        public virtual void UpdateBoundingBox(BlockView blockView, int x, int y, int z)
        {
        }

        public virtual int GetColor(int meta)
        {
            return 0xffffff;
        }

        public virtual int GetColorMultiplier(BlockView blockView, int x, int y, int z)
        {
            return 0xffffff;
        }

        public virtual bool IsPoweringSide(BlockView blockView, int x, int y, int z, int side)
        {
            return false;
        }

        public virtual bool CanEmitRedstonePower()
        {
            return false;
        }

        public virtual void OnEntityCollision(World world, int x, int y, int z, Entity entity)
        {
        }

        public virtual bool IsStrongPoweringSide(World world, int x, int y, int z, int side)
        {
            return false;
        }

        public virtual void SetupRenderBoundingBox()
        {
        }

        public virtual void AfterBreak(World world, EntityPlayer player, int x, int y, int z, int meta)
        {
            player.increaseStat(Stats.Stats.mineBlockStatArray[id], 1);
            DropStacks(world, x, y, z, meta);
        }

        public virtual bool CanGrow(World world, int x, int y, int z)
        {
            return true;
        }

        public virtual void OnPlaced(World world, int x, int y, int z, EntityLiving placer)
        {
        }

        public Block SetBlockName(string name)
        {
            BlockName = "tile." + name;
            return this;
        }

        public string TranslateBlockName()
        {
            return StatCollector.translateToLocal(GetBlockName() + ".name");
        }

        public string GetBlockName()
        {
            return BlockName;
        }

        public virtual void OnBlockAction(World world, int x, int y, int z, int data1, int data2)
        {
        }

        public bool GetEnableStats()
        {
            return EnableStats;
        }

        protected Block DisableStats()
        {
            EnableStats = false;
            return this;
        }

        public virtual int GetPistonBehavior()
        {
            return Material.getPistonBehavior();
        }

        static Block()
        {
            Item.ITEMS[WOOL.id] = (new ItemCloth(WOOL.id - 256)).setItemName("cloth");
            Item.ITEMS[LOG.id] = (new ItemLog(LOG.id - 256)).setItemName("log");
            Item.ITEMS[SLAB.id] = (new ItemSlab(SLAB.id - 256)).setItemName("stoneSlab");
            Item.ITEMS[SAPLING.id] = (new ItemSapling(SAPLING.id - 256)).setItemName("sapling");
            Item.ITEMS[LEAVES.id] = (new ItemLeaves(LEAVES.id - 256)).setItemName("leaves");
            Item.ITEMS[PISTON.id] = new ItemPiston(PISTON.id - 256);
            Item.ITEMS[STICKY_PISTON.id] = new ItemPiston(STICKY_PISTON.id - 256);

            for (int blockId = 0; blockId < 256; ++blockId)
            {
                if (BLOCKS[blockId] != null && Item.ITEMS[blockId] == null)
                {
                    Item.ITEMS[blockId] = new ItemBlock(blockId - 256);
                    BLOCKS[blockId].Init();
                }
            }

            BLOCKS_ALLOW_VISION[0] = true;
            Stats.Stats.initializeItemStats();
        }
    }

}