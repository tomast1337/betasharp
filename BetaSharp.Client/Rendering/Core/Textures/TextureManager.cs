using System.Buffers;
using BetaSharp.Client.Options;
using BetaSharp.Client.Rendering.Backends;
using BetaSharp.Client.Resource.Pack;
using BetaSharp.Registries.Data;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BetaSharp.Client.Rendering.Core.Textures;

public class TextureManager : ITextureManager
{
    /// <summary>
    /// Full 2D terrain atlas for fixed-function and inventory paths; chunk rendering uses <see cref="TerrainTexturePath"/> (2D array).
    /// </summary>
    public const string TerrainLegacy2dTexturePath = "/terrain.png##legacy2d";

    private const string TerrainTexturePath = "/terrain.png";
    private const string ItemsTexturePath = "/gui/items.png";

    private readonly ILogger _logger = Log.Instance.For<TextureManager>();
    private readonly Dictionary<string, TextureHandle> _textures = [];
    private readonly Dictionary<string, int[]> _colors = [];
    private readonly Dictionary<uint, (Image<Rgba32> Image, TextureHandle Handle)> _images = [];
    private readonly List<DynamicTexture> _dynamicTextures = [];
    private readonly Dictionary<string, int> _atlasTileSizes = [];
    private TextureHandle? _terrainHandle;
    private TextureHandle? _itemsHandle;
    private TextureArrayRegistry? _terrainArrayRegistry;
    private readonly GameOptions _gameOptions;
    private bool _clamp;
    private bool _blur;
    private readonly TexturePacks _texturePacks;
    private readonly BetaSharp _game;
    private readonly ITextureResourceFactory _textureResourceFactory;
    private readonly ITextureUploadService _textureUploadService;
    private readonly Image<Rgba32> _missingTextureImage = new(256, 256);
    private IRendererServices? _rendererServices;

    public TextureManager(
        BetaSharp game,
        TexturePacks texturePacks,
        GameOptions options,
        ITextureResourceFactory textureResourceFactory,
        ITextureUploadService textureUploadService)
    {
        _game = game;
        _texturePacks = texturePacks;
        _gameOptions = options;
        _textureResourceFactory = textureResourceFactory;
        _textureUploadService = textureUploadService;
        _missingTextureImage.Mutate(ctx =>
        {
            ctx.BackgroundColor(Color.Magenta);
            ctx.Fill(Color.Black, new RectangleF(0, 0, 128, 128));
            ctx.Fill(Color.Black, new RectangleF(128, 128, 128, 128));
        });
    }

    internal void SetRendererServices(IRendererServices rendererServices) => _rendererServices = rendererServices;

    /// <summary>Layer aliases for the terrain <c>TEXTURE_2D_ARRAY</c>; non-null after the first terrain array upload.</summary>
    public TextureArrayRegistry? TerrainTextureArrayRegistry => _terrainArrayRegistry;

    private void EnsureTerrainLegacy2dCopy(Image<Rgba32> terrainImage)
    {
        if (_textures.TryGetValue(TerrainLegacy2dTexturePath, out TextureHandle? existing))
        {
            if (existing.Texture != null)
            {
                Load(terrainImage, existing.Texture, false);
            }

            return;
        }

        ITextureResource tex2d = _textureResourceFactory.CreateTexture(TerrainLegacy2dTexturePath);
        var handle = new TextureHandle(tex2d);
        _textures[TerrainLegacy2dTexturePath] = handle;
        _atlasTileSizes[TerrainLegacy2dTexturePath] = terrainImage.Width / 16;
        Load(terrainImage, tex2d, false);
    }

    public int ActiveTextureCount => _textureResourceFactory.ActiveTextureCount;

    public int[] GetColors(string path)
    {
        if (_colors.TryGetValue(path, out int[]? cachedColors)) return cachedColors;
        try
        {
            using Image<Rgba32> img = LoadImageFromResource(path);
            int[] result = ReadColorsFromImage(img);
            _colors[path] = result;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get colors from image {Path}", path);
            int[] fallback = ReadColorsFromImage(_missingTextureImage);
            _colors[path] = fallback;
            return fallback;
        }
    }

