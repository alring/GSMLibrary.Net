using System;
using GSMLibrary.Commands.TrspSpecific;
using System.Collections.Generic;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class AppModeCommandTest : WritableBaseCommand, IReadableCommandCases
    {
        private AppModeCommand _AppModeCommand;

        public AppModeCommandTest()
        {
            _AppModeCommand = new AppModeCommand();
            _Command = _AppModeCommand;
        }
        
        [Fact]
        public void ParseCorrect()
        {
            bool zResult;

            List<string> zList = new List<string>();

            zList.Add("AT+MODE?");
            zList.Add("");
            zList.Add("+MODE: \"GPRS\", \"AWAIT\"");
            zList.Add("OK");            

            zResult = _AppModeCommand.Parse(zList);
            Assert.True(zResult);
            Assert.Equal(_AppModeCommand.DeviceConnectionMode, ConnectionMode.GPRS);
            Assert.Equal(_AppModeCommand.DeviceConnectionType, ConnectionType.AWAIT);

            zList.Clear();
            zList.Add("+MODE: \"CSD\", \"AWAIT\"");
            zList.Add("OK");            

            zResult = _AppModeCommand.Parse(zList);
            Assert.True(zResult);
            Assert.Equal(_AppModeCommand.DeviceConnectionMode, ConnectionMode.CSD);
            Assert.Equal(_AppModeCommand.DeviceConnectionType, ConnectionType.AWAIT);

            zList.Clear();
            zList.Add("+MODE: \"GPRS\", \"ACTIVE\"");
            zList.Add("OK");            

            zResult = _AppModeCommand.Parse(zList);
            Assert.True(zResult);
            Assert.Equal(_AppModeCommand.DeviceConnectionMode, ConnectionMode.GPRS);
            Assert.Equal(_AppModeCommand.DeviceConnectionType, ConnectionType.ACTIVE);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+MDE: \"GPRS\", \"AWAIT\"");
            zList.Add("OK");
            
            Assert.False(_AppModeCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+MODE: \"GPRS\"");
            zList.Add("OK");

            Assert.False(_AppModeCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Add("+MODE: \"GPR\", \"AWAIT\"");
            zList.Add("OK");

            Assert.False(_AppModeCommand.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+MODE?", _AppModeCommand.ReadParamsCommand());
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _AppModeCommand.DeviceConnectionMode = ConnectionMode.CSD;
            _AppModeCommand.DeviceConnectionType = ConnectionType.AWAIT;
            Assert.Equal("AT+MODE=\"CSD\",\"AWAIT\"", _AppModeCommand.WriteCommand());

            _AppModeCommand.DeviceConnectionMode = ConnectionMode.GPRS;
            _AppModeCommand.DeviceConnectionType = ConnectionType.AWAIT;
            Assert.Equal("AT+MODE=\"GPRS\",\"AWAIT\"", _AppModeCommand.WriteCommand());

            _AppModeCommand.DeviceConnectionMode = ConnectionMode.GPRS;
            _AppModeCommand.DeviceConnectionType = ConnectionType.ACTIVE;
            Assert.Equal("AT+MODE=\"GPRS\",\"ACTIVE\"", _AppModeCommand.WriteCommand());
        }
    }
}
