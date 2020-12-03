# Scanner
An SCP:SL plugin that scans the facility and announces who is still alive. Requires Exiled 2.1.18 or higher.

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
| scan_finish_message          | string                       | The C.A.S.S.I.E announcement to play when a scan has finished. Replaces `{humanCount}` with the amount of humans, `{scpCount}` with the amount of SCPs, and `{list}` with a full list of alive classes. |
| scan_nobody_message          | string                       | The C.A.S.S.I.E announcement to play when a scan has finished and nobody is alive.                                                                                                                      |
| include_scp_list_in_scan     | bool                         | Determines whether or not a list of SCPs will be announced in the `{list}` variable.                                                                                                                    |
| scan_after_nuke              | bool                         | If set to `false`, scans will be disabled after the nuke has been detonated.                                                                                                                            |
| scan_zones                   | List<ZoneType>               | Determines which zones are scanned. Removing a zone from this list will not include users in the zone in the announcement.                                                                              |
| team_pronounciation_singular | Dictionary<RoleType, string> | Determines how C.A.S.S.I.E pronounces single teams (eg. 1 SCIENTIST).                                                                                                                                   |
| team_pronounciation_multiple | Dictionary<RoleType, string> | Determines how C.A.S.S.I.E pronounces plural teams (eg. 5 SCIENTISTS).                                                                                                                                  |
| scp_pronounciation           | Dictionary<RoleType, string> | Determines how C.A.S.S.I.E pronounces SCPs.                                                                                                                                                             |

## Supported Plugins
* [SCP-035](https://github.com/Cyanox62/scp035)
* [Serpent's Hand](https://github.com/Cyanox62/SerpentsHand)
* [GhostSpectator](https://github.com/Thundermaker300/GhostSpectator)