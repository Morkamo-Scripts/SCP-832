using System;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using SCP_832.Configuration;
using SCP_832.Items;

namespace SCP_832;

public sealed class Plugin : Plugin<Config>
{
    public override string Name => "SCP-832";
    public override string Prefix => Name;
    public override string Author => "Morkamo";
    public override Version RequiredExiledVersion => new(9, 12, 2);
    public override Version Version => new(1, 2, 0);

    internal static Plugin Instance { get; private set; }
    internal Messages Messages { get; private set; }
    private Variant832A _registeredItem;

    public override void OnEnabled()
    {
        Instance = this;
        Messages = MessageFile.Load();
        _registeredItem = Config.Scp832 ?? new Variant832A();
        _registeredItem.Register();
        Exiled.Events.Handlers.Server.ReloadedConfigs += ReloadMessages;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.ReloadedConfigs -= ReloadMessages;
        _registeredItem?.Unregister();
        _registeredItem = null;
        Messages = null;
        Instance = null;
        base.OnDisabled();
    }

    private void ReloadMessages() => Messages = MessageFile.Load();
}
