using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands.TrspSpecific
{
    [Serializable]
    public enum ConnectionMode
    {
        // Сводка:
        //     GSM
        CSD = 0,
        //
        // Сводка:
        //     GPRS
        GPRS = 1,     
    }

    [Serializable]
    public enum ConnectionType
    {
        // Сводка:
        //     GSM
        AWAIT = 0,
        //
        // Сводка:
        //     GPRS
        ACTIVE = 1,     
    }

    public class AppModeCommand : BaseReadableCommand, IWritableCommand
    {
        public AppModeCommand()
        {
            CommandPrefix = "MODE";
        }
        
        public ConnectionMode DeviceConnectionMode { get; set; }

        public ConnectionType DeviceConnectionType { get; set; }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', ','});
                if (zSplit.Count() == 3)
                {
                    String zCleanMode = TrimValue(zSplit[1]);
                    String zCleanType = TrimValue(zSplit[2]);

                    if (Enum.IsDefined(typeof(ConnectionMode), zCleanMode) && Enum.IsDefined(typeof(ConnectionType), zCleanType))
                    {
                        DeviceConnectionMode = (ConnectionMode)Enum.Parse(typeof(ConnectionMode), zCleanMode);
                        DeviceConnectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), zCleanType);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public string WriteCommand()
        {
            string zModeStr = "";
            string zTypeStr = "";

            switch (DeviceConnectionMode)
            {
                case ConnectionMode.CSD:
                    zModeStr = "CSD";
                    break;
                case ConnectionMode.GPRS:
                    zModeStr = "GPRS";
                    break;                
            }

            switch (DeviceConnectionType)
            {
                case ConnectionType.AWAIT:
                    zTypeStr = "AWAIT";
                    break;
                case ConnectionType.ACTIVE:
                    zTypeStr = "ACTIVE";
                    break;                
            }
            
            return String.Concat("AT+", CommandPrefix, "=\"", zModeStr, "\",\"", zTypeStr, "\"");
        }
    }
}
