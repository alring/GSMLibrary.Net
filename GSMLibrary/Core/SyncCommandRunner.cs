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
    
    public struct CommandResultContainer
    {
        public List<BaseATCommand> Commands { get; set; }
        public string ErrorMessage { get; set; }
        public bool CommonResult { get; set; }
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

        public CommandResultContainer RunCommands(List<BaseATCommand> aCommands, CommandType aType)
        {
            CommandResultContainer zResult = new CommandResultContainer();
            zResult.Commands = aCommands;
            zResult.CommonResult = true;            

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
                                zResult.CommonResult &= RunSingleReadableCommand((IReadableCommand)zCommand);
                                break;
                            case CommandType.ctWrite:
                                zResult.CommonResult &= RunSingleWritableCommand((IWritableCommand)zCommand);
                                break;
                            case CommandType.ctRun:
                                zResult.CommonResult &= RunSingleRunnableCommand((IRunnableComand)zCommand);
                                break;
                            default:
                                zResult.ErrorMessage = "Неизвестный тип команды";
                                zResult.CommonResult = false;
                                break;
                        }
                    }

                    _communicatorInstance.Deactivate();

                    zResult.ErrorMessage = "";
                    return zResult; ;
                }
                else
                {
                    zResult.CommonResult = false;
                    zResult.Commands.Clear();
                    zResult.ErrorMessage = "Нет ответа от устройства";
                    return zResult;
                }
            }
            catch (Exception zException)
            {
                zResult.ErrorMessage = zException.Message;
                zResult.Commands.Clear();
                zResult.CommonResult = false;
            }

            return zResult;
        }
    }
}
