using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SCP_832.Components.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCP_832.Items;

public partial class Variant832A
{
    private string ApplyNeutralEvent(Player player, NeutralEvents ev)
    {
        switch (ev)
        {
            case NeutralEvents.None:
            {
                return Message("Neutral", "None");
            }
            case NeutralEvents.SwapWithRandomPlayer:
            {
                if (player.Role.Type == RoleTypeId.Tutorial)
                    return Message("Neutral", "SwapWithRandomPlayer.Tutorial");

                if (player.CurrentRoom?.Type == RoomType.Pocket)
                    return Message("Neutral", "SwapWithRandomPlayer.Pocket");

                var playerPos = player.Position;
                var target = PickRandom(Player.List, pl => pl.IsAlive && pl.Role.Type != RoleTypeId.Scp079
                    && pl.CurrentRoom?.Type != RoomType.Pocket
                    && pl.Role.Type != RoleTypeId.Tutorial);

                if (target == null)
                    return Message("Neutral", "SwapWithRandomPlayer.Unavailable");

                var targetPos = target.Position;

                if (target == player)
                    return Message("Neutral", "SwapWithRandomPlayer.Self");

                player.Teleport((Vector3)targetPos);
                target.Teleport(playerPos);

                ShowHint(target, Message("Other", "SwapPartner", ("player", player.Nickname)));

                return Message("Neutral", "SwapWithRandomPlayer.Success", ("player", target.Nickname));
            }
            case NeutralEvents.TeleportInRandomRoom:
            {
                if (Warhead.IsDetonated)
                    return Message("Neutral", "TeleportInRandomRoom.Unavailable");

                return Random.Range(0, 3) switch
                {
                    0 => TeleportToRoom(player, RoomTypesLcz, "LightContainment"),
                    1 => TeleportToRoom(player, RoomTypesHcz, "HeavyContainment"),
                    _ => TeleportToRoom(player, RoomTypesEntrance, "Entrance"),
                };
            }
        }

        return string.Empty;
    }

    private static string TeleportToRoom(Player player, HashSet<RoomType> types, string zone)
    {
        if (types == null || types.Count == 0)
            return Message("Neutral", "TeleportInRandomRoom.Unavailable");
        var rooms = Room.List.Where(room => types.Contains(room.Type)
            && !(room.Zone == ZoneType.LightContainment && Map.IsLczDecontaminated)).ToArray();
        if (rooms.Length == 0)
            return Message("Neutral", "TeleportInRandomRoom.Unavailable");
        player.Teleport(rooms[Random.Range(0, rooms.Length)]);
        return Message("Neutral", "TeleportInRandomRoom." + zone);
    }
}
