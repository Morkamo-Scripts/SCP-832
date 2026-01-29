using MorkamoEventsRegistrator.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.ServerEvents;
using MEC;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using SCP_832.Components;
using SCP_832.Components.Enums;
using SCP_832.Components.Extensions;
using SCP_832.Components.Features;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Player = Exiled.API.Features.Player;
using events = Exiled.Events.Handlers;

namespace SCP_832;

public class GlobalCoin : IEventsRegistrator
{
    public void RegisterEvents()
    {
        events.Player.FlippingCoin += OnCoinFlipped;
        events.Player.ChangingRole += OnChangingRole;
        events.Player.RemovedHandcuffs += OnRemovedHandcuffs;
        events.Player.DroppedItem += OnDroppedItem;
        LabApi.Events.Handlers.ServerEvents.PickupCreated += OnPickupCreated;
    }

    public void UnregisterEvents()
    {
        events.Player.FlippingCoin -= OnCoinFlipped;
        events.Player.ChangingRole -= OnChangingRole;
        events.Player.RemovedHandcuffs -= OnRemovedHandcuffs;
        events.Player.DroppedItem -= OnDroppedItem;
        LabApi.Events.Handlers.ServerEvents.PickupCreated -= OnPickupCreated;
    }
    
    private const int HintWidth = 300;
    private const float HintDuration = 4f;
    
    #region RandomItems

        public Dictionary<ItemType, ushort> CommonItems { get; set; } = new()
        {
            [ItemType.Coin] = 1,
            [ItemType.Ammo9x19] = 60,
            [ItemType.Ammo12gauge] = 18,
            [ItemType.Ammo44cal] = 36,
            [ItemType.Ammo556x45] = 90,
            [ItemType.Ammo762x39] = 90,
            [ItemType.ArmorLight] = 1,
            [ItemType.ArmorCombat] = 1,
            [ItemType.ArmorHeavy] = 1,
            [ItemType.Flashlight] = 1,
            [ItemType.Adrenaline] = 2,
            [ItemType.Radio] = 1,
            [ItemType.GrenadeFlash] = 1,
            [ItemType.SurfaceAccessPass] = 1,
        };
        
        public Dictionary<ItemType, ushort> RareItems { get; set; } = new()
        {
            [ItemType.Coin] = 1,
            [ItemType.GrenadeHE] = 2,
            [ItemType.GrenadeFlash] = 2,
            [ItemType.KeycardJanitor] = 1,
            [ItemType.KeycardScientist] = 1,
            [ItemType.KeycardResearchCoordinator] = 1,
            [ItemType.KeycardZoneManager] = 1,
            [ItemType.KeycardGuard] = 1,
            [ItemType.KeycardMTFPrivate] = 1,
            [ItemType.KeycardContainmentEngineer] = 1,
            [ItemType.KeycardMTFOperative] = 1,
            [ItemType.KeycardMTFCaptain] = 1,
            [ItemType.KeycardFacilityManager] = 1,
            [ItemType.KeycardChaosInsurgency] = 1,
            [ItemType.KeycardO5] = 1,
            [ItemType.Painkillers] = 3,
            [ItemType.Medkit] = 2,
            [ItemType.GunCOM15] = 1,
            [ItemType.GunCOM18] = 1,
            [ItemType.GunCom45] = 1,
            [ItemType.GunRevolver] = 1,
            [ItemType.SCP500] = 1,
            [ItemType.SCP268] = 1,
            [ItemType.SCP207] = 1,
            [ItemType.GunFSP9] = 1,
            [ItemType.GunE11SR] = 1,
            [ItemType.GunShotgun] = 1,
            [ItemType.GunAK] = 1,
            [ItemType.GunCrossvec] = 1,
            [ItemType.SCP1853] = 1,
            [ItemType.SCP1576] = 1,
            [ItemType.SCP2176] = 1,
            [ItemType.SurfaceAccessPass] = 2,
        };
        
        public Dictionary<ItemType, ushort> EpicItems { get; set; } = new()
        {
            [ItemType.Coin] = 1,
            [ItemType.Jailbird] = 1,
            [ItemType.SCP500] = 2,
            [ItemType.SCP207] = 1,
            [ItemType.SCP268] = 1,
            [ItemType.AntiSCP207] = 1,
            [ItemType.SCP244a] = 1,
            [ItemType.SCP244b] = 1,
            [ItemType.GunA7] = 1,
            [ItemType.SCP018] = 1,
            [ItemType.Adrenaline] = 5,
            [ItemType.Medkit] = 5,
            [ItemType.Painkillers] = 5,
            [ItemType.GrenadeHE] = 3,
            [ItemType.SCP1344] = 1,
            [ItemType.SCP1853] = 1,
            [ItemType.SCP1576] = 2,
            [ItemType.SCP2176] = 2,
            [ItemType.GunE11SR] = 1,
            [ItemType.GunLogicer] = 1,
            [ItemType.GunFRMG0] = 1,
            [ItemType.GunShotgun] = 1,
            [ItemType.KeycardO5] = 1,
            [ItemType.KeycardFacilityManager] = 1,
            [ItemType.KeycardChaosInsurgency] = 1,
        };
        
        public Dictionary<ItemType, ushort> LegendaryItems { get; set; } = new()
        {
            [ItemType.KeycardO5] = 3,
            [ItemType.SCP500] = 3,
            [ItemType.AntiSCP207] = 3,
            [ItemType.MicroHID] = 1,
            [ItemType.ParticleDisruptor] = 1,
            [ItemType.GunSCP127] = 1,
            [ItemType.SCP1344] = 1,
        };
        
        public HashSet<ItemType> Keycards { get; set; } = new()
        {
            ItemType.KeycardJanitor,
            ItemType.KeycardScientist,
            ItemType.KeycardResearchCoordinator,
            ItemType.KeycardZoneManager,
            ItemType.KeycardGuard,
            ItemType.KeycardMTFPrivate,
            ItemType.KeycardContainmentEngineer,
            ItemType.KeycardMTFOperative,
            ItemType.KeycardMTFCaptain,
            ItemType.KeycardFacilityManager,
            ItemType.KeycardChaosInsurgency,
            ItemType.KeycardO5
        };
        
        public HashSet<ItemType> ScpItems { get; set; } = new()
        {
            ItemType.SCP500,
            ItemType.SCP207,
            ItemType.SCP018,
            ItemType.SCP268,
            ItemType.SCP2176,
            ItemType.SCP244a,
            ItemType.SCP244b,
            ItemType.SCP1853,
            ItemType.SCP1576,
            ItemType.SCP1344,
            ItemType.SCP1509,
        };

    #endregion
    
    #region RandomRoom

        public HashSet<RoomType> RoomTypesLcz { get; set; } = new()
        {
            RoomType.Lcz330,
            RoomType.LczAirlock,
            RoomType.LczCafe,
            RoomType.Lcz914,
            RoomType.LczPlants,
            RoomType.LczCheckpointA,
            RoomType.LczCheckpointB,
            RoomType.LczGlassBox,
        };
        
        public HashSet<RoomType> RoomTypesHcz { get; set; } = new()
        {
            RoomType.Hcz049,
            RoomType.Hcz127,
            RoomType.HczHid,
            RoomType.HczNuke,
            RoomType.HczServerRoom,
            RoomType.HczEzCheckpointB,
            RoomType.HczEzCheckpointA
        };
        
        public HashSet<RoomType> RoomTypesEntrance { get; set; } = new()
        {
            RoomType.EzCafeteria,
            RoomType.EzConference,
            RoomType.EzUpstairsPcs,
            RoomType.EzCheckpointHallwayA,
            RoomType.EzCheckpointHallwayB,
        };
    
    #endregion
    
