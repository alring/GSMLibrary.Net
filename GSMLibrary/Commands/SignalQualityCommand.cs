using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSMLibrary.Commands
{
    public class SignalQualityCommand : BaseReadableCommand
    {
        private const int MINIMAL_DBM_QUALITY = -113;
        private const int MAXIMUM_DBM_QUALITY = -51;
        private const float DBM_TOPERCENT_COEFFICENT = 100F / ((-1) * MINIMAL_DBM_QUALITY - ((-1) * MAXIMUM_DBM_QUALITY) );

        public SignalQualityCommand()
        {
            CommandPrefix = "CSQ";
        }

        private int _dbmQuality;
        public int dbmQuality
        {
            get
            {
                return _dbmQuality;
            }
            private set
            {
                _dbmQuality = value;
            }
        }

        public byte Quality
        {
            get
            {   
                return (byte) ((Math.Abs(MINIMAL_DBM_QUALITY) + _dbmQuality) * DBM_TOPERCENT_COEFFICENT);
            }
        }

        public override bool Parse(List<string> aAnswer)
        {   
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', ',' });
                if (zSplit.Count() == 3)
                {
                    String _qualitystr = TrimValue(zSplit[1]);
                    String _biterrorstr = TrimValue(zSplit[2]);

                    byte _qualityVal;
                    byte _bitErrorVal;

                    if (byte.TryParse(_qualitystr, out _qualityVal) && (byte.TryParse(_biterrorstr, out _bitErrorVal)))
                    {   
                        if ((_bitErrorVal > 7) && (_bitErrorVal != 99))
                            return false;

                        if (_qualityVal > 31)
                            _qualityVal = 31;

                        dbmQuality = MINIMAL_DBM_QUALITY + _qualityVal * 2;
                        return true;
                    }
                    else
                        return false;                    
                }
                else
                {
                    _logger.Debug("InCorrect Params Count: {0}", zSplit.Count());
                    return false;
                }
            }
            else
                return false;
        }

        public override String ReadParamsCommand()
        {
            return String.Concat("AT+", CommandPrefix);
        }
    }
}
