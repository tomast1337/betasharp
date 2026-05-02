using BetaSharp.Blocks;
using BetaSharp.Blocks.Entities;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Blocks;
using BetaSharp.Client.Rendering.Blocks.Entities;
using BetaSharp.Client.Rendering.Chunks;
using BetaSharp.Client.Rendering.Core;
using BetaSharp.Client.Rendering.Core.Textures;
using BetaSharp.Client.Rendering.Entities;
using BetaSharp.Client.Rendering.Legacy;
using BetaSharp.Client.Rendering.Particles;
using BetaSharp.Entities;
using BetaSharp.Items;
using BetaSharp.Profiling;
using BetaSharp.Util;
using BetaSharp.Util.Hit;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Chunks;
using BetaSharp.Worlds.Core;
using Silk.NET.Maths;

namespace BetaSharp.Client.Rendering;

public class WorldRenderer : IWorldEventListener, IWorldRenderer
{
    public int CountEntitiesTotal { get; private set; }
    public int CountEntitiesRendered { get; private set; }
    public int CountEntitiesHidden { get; private set; }
    public float DamagePartialTime { get; set; }

    private IChunkRenderer? _chunkRenderer;
    private World? _world;
    private readonly IChunkRendererFactory _chunkRendererFactory;
    private readonly ITextureManager _textureManager;
    private readonly BetaSharp _game;
    private readonly ILegacyFixedFunctionApi _sceneRenderBackend;
    private int _cloudOffsetX;
    private readonly int _starGLCallList;
    private readonly int _glSkyList;
    private readonly int _glSkyList2;
    private int _glCloudsList = -1;
    private int _renderDistance = -1;
    private int _renderEntitiesStartupCounter = 2;

    public WorldRenderer(BetaSharp gameInstance, ITextureManager textureManager,
        IChunkRendererFactory chunkRendererFactory)
    {
        _game = gameInstance;
        _sceneRenderBackend = gameInstance.LegacyFixedFunctionApi;
        _textureManager = textureManager;
        _chunkRendererFactory = chunkRendererFactory;

        _starGLCallList = _sceneRenderBackend.GenerateDisplayLists(3);
        _sceneRenderBackend.PushMatrix();
        _sceneRenderBackend.BeginDisplayList(_starGLCallList);
        RenderStars();
        _sceneRenderBackend.EndDisplayList();
        _sceneRenderBackend.PopMatrix();
        Tessellator tessellator = Tessellator.instance;
        _glSkyList = _starGLCallList + 1;
        _sceneRenderBackend.BeginDisplayList(_glSkyList);
        byte skyPlaneStep = 64;
        int skyPlaneRadius = 256 / skyPlaneStep + 2;
        float skyPlaneY = 16.0F;

        int planeX;
        int planeZ;
        for (planeX = -skyPlaneStep * skyPlaneRadius; planeX <= skyPlaneStep * skyPlaneRadius; planeX += skyPlaneStep)
        {
            for (planeZ = -skyPlaneStep * skyPlaneRadius; planeZ <= skyPlaneStep * skyPlaneRadius; planeZ += skyPlaneStep)
            {
                tessellator.startDrawingQuads();
                tessellator.addVertex(planeX + 0, (double)skyPlaneY, planeZ + 0);
                tessellator.addVertex(planeX + skyPlaneStep, (double)skyPlaneY, planeZ + 0);
                tessellator.addVertex(planeX + skyPlaneStep, (double)skyPlaneY, planeZ + skyPlaneStep);
                tessellator.addVertex(planeX + 0, (double)skyPlaneY, planeZ + skyPlaneStep);
                tessellator.draw();
            }
        }

        _sceneRenderBackend.EndDisplayList();
        _glSkyList2 = _starGLCallList + 2;
        _sceneRenderBackend.BeginDisplayList(_glSkyList2);
        skyPlaneY = -16.0F;
        tessellator.startDrawingQuads();

        for (planeX = -skyPlaneStep * skyPlaneRadius; planeX <= skyPlaneStep * skyPlaneRadius; planeX += skyPlaneStep)
        {
            for (planeZ = -skyPlaneStep * skyPlaneRadius; planeZ <= skyPlaneStep * skyPlaneRadius; planeZ += skyPlaneStep)
            {
                tessellator.addVertex(planeX + skyPlaneStep, (double)skyPlaneY, planeZ + 0);
                tessellator.addVertex(planeX + 0, (double)skyPlaneY, planeZ + 0);
                tessellator.addVertex(planeX + 0, (double)skyPlaneY, planeZ + skyPlaneStep);
                tessellator.addVertex(planeX + skyPlaneStep, (double)skyPlaneY, planeZ + skyPlaneStep);
            }
        }

        tessellator.draw();
        _sceneRenderBackend.EndDisplayList();
        BuildCloudDisplayLists();
    }

