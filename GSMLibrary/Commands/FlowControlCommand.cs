using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace GSMLibrary.Commands
{
    public class FlowControlCommand :BaseReadableCommand, IWritableCommand
    {
        public bool CTSSignalEnabled { get; set; }
        public bool RTSSignalEnabled { get; set; }

        public FlowControlCommand()
        {
            CommandPrefix = "IFC";
        }

        static public bool EncodeValue(ushort aValue)
        {            
            bool zReturn = false;

            switch (aValue)
            {
                case 0:
                    zReturn = false;
                    break;
                case 2:
                    zReturn = true;
                    break;                    
                default:
                    throw new ArgumentOutOfRangeException();                    
            }

            return zReturn;
        }

        static public ushort DecodeValue(bool aValue)
        {
            if (aValue)
                return 2;
            else
                return 0;
        }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {                
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',' });
                if (zSplit.Count() == 3)
                {
                    try
                    {
                        RTSSignalEnabled = FlowControlCommand.EncodeValue(ushort.Parse(TrimValue(zSplit[1])));
                        CTSSignalEnabled = FlowControlCommand.EncodeValue(ushort.Parse(TrimValue(zSplit[2])));

                        return true;
                    }
                    catch (Exception zException)
                    {
                        _logger.WarnException("Handled exception", zException);
                        return false;
                    }
                }
                else
                {
                    _logger.Debug("InCorrect Params Count: {0}", zSplit.Count());
                    return false;
                }
            }
            else
                return false;
        }

        public string WriteCommand()
        {
            return String.Format("AT+{0}={1},{2}", CommandPrefix, DecodeValue(RTSSignalEnabled), DecodeValue(CTSSignalEnabled)); ;
        }
    }
}
