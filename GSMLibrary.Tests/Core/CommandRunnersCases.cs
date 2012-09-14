using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using GSMLibrary.Core;
using GSMLibrary.Commands;
using GSMLibrary.Commands.TrspSpecific;

namespace GSMLibrary.Tests.Commands.Core
{
    internal class MockCommunicator : ICommunicator
    {
        public List<string> Answers;
        public List<string> Commands;

        public MockCommunicator()
        {
            Answers = new List<string>();
            Commands = new List<string>();
        }

        public bool Activate()
        {
            return true;
        }
        
        public void ApplyPortSettings()
        {
            // dummy
        }

        public void Deactivate()
        {
            // dummy
        }

        public void FlushBuffer()
        {
            // dummy;
        }

        public bool LoadSettings()
        {
            return true;//dummy
        }

        public string ReadLine()
        {
            if (Answers.Count > 0)
            {
                string zAnswer = Answers.First();
                Answers.RemoveAt(0);
                return zAnswer;
            }
            else
                throw new System.TimeoutException();
        }

        public bool SaveSettings()
        {
            return true;//dummy
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            Commands.Add(Encoding.Default.GetString(buffer));            
        }

        public void WriteLine(string text)
        {
            Commands.Add(text);
        }
    }
        
    public class CommandRunnersCases : SyncCommandRunner
    {
        private MockCommunicator _mock;

        public CommandRunnersCases()
        {
            _mock = new MockCommunicator();
            base._communicatorInstance = _mock;
        }
        
        [Fact]
        public void CheckInCorrectWakeUp()
        {
            _mock.Answers.Add("");
            _mock.Answers.Add("");
            _mock.Answers.Add("");
            _mock.Answers.Add("");

            Assert.False(base.DeviceWakeUp());

            _mock.Answers.Add("AT");
            _mock.Answers.Add("");
            _mock.Answers.Add("asd");
            _mock.Answers.Add("");

            Assert.False(base.DeviceWakeUp());
        }

        [Fact]
        public void CheckCorrectWakeUp()
        {
            _mock.Answers.Add("AT");
            _mock.Answers.Add("");
            _mock.Answers.Add("OK");
            _mock.Answers.Add("");

            Assert.True(base.DeviceWakeUp());
        }

        [Fact]
        public void CorrectReadableCommandRun()
        {
            _mock.Answers.Add("AT+MODE?");
            _mock.Answers.Add("");
            _mock.Answers.Add("+MODE: \"GPRS\", \"AWAIT\"");
            _mock.Answers.Add("OK");

            Assert.True(base.RunSingleReadableCommand(new AppModeCommand()));            
        }

        [Fact]
        public void WrongReadableCommandRun()
        {            
            _mock.Answers.Add("");

            Assert.False(base.RunSingleReadableCommand(new GPRSConfigCommand()));

            _mock.Answers.Add("AT");
            _mock.Answers.Add("");
            _mock.Answers.Add("ERROR");
            _mock.Answers.Add("");
                        
            Assert.False(base.RunSingleReadableCommand(new GPRSConfigCommand()));
        }

        [Fact]
        public void CorrectRunnableCommandRun()
        {            
            _mock.Answers.Add("OK");

            Assert.True(base.RunSingleRunnableCommand(new RestartCommand()));

            Assert.Equal("AT+TRSPSTART", _mock.Commands.First());
        }

        [Fact]
        public void WrongRunnableCommandRun()
        {
            _mock.Answers.Add("");

            Assert.False(base.RunSingleRunnableCommand(new RestartCommand()));

            _mock.Answers.Add("AT");
            _mock.Answers.Add("");
            _mock.Answers.Add("ERROR");
            _mock.Answers.Add("");

            Assert.False(base.RunSingleRunnableCommand(new RestartCommand()));
        }

        [Fact]
        public void CorrectWritableCommandRun()
        {
            _mock.Answers.Add("OK");

            AppModeCommand zCommand = new AppModeCommand();
            zCommand.DeviceConnectionMode = ConnectionMode.CSD;
            zCommand.DeviceConnectionType = ConnectionType.ACTIVE;
            Assert.True(base.RunSingleWritableCommand(zCommand));
            Assert.Equal("AT+MODE=\"CSD\",\"ACTIVE\"", _mock.Commands.First());
        }

        [Fact]
        public void WrongWritableCommandRun()
        {
            _mock.Answers.Add("");

            Assert.False(base.RunSingleWritableCommand(new GPRSConfigCommand()));

            _mock.Answers.Add("AT");
            _mock.Answers.Add("");
            _mock.Answers.Add("ERROR");
            _mock.Answers.Add("");

            Assert.False(base.RunSingleWritableCommand(new GPRSConfigCommand()));
        }