    private static void RenderStars()
    {
        Random random = new(10842);
        Tessellator tessellator = Tessellator.instance;
        tessellator.startDrawingQuads();

        for (int starIndex = 0; starIndex < 1500; ++starIndex)
        {
            double dirX = (double)(random.NextDouble() * 2.0 - 1.0);
            double dirY = (double)(random.NextDouble() * 2.0 - 1.0);
            double dirZ = (double)(random.NextDouble() * 2.0 - 1.0);
            double starSize = (double)(0.25 + random.NextDouble() * 0.25);
            double dirLengthSq = dirX * dirX + dirY * dirY + dirZ * dirZ;
            if (dirLengthSq < 1.0 && dirLengthSq > 0.01)
            {
                dirLengthSq = 1.0 / Math.Sqrt(dirLengthSq);
                dirX *= dirLengthSq;
                dirY *= dirLengthSq;
                dirZ *= dirLengthSq;
                double starX = dirX * 100.0;
                double starY = dirY * 100.0;
                double starZ = dirZ * 100.0;
                double yaw = Math.Atan2(dirX, dirZ);
                double sinYaw = Math.Sin(yaw);
                double cosYaw = Math.Cos(yaw);
                double pitch = Math.Atan2(Math.Sqrt(dirX * dirX + dirZ * dirZ), dirY);
                double sinPitch = Math.Sin(pitch);
                double cosPitch = Math.Cos(pitch);
                double roll = random.NextDouble() * Math.PI * 2.0;
                double sinRoll = Math.Sin(roll);
                double cosRoll = Math.Cos(roll);

                for (int cornerIndex = 0; cornerIndex < 4; ++cornerIndex)
                {
                    double cornerY = 0.0D;
                    double cornerX = ((cornerIndex & 2) - 1) * starSize;
                    double cornerZ = ((cornerIndex + 1 & 2) - 1) * starSize;
                    double rotatedCornerX = cornerX * cosRoll - cornerZ * sinRoll;
                    double rotatedCornerZ = cornerZ * cosRoll + cornerX * sinRoll;
                    double pitchedCornerY = rotatedCornerX * sinPitch + cornerY * cosPitch;
                    double pitchedCornerX = cornerY * sinPitch - rotatedCornerX * cosPitch;
                    double finalX = pitchedCornerX * sinYaw - rotatedCornerZ * cosYaw;
                    double finalZ = rotatedCornerZ * sinYaw + pitchedCornerX * cosYaw;
                    tessellator.addVertex(starX + finalX, starY + pitchedCornerY, starZ + finalZ);
                }
            }
        }

        tessellator.draw();
    }

    public void ChangeWorld(World world)
    {
        _world?.EventListeners.Remove(this);

        _game.EntityRenderDispatcher.World = world;
        _world = world;
        if (world != null)
        {
            world.EventListeners.Add(this);
            LoadRenderers();
        }
        else
        {
            _chunkRenderer?.Dispose();
            _chunkRenderer = null;
        }
    }

    public void Tick(Entity view, float partialTicks)
    {
        if (view == null)
        {
            return;
        }

        double viewX = view.LastTickX + (view.X - view.LastTickX) * partialTicks;
        double viewY = view.LastTickY + (view.Y - view.LastTickY) * partialTicks;
        double viewZ = view.LastTickZ + (view.Z - view.LastTickZ) * partialTicks;
        _chunkRenderer?.Tick(new(viewX, viewY, viewZ));
    }

    public void LoadRenderers()
    {
        Block.Leaves.setGraphicsLevel(true);
        _renderDistance = _game.Options.renderDistance;

        _chunkRenderer?.Dispose();
        _chunkRenderer = _chunkRendererFactory.Create(_world!, () => _game.Options.AlternateBlocksEnabled,
            _game.FrameContext);

        _renderEntitiesStartupCounter = 2;
    }

    public bool TryGetChunkStats(out ChunkRendererStats stats)
    {
        if (_chunkRenderer == null)
        {
            stats = default;
            return false;
        }

        stats = new ChunkRendererStats(
            _chunkRenderer.TotalChunks,
            _chunkRenderer.ChunksInFrustum,
            _chunkRenderer.ChunksOccluded,
            _chunkRenderer.ChunksRendered,
            _chunkRenderer.TranslucentMeshes);
        return true;
    }

    public void MarkAllVisibleChunksDirty()
    {
        _chunkRenderer?.MarkAllVisibleChunksDirty();
    }

    public void SetChunkFogColor(float red, float green, float blue, float alpha)
    {
        _chunkRenderer?.SetFogColor(red, green, blue, alpha);
    }

    public void SetChunkFogMode(int mode)
    {
        _chunkRenderer?.SetFogMode(mode);
    }

    public void SetChunkFogDensity(float density)
    {
        _chunkRenderer?.SetFogDensity(density);
    }

    public void SetChunkFogStart(float start)
    {
        _chunkRenderer?.SetFogStart(start);
    }

    public void SetChunkFogEnd(float end)
    {
        _chunkRenderer?.SetFogEnd(end);
    }

    public void RenderEntities(Vec3D cameraPos, ICuller culler, float partialTicks)
    {
        if (_renderEntitiesStartupCounter > 0)
        {
            --_renderEntitiesStartupCounter;
        }
        else
        {
            _game.BlockEntityRenderDispatcher.CacheActiveRenderInfo(_world, _textureManager, _game.TextRenderer, _game.Camera, partialTicks);
            _game.EntityRenderDispatcher.CacheRenderInfo(_world, _textureManager, _game.TextRenderer, _game.Camera, _game.Options, _sceneRenderBackend, partialTicks);
            CountEntitiesTotal = 0;
            CountEntitiesRendered = 0;
            CountEntitiesHidden = 0;
            EntityLiving camera = _game.Camera;
            _game.EntityRenderDispatcher.OffsetX = camera.LastTickX + (camera.X - camera.LastTickX) * (double)partialTicks;
            _game.EntityRenderDispatcher.OffsetY = camera.LastTickY + (camera.Y - camera.LastTickY) * (double)partialTicks;
            _game.EntityRenderDispatcher.OffsetZ = camera.LastTickZ + (camera.Z - camera.LastTickZ) * (double)partialTicks;
            _game.BlockEntityRenderDispatcher.StaticPlayerX = camera.LastTickX + (camera.X - camera.LastTickX) * (double)partialTicks;
            _game.BlockEntityRenderDispatcher.StaticPlayerY = camera.LastTickY + (camera.Y - camera.LastTickY) * (double)partialTicks;
            _game.BlockEntityRenderDispatcher.StaticPlayerZ = camera.LastTickZ + (camera.Z - camera.LastTickZ) * (double)partialTicks;
            List<Entity> entities = _world.Entities.Entities;
            CountEntitiesTotal = entities.Count;

            int index;
            Entity entity;
            for (index = 0; index < _world.Entities.GlobalEntities.Count; ++index)
            {
                entity = _world.Entities.GlobalEntities[index];
                ++CountEntitiesRendered;
                if (entity.ShouldRender(cameraPos))
                {
                    _game.EntityRenderDispatcher.RenderEntity(entity, partialTicks);
                }
            }

            for (index = 0; index < entities.Count; ++index)
            {
                entity = entities[index];
                if (entities[index].Dead)
                {
                    if (entities[index] is EntityLiving living)
                    {
                        if (living.DeathTime >= 20)
                        {
                            entities.RemoveAt(index--);
                            continue;
                        }
                    }
                    else
                    {
                        entities.RemoveAt(index--);
                        continue;
                    }
                }
                if (entity.ShouldRender(cameraPos) && (entity.IgnoreFrustumCheck || culler.IsBoundingBoxInFrustum(entity.BoundingBox)) && (entity != _game.Camera || _game.Options.CameraMode != CameraMode.FirstPerson || _game.Camera.IsSleeping))
                {
                    int yFloor = MathHelper.Floor(entity.Y);
                    if (yFloor < 0)
                    {
                        yFloor = 0;
                    }
                    else if (yFloor >= ChuckFormat.WorldHeight)
                    {
                        yFloor = ChuckFormat.WorldHeight - 1;
                    }

                    if (_world.Reader.IsPosLoaded(MathHelper.Floor(entity.X), yFloor, MathHelper.Floor(entity.Z)))
                    {
                        ++CountEntitiesRendered;
                        _game.EntityRenderDispatcher.RenderEntity(entity, partialTicks);
                    }
                }
            }

            for (index = 0; index < _world.Entities.BlockEntities.Count; ++index)
            {
                BlockEntity blockEntity = _world.Entities.BlockEntities[index];
                if (!blockEntity.isRemoved() && culler.IsBoundingBoxInFrustum(new Box(blockEntity.X, blockEntity.Y, blockEntity.Z, blockEntity.X + 1, blockEntity.Y + 1, blockEntity.Z + 1)))
                {
                    _game.BlockEntityRenderDispatcher.RenderTileEntity(blockEntity, partialTicks);
                }
            }
        }
    }

    public int SortAndRender(EntityLiving entity, int pass, double partialTicks, ICuller culler)
    {
        if (_world == null || _chunkRenderer == null)
        {
            return 0;
        }

        if (_game.Options.renderDistance != _renderDistance)
        {
            LoadRenderers();
        }

        double viewX = entity.LastTickX + (entity.X - entity.LastTickX) * partialTicks;
        double viewY = entity.LastTickY + (entity.Y - entity.LastTickY) * partialTicks;
        double viewZ = entity.LastTickZ + (entity.Z - entity.LastTickZ) * partialTicks;

        Lighting.turnOff();

        ChunkRenderParams renderParams = new()
        {
            Camera = culler,
            ViewPos = new Vector3D<double>(viewX, viewY, viewZ),
            RenderDistance = _renderDistance,
            Ticks = _world.GetTime(),
            PartialTicks = (float)partialTicks,
            DeltaTime = _game.Timer.DeltaTime,
            EnvironmentAnimation = _game.Options.EnvironmentAnimation,
            ChunkFade = _game.Options.ChunkFade,
            RenderOccluded = false
        };

        if (pass == 0)
        {
            _chunkRenderer.Render(renderParams);
        }
        else
        {
            _chunkRenderer.RenderTransparent(renderParams);
        }

        return 0;
    }

    public void UpdateClouds()
    {
        ++_cloudOffsetX;
    }

