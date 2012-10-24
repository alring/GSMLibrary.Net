using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands.TrspSpecific
{
    public class GUIDCommand : BaseReadableCommand, IWritableCommand
    {
        private Guid _DevGUIDprop;

        public GUIDCommand()
        {
            CommandPrefix = "GUID";
        }
            
        public String DeviceGUID
        {
            get
            {
                return String.Concat('{', _DevGUIDprop.ToString().ToUpper(), '}');
            }
            set
            {
                _DevGUIDprop = Guid.Parse(value);
            }
        }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', ',' });
                if (zSplit.Count() == 2)
                {
                    String zCleanGUID = TrimValue(zSplit[1]);
                    if (zCleanGUID.StartsWith("{") && zCleanGUID.EndsWith("}"))
                    {
                        try
                        {
                            DeviceGUID = zCleanGUID;
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
            else
                return false;
        }

        public string WriteCommand()
        {
            return String.Concat("AT+GUID=", DeviceGUID);

        }
    }
}