        [Fact]
        public void CorrectRunCommandCases()
        {
            _mock.Answers.Add("OK");
            
            _mock.Answers.Add("AT+MODE?");
            _mock.Answers.Add("");
            _mock.Answers.Add("+MODE: \"GPRS\", \"AWAIT\"");
            _mock.Answers.Add("OK");

            _mock.Answers.Add("AT+MODE?");
            _mock.Answers.Add("");
            _mock.Answers.Add("+MODE: \"GPRS\", \"AWAIT\"");
            _mock.Answers.Add("OK");

            List<BaseATCommand> zList = new List<BaseATCommand>();
            AppModeCommand zAppCommand = new AppModeCommand();
            zList.Add(zAppCommand);
            AppModeCommand zAppCommand2 = new AppModeCommand();            
            zList.Add(zAppCommand2);

            Dictionary<BaseATCommand, bool> zResult;
            string zErrorMessage = "";
            zResult = base.RunCommands(zList, CommandType.ctRead, out zErrorMessage);
            Assert.Equal(2, zResult.Count);
            Assert.Equal("", zErrorMessage);
            Assert.True(zResult[zAppCommand]);
            Assert.True(zResult[zAppCommand2]);

            _mock.Answers.Add("OK");
            _mock.Answers.Add("OK");                       
            _mock.Answers.Add("OK");
            
            zList.Clear();
            AppModeCommand zCommand;
            zCommand = new AppModeCommand();
            zCommand.DeviceConnectionMode = ConnectionMode.CSD;
            zCommand.DeviceConnectionType = ConnectionType.ACTIVE;
            zList.Add(zCommand);

            AppModeCommand zCommand2;
            zCommand2 = new AppModeCommand();
            zCommand2.DeviceConnectionMode = ConnectionMode.CSD;
            zCommand2.DeviceConnectionType = ConnectionType.ACTIVE;
            zList.Add(zCommand2);

            zResult = base.RunCommands(zList, CommandType.ctWrite, out zErrorMessage);

            Assert.Equal(2, zResult.Count);
            Assert.Equal("", zErrorMessage);
            Assert.True(zResult[zCommand]);
            Assert.True(zResult[zCommand2]);

            zList.Clear();
            _mock.Answers.Add("OK");
            _mock.Answers.Add("OK");
            _mock.Answers.Add("OK");

            RestartCommand zRestCommand = new RestartCommand();            
            zList.Add(zRestCommand);
            RestartCommand zRestCommand2 = new RestartCommand();            
            zList.Add(zRestCommand2);            

            zResult = base.RunCommands(zList, CommandType.ctRun, out zErrorMessage);
            Assert.Equal(2, zResult.Count);
            Assert.Equal("", zErrorMessage);
            Assert.True(zResult[zRestCommand]);
            Assert.True(zResult[zRestCommand2]);
        }

        [Fact]
        public void WrongRunCommandCases()
        {
            _mock.Answers.Add("");
            List<BaseATCommand> zList = new List<BaseATCommand>();
            zList.Add(new AppModeCommand());
            zList.Add(new AppModeCommand());

            Dictionary<BaseATCommand, bool> zResult;

            string zErrorMessage = "";
            zResult = base.RunCommands(zList, CommandType.ctRead, out zErrorMessage);
            Assert.Equal(0, zResult.Count);
        }

        [Fact]
        public void WrongRunCommandTypesCases()
        {
            _mock.Answers.Add("OK");

            _mock.Answers.Add("AT+MODE?");
            _mock.Answers.Add("");
            _mock.Answers.Add("+MODE: \"GPRS\", \"AWAIT\"");
            _mock.Answers.Add("OK");

            _mock.Answers.Add("");
            _mock.Answers.Add("OK");

            List<BaseATCommand> zList = new List<BaseATCommand>();
            AppModeCommand zAppCommand = new AppModeCommand();
            zList.Add(zAppCommand);
            RestartCommand zRestartCommand = new RestartCommand();
            zList.Add(zRestartCommand);

            Dictionary<BaseATCommand, bool> zResult;
            string zErrorMessage = "";
            zResult = base.RunCommands(zList, CommandType.ctRead, out zErrorMessage);
            Assert.Equal(2, zResult.Count);
            Assert.Equal("", zErrorMessage);
            Assert.True(zResult[zAppCommand]);
            Assert.False(zResult[zRestartCommand]);
        }
    }

    public class TrspCommandRunnersCases : SyncTrspCommandsRunner
    {
        private MockCommunicator _mock;

        public TrspCommandRunnersCases()
        {
            _mock = new MockCommunicator();
            base._communicatorInstance = _mock;
        }

        [Fact]
        public void CheckInCorrectWakeUp()
        {
            _mock.Answers.Add("");
            _mock.Answers.Add("");
            _mock.Answers.Add("");
            _mock.Answers.Add("");

            Assert.False(base.DeviceWakeUp());

            _mock.Answers.Add("AT");
            _mock.Answers.Add("");
            _mock.Answers.Add("asd");
            _mock.Answers.Add("");

            Assert.False(base.DeviceWakeUp());
        }

        [Fact]
        public void CheckCorrectWakeUp()
        {
            _mock.Answers.Add("AT");
            _mock.Answers.Add("");
            _mock.Answers.Add("OK");
            _mock.Answers.Add("");

            Assert.True(base.DeviceWakeUp());

            Assert.Equal("exit", _mock.Commands.First());
            Assert.Equal("exit", _mock.Commands[1]);
            Assert.Equal("AT", _mock.Commands[2]);
        }
    }
}
