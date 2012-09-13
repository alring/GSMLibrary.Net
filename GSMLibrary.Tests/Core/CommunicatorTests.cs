using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Reflection;
using System.IO.Ports;
using GSMLibrary.Core;

namespace GSMLibrary.Tests.Core
{
    public class CommunicatorCases
    {
        private void SetDefaultSettings()
        {
            Communicator.Instance.BaudRate = 19200;
            Communicator.Instance.DataBits = 7;
            Communicator.Instance.Parity = System.IO.Ports.Parity.Mark;
            Communicator.Instance.StopBits = System.IO.Ports.StopBits.OnePointFive;
            Communicator.Instance.PortName = "COM5";
        }
        
        [Fact]
        public void SettingsCheck()
        {
            SetDefaultSettings();

            Assert.Equal(19200, Communicator.Instance.BaudRate);
            Assert.Equal(7, Communicator.Instance.DataBits);
            Assert.Equal(System.IO.Ports.Parity.Mark, Communicator.Instance.Parity);
            Assert.Equal(System.IO.Ports.StopBits.OnePointFive, Communicator.Instance.StopBits);
            Assert.Equal("COM5", Communicator.Instance.PortName);

            SerialPortSettings zSettings = (SerialPortSettings) typeof(Communicator).GetField("_serialSettings", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Communicator.Instance);
            Assert.Equal(19200, zSettings.BaudRate);
            Assert.Equal(7, zSettings.DataBits);
            Assert.Equal(System.IO.Ports.Parity.Mark, zSettings.Parity);
            Assert.Equal(System.IO.Ports.StopBits.OnePointFive, zSettings.StopBits);
            Assert.Equal("COM5", zSettings.PortName);
        }

        [Fact]
        public void CheckSerialSets()
        {
            SetDefaultSettings();

            SerialPort zPort = (SerialPort)typeof(Communicator).GetField("_serialPort", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Communicator.Instance);
            
            Assert.NotEqual(19200, zPort.BaudRate);
            Assert.NotEqual(7, zPort.DataBits);
            Assert.NotEqual(System.IO.Ports.Parity.Mark, zPort.Parity);
            Assert.NotEqual(System.IO.Ports.StopBits.OnePointFive, zPort.StopBits);
            Assert.NotEqual("COM5", zPort.PortName);

            Communicator.Instance.ApplyPortSettings();

            zPort = (SerialPort)typeof(Communicator).GetField("_serialPort", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Communicator.Instance);

            Assert.Equal(19200, zPort.BaudRate);
            Assert.Equal(7, zPort.DataBits);
            Assert.Equal(System.IO.Ports.Parity.Mark, zPort.Parity);
            Assert.Equal(System.IO.Ports.StopBits.OnePointFive, zPort.StopBits);
            Assert.Equal("COM5", zPort.PortName);
        }

        [Fact]
        public void LoadAndSave()
        {
            SetDefaultSettings();

            Assert.True(Communicator.Instance.SaveSettings());            

            Communicator.Instance.BaudRate = 2400;
            Communicator.Instance.DataBits = 8;
            Communicator.Instance.Parity = System.IO.Ports.Parity.Space;
            Communicator.Instance.StopBits = System.IO.Ports.StopBits.One;
            Communicator.Instance.PortName = "COM7";

            Assert.True(Communicator.Instance.LoadSettings());

            Assert.Equal(19200, Communicator.Instance.BaudRate);
            Assert.Equal(7, Communicator.Instance.DataBits);
            Assert.Equal(System.IO.Ports.Parity.Mark, Communicator.Instance.Parity);
            Assert.Equal(System.IO.Ports.StopBits.OnePointFive, Communicator.Instance.StopBits);
            Assert.Equal("COM5", Communicator.Instance.PortName);
        }

        [Fact]
        public void FlushBufferExceptionTest()
        {
            Assert.Throws<System.InvalidOperationException>(
               delegate
               {
                   Communicator.Instance.FlushBuffer();
               });            
        }

        [Fact]
        public void WriteExceptionTests()
        {
            Assert.Throws<System.InvalidOperationException>(
               delegate
               {
                   Communicator.Instance.Write(new byte[] {1, 2, 3}, 0, 3);
               });
        }

        [Fact]
        public void WriteLineExceptionTest()
        {
            Assert.Throws<System.InvalidOperationException>(
               delegate
               {
                   Communicator.Instance.WriteLine("Test");
               });
        }

        [Fact]
        public void ReadLineExceptionTest()
        {
            Assert.Throws<System.InvalidOperationException>(
               delegate
               {
                   Communicator.Instance.ReadLine();
               });
        }
    }
}
