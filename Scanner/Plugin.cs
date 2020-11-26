using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Handlers = Exiled.Events.Handlers;

namespace Scanner
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Singleton;
        private EventHandlers handler;
        public override void OnEnabled()
        {
            Singleton = this;
            handler = new EventHandlers(this);

            Handlers.Server.RoundStarted += handler.OnRoundStarted;
            Handlers.Server.RoundEnded += handler.OnRoundEnded;
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
        public override Version Version => new Version(0, 0, 0);
        public override Version RequiredExiledVersion => new Version(2, 1, 18);
        public override PluginPriority Priority => PluginPriority.Low;
    }
}
