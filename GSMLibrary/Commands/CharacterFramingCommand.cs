using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace GSMLibrary.Commands
{
    public class CharacterFramingCommand :BaseReadableCommand, IWritableCommand
    {
        public ushort DataBits { get; set; }
        public Parity ParityValue { get; set; }
        public StopBits StopBitsValue { get; set; }
        
        public CharacterFramingCommand()
        {
            CommandPrefix = "ICF";
        }

        static public ushort StopBitsDecode(ushort aDataBits, Parity aParity, StopBits aStopBits)
        {            
            ushort zReturn;

            switch (aDataBits)
            {
                case 8:
                    switch (aStopBits)
	                {
                        case StopBits.One:
                            if (aParity != Parity.None)
                                zReturn = 2;
                            else
                                zReturn = 3;
                            break;
                        case StopBits.Two:
                            zReturn = 1;
                            break;
		                default:
                            throw new ArgumentOutOfRangeException();
	                }
                    break;
                case 7:
                    switch (aStopBits)
                    {
                        case StopBits.One:
                            if (aParity != Parity.None)
                                zReturn = 5;
                            else
                                zReturn = 6;
                            break;
                        case StopBits.Two:
                            zReturn = 4;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return zReturn;
        }

        static public ushort ParityDecode(Parity aParity)
        {
            ushort zReturn;            

            switch (aParity)
            {
                case Parity.Even:
                    zReturn = 1;
                    break;
                case Parity.Mark:
                    zReturn = 2;
                    break;
                case Parity.None:
                    zReturn = 4;
                    break;
                case Parity.Odd:
                    zReturn = 0;
                    break;
                case Parity.Space:
                    zReturn = 3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();                    
            }

            return zReturn;
        }

        static public bool StopBitsEncode(ushort aBits, out ushort aDataBits, out StopBits aStopBits)
        {
            bool zReturn = true;
            aDataBits = 8;
            aStopBits = StopBits.One;

            switch (aBits)
            {
                case 1:
                    aDataBits = 8;
                    aStopBits = StopBits.Two;
                    break;
                case 2:
                case 3:
                    aDataBits = 8;
                    aStopBits = StopBits.One;
                    break;                    
                case 4:
                    aDataBits = 7;
                    aStopBits = StopBits.Two;
                    break;
                case 5:
                case 6:
                    aDataBits = 7;
                    aStopBits = StopBits.One;
                    break;                    
                default:
                    zReturn = false;
                    break;
            }

            return zReturn;
        }

        static public Parity ParityEncode(ushort aDecParity)
        {
            Parity zResult;

            switch (aDecParity)
            {
                case 0:
                    zResult = Parity.Odd;
                    break;
                case 1:
                    zResult = Parity.Even;
                    break;
                case 2:
                    zResult = Parity.Mark;
                    break;
                case 3:
                    zResult = Parity.Space;
                    break;
                case 4:
                    zResult = Parity.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return zResult;
        }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',' });
                if (zSplit.Count() == 3)
                {
                    try
                    {
                        ushort zBits = ushort.Parse(TrimValue(zSplit[1]));
                        ushort zParity = ushort.Parse(TrimValue(zSplit[2]));

                        ushort zUartDataBits;
                        StopBits zUartStopBits;

                        if (CharacterFramingCommand.StopBitsEncode(zBits, out zUartDataBits, out zUartStopBits))
                        {
                            DataBits = zUartDataBits;
                            ParityValue = CharacterFramingCommand.ParityEncode(zParity);
                            StopBitsValue = zUartStopBits;

                            return true;
                        }
                        else
                            return false;
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

        public string WriteCommand()
        {            
            return String.Format("AT+{0}={1},{2}", CommandPrefix, StopBitsDecode(DataBits, ParityValue, StopBitsValue), ParityDecode(ParityValue)); ;
        }
    }
}
