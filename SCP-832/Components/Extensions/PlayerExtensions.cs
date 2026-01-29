using Exiled.API.Features;
using SCP_832.Components.Features;

namespace SCP_832.Components.Extensions;

public static class PlayerExtensions
{
    public static Scp832Properties Scp832Properties(this Player player)
        => player.ReferenceHub.gameObject.GetComponent<Scp832Properties>();
}