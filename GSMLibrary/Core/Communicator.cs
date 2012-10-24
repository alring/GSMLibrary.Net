using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using NLog;

namespace GSMLibrary.Core
{
    [Serializable()]
    public sealed class SerialPortSettings
    {
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public string PortName { get; set; }
        public StopBits StopBits { get; set; }        
    }

    public interface ICommunicator
    {
        bool Activate();

        void ApplyPortSettings();

        void Deactivate();

        void FlushBuffer();

        bool LoadSettings();
        
        string ReadLine();

        bool SaveSettings();

        void Write(byte[] buffer, int offset, int count);
        void WriteLine(string text);
    }
    
    public sealed class Communicator : ICommunicator
    {
        private static readonly Communicator instance = new Communicator();
        private SerialPort _serialPort;
        private SerialPortSettings _serialSettings;
        private Logger _logger;
        
        public int BaudRate 
        {
            get
            {
                return _serialSettings.BaudRate;
            }
            set
            {
                _serialSettings.BaudRate = value;
            }
        }
        
        public int DataBits 
        {
            get
            {
                return _serialSettings.DataBits;
            }
            set
            {
                _serialSettings.DataBits = value;
            }
        }
        
        public Parity Parity 
        {
            get
            {
                return _serialSettings.Parity;
            }
            set
            {               
                _serialSettings.Parity = value;                
            }
        }

        public string PortName 
        {
            get
            {
                return _serialSettings.PortName;
            }
            set
            {
                _serialSettings.PortName = value;
            }
        }
        
        public StopBits StopBits 
        {
            get
            {
                return _serialSettings.StopBits;
            }
            set
            {
                _serialSettings.StopBits = value;                
            }
        }

        private Communicator() 
        {
            _serialPort = new SerialPort();

            _serialPort.ReadTimeout = 2000;
            
            _serialSettings = new SerialPortSettings();
            _serialSettings.BaudRate = _serialPort.BaudRate;
            _serialSettings.DataBits = _serialPort.DataBits;
            _serialSettings.Parity = _serialPort.Parity;
            _serialSettings.PortName = _serialPort.PortName;
            _serialSettings.StopBits = _serialPort.StopBits;

            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool Activate()
        {
            try
            {
                LoadSettings();
                                
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                ApplyPortSettings();

                _serialPort.Open();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public void ApplyPortSettings() 
        {
            _logger.Debug("ApplyPortSettings Start");
            try
            {
                //LoadSettings();
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                _serialPort.BaudRate = _serialSettings.BaudRate;
                _serialPort.DataBits = _serialSettings.DataBits;
                _serialPort.Parity = _serialSettings.Parity;
                _serialPort.PortName = _serialSettings.PortName;
                _serialPort.StopBits = _serialSettings.StopBits;

                _serialPort.NewLine = "\r\n";

                _logger.Debug("Try to Open Serial Port");
                _serialPort.Open();
            }
            catch (Exception zException)
            {
                _logger.WarnException("Handled exception", zException);
            }
            _logger.Debug("ApplyPortSettings OK");
        }

        public void Deactivate()
        {
            try
            {
                _logger.Debug("Deactivate Start");
                if (_serialPort.IsOpen)
                {
                    _logger.Debug("Try to Close Serial Port");
                    _serialPort.Close();
                }
                else
                    _logger.Debug("No need to Close; Serial Port Was Not opened");
            }
            catch (Exception zException)
            {
                _logger.WarnException("Handled exception", zException);
            }
        }

        public void FlushBuffer()
        {
            _serialPort.DiscardInBuffer();
        }

        public string ReadLine()
        {
            string zResult = _serialPort.ReadLine();
            _logger.Info("Read New Line : {0}", zResult);
            _logger.Debug("HEX string: {0}", zResult.ToCharArray().ToString());
            return zResult;
        }        

        public static Communicator Instance
        {
            get 
            {
                return instance; 
            }
        }

        public bool LoadSettings()
        {
            Logger zlogger = LogManager.GetCurrentClassLogger();
            zlogger.Debug("LoadSettings Start");

            XmlSerializer serializer = new XmlSerializer(typeof(SerialPortSettings));
            try
            {
                FileStream zReader = new FileStream("Config\\PortSettings.xml", FileMode.OpenOrCreate);
                Communicator.Instance._serialSettings = (SerialPortSettings)serializer.Deserialize(zReader);
                zReader.Close();

                zlogger.Info("LoadSettings OK");
                return true;
            }
            catch (Exception zException)
            {
                zlogger.WarnException("Handled exception", zException);
                return false;
            }

        }

        public bool SaveSettings() {
            Logger zlogger = LogManager.GetCurrentClassLogger();
            zlogger.Debug("SaveSettings Start");
            
            XmlSerializer serializer = new XmlSerializer(typeof(SerialPortSettings));

            try
            {
                Directory.CreateDirectory("Config");                

                TextWriter zWriter = new StreamWriter("Config\\PortSettings.xml");                
                serializer.Serialize(zWriter, Communicator.Instance._serialSettings);
                zWriter.Close();

                zlogger.Info("SaveSettings OK");
                return true;
            }
            catch (Exception zException)
            {
                zlogger.WarnException("Handled exception", zException);
                return false;
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _logger.Info("Write buffer: {0}, offset: {1}, count: {2}", BitConverter.ToString(buffer).Replace("-", " 0x"), offset.ToString(), count.ToString());
            _serialPort.Write(buffer, offset, count);
        }

        public void WriteLine(string text)
        {
            _logger.Info("Write New Line: {0}", text);
            _logger.Debug("HEX string: {0}", BitConverter.ToString(text.ToCharArray()).ToString());
            _serialPort.WriteLine(text);
        }
    }
}
