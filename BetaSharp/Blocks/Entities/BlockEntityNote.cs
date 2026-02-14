using BetaSharp.Blocks.Materials;
using BetaSharp.NBT;
using BetaSharp.Worlds;

namespace BetaSharp.Blocks.Entities;

public class BlockEntityNote : BlockEntity
{
    public sbyte note = 0;
    public bool powered = false;

    public override void WriteNbt(NBTTagCompound nbt)
    {
        base.WriteNbt(nbt);
        nbt.SetByte("note", note);
    }

    public override void ReadNbt(NBTTagCompound nbt)
    {
        base.ReadNbt(nbt);
        note = nbt.GetByte("note");
        if (note < 0)
        {
            note = 0;
        }

        if (note > 24)
        {
            note = 24;
        }

    }

    public void cycleNote()
    {
        note = (sbyte)((note + 1) % 25);
        MarkDirty();
    }

    public void playNote(World world, int x, int y, int z)
    {
        if (world.getMaterial(x, y + 1, z) == Material.Air)
        {
            Material material = world.getMaterial(x, y - 1, z);
            byte instrument = 0;
            if (material == Material.Stone)
            {
                instrument = 1;
            }

            if (material == Material.Sand)
            {
                instrument = 2;
            }

            if (material == Material.Glass)
            {
                instrument = 3;
            }

            if (material == Material.Wood)
            {
                instrument = 4;
            }

            world.playNoteBlockActionAt(x, y, z, instrument, note);
        }
    }
}