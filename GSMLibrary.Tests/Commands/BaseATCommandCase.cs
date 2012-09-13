using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSMLibrary.Commands;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public abstract class BaseATCommandCase
    {
        protected BaseATCommand _Command;
    }

    public abstract class BaseReadableATCommand : BaseATCommandCase
    {
        [Fact]
        public void ErrorHandled()
        {
            List<string> zList = new List<string>();

            zList.Add("ERROR");
            Assert.False(((IReadableCommand)_Command).Parse(zList));
        }

        [Fact]
        public virtual void CheckInterfaces()
        {
            Assert.True(_Command is IReadableCommand);
        }
    }
}
