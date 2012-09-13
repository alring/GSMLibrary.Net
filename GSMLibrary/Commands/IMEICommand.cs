using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands
{
    public class IMEICommand : BaseReadableCommand
    {
        public IMEICommand()
        {
            CommandPrefix = "IMEI";
        }

        public String DeviceIMEI { get; set; }        

        public override bool Parse(List<string> aAnswer)
        {   
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', ',' });
                if (zSplit.Count() == 2)
                {
                    String zCleanIMEI = TrimValue(zSplit[1]);
                    if (zCleanIMEI.Length == 15)
                    {
                        DeviceIMEI = zCleanIMEI;
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
    }
}
