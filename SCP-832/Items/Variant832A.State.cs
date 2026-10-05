using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using MEC;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using SCP_832.Runtime;
using UnityEngine;

namespace SCP_832.Items;

public partial class Variant832A
{
    private readonly Dictionary<Player, PlayerState> states = new();
    private readonly HighlightService highlights = new();
    private readonly Tag hintTag = new();
    private readonly Dictionary<Pickup, CoroutineHandle> pendingHighlights = new();

    private PlayerState GetState(Player player)
    {
        if (!states.TryGetValue(player, out var state))
            states.Add(player, state = new PlayerState(player.Role.Base));
        return state;
    }

    private void StartEffect(Player player, string key, IEnumerator<float> routine)
    {
        var state = GetState(player);
        StopEffect(state, key);
        state.Effects[key] = Timing.RunCoroutine(routine);
    }

    private static void StopEffect(PlayerState state, string key)
    {
        if (!state.Effects.TryGetValue(key, out var handle))
            return;
        state.Effects.Remove(key);
        Timing.KillCoroutines(handle);
    }

    private void Schedule(Player player, string key, float delay, Action action)
        => StartEffect(player, key, DelayedAction(player, GetState(player), key, delay, action));

    private IEnumerator<float> DelayedAction(Player player, PlayerState state, string key, float delay, Action action)
    {
        yield return Timing.WaitForSeconds(delay);
        state.Effects.Remove(key);
        if (player.IsConnected && player.IsAlive && states.TryGetValue(player, out var current) && current == state)
            action();
    }

    private IEnumerator<float> ZombieRegeneration(Player player, PlayerState state)
    {
        int shieldTicks = 0;
        while (player.IsConnected && player.IsAlive && player.Role.Type == RoleTypeId.Scp0492 && state.IsCoinZombie)
        {
            player.Heal(1f);
            // Same 100 shield points over ten seconds, with one coroutine per zombie.
            if (shieldTicks++ < 20)
                player.HumeShield += 5f;
            yield return Timing.WaitForSeconds(0.5f);
        }
        state.Effects.Remove("ZombieRegeneration");
    }

    private static void RestoreStamina(Player player, PlayerState state)
    {
        if (state.OriginalStaminaUsage is bool original && player.IsConnected && ReferenceEquals(player.Role.Base, state.Role))
            player.IsUsingStamina = original;
        state.OriginalStaminaUsage = null;
    }

    private void ResetPlayer(Player player)
    {
        if (!states.TryGetValue(player, out var state))
            return;
        states.Remove(player);
        foreach (var handle in state.Effects.Values)
            Timing.KillCoroutines(handle);
        state.Effects.Clear();
        state.IsCoinZombie = false;
        RestoreStamina(player, state);
        if (state.HasHandcuffedByCoin && player.IsConnected && ReferenceEquals(player.Role.Base, state.Role))
            player.RemoveHandcuffs();
        if (player.ReferenceHub != null)
            highlights.Remove(player.ReferenceHub.gameObject);
        if (player.IsConnected)
            RueDisplay.Get(player).Remove(hintTag);
    }

    private void ResetRound()
    {
        foreach (Player player in states.Keys.ToArray())
            ResetPlayer(player);
        foreach (var handle in pendingHighlights.Values)
            Timing.KillCoroutines(handle);
        pendingHighlights.Clear();
        highlights.Clear();
    }

    private void ShowHint(Player player, string text)
    {
        if (!player.IsConnected || string.IsNullOrEmpty(text))
            return;
        string formatted = Message("Other", "HintFormat", ("text", text));
        if (string.IsNullOrEmpty(formatted))
            return;
        GetState(player);
        var config = Plugin.Instance.Config;
        RueDisplay.Get(player).Show(hintTag,
            new BasicElement(config.HintPosition, formatted),
            Mathf.Clamp(config.HintDuration, 0.1f, 60f));
    }

    private void QueueHighlight(Pickup pickup)
    {
        var config = Plugin.Instance.Config;
        if (!config.EnableHighlights || config.MaxHighlights <= 0
            || pendingHighlights.ContainsKey(pickup))
            return;
        pendingHighlights[pickup] = Timing.RunCoroutine(HighlightNextFrame(pickup));
    }

    private IEnumerator<float> HighlightNextFrame(Pickup pickup)
    {
        yield return Timing.WaitForOneFrame;
        pendingHighlights.Remove(pickup);
        HighlightCoin(pickup);
    }

    private void HighlightItem(Pickup pickup, string hexColor)
    {
        if (pickup?.Base == null)
            return;
        if (!ColorUtility.TryParseHtmlString(hexColor, out var color))
            color = Color.white;
        highlights.Add(pickup.GameObject, color);
    }

    private void SpawnHighlighted(Player player, ItemType type, string color)
        => HighlightItem(Pickup.CreateAndSpawn(type, player.Position, Quaternion.identity), color);

    private Pickup SpawnReward(Player player, Dictionary<ItemType, ushort> table)
    {
        if (table == null || table.Count == 0)
            return null;
        var reward = table.ElementAt(UnityEngine.Random.Range(0, table.Count));
        if (reward.Value == 0)
            return null;
        var pickup = Pickup.Create(reward.Key);
        if (pickup is AmmoPickup ammo)
            ammo.Ammo = reward.Value;
        pickup.Spawn(player.Position, Quaternion.identity);
        // Limit configuration-driven bursts of network spawns.
        if (!(pickup is AmmoPickup))
            for (int i = 1; i < Math.Min((int)reward.Value, 8); i++)
                Pickup.CreateAndSpawn(reward.Key, player.Position, Quaternion.identity);
        return pickup;
    }
}
