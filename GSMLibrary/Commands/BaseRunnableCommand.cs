using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSMLibrary.Commands
{
    public interface IRunnableComand
    {
        string RunCommand();
    }
    
    public class BaseRunnableCommand : BaseATCommand, IRunnableComand
    {
        public string RunCommand()
        {
            return String.Format("AT+{0}", CommandPrefix);
        }
    }
}
