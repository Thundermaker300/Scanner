using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace Scanner.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceScan : ICommand
    {
        public string Command => "forcescan";

        public string[] Aliases => new string[] { };

        public string Description => "Forces C.A.S.S.I.E to announce who is alive.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("sc.force"))
            {
                response = "Access denied.";
                return false;
            }
            if (Plugin.ScanInProgress)
            {
                response = "A scan is already in progress.";
                return false;
            }
            EventHandlers.Scan();
            response = "Success.";
            return true;
        }
    }
}
