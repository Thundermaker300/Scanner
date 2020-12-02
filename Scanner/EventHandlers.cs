using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Exiled.API.Features;
using Exiled.Loader;
using Exiled.Events.EventArgs;
using MEC;

namespace Scanner
{
    public class EventHandlers
    {
        private static Plugin plugin;
        private List<CoroutineHandle> Coroutines = new List<CoroutineHandle> { };
        public EventHandlers(Plugin P) => plugin = P;
        private static List<Team> GetAliveTeams()
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

        private static int GetNumPlayersInTeam(Team t)
        {
            int amount = 0;
            foreach (Player Ply in Player.List)
            {
                if (Ply.Team == t && Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != Ply)
                {
                    amount++;
                }
            }
            return amount;
        }

        public static string GetCassieMessage(Team t)
        {
            int amount = GetNumPlayersInTeam(t);
            if (amount == 0)
            {
                return string.Empty;
            }
            return $"{amount} {(amount > 1 ? plugin.Config.TeamPronounciationPlural[t] : plugin.Config.TeamPronounciationSingular[t])} . ";
        }

        public static string GetScpString(RoleType rt, int amount) => $"{(amount == 1 ? string.Empty : $"{amount} ")}{plugin.Config.ScpPronounciation[rt]}";

        public static void Scan()
        {
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
                // SCP-035 support
                Player Scp035 = Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) as Player;
                if (Scp035 != null)
                {
                    builder.Append("SCP 0 3 5 . ");
                }
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
            int numberHuman = Player.List.Count(Ply => Ply.IsAlive && Ply.Team != Team.SCP && Ply.Team != Team.TUT) + (((List<Player>)Loader.Plugins.FirstOrDefault(pl => pl.Name == "SerpentsHand")?.Assembly.GetType("SerpentsHand.API.SerpentsHand")?.GetMethod("GetSHPlayers")?.Invoke(null, null))?.Count ?? 0) - (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != null ? 1 : 0);
            int numberSCPs = Player.List.Count(Ply => Ply.Team == Team.SCP) + (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != null ? 1 : 0);
            Cassie.Message(plugin.Config.ScanFinishMessage.Replace("{humanCount}", numberHuman.ToString()).Replace("{scpCount}", numberSCPs.ToString()).Replace("{list}", list));
            Plugin.ScanInProgress = false;
        }

        public IEnumerator<float> ScanLoop()
        {
            for (; ;)
            {
                while (Player.List.Where(Ply => Ply.IsAlive).Count() < 1)
                {
                    yield return Timing.WaitForSeconds(1f);
                }
                yield return Timing.WaitForSeconds(plugin.Config.LengthBetweenScans);
                if (Plugin.ScanInProgress)
                {
                    continue;
                }
                Plugin.ScanInProgress = true;
                Cassie.Message(plugin.Config.ScanStartMessage.Replace("{length}", plugin.Config.ScanLength.ToString()));
                yield return Timing.WaitForSeconds(plugin.Config.ScanLength);
                Scan();
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
                Coroutines.Add(Timing.RunCoroutine(ScanLoop()));
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
