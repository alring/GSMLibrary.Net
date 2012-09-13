using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMLibrary.Commands.TrspSpecific
{
    [Serializable]
    public enum SyncMode
    {
        // Сводка:
        //     Синхронизация вручную
        CustomSync = 0,
        //
        // Сводка:
        //     Синхронизация с оператором
        OperatorSync = 1,
    }

    public class TaskPlannerConfigCommand : BaseReadableCommand, IWritableCommand
    {        
        private SyncMode _DevSyncMode;        

        public TaskPlannerConfigCommand()
        {
            CommandPrefix = "TASKPLANNER";
        }
            
        public SyncMode DeviceSyncMode
        {
            get
            {
                return _DevSyncMode;
            }
            set
            {
                _DevSyncMode = value;
            }
        }

        public DateTime WindowOpenTime  { get; set; }
        public TimeSpan ActivityPeriod { get; set; }

        public override bool Parse(List<string> aAnswer)
        {
            if (base.Parse(aAnswer))
            {
                string[] zSplit = aAnswer[0].Split(new Char[] { ':', ',', ',', ',' });
                if (zSplit.Count() == 6)
                {
                    try
                    {
                        string zSyncMode = TrimValue(zSplit[1]);                        

                        WindowOpenTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(zSplit[2]), int.Parse(zSplit[3]), 0);
                        ActivityPeriod = new TimeSpan(int.Parse(zSplit[4]), int.Parse(zSplit[5]), 00);                        

                        switch (int.Parse(zSyncMode))
                        {
                            case 0: DeviceSyncMode = SyncMode.CustomSync;
                                break;
                            case 1: DeviceSyncMode = SyncMode.OperatorSync;
                                break;
                            default:
                                return false;                         
                        }

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

        public string WriteCommand()
        {
            return String.Concat("AT+", CommandPrefix, "=", DeviceSyncMode.GetHashCode(), ",", 
                WindowOpenTime.Hour, ":", WindowOpenTime.Minute, ",", ActivityPeriod.Hours, ":", ActivityPeriod.Minutes);
        }
    }
}
