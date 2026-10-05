using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using SCP_832.Components.Enums;
using UnityEngine;

namespace SCP_832.Items;

public partial class Variant832A
{
    private string ApplyPositiveEvent(Player player, PositiveEvents ev)
    {
        switch (ev)
        {
            case PositiveEvents.GiveRandomItem:
            {
                int value = Random.Range(0, 100);
                return value switch
                {
                    < 5 => GiveReward(player, LegendaryItems, "Legendary", "#fc7703"),
                    < 30 => GiveReward(player, EpicItems, "Epic", "#b603fc"),
                    < 60 => GiveReward(player, RareItems, "Rare", "#06b300"),
                    _ => GiveReward(player, CommonItems, "Common", "#6a7069"),
                };
            }
            case PositiveEvents.GiveRandomCard:
            {
                if (Keycards == null || Keycards.Count == 0)
                    return Message("Positive", "RewardUnavailable");
                SpawnHighlighted(player, CommonExtensions.GetRandomValue<ItemType>(Keycards), "#BDBDBD");
                return Message("Positive", "GiveRandomCard");
            }
            case PositiveEvents.GiveRandomScp:
            {
                if (ScpItems == null || ScpItems.Count == 0)
                    return Message("Positive", "RewardUnavailable");
                SpawnHighlighted(player, CommonExtensions.GetRandomValue<ItemType>(ScpItems), "#1A6CC7");
                return Message("Positive", "GiveRandomScp");
            }
            case PositiveEvents.GiveHealKit:
            {
                SpawnHighlighted(player, ItemType.Medkit, "#E339BE");
                SpawnHighlighted(player, ItemType.Adrenaline, "#E339BE");
                SpawnHighlighted(player, ItemType.Painkillers, "#E339BE");
                return Message("Positive", "GiveHealKit");
            }
            case PositiveEvents.GiveStaminaKit:
            {
                for (int i = 0; i < 3; i++)
                    SpawnHighlighted(player, ItemType.Adrenaline, "#E3B039");
                return Message("Positive", "GiveStaminaKit");
            }
            case PositiveEvents.TemporaryInfinityStamina:
            {
                byte duration = 60;
                var state = GetState(player);
                state.OriginalStaminaUsage ??= player.IsUsingStamina;
                player.IsUsingStamina = false;
                player.ResetStamina();

                Schedule(player, "Stamina", duration, () => RestoreStamina(player, state));

                return Message("Positive", "TemporaryInfinityStamina", ("duration", duration));
            }
            case PositiveEvents.TemporaryMovementBoost:
            {
                byte duration = 30;
                player.EnableEffect(EffectType.MovementBoost, 100, duration);
                return Message("Positive", "TemporaryMovementBoost", ("duration", duration));
            }
            case PositiveEvents.OverHeal:
            {
                var value = 50;
                player.Heal(value, true);
                return Message("Positive", "OverHeal", ("amount", value));
            }
            case PositiveEvents.SuperShield:
            {
                player.AddAhp(200f, player.MaxArtificialHealth + 200, 3f, 1f);
                return Message("Positive", "SuperShield");
            }
            case PositiveEvents.TemporaryCasperKit:
            {
                var duration = 60;
                player.EnableEffect(EffectType.Ghostly, 1, duration);
                return Message("Positive", "TemporaryCasperKit", ("duration", duration));
            }
            case PositiveEvents.TemporaryNightVision:
            {
                var duration = 30;
                player.EnableEffect(EffectType.NightVision, 125, duration);
                return Message("Positive", "TemporaryNightVision", ("duration", duration));
            }
        }

        return string.Empty;
    }

    private string GiveReward(Player player, Dictionary<ItemType, ushort> table, string rarity, string color)
    {
        var pickup = SpawnReward(player, table);
        if (pickup == null)
            return Message("Positive", "RewardUnavailable");
        HighlightItem(pickup, color);
        return Message("Positive", "GiveRandomItem." + rarity, ("item", pickup.Type));
    }
}
