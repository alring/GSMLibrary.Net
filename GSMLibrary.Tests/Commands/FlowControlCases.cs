using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using GSMLibrary.Commands;

namespace GSMLibrary.Tests.Commands.CommandsCases
{
    public class FlowControlCases : WritableBaseCommand, IReadableCommandCases
    {
        private FlowControlCommand _flowControlCommand;

        public FlowControlCases()
        {
            _flowControlCommand = new FlowControlCommand();
            _Command = _flowControlCommand;
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _flowControlCommand.RTSSignalEnabled = true;
            _flowControlCommand.CTSSignalEnabled = true;
            Assert.Equal("AT+IFC=2,2", _flowControlCommand.WriteCommand());
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("AT+IFC?");
            zList.Add("");
            zList.Add("+IFC: 2,2");
            zList.Add("OK");
            Assert.True(_flowControlCommand.Parse(zList));

            Assert.Equal(true, _flowControlCommand.RTSSignalEnabled);
            Assert.Equal(true, _flowControlCommand.CTSSignalEnabled);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();
                        
            zList.Add("+IFdC: 0,0");
            zList.Add("OK");
            Assert.False(_flowControlCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+IFC: 0");
            zList.Add("OK");
            Assert.False(_flowControlCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();
                        
            zList.Add("+IFC: 4,0");
            zList.Add("OK");
            Assert.False(_flowControlCommand.Parse(zList));
        }

        [Fact]
        public void CheckEnCoder()
        {
            Assert.Equal(2, FlowControlCommand.DecodeValue(true));
            Assert.Equal(0, FlowControlCommand.DecodeValue(false));            
        }

        [Fact]
        public void CheckDeCoder()
        {
            Assert.Equal(true, FlowControlCommand.EncodeValue(2));
            Assert.Equal(false, FlowControlCommand.EncodeValue(0));

            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   FlowControlCommand.EncodeValue(1);
               });   
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+IFC?", _flowControlCommand.ReadParamsCommand());
        }
    }
}
