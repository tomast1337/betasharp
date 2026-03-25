using BetaSharp.Entities;
using BetaSharp.NBT;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds.Core.Systems;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityPiston : BlockEntity
{
    private readonly bool _source;
    private bool _extending;
    private Side _facing;
    private float _lastProgress;
    private float _progress;
    private int _pushedBlockData;
    private int _pushedBlockId;

    public BlockEntityPiston()
    {
    }

    public BlockEntityPiston(int pushedBlockId, int pushedBlockData, Side facing, bool extending, bool source)
    {
        _pushedBlockId = pushedBlockId;
        _pushedBlockData = pushedBlockData;
        _facing = facing;
        _extending = extending;
        _source = source;
    }

    public override BlockEntityType Type => Piston;

    public int GetPushedBlockId() => _pushedBlockId;

    public override int GetPushedBlockData() => _pushedBlockData;

    public bool IsExtending() => _extending;

    public Side GetFacing() => _facing;

    public bool IsSource() => _source;

    public float GetProgress(float tickDelta)
    {
        if (tickDelta > 1.0F)
        {
            tickDelta = 1.0F;
        }

        return _progress + (_lastProgress - _progress) * tickDelta;
    }

    public float GetRenderOffsetX(float tickDelta) => _extending ? (GetProgress(tickDelta) - 1.0F) * PistonConstants.HeadOffsetX(_facing) : (1.0F - GetProgress(tickDelta)) * PistonConstants.HeadOffsetX(_facing);

    public float GetRenderOffsetY(float tickDelta) => _extending ? (GetProgress(tickDelta) - 1.0F) * PistonConstants.HeadOffsetY(_facing) : (1.0F - GetProgress(tickDelta)) * PistonConstants.HeadOffsetY(_facing);

    public float GetRenderOffsetZ(float tickDelta) => _extending ? (GetProgress(tickDelta) - 1.0F) * PistonConstants.HeadOffsetZ(_facing) : (1.0F - GetProgress(tickDelta)) * PistonConstants.HeadOffsetZ(_facing);

    private void PushEntities(EntityManager entities, float collisionShapeSizeMultiplier, float entityMoveMultiplier)
    {
        if (!_extending)
        {
            --collisionShapeSizeMultiplier;
        }
        else
        {
            collisionShapeSizeMultiplier = 1.0F - collisionShapeSizeMultiplier;
        }

        Box? pushCollisionBox = Block.MovingPiston.GetPushedBlockCollisionShape(World.Reader, entities, X, Y, Z, _pushedBlockId, collisionShapeSizeMultiplier, _facing);
        if (pushCollisionBox == null)
        {
            return;
        }

        List<Entity> entitiesToPush = World.Entities.GetEntities(null!, pushCollisionBox.Value);
        if (entitiesToPush.Count <= 0)
        {
            return;
        }

        List<Entity> pushedEntities = [];
        pushedEntities.AddRange(entitiesToPush);
        foreach (Entity entity in pushedEntities)
        {
            entity.move(
                entityMoveMultiplier * PistonConstants.HeadOffsetX(_facing),
                entityMoveMultiplier * PistonConstants.HeadOffsetY(_facing),
                entityMoveMultiplier * PistonConstants.HeadOffsetZ(_facing)
            );
        }

        pushedEntities.Clear();
    }

    public void Finish()
    {
        if (!(_progress < 1.0F))
        {
            return;
        }

        _progress = _lastProgress = 1.0F;
        World.Entities.RemoveBlockEntity(X, Y, Z);
        MarkRemoved();
        if (World.Reader.GetBlockId(X, Y, Z) == Block.MovingPiston.Id)
        {
            World.Writer.SetBlock(X, Y, Z, _pushedBlockId, _pushedBlockData);
        }
    }

    public override void Tick(EntityManager entities)
    {
        _progress = _lastProgress;
        if (_progress >= 1.0F)
        {
            PushEntities(entities, 1.0F, 0.25F);
            World.Entities.RemoveBlockEntity(X, Y, Z);
            MarkRemoved();
            if (World.Reader.GetBlockId(X, Y, Z) == Block.MovingPiston.Id)
            {
                World.Writer.SetBlock(X, Y, Z, _pushedBlockId, _pushedBlockData);
            }
        }
        else
        {
            _lastProgress += 0.5F;
            if (_lastProgress >= 1.0F)
            {
                _lastProgress = 1.0F;
            }

            if (_extending)
            {
                PushEntities(entities, _lastProgress, _lastProgress - _progress + 1.0F / 16.0F);
            }
        }
    }

    public override void ReadNbt(NBTTagCompound nbt)
    {
        base.ReadNbt(nbt);
        _pushedBlockId = nbt.GetInteger("blockId");
        _pushedBlockData = nbt.GetInteger("blockData");
        _facing = nbt.GetInteger("facing").ToSide();
        _progress = _lastProgress = nbt.GetFloat("progress");
        _extending = nbt.GetBoolean("extending");
    }

    public override void WriteNbt(NBTTagCompound nbt)
    {
        base.WriteNbt(nbt);
        nbt.SetInteger("blockId", _pushedBlockId);
        nbt.SetInteger("blockData", _pushedBlockData);
        nbt.SetInteger("facing", _facing.ToInt());
        nbt.SetFloat("progress", _progress);
        nbt.SetBoolean("extending", _extending);
    }
}
