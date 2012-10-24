using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSMLibrary.Commands
{
    public class SerialNumberCommand : BaseReadableCommand    
    {     
        public SerialNumberCommand()
        {
            CommandPrefix = "WMSN";
        }

        public String SerialNumber { get; set; }

        public override bool Parse(List<string> aAnswer)
        {
            if (!aAnswer.Contains(BASE_ERROR) && aAnswer.Contains("OK"))
            {
                aAnswer.RemoveAll(OKString);
                aAnswer.RemoveAll(this.ReadParamsClone);
                aAnswer.RemoveAll(String.IsNullOrEmpty);

                if (aAnswer.Count == 1)
                {
                    string[] zSplit = aAnswer[0].Split(new Char[] { ' ', ' ', ' ' });
                    if (zSplit.Count() == 3)
                    {
                        String zCleanSN = TrimValue(zSplit[2]);
                        SerialNumber = zCleanSN;
                        return true;
                    }
                    else
                    {
                        _logger.Debug("InCorrect Params Count: {0}", zSplit.Count());
                        return false;
                    }
                }
                else
                    return false;
            } else
                return false;
        }

        public override String ReadParamsCommand()
        {
            return String.Concat("AT+", CommandPrefix);
        }
    }
}