    public void RenderSky(float tickDelta)
    {
        if (!_game.World.Dimension.IsNether)
        {
            _sceneRenderBackend.Disable(SceneRenderCapability.Texture2D);
            Vector3D<double> skyColor = _world.Environment.GetSkyColor(_game.Camera, tickDelta);
            float skyRed = (float)skyColor.X;
            float skyGreen = (float)skyColor.Y;
            float skyBlue = (float)skyColor.Z;
            float rainFade;
            float celestialAngle;


            _sceneRenderBackend.SetColorRgb(skyRed, skyGreen, skyBlue);
            Tessellator tessellator = Tessellator.instance;
            _sceneRenderBackend.SetDepthMask(false);
            _sceneRenderBackend.Enable(SceneRenderCapability.Fog);
            _sceneRenderBackend.SetColorRgb(skyRed, skyGreen, skyBlue);
            _sceneRenderBackend.CallDisplayList(_glSkyList);
            _sceneRenderBackend.Disable(SceneRenderCapability.Fog);
            _sceneRenderBackend.Disable(SceneRenderCapability.AlphaTest);
            _sceneRenderBackend.Enable(SceneRenderCapability.Blend);
            _sceneRenderBackend.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.OneMinusSrcAlpha);
            Lighting.turnOff();
            float[] backgroundColor = _world.Dimension.GetBackgroundColor(_world.GetTime(tickDelta), tickDelta);
            float sunriseRed;
            float sunriseGreen;
            float sunQuadSize;
            float starBrightness;
            if (backgroundColor != null)
            {
                _sceneRenderBackend.Disable(SceneRenderCapability.Texture2D);
                _sceneRenderBackend.SetShadeModel(SceneShadeModel.Smooth);
                _sceneRenderBackend.PushMatrix();
                _sceneRenderBackend.Rotate(90.0F, 1.0F, 0.0F, 0.0F);
                celestialAngle = _world.GetTime(tickDelta);
                _sceneRenderBackend.Rotate(celestialAngle > 0.5F ? 180.0F : 0.0F, 0.0F, 0.0F, 1.0F);
                sunriseRed = backgroundColor[0];
                sunriseGreen = backgroundColor[1];
                sunQuadSize = backgroundColor[2];
                float angle;

                tessellator.startDrawing(6);
                tessellator.setColorRGBA_F(sunriseRed, sunriseGreen, sunQuadSize, backgroundColor[3]);
                tessellator.addVertex(0.0D, 100.0D, 0.0D);
                byte segments = 16;
                tessellator.setColorRGBA_F(backgroundColor[0], backgroundColor[1], backgroundColor[2], 0.0F);

                for (int segment = 0; segment <= segments; ++segment)
                {
                    angle = segment * (float)Math.PI * 2.0F / segments;
                    float ringX = MathHelper.Sin(angle);
                    float ringY = MathHelper.Cos(angle);
                    tessellator.addVertex((double)(ringX * 120.0F), (double)(ringY * 120.0F), (double)(-ringY * 40.0F * backgroundColor[3]));
                }

                tessellator.draw();
                _sceneRenderBackend.PopMatrix();
                _sceneRenderBackend.SetShadeModel(SceneShadeModel.Flat);
            }

            _sceneRenderBackend.Enable(SceneRenderCapability.Texture2D);
            _sceneRenderBackend.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.One);
            _sceneRenderBackend.PushMatrix();
            rainFade = 1.0F - _world.Environment.GetRainGradient(tickDelta);
            celestialAngle = 0.0F;
            sunriseRed = 0.0F;
            sunriseGreen = 0.0F;
            _sceneRenderBackend.SetColor(1.0F, 1.0F, 1.0F, rainFade);
            _sceneRenderBackend.Translate(celestialAngle, sunriseRed, sunriseGreen);
            _sceneRenderBackend.Rotate(0.0F, 0.0F, 0.0F, 1.0F);
            _sceneRenderBackend.Rotate(_world.GetTime(tickDelta) * 360.0F, 1.0F, 0.0F, 0.0F);
            sunQuadSize = 30.0F;
            _textureManager.BindTexture(_textureManager.GetTextureId("/terrain/sun.png"));
            tessellator.startDrawingQuads();
            tessellator.addVertexWithUV((double)-sunQuadSize, 100.0D, (double)-sunQuadSize, 0.0D, 0.0D);
            tessellator.addVertexWithUV((double)sunQuadSize, 100.0D, (double)-sunQuadSize, 1.0D, 0.0D);
            tessellator.addVertexWithUV((double)sunQuadSize, 100.0D, (double)sunQuadSize, 1.0D, 1.0D);
            tessellator.addVertexWithUV((double)-sunQuadSize, 100.0D, (double)sunQuadSize, 0.0D, 1.0D);
            tessellator.draw();
            sunQuadSize = 20.0F;
            _textureManager.BindTexture(_textureManager.GetTextureId("/terrain/moon.png"));
            tessellator.startDrawingQuads();
            tessellator.addVertexWithUV((double)-sunQuadSize, -100.0D, (double)sunQuadSize, 1.0D, 1.0D);
            tessellator.addVertexWithUV((double)sunQuadSize, -100.0D, (double)sunQuadSize, 0.0D, 1.0D);
            tessellator.addVertexWithUV((double)sunQuadSize, -100.0D, (double)-sunQuadSize, 0.0D, 0.0D);
            tessellator.addVertexWithUV((double)-sunQuadSize, -100.0D, (double)-sunQuadSize, 1.0D, 0.0D);
            tessellator.draw();
            _sceneRenderBackend.Disable(SceneRenderCapability.Texture2D);
            starBrightness = _world.CalculateSkyLightIntensity(tickDelta) * rainFade;
            if (starBrightness > 0.0F)
            {
                _sceneRenderBackend.SetColor(starBrightness, starBrightness, starBrightness, starBrightness);
                _sceneRenderBackend.CallDisplayList(_starGLCallList);
            }

            _sceneRenderBackend.SetColor(1.0F, 1.0F, 1.0F, 1.0F);
            _sceneRenderBackend.Disable(SceneRenderCapability.Blend);
            _sceneRenderBackend.Enable(SceneRenderCapability.AlphaTest);
            _sceneRenderBackend.Enable(SceneRenderCapability.Fog);
            _sceneRenderBackend.PopMatrix();
            if (_world.Dimension.HasGround)
            {
                _sceneRenderBackend.SetColorRgb(skyRed * 0.2F + 0.04F, skyGreen * 0.2F + 0.04F, skyBlue * 0.6F + 0.1F);
            }
            else
            {
                _sceneRenderBackend.SetColorRgb(skyRed, skyGreen, skyBlue);
            }

