using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Exiled.API.Features;
using Exiled.Loader;
using Exiled.Events.EventArgs;
using MEC;
using PlayerRoles;
using Exiled.Events.EventArgs.Server;

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
                if (Ply.IsAlive && Ply.Role.Team is not Team.OtherAlive && !AliveRoles.Contains(Ply.Role.Team))
                {
                    AliveRoles.Add(Ply.Role.Team);
                }
            }
            return AliveRoles;
        }

        /* GhostSpectator */ private static bool IsGhost(Player Ply)
        {/*
            Assembly assembly = Loader.Plugins.FirstOrDefault(pl => pl.Name == "GhostSpectator")?.Assembly;
            if (assembly == null) return false;
            return ((bool)assembly.GetType("GhostSpectator.API")?.GetMethod("IsGhost")?.Invoke(null, new object[] { Ply })) == true;*/
            return false;
        }

        private static int GetNumPlayersInTeam(Team t)
        {
            int amount = 0;
            foreach (Player Ply in Player.List)
            {
                if (plugin.Config.ScanZones.Contains(Ply.CurrentRoom.Zone) && Ply.Role.Team == t && /* GhostSpectator */ !IsGhost(Ply) && /* SCP-035 */ Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != Ply)
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

        public static string GetScpString(RoleTypeId rt, int amount) => $"{(amount == 1 ? string.Empty : $"{amount} ")}{plugin.Config.ScpPronounciation[rt]}";

        public static void Scan()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                foreach (Team t in GetAliveTeams())
                {
                    if (t == Team.SCPs) continue;
                    builder.Append(GetCassieMessage(t));
                }
                // SH Support
                var SHPlayers = Player.Get(ply => ply.SessionVariables.ContainsKey("IsSH"));
                if (SHPlayers.Count() > 0)
                {
                    builder.Append($"{SHPlayers.Count()} SERPENTS HAND . ");
                }
                if (plugin.Config.IncludeScpListInScan == true)
                {
                    // SCP-035 support
                    /*Player Scp035 = Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) as Player;
                    if (Scp035 != null)
                    {
                        builder.Append("SCP 0 3 5 . ");
                    }*/
                    Dictionary<RoleTypeId, int> ScpCount = new Dictionary<RoleTypeId, int> { };
                    foreach (Player Ply in Player.List.Where(Ply => Ply.Role.Team is Team.SCPs))
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
                    foreach (KeyValuePair<RoleTypeId, int> item in ScpCount)
                    {
                        builder.Append(GetScpString(item.Key, item.Value));
                        builder.Append(" . ");
                    }
                }
                string list = builder.ToString();
                if (builder.Length == 0)
                {
                    Cassie.Message(plugin.Config.ScanNobodyMessage);
                }
                else
                {
                    int numberHuman = Player.List.Count(Ply => Ply.IsAlive && Ply.Role.Team != Team.SCPs && Ply.Role.Team != Team.OtherAlive && !IsGhost(Ply)) + Player.Get(ply => ply.SessionVariables.ContainsKey("IsSH")).Count();
                    int numberSCPs = Player.List.Count(Ply => Ply.Role.Team == Team.SCPs && !IsGhost(Ply)) /*+ (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != null ? 1 : 0)*/;
                    Cassie.Message(plugin.Config.ScanFinishMessage.Replace("{HUMANCOUNT}", numberHuman.ToString()).Replace("{SCPCOUNT}", numberSCPs.ToString()).Replace("{LIST}", list));

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
                Cassie.Message(plugin.Config.ScanStartMessage.Replace("{LENGTH}", plugin.Config.ScanLength.ToString()));
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
                    Dictionary<RoleTypeId, int> ScpCount = new Dictionary<RoleTypeId, int> { };
                    foreach (Player Ply in Player.List.Where(Ply => Ply.Role.Team == Team.SCPs))
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
                    if (ScpCount.Count < 1)
                    {
                        return;
                    }
                    foreach (KeyValuePair<RoleTypeId, int> item in ScpCount)
                    {
                        scpList.Append(GetScpString(item.Key, item.Value));
                        scpList.Append(" . ");
                    }
                    string str = (GetNumPlayersInTeam(Team.SCPs) > 1 ? plugin.Config.ScpAnnounceStringMultiple : plugin.Config.ScpAnnounceStringSingular).Replace("{list}", scpList.ToString());
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

        public void OnDetonated()
        {
            if (plugin.Config.ScanAfterNuke == false)
            {
                foreach (CoroutineHandle CHandle in Coroutines)
                {
                    Timing.KillCoroutines(CHandle);
                }
            }
        }
    }
}