    private void OnPickupCreated(PickupCreatedEventArgs ev)
    {
        
    }
    private void OnDroppedItem(DroppedItemEventArgs ev)
    {
        if (Plugin.Instance.Config.ServerIdenifier != "Modded")
            return;
        
        HighlightCoin(ev.Pickup);
    }

    public void OnRemovedHandcuffs(RemovedHandcuffsEventArgs ev)
    {
        if (Plugin.Instance.Config.ServerIdenifier != "Modded")
            return;
        
        if (ev.Target.Scp832Properties().CoinProps.HasHandcuffedByCoin)
            ev.Target.Scp832Properties().CoinProps.HasHandcuffedByCoin = false;
    }
    
    private void HighlightCoin(Pickup pickup)
    {
        if (Plugin.Instance.Config.ServerIdenifier != "Modded")
            return;
        
        if (pickup.Type != ItemType.Coin)
            return; 
        
        var anchor = HighlightManager.MakeLight(pickup.Position, Color.green,
            LightShadows.None, 1, 3);
                
        HighlightManager.ProceduralParticles(pickup.GameObject, Color.green, 0, 0.03f,
            new(0.7f, 0.7f, 0.7f), 0.1f, 8, 8, 60, 1f);
                
        anchor.Transform.SetParent(pickup.Transform);
        anchor.Spawn();
    }
    
    public void OnCoinFlipped(FlippingCoinEventArgs ev)
    {
        if (Plugin.Instance.Config.ServerIdenifier != "Modded")
            return;
        
        Timing.CallDelayed(1.1f, () =>
        {
            if (ev.Player.CurrentItem.Type == ItemType.Coin)
            {
                string eventResult = CallRandomEvent(ev.Player);
                
                RueHint(ev.Player, $"<size=60%>{eventResult}</size>");
                
                foreach (var spectator in ev.Player.CurrentSpectatingPlayers)
                    RueHint(spectator, $"<size=60%>{eventResult}</size>");
            }
        });
    }

    private string CallRandomEvent(Player player)
    {
        byte eventNumber = (byte)Random.Range(0, 3);
        
        bool forceRandomItemEvent = false;
        if (eventNumber == 0 && (byte)Random.Range(0, 3) == 2)
        {
            eventNumber = 1;
            forceRandomItemEvent = true;
        }
        
        switch (eventNumber)
        {
            case 0: // Негативные события
            {
                var values = (NegativeEvents[])Enum.GetValues(typeof(NegativeEvents));
                NegativeEvents ev = values[Random.Range(0, values.Length)];
                
                switch (ev)
                {
                    case NegativeEvents.Handcuff:
                    {
                        player.Handcuff();
                        player.DropHeldItem();
                        player.Scp832Properties().CoinProps.HasHandcuffedByCoin = true;

                        var duration = 40f;
                        
                        Timing.CallDelayed(duration, () =>
                        {
                            if (player.Scp832Properties().CoinProps.HasHandcuffedByCoin)
                            {
                                player.RemoveHandcuffs();
                                player.Scp832Properties().CoinProps.HasHandcuffedByCoin = false;
                            }
                        });
                        
                        return $"Ой, тебя связали... \nРазвяжем через {(ushort)duration} секунд!";
                    }
                    case NegativeEvents.TeleportToRandomScp:
                    {
                        var target = Player.List.GetRandomValue(pl => pl.Role.Team == Team.SCPs
                                                                      && pl.Role.Type != RoleTypeId.Scp079
                                                                      && pl.Role.Type != RoleTypeId.Scp0492
                                                                      && pl.Role.Type != RoleTypeId.Spectator
                                                                      && pl.Role.Type != RoleTypeId.Overwatch
                                                                      && pl.Role.Type != RoleTypeId.Filmmaker
                                                                      && pl.IsAlive);

                        if (target == null)
                            return "Не получилось переместить вас к живому SCP.";
                        
                        player.Teleport(target);
                        return "Вы были перемещены к случайному SCP.";
                    }
                    case NegativeEvents.ExplodeFlashbang:
                    {
                        FlashGrenade flash = (FlashGrenade)Item.Create(ItemType.GrenadeFlash, player);
                        flash.FuseTime = 0f;
                        flash.SpawnActive(player.Position);
                        return "<size=50>Вспышка!</size>";
                    }
                    case NegativeEvents.ExplodeFragGrenade:
                    {
                        ExplosiveGrenade he = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
                        he.FuseTime = 1f;
                        he.SpawnActive(player.Position, player);
                        return "<size=50>Вам выпал колобок!</size>\n\n<size=60>Бум?</size>";
                    }
                    case NegativeEvents.RandomDamage:
                    {
                        var damageValue = Random.Range(1, player.MaxHealth + 1);
                        bool isCritical = damageValue >= player.Health;
                        
                        player.Hurt(damageValue, "Орёл или решка?");
                        
                        if (isCritical) 
                            return $"Здоровье понижено на {damageValue}HP.\nНо вашего запаса не хватило...";
                        
                        return $"Здоровье понижено на {damageValue}HP.";
                    }
                    case NegativeEvents.ForceStop:
                    {
                        var duration = 20;
                        player.EnableEffect(EffectType.Ensnared, duration);
                        return $"СТОП!\nОтдохни {duration} секунд, сейчас самое время.";
                    }
                    case NegativeEvents.DestroyRandomItemInInventory:
                    {
                        player.RemoveItem(player.Items.GetRandomValue());
                        return "У вас что то пропало! Но что?";
                    }
                    case NegativeEvents.TemporaryReversedMovement:
                    {
                        byte duration = 20;
                        player.EnableEffect(EffectType.Slowness, 255, duration);
                        return $"Ой, управление перепуталось!\nПодождите {duration} cекунд.";
                    }
                    case NegativeEvents.ForceClassScp0492:
                    {
                        var scp049 = Player.List.FirstOrDefault(p => p.Role.Type == RoleTypeId.Scp049);
                        var oldPosition = player.Position;

                        if (player.CurrentRoom.Type == RoomType.Pocket)
                        {
                            if (scp049 != null)
                            {
                                player.Teleport(scp049);
                                return "Ты живой?\nДети должны держаться с родителями!";
                            }
                            
                            return "Хмм...\nПревращение не удалось.";
                        }
                        
                        player.Role.Set(RoleTypeId.Scp0492);
                        player.Teleport(oldPosition);
                        player.MaxHealth = 700;
                        player.Health = 700;
                        
                        if (scp049 != null)
                            RueHint(scp049, $"В вашей армии пополнение!\n{player.Nickname}");

                        var props = player.Scp832Properties().CoinProps;
                        props.IsCoinZombie = true;
                        props.ZombieHightlighterParent = new GameObject(nameof(props.ZombieHightlighterParent))
                        {
                            transform =
                            {
                                position = player.Position
                            }
                        };
                        props.ZombieHightlighterParent.transform.SetParent(player.Transform);
                        
                        HighlightManager.ProceduralParticles(props.ZombieHightlighterParent, Color.red, 0, 0.05f,
                            new(0.5f, 0.5f, 0.5f), 0.1f, 5);
                        
                        props.ActiveCustomEffects.Add(nameof(ZombieCoinHeal), 
                            CoroutineRunner.Run(ZombieCoinHeal(player)));
                        props.ActiveCustomEffects.Add(nameof(ZombieHumeShieldRegen), 
                            CoroutineRunner.Run(ZombieHumeShieldRegen(player)));
                        
                        return $"Ты живой?";
                    }
                    case NegativeEvents.TemporaryCardiacArrest:
                    {
                        byte duration = 15;
                        player.EnableEffect(EffectType.CardiacArrest, 1, duration);
                        return $"Последние {duration}?\nВыжить любой ценой!";
                    }
                    case NegativeEvents.TeleportToPocket:
                    {
                        player.EnableEffect(EffectType.PocketCorroding);
                        return $"Добро пожаловать в деморган!";
                    }
                }
                break;
            }
            case 1: // Позитивные события
            {
                var values = (PositiveEvents[])Enum.GetValues(typeof(PositiveEvents));
                PositiveEvents ev = values[Random.Range(0, values.Length + 1)];

                if (forceRandomItemEvent)
                    ev = PositiveEvents.GiveRandomItem;
                
                switch (ev)
                {
                    case PositiveEvents.GiveRandomItem:
                    {
                        float value = Random.Range(1f, 101f);

                        switch (value)
                        {
                            case <= 5:
                            {
                                var pickup = Pickup.CreateAndSpawn(LegendaryItems.Keys.GetRandomValue(), 
                                    player.Position, Quaternion.identity);
                                
                                HighlightItem(pickup, "#fc7703");
                                
                                return $"<color=#fc7703>Легендарный</color> предмет.\n" +
                                       $"{pickup.Type}? А ты счастливчик...";
                            }
                            case <= 30:
                            {
                                var pickup = Pickup.CreateAndSpawn(EpicItems.Keys.GetRandomValue(), 
                                    player.Position, Quaternion.identity);
                                
                                HighlightItem(pickup, "#b603fc");
                                
                                return $"<color=#b603fc>Эпический</color> предмет.\n" +
                                       $"{pickup.Type}? Отличный дроп!";
                            }
                            case <= 60:
                            {
                                var pickup = Pickup.CreateAndSpawn(RareItems.Keys.GetRandomValue(), 
                                    player.Position, Quaternion.identity);
                                
                                HighlightItem(pickup, "#06b300");
                                
                                return $"<color=#06b300>Редкий</color> предмет.\n" +
                                       $"{pickup.Type}? Сойдёт!";
                            }
                            case > 60:
                            {
                                var pickup = Pickup.CreateAndSpawn(CommonItems.Keys.GetRandomValue(), 
                                    player.Position, Quaternion.identity);
                                
                                HighlightItem(pickup, "#6a7069");
                                
                                return $"<color=#6a7069>Обычный</color> предмет.\n" +
                                       $"{pickup.Type}? Лишним ничего не будет!";
                            }
                        }
                        break;
                    }
                    case PositiveEvents.GiveRandomCard:
                    {
                        var pickup = Pickup.CreateAndSpawn(Keycards.GetRandomValue(), player.Position, Quaternion.identity);
                        HighlightItem(pickup, "#BDBDBD");
                        return "Случайна ключ-карта прямо под тобой!";
                    }
                    case PositiveEvents.GiveRandomScp:
                    {
                        var pickup = Pickup.CreateAndSpawn(ScpItems.GetRandomValue(), player.Position, Quaternion.identity);
                        HighlightItem(pickup, "#1A6CC7");
                        return "Случайный SCP предмет прямо под тобой!";
                    }
                    case PositiveEvents.GiveHealKit:
                    {
                        var pickup1 = Pickup.CreateAndSpawn(ItemType.Medkit, player.Position, Quaternion.identity);
                        var pickup2 = Pickup.CreateAndSpawn(ItemType.Adrenaline, player.Position, Quaternion.identity);
                        var pickup3 = Pickup.CreateAndSpawn(ItemType.Painkillers, player.Position, Quaternion.identity);
                        
                        HighlightItem(pickup1, "#E339BE");
                        HighlightItem(pickup2, "#E339BE");
                        HighlightItem(pickup3, "#E339BE");
                        
                        return "Ты обронил набор первой помощи!\nХотя бы самому себе.";
                    }
                    case PositiveEvents.GiveStaminaKit:
                    {
                        var pickup1 = Pickup.CreateAndSpawn(ItemType.Adrenaline, player.Position, Quaternion.identity);
                        var pickup2 = Pickup.CreateAndSpawn(ItemType.Adrenaline, player.Position, Quaternion.identity);
                        var pickup3 = Pickup.CreateAndSpawn(ItemType.Adrenaline, player.Position, Quaternion.identity);
                        
                        HighlightItem(pickup1, "#E3B039");
                        HighlightItem(pickup2, "#E3B039");
                        HighlightItem(pickup3, "#E3B039");
                        
                        return "Много адреналина не бывает!";
                    }
                    case PositiveEvents.TemporaryInfinityStamina:
                    {
                        byte duration = 60;
                        player.IsUsingStamina = false;
                        player.ResetStamina();

                        Timing.CallDelayed(duration, () =>
                        {
                            player.IsUsingStamina = true;
                        });
                        
                        return $"Почувствуй силу земли!\n" +
                               $"Выносливость не тратиться следующие {duration} секунд!";
                    }
                    case PositiveEvents.TemporaryMovementBoost:
                    {
                        byte duration = 30;
                        player.EnableEffect(EffectType.MovementBoost, 100, duration);
                        return $"Всегда знал что ты атлет!\n" +
                               $"Тройное увеличение скорости на следующие {duration} секунд!";
                    }
                    case PositiveEvents.OverHeal:
                    {
                        var value = 50;
                        player.Heal(value, true);
                        return $"Жизней много не бывает!\n" +
                               $"Здоровье увеличено на {value}HP.";
                    }
                    case PositiveEvents.SuperShield:
                    {
                        player.AddAhp(200f, player.MaxArtificialHealth + 200, 3f, 1f);
                        return $"Броня! Жалко что не вечная.";
                    }
                    case PositiveEvents.TemporaryCasperKit:
                    {
                        var duration = 60;
                        player.EnableEffect(EffectType.Ghostly, 1, duration);
                        return $"Каково быть призраком?\n" +
                               $"Вы проходите сквозь двери следующие {duration} секунд!";
                    }
                    case PositiveEvents.TemporaryNightVision:
                    {
                        var duration = 30;
                        player.EnableEffect(EffectType.NightVision, 125, duration);
                        return $"Повышаем яркость на {duration} секунд!\nТак лучше?";
                    }
                }
                break;
            }
            case 2: // Нейтральные события
            {
                var values = (NeutralEvents[])Enum.GetValues(typeof(NeutralEvents));
                NeutralEvents ev = values[Random.Range(0, values.Length)];
                
                switch (ev)
                {
                    case NeutralEvents.None:
                    {
                        return $"Вам выпало целое ничего!";
                    }
                    case NeutralEvents.SwapWithRandomPlayer:
                    {
                        if (player.Role.Type == RoleTypeId.Tutorial)
                            return "Не бро, за эту роль нельзя переместиться.";
                        
                        if (player.CurrentRoom.Type == RoomType.Pocket)
                            return "Тут связь не ловит.\nОставайся на своём месте.";
                         
                        var playerPos = player.Position;
                        var target = Player.List.GetRandomValue(pl => pl.IsAlive && pl.Role.Type != RoleTypeId.Scp079 
                            && pl.CurrentRoom.Type != RoomType.Pocket
                            && pl.Role.Type != RoleTypeId.Tutorial);

                        if (target == null)
                            return "Абонент не доступен!\nОставайся на своём месте.";
                        
                        var targetPos = target.Position;
                        
                        if (target == player)
                            return $"Поменялся сам с собой?\n" +
                                   $"Да ты везунчик...";
                         
                        player.Teleport(targetPos);
                        target.Teleport(playerPos);
                        
                        RueHint(target, "Ты поменялся позицией с\n" +
                                        $"{player.Nickname}");
                        
                        return $"Ты поменялся позицией с\n" +
                               $"{target.Nickname}";
                    }
                    case NeutralEvents.TeleportInRandomRoom:
                    {
                        if (Warhead.IsDetonated)
                            return "Абонент недоступен...";
                        
                        var value = Random.Range(0, 3);
                        switch (value)
                        {
                            case 0:
                            {
                                player.Teleport(RoomTypesLcz.GetRandomValue());
                                return $"Вжух!\n" +
                                       $"И вот мы в Зоне Лёгкого Содержания!";
                            }
                            case 1:
                            {
                                player.Teleport(RoomTypesHcz.GetRandomValue());
                                return $"Вжух!\n" +
                                       $"И вот мы в Зоне Тяжелого Содержания!";
                            }
                            case 2:
                            {
                                player.Teleport(RoomTypesEntrance.GetRandomValue());
                                return $"Вжух!\n" +
                                       $"И вот мы во Входной Зоне";
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }

        return string.Empty;
    }
    
    private static void RueHint(Player player, string text)
    {
        if (text.IsEmpty())
            return;
        
        RueDisplay
            .Get(player)
            .Show(
                new Tag(),
                new BasicElement(HintWidth, $"<b>{text}</b>"),
                HintDuration);

        Timing.CallDelayed(HintDuration, () =>
        {
            RueDisplay.Get(player).Update();
        });
    }
    
    private void HighlightItem(Pickup pickup, string hexColor)
    {
        if (Plugin.Instance.Config.ServerIdenifier != "Modded")
            return;
        
        if (ColorUtility.TryParseHtmlString(hexColor, out var color))
        {
            var anchor = HighlightManager.MakeLight(pickup.Position, color,
                LightShadows.None, 0.8f, 0.8f);
            
            HighlightManager.ProceduralParticles(anchor.GameObject, color, 0, 0.05f,
                new(0.35f, 0.35f, 0.35f), 0.1f, 5);
            
            anchor.Transform.SetParent(pickup.Transform);
            anchor.Spawn();
        }
        else
        {
            var anchor = HighlightManager.MakeLight(pickup.Position, Color.white,
                LightShadows.None, 0.8f, 0.8f);
            
            HighlightManager.ProceduralParticles(anchor.GameObject, Color.white, 0, 0.05f,
                new(0.35f, 0.35f, 0.35f), 0.1f, 5);
            
            anchor.Transform.SetParent(pickup.Transform);
            anchor.Spawn();
                
            Log.Warn("Установлен некорректный цвет подсветки, выбор значения по умолчанию..."); 
        }
    }

    private IEnumerator ZombieCoinHeal(Player player)
    {
        while (player.IsAlive && player.Scp832Properties().CoinProps.IsCoinZombie)
        {
            player.Heal(1f);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator ZombieHumeShieldRegen(Player player)
    {
        for (int i = 0; i < 100 && player.IsAlive && player.Scp832Properties().CoinProps.IsCoinZombie; i++)
        {
            player.HumeShield++;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void OnChangingRole(ChangingRoleEventArgs ev)
    {
        if (Plugin.Instance.Config.ServerIdenifier != "Modded")
            return;
        
        DestroyZombieHandler(ev.Player, ref ev);
    }
    
    private void DestroyZombieHandler(Player player, ref ChangingRoleEventArgs ev)
    {
        var props = player.Scp832Properties();
        
        if (props != null && ev.Player.Role == RoleTypeId.Scp0492 && props.CoinProps.IsCoinZombie)
        {
            props.CoinProps.IsCoinZombie = false;
            
            Object.Destroy(props.CoinProps.ZombieHightlighterParent);
            
            props.CoinProps.ActiveCustomEffects.TryGetValue(nameof(ZombieCoinHeal),
                out var coroutine);
            props.CoinProps.ActiveCustomEffects.TryGetValue(nameof(ZombieHumeShieldRegen),
                out var coroutine2);
            
            if (coroutine != null) CoroutineRunner.Stop(coroutine);
            if (coroutine2 != null) CoroutineRunner.Stop(coroutine2);

            props.CoinProps.ActiveCustomEffects.Remove(nameof(ZombieCoinHeal));
            props.CoinProps.ActiveCustomEffects.Remove(nameof(ZombieHumeShieldRegen));
        }
    }
}