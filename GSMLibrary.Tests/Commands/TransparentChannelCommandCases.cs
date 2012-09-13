using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSMLibrary.Commands.TrspSpecific;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class TransparentChannelCommandCases : WritableBaseCommand, IReadableCommandCases
    {
        private TransparentConfigCommand _TrspCommand;
        
        public TransparentChannelCommandCases()
        {
            _TrspCommand = new TransparentConfigCommand();
            _Command = _TrspCommand;
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("AT+TRSPCONFIG?");
            zList.Add("");
            zList.Add("+TRSPCONFIG: \"SIMPLE\", 15, 40, 500");
            zList.Add("OK");

            Assert.True(_TrspCommand.Parse(zList));
            Assert.Equal(TrspChannelType.SIMPLE, _TrspCommand.ChannelType);
            Assert.Equal(15, _TrspCommand.tSilent);
            Assert.Equal(40, _TrspCommand.tConnection);
            Assert.Equal<uint>(500, _TrspCommand.tReadTimeInterval);

            zList.Clear();
            zList.Add("+TRSPCONFIG: \"STEL-AP\", 30, 60, 100");
            zList.Add("OK");

            Assert.True(_TrspCommand.Parse(zList));
            Assert.Equal(TrspChannelType.STELAP, _TrspCommand.ChannelType);
            Assert.Equal(30, _TrspCommand.tSilent);
            Assert.Equal(60, _TrspCommand.tConnection);
            Assert.Equal<uint>(100, _TrspCommand.tReadTimeInterval);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+TRSPCFIG: \"SIMPLE\", 15, 40, 500");
            zList.Add("OK");

            Assert.False(_TrspCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+TRSPCONFIG: \"SIMPLE\", 15, 50");
            zList.Add("OK");

            Assert.False(_TrspCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Add("+TRSPCONFIG: \"SIMPasLE\", 15, 40, 500");
            zList.Add("OK");

            Assert.False(_TrspCommand.Parse(zList));

            zList.Clear();
            zList.Add("+TRSPCONFIG: \"SIMPLE\", 4545545, 40, 500");
            zList.Add("OK");

            Assert.False(_TrspCommand.Parse(zList));

            zList.Clear();
            zList.Add("+TRSPCONFIG: \"SIMPLE\", 15, 44440, 500");
            zList.Add("OK");

            Assert.False(_TrspCommand.Parse(zList));

            zList.Clear();
            zList.Add("+TRSPCONFIG: \"SIMPLE\", 15, 44440, 50055454");
            zList.Add("OK");

            Assert.False(_TrspCommand.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+TRSPCONFIG?", _TrspCommand.ReadParamsCommand());
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _TrspCommand.ChannelType = TrspChannelType.STELAP;
            _TrspCommand.tSilent = 15;
            _TrspCommand.tConnection = 40;
            _TrspCommand.tReadTimeInterval = 500;
            Assert.Equal("AT+TRSPCONFIG=\"STEL-AP\",15,40,500", _TrspCommand.WriteCommand());

            _TrspCommand.ChannelType = TrspChannelType.SIMPLE;
            _TrspCommand.tSilent = 15;
            _TrspCommand.tConnection = 40;
            _TrspCommand.tReadTimeInterval = 500;
            Assert.Equal("AT+TRSPCONFIG=\"SIMPLE\",15,40,500", _TrspCommand.WriteCommand());
        }
    }
}
