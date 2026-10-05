using System.Collections.Generic;
using MEC;
using PlayerRoles;

namespace SCP_832.Runtime;

internal sealed class PlayerState
{
    internal PlayerState(PlayerRoleBase role) => Role = role;
    internal PlayerRoleBase Role { get; }
    internal Dictionary<string, CoroutineHandle> Effects { get; } = new();
    internal bool IsCoinZombie { get; set; }
    internal bool HasHandcuffedByCoin { get; set; }
    internal bool? OriginalStaminaUsage { get; set; }
}
