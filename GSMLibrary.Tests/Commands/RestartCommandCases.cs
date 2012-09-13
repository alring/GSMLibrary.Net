using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using GSMLibrary.Commands.TrspSpecific;

namespace GSMLibrary.Tests.Commands
{
    public class RestartCommandCases
    {
        [Fact]
        public void CheckRunCommand()
        {
            RestartCommand zCommand = new RestartCommand();
            Assert.Equal("AT+TRSPSTART", zCommand.RunCommand());
        }
    }
}
