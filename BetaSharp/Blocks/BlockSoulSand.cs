using BetaSharp.Blocks.Materials;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks;

internal class BlockSoulSand(int id, int textureId) : Block(id, textureId, Material.Sand)
{
    public override Box? GetCollisionShape(IBlockReader world, EntityManager entities, int x, int y, int z)
    {
        const float height = 2.0F / 16.0F;
        return new Box(x, y, z, x + 1, y + 1 - height, z + 1);
    }

    public override void OnEntityCollision(OnEntityCollisionEvent @event)
    {
        @event.Entity.velocityX *= 0.4;
        @event.Entity.velocityZ *= 0.4;
    }
}
