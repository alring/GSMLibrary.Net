using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSMLibrary.Commands
{
    public class CurrentTimeCommand : BaseReadableCommand, IWritableCommand
    {
        public DateTime DeviceDateTime { get; set; }
        
        public CurrentTimeCommand()
        {
            CommandPrefix = "CCLK";
        }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', '/' });
                if (zSplit.Count() == 7)
                {
                    try
                    {
                        DeviceDateTime = new DateTime(2000 + int.Parse(TrimValue(zSplit[1])), int.Parse(TrimValue(zSplit[2])), int.Parse(TrimValue(zSplit[3])),
                            int.Parse(TrimValue(zSplit[4])), int.Parse(TrimValue(zSplit[5])), int.Parse(TrimValue(zSplit[6])));
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
            int zYear = 0;

            if (DeviceDateTime.Year >= 2000)
                zYear = DeviceDateTime.Year - 2000;

            return String.Format("AT+{0}=\"{1}/{2,2:00}/{3,2:00},{4,2:00}:{5,2:00}:{6,2:00}\"", CommandPrefix, zYear, DeviceDateTime.Month, 
                DeviceDateTime.Day, DeviceDateTime.Hour, DeviceDateTime.Minute, DeviceDateTime.Second);
        }
    }
}
