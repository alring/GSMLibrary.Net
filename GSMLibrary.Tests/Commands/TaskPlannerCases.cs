using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSMLibrary.Commands.TrspSpecific;
using Xunit;

namespace GSMLibrary.Tests.Commands
{
    public class TaskPlannerCases : WritableBaseCommand, IReadableCommandCases
    {
        private TaskPlannerConfigCommand _TaskPlannerCommand;

        public TaskPlannerCases()
        {
            _TaskPlannerCommand = new TaskPlannerConfigCommand();
            _Command = _TaskPlannerCommand;
        }

        [Fact]
        public void ParseCorrect()
        {
            List<string> zList = new List<string>();

            zList.Clear();
            zList.Add("AT+TASKPLANNER?");
            zList.Add("");
            zList.Add("+TASKPLANNER: 0, 11:20, 23:12");
            zList.Add("OK");
            Assert.True(_TaskPlannerCommand.Parse(zList));

            Assert.Equal(SyncMode.CustomSync, _TaskPlannerCommand.DeviceSyncMode);

            Assert.Equal(_TaskPlannerCommand.WindowOpenTime.Hour, 11);
            Assert.Equal(_TaskPlannerCommand.WindowOpenTime.Minute, 20);

            Assert.Equal(_TaskPlannerCommand.ActivityPeriod.Hours, 23);
            Assert.Equal(_TaskPlannerCommand.ActivityPeriod.Minutes, 12);

            zList.Clear();
            zList.Add("+TASKPLANNER: 1, 11:20, 23:12");
            zList.Add("OK");
            Assert.True(_TaskPlannerCommand.Parse(zList));

            Assert.Equal(SyncMode.OperatorSync, _TaskPlannerCommand.DeviceSyncMode);
        }

        [Fact]
        public void WrongPrefix()
        {
            List<string> zList = new List<string>();

            zList.Add("+TASKLANNER: 0, 11:20, 23:12");
            zList.Add("OK");
            Assert.False(_TaskPlannerCommand.Parse(zList));
        }

        [Fact]
        public void WrongParamsCount()
        {
            List<string> zList = new List<string>();

            zList.Add("+TASKLANNER: 0, 11:20, 23:12, 34345");
            zList.Add("OK");
            Assert.False(_TaskPlannerCommand.Parse(zList));
        }

        [Fact]
        public void WrongParams()
        {
            List<string> zList = new List<string>();

            zList.Add("+TASKPLANNER: 0, 38:20, 23:12");
            zList.Add("OK");
            Assert.False(_TaskPlannerCommand.Parse(zList));

            zList.Clear();
            zList.Add("+TASKPLANNER: 0, 38:20, 243:12");
            zList.Add("OK");
            Assert.False(_TaskPlannerCommand.Parse(zList));

            zList.Clear();
            zList.Add("+TASKPLANNER: 2, 11:20, 23:12");
            zList.Add("OK");
            Assert.False(_TaskPlannerCommand.Parse(zList));
        }

        [Fact]
        public void ReadCommandTest()
        {
            Assert.Equal("AT+TASKPLANNER?", _TaskPlannerCommand.ReadParamsCommand());
        }

        [Fact]
        public override void WriteCommandTest()
        {
            _TaskPlannerCommand.DeviceSyncMode = SyncMode.CustomSync;

            DateTime zDateTime = new DateTime(2008, 5, 1, 11, 40, 00);
            TimeSpan zTimeSpan = new TimeSpan(12, 30, 00);            

            _TaskPlannerCommand.WindowOpenTime = zDateTime;
            _TaskPlannerCommand.ActivityPeriod = zTimeSpan;

            Assert.Equal("AT+TASKPLANNER=0,11:40,12:30", _TaskPlannerCommand.WriteCommand());
        }
    }
}
