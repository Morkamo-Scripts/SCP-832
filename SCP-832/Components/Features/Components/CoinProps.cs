using System.Collections.Generic;
using SCP_832.Components.Features.Components.Interfaces;
using UnityEngine;

namespace SCP_832.Components.Features.Components;

public class CoinProps(Scp832Properties scp832Properties) : IPlayerPropertyModule
{
    public Scp832Properties Scp832Properties { get; } = scp832Properties;
    public Dictionary<string, Coroutine> ActiveCustomEffects { get; set; } = new();
    public GameObject ZombieHightlighterParent { get; set; }
    public bool IsCoinZombie { get; set; } = false;
    public bool HasHandcuffedByCoin { get; set; } = false;
}