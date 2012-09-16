using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSMLibrary.Commands
{
    public interface IReadableCommand
    {
        bool Parse(List<string> aAnswer);
        string ReadParamsCommand();
    }

    public class BaseReadableCommand : BaseATCommand, IReadableCommand
    {
        public virtual bool ClarifyAnswer(List<string> aAnswer)
        {   
            if (aAnswer.Contains("OK"))
            {
                aAnswer.RemoveAll(OKString);
                aAnswer.RemoveAll(this.ReadParamsClone);
                aAnswer.RemoveAll(String.IsNullOrEmpty);

                if (aAnswer.Count > 0)
                {
                    return aAnswer[0].StartsWith(String.Concat("+", CommandPrefix));
                }
                else
                    return false;
            }
            else
                return false;
        }

        // Search predicate returns true if a string ends in "OK".
        protected static bool OKString(String s)
        {
            return s.ToUpper().Equals("OK");
        }

        public virtual bool Parse(List<string> aAnswer)
        {
            if (aAnswer.Contains(BASE_ERROR))
                return false;
            else
                return ClarifyAnswer(aAnswer);
        }

        protected bool ReadParamsClone(String s)
        {
            return s.ToUpper().Equals(this.ReadParamsCommand());
        }

        public virtual String ReadParamsCommand()
        {
            return String.Concat("AT+", CommandPrefix, "?");
        }
    }
}