    public int GetAtlasTileSize(string path)
    {
        if (_atlasTileSizes.TryGetValue(path, out int size)) return size;
        return 16;
    }

    public TextureHandle Load(Image<Rgba32> image)
    {
        ITextureResource texture = _textureResourceFactory.CreateTexture("Image_Direct");
        Load(image, texture, false);
        var handle = new TextureHandle(texture);
        _images[texture.Id] = (image, handle);
        return handle;
    }

    public TextureHandle GetTextureId(string path)
    {
        if (_textures.TryGetValue(path, out TextureHandle? handle)) return handle;

        bool legacyTerrain2d = string.Equals(path, TerrainLegacy2dTexturePath, StringComparison.Ordinal);
        bool terrainArray = path.Contains("terrain.png", StringComparison.Ordinal) && !legacyTerrain2d;

        ITextureResource texture = terrainArray
            ? _textureResourceFactory.CreateTexture(path, TextureTarget.Texture2DArray)
            : _textureResourceFactory.CreateTexture(path);
        handle = new TextureHandle(texture);
        _textures[path] = handle;

        try
        {
            using Image<Rgba32> img = LoadImageFromResource(legacyTerrain2d ? TerrainTexturePath : path);

            _atlasTileSizes[path] = img.Width / 16;

            if (legacyTerrain2d)
            {
                Load(img, texture, false);
            }
            else if (terrainArray)
            {
                Load(img, texture, true);
                EnsureTerrainLegacy2dCopy(img);
            }
            else
            {
                Load(img, texture, false);
            }

            return handle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get texture id for path {Path}", path);
            Load(_missingTextureImage, texture, false);
            return handle;
        }
    }

    public void Load(Image<Rgba32> image, ITextureResource texture, bool isTerrain)
    {
        texture.Bind();

        if (isTerrain)
        {
            byte[] contiguous = BuildLegacyTerrainArrayBuffer(image, out int tileSize);
            texture.AllocateTexture2DArrayStorage(tileSize, tileSize, TextureArrayRegistry.MaxTerrainLayers,
                TextureStorageFormat.Rgba8, 1);
            _textureUploadService.UploadSubImage3D(
                texture,
                0,
                0,
                0,
                tileSize,
                tileSize,
                256,
                contiguous,
                0,
                TextureDataFormat.Rgba);
            _terrainArrayRegistry ??= new TextureArrayRegistry();

            texture.SetFilter(TextureMinificationFilter.Nearest, TextureMagnificationFilter.Nearest);
            texture.SetMaxLevel(0);
            texture.SetWrap(TextureAddressMode.ClampToEdge, TextureAddressMode.ClampToEdge);

            float aniso = _gameOptions.AnisotropicLevel == 0 ? 1.0f : (float)Math.Pow(2, _gameOptions.AnisotropicLevel);
            aniso = Math.Clamp(aniso, 1.0f, GameOptions.MaxAnisotropy);
            texture.SetAnisotropicFilter(aniso);

            return;
        }

        texture.SetFilter(
            _blur ? TextureMinificationFilter.Linear : TextureMinificationFilter.Nearest,
            _blur ? TextureMagnificationFilter.Linear : TextureMagnificationFilter.Nearest);
        texture.SetWrap(
            _clamp ? TextureAddressMode.ClampToEdge : TextureAddressMode.Repeat,
            _clamp ? TextureAddressMode.ClampToEdge : TextureAddressMode.Repeat);

        byte[] rawPixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(rawPixels);
        _textureUploadService.Upload(
            texture,
            image.Width,
            image.Height,
            rawPixels,
            0,
            TextureDataFormat.Rgba,
            TextureStorageFormat.Rgba8);

        _clamp = false;
        _blur = false;
    }

    public void BindTexture(TextureHandle? handle)
    {
        handle?.Bind();
    }

    private Image<Rgba32> Rescale(Image<Rgba32> image)
    {
        int scale = image.Width / 16;
        var rescaled = new Image<Rgba32>(16, image.Height * scale);
        rescaled.Mutate(ctx =>
        {
            for (int i = 0; i < scale; i++)
            {
                using Image<Rgba32> frame = image.Clone(x =>
                    x.Crop(new SixLabors.ImageSharp.Rectangle(i * 16, 0, 16, image.Height)));
                ctx.DrawImage(frame, new SixLabors.ImageSharp.Point(0, i * image.Height), 1f);
            }
        });
        return rescaled;
    }

    private int[] ReadColorsFromImage(Image<Rgba32> image)
    {
        int[] argb = new int[image.Width * image.Height];
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgba32> row = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    Rgba32 p = row[x];
                    argb[y * accessor.Width + x] = (p.A << 24) | (p.R << 16) | (p.G << 8) | p.B;
                }
            }
        });
        return argb;
    }


    private Image<Rgba32> LoadImageFromResource(string path)
    {
        TexturePack pack = _texturePacks.SelectedTexturePack;

        if (path.StartsWith("##"))
        {
            using Stream? s = pack.GetResourceAsStream(path[2..]);
            return s == null ? _missingTextureImage.Clone() : Rescale(Image.Load<Rgba32>(s));
        }

        string cleanPath = path;
        while (true)
        {
            if (cleanPath.StartsWith("%clamp%"))
            {
                _clamp = true;
                cleanPath = cleanPath[7..];
            }
            else if (cleanPath.StartsWith("%blur%"))
            {
                _blur = true;
                cleanPath = cleanPath[6..];
            }
            else break;
        }

        using Stream? stream = pack.GetResourceAsStream(cleanPath);
        Image<Rgba32> img = stream == null ? _missingTextureImage.Clone() : Image.Load<Rgba32>(stream);

        return img;
    }


    public void Bind(int[] packedARGB, int width, int height, ITextureResource texture)
    {
        //TODO: this is potentially wrong but shouldn't crash

        texture.Bind();

        texture.SetFilter(
            _blur ? TextureMinificationFilter.Linear : TextureMinificationFilter.Nearest,
            _blur ? TextureMagnificationFilter.Linear : TextureMagnificationFilter.Nearest);
        texture.SetWrap(
            _clamp ? TextureAddressMode.ClampToEdge : TextureAddressMode.Repeat,
            _clamp ? TextureAddressMode.ClampToEdge : TextureAddressMode.Repeat);

        byte[] unpackedRGBA = new byte[width * height * 4];

        for (int i = 0; i < packedARGB.Length; ++i)
        {
            int a = packedARGB[i] >> 24 & 255;
            int r = packedARGB[i] >> 16 & 255;
            int g = packedARGB[i] >> 8 & 255;
            int b = packedARGB[i] & 255;

            unpackedRGBA[i * 4 + 0] = (byte)r;
            unpackedRGBA[i * 4 + 1] = (byte)g;
            unpackedRGBA[i * 4 + 2] = (byte)b;
            unpackedRGBA[i * 4 + 3] = (byte)a;
        }

        _textureUploadService.UploadSubImage(
            texture,
            0,
            0,
            width,
            height,
            unpackedRGBA,
            0,
            TextureDataFormat.Rgba);
    }

    public void Delete(ITextureResource texture)
    {
        foreach (KeyValuePair<string, TextureHandle> entry in _textures)
        {
            if (entry.Value.Texture == texture)
            {
                _textures.Remove(entry.Key);
                break;
            }
        }

        _images.Remove(texture.Id);
        texture.Dispose();
    }

    public void Delete(TextureHandle handle)
    {
        if (handle.Texture != null) Delete(handle.Texture);
    }

    public void UploadSubImage(
        TextureHandle handle,
        int x,
        int y,
        int width,
        int height,
        ReadOnlySpan<byte> pixelData,
        int level = 0,
        TextureDataFormat format = TextureDataFormat.Rgba)
    {
        if (handle.Texture == null)
        {
            return;
        }

        _textureUploadService.UploadSubImage(handle.Texture, x, y, width, height, pixelData, level, format);
    }


    public void AddDynamicTexture(DynamicTexture t)
    {
        _dynamicTextures.Add(t);
        if (_rendererServices != null)
        {
            t.Setup(_rendererServices);
        }

        t.tick();

        _terrainHandle = null;
        _itemsHandle = null;
    }

    public void Reload()
    {
        _atlasTileSizes.Clear();
        _terrainArrayRegistry = null;
        foreach (KeyValuePair<string, TextureHandle> entry in _textures)
        {
            entry.Value.Texture?.Dispose();

            bool legacy2d = string.Equals(entry.Key, TerrainLegacy2dTexturePath, StringComparison.Ordinal);
            bool reloadTerrain = entry.Key.Contains("terrain.png", StringComparison.Ordinal) && !legacy2d;
            ITextureResource newTexture = reloadTerrain
                ? _textureResourceFactory.CreateTexture(entry.Key, TextureTarget.Texture2DArray)
                : _textureResourceFactory.CreateTexture(entry.Key);
            entry.Value.Texture = newTexture;

            try
            {
                using Image<Rgba32> img = LoadImageFromResource(legacy2d ? TerrainTexturePath : entry.Key);
                _atlasTileSizes[entry.Key] = img.Width / 16;
                if (legacy2d)
                {
                    Load(img, newTexture, false);
                }
                else if (reloadTerrain)
                {
                    Load(img, newTexture, true);
                    EnsureTerrainLegacy2dCopy(img);
                }
                else
                {
                    Load(img, newTexture, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reload texture {Path}", entry.Key);
                _atlasTileSizes[entry.Key] = _missingTextureImage.Width / 16;
                Load(_missingTextureImage, newTexture, false);
            }
        }

        var oldImages = new Dictionary<uint, (Image<Rgba32> Image, TextureHandle Handle)>(_images);
        _images.Clear();
        foreach (KeyValuePair<uint, (Image<Rgba32> Image, TextureHandle Handle)> entry in oldImages)
        {
            entry.Value.Handle.Texture?.Dispose();

            ITextureResource newTexture = _textureResourceFactory.CreateTexture(entry.Value.Handle.Source);
            entry.Value.Handle.Texture = newTexture;
            Load(entry.Value.Image, newTexture, false);
            _images[newTexture.Id] = entry.Value;
        }

        foreach (string key in new List<string>(_colors.Keys)) GetColors(key);

        if (_rendererServices != null)
        {
            foreach (DynamicTexture dynamicTexture in _dynamicTextures)
            {
                dynamicTexture.Setup(_rendererServices);
            }
        }

        _terrainHandle = null;
        _itemsHandle = null;
    }

    public void Tick()
    {
        _terrainHandle ??= GetTextureHandleOrLoad(TerrainTexturePath);
        _itemsHandle ??= GetTextureHandleOrLoad(ItemsTexturePath);

        foreach (DynamicTexture texture in _dynamicTextures)
        {
            texture.tick();

            TextureHandle atlasHandle = texture.Atlas == DynamicTexture.FxImage.Terrain
                ? _terrainHandle
                : _itemsHandle;

            ITextureResource? atlasTexture = atlasHandle?.Texture;
            if (atlasTexture == null) continue;

            bool terrainArray = atlasTexture.Depth > 1;
            int targetTileSize = terrainArray ? atlasTexture.Width : atlasTexture.Width / 16;

            int fxSize = (int)Math.Sqrt(texture.Pixels.Length / 4);
            int scale = targetTileSize / fxSize;
            if (scale < 1) scale = 1;

            byte[] uploadPixels = texture.Pixels;
            int uploadSize = fxSize;
            byte[]? rentedArray = null;

            try
            {
                if (scale > 1)
                {
                    uploadSize = fxSize * scale;
                    rentedArray = System.Buffers.ArrayPool<byte>.Shared.Rent(uploadSize * uploadSize * 4);
                    UpscaleNearestNeighbor(texture.Pixels, rentedArray, fxSize, uploadSize, scale);
                    uploadPixels = rentedArray;
                }

                int finalReplicate = texture.Replicate;
                ReadOnlySpan<byte> uploadPixelSpan = uploadPixels.AsSpan(0, uploadSize * uploadSize * 4);

                if (terrainArray && texture.Atlas == DynamicTexture.FxImage.Terrain)
                {
                    int baseCol = texture.Sprite % 16;
                    int baseRow = texture.Sprite / 16;

                    for (int rx = 0; rx < finalReplicate; rx++)
                    {
                        for (int ry = 0; ry < finalReplicate; ry++)
                        {
                            int layerCol = baseCol + rx;
                            int layerRow = baseRow + ry;
                            if (layerCol is < 0 or > 15 || layerRow is < 0 or > 15)
                            {
                                continue;
                            }

                            int layer = layerRow * 16 + layerCol;
                            _textureUploadService.UploadSubImage3D(
                                atlasTexture,
                                0,
                                0,
                                layer,
                                uploadSize,
                                uploadSize,
                                1,
                                uploadPixelSpan,
                                0,
                                TextureDataFormat.Rgba);
                        }
                    }
                }
                else
                {
                    int tileX = (texture.Sprite % 16) * targetTileSize;
                    int tileY = (texture.Sprite / 16) * targetTileSize;

                    for (int x = 0; x < finalReplicate; x++)
                    {
                        for (int y = 0; y < finalReplicate; y++)
                        {
                            _textureUploadService.UploadSubImage(
                                atlasTexture,
                                tileX + (x * uploadSize),
                                tileY + (y * uploadSize),
                                uploadSize,
                                uploadSize,
                                uploadPixelSpan,
                                0,
                                TextureDataFormat.Rgba);
                        }
                    }

                    if (texture.Atlas == DynamicTexture.FxImage.Terrain &&
                        _gameOptions.UseMipmaps)
                    {
                        for (int x = 0; x < finalReplicate; x++)
                        {
                            for (int y = 0; y < finalReplicate; y++)
                            {
                                UpdateTileMipmaps(
                                    tileX + (x * uploadSize),
                                    tileY + (y * uploadSize),
                                    uploadSize,
                                    targetTileSize,
                                    uploadPixels,
                                    atlasTexture);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (rentedArray != null)
                {
                    System.Buffers.ArrayPool<byte>.Shared.Return(rentedArray);
                }
            }
        }
    }

    private TextureHandle GetTextureHandleOrLoad(string path)
    {
        foreach (KeyValuePair<string, TextureHandle> entry in _textures)
        {
            if (entry.Key.EndsWith(path, StringComparison.Ordinal))
            {
                return entry.Value;
            }
        }

        return GetTextureId(path);
    }

    private static void UpscaleNearestNeighbor(byte[] src, byte[] dst, int srcSize, int dstSize, int scale)
    {
        ReadOnlySpan<byte> srcSpan = src;
        Span<byte> dstSpan = dst;

        for (int y = 0; y < dstSize; y++)
        {
            int srcY = y / scale;
            for (int x = 0; x < dstSize; x++)
            {
                int srcX = x / scale;
                int srcIdx = (srcY * srcSize + srcX) * 4;
                int dstIdx = (y * dstSize + x) * 4;

                dstSpan[dstIdx] = srcSpan[srcIdx];
                dstSpan[dstIdx + 1] = srcSpan[srcIdx + 1];
                dstSpan[dstIdx + 2] = srcSpan[srcIdx + 2];
                dstSpan[dstIdx + 3] = srcSpan[srcIdx + 3];
            }
        }
    }

    private void UpdateTileMipmaps(
        int baseX,
        int baseY,
        int dataSize,
        int targetTileSize,
        byte[] tileData,
        ITextureResource texture)
    {
        int maxMipLevels = (int)Math.Log2(targetTileSize) + 1;
        byte[] currentData = tileData;
        int currentSize = dataSize;

        for (int mipLevel = 1; mipLevel < maxMipLevels; mipLevel++)
        {
            int newSize = currentSize >> 1;
            if (newSize < 1) newSize = 1;

            byte[] downsampled = ArrayPool<byte>.Shared.Rent(newSize * newSize * 4);

            try
            {
                if (currentSize > 1)
                {
                    for (int y = 0; y < newSize; y++)
                    {
                        for (int x = 0; x < newSize; x++)
                        {
                            int src0 = ((y * 2) * currentSize + (x * 2)) * 4;
                            int src1 = ((y * 2) * currentSize + (x * 2 + 1)) * 4;
                            int src2 = ((y * 2 + 1) * currentSize + (x * 2)) * 4;
                            int src3 = ((y * 2 + 1) * currentSize + (x * 2 + 1)) * 4;

                            int dst = (y * newSize + x) * 4;

                            downsampled[dst] = (byte)((currentData[src0] + currentData[src1] + currentData[src2] +
                                                       currentData[src3]) >> 2);
                            downsampled[dst + 1] = (byte)((currentData[src0 + 1] + currentData[src1 + 1] +
                                                           currentData[src2 + 1] + currentData[src3 + 1]) >> 2);
                            downsampled[dst + 2] = (byte)((currentData[src0 + 2] + currentData[src1 + 2] +
                                                           currentData[src2 + 2] + currentData[src3 + 2]) >> 2);
                            downsampled[dst + 3] = (byte)((currentData[src0 + 3] + currentData[src1 + 3] +
                                                           currentData[src2 + 3] + currentData[src3 + 3]) >> 2);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++) downsampled[i] = currentData[i];
                }

                int mipX = baseX >> mipLevel;
                int mipY = baseY >> mipLevel;
                _textureUploadService.UploadSubImage(
                    texture,
                    mipX,
                    mipY,
                    newSize,
                    newSize,
                    downsampled.AsSpan(0, newSize * newSize * 4),
                    mipLevel,
                    TextureDataFormat.Rgba);

                if (mipLevel > 1)
                {
                    ArrayPool<byte>.Shared.Return(currentData);
                }

                currentData = downsampled;
                currentSize = newSize;
            }
            catch
            {
                ArrayPool<byte>.Shared.Return(downsampled);
                throw;
            }
        }

        if (currentData != tileData)
        {
            ArrayPool<byte>.Shared.Return(currentData);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach (TextureHandle handle in _textures.Values)
        {
            handle.Texture?.Dispose();
        }

        _textures.Clear();

        foreach ((Image<Rgba32> Image, TextureHandle Handle) entry in _images.Values)
        {
            entry.Handle.Texture?.Dispose();
            entry.Image.Dispose();
        }

        _images.Clear();

        _missingTextureImage.Dispose();
        _colors.Clear();
        _dynamicTextures.Clear();
        _terrainArrayRegistry = null;
    }

    /// <summary>Copies the 16×16 legacy terrain atlas into 256 contiguous RGBA8 tiles (layer-major) for array upload.</summary>
    private static byte[] BuildLegacyTerrainArrayBuffer(Image<Rgba32> atlas, out int tileSize)
    {
        if (atlas.Width != atlas.Height)
        {
            throw new ArgumentException("Terrain atlas must be square.", nameof(atlas));
        }

        if (atlas.Width % 16 != 0)
        {
            throw new ArgumentException("Terrain atlas width must be divisible by 16.", nameof(atlas));
        }

        int ts = atlas.Width / 16;
        tileSize = ts;
        const int layerCount = 256;
        int bytesPerTile = ts * ts * 4;
        var layers = new byte[layerCount][];

        for (int i = 0; i < layerCount; i++)
        {
            layers[i] = new byte[bytesPerTile];
        }

        atlas.ProcessPixelRows(accessor =>
        {
            for (int row = 0; row < 16; row++)
            {
                for (int col = 0; col < 16; col++)
                {
                    int layer = col + row * 16;
                    byte[] dst = layers[layer];
                    int baseX = col * ts;
                    int baseY = row * ts;

                    for (int ty = 0; ty < ts; ty++)
                    {
                        Span<Rgba32> srcRow = accessor.GetRowSpan(baseY + ty);
                        int dstOfs = ty * ts * 4;

                        for (int tx = 0; tx < ts; tx++)
                        {
                            Rgba32 p = srcRow[baseX + tx];
                            int o = dstOfs + tx * 4;
                            dst[o] = p.R;
                            dst[o + 1] = p.G;
                            dst[o + 2] = p.B;
                            dst[o + 3] = p.A;
                        }
                    }
                }
            }
        });

        byte[] result = new byte[checked(layerCount * bytesPerTile)];

        for (int layer = 0; layer < layerCount; layer++)
        {
            System.Buffer.BlockCopy(layers[layer], 0, result, layer * bytesPerTile, bytesPerTile);
        }

        return result;
    }
}
