﻿using System;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using Handlers = Exiled.Events.Handlers;

namespace Scanner
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Singleton;
        private EventHandlers handler;
        public static bool ScanInProgress = false;
        public static bool Force = false;
        public override void OnEnabled()
        {
            Singleton = this;
            handler = new EventHandlers(this);

            Handlers.Server.RoundStarted += handler.OnRoundStarted;
            Handlers.Server.RoundEnded += handler.OnRoundEnded;

            Handlers.Warhead.Detonated += handler.OnDetonated;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Handlers.Server.RoundStarted -= handler.OnRoundStarted;
            Handlers.Server.RoundEnded -= handler.OnRoundEnded;

            Handlers.Warhead.Detonated -= handler.OnDetonated;

            handler = null;
            Singleton = null;
            base.OnDisabled();
        }

        public override string Name => "Scanner";
        public override string Author => "Thunder";
        public override Version Version => new Version(1, 0, 1);
        public override Version RequiredExiledVersion => new Version(2, 1, 18);
        public override PluginPriority Priority => PluginPriority.High;
    }
}
