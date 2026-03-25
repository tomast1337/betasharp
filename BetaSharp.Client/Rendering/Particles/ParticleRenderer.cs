using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Util;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;
using Silk.NET.Maths;
using Shader = BetaSharp.Client.Rendering.Core.Shader;
using VertexArray = BetaSharp.Client.Rendering.Core.VertexArray;
using GLEnum = BetaSharp.Client.Rendering.Core.OpenGL.GLEnum;

namespace BetaSharp.Client.Rendering.Particles;

public static class ParticleRenderer
{
    private static readonly string[] s_layerTextures =
    [
        "/particles.png",
        "/terrain.png",
        "/gui/items.png"
    ];

    private static bool _initialized;
    private static Shader _shader;
    private static VertexBuffer<ParticleVertex>[] _vertexBuffers;
    private static VertexArray[] _vertexArrays;
    private static ParticleVertex[] _vertexData = Array.Empty<ParticleVertex>();

    private static void EnsureCapacity(int particleCount)
    {
        int requiredVertices = particleCount * 6;
        if (_vertexData.Length < requiredVertices)
        {
            // Grow to power of 2
            int nextPow2 = 1024;
            while (nextPow2 < requiredVertices)
            {
                nextPow2 *= 2;
            }
            _vertexData = new ParticleVertex[nextPow2];
        }
    }

    private static unsafe void UploadMesh(int bufferIdx, Span<ParticleVertex> meshData)
    {
        if (_vertexBuffers[bufferIdx] == null)
        {
            _vertexBuffers[bufferIdx] = new VertexBuffer<ParticleVertex>(meshData);
        }
        else
        {
            _vertexBuffers[bufferIdx].BufferData(meshData);
        }

        if (_vertexArrays[bufferIdx] == null)
        {
            _vertexArrays[bufferIdx] = new VertexArray();
            _vertexArrays[bufferIdx].Bind();
            _vertexBuffers[bufferIdx].Bind();

            const uint stride = 36;

            GLManager.GL.EnableVertexAttribArray(0);
            GLManager.GL.VertexAttribPointer(0, 3, GLEnum.Float, false, stride, (void*)0);

            GLManager.GL.EnableVertexAttribArray(1);
            GLManager.GL.VertexAttribPointer(1, 2, GLEnum.Float, false, stride, (void*)12);

            GLManager.GL.EnableVertexAttribArray(2);
            GLManager.GL.VertexAttribPointer(2, 4, GLEnum.Float, false, stride, (void*)20);

            VertexArray.Unbind();
        }
    }

