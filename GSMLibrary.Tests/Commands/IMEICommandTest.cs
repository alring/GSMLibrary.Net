using System;
using GSMLibrary.Commands;
using System.Collections.Generic;
using Xunit;

namespace GSMLibrary.Tests.Commands
{    
    public class IMEICommandTest : BaseATCommandCase, IReadableCommandCases
    {
        private IMEICommand _IMEICommand;

        public IMEICommandTest()
        {
            _IMEICommand = new IMEICommand();
            _Command = _IMEICommand;
        }
        
        [Fact]
        public void ParseCorrect()
        {
            bool zResult;

            List<string> zList = new List<string>();

            zList.Add("AT+IMEI?");
            zList.Add("+IMEI: 123456789012345");
            zList.Add("OK");            

            zResult = _IMEICommand.Parse(zList);
            Assert.Equal(true, zResult);
            Assert.Equal("123456789012345", _IMEICommand.DeviceIMEI);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+IME: 123456789012345");
            zList.Add("OK");
                        
            Assert.False(_IMEICommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+IMEI: 123456789012345, 3423");
            zList.Add("OK");

            Assert.False(_IMEICommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Add("+IMEI: 13423456789012345");
            zList.Add("OK");

            Assert.False(_IMEICommand.Parse(zList));

            zList.Clear();
            zList.Add("+IMEI: 1342345678901234545446546546464646949646354654646547987946465465987");
            zList.Add("OK");

            Assert.False(_IMEICommand.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+IMEI?", _IMEICommand.ReadParamsCommand());
        }
    }
}
