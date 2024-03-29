![VERSION](https://img.shields.io/github/v/release/Thundermaker300/Scanner?include_prereleases&style=for-the-badge)
![DOWNLOADS](https://img.shields.io/github/downloads/Thundermaker300/Scanner/total?style=for-the-badge)
[![DISCORD](https://img.shields.io/discord/1060274824330620979?label=Discord&style=for-the-badge)](https://discord.gg/3j54zBnbbD)

# Scanner
An SCP:SL plugin that scans the facility and announces who is still alive. Requires Exiled 6.0.

## Supported Plugins
* [SCP-035](https://github.com/Exiled-Team/scp035) (SCP-035 will be announced if `include_scp_list_in_scan` is set to `true`) **(WAITING FOR UPDATE TO PLUGIN)**
* [Serpent's Hand](https://github.com/Exiled-Team/SerpentsHand) (Serpent's Hand will be announced with the rest of the human classes)
* GhostSpectator (Ghosts will not be announced at all) **(WAITING FOR UPDATE TO PLUGIN)**

## Config
| Name                         | Type                         | Description                                                                                                                                                                                             |
|------------------------------|------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| is_enabled                   | bool                         | Enables the plugin.                                                                                                                                                                                     |
| announce_scps_at_start       | bool                         | Determines whether or not the breached SCPs will be announced at the start.                                                                                                                             |
| announce_scps_delay          | int                          | Determines the delay after the round starts that the SCPs will be announced.                                                                                                                            |
| scp_announce_string_singular | string                       | The C.A.S.S.I.E announcement to play when there is one breached SCP.                                                                                                                                    |
| scp_announce_string_multiple | string                       | The C.A.S.S.I.E announcement to play when there is more than one breached SCP.                                                                                                                          |
| regular_scanning             | bool                         | Determines whether or not C.A.S.S.I.E will regularly announce who is alive.                                                                                                                             |
| length_between_scans         | int                          | Determines the amount of seconds to wait in between each scan (and before the first scan)                                                                                                               |
| scan_start_message           | string                       | The C.A.S.S.I.E announcement to play when a scan has started.                                                                                                                                           |
| scan_length                  | int                          | Determines how long it takes C.A.S.S.I.E to complete a scan.                                                                                                                                            |
| scan_finish_message          | string                       | The C.A.S.S.I.E announcement to play when a scan has finished. Replaces `{HUMANCOUNT}` with the amount of humans, `{SCPCOUNT}` with the amount of SCPs, and `{LIST}` with a full list of alive classes. |
| scan_nobody_message          | string                       | The C.A.S.S.I.E announcement to play when a scan has finished and nobody is alive.                                                                                                                      |
| include_scp_list_in_scan     | bool                         | Determines whether or not a list of SCPs will be announced in the `{LIST}` variable.                                                                                                                    |
| scan_after_nuke              | bool                         | If set to `false`, scans will be disabled after the nuke has been detonated.                                                                                                                            |
| scan_zones                   | List<ZoneType>               | Determines which zones are scanned. Removing a zone from this list will not include users in the zone in the announcement.                                                                              |
| team_pronounciation_singular | Dictionary<RoleType, string> | Determines how C.A.S.S.I.E pronounces single teams (eg. 1 SCIENTIST).                                                                                                                                   |
| team_pronounciation_multiple | Dictionary<RoleType, string> | Determines how C.A.S.S.I.E pronounces plural teams (eg. 5 SCIENTISTS).                                                                                                                                  |
| scp_pronounciation           | Dictionary<RoleType, string> | Determines how C.A.S.S.I.E pronounces SCPs.                                                                                                                                                             |
