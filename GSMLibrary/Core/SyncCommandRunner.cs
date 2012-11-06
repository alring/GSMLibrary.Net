using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using GSMLibrary.Commands;
using NLog;

namespace GSMLibrary.Core
{   
    public enum CommandType
    {
        ctRead,
        ctWrite,
        ctRun
    }

    public delegate void ObservePickUp();

    public class SyncTrspCommandsRunner : SyncCommandRunner
    {
        public ObservePickUp ObservePickUpDelegate;
        
        protected override bool DeviceWakeUp()
        {
            _logger.Debug("Try to special Device wake up");

            byte[] zExitCommand = Encoding.Default.GetBytes("exit");

            _communicatorInstance.Write(zExitCommand, 0, zExitCommand.Length);
            Thread.Sleep(500);
            _communicatorInstance.Write(zExitCommand, 0, zExitCommand.Length);
            Thread.Sleep(500);

            return base.DeviceWakeUp();
        }
        
        protected bool TryDataBits(List<int> aDataBits, List<System.IO.Ports.Parity> aParities, List<System.IO.Ports.StopBits> aStopBits)
        {
            foreach (int dataBits in aDataBits)
            {
                Communicator.Instance.DataBits = dataBits;
                if (TryParity(aParities, aStopBits))
                    return true;
            }
            return false;
        }

        protected bool TryParity(List<System.IO.Ports.Parity> aParities, List<System.IO.Ports.StopBits> aStopBits)
        {
            foreach (System.IO.Ports.Parity parity in aParities)
            {
                Communicator.Instance.Parity = parity;
                if (TryStopBits(aStopBits))
                    return true;
            }

            return false;
        }

        protected bool TryStopBits(List<System.IO.Ports.StopBits> aStopBits)
        {
            foreach (System.IO.Ports.StopBits stopBits in aStopBits)
            {
                Communicator.Instance.StopBits = stopBits;
                Communicator.Instance.ApplyPortSettings();

                // указываем на следующую попытку разбудить устройство
                ObservePickUpDelegate();

                try
                {
                    bool zResult = DeviceWakeUp();
                    Communicator.Instance.Deactivate();
                    if (zResult)
                        return true;
                }
                catch (Exception zException)
                {
                    _logger.WarnException("Failed Device wake up", zException);
                    return false;
                }
                    
            }

            return false;
        }

        public bool PickUpSettings(List<int> aBaudrate, List<int> aDataBits, List<System.IO.Ports.Parity> aParities, List<System.IO.Ports.StopBits> aStopBits)
        {
            foreach (int baudRate in aBaudrate)
            {
                Communicator.Instance.BaudRate = baudRate;
                if (TryDataBits(aDataBits, aParities, aStopBits))
                    return true;
            }

            return false;
        }
    }

    public class SyncCommandRunner
    {
        protected ICommunicator _communicatorInstance;
        protected Logger _logger;

        public SyncCommandRunner()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _logger.Debug("Try to Get Communicator");
            _communicatorInstance = (ICommunicator) Communicator.Instance;
        }
        
        public SyncCommandRunner(ICommunicator aInstance)
        {
            _communicatorInstance = aInstance;
        }

        protected virtual bool DeviceWakeUp()
        {
            _logger.Debug("Try to Device wake up");
            try
            {
                _communicatorInstance.WriteLine("AT");

                string zAnswer = "";
                while (!BaseATCommand.PositiveAnswer(zAnswer))
                {
                    zAnswer = _communicatorInstance.ReadLine();
                }
                
                _logger.Debug("Device wake up OK");
                return true;
            }
            catch (Exception zException)
            {
                _logger.WarnException("Failed Device wake up", zException);
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

                _logger.Debug("Correct Answer received. Try to parse it");
                return aCommand.Parse(zAnswerList);                
            }
            catch (Exception zException)
            {
                _logger.WarnException("Failed Single command run", zException);
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
                bool zResult = false;
                while (!BaseATCommand.CheckCommandAnswer(zAnswerLine))
                {
                    zAnswerLine = _communicatorInstance.ReadLine();
                    zResult = zResult || BaseATCommand.PositiveAnswer(zAnswerLine);
                }

                _logger.Debug("Correct Answer received OK");
                return zResult;
            }
            catch (Exception zException)
            {
                _logger.WarnException("Failed Single command run", zException);
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

                _logger.Debug("Correct Answer received OK");
                return BaseATCommand.PositiveAnswer(zAnswerLine);
            }
            catch (Exception zException)
            {
                _logger.WarnException("Failed Single command run", zException);
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
                                _logger.Debug("readable command");
                                if (zCommand is IReadableCommand)
                                    zResult[zCommand] = RunSingleReadableCommand((IReadableCommand)zCommand);
                                else
                                    zResult[zCommand] = false;
                                break;
                            case CommandType.ctWrite:
                                _logger.Debug("writable command");
                                if (zCommand is IWritableCommand)
                                    zResult[zCommand] = RunSingleWritableCommand((IWritableCommand)zCommand);
                                else
                                    zResult[zCommand] = false;
                                break;
                            case CommandType.ctRun:
                                _logger.Debug("runnable command");
                                if (zCommand is IRunnableComand)
                                    zResult[zCommand] = RunSingleRunnableCommand((IRunnableComand)zCommand);
                                else
                                    zResult[zCommand] = false;
                                break;
                            default:
                                _logger.Debug("Unknown command type");
                                aErrorMessage = "Неизвестный тип команды";
                                break;
                        }
                    }

                    _communicatorInstance.Deactivate();                    
                    return zResult; ;
                }
                else
                {
                    _logger.Debug("No answer");
                    aErrorMessage = "Нет ответа от устройства";
                    return zResult;
                }
            }
            catch (Exception zException)
            {
                _logger.WarnException("Handled exception", zException);
                aErrorMessage = zException.Message;                
            }

            return zResult;
        }
    }
}
