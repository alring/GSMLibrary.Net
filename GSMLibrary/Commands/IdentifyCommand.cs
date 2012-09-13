using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands
{
    enum DeviceComponents
    {
        DWL,
        FW,
        OAT        
    }
    
    public class DeviceComponent
    {
        public DeviceComponent()
        {
            // dummy
        }
        
        public string Name { get; set; }
        public string Version { get; set; }
        public string CompanyName { get; set; }
        public int Size { get; set; }
        public string TimeStamp { get; set; }
        public string CheckSum { get; set; }
        public string OffSet { get; set; }

        public void Parse(string aString)
        {
            string[] zSplit = aString.Split(new Char[] { ',' });

            Version = BaseATCommand.TrimValue(zSplit.ElementAtOrDefault(1));
            Name = BaseATCommand.TrimValue(zSplit.ElementAtOrDefault(2));            
            CompanyName = BaseATCommand.TrimValue(zSplit.ElementAtOrDefault(3));
            int zSize = 0;
            int.TryParse(BaseATCommand.TrimValue(zSplit.ElementAtOrDefault(4)), out zSize);
            Size = zSize;
            TimeStamp = BaseATCommand.TrimValue(zSplit.ElementAtOrDefault(5));
            CheckSum = BaseATCommand.TrimValue(zSplit.ElementAtOrDefault(6));
            OffSet = BaseATCommand.TrimValue(zSplit.ElementAtOrDefault(7));
        }
    }

    public class IdentifyCommand : BaseReadableCommand
    {
        public DeviceComponent Application { get; private set; }
        public DeviceComponent FirmWare { get; private set; }
        
        public IdentifyCommand()
        {
            CommandPrefix = "ATI9";

            Application = new DeviceComponent();
            FirmWare = new DeviceComponent();
        }

        public string ModuleName { get; private set; }

        protected bool ParseFirmware(string aString)
        {
            string[] zSplit = aString.Split(new Char[] { ',' });

            if ((zSplit.Count() >= 2) && (zSplit[0] == "\"FW\""))
            {
                FirmWare.Parse(aString);
                return true;
            }

            return false;
        }

        protected bool ParseOAT(string aString)
        {
            string[] zSplit = aString.Split(new Char[] { ',' });

            if ((zSplit.Count() >= 2) && (zSplit[0] == "\"OAT\""))
            {
                Application.Parse(aString);
                return true;
            }

            return false;
        }

        protected bool ParseModuleName(string aString)
        {
            string[] zSplit = aString.Split(new Char[] { ',' });

            if ((zSplit.Count() == 2) && (zSplit[0] == "\"DWLNAME\""))
            {
                ModuleName = TrimValue(zSplit[1]);
                return true;
            }

            return false;
        }

        public override bool Parse(List<string> aAnswer)
        {
            if (aAnswer.Contains(BASE_ERROR))
                return false;
            else
            {
                if (aAnswer.Contains("OK"))
                {
                    aAnswer.RemoveAll(OKString);
                    aAnswer.RemoveAll(this.ReadParamsClone);
                    aAnswer.RemoveAll(String.IsNullOrEmpty);

                    bool zModuleParseResult = false;
                    bool zFirmwareParseResult = false;
                    bool zAppParseResult = false;

                    foreach (string zItem in aAnswer)
                    {                        
                        zModuleParseResult = zModuleParseResult || ParseModuleName(zItem);
                        zAppParseResult = zAppParseResult || ParseOAT(zItem);
                        zFirmwareParseResult = zFirmwareParseResult || ParseFirmware(zItem);
                    }

                    return zModuleParseResult && zAppParseResult && zFirmwareParseResult;
                }
                else
                    return false;
            }
                
        }

        public override String ReadParamsCommand()
        {
            return String.Concat("ATI9");
        }
    }
}
