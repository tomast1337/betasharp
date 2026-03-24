using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

public class BlockPistonExtension : Block
{
    private readonly int _pistonHeadSprite = -1;

    public BlockPistonExtension(int id, int textureId) : base(id, textureId, Material.Piston)
    {
        setSoundGroup(SoundStoneFootstep);
        SetHardness(0.5F);
    }

    public override void OnBreak(OnBreakEvent @event)
    {
        base.OnBreak(@event);
        int x = @event.X;
        int y = @event.Y;
        int z = @event.Z;
        int blockMeta = @event.World.Reader.GetBlockMeta(x, y, z);
        int var6 = PistonConstants.field_31057_a[getFacing(blockMeta)];
        x += PistonConstants.HEAD_OFFSET_X[var6];
        y += PistonConstants.HEAD_OFFSET_Y[var6];
        z += PistonConstants.HEAD_OFFSET_Z[var6];
        int blockId = @event.World.Reader.GetBlockId(x, y, z);
        if (blockId != Piston.Id && blockId != StickyPiston.Id)
        {
            return;
        }

        blockMeta = @event.World.Reader.GetBlockMeta(x, y, z);
        if (!BlockPistonBase.isExtended(blockMeta))
        {
            return;
        }

        Blocks[blockId]?.DropStacks(new OnDropEvent(@event.World, x, y, z, blockMeta));
        @event.World.Writer.SetBlock(x, y, z, 0);
    }

    public override int GetTexture(int side, int meta)
    {
        int var3 = getFacing(meta);
        return side == var3 ? _pistonHeadSprite >= 0 ? _pistonHeadSprite : (meta & 8) != 0 ? TextureId - 1 : TextureId : side == PistonConstants.field_31057_a[var3] ? 107 : 108;
    }

    public override BlockRendererType GetRenderType() => BlockRendererType.PistonExtension;

    public override bool IsOpaque() => false;

    public override bool IsFullCube() => false;

    public override bool CanPlaceAt(CanPlaceAtContext context) => false;

    public override int GetDroppedItemCount() => 0;

    public override void AddIntersectingBoundingBox(IBlockReader reader, EntityManager entities, int x, int y, int z, Box box, List<Box> boxes)
    {
        int var7 = reader.GetBlockMeta(x, y, z);
        switch (getFacing(var7))
        {
            case 0:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                SetBoundingBox(6.0F / 16.0F, 0.25F, 6.0F / 16.0F, 10.0F / 16.0F, 1.0F, 10.0F / 16.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                break;
            case 1:
                SetBoundingBox(0.0F, 12.0F / 16.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                SetBoundingBox(6.0F / 16.0F, 0.0F, 6.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F, 10.0F / 16.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                break;
            case 2:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                SetBoundingBox(0.25F, 6.0F / 16.0F, 0.25F, 12.0F / 16.0F, 10.0F / 16.0F, 1.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                break;
            case 3:
                SetBoundingBox(0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F, 1.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                SetBoundingBox(0.25F, 6.0F / 16.0F, 0.0F, 12.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                break;
            case 4:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                SetBoundingBox(6.0F / 16.0F, 0.25F, 0.25F, 10.0F / 16.0F, 12.0F / 16.0F, 1.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                break;
            case 5:
                SetBoundingBox(12.0F / 16.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                SetBoundingBox(0.0F, 6.0F / 16.0F, 0.25F, 12.0F / 16.0F, 10.0F / 16.0F, 12.0F / 16.0F);
                base.AddIntersectingBoundingBox(reader, entities, x, y, z, box, boxes);
                break;
        }

        SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
    }

    public override void UpdateBoundingBox(IBlockReader blockReader, EntityManager? entities, int x, int y, int z)
    {
        int var5 = blockReader.GetBlockMeta(x, y, z);
        switch (getFacing(var5))
        {
            case 0:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 0.25F, 1.0F);
                break;
            case 1:
                SetBoundingBox(0.0F, 12.0F / 16.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                break;
            case 2:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 1.0F, 1.0F, 0.25F);
                break;
            case 3:
                SetBoundingBox(0.0F, 0.0F, 12.0F / 16.0F, 1.0F, 1.0F, 1.0F);
                break;
            case 4:
                SetBoundingBox(0.0F, 0.0F, 0.0F, 0.25F, 1.0F, 1.0F);
                break;
            case 5:
                SetBoundingBox(12.0F / 16.0F, 0.0F, 0.0F, 1.0F, 1.0F, 1.0F);
                break;
        }
    }

    public override void NeighborUpdate(OnTickEvent @event)
    {
        int facing = getFacing(@event.World.Reader.GetBlockMeta(@event.X, @event.Y, @event.Z));
        int var7 = @event.World.Reader.GetBlockId(@event.X - PistonConstants.HEAD_OFFSET_X[facing], @event.Y - PistonConstants.HEAD_OFFSET_Y[facing], @event.Z - PistonConstants.HEAD_OFFSET_Z[facing]);
        if (var7 != Piston.Id && var7 != StickyPiston.Id)
        {
            @event.World.Writer.SetBlock(@event.X, @event.Y, @event.Z, 0);
        }
        else
        {
            Blocks[var7]!.NeighborUpdate(new OnTickEvent(@event.World,
                @event.X - PistonConstants.HEAD_OFFSET_X[facing],
                @event.Y - PistonConstants.HEAD_OFFSET_Y[facing],
                @event.Z - PistonConstants.HEAD_OFFSET_Z[facing],
                @event.World.Reader.GetBlockMeta(@event.X - PistonConstants.HEAD_OFFSET_X[facing],
                    @event.Y - PistonConstants.HEAD_OFFSET_Y[facing],
                    @event.Z - PistonConstants.HEAD_OFFSET_Z[facing]),
                Id
            ));
        }
    }

    public static int getFacing(int meta) => meta & 7;
}
