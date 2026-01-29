using System;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using Exiled.Events.EventArgs.Player;
using SCP_832.Components.Events;
using SCP_832.Components.Features;
using events = Exiled.Events.Handlers;

namespace SCP_832
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "SCP-832";
        public override string Prefix => Name;
        public override string Author => "Morkamo";
        public override Version RequiredExiledVersion => new(9, 1, 0);
        public override Version Version => new(1, 0, 0);

        public static Plugin Instance;
        public GlobalCoin GlobalCoin;

        public override void OnEnabled()
        {
            Instance = this;
            events.Player.Verified += OnVerifiedPlayer;
            
            if (Config.ServerIdenifier == "Modded")
            {
                GlobalCoin = new GlobalCoin();
                MorkamoEventsRegistrator.Plugin.AddRegistrator(GlobalCoin);
            }
            else
                Config.Scp832.Register();
            
            base.OnEnabled();
        }

        public override void OnDisabled() 
        {
            if (Config.ServerIdenifier == "Modded")
            {
                MorkamoEventsRegistrator.Plugin.RemoveRegistrator(GlobalCoin);
                GlobalCoin = null;
            }
            else
                Config.Scp832.Unregister();
            
            events.Player.Verified -= OnVerifiedPlayer;
            Instance = null;
            base.OnDisabled();
        }
        
        private void OnVerifiedPlayer(VerifiedEventArgs ev)
        {
            if (ev.Player.ReferenceHub.gameObject.GetComponent<Scp832Properties>() != null)
                return;

            ev.Player.ReferenceHub.gameObject.AddComponent<Scp832Properties>();
            
            EventManager.PlayerEvents.InvokePlayerFullConnected(ev.Player);
        }
    }
}