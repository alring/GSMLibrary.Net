using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSMLibrary.Commands;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class CurrentTimeCases : WritableBaseCommand, IReadableCommandCases
    {
        private CurrentTimeCommand _CurTimeCommand;
        
        public CurrentTimeCases()
        {
            _CurTimeCommand = new CurrentTimeCommand();
            _Command = _CurTimeCommand;
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("AT+CCLK?");
            zList.Add("");
            zList.Add("+CCLK: \"12/08/07,11:23:45\"");
            zList.Add("OK");
            Assert.True(_CurTimeCommand.Parse(zList));

            Assert.Equal(_CurTimeCommand.DeviceDateTime, new DateTime(2012, 08, 07, 11, 23, 45, 00));
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("+CsadCLK: \"12/08/07,11:23:45\"");
            zList.Add("OK");
            Assert.False(_CurTimeCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("+CCLK: \"12/08/07,11:23:45\", \"sdf\"");
            zList.Add("OK");
            Assert.False(_CurTimeCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("+CCLK: \"1208/07,11:23:45\"");
            zList.Add("OK");
            Assert.False(_CurTimeCommand.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+CCLK?", _CurTimeCommand.ReadParamsCommand());
        }

        [Fact]
        public override void WriteCommandTest()
        {
            DateTime zDateTime = new DateTime(2012, 08, 07, 11, 23, 45, 00);

            _CurTimeCommand.DeviceDateTime = zDateTime;

            Assert.Equal("AT+CCLK=\"12/08/07,11:23:45\"", _CurTimeCommand.WriteCommand());
        }
    }
}
