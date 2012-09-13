using System;
using GSMLibrary.Commands;
using GSMLibrary.Commands.TrspSpecific;
using System.Net;
using System.Collections.Generic;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class GPRSConfigCommandTest : WritableBaseCommand, IReadableCommandCases
    {
        private GPRSConfigCommand _GPRSCommand;
        
        public GPRSConfigCommandTest()
        {
            _GPRSCommand = new GPRSConfigCommand();
            _Command = _GPRSCommand;
        }
        
        [Fact]
        public void APNNameLengthTest()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   _GPRSCommand.APNName = "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "n";
               });            
        }

        [Fact]
        public void APNNameEmptyTest()
        {
            Assert.Throws<System.ArgumentNullException>(
               delegate
               {
                   _GPRSCommand.APNName = "";
               });            
        }

        [Fact]
        public void CorrectAPNNameTest()
        {
            _GPRSCommand.APNName = "APNName";
            Assert.Equal("APNName", _GPRSCommand.APNName);
        }

        [Fact]
        public void APNLoginLengthTest()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                delegate
                {
                    _GPRSCommand.APNLogin = "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "n";
                });            
        }

        [Fact]
        public void CorrectAPNLoginTest()
        {
            _GPRSCommand.APNLogin = "APNLogin";
            Assert.Equal("APNLogin", _GPRSCommand.APNLogin);
        }

        [Fact]
        public void APNPasswordLengthTest()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   _GPRSCommand.APNPassword = "1234567890" + "1234567890" + "1234567890" + "1234567890" + "1234567890" + "n";
               });            
        }

        [Fact]
        public void CorrectAPNPasswordTest()
        {
            _GPRSCommand.APNPassword = "APNPassword";
            Assert.Equal("APNPassword", _GPRSCommand.APNPassword);
        }

        [Fact]
        public void NullExceptServerPortTest()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   _GPRSCommand.ServerPort = 0;
               });
        }

        [Fact]
        public void CorrectServerPortTest()
        {
            _GPRSCommand.ServerPort = 512;            
            Assert.Equal(512, _GPRSCommand.ServerPort);
        }

        [Fact]
        public void WrongFormatDataCenterIPTest()
        {
            Assert.Throws<System.FormatException>(
               delegate
               {
                   _GPRSCommand.DataCenterIP = "";
               });            
        }

        [Fact]
        public void CorrectDataCenterIPTest()
        {
            _GPRSCommand.DataCenterIP = "127.0.0.1";
            Assert.Equal("127.0.0.1", _GPRSCommand.DataCenterIP);
        }

        [Fact]
        public void NullExceptDataCenterPortTest()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
               delegate
               {
                   _GPRSCommand.DataCenterPort = 0;
               });            
        }

        [Fact]
        public void CorrectDataCenterPortTest()
        {
            _GPRSCommand.DataCenterPort = 1024;
            Assert.Equal(1024, _GPRSCommand.DataCenterPort);
        }

        [Fact]
        public void ParseCorrect()
        {
            bool zResult;

            List<string> zList = new List<string>();

            zList.Add("AT+GPRSCONFIG?");
            zList.Add("");
            zList.Add("+GPRSCONFIG: \"APN\", \"Login\", \"Password\", 12000, \"10.0.4.7\", 12300");
            zList.Add("OK");            

            zResult = _GPRSCommand.Parse(zList);
            Assert.Equal(true, zResult);
            Assert.Equal("APN", _GPRSCommand.APNName);
            Assert.Equal("Login", _GPRSCommand.APNLogin);
            Assert.Equal("Password", _GPRSCommand.APNPassword);
            Assert.Equal(12000, _GPRSCommand.ServerPort);
            Assert.Equal("10.0.4.7", _GPRSCommand.DataCenterIP);
            Assert.Equal(12300, _GPRSCommand.DataCenterPort);
        }

        [Fact]
        public void ParseInCorrect()
        {
            bool zResult;

            List<string> zList = new List<string>();

            zList.Add("+GFIG: \"APN\", \"Login\", \"Password\", 12000, \"10.0.4.7\", 12300");
            zList.Add("OK");            

            zResult = _GPRSCommand.Parse(zList);
            Assert.False(zResult);

            zList.Clear();
            zList.Add("+GPRSCONFIG: \"APN\", \"Login\", \"Password\", 12000, \"10.0.4.7\", 12300, \"asdasd\"");
            zList.Add("OK");            

            zResult = _GPRSCommand.Parse(zList);
            Assert.False(zResult);

            zList.Clear();
            zList.Add("+GPRSCONFIG: \"APN\", \"Login\", \"Password\", 120000, \"10.0.4.7\", 12300");
            zList.Add("OK");

            zResult = _GPRSCommand.Parse(zList);
            Assert.False(zResult);

            zList.Clear();
            zList.Add("+GPRSCONFIG: \"APN\", \"Login\", \"Password\", 20000, \"1002.0.4.7\", 12300");
            zList.Add("OK");

            zResult = _GPRSCommand.Parse(zList);
            Assert.False(zResult);

            zList.Clear();
            zList.Add("+GPRSCONFIG: \"APN\", \"Login\", \"Password\", 20000, \"10.0.4.7\", 125300");
            zList.Add("OK");

            zResult = _GPRSCommand.Parse(zList);
            Assert.False(zResult);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+GFIG: \"APN\", \"Login\", \"Password\", 12000, \"10.0.4.7\", 12300");
            zList.Add("OK");
                        
            Assert.False(_GPRSCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+GPRSCONFIG?", _GPRSCommand.ReadParamsCommand());
        }

        [Fact]
        public override void CheckInterfaces()
        {
            base.CheckInterfaces();
            
            Assert.True(_Command is IWritableCommand);
        }

        [Fact]
        public override void WriteCommandTest()
        {
            Assert.True(_Command is IWritableCommand);

            _GPRSCommand.APNName = "turbogprs.kvk";
            _GPRSCommand.APNLogin = "login";
            _GPRSCommand.APNPassword = "pass";

            _GPRSCommand.ServerPort = 2000;
            
            _GPRSCommand.DataCenterIP = "127.0.0.1";
            _GPRSCommand.DataCenterPort = 2300;

            Assert.Equal("AT+GPRSCONFIG=\"turbogprs.kvk\",\"login\",\"pass\",2000,\"127.0.0.1\",2300", _GPRSCommand.WriteCommand());
        }
    }
}
