using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace GSMLibrary.Commands
{
    public class BaseATCommand
    {
        public const string BASE_ERROR = "ERROR";

        protected String CommandPrefix;
        protected Logger _logger;

        public BaseATCommand()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public static String TrimValue(String aValue)
        {
            return aValue.Trim(new Char[] { '"', ' ' });
        }

        public static bool PositiveAnswer(string aAnswer)
        {
            return aAnswer.Equals("OK");
        }

        public static bool CheckCommandAnswer(string aAnswer)
        {
            return aAnswer.Contains("OK") || aAnswer.Contains("ERROR");
        }
    }
}
