using System.Collections.Generic;
using Exiled.API.Enums;

namespace SCP_832.Items;

public partial class Variant832A
{
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

}
