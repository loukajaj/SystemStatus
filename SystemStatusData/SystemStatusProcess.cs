using System;
using System.Diagnostics.Eventing.Reader;

namespace SystemStatusData
{
    class SystemStatusProcess
    {
        public SystemStatusProcess()
        {
        }

        public int ProcessId { get; set; }
        public string Processname { get; set; }
        public float ProcessorTime { get; set; }
        public float ProcessorUserTime { get; set; }
        public float PrivilegedProcessorTime { get; set; }
        public long WorkingSetMemory { get; set; }
        public long WorkingSetPeakMemory { get; set; }
        public long WorkingSetPrivateMemory { get; set; }
        public float ThreadCount { get; set; }
        public float HandleCount { get; set; }
    }
}