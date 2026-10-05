using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using SCP_832.Components.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCP_832.Items;

public partial class Variant832A
{
    private string ApplyNegativeEvent(Player player, NegativeEvents ev)
    {
        switch (ev)
        {
            case NegativeEvents.Handcuff:
            {
                var state = GetState(player);
                player.Handcuff();
                player.DropHeldItem();
                state.HasHandcuffedByCoin = true;

                var duration = 40f;

                Schedule(player, "Handcuff", duration, () =>
                {
                    if (state.HasHandcuffedByCoin)
                    {
                        state.HasHandcuffedByCoin = false;
                        player.RemoveHandcuffs();
                    }
                });

                return Message("Negative", "Handcuff", ("duration", (ushort)duration));
            }
            case NegativeEvents.TeleportToRandomScp:
            {
                var target = PickRandom(Player.List, pl => pl.Role.Team == Team.SCPs
                                                              && pl.Role.Type != RoleTypeId.Scp079
                                                              && pl.Role.Type != RoleTypeId.Scp0492
                                                              && pl.IsAlive);

                if (target == null)
                    return Message("Negative", "TeleportToRandomScp.Unavailable");

                player.Teleport((object)target);
                return Message("Negative", "TeleportToRandomScp.Success");
            }
            case NegativeEvents.ExplodeFlashbang:
            {
                FlashGrenade flash = (FlashGrenade)Item.Create(ItemType.GrenadeFlash, player);
                flash.FuseTime = 0f;
                flash.SpawnActive(player.Position);
                return Message("Negative", "ExplodeFlashbang");
            }
            case NegativeEvents.ExplodeFragGrenade:
            {
                ExplosiveGrenade he = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
                he.FuseTime = 1f;
                he.SpawnActive(player.Position, player);
                return Message("Negative", "ExplodeFragGrenade");
            }
            case NegativeEvents.RandomDamage:
            {
                var damageValue = Random.Range(1, Mathf.Max(2, Mathf.CeilToInt(player.MaxHealth) + 1));
                bool isCritical = damageValue >= player.Health;

                player.Hurt(damageValue, Message("Other", "DamageReason"));

                if (isCritical)
                    return Message("Negative", "RandomDamage.Fatal", ("damage", damageValue));

                return Message("Negative", "RandomDamage.Success", ("damage", damageValue));
            }
            case NegativeEvents.ForceStop:
            {
                var duration = 20;
                player.EnableEffect(EffectType.Ensnared, duration);
                return Message("Negative", "ForceStop", ("duration", duration));
            }
            case NegativeEvents.DestroyRandomItemInInventory:
            {
                var item = PickRandom(player.Items);
                if (item != null)
                    player.RemoveItem(item);
                return Message("Negative", "DestroyRandomItemInInventory");
            }
            case NegativeEvents.TemporaryReversedMovement:
            {
                byte duration = 20;
                player.EnableEffect(EffectType.Slowness, 255, duration);
                return Message("Negative", "TemporaryReversedMovement", ("duration", duration));
            }
            case NegativeEvents.ForceClassScp0492:
            {
                var scp049 = Player.List.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp049);
                var oldPosition = player.Position;

                if (player.CurrentRoom?.Type == RoomType.Pocket)
                {
                    if (scp049 != null)
                    {
                        player.Teleport(scp049);
                        return Message("Negative", "ForceClassScp0492.PocketWithParent");
                    }

                    return Message("Negative", "ForceClassScp0492.Failed");
                }

                player.Role.Set(RoleTypeId.Scp0492);
                if (player.Role.Type != RoleTypeId.Scp0492)
                    return Message("Negative", "ForceClassScp0492.Failed");
                player.Teleport(oldPosition);
                player.MaxHealth = 700;
                player.Health = 700;

                if (scp049 != null)
                    ShowHint(scp049, Message("Other", "ZombieArmyJoined", ("player", player.Nickname)));

                var state = GetState(player);
                state.IsCoinZombie = true;
                highlights.Add(player.GameObject, Color.red);
                StartEffect(player, "ZombieRegeneration", ZombieRegeneration(player, state));

                return Message("Negative", "ForceClassScp0492.Success");
            }
            case NegativeEvents.TemporaryCardiacArrest:
            {
                byte duration = 15;
                player.EnableEffect(EffectType.CardiacArrest, 1, duration);
                return Message("Negative", "TemporaryCardiacArrest", ("duration", duration));
            }
            case NegativeEvents.TeleportToPocket:
            {
                player.EnableEffect(EffectType.PocketCorroding);
                return Message("Negative", "TeleportToPocket");
            }
        }

        return string.Empty;
    }
}
