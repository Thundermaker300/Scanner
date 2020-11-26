using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace Scanner
{
    public class EventHandlers
    {
        private static Plugin plugin;
        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle> { };
        public EventHandlers(Plugin P) => plugin = P;
        private List<Team> GetAliveTeams()
        {
            List<Team> AliveRoles = new List<Team> { };
            foreach (Player Ply in Player.List)
            {
                if (Ply.IsAlive && Ply.Team != Team.TUT && !AliveRoles.Contains(Ply.Team))
                {
                    AliveRoles.Add(Ply.Team);
                }
            }
            return AliveRoles;
        }

        private int GetNumPlayersInTeam(Team t)
        {
            int amount = 0;
            foreach (Player Ply in Player.List)
            {
                if (Ply.Team == t)
                {
                    amount++;
                }
            }
            return amount;
        }

        public string GetCassieMessage(Team t)
        {
            int amount = GetNumPlayersInTeam(t);
            return $"{amount} {(amount > 1 ? plugin.Config.TeamPronounciationPlural[t] : plugin.Config.TeamPronounciationSingular[t])} . ";
        }

        public string GetScpString(RoleType rt)
        {
            StringBuilder scpList = new StringBuilder();
            scpList.Append("SCP ");
            foreach (char c in rt.ToString().Substring(2))
            {
                scpList.Append($"{c} ");
            }
            return scpList.ToString();
        }

        public IEnumerator<float> Scan()
        {
            for (; ;)
            {
                yield return Timing.WaitForSeconds(plugin.Config.LengthBetweenScans);
                Cassie.Message(plugin.Config.ScanStartMessage.Replace("{length}", plugin.Config.ScanLength.ToString()));
                yield return Timing.WaitForSeconds(plugin.Config.ScanLength);
                StringBuilder builder = new StringBuilder();
                foreach (Team t in GetAliveTeams())
                {
                    if (t == Team.SCP) continue;
                    builder.Append(GetCassieMessage(t));
                }
                if (plugin.Config.IncludeScpListInScan == true)
                {
                    foreach (Player Ply in Player.List.Where(Ply => Ply.Team == Team.SCP && Ply.Role != RoleType.Scp0492))
                    {
                        builder.Append(GetScpString(Ply.Role));
                        builder.Append(" . ");
                    }
                }
                string list = builder.ToString();
                string numberHuman = Player.List.Count(Ply => Ply.IsAlive && Ply.Team != Team.SCP && Ply.Team != Team.TUT).ToString();
                string numberSCPs = Player.List.Count(Ply => Ply.Team == Team.SCP).ToString();
                Cassie.Message(plugin.Config.ScanFinishMessage.Replace("{humanCount}", numberHuman).Replace("{scpCount}", numberSCPs).Replace("{list}", list));
            }
        }

        public void OnRoundStarted()
        {
            foreach (CoroutineHandle CHandle in Coroutines)
            {
                Timing.KillCoroutines(CHandle);
            }
            if (plugin.Config.AnnounceScpsAtStart == true)
            {
                Timing.CallDelayed(0.3f + plugin.Config.AnnounceScpsDelay, () =>
                {
                    StringBuilder scpList = new StringBuilder();
                    foreach (Player Ply in Player.List.Where(Ply => Ply.Team == Team.SCP && Ply.Role != RoleType.Scp0492))
                    {
                        scpList.Append(GetScpString(Ply.Role));
                        scpList.Append(" . ");
                    }
                    string str = (GetNumPlayersInTeam(Team.SCP) > 1 ? plugin.Config.ScpAnnounceStringMultiple : plugin.Config.ScpAnnounceStringSingular).Replace("{list}", scpList.ToString());
                    Cassie.Message(str);
                });
            }
            if (plugin.Config.RegularScanning == true)
            {
                Coroutines.Add(Timing.RunCoroutine(Scan()));
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            foreach (CoroutineHandle CHandle in Coroutines)
            {
                Timing.KillCoroutines(CHandle);
            }
        }
    }
}
