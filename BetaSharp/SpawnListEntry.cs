using BetaSharp.Entities;
using BetaSharp.Worlds.Core;

namespace BetaSharp;

public record SpawnListEntry(Func<IWorldContext, EntityLiving> Factory);
