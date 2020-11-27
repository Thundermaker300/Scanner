using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Handlers = Exiled.Events.Handlers;
using MEC;

namespace Scanner
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Singleton;
        private EventHandlers handler;
        public static bool SerpentsHandEnabled = false;
        public override void OnEnabled()
        {
            Singleton = this;
            handler = new EventHandlers(this);

            Handlers.Server.RoundStarted += handler.OnRoundStarted;
            Handlers.Server.RoundEnded += handler.OnRoundEnded;

            foreach (var p in Loader.Plugins)
            {
                if (p.Name == "SerpentsHand")
                {
                    SerpentsHandEnabled = true;
                }
            }

            if (!SerpentsHandEnabled)
            {
                Log.Info("Serpent's hand not enabled!");
            }

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Handlers.Server.RoundStarted -= handler.OnRoundStarted;
            Handlers.Server.RoundEnded -= handler.OnRoundEnded;

            handler = null;
            Singleton = null;
            base.OnDisabled();
        }

        public override string Name => "Scanner";
        public override string Author => "Thunder";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(2, 1, 18);
        public override PluginPriority Priority => PluginPriority.High;
    }
}
