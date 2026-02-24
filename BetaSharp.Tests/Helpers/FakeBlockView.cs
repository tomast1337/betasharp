using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds;
using BetaSharp.Worlds.Biomes.Source;

namespace BetaSharp.Tests.Helpers;

internal class FakeBlockView : BlockView
{
    private readonly Dictionary<(int x, int y, int z), int> _blockIds = new();
    private readonly Dictionary<(int x, int y, int z), int> _blockMeta = new();

    public void SetBlock(int x, int y, int z, int blockId, int meta = 0)
    {
        _blockIds[(x, y, z)] = blockId;
        if (meta != 0)
            _blockMeta[(x, y, z)] = meta;
    }

    public void SetSolid(int x, int y, int z) =>
        SetBlock(x, y, z, Block.Stone.id);

    public void SetFloorRange(int xMin, int xMax, int zMin, int zMax, int y = 0)
    {
        for (int x = xMin; x <= xMax; x++)
            for (int z = zMin; z <= zMax; z++)
                SetSolid(x, y, z);
    }

    public int getBlockId(int x, int y, int z) =>
        _blockIds.TryGetValue((x, y, z), out int id) ? id : 0;

    public int getBlockMeta(int x, int y, int z) =>
        _blockMeta.TryGetValue((x, y, z), out int meta) ? meta : 0;

    public BlockEntity? getBlockEntity(int x, int y, int z) => null;
    public float getNaturalBrightness(int x, int y, int z, int blockLight) => 0f;
    public float getLuminance(int x, int y, int z) => 0f;
    public Material getMaterial(int x, int y, int z) => null!;
    public bool isOpaque(int x, int y, int z) => false;
    public bool shouldSuffocate(int x, int y, int z) => false;
    public BiomeSource getBiomeSource() => null!;
}
