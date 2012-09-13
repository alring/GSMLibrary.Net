using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands
{
    public class BaseATCommand
    {
        public const string BASE_ERROR = "ERROR";

        protected String CommandPrefix;

        public static String TrimValue(String aValue)
        {
            return aValue.Trim(new Char[] { '"', ' ' });
        }

        public static bool PositiveAnswer(string aAnswer)
        {
            return aAnswer.Contains("OK");
        }

        public static bool CheckCommandAnswer(string aAnswer)
        {
            return aAnswer.Contains("OK") || aAnswer.Contains("ERROR");
        }
    }
}
