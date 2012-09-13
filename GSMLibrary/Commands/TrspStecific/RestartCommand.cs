using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSMLibrary.Commands;

namespace GSMLibrary.Commands.TrspSpecific
{
    public class RestartCommand : BaseRunnableCommand
    {
        public RestartCommand()
        {
            CommandPrefix = "TRSPSTART";
        }
    }
}
