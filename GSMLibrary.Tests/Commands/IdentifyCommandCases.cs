using GSMLibrary.Commands;
using System.Collections.Generic;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class IdentifyCommandCases : BaseATCommandCase, IReadableCommandCases
    {
        private IdentifyCommand _IdentifyCommand;

        public IdentifyCommandCases()
        {
            _IdentifyCommand = new IdentifyCommand();
            _Command = _IdentifyCommand;
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("ATI9");
            zList.Add("");
            zList.Add("\"DWL\",\"V08b11\",\"\",\"Sierra Wireless\",55236,\"122210 15:25\",\"e8a16b54\",\"00010000\"");
            zList.Add("\"FW\",\"FW_SRC_746_8.Q2687H\",\"R7.46.0.201108091301.FSU001\",\"Sierra Wireless\",2216044,\"080911 13:01\",\"2f1beedd\",\"00020000\"");
            zList.Add("\"OAT\",\"0.98a\",\"TransparentChannel\",\"OOO NPP 'Turbotron'\",148356,\"090612 13:30\",\"7d270e41\",\"00260000\"");
            zList.Add("-\"Developer Studio\",\"2.2.1.201206182209-R9667\"");
            zList.Add("-\"Open AT Embedded Software Suite package\",\"2.36.0.201108300638\"");
            zList.Add("-\"Open AT OS Package\",\"6.36.0.201108111228\"");
            zList.Add("-\"Firmware Package\",\"7.46.0.201108091301\"");
            zList.Add("-\"WIP Plug-in Package\",\"5.42.0.201108100923\"");
            zList.Add("\"ROM\",\"400000\"");  
            zList.Add("\"RAM\",\"100000\"");
            zList.Add("\"DWLNAME\",\"FSU001\"");
            zList.Add("OK");
            
            Assert.True(_IdentifyCommand.Parse(zList));

            Assert.Equal("FW_SRC_746_8.Q2687H", _IdentifyCommand.FirmWare.Version);
            Assert.Equal("R7.46.0.201108091301.FSU001", _IdentifyCommand.FirmWare.Name);
            Assert.Equal("Sierra Wireless", _IdentifyCommand.FirmWare.CompanyName);
            Assert.Equal(2216044, _IdentifyCommand.FirmWare.Size);
            Assert.Equal("080911 13:01", _IdentifyCommand.FirmWare.TimeStamp);
            Assert.Equal("2f1beedd", _IdentifyCommand.FirmWare.CheckSum);
            Assert.Equal("00020000", _IdentifyCommand.FirmWare.OffSet);

            Assert.Equal("0.98a", _IdentifyCommand.Application.Version);
            Assert.Equal("TransparentChannel", _IdentifyCommand.Application.Name);
            Assert.Equal("OOO NPP 'Turbotron'", _IdentifyCommand.Application.CompanyName);
            Assert.Equal(148356, _IdentifyCommand.Application.Size);
            Assert.Equal("090612 13:30", _IdentifyCommand.Application.TimeStamp);
            Assert.Equal("7d270e41", _IdentifyCommand.Application.CheckSum);
            Assert.Equal("00260000", _IdentifyCommand.Application.OffSet);
            
            Assert.Equal("FSU001", _IdentifyCommand.ModuleName);
        }

        [Fact]
        public void WrongPrefix()
        {
            Assert.True(true); // нет префикса
        }

        [Fact]
        public void WrongParamsCount()
        {
            Assert.True(true); // переменное число параметров
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("\"DWL\",\"V08b11\",\"\",\"Sierra Wireless\",55236,\"122210 15:25\",\"e8a16b54\",\"00010000\"");
            zList.Add("\"FfW\",\"FW_SRC_746_8.Q2687H\",\"R7.46.0.201108091301.FSU001\",\"Sierra Wireless\",2216044,\"080911 13:01\",\"2f1beedd\",\"00020000\"");
            zList.Add("\"OAT\",\"0.98a\",\"TransparentChannel\",\"OOO NPP 'Turbotron'\",148356,\"090612 13:30\",\"7d270e41\",\"00260000\"");
            zList.Add("-\"Developer Studio\",\"2.2.1.201206182209-R9667\"");
            zList.Add("-\"Open AT Embedded Software Suite package\",\"2.36.0.201108300638\"");
            zList.Add("-\"Open AT OS Package\",\"6.36.0.201108111228\"");
            zList.Add("-\"Firmware Package\",\"7.46.0.201108091301\"");
            zList.Add("-\"WIP Plug-in Package\",\"5.42.0.201108100923\"");
            zList.Add("\"ROM\",\"400000\"");
            zList.Add("\"RAM\",\"100000\"");
            zList.Add("\"DWLNAME\",\"FSU001\"");
            zList.Add("OK");

            Assert.False(_IdentifyCommand.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("ATI9", _IdentifyCommand.ReadParamsCommand());
        }
    }
}
