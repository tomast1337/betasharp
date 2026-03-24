using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;

namespace BetaSharp.Blocks;

internal class BlockPumpkin : Block
{
    private readonly bool _lit;

    public BlockPumpkin(int id, int textureId, bool lit) : base(id, Material.Pumpkin)
    {
        this.TextureId = textureId;
        SetTickRandomly(true);
        _lit = lit;
    }

    public override int GetTexture(int side, int meta)
    {
        if (side is 1 or 0)
        {
            return TextureId;
        }

        int faceTexture = TextureId + 1 + 16;
        if (_lit)
        {
            ++faceTexture;
        }

        return meta == 2 && side == 2 ? faceTexture :
            meta == 3 && side == 5 ? faceTexture :
            meta == 0 && side == 3 ? faceTexture :
            meta == 1 && side == 4 ? faceTexture : TextureId + 16;
    }

    public override int GetTexture(int side) => side switch
    {
        1 => TextureId,
        0 => TextureId,
        3 => TextureId + 1 + 16,
        _ => TextureId + 16
    };

    public override bool CanPlaceAt(CanPlaceAtContext evt)
    {
        int blockId = evt.World.Reader.GetBlockId(evt.X, evt.Y, evt.Z);
        return (blockId == 0 || Blocks[blockId]!.Material.IsReplaceable) && evt.World.Reader.ShouldSuffocate(evt.X, evt.Y - 1, evt.Z);
    }

    public override void OnPlaced(OnPlacedEvent @event)
    {
        if (@event.Placer == null)
        {
            return;
        }

        int direction = MathHelper.Floor(@event.Placer.yaw * 4.0F / 360.0F + 2.5D) & 3;
        @event.World.Writer.SetBlockMeta(@event.X, @event.Y, @event.Z, direction);
    }
}