    public static void Render(
        ParticleBuffer[] layers,
        float yaw, float pitch,
        double x, double y, double z,
        double lastTickX, double lastTickY, double lastTickZ,
        float partialTick,
        TextureManager textureManager,
        IWorldContext world)
    {
        if (!_initialized)
        {
            _shader = new Shader(AssetManager.Instance.getAsset("shaders/particle.vert").GetTextContent(), AssetManager.Instance.getAsset("shaders/particle.frag").GetTextContent());
            _vertexBuffers = new VertexBuffer<ParticleVertex>[3];
            _vertexArrays = new VertexArray[3];
            _initialized = true;
        }

        float radYaw = yaw * MathF.PI / 180.0f;
        float radPitch = pitch * MathF.PI / 180.0f;

        float cosYaw = MathHelper.Cos(radYaw);
        float sinYaw = MathHelper.Sin(radYaw);
        float cosPitch = MathHelper.Cos(radPitch);
        float sinPitch = MathHelper.Sin(radPitch);

        // Billboarding vecs
        float upX = -sinYaw * sinPitch;
        float upZ = cosYaw * sinPitch;

        double interpX = lastTickX + (x - lastTickX) * partialTick;
        double interpY = lastTickY + (y - lastTickY) * partialTick;
        double interpZ = lastTickZ + (z - lastTickZ) * partialTick;

        _shader.Bind();
        _shader.SetUniform1("textureSampler", 0);

        Matrix4X4<float> modelView = default;
        Matrix4X4<float> projection = default;
        
        unsafe 
        {
            GLManager.GL.GetFloat(GLEnum.ModelviewMatrix, (float*)&modelView);
            GLManager.GL.GetFloat(GLEnum.ProjectionMatrix, (float*)&projection);
            
            Vector4D<float> fogColor;
            GLManager.GL.GetFloat(GLEnum.FogColor, (float*)&fogColor);
            _shader.SetUniform4("fogColor", fogColor);
            
            float fogStart, fogEnd, fogDensity, fogModeVal;
            GLManager.GL.GetFloat(GLEnum.FogStart, out fogStart);
            GLManager.GL.GetFloat(GLEnum.FogEnd, out fogEnd);
            GLManager.GL.GetFloat(GLEnum.FogDensity, out fogDensity);
            GLManager.GL.GetFloat(GLEnum.FogMode, out fogModeVal);

            _shader.SetUniform1("fogStart", fogStart);
            _shader.SetUniform1("fogEnd", fogEnd);
            _shader.SetUniform1("fogDensity", fogDensity);
            
            int modeInt = (int)fogModeVal;
            if (modeInt == (int)GLEnum.Linear) _shader.SetUniform1("fogMode", 0);
            else if (modeInt == (int)GLEnum.Exp) _shader.SetUniform1("fogMode", 1);
            else _shader.SetUniform1("fogMode", 2);

            _shader.SetUniform1("fogEnabled", 1);
        }

        _shader.SetUniformMatrix4("modelViewMatrix", modelView);
        _shader.SetUniformMatrix4("projectionMatrix", projection);

        for (int layer = 0; layer < 3; layer++)
        {
            ParticleBuffer buf = layers[layer];
            if (buf.Count == 0)
            {
                continue;
            }

            EnsureCapacity(buf.Count);

            int vertexIdx = 0;

            for (int i = 0; i < buf.Count; i++)
            {
                ref readonly ParticleTypeConfig config = ref ParticleTypeConfig.Configs[(int)buf.Type[i]];

                float rx = (float)(buf.PrevX[i] + (buf.X[i] - buf.PrevX[i]) * partialTick - interpX);
                float ry = (float)(buf.PrevY[i] + (buf.Y[i] - buf.PrevY[i]) * partialTick - interpY);
                float rz = (float)(buf.PrevZ[i] + (buf.Z[i] - buf.PrevZ[i]) * partialTick - interpZ);

                float scale = ComputeScale(config.Scale, buf, i, partialTick);
                float size = 0.1f * scale;

                float brightness = ComputeBrightness(config.Brightness, buf, i, partialTick, world);

                ComputeUVs(config.UV, buf.TextureIndex[i], buf.TexJitterX[i], buf.TexJitterY[i],
                    out float minU, out float maxU, out float minV, out float maxV);

                float rCol = buf.Red[i] * brightness;
                float gCol = buf.Green[i] * brightness;
                float bCol = buf.Blue[i] * brightness;
                float aCol = 1.0f;

                // 4 corners
                var v0 = new ParticleVertex(rx - cosYaw * size - upX * size, ry - cosPitch * size, rz - sinYaw * size - upZ * size, maxU, maxV, rCol, gCol, bCol, aCol);
                var v1 = new ParticleVertex(rx - cosYaw * size + upX * size, ry + cosPitch * size, rz - sinYaw * size + upZ * size, maxU, minV, rCol, gCol, bCol, aCol);
                var v2 = new ParticleVertex(rx + cosYaw * size + upX * size, ry + cosPitch * size, rz + sinYaw * size + upZ * size, minU, minV, rCol, gCol, bCol, aCol);
                var v3 = new ParticleVertex(rx + cosYaw * size - upX * size, ry - cosPitch * size, rz + sinYaw * size - upZ * size, minU, maxV, rCol, gCol, bCol, aCol);

                // Triangle 1
                _vertexData[vertexIdx++] = v0;
                _vertexData[vertexIdx++] = v1;
                _vertexData[vertexIdx++] = v2;

                // Triangle 2
                _vertexData[vertexIdx++] = v0;
                _vertexData[vertexIdx++] = v2;
                _vertexData[vertexIdx++] = v3;
            }

            UploadMesh(layer, new Span<ParticleVertex>(_vertexData, 0, vertexIdx));

            textureManager.BindTexture(textureManager.GetTextureId(s_layerTextures[layer]));
            
            _vertexArrays[layer].Bind();
            GLManager.GL.DrawArrays(GLEnum.Triangles, 0, (uint)vertexIdx);
        }

        VertexArray.Unbind();
        GLManager.GL.UseProgram(0);
    }

