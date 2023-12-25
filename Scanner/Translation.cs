using Exiled.API.Interfaces;
using PlayerRoles;
using Scanner.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class Translation : ITranslation
    {
        [Description("Determines the C.A.S.S.I.E string to use when there is one breached SCP.")]
        public CassieMessage ScpAnnounceStringSingular { get; set; } = new(
            "{LIST} HAS BREACHED CONTAINMENT",
            "{LIST} has breached containment."
        );
        [Description("Determines the C.A.S.S.I.E string to use when there is more than one breached SCP.")]
        public CassieMessage ScpAnnounceStringMultiple { get; set; } = new(
            "{LIST} HAVE BREACHED CONTAINMENT",
            "{LIST} have breached containment."
        );

        [Description("Determines the C.A.S.S.I.E string to use when the scan is started.")]
        public CassieMessage ScanStartMessage { get; set; } = new(
            "FACILITY SCAN BEGUN . ESTIMATED TIME {LENGTH} SECONDS",
            "Facility scan begun. Estimated time: {LENGTH} seconds."
        );

        [Description("Determines the C.A.S.S.I.E string to use when the scan is completed.")]
        public CassieMessage ScanFinishMessage { get; set; } = new(
            "FACILITY SCAN COMPLETE . {HUMANCOUNT} HUMANS DETECTED . {SCPCOUNT} SCPS DETECTED . FOUND {LIST}",
            "Facility scan complete. {HUMANCOUNT} humans detected. {SCPCOUNT} SCPs detected. Found {LIST}."
        );
        [Description("Determines the C.A.S.S.I.E string to use when the scan is completed and nobody is alive.")]
        public CassieMessage ScanNobodyMessage { get; set; } = new(
            "FACILITY SCAN COMPLETE . NO HUMANS OR SCPS DETECTED",
            "Facility scan complete. No humans or SCPs detected.");

        [Description("Determines how cassie will pronounce single classes (eg. '1 SCIENTIST').")]
        public Dictionary<Team, CassieMessage> TeamPronounciationSingular { get; set; } = new Dictionary<Team, CassieMessage>
        {
            [Team.ClassD] = new("CLASS D PERSONNEL", "Class-D Personnel"),
            [Team.Scientists] = new("SCIENTIST", "Scientist"),
            [Team.SCPs] = new("SCPSUBJECT", "SCP Subject"),
            [Team.FoundationForces] = new("MTFUNIT", "Mobile Task Force Unit"),
            [Team.ChaosInsurgency] = new("CHAOS INSURGENT", "Chaos Insurgent"),
        };
        [Description("Determines how cassie will pronounce plural classes (eg. '4 SCIENTISTS').")]
        public Dictionary<Team, CassieMessage> TeamPronounciationMultiple { get; set; } = new Dictionary<Team, CassieMessage>
        {
            [Team.ClassD] = new("CLASS D PERSONNEL", "Class-D Personnel"),
            [Team.Scientists] = new("SCIENTIST", "Scientists"),
            [Team.SCPs] = new("SCPSUBJECT", "SCP Subjects"),
            [Team.FoundationForces] = new("MTFUNIT", "Mobile Task Force Units"),
            [Team.ChaosInsurgency] = new("CHAOS INSURGENT", "Chaos Insurgency"),
        };

        [Description("Determines how cassie will pronounce SCPs.")]
        public Dictionary<RoleTypeId, CassieMessage> ScpPronounciation { get; set; } = new Dictionary<RoleTypeId, CassieMessage>
        {
            [RoleTypeId.Scp049] = new("SCP 0 4 9", "SCP-049"),
            [RoleTypeId.Scp0492] = new("SCP 0 4 9 2", "SCP-049-2"),
            [RoleTypeId.Scp079] = new("SCP 0 7 9", "SCP-079"),
            [RoleTypeId.Scp096] = new("SCP 0 9 6", "SCP-096"),
            [RoleTypeId.Scp106] = new("SCP 1 0 6", "SCP-106"),
            [RoleTypeId.Scp173] = new("SCP 1 7 3", "SCP-173"),
            [RoleTypeId.Scp939] = new("SCP 9 3 9", "SCP-939"),
            [RoleTypeId.Scp3114] = new("SCP 3 1 1 4", "SCP-3114"),
            [RoleTypeId.Flamingo] = new("SCP 1 5 0 7", "SCP-1507"),
            [RoleTypeId.AlphaFlamingo] = new("SCP 1 5 0 7", "SCP-1507"),
            [RoleTypeId.ZombieFlamingo] = new("SCP 1 5 0 7 0 4 9", "SCP-1507-049")
        };

        [Description("Determines how cassie will pronounce Serpent's Hand, only if the plugin is installed.")]
        public CassieMessage SerpentsHandPronounce { get; set; } = new("SERPENTS HAND", "Serpent's Hand");
    }
}
