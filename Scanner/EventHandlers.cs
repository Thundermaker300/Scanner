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
using Scanner.Structures;
using Exiled.API.Features.Pools;
using Exiled.API.Enums;

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

        /* GhostSpectator */
        private static bool IsGhost(Player Ply)
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
                if (Ply.CurrentRoom != null && Ply.Role.Team == t && /* GhostSpectator */ !IsGhost(Ply) /*&&*/ /* SCP-035 */ /*Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != Ply*/)
                {
                    bool count = false;
                    foreach (ZoneType zt in plugin.Config.ScanZones)
                    {
                        if (Ply.CurrentRoom.Zone.HasFlag(zt))
                        {
                            count = true;
                        }
                    }

                    if (count)
                        amount++;
                }
            }
            return amount;
        }

        public static CassieMessage GetCassieMessage(Team t)
        {
            int amount = GetNumPlayersInTeam(t);
            if (amount == 0)
            {
                return new(string.Empty, string.Empty);
            }
            return new(
                $"{amount} {(amount > 1 ? plugin.Translation.TeamPronounciationMultiple[t].CassieText : plugin.Translation.TeamPronounciationSingular[t].CassieText)} . ",
                $"{amount} {(amount > 1 ? plugin.Translation.TeamPronounciationMultiple[t].CaptionText : plugin.Translation.TeamPronounciationSingular[t].CaptionText)}, "
            );
        }

        public static CassieMessage GetScpString(RoleTypeId rt, int amount) => new(
            $"{(amount == 1 ? string.Empty : $"{amount} ")}{plugin.Translation.ScpPronounciation[rt].CassieText}",
            $"{(amount == 1 ? string.Empty : $"{amount} ")}{plugin.Translation.ScpPronounciation[rt].CaptionText}"
        );

        public static void Scan()
        {
            try
            {
                StringBuilder builderCassie = StringBuilderPool.Pool.Get();
                StringBuilder builderCaption = StringBuilderPool.Pool.Get();
                foreach (Team t in GetAliveTeams())
                {
                    if (t == Team.SCPs) continue;
                    CassieMessage message = GetCassieMessage(t);
                    builderCassie.Append(message.CassieText);
                    builderCaption.Append(message.CaptionText);
                }
                // SH Support
                var SHPlayers = Player.Get(ply => ply.SessionVariables.ContainsKey("IsSH"));
                if (SHPlayers.Count() > 0)
                {
                    builderCassie.Append($"{SHPlayers.Count()} SERPENTS HAND . ");
                    builderCaption.Append($"{SHPlayers.Count()} Serpent's Hand, ");
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
                        CassieMessage message = GetScpString(item.Key, item.Value);
                        builderCassie.Append(message.CassieText);
                        builderCassie.Append(" . ");

                        builderCaption.Append($"{message.CaptionText}, ");
                    }
                }
                string listCassie = StringBuilderPool.Pool.ToStringReturn(builderCassie);
                string listCaption = StringBuilderPool.Pool.ToStringReturn(builderCaption);
                if (listCassie.Length == 0)
                {
                    Cassie.MessageTranslated(plugin.Translation.ScanNobodyMessage.CassieText, plugin.Translation.ScanNobodyMessage.CaptionText);
                }
                else
                {
                    listCaption = listCaption.Substring(0, listCaption.Length - 2); // Remove stray comma
                    int numberHuman = Player.List.Count(Ply => Ply.IsAlive && Ply.Role.Team != Team.SCPs && Ply.Role.Team != Team.OtherAlive && !IsGhost(Ply)) + Player.Get(ply => ply.SessionVariables.ContainsKey("IsSH")).Count();
                    int numberSCPs = Player.List.Count(Ply => Ply.Role.Team == Team.SCPs && !IsGhost(Ply)) /*+ (Loader.Plugins.FirstOrDefault(pl => pl.Name == "scp035")?.Assembly.GetType("scp035.API.Scp035Data")?.GetMethod("GetScp035")?.Invoke(null, null) != null ? 1 : 0)*/;
                    Cassie.MessageTranslated(
                        plugin.Translation.ScanFinishMessage.CassieText.Replace("{HUMANCOUNT}", numberHuman.ToString()).Replace("{SCPCOUNT}", numberSCPs.ToString()).Replace("{LIST}", listCassie),
                        plugin.Translation.ScanFinishMessage.CaptionText.Replace("{HUMANCOUNT}", numberHuman.ToString()).Replace("{SCPCOUNT}", numberSCPs.ToString()).Replace("{LIST}", listCaption)
                    );

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
            for (; ; )
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
                CassieMessage message = plugin.Translation.ScanStartMessage;
                string cassie = message.CassieText.Replace("{LENGTH}", plugin.Config.ScanLength.ToString());
                string caption = message.CaptionText.Replace("{LENGTH}", plugin.Config.ScanLength.ToString());
                Plugin.ScanInProgress = true;
                Cassie.MessageTranslated(cassie, caption);
                yield return Timing.WaitForSeconds(plugin.Config.ScanLength);
                Scan();
            }
        }

        public void OnRoundStarted()
        {
            Plugin.ScanInProgress = false;
            foreach (CoroutineHandle CHandle in Coroutines)
            {
                Timing.KillCoroutines(CHandle);
            }
            if (plugin.Config.AnnounceScpsAtStart == true)
            {
                Timing.CallDelayed(0.3f + plugin.Config.AnnounceScpsDelay, () =>
                {
                    StringBuilder scpList = StringBuilderPool.Pool.Get();
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
                        scpList.Append(GetScpString(item.Key, item.Value).CassieText);
                        scpList.Append(" . ");
                    }
                    string scpListString = StringBuilderPool.Pool.ToStringReturn(scpList);
                    CassieMessage msg = GetNumPlayersInTeam(Team.SCPs) > 1 ? plugin.Translation.ScpAnnounceStringMultiple : plugin.Translation.ScpAnnounceStringSingular;
                    string str = msg.CassieText.Replace("{list}", scpListString);
                    string caption = msg.CaptionText.Replace("{list}", scpListString);
                    Cassie.MessageTranslated(str, caption);
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
            Plugin.ScanInProgress = false;
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
