using System;
using System.Collections.Generic;
using System.ComponentModel;

using Exiled.API.Interfaces;
using Exiled.API.Enums;
using PlayerRoles;

namespace Scanner
{
    public class Config : IConfig
    {
        [Description("Determines whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;
        [Description("Whether or not to show debug logs.")]
        public bool Debug { get; set; } = true;
        [Description("Determines whether or not the SCPs that have breached will be announced at the start.")]
        public bool AnnounceScpsAtStart { get; set; } = false;
        [Description("Determines the amount of seconds that will pass before SCPs are announced at the start.")]
        public int AnnounceScpsDelay { get; set; } = 3;
        [Description("Determines the C.A.S.S.I.E string to use when there is one breached SCP.")]
        public string ScpAnnounceStringSingular { get; set; } = "{LIST} HAS BREACHED CONTAINMENT.";
        [Description("Determines the C.A.S.S.I.E string to use when there is more than one breached SCP.")]
        public string ScpAnnounceStringMultiple { get; set; } = "{LIST} HAVE BREACHED CONTAINMENT";
        [Description("Determines whether or not to regularly scan and announce who is alive.")]
        public bool RegularScanning { get; set; } = true;
        [Description("Determines the length of time between each scan (and before the 1st scan) each round.")]
        public int LengthBetweenScans { get; set; } = 300;
        [Description("Determines the C.A.S.S.I.E string to use when the scan is started.")]
        public string ScanStartMessage { get; set; } = "FACILITY SCAN BEGUN . ESTIMATED TIME {LENGTH} SECONDS";
        [Description("Determines the length of time a scan takes to complete.")]
        public int ScanLength { get; set; } = 30;
        [Description("Determines if a list of SCPs will be included at the end of the scan.")]
        public bool IncludeScpListInScan { get; set; } = true;
        [Description("If set to false, scanning will be disabled after the nuke has been detonated.")]
        public bool ScanAfterNuke { get; set; } = false;
        [Description("Determines which zones will be scanned.")]
        public List<ZoneType> ScanZones { get; set; } = new List<ZoneType> { ZoneType.LightContainment, ZoneType.HeavyContainment, ZoneType.Entrance, ZoneType.HeavyContainment | ZoneType.Entrance, ZoneType.Surface };
    }
}
