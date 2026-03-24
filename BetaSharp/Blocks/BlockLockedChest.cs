using BetaSharp.Blocks.Materials;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockLockedChest : Block
{
    public BlockLockedChest(int id) : base(id, Material.Wood) => TextureId = 26;

    public override int GetTextureId(IBlockReader iBlockReader, int x, int y, int z, int side)
    {
        if (side is 1 or 0)
        {
            return TextureId - 1;
        }

        int var6 = iBlockReader.GetBlockId(x, y, z - 1);
        int var7 = iBlockReader.GetBlockId(x, y, z + 1);
        int var8 = iBlockReader.GetBlockId(x - 1, y, z);
        int var9 = iBlockReader.GetBlockId(x + 1, y, z);
        sbyte var10 = 3;
        if (BlocksOpaque[var6] && !BlocksOpaque[var7])
        {
            var10 = 3;
        }

        if (BlocksOpaque[var7] && !BlocksOpaque[var6])
        {
            var10 = 2;
        }

        if (BlocksOpaque[var8] && !BlocksOpaque[var9])
        {
            var10 = 5;
        }

        if (BlocksOpaque[var9] && !BlocksOpaque[var8])
        {
            var10 = 4;
        }

        return side == var10 ? TextureId + 1 : TextureId;
    }

    public override int GetTexture(int side) => side switch
    {
        1 or 0 => TextureId - 1,
        3 => TextureId + 1,
        _ => TextureId
    };

    public override bool CanPlaceAt(CanPlaceAtContext context) => true;

    public override void OnTick(OnTickEvent @event) => @event.World.Writer.SetBlock(@event.X, @event.Y, @event.Z, 0);
}
