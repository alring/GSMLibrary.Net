using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSMLibrary.Commands.TrspSpecific;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class ModbusPasswordCases :BasePasswordTest
    {
        public ModbusPasswordCases()
        {
            _PasswordCommand = new ModBusPasswordCommand();
            _Command = _PasswordCommand;
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _PasswordCommand.NewPassword = "NEW";
            _PasswordCommand.OldPassword = "OLD";

            Assert.Equal("AT+MODBUS=\"OLD\",\"NEW\"", _PasswordCommand.WriteCommand());
        }
    }
}
