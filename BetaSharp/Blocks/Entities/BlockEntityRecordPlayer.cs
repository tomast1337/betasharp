using BetaSharp.NBT;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityRecordPlayer : BlockEntity
{
    public int recordId;

    public override void ReadNbt(NBTTagCompound nbt)
    {
        base.ReadNbt(nbt);
        recordId = nbt.GetInteger("Record");
    }

    public override void WriteNbt(NBTTagCompound nbt)
    {
        base.WriteNbt(nbt);
        if (recordId > 0)
        {
            nbt.SetInteger("Record", recordId);
        }

    }
}