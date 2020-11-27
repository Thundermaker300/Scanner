using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool AnnounceScpsAtStart { get; set; } = false;
        public int AnnounceScpsDelay { get; set; } = 3;
        public string ScpAnnounceStringSingular { get; set; } = "{list} HAS BREACHED CONTAINMENT.";
        public string ScpAnnounceStringMultiple { get; set; } = "{list} HAVE BREACHED CONTAINMENT";
        public bool RegularScanning { get; set; } = true;
        public int LengthBetweenScans { get; set; } = 300;
        public string ScanStartMessage { get; set; } = "FACILITY SCAN BEGUN . ESTIMATED TIME {length} SECONDS";
        public int ScanLength { get; set; } = 30;
        public string ScanFinishMessage { get; set; } = "FACILITY SCAN COMPLETE . {humanCount} HUMANS DETECTED . {scpCount} SCPS DETECTED . FOUND {list}";
        public string ScanNobodyMessage { get; set; } = "FACILITY SCAN COMPLETE . {humanCount} HUMANS DETECTED . {scpCount} SCPS DETECTED";
        public bool IncludeScpListInScan { get; set; } = true;
        public Dictionary<Team, string> TeamPronounciationSingular { get; set; } = new Dictionary<Team, string>
        {
            [Team.CDP] = "CLASS D PERSONNEL",
            [Team.RSC] = "SCIENTIST",
            [Team.SCP] = "SCPSUBJECT",
            [Team.MTF] = "MTFUNIT",
            [Team.CHI] = "CHAOS INSURGENT",
        };

        public Dictionary<Team, string> TeamPronounciationPlural { get; set; } = new Dictionary<Team, string>
        {
            [Team.CDP] = "CLASS D PERSONNEL",
            [Team.RSC] = "SCIENTISTS",
            [Team.SCP] = "SCPSUBJECTS",
            [Team.MTF] = "MTFUNITS",
            [Team.CHI] = "CHAOS INSURGENTS",
        };

        public Dictionary<RoleType, string> ScpPronounciation { get; set; } = new Dictionary<RoleType, string>
        {
            [RoleType.Scp049] = "SCP 0 4 9",
            [RoleType.Scp0492] = "SCP 0 4 9 2",
            [RoleType.Scp079] = "SCP 0 7 9",
            [RoleType.Scp096] = "SCP 0 9 6",
            [RoleType.Scp106] = "SCP 1 0 6",
            [RoleType.Scp173] = "SCP 1 7 3",
            [RoleType.Scp93953] = "SCP 9 3 9 5 3",
            [RoleType.Scp93989] = "SCP 9 3 9 8 9",
        };
    }
}
