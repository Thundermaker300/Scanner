using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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

        /* GhostSpectator */ private static bool IsGhost(Player Ply)
        {
            Assembly assembly = Loader.Plugins.FirstOrDefault(pl => pl.Name == "GhostSpectator")?.Assembly;
            if (assembly == null) return false;
            return ((bool)assembly.GetType("GhostSpectator.API")?.GetMethod("IsGhost")?.Invoke(null, new object[] { Ply })) == true;
        }

        private static int GetNumPlayersInTeam(Team t)
        {
            int amount = 0;
            foreach (Player Ply in Player.List)
            {
                if (plugin.Config.ScanZones.Contains(Ply.CurrentRoom.Zone) && Ply.Team == t && /* GhostSpectator */ !IsGhost(Ply) && /* SCP-035 */ Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != Ply)
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
            return $"{amount} {(amount > 1 ? plugin.Config.TeamPronounciationMultiple[t] : plugin.Config.TeamPronounciationSingular[t])} . ";
        }

        public static string GetScpString(RoleType rt, int amount) => $"{(amount == 1 ? string.Empty : $"{amount} ")}{plugin.Config.ScpPronounciation[rt]}";

        public static void Scan()
        {
            Log.Info(2);
            try
            {
                StringBuilder builder = new StringBuilder();
                Log.Info(2.5);
                foreach (Team t in GetAliveTeams())
                {
                    if (t == Team.SCP) continue;
                    builder.Append(GetCassieMessage(t));
                }
                Log.Info(3);
                // SH Support
                var SHPlayers = ((List<Player>)Loader.Plugins.FirstOrDefault(pl => pl.Name == "SerpentsHand")?.Assembly.GetType("SerpentsHand.API.SerpentsHand")?.GetMethod("GetSHPlayers")?.Invoke(null, null))?.Count ?? 0;
                Log.Info(4);
                if (SHPlayers > 0)
                {
                    builder.Append($"{SHPlayers} SERPENTS HAND . ");
                }
                Log.Info(5);
                if (plugin.Config.IncludeScpListInScan == true)
                {
                    // SCP-035 support
                    Log.Info(6);
                    Player Scp035 = Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) as Player;
                    Log.Info(7);
                    if (Scp035 != null)
                    {
                        builder.Append("SCP 0 3 5 . ");
                    }
                    Log.Info(8);
                    Dictionary<RoleType, int> ScpCount = new Dictionary<RoleType, int> { };
                    Log.Info(9);
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
                    Log.Info(10);
                    foreach (KeyValuePair<RoleType, int> item in ScpCount)
                    {
                        builder.Append(GetScpString(item.Key, item.Value));
                        builder.Append(" . ");
                    }
                    Log.Info(11);
                }
                Log.Info(12);
                string list = builder.ToString();
                if (builder.Length == 0)
                {
                    Log.Info("13-A");
                    Cassie.Message(plugin.Config.ScanNobodyMessage);
                }
                else
                {
                    Log.Info("13-B");
                    int numberHuman = Player.List.Count(Ply => Ply.IsAlive && Ply.Team != Team.SCP && Ply.Team != Team.TUT && !IsGhost(Ply)) + (((List<Player>)Loader.Plugins.FirstOrDefault(pl => pl.Name == "SerpentsHand")?.Assembly.GetType("SerpentsHand.API.SerpentsHand")?.GetMethod("GetSHPlayers")?.Invoke(null, null))?.Count ?? 0) - (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != null ? 1 : 0);
                    Log.Info("14-B");
                    int numberSCPs = Player.List.Count(Ply => Ply.Team == Team.SCP && !IsGhost(Ply)) + (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != null ? 1 : 0);
                    Log.Info("15-B");
                    Cassie.Message(plugin.Config.ScanFinishMessage.Replace("{humanCount}", numberHuman.ToString()).Replace("{scpCount}", numberSCPs.ToString()).Replace("{list}", list));

                }
                Plugin.ScanInProgress = false;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
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
                Log.Info(0);
                yield return Timing.WaitForSeconds(plugin.Config.ScanLength);
                Log.Info(1);
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
                Timing.PauseCoroutines(CHandle);
                Timing.KillCoroutines(CHandle);
            }
        }

        public void OnDetonated()
        {
            if (plugin.Config.ScanAfterNuke == false)
            {
                foreach (CoroutineHandle CHandle in Coroutines)
                {
                    Timing.PauseCoroutines(CHandle);
                    Timing.KillCoroutines(CHandle);
                }
            }
        }
    }
}
