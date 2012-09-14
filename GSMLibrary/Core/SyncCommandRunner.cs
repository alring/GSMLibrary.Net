using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using GSMLibrary.Commands;

namespace GSMLibrary.Core
{   
    public enum CommandType
    {
        ctRead,
        ctWrite,
        ctRun
    }

    public class SyncTrspCommandsRunner : SyncCommandRunner
    {        
        protected override bool DeviceWakeUp()
        {
            byte[] zExitCommand = Encoding.Default.GetBytes("exit");

            _communicatorInstance.Write(zExitCommand, 0, zExitCommand.Length);
            Thread.Sleep(500);
            _communicatorInstance.Write(zExitCommand, 0, zExitCommand.Length);
            Thread.Sleep(500);

            return base.DeviceWakeUp();
        }
    }

    public class SyncCommandRunner
    {
        protected ICommunicator _communicatorInstance;

        public SyncCommandRunner()
        {
            _communicatorInstance = (ICommunicator) Communicator.Instance;
        }
        
        public SyncCommandRunner(ICommunicator aInstance)
        {
            _communicatorInstance = aInstance;
        }

        protected virtual bool DeviceWakeUp()
        {            
            _communicatorInstance.WriteLine("AT");
            //_communicatorInstance.ReadTimeout = 2000;
            try
            {
                string zAnswer = "";
                while (!BaseATCommand.PositiveAnswer(zAnswer))
                {
                    zAnswer = _communicatorInstance.ReadLine();
                }

                return true;
            }
            catch (Exception)
            {                
                return false;
            }
        }

        protected bool RunSingleReadableCommand(IReadableCommand aCommand)
        {            
            try
            {
                //_communicatorInstance.ReadTimeout = 2000;
                _communicatorInstance.FlushBuffer();

                _communicatorInstance.WriteLine(aCommand.ReadParamsCommand());

                string zAnswerLine = "";
                List<string> zAnswerList = new List<string>();
                
                while (!BaseATCommand.CheckCommandAnswer(zAnswerLine))
                {
                    zAnswerLine = _communicatorInstance.ReadLine();
                    zAnswerList.Add(zAnswerLine);
                }
                
                return aCommand.Parse(zAnswerList);                
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected bool RunSingleWritableCommand(IWritableCommand aCommand)
        {
            try
            {
                //_communicatorInstance.ReadTimeout = 2000;
                _communicatorInstance.FlushBuffer();

                _communicatorInstance.WriteLine(aCommand.WriteCommand());

                string zAnswerLine = "";
                while (!BaseATCommand.CheckCommandAnswer(zAnswerLine))
                {
                    zAnswerLine = _communicatorInstance.ReadLine();                    
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected bool RunSingleRunnableCommand(IRunnableComand aCommand)
        {
            try
            {
                //_communicatorInstance.ReadTimeout = 2000;
                _communicatorInstance.FlushBuffer();

                _communicatorInstance.WriteLine(aCommand.RunCommand());

                string zAnswerLine = "";
                while (!BaseATCommand.CheckCommandAnswer(zAnswerLine))
                {
                    zAnswerLine = _communicatorInstance.ReadLine();                    
                }

                return BaseATCommand.PositiveAnswer(zAnswerLine);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Dictionary<BaseATCommand, bool> RunCommands(List<BaseATCommand> aCommands, CommandType aType, out string aErrorMessage)
        {            
            aErrorMessage = "";
            Dictionary<BaseATCommand, bool> zResult = new Dictionary<BaseATCommand, bool>();

            try
            {
                _communicatorInstance.Activate();

                if (DeviceWakeUp())
                {
                    foreach (BaseATCommand zCommand in aCommands)                    
                    {                        
                        switch (aType)
                        {
                            case CommandType.ctRead:
                                if (zCommand is IReadableCommand)
                                    zResult[zCommand] = RunSingleReadableCommand((IReadableCommand)zCommand);
                                else
                                    zResult[zCommand] = false;
                                break;
                            case CommandType.ctWrite:
                                if (zCommand is IWritableCommand)
                                    zResult[zCommand] = RunSingleWritableCommand((IWritableCommand)zCommand);
                                else
                                    zResult[zCommand] = false;
                                break;
                            case CommandType.ctRun:
                                if (zCommand is IRunnableComand)
                                    zResult[zCommand] = RunSingleRunnableCommand((IRunnableComand)zCommand);
                                else
                                    zResult[zCommand] = false;
                                break;
                            default:
                                aErrorMessage = "Неизвестный тип команды";
                                break;
                        }
                    }

                    _communicatorInstance.Deactivate();                    
                    return zResult; ;
                }
                else
                {                    
                    aErrorMessage = "Нет ответа от устройства";
                    return zResult;
                }
            }
            catch (Exception zException)
            {
                aErrorMessage = zException.Message;                
            }

            return zResult;
        }
    }
}
