using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GSMLibrary.Commands.TrspSpecific
{
    public class GPRSConfigCommand : BaseReadableCommand, IWritableCommand
    {
        public const int MAX_APN_NAME_LENGTH = 96;
        public const int MAX_APN_LOGIN_LENGTH = 50;
        public const int MAX_APN_PASSWORD_LENGTH = 50;
        public const String DEFAULT_APN_NAME = "turbogprs.kvk";
        public const UInt16 DEFAULT_SERVER_PORT = 2000;
        public const UInt16 DEFAULT_DATACENTER_PORT = 2300;

        public GPRSConfigCommand()
        {            
            CommandPrefix = "GPRSCONFIG";
        }
        
        private String _APNName;
        public String APNName
        {               
            get
            {
                return _APNName;
            }
            set
            {
                if (value == String.Empty)
                    throw new ArgumentNullException(String.Concat("имя APN"));
                else if (value.Length <= MAX_APN_NAME_LENGTH)
                    _APNName = value;
                else
                    throw new ArgumentOutOfRangeException(String.Concat("имя APN"));
            }
        }

        private String _APNLogin;
        public String APNLogin
        {
            get
            {
                return _APNLogin;
            }
            set
            {
                if (value.Length <= MAX_APN_LOGIN_LENGTH)
                    _APNLogin = value;
                else
                    throw new ArgumentOutOfRangeException(String.Concat("APN login не может быть длиннее (", MAX_APN_LOGIN_LENGTH.ToString(), ") символов"));
            }
        }

        private String _APNPassword;
        public String APNPassword
        {
            get
            {
                return _APNPassword;
            }
            set
            {
                if (value.Length <= MAX_APN_LOGIN_LENGTH)
                    _APNPassword = value;
                else
                    throw new ArgumentOutOfRangeException(String.Concat("пароль к APN не может быть длиннее (", MAX_APN_PASSWORD_LENGTH.ToString(), ") символов"));
            }
        }

        private ushort _ServerPort;
        public ushort ServerPort {
            get 
            {
                return _ServerPort;
            }
            set 
            { 
                if (value != 0) 
                    _ServerPort = value;
                else 
                    throw new ArgumentOutOfRangeException("порт серверного сокета");
            }
        }

        private IPAddress _IPAdressprop;
        public String DataCenterIP
        {
            get
            {               
                return _IPAdressprop.ToString();                
            }
            set
            {
                _IPAdressprop = IPAddress.Parse(value);
            }
        }

        private ushort _DataCenterPort;
        public ushort DataCenterPort
        {
            get
            {
                return _DataCenterPort;
            }
            set
            {
                if (value != 0)
                    _DataCenterPort = value;
                else
                    throw new ArgumentOutOfRangeException("порт датацентра");
            }
        }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', ','});
                if (zSplit.Count() == 7)
                {
                    try
                    {
                        _APNName = TrimValue(zSplit[1]);
                        _APNLogin = TrimValue(zSplit[2]);
                        _APNPassword = TrimValue(zSplit[3]);
                        _ServerPort = ushort.Parse(zSplit[4]);
                        _IPAdressprop = IPAddress.Parse(TrimValue(zSplit[5]));
                        _DataCenterPort = ushort.Parse(zSplit[6]);
                        
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
            return String.Concat("AT+", CommandPrefix, "=\"", APNName, "\",\"", APNLogin, "\",\"", APNPassword, "\",", ServerPort.ToString(), ",\"", DataCenterIP.ToString(), "\"," + DataCenterPort.ToString()); 
        }
    }
}
