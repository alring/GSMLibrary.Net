using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands.TrspSpecific
{
    public enum TrspChannelType
    {
        unknown,
        STELAP,
        SIMPLE
    }

    public class TransparentConfigCommand : BaseReadableCommand, IWritableCommand
    {
        public TransparentConfigCommand()
        {
            CommandPrefix = "TRSPCONFIG";
        }
        
        public TrspChannelType ChannelType { get; set; }

        public byte tSilent { get; set; }

        public byte tConnection { get; set; }

        public uint tReadTimeInterval { get; set; }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', ',', ',', ',' });
                if (zSplit.Count() == 5)
                {
                    try
                    {
                        if (TrimValue(zSplit[1]) == "STEL-AP")
                            ChannelType = TrspChannelType.STELAP;
                        else if (TrimValue(zSplit[1]) == "SIMPLE")
                            ChannelType = TrspChannelType.SIMPLE;
                        else
                            return false;
                        
                        tSilent = byte.Parse(TrimValue(zSplit[2]));
                        tConnection = byte.Parse(TrimValue(zSplit[3]));
                        tReadTimeInterval = UInt32.Parse(TrimValue(zSplit[4]));

                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }

        public string WriteCommand()
        {
            string zTypeStr = "";

            switch (ChannelType)
            {   
                case TrspChannelType.STELAP:
                    zTypeStr = "STEL-AP";
                    break;
                case TrspChannelType.SIMPLE:
                    zTypeStr = "SIMPLE";
                    break;                
            }
            
            return String.Concat("AT+", CommandPrefix, "=\"", zTypeStr, "\",", tSilent.ToString(), ",", tConnection.ToString(), ",", tReadTimeInterval.ToString());
        }
    }
}