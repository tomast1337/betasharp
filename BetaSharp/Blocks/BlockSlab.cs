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
        if (!doubleSlab)
        {
            SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.5F, 1.0F);
        }

        SetOpacity(255);
    }

    public override int GetTexture(int side, int meta) => meta == 0 ? side <= 1 ? 6 : 5 : meta == 1 ? side == 0 ? 208 : side == 1 ? 176 : 192 : meta == 2 ? 4 : meta == 3 ? 16 : 6;

    public override int GetTexture(int side) => GetTexture(side, 0);

    public override bool IsOpaque() => _doubleSlab;

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

    public override int GetDroppedItemCount() => _doubleSlab ? 2 : 1;

    protected override int GetDroppedItemMeta(int blockMeta) => blockMeta;

    public override bool IsFullCube() => _doubleSlab;

    public override bool IsSideVisible(IBlockReader iBlockReader, int x, int y, int z, int side)
    {
        if (this != Slab)
        {
            base.IsSideVisible(iBlockReader, x, y, z, side);
        }

        return side == 1 ? true : !base.IsSideVisible(iBlockReader, x, y, z, side) ? false : side == 0 ? true : iBlockReader.GetBlockId(x, y, z) != Id;
    }
}
