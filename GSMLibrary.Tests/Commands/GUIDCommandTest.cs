using System;
using GSMLibrary.Commands.TrspSpecific;
using System.Collections.Generic;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class GUIDCommandTest : WritableBaseCommand, IReadableCommandCases
    {
        private GUIDCommand _GUIDCommand;

        public GUIDCommandTest()
        {
            _GUIDCommand = new GUIDCommand();
            _Command = _GUIDCommand;
        }

        [Fact]
        public void CorrectGUDTest()
        {
            _GUIDCommand.DeviceGUID = "{6F9619FF-8B86-D011-B42D-00CF4FC964FF}";
            Assert.Equal("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}", _GUIDCommand.DeviceGUID.ToUpper());
        }

        [Fact]
        public void WrongGUDTest()
        {
            Assert.Throws<System.FormatException>(
                delegate
                {
                    _GUIDCommand.DeviceGUID = "{as6F9619FF-8B86-D011-B42D-00CF4FC964FF}";
                });
        }

        [Fact]
        public void ParseCorrect()
        {
            bool zResult;

            List<string> zList = new List<string>();

            zList.Add("AT+GUID?");
            zList.Add("");
            zList.Add("+GUID: \"{6F9619FF-8B86-D011-B42D-00CF4FC964FF}\"");
            zList.Add("OK");

            zResult = _GUIDCommand.Parse(zList);
            Assert.True(zResult);
            Assert.Equal("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}", _GUIDCommand.DeviceGUID);
        }    

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+GID: \"{6F9619FF-8B86-D011-B42D-00CF4FC964FF}\"");
            zList.Add("OK");

            Assert.False(_GUIDCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+GUID: \"{6F9619FF-8B86-D011-B42D-00CF4FC964sF}\", \"kjkj\"");
            zList.Add("OK");

            Assert.False(_GUIDCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();
                        
            zList.Add("+GUID: \"6F9619FF-8B86-D011-B42D-00CF4FC964FF\"");
            zList.Add("OK");
                        
            Assert.False(_GUIDCommand.Parse(zList));

            zList.Clear();
            zList.Add("+GUID: \"{6F9619FF-8B86-D011-B42D-00CF4FC964sF}\"");
            zList.Add("OK");

            Assert.False(_GUIDCommand.Parse(zList));
        }

        [Fact]
        public  void ReadCommandTest()
        {
            Assert.Equal("AT+GUID?", _GUIDCommand.ReadParamsCommand());
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _GUIDCommand.DeviceGUID = "{235D8DFE-52E8-4A59-9D5C-111B82920EC6}";
            Assert.Equal("AT+GUID={235D8DFE-52E8-4A59-9D5C-111B82920EC6}", _GUIDCommand.WriteCommand());
        }
    }
}
