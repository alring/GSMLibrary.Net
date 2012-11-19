using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using GSMLibrary.Commands;

namespace GSMLibrary.Tests.Commands.Commands
{
    public class SignalQualityCommandCases : BaseATCommandCase, IReadableCommandCases
    {
        private SignalQualityCommand _SignalCommand;        

        public SignalQualityCommandCases()
        {
            _SignalCommand = new SignalQualityCommand();
            _Command = _SignalCommand;
        }

        [Fact]
        public void ParseCorrect()
        {
            bool zResult;

            List<string> zList = new List<string>();

            zList.Add("AT+CSQ");
            zList.Add("+CSQ: 0,0");
            zList.Add("OK");

            zResult = _SignalCommand.Parse(zList);
            Assert.Equal(true, zResult);
            Assert.Equal(0, _SignalCommand.Quality);

            zList.Clear();

            zList.Add("AT+CSQ");
            zList.Add("+CSQ: 24,0");
            zList.Add("OK");

            zResult = _SignalCommand.Parse(zList);
            Assert.Equal(true, zResult);
            Assert.Equal(77, _SignalCommand.Quality);

            zList.Clear();

            zList.Add("AT+CSQ");
            zList.Add("+CSQ: 31,0");
            zList.Add("OK");

            zResult = _SignalCommand.Parse(zList);
            Assert.Equal(true, zResult);
            Assert.Equal(100, _SignalCommand.Quality);

            zList.Clear();

            zList.Add("AT+CSQ");
            zList.Add("+CSQ: 35,0");
            zList.Add("OK");

            zResult = _SignalCommand.Parse(zList);
            Assert.Equal(true, zResult);
            Assert.Equal(100, _SignalCommand.Quality);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+CS: 24,0");
            zList.Add("OK");

            Assert.False(_SignalCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+CSQ: 24, 0, 6");
            zList.Add("OK");

            Assert.False(_SignalCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Add("+CSQ: -89,0");
            zList.Add("OK");

            Assert.False(_SignalCommand.Parse(zList));

            zList.Clear();
            zList.Add("+CSQ: 25, 15");
            zList.Add("OK");

            Assert.False(_SignalCommand.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+CSQ", _SignalCommand.ReadParamsCommand());
        }
    }
}
