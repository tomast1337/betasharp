using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockSlab : Block
{
    public static readonly string[] Names = ["stone", "sand", "wood", "cobble"];
    private readonly bool _doubleSlab;

    public BlockSlab(int id, bool doubleSlab) : base(id, 6, Material.Stone)
    {
        _doubleSlab = doubleSlab;
        if (!_doubleSlab) SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);

        SetOpacity(255);
    }

    public override int GetTexture(Side side, int meta) => (meta, side) switch
    {
        (0, Side.Up or Side.Down) => BlockTextures.StoneSlabTop,
        (0, _) => BlockTextures.StoneSlabSide,
        (1, Side.Up) => BlockTextures.SandstoneTop,
        (1, Side.Down) => BlockTextures.SandstoneBottom,
        (1, _) => BlockTextures.SandstoneSide,
        (2, _) => BlockTextures.OakPlanks,
        (3, _) => BlockTextures.Cobblestone,
        _ => BlockTextures.StoneSlabTop
    };

    public override int GetTexture(Side side) => GetTexture(side, 0);

    public override bool IsOpaque => _doubleSlab;

    public override void OnPlaced(OnPlacedEvent etv)
    {
        if (this != Slab) base.OnPlaced(etv);

        int blockBelowId = etv.World.Reader.GetBlockId(etv.X, etv.Y - 1, etv.Z);
        int slabMeta = etv.World.Reader.GetBlockMeta(etv.X, etv.Y, etv.Z);
        int blockBelowMeta = etv.World.Reader.GetBlockMeta(etv.X, etv.Y - 1, etv.Z);
        if (slabMeta != blockBelowMeta) return;

        if (blockBelowId != Slab.Id) return;

        etv.World.Writer.SetBlock(etv.X, etv.Y, etv.Z, 0);
        etv.World.Writer.SetBlock(etv.X, etv.Y - 1, etv.Z, DoubleSlab.Id, slabMeta);
    }

    public override int GetDroppedItemId(int blockMeta) => Slab.Id;

    public override int DroppedItemCount => _doubleSlab ? 2 : 1;

    protected override int GetDroppedItemMeta(int blockMeta) => blockMeta;

    public override bool IsFullCube => _doubleSlab;

    public override bool IsSideVisible(IBlockReader iBlockReader, int x, int y, int z, Side side)
    {
        if (this != Slab) base.IsSideVisible(iBlockReader, x, y, z, side);

        return side == Side.Up || (base.IsSideVisible(iBlockReader, x, y, z, side) && (side == Side.Down || iBlockReader.GetBlockId(x, y, z) != Id));
    }
}
