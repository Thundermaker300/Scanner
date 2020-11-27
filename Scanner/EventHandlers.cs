using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;
using Exiled.Loader;
using Exiled.Events.EventArgs;
using MEC;
using System.Reflection;
using System.IO;

namespace Scanner
{
    public class EventHandlers
    {
        private static Plugin plugin;
        private List<CoroutineHandle> Coroutines = new List<CoroutineHandle> { };
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

        public string GetScpString(RoleType rt, int amount) /*
        {
            StringBuilder scpList = new StringBuilder();
            scpList.Append("SCP ");
            foreach (char c in rt.ToString().Substring(2))
            {
                scpList.Append($"{c} ");
            }
            return scpList.ToString();
        }*/ => $"{(amount == 1 ? string.Empty : $"{amount} ")}{plugin.Config.ScpPronounciation[rt]}";

        public IEnumerator<float> Scan()
        {
            for (; ;)
            {
                while (Player.List.Where(Ply => Ply.IsAlive).Count() < 1)
                {
                    yield return Timing.WaitForSeconds(1f);
                }
                yield return Timing.WaitForSeconds(plugin.Config.LengthBetweenScans);
                Cassie.Message(plugin.Config.ScanStartMessage.Replace("{length}", plugin.Config.ScanLength.ToString()));
                yield return Timing.WaitForSeconds(plugin.Config.ScanLength);
                StringBuilder builder = new StringBuilder();
                foreach (Team t in GetAliveTeams())
                {
                    if (t == Team.SCP) continue;
                    builder.Append(GetCassieMessage(t));
                }
                // SH Support
                var SHPlayers = ((List<Player>)Loader.Plugins.FirstOrDefault(pl => pl.Name == "SerpentsHand")?.Assembly.GetType("SerpentsHand.API.SerpentsHand")?.GetMethod("GetSHPlayers")?.Invoke(null, null))?.Count ?? 0;
                if (SHPlayers > 0)
                {
                    builder.Append($"{SHPlayers} SERPENTS HAND . ");
                }
                if (plugin.Config.IncludeScpListInScan == true)
                {
                    Dictionary<RoleType, int> ScpCount = new Dictionary<RoleType, int> { };
                    foreach (Player Ply in Player.List.Where(Ply => Ply.Team == Team.SCP))
                    {
                        if (!ScpCount.ContainsKey(Ply.Role))
                        {
                            ScpCount[Ply.Role] = 1;
                        }
                        else
                        {
                            ScpCount[Ply.Role]++;
                        }
                    }
                    foreach (KeyValuePair<RoleType, int> item in ScpCount)
                    {
                        builder.Append(GetScpString(item.Key, item.Value));
                        builder.Append(" . ");
                    }
                }
                string list = builder.ToString();
                int numberHuman = Player.List.Count(Ply => Ply.IsAlive && Ply.Team != Team.SCP && Ply.Team != Team.TUT) + (((List<Player>)Loader.Plugins.FirstOrDefault(pl => pl.Name == "SerpentsHand")?.Assembly.GetType("SerpentsHand.API.SerpentsHand")?.GetMethod("GetSHPlayers")?.Invoke(null, null))?.Count ?? 0);
                int numberSCPs = Player.List.Count(Ply => Ply.Team == Team.SCP);
                Cassie.Message(plugin.Config.ScanFinishMessage.Replace("{humanCount}", numberHuman.ToString()).Replace("{scpCount}", numberSCPs.ToString()).Replace("{list}", list));
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
                    Dictionary<RoleType, int> ScpCount = new Dictionary<RoleType, int> { };
                    foreach (Player Ply in Player.List.Where(Ply => Ply.Team == Team.SCP))
                    {
                        if (!ScpCount.ContainsKey(Ply.Role))
                        {
                            ScpCount[Ply.Role] = 1;
                        }
                        else
                        {
                            ScpCount[Ply.Role]++;
                        }
                    }
                    if (ScpCount.Count() < 1)
                    {
                        return;
                    }
                    foreach (KeyValuePair<RoleType, int> item in ScpCount)
                    {
                        scpList.Append(GetScpString(item.Key, item.Value));
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