            _sceneRenderBackend.Disable(SceneRenderCapability.Texture2D);
            _sceneRenderBackend.CallDisplayList(_glSkyList2);
            _sceneRenderBackend.Enable(SceneRenderCapability.Texture2D);
            _sceneRenderBackend.SetDepthMask(true);
        }
    }

    public void RenderClouds(float tickDelta)
    {
        using (Profiler.Begin("RenderClouds"))
        {
            if (!_game.World.Dimension.IsNether)
            {
                RenderCloudsFancy(tickDelta);
            }
        }
    }

    private void BuildCloudDisplayLists()
    {
        _glCloudsList = _sceneRenderBackend.GenerateDisplayLists(4);
        Tessellator tessellator = Tessellator.instance;
        int cloudAtlasPixels = 256;
        TextureHandle cloudHandle = _textureManager.GetTextureId("/environment/clouds.png");
        if (cloudHandle.Texture is { Width: > 0 })
        {
            cloudAtlasPixels = cloudHandle.Texture.Width;
        }

        for (int i = 0; i < 4; ++i)
        {
            _sceneRenderBackend.BeginDisplayList(_glCloudsList + i);
            tessellator.startDrawingQuads();
            float cloudHeight = 4.0F;
            float uvScale = 1.0F / cloudAtlasPixels;
            float edgeInset = 1.0F / 1024.0F;
            byte tileSize = 8;
            byte cloudRadius = 3;

            for (int tileX = -cloudRadius + 1; tileX <= cloudRadius; ++tileX)
            {
                for (int tileZ = -cloudRadius + 1; tileZ <= cloudRadius; ++tileZ)
                {
                    float uvX = tileX * tileSize;
                    float uvZ = tileZ * tileSize;
                    float x = uvX;
                    float z = uvZ;

                    if (i == 0)
                    {
                        tessellator.setNormal(0.0F, -1.0F, 0.0F);
                        tessellator.addVertexWithUV((double)(x + 0.0F), (double)(0.0F), (double)(z + tileSize), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + tileSize) * uvScale));
                        tessellator.addVertexWithUV((double)(x + tileSize), (double)(0.0F), (double)(z + tileSize), (double)((uvX + tileSize) * uvScale), (double)((uvZ + tileSize) * uvScale));
                        tessellator.addVertexWithUV((double)(x + tileSize), (double)(0.0F), (double)(z + 0.0F), (double)((uvX + tileSize) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                        tessellator.addVertexWithUV((double)(x + 0.0F), (double)(0.0F), (double)(z + 0.0F), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                    }

                    else if (i == 1)
                    {
                        tessellator.setNormal(0.0F, 1.0F, 0.0F);
                        tessellator.addVertexWithUV((double)(x + 0.0F), (double)(cloudHeight - edgeInset), (double)(z + tileSize), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + tileSize) * uvScale));
                        tessellator.addVertexWithUV((double)(x + tileSize), (double)(cloudHeight - edgeInset), (double)(z + tileSize), (double)((uvX + tileSize) * uvScale), (double)((uvZ + tileSize) * uvScale));
                        tessellator.addVertexWithUV((double)(x + tileSize), (double)(cloudHeight - edgeInset), (double)(z + 0.0F), (double)((uvX + tileSize) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                        tessellator.addVertexWithUV((double)(x + 0.0F), (double)(cloudHeight - edgeInset), (double)(z + 0.0F), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                    }

                    else if (i == 2)
                    {
                        if (tileX > -1)
                        {
                            tessellator.setNormal(-1.0F, 0.0F, 0.0F);
                            for (int edgeSlice = 0; edgeSlice < tileSize; ++edgeSlice)
                            {
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 0.0F), (double)(0.0F), (double)(z + tileSize), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + tileSize) * uvScale));
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 0.0F), (double)(cloudHeight), (double)(z + tileSize), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + tileSize) * uvScale));
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 0.0F), (double)(cloudHeight), (double)(z + 0.0F), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 0.0F), (double)(0.0F), (double)(z + 0.0F), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                            }
                        }
                        if (tileX <= 1)
                        {
                            tessellator.setNormal(1.0F, 0.0F, 0.0F);
                            for (int edgeSlice = 0; edgeSlice < tileSize; ++edgeSlice)
                            {
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 1.0F - edgeInset), (double)(0.0F), (double)(z + tileSize), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + tileSize) * uvScale));
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 1.0F - edgeInset), (double)(cloudHeight), (double)(z + tileSize), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + tileSize) * uvScale));
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 1.0F - edgeInset), (double)(cloudHeight), (double)(z + 0.0F), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + edgeSlice + 1.0F - edgeInset), (double)(0.0F), (double)(z + 0.0F), (double)((uvX + edgeSlice + 0.5F) * uvScale), (double)((uvZ + 0.0F) * uvScale));
                            }
                        }
                    }

                    else if (i == 3)
                    {
                        if (tileZ > -1)
                        {
                            tessellator.setNormal(0.0F, 0.0F, -1.0F);
                            for (int edgeSlice = 0; edgeSlice < tileSize; ++edgeSlice)
                            {
                                tessellator.addVertexWithUV((double)(x + 0.0F), (double)(cloudHeight), (double)(z + edgeSlice + 0.0F), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + tileSize), (double)(cloudHeight), (double)(z + edgeSlice + 0.0F), (double)((uvX + tileSize) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + tileSize), (double)(0.0F), (double)(z + edgeSlice + 0.0F), (double)((uvX + tileSize) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + 0.0F), (double)(0.0F), (double)(z + edgeSlice + 0.0F), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                            }
                        }
                        if (tileZ <= 1)
                        {
                            tessellator.setNormal(0.0F, 0.0F, 1.0F);
                            for (int edgeSlice = 0; edgeSlice < tileSize; ++edgeSlice)
                            {
                                tessellator.addVertexWithUV((double)(x + 0.0F), (double)(cloudHeight), (double)(z + edgeSlice + 1.0F - edgeInset), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + tileSize), (double)(cloudHeight), (double)(z + edgeSlice + 1.0F - edgeInset), (double)((uvX + tileSize) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + tileSize), (double)(0.0F), (double)(z + edgeSlice + 1.0F - edgeInset), (double)((uvX + tileSize) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                                tessellator.addVertexWithUV((double)(x + 0.0F), (double)(0.0F), (double)(z + edgeSlice + 1.0F - edgeInset), (double)((uvX + 0.0F) * uvScale), (double)((uvZ + edgeSlice + 0.5F) * uvScale));
                            }
                        }
                    }
                }
            }

            tessellator.draw();
            _sceneRenderBackend.EndDisplayList();
        }
    }

    private void RenderCloudsFancy(float tickDelta)
    {
        _sceneRenderBackend.Disable(SceneRenderCapability.CullFace);
        float cameraY = (float)(_game.Camera.LastTickY + (_game.Camera.Y - _game.Camera.LastTickY) * (double)tickDelta);
        float cloudScale = 12.0F;
        float cloudHeight = 4.0F;
        double cloudOffsetX = (_game.Camera.PrevX + (_game.Camera.X - _game.Camera.PrevX) * (double)tickDelta + (double)((_cloudOffsetX + tickDelta) * 0.03F)) / (double)cloudScale;
        double cloudOffsetZ = (_game.Camera.PrevZ + (_game.Camera.Z - _game.Camera.PrevZ) * (double)tickDelta) / (double)cloudScale + (double)0.33F;
        float cloudY = _world.Dimension.CloudHeight - cameraY + 0.33F;
        int cloudChunkX = MathHelper.Floor(cloudOffsetX / 2048.0D);
        int cloudChunkZ = MathHelper.Floor(cloudOffsetZ / 2048.0D);
        cloudOffsetX -= cloudChunkX * 2048;
        cloudOffsetZ -= cloudChunkZ * 2048;
        TextureHandle cloudTex = _textureManager.GetTextureId("/environment/clouds.png");
        _textureManager.BindTexture(cloudTex);
        _sceneRenderBackend.Enable(SceneRenderCapability.Blend);
        _sceneRenderBackend.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.OneMinusSrcAlpha);
        Vector3D<double> cloudColor = _world.Environment.GetCloudColor(tickDelta);
        float cloudRed = (float)cloudColor.X;
        float cloudGreen = (float)cloudColor.Y;
        float cloudBlue = (float)cloudColor.Z;

        int cloudAtlasPixels = cloudTex.Texture is { Width: > 0 } t ? t.Width : 256;
        float textureScale = 1f / cloudAtlasPixels;
        float textureOffsetU = MathHelper.Floor(cloudOffsetX) * textureScale;
        float textureOffsetV = MathHelper.Floor(cloudOffsetZ) * textureScale;
        float subCloudOffsetX = (float)(cloudOffsetX - MathHelper.Floor(cloudOffsetX));
        float subCloudOffsetZ = (float)(cloudOffsetZ - MathHelper.Floor(cloudOffsetZ));

        _sceneRenderBackend.Scale(cloudScale, 1.0F, cloudScale);

        for (int passIndex = 0; passIndex < 2; ++passIndex)
        {
            if (passIndex == 0)
            {
                _sceneRenderBackend.SetColorMask(false, false, false, false);
            }
            else
            {
                _sceneRenderBackend.SetColorMask(true, true, true, true);
            }

            _sceneRenderBackend.PushMatrix();
            _sceneRenderBackend.Translate(-subCloudOffsetX, cloudY, -subCloudOffsetZ);

            _sceneRenderBackend.SetMatrixMode(SceneMatrixMode.Texture);
            _sceneRenderBackend.PushMatrix();
            _sceneRenderBackend.Translate(textureOffsetU, textureOffsetV, 0.0F);
            _sceneRenderBackend.SetMatrixMode(SceneMatrixMode.Modelview);

            if (cloudY > -cloudHeight - 1.0F)
            {
                _sceneRenderBackend.SetColor(cloudRed * 0.7F, cloudGreen * 0.7F, cloudBlue * 0.7F, 0.8F);
                _sceneRenderBackend.CallDisplayList(_glCloudsList + 0); // Bottom
            }

            if (cloudY <= cloudHeight + 1.0F)
            {
                _sceneRenderBackend.SetColor(cloudRed, cloudGreen, cloudBlue, 0.8F);
                _sceneRenderBackend.CallDisplayList(_glCloudsList + 1); // Top
            }

            _sceneRenderBackend.SetColor(cloudRed * 0.9F, cloudGreen * 0.9F, cloudBlue * 0.9F, 0.8F);
            _sceneRenderBackend.CallDisplayList(_glCloudsList + 2); // Side X

            _sceneRenderBackend.SetColor(cloudRed * 0.8F, cloudGreen * 0.8F, cloudBlue * 0.8F, 0.8F);
            _sceneRenderBackend.CallDisplayList(_glCloudsList + 3); // Side Z

            _sceneRenderBackend.SetMatrixMode(SceneMatrixMode.Texture);
            _sceneRenderBackend.PopMatrix();
            _sceneRenderBackend.SetMatrixMode(SceneMatrixMode.Modelview);

            _sceneRenderBackend.PopMatrix();
        }

        _sceneRenderBackend.SetColor(1.0F, 1.0F, 1.0F, 1.0F);
        _sceneRenderBackend.Disable(SceneRenderCapability.Blend);
        _sceneRenderBackend.Enable(SceneRenderCapability.CullFace);
    }

    public void DrawBlockBreaking(EntityPlayer entityPlayer, HitResult hit, ItemStack? itemStack, float tickDelta)
    {
        if (DamagePartialTime <= 0.0F) return;

        Tessellator tessellator = Tessellator.instance;

        _sceneRenderBackend.PushMatrix();
        _sceneRenderBackend.Enable(SceneRenderCapability.Blend);
        _sceneRenderBackend.Enable(SceneRenderCapability.AlphaTest);
        _sceneRenderBackend.Enable(SceneRenderCapability.PolygonOffsetFill);

        _sceneRenderBackend.SetBlendFunction(SceneBlendFactor.DstColor, SceneBlendFactor.SrcColor);
        _sceneRenderBackend.SetColor(1.0F, 1.0F, 1.0F, 0.5F);
        _sceneRenderBackend.SetPolygonOffset(-3.0F, -50.0F);

        _textureManager.BindTexture(_textureManager.GetTextureId(TextureManager.TerrainLegacy2dTexturePath));

        int targetBlockId = _world.Reader.GetBlockId(hit.BlockX, hit.BlockY, hit.BlockZ);
        Block targetBlock = targetBlockId > 0 ? Block.Blocks[targetBlockId] : Block.Stone;

        double renderX = entityPlayer.LastTickX + (entityPlayer.X - entityPlayer.LastTickX) * (double)tickDelta;
        double renderY = entityPlayer.LastTickY + (entityPlayer.Y - entityPlayer.LastTickY) * (double)tickDelta;
        double renderZ = entityPlayer.LastTickZ + (entityPlayer.Z - entityPlayer.LastTickZ) * (double)tickDelta;

        tessellator.startDrawingQuads();
        tessellator.setTranslationD(-renderX, -renderY, -renderZ);
        tessellator.disableColor();

        int terrainAtlasTileSize =
            _game.FrameContext.Textures.GetAtlasTileSize(TextureManager.TerrainLegacy2dTexturePath);
        BlockRenderer.RenderBlockByRenderType(_world.Reader, _world.Lighting, targetBlock,
            new BlockPos(hit.BlockX, hit.BlockY, hit.BlockZ), tessellator, 240 + (int)(DamagePartialTime * 10.0F), true,
            _game.Options.AlternateBlocksEnabled, terrainAtlasTileSize, useArrayTextures: false);
        tessellator.draw();

        tessellator.setTranslationD(0.0D, 0.0D, 0.0D);
        _sceneRenderBackend.SetPolygonOffset(0.0F, 0.0F);
        _sceneRenderBackend.SetColor(1.0F, 1.0F, 1.0F, 1.0F);
        _sceneRenderBackend.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.OneMinusSrcAlpha);

        _sceneRenderBackend.Disable(SceneRenderCapability.PolygonOffsetFill);
        _sceneRenderBackend.Disable(SceneRenderCapability.AlphaTest);
        _sceneRenderBackend.Disable(SceneRenderCapability.Blend);
        _sceneRenderBackend.PopMatrix();
    }

    public void DrawSelectionBox(EntityPlayer player, HitResult hit, int renderPass, ItemStack itemStack, float tickDelta)
    {
        if (renderPass == 0 && hit.Type == HitResultType.TILE)
        {
            _sceneRenderBackend.Enable(SceneRenderCapability.Blend);
            _sceneRenderBackend.SetBlendFunction(SceneBlendFactor.SrcAlpha, SceneBlendFactor.OneMinusSrcAlpha);
            _sceneRenderBackend.SetColor(0.0F, 0.0F, 0.0F, 0.4F);
            _sceneRenderBackend.SetLineWidth(2.0F);
            _sceneRenderBackend.Disable(SceneRenderCapability.Texture2D);
            _sceneRenderBackend.SetDepthMask(false);
            float outlinePadding = 0.002F;
            int targetBlockId = _world.Reader.GetBlockId(hit.BlockX, hit.BlockY, hit.BlockZ);
            if (targetBlockId > 0)
            {
                Block.Blocks[targetBlockId].updateBoundingBox(_world.Reader, hit.BlockX, hit.BlockY, hit.BlockZ);
                double renderX = player.LastTickX + (player.X - player.LastTickX) * (double)tickDelta;
                double renderY = player.LastTickY + (player.Y - player.LastTickY) * (double)tickDelta;
                double renderZ = player.LastTickZ + (player.Z - player.LastTickZ) * (double)tickDelta;
                DrawOutlinedBoundingBox(Block.Blocks[targetBlockId].getBoundingBox(_world.Reader, _world.Entities, hit.BlockX, hit.BlockY, hit.BlockZ).Expand((double)outlinePadding, (double)outlinePadding, (double)outlinePadding).Offset(-renderX, -renderY, -renderZ));
            }

            _sceneRenderBackend.SetDepthMask(true);
            _sceneRenderBackend.Enable(SceneRenderCapability.Texture2D);
            _sceneRenderBackend.Disable(SceneRenderCapability.Blend);
        }
    }

    private static void DrawOutlinedBoundingBox(Box box)
    {
        Tessellator tessellator = Tessellator.instance;
        tessellator.startDrawing(3);
        tessellator.addVertex(box.MinX, box.MinY, box.MinZ);
        tessellator.addVertex(box.MaxX, box.MinY, box.MinZ);
        tessellator.addVertex(box.MaxX, box.MinY, box.MaxZ);
        tessellator.addVertex(box.MinX, box.MinY, box.MaxZ);
        tessellator.addVertex(box.MinX, box.MinY, box.MinZ);
        tessellator.draw();
        tessellator.startDrawing(3);
        tessellator.addVertex(box.MinX, box.MaxY, box.MinZ);
        tessellator.addVertex(box.MaxX, box.MaxY, box.MinZ);
        tessellator.addVertex(box.MaxX, box.MaxY, box.MaxZ);
        tessellator.addVertex(box.MinX, box.MaxY, box.MaxZ);
        tessellator.addVertex(box.MinX, box.MaxY, box.MinZ);
        tessellator.draw();
        tessellator.startDrawing(1);
        tessellator.addVertex(box.MinX, box.MinY, box.MinZ);
        tessellator.addVertex(box.MinX, box.MaxY, box.MinZ);
        tessellator.addVertex(box.MaxX, box.MinY, box.MinZ);
        tessellator.addVertex(box.MaxX, box.MaxY, box.MinZ);
        tessellator.addVertex(box.MaxX, box.MinY, box.MaxZ);
        tessellator.addVertex(box.MaxX, box.MaxY, box.MaxZ);
        tessellator.addVertex(box.MinX, box.MinY, box.MaxZ);
        tessellator.addVertex(box.MinX, box.MaxY, box.MaxZ);
        tessellator.draw();
    }

    public void MarkBlocksDirty(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        if (_chunkRenderer == null)
        {
            return;
        }

        int sectionSize = _chunkRenderer.SectionSize;
        int xStart = (int)Math.Floor((double)minX / sectionSize);
        int yStart = (int)Math.Floor((double)minY / sectionSize);
        int zStart = (int)Math.Floor((double)minZ / sectionSize);
        int xEnd = (int)Math.Ceiling((double)maxX / sectionSize);
        int yEnd = (int)Math.Ceiling((double)maxY / sectionSize);
        int zEnd = (int)Math.Ceiling((double)maxZ / sectionSize);

        for (int x = xStart; x <= xEnd; x++)
        {
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int z = zStart; z <= zEnd; z++)
                {
                    _chunkRenderer.MarkDirty(new Vector3D<int>(x, y, z) * sectionSize, true);
                }
            }
        }
    }

    public void BlockUpdate(int x, int y, int z)
    {
        MarkBlocksDirty(x - 1, y - 1, z - 1, x + 1, y + 1, z + 1);
    }

    public void SetBlocksDirty(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        if (!_world.BlockHost.IsRegionLoaded(minX, minY, minZ, maxX, maxY, maxZ))
        {
            return;
        }

        MarkBlocksDirty(minX - 1, minY - 1, minZ - 1, maxX + 1, maxY + 1, maxZ + 1);
    }

    public void PlayStreaming(string soundName, int x, int y, int z)
    {
        if (soundName != null)
        {
            _game.HUD.Chat.SetRecordPlaying(soundName);
        }

        _game.SoundManager.PlayStreaming(soundName, x, y, z, 1.0F, 1.0F);
    }

    public void PlaySound(string soundName, double x, double y, double z, float volume, float pitch)
    {
        float maxDistance = 16.0F;
        if (volume > 1.0F)
        {
            maxDistance *= volume;
        }

        if (_game.Camera.GetSquaredDistance(x, y, z) < (double)(maxDistance * maxDistance))
        {
            _game.SoundManager.PlaySound(soundName, (float)x, (float)y, (float)z, volume, pitch);
        }
    }

    public void SpawnParticle(string particleName, double x, double y, double z, double velocityX, double velocityY, double velocityZ)
    {
        if (_game != null && _game.Camera != null && _game.ParticleManager != null)
        {
            double cameraDx = _game.Camera.X - x;
            double cameraDy = _game.Camera.Y - y;
            double cameraDz = _game.Camera.Z - z;
            double maxDistance = 16.0D;
            if (cameraDx * cameraDx + cameraDy * cameraDy + cameraDz * cameraDz <= maxDistance * maxDistance)
            {
                IParticleManager pm = _game.ParticleManager;
                switch (particleName)
                {
                    case "bubble": pm.AddBubble(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "smoke": pm.AddSmoke(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "note": pm.AddNote(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "portal": pm.AddPortal(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "explode": pm.AddExplode(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "flame": pm.AddFlame(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "lava": pm.AddLava(x, y, z); break;
                    case "footstep": pm.AddFootstep(x, y, z); break;
                    case "splash": pm.AddSplash(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "largesmoke": pm.AddSmoke(x, y, z, velocityX, velocityY, velocityZ, 2.5f); break;
                    case "reddust": pm.AddReddust(x, y, z, (float)velocityX, (float)velocityY, (float)velocityZ); break;
                    case "snowballpoof": pm.AddSlime(x, y, z, Item.Snowball); break;
                    case "snowshovel": pm.AddSnowShovel(x, y, z, velocityX, velocityY, velocityZ); break;
                    case "slime": pm.AddSlime(x, y, z, Item.Slimeball); break;
                    case "heart": pm.AddHeart(x, y, z, velocityX, velocityY, velocityZ); break;
                }
            }
        }
    }

    public void NotifyEntityAdded(Entity entity)
    {
        entity.UpdateCloak();
        _game.EntityRenderDispatcher.SkinManager.RequestDownload((entity as EntityPlayer)?.Name);
    }

    public void NotifyEntityRemoved(Entity entity)
    {
    }

    public void NotifyAmbientDarknessChanged()
    {
        _chunkRenderer?.UpdateAllRenderers();
    }

    public void UpdateBlockEntity(int x, int y, int z, BlockEntity blockEntity)
    {
    }

    public void WorldEvent(EntityPlayer player, int eventId, int x, int y, int z, int data)
    {
        JavaRandom random = _world.Random;
        int blockId;
        switch (eventId)
        {
            case 1000:
                _game.SoundManager.PlaySound("random.click", x, y, z, 1.0F, 1.0F);
                break;
            case 1001:
                _game.SoundManager.PlaySound("random.click", x, y, z, 1.0F, 1.2F);
                break;
            case 1002:
                _game.SoundManager.PlaySound("random.bow", x, y, z, 1.0F, 1.2F);
                break;
            case 1003:
                if (Random.Shared.NextDouble() < 0.5D)
                {
                    _game.SoundManager.PlaySound("random.door_open", x + 0.5F, y + 0.5F, z + 0.5F, 1.0F, _world.Random.NextFloat() * 0.1F + 0.9F);
                }
                else
                {
                    _game.SoundManager.PlaySound("random.door_close", x + 0.5F, y + 0.5F, z + 0.5F, 1.0F, _world.Random.NextFloat() * 0.1F + 0.9F);
                }

                break;
            case 1004:
                _game.SoundManager.PlaySound("random.fizz", x + 0.5F, y + 0.5F, z + 0.5F, 0.5F, 2.6F + (random.NextFloat() - random.NextFloat()) * 0.8F);
                for (int particleIndex = 0; particleIndex < Random.Shared.Next(8, 12); ++particleIndex)
                {
                    _world.Broadcaster.AddParticle("largesmoke", x + random.NextDouble(), y + 1.2D, z + random.NextDouble(), 0.0D, 0.0D, 0.0D);
                }

                break;
            case 1005:
                if (Item.ITEMS[data] is ItemRecord)
                {
                    _game.SoundManager.PlayStreaming(((ItemRecord)Item.ITEMS[data]).recordName, x, y, z, 1.0F, 1.0F);
                }
                else
                {
                    _game.SoundManager.PlayStreaming(null, x, y, z, 1.0F, 1.0F);
                }

                break;
            case 2000:
                int offsetX = data % 3 - 1;
                int offsetZ = data / 3 % 3 - 1;
                double particleX = x + offsetX * 0.6D + 0.5D;
                double particleY = y + 0.5D;
                double particleZ = z + offsetZ * 0.6D + 0.5D;

                for (blockId = 0; blockId < 10; ++blockId)
                {
                    double speed = random.NextDouble() * 0.2D + 0.01D;
                    double smokeX = particleX + offsetX * 0.01D + (random.NextDouble() - 0.5D) * offsetZ * 0.5D;
                    double smokeY = particleY + (random.NextDouble() - 0.5D) * 0.5D;
                    double smokeZ = particleZ + offsetZ * 0.01D + (random.NextDouble() - 0.5D) * offsetX * 0.5D;
                    double velocityX = offsetX * speed + random.NextGaussian() * 0.01D;
                    double velocityY = -0.03D + random.NextGaussian() * 0.01D;
                    double velocityZ = offsetZ * speed + random.NextGaussian() * 0.01D;
                    SpawnParticle("smoke", smokeX, smokeY, smokeZ, velocityX, velocityY, velocityZ);
                }

                return;
            case 2001: // This is for breaking a block
                blockId = data & 255;
                if (blockId > 0)
                {
                    Block block = Block.Blocks[blockId];
                    _game.SoundManager.PlaySound(block.SoundGroup.BreakSound, x + 0.5F, y + 0.5F, z + 0.5F, (block.SoundGroup.Volume + 1.0F) / 2.0F, block.SoundGroup.Pitch * 0.8F);
                }

                _game.ParticleManager.addBlockDestroyEffects(x, y, z, data & 255, data >> 8 & 255);
                break;
        }
    }

    public void PlayNote(int x, int y, int z, int soundType, int pitch)
    {
    }

    public void BroadcastEntityEvent(Entity entity, byte @event)
    {
    }
}
