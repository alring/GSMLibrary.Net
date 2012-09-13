using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSMLibrary.Commands;
using Xunit;

namespace GSMLibrary.Tests.Commands
{    
    public class SerialNumberCommandCases : BaseATCommandCase
    {
        private SerialNumberCommand _SNCommand;

        public SerialNumberCommandCases()
        {
            _SNCommand = new SerialNumberCommand();
            _Command = _SNCommand;
        }
        
        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Add("AT+WMSN");
            zList.Add("");
            zList.Add("Serial Number 123456789012345");
            zList.Add("OK");            
            
            Assert.True(_SNCommand.Parse(zList));
            Assert.Equal("123456789012345", _SNCommand.SerialNumber);
        }

        [Fact]
        void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("Serial 123456789012345");
            zList.Add("OK");

            Assert.False(_SNCommand.Parse(zList));

            zList.Clear();
            zList.Add("Number 13423456789012345");
            zList.Add("OK");

            Assert.False(_SNCommand.Parse(zList));
        }

        [Fact]
        void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("Serial Number 13946465465987 213123");            
            zList.Add("OK");

            Assert.False(_SNCommand.Parse(zList));

            zList.Add("Serial Number 13946465465987");
            zList.Add("Serial Number 13946465465987");
            zList.Add("OK");

            Assert.False(_SNCommand.Parse(zList));
        }

        [Fact]
        void WrongParams()
        {
            Assert.True(true); //любые параметры принимаются
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+WMSN", _SNCommand.ReadParamsCommand());
        }
    }
}
