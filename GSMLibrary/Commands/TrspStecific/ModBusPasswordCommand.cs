using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands.TrspSpecific
{
    public class ModBusPasswordCommand : BasePasswordCommand
    {
        public ModBusPasswordCommand()
        {
            CommandPrefix = "MODBUS";
        }
    }
}
