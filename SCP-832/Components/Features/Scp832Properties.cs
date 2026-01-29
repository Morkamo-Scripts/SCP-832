using Exiled.API.Features;
using SCP_832.Components.Features.Components;
using UnityEngine;

namespace SCP_832.Components.Features;

public sealed class Scp832Properties() : MonoBehaviour
{
    private void Awake()
    {
        Player = Player.Get(gameObject);
        CoinProps = new CoinProps(this);
    }

    public Player Player { get; private set; }
    public CoinProps CoinProps { get; private set; }
}