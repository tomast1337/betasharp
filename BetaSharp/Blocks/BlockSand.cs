using BetaSharp.Blocks.Materials;
using BetaSharp.Entities;

namespace BetaSharp.Blocks;

internal class BlockSand(int id, int textureId) : Block(id, textureId, Material.Sand)
{
    private static readonly ThreadLocal<bool> s_fallInstantly = new(() => false);

    public static bool FallInstantly
    {
        get => s_fallInstantly.Value;
        set => s_fallInstantly.Value = value;
    }

    public override void OnPlaced(OnPlacedEvent ctx) => ctx.World.TickScheduler.ScheduleBlockUpdate(ctx.X, ctx.Y, ctx.Z, Id, GetTickRate());

    public override void NeighborUpdate(OnTickEvent ctx) => ctx.World.TickScheduler.ScheduleBlockUpdate(ctx.X, ctx.Y, ctx.Z, Id, GetTickRate());

    public override void OnTick(OnTickEvent @event) => processFall(@event);

    private void processFall(OnTickEvent @event)
    {
        int x = @event.X;
        int y = @event.Y;
        int z = @event.Z;
        if (y <= 0 || !canFallThrough(new OnTickEvent(@event.World, x, y - 1, z, 0, @event.BlockId)))
        {
            return;
        }

        const sbyte checkRadius = 32;
        if (!FallInstantly && @event.World.ChunkHost.IsRegionLoaded(x - checkRadius, y - checkRadius, z - checkRadius, x + checkRadius, y + checkRadius, z + checkRadius))
        {
            EntityFallingSand fallingSand = new(@event.World, x + 0.5F, y + 0.5F, z + 0.5F, Id);
            @event.World.Entities.SpawnEntity(fallingSand);
        }
        else
        {
            @event.World.Writer.SetBlock(x, y, z, 0);

            while (canFallThrough(new OnTickEvent(@event.World, x, y - 1, z, 0, @event.BlockId)) && y > 0)
            {
                --y;
            }

            if (y > 0)
            {
                @event.World.Writer.SetBlock(x, y, z, Id);
            }
        }
    }

    public override int GetTickRate() => 3;

    public static bool canFallThrough(OnTickEvent ctx)
    {
        int blockId = ctx.World.Reader.GetBlockId(ctx.X, ctx.Y, ctx.Z);
        if (blockId == 0)
        {
            return true;
        }

        if (blockId == Fire.Id)
        {
            return true;
        }

        Material? material = Blocks[blockId]?.Material;
        return material == Material.Water || material == Material.Lava;
    }
}
