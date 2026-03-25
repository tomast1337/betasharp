using System.Runtime.InteropServices;
using Silk.NET.Maths;

namespace BetaSharp.Client.Rendering.Particles;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ParticleVertex
{
    public Vector3D<float> Position;
    public Vector2D<float> TexCoords;
    public Vector4D<float> Color;

    public ParticleVertex(float x, float y, float z, float u, float v, float r, float g, float b, float a)
    {
        Position = new Vector3D<float>(x, y, z);
        TexCoords = new Vector2D<float>(u, v);
        Color = new Vector4D<float>(r, g, b, a);
    }
}
