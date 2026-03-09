namespace BetaSharp.Worlds.Core;

public interface ILightProvider
{
    float GetNaturalBrightness(int x, int y, int z, int minLight);
    float GetLuminance(int x, int y, int z);
}
