namespace Intervent.HWS.Model
{
    public class DexcomDevices : ProcessResponse
    {
        public string recordType { get; set; }

        public string recordVersion { get; set; }

        public string userId { get; set; }

        public List<Record> records { get; set; }

        public class AlertSchedule
        {
            public AlertScheduleSettings alertScheduleSettings { get; set; }

            public List<AlertSetting> alertSettings { get; set; }
        }

        public class AlertScheduleSettings
        {
            public string alertScheduleName { get; set; }

            public bool isEnabled { get; set; }

            public bool isDefaultSchedule { get; set; }

            public string startTime { get; set; }

            public string endTime { get; set; }

            public List<string> daysOfWeek { get; set; }

            public bool isActive { get; set; }

            public Override @override { get; set; }
        }

        public class AlertSetting
        {
            public string alertName { get; set; }

            public int value { get; set; }

            public string unit { get; set; }

            public bool enabled { get; set; }

            public DateTime systemTime { get; set; }

            public DateTime displayTime { get; set; }

            public int secondaryTriggerCondition { get; set; }

            public string soundTheme { get; set; }

            public string soundOutputMode { get; set; }

            public int? snooze { get; set; }
        }

        public class Override
        {
            public bool isOverrideEnabled { get; set; }

            public string mode { get; set; }

            public string endTime { get; set; }
        }

        public class Record
        {
            public DateTime lastUploadDate { get; set; }

            public string transmitterId { get; set; }

            public string transmitterGeneration { get; set; }

            public string displayDevice { get; set; }

            public string displayApp { get; set; }

            public List<AlertSchedule> alertSchedules { get; set; }
        }
    }
}
