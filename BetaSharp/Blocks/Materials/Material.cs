using BetaSharp.Worlds.Maps;

namespace BetaSharp.Blocks.Materials
{
    /// <summary>
    /// Base class for block materials, defining properties such as whether the material is solid, transparent, burnable, etc. and containing static instances for each material type in Minecraft.
    /// </summary>
    public class Material
    {
        public static readonly Material Air = new MaterialTransparent(MapColor.Air);
        public static readonly Material SolidOrganic = new(MapColor.Grass);
        public static readonly Material Soil = new(MapColor.Dirt);
        public static readonly Material Wood = new Material(MapColor.Wood).SetBurning();
        public static readonly Material Stone = new Material(MapColor.Stone).SetRequiresTool();
        public static readonly Material Metal = new Material(MapColor.Iron).SetRequiresTool();
        public static readonly Material Water = new MaterialLiquid(MapColor.Water).SetDestroyPistonBehavior();
        public static readonly Material Lava = new MaterialLiquid(MapColor.TNT).SetDestroyPistonBehavior();
        public static readonly Material Leaves = new Material(MapColor.Foliage).SetBurning().SetTransparent().SetDestroyPistonBehavior();
        public static readonly Material Plant = new MaterialLogic(MapColor.Foliage).SetDestroyPistonBehavior();
        public static readonly Material Sponge = new(MapColor.Cloth);
        public static readonly Material Wool = new Material(MapColor.Cloth).SetBurning();
        public static readonly Material Fire = new MaterialTransparent(MapColor.Air).SetDestroyPistonBehavior();
        public static readonly Material Sand = new(MapColor.Sand);
        public static readonly Material PistonBreakable = new MaterialLogic(MapColor.Air).SetDestroyPistonBehavior();
        public static readonly Material Glass = new Material(MapColor.Air).SetTransparent();
        public static readonly Material Tnt = new Material(MapColor.TNT).SetBurning().SetTransparent();
        public static readonly Material Foliage = new Material(MapColor.Foliage).SetDestroyPistonBehavior();
        public static readonly Material Ice = new Material(MapColor.Ice).SetTransparent();
        public static readonly Material SnowLayer = new MaterialLogic(MapColor.Snow).SetReplaceable().SetTransparent().SetRequiresTool().SetDestroyPistonBehavior();
        public static readonly Material SnowBlock = new Material(MapColor.Snow).SetRequiresTool();
        public static readonly Material Cactus = new Material(MapColor.Foliage).SetTransparent().SetDestroyPistonBehavior();
        public static readonly Material Clay = new(MapColor.Clay);
        public static readonly Material Pumpkin = new Material(MapColor.Foliage).SetDestroyPistonBehavior();
        public static readonly Material NetherPortal = new MaterialPortal(MapColor.Air).SetUnpushablePistonBehavior();
        public static readonly Material Cake = new Material(MapColor.Air).SetDestroyPistonBehavior();
        public static readonly Material Cobweb = new Material(MapColor.Cloth).SetRequiresTool().SetDestroyPistonBehavior();
        public static readonly Material Piston = new Material(MapColor.Stone).SetUnpushablePistonBehavior();

        private bool _transparent;

        /// <summary>
        /// Map color of the material.
        /// </summary>
        public MapColor MapColor { get; }

        /// <summary>
        /// If the material is a fluid.
        /// </summary>
        public virtual bool IsFluid => false;

        /// <summary>
        /// If the material is a solid.
        /// </summary>
        public virtual bool IsSolid => true;

        /// <summary>
        /// If the materials blocks vision through it.
        /// </summary>
        public virtual bool BlocksVision => true;

        /// <summary>
        /// If the materials blocks movement through it.
        /// </summary>
        public virtual bool BlocksMovement => true;

        /// <summary>
        /// If the block is burnable (by fire or lava).
        /// </summary>
        public bool IsBurnable { get; private set; }

        /// <summary>
        /// If the block is replaceable.
        /// </summary>
        public bool IsReplaceable { get; private set; }

        /// <summary>
        /// If the block can be harvested by hand.
        /// </summary>
        public bool IsHandHarvestable { get; private set; } = true;

        /// <summary>
        /// <see cref="MaterialPistonBehavior"/> defining how the block behaves when pushed by a piston.
        /// </summary>
        public MaterialPistonBehavior PistonBehavior { get; private set; }

        /// <summary>
        /// If the material suffocates entities inside it. A material suffocates if it blocks movement and is not transparent.
        /// </summary>
        public bool Suffocates => _transparent ? false : BlocksMovement;

        public Material(MapColor mapColor)
        {
            MapColor = mapColor;
        }

        /// <summary>
        /// Sets the material to use transparency and returns the current instance.
        /// </summary>
        /// <returns>The current instance with transparency enabled.</returns>
        private Material SetTransparent()
        {
            _transparent = true;
            return this;
        }

        /// <summary>
        /// Marks the material as requiring a tool for harvesting (not hand harvestable).
        /// </summary>
        /// <returns>The current instance with the tool requirement set.</returns>
        private Material SetRequiresTool()
        {
            IsHandHarvestable = false;
            return this;
        }

        /// <summary>
        /// Marks the material as burnable and returns the updated instance.
        /// </summary>
        /// <returns>The current instance of the material with burnable state enabled.</returns>
        private Material SetBurning()
        {
            IsBurnable = true;
            return this;
        }

        /// <summary>
        /// Marks the material as replaceable and returns the current instance.
        /// </summary>
        /// <returns>The current instance of the material with the replaceable flag set.</returns>
        public Material SetReplaceable()
        {
            IsReplaceable = true;
            return this;
        }

        /// <summary>
        /// Sets the piston behavior to break when acted upon by a piston.
        /// </summary>
        /// <returns>The current instance with the updated piston behavior.</returns>
        protected Material SetDestroyPistonBehavior()
        {
            PistonBehavior = MaterialPistonBehavior.Break;
            return this;
        }

        /// <summary>
        /// Sets the piston behavior to unpushable, meaning it cannot be moved by pistons.
        /// </summary>
        /// <returns>The current instance with the updated piston behavior.</returns>
        protected Material SetUnpushablePistonBehavior()
        {
            PistonBehavior = MaterialPistonBehavior.Unpushable;
            return this;
        }
    }
}
