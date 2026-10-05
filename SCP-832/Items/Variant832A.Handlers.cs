using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using events = Exiled.Events.Handlers;

namespace SCP_832.Items;

public partial class Variant832A
{
    protected override void SubscribeEvents()
    {
        base.SubscribeEvents();
        events.Player.FlippingCoin += OnCoinFlipped;
        events.Player.Spawned += OnSpawned;
        events.Player.Died += OnDied;
        events.Player.Left += OnLeft;
        events.Player.RemovedHandcuffs += OnRemovedHandcuffs;
        events.Player.DroppedItem += OnDroppedItem;
        events.Map.PickupAdded += OnPickupAdded;
        events.Server.RestartingRound += ResetRound;
        events.Server.WaitingForPlayers += ResetRound;
        /*events.Player.Jumping += Jump;*/
    }

    protected override void UnsubscribeEvents()
    {
        events.Player.FlippingCoin -= OnCoinFlipped;
        events.Player.Spawned -= OnSpawned;
        events.Player.Died -= OnDied;
        events.Player.Left -= OnLeft;
        events.Player.RemovedHandcuffs -= OnRemovedHandcuffs;
        events.Player.DroppedItem -= OnDroppedItem;
        events.Map.PickupAdded -= OnPickupAdded;
        events.Server.RestartingRound -= ResetRound;
        events.Server.WaitingForPlayers -= ResetRound;
        /*events.Player.Jumping -= Jump;*/
        ResetRound();
        base.UnsubscribeEvents();
    }

    /*public void Jump(JumpingEventArgs ev)
    {
        CallRandomEvent(ev.Player);
    }*/
    
    private void OnCoinFlipped(FlippingCoinEventArgs ev)
    {
        if (!ev.IsAllowed || !ev.Player.IsAlive || !Check(ev.Item))
            return;

        Player player = ev.Player;
        ushort serial = ev.Item.Serial;
        // Replace the pending flip so repeated input cannot queue unlimited callbacks.
        Schedule(player, "Flip", 1.1f, () =>
        {
            if (player.CurrentItem == null || player.CurrentItem.Serial != serial || !Check(player.CurrentItem))
                return;

            string result = CallRandomEvent(player);
            ShowHint(player, result);
            foreach (Player spectator in player.CurrentSpectatingPlayers)
                ShowHint(spectator, result);
        });
    }

    private void OnSpawned(SpawnedEventArgs ev) => ResetPlayer(ev.Player);
    private void OnDied(DiedEventArgs ev) => ResetPlayer(ev.Player);
    private void OnLeft(LeftEventArgs ev) => ResetPlayer(ev.Player);

    private void OnRemovedHandcuffs(RemovedHandcuffsEventArgs ev)
    {
        if (!states.TryGetValue(ev.Target, out var state))
            return;
        state.HasHandcuffedByCoin = false;
        StopEffect(state, "Handcuff");
    }

    private void OnDroppedItem(DroppedItemEventArgs ev) => HighlightCoin(ev.Pickup);

    private void OnPickupAdded(PickupAddedEventArgs ev)
    {
        if (ev.Pickup?.Type != ItemType.Coin)
            return;
        
        QueueHighlight(ev.Pickup);
    }

    private void HighlightCoin(Pickup pickup)
    {
        if (pickup?.Base != null && Check(pickup))
            highlights.Add(pickup.GameObject, Color.green);
    }
}
