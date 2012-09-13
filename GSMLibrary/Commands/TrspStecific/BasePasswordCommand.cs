using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands.TrspSpecific
{
    public class BasePasswordCommand : BaseATCommand, IWritableCommand
    {        
        public const int MAX_PASS_LENGTH = 12;        

        private String _NewPassword;
        public String NewPassword
        {
            set
            {
                if (value == String.Empty)
                    throw new ArgumentNullException();
                else if (value.Length <= MAX_PASS_LENGTH)
                    _NewPassword = value;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private String _OldPassword;
        public String OldPassword
        {            
            set
            {
                if (value == String.Empty)
                    throw new ArgumentNullException();
                else if (value.Length <= MAX_PASS_LENGTH)
                    _OldPassword = value;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string WriteCommand()
        {
            return String.Format("AT+{0}=\"{1}\",\"{2}\"", CommandPrefix, _OldPassword, _NewPassword);
        }
    }
}
