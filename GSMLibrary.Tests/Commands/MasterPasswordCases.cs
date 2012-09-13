using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using GSMLibrary.Commands.TrspSpecific;

namespace GSMLibrary.Tests.Commands
{
    public class MasterPasswordCases : BasePasswordTest
    {
        public MasterPasswordCases()
        {
            _PasswordCommand = new MasterPasswordCommand();
            _Command = _PasswordCommand;
        }
        
        [Fact]
        public override void WriteCommandTest()
        {
            _PasswordCommand.NewPassword = "NEW";
            _PasswordCommand.OldPassword = "OLD";

            Assert.Equal("AT+MASTERPWD=\"OLD\",\"NEW\"", _PasswordCommand.WriteCommand());
        }
    }
}
