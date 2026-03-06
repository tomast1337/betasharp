using BetaSharp.Blocks.Entities;
using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Biomes.Source;

namespace BetaSharp.Worlds;

public interface IBlockAccess
{
    int GetBlockId(int x, int y, int z);

    BlockEntity? GetBlockEntity(int x, int y, int z);

    float GetNaturalBrightness(int x, int y, int z, int blockLight);

    float GetLuminance(int x, int y, int z);

    int GetBlockMeta(int x, int y, int z);

    Material GetMaterial(int x, int y, int z);

    bool IsOpaque(int x, int y, int z);

    bool ShouldSuffocate(int x, int y, int z);

    BiomeSource GetBiomeSource();
}