    public static void RenderSpecial(List<ISpecialParticle> specialParticles,
        double x, double y, double z,
        double lastTickX, double lastTickY, double lastTickZ,
        float partialTick)
    {
        if (specialParticles.Count == 0)
        {
            return;
        }

        double interpX = lastTickX + (x - lastTickX) * partialTick;
        double interpY = lastTickY + (y - lastTickY) * partialTick;
        double interpZ = lastTickZ + (z - lastTickZ) * partialTick;

        Tessellator t = Tessellator.instance;
        for (int i = 0; i < specialParticles.Count; i++)
        {
            specialParticles[i].Render(t, partialTick, interpX, interpY, interpZ);
        }
    }

    private static float ComputeScale(ScaleModel model, ParticleBuffer buf, int i, float partialTick)
    {
        float progress = ((float)buf.Age[i] + partialTick) / buf.MaxAge[i];

        return model switch
        {
            ScaleModel.Constant => buf.BaseScale[i],
            ScaleModel.GrowToFull => buf.BaseScale[i] * Math.Clamp(progress * 32.0f, 0.0f, 1.0f),
            ScaleModel.ShrinkHalf => buf.BaseScale[i] * (1.0f - progress * progress * 0.5f),
            ScaleModel.ShrinkSquared => buf.BaseScale[i] * (1.0f - progress * progress),
            ScaleModel.PortalEase => buf.BaseScale[i] * (1.0f - (1.0f - progress) * (1.0f - progress)),
            _ => buf.BaseScale[i],
        };
    }

    private static float ComputeBrightness(BrightnessModel model, ParticleBuffer buf, int i,
        float partialTick, IWorldContext world)
    {
        switch (model)
        {
            case BrightnessModel.AlwaysFull: return 1.0f;
            case BrightnessModel.FadeFromFull:
                {
                    float p = Math.Clamp(((float)buf.Age[i] + partialTick) / buf.MaxAge[i], 0.0f, 1.0f);
                    float worldBright = GetWorldBrightness(buf, i, world);
                    return worldBright * p + (1.0f - p);
                }
            case BrightnessModel.EaseToFull:
                {
                    float p = Math.Clamp(((float)buf.Age[i] + partialTick) / buf.MaxAge[i], 0.0f, 1.0f);
                    float worldBright = GetWorldBrightness(buf, i, world);
                    float ease = p * p * p * p; // Quartic ease-in
                    return worldBright * (1.0f - ease) + ease;
                }
            default: return GetWorldBrightness(buf, i, world); // WorldBased
        }
    }

    private static float GetWorldBrightness(ParticleBuffer buf, int i, IWorldContext world)
    {
        // World sampling uses the Floor of coordinates to find the specific block voxel
        int bx = MathHelper.Floor(buf.X[i]);
        int by = MathHelper.Floor(buf.Y[i]);
        int bz = MathHelper.Floor(buf.Z[i]);
        return world.Lighting.GetLuminance(bx, by, bz);
    }

    private static void ComputeUVs(UVModel model, int textureIndex, float jitterX, float jitterY,
        out float minU, out float maxU, out float minV, out float maxV)
    {
        // 0.999f is used to clamp UVs slightly inside the tile boundary to prevent texture bleeding at quad edges
        switch (model)
        {
            case UVModel.Jittered4x4:
                {
                    minU = ((textureIndex % 16) + jitterX / 4.0f) / 16.0f;
                    maxU = minU + 0.999f / 64.0f;
                    minV = ((textureIndex / 16) + jitterY / 4.0f) / 16.0f;
                    maxV = minV + 0.999f / 64.0f;
                    break;
                }
            case UVModel.Standard16x16:
            default:
                {
                    minU = (textureIndex % 16) / 16.0f;
                    maxU = minU + 0.999f / 16.0f;
                    minV = (textureIndex / 16) / 16.0f;
                    maxV = minV + 0.999f / 16.0f;
                    break;
                }
        }
    }
}
