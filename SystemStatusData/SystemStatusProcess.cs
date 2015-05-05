using System;

namespace SystemStatusData
{
    class SystemStatusProcess
    {
        public SystemStatusProcess()
        {
            this.MachineName = MachineName;
            this.ProcessId = ProcessId;
            this.ProcessName = ProcessName;
            this.ProcessorTime = ProcessorTime;
            this.ProcessorUserTime = ProcessorUserTime;
            this.PrivilegedProcessorTime = PrivilegedProcessorTime;
            this.WorkingSetMemory = WorkingSetMemory;
            this.WorkingSetPeakMemory = WorkingSetPeakMemory;
            this.WorkingSetPrivateMemory = WorkingSetPrivateMemory;
            this.ThreadCount = ThreadCount;
            this.HandleCount = HandleCount;
            this.Time = Time;
        }

        public string MachineName { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public float ProcessorTime { get; set; }
        public float ProcessorUserTime { get; set; }
        public float PrivilegedProcessorTime { get; set; }
        public long WorkingSetMemory { get; set; }
        public long WorkingSetPeakMemory { get; set; }
        public long WorkingSetPrivateMemory { get; set; }
        public float ThreadCount { get; set; }
        public float HandleCount { get; set; }
        public DateTime Time { get; set; }
    }
}