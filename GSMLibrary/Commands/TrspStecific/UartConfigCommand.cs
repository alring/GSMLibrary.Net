using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands.TrspSpecific
{    
    public class Uart1ConfigCommand : Uart2ConfigCommand
    {
        public Uart1ConfigCommand()
        {
            CommandPrefix = "UARTCONFIG";
        }

        public bool RTSSignalEnabled { get; set; }
        public bool CTSSignalEnabled { get; set; }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {   
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',' });                
                if (zSplit.Count()  == 7)
                {                    
                    try
                    {
                        RTSSignalEnabled = FlowControlCommand.EncodeValue(ushort.Parse(TrimValue(zSplit[4])));
                        CTSSignalEnabled = FlowControlCommand.EncodeValue(ushort.Parse(TrimValue(zSplit[5])));
                        UARTReadTimerInterval = UInt16.Parse(TrimValue(zSplit[6]));

                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }

        public override string WriteCommand()
        {
            ushort zBits;
            ushort zParity;

            zBits = CharacterFramingCommand.StopBitsDecode(UARTDataBits, UARTParity, UARTStopBits);
            zParity = CharacterFramingCommand.ParityDecode(UARTParity);
            
            return String.Format("AT+{0}={1},{2},{3},{4},{5},{6}", CommandPrefix, UARTBaudRate, zBits, zParity, FlowControlCommand.DecodeValue(RTSSignalEnabled), FlowControlCommand.DecodeValue(CTSSignalEnabled), UARTReadTimerInterval);            
        }
    }

    public class Uart2ConfigCommand : BaseReadableCommand, IWritableCommand
    {
        public UInt32 UARTBaudRate { get; set; }
        public UInt16 UARTDataBits { get; set; }
        public Parity UARTParity { get; set; }
        public StopBits UARTStopBits { get; set; }        
        public UInt16 UARTReadTimerInterval { get; set; }
        
        public Uart2ConfigCommand()
        {
            CommandPrefix = "UART2CONFIG";
        }        

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',' });                
                if (zSplit.Count() >= 5)
                {                    
                    try
                    {
                        ushort zBits = ushort.Parse(TrimValue(zSplit[2]));
                        ushort zParity = ushort.Parse(TrimValue(zSplit[3]));

                        ushort zUartDataBits = 0;
                        StopBits zUartStopBits = StopBits.One;                        

                        if (!CharacterFramingCommand.StopBitsEncode(zBits, out zUartDataBits, out zUartStopBits))
                            return false;                        
                        
                        UARTBaudRate = UInt32.Parse(TrimValue(zSplit[1]));
                        UARTDataBits = zUartDataBits;
                        UARTParity = CharacterFramingCommand.ParityEncode(zParity);
                        UARTStopBits = zUartStopBits;
                        UARTReadTimerInterval = UInt16.Parse(TrimValue(zSplit[4]));

                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }

        public virtual string WriteCommand()
        {
            ushort zBits;
            ushort zParity;

            zBits = CharacterFramingCommand.StopBitsDecode(UARTDataBits, UARTParity, UARTStopBits);
            zParity = CharacterFramingCommand.ParityDecode(UARTParity);

            return String.Format("AT+{0}={1},{2},{3},{4}", CommandPrefix, UARTBaudRate, zBits, zParity, UARTReadTimerInterval);            
        }
    }
}
