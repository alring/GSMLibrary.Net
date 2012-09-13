using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using GSMLibrary.Commands.TrspSpecific;

namespace GSMLibrary.Tests.Commands
{
    public class Uart1ConfigCases : WritableBaseCommand, IReadableCommandCases
    {
        private Uart1ConfigCommand _uart1Command;

        public Uart1ConfigCases()
        {
            _uart1Command = new Uart1ConfigCommand();
            _Command = _uart1Command;
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("AT+UARTCONFIG?");
            zList.Add("");
            zList.Add("+UARTCONFIG: 19200,3,4,0,2,500");
            zList.Add("OK");
            Assert.True(_uart1Command.Parse(zList));
            
            zList.Clear();
            zList.Add("+UARTCONFIG: 19200,3,4,0,2,500");
            zList.Add("OK");
            Assert.True(_uart1Command.Parse(zList));
            Assert.Equal<uint>(19200, _uart1Command.UARTBaudRate);
            Assert.Equal(8, _uart1Command.UARTDataBits);
            Assert.Equal(System.IO.Ports.Parity.None, _uart1Command.UARTParity);
            Assert.Equal(System.IO.Ports.StopBits.One, _uart1Command.UARTStopBits);
            Assert.Equal(false, _uart1Command.RTSSignalEnabled);
            Assert.Equal(true, _uart1Command.CTSSignalEnabled);
            Assert.Equal(500, _uart1Command.UARTReadTimerInterval);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+UARTasdCONFIG: 19200,3,4,0,0,500");
            zList.Add("OK");
            Assert.False(_uart1Command.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+UARTCONFIG: 19200,5,4,0,0");
            zList.Add("OK");
            Assert.False(_uart1Command.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("+UARTCONFIG: 19200,5,4,0,0500");
            zList.Add("OK");
            Assert.False(_uart1Command.Parse(zList));

            zList.Clear();
            zList.Add("+UARTCONFIG: 19200,3,8,0,0,500");
            zList.Add("OK");
            Assert.False(_uart1Command.Parse(zList));

            zList.Clear();
            zList.Add("+UARTCONFIG: 19200,3,4,0,3,500");
            zList.Add("OK");
            Assert.False(_uart1Command.Parse(zList));

            zList.Clear();
            zList.Add("+UARTCONFIG: 19200,3,4,1,0,500");
            zList.Add("OK");
            Assert.False(_uart1Command.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+UARTCONFIG?", _uart1Command.ReadParamsCommand());
        }


        [Fact]
        public override void WriteCommandTest()
        {
            _uart1Command.UARTBaudRate = 115200;
            _uart1Command.UARTDataBits = 8;
            _uart1Command.UARTParity = System.IO.Ports.Parity.None;
            _uart1Command.UARTStopBits = System.IO.Ports.StopBits.One;
            _uart1Command.RTSSignalEnabled = false;
            _uart1Command.CTSSignalEnabled = false;
            _uart1Command.UARTReadTimerInterval = 100;

            Assert.Equal("AT+UARTCONFIG=115200,3,4,0,0,100", _uart1Command.WriteCommand());

            Assert.Throws<System.ArgumentOutOfRangeException>(
                delegate
                {
                    _uart1Command.UARTStopBits = System.IO.Ports.StopBits.OnePointFive;
                    _uart1Command.WriteCommand();
                });
        }
    }

    public class Uart2ConfigCases : WritableBaseCommand, IReadableCommandCases
    {
        private Uart2ConfigCommand _uart2Command;

        public Uart2ConfigCases()
        {
            _uart2Command = new Uart2ConfigCommand();
            _Command = _uart2Command;
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("AT+UART2CONFIG?");
            zList.Add("");
            zList.Add("+UART2CONFIG: 19200,3,4,500");
            zList.Add("OK");
            Assert.True(_uart2Command.Parse(zList));            

            zList.Clear();
            zList.Add("+UART2CONFIG: 19200,3,4,500");
            zList.Add("OK");
            Assert.True(_uart2Command.Parse(zList));
            Assert.Equal<uint>(19200, _uart2Command.UARTBaudRate);
            Assert.Equal(8, _uart2Command.UARTDataBits);
            Assert.Equal(System.IO.Ports.Parity.None, _uart2Command.UARTParity);
            Assert.Equal(System.IO.Ports.StopBits.One, _uart2Command.UARTStopBits);            
            Assert.Equal(500, _uart2Command.UARTReadTimerInterval);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+UART2asdCONFIG: 19200,3,4,500");
            zList.Add("OK");
            Assert.False(_uart2Command.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+UART2CONFIG: 19200,5,4");
            zList.Add("OK");
            Assert.False(_uart2Command.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Add("+UART2CONFIG: 19200,9,4,500");
            zList.Add("OK");
            Assert.False(_uart2Command.Parse(zList));

            zList.Clear();
            zList.Add("+UART2CONFIG: 19200,3,8,500");
            zList.Add("OK");
            Assert.False(_uart2Command.Parse(zList));

            zList.Clear();
            zList.Add("+UART2CONFIG: 19200,39999999999,8,500"); // проверка на исключение при приведении типов
            zList.Add("OK");
            Assert.False(_uart2Command.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+UART2CONFIG?", _uart2Command.ReadParamsCommand());
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _uart2Command.UARTBaudRate = 115200;
            _uart2Command.UARTDataBits = 8;
            _uart2Command.UARTParity = System.IO.Ports.Parity.None;
            _uart2Command.UARTStopBits = System.IO.Ports.StopBits.One;            
            _uart2Command.UARTReadTimerInterval = 100;

            Assert.Equal("AT+UART2CONFIG=115200,3,4,100", _uart2Command.WriteCommand());

            Assert.Throws<System.ArgumentOutOfRangeException>(
                delegate
                {
                    _uart2Command.UARTStopBits = System.IO.Ports.StopBits.OnePointFive;
                    _uart2Command.WriteCommand();
                });
        }
    }
}
