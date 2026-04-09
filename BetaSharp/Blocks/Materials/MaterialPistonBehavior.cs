using System;
using System.Collections.Generic;
using System.Text;

namespace BetaSharp.Blocks.Materials;

/// <summary>
/// Behavior for pistions on a material.
/// </summary>
public enum MaterialPistonBehavior
{
    Normal, // behaves like a normal block, can be pushed by pistons
    Break, // breaks when pushed by a piston
    Unpushable, // cannot be pushed by pistons
}
