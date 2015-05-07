using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemStatusData
{
    class SystemStatusData
    {
        
        public SystemStatusData(string machinename)
        {
            this.MachineName = machinename;
            this.SystemStatusProcess = new List<SystemStatusProcess>();
            this.NetworkStatusData = new List<NetworkStatusData>();
            this.MemoryStatus = new List<MemoryStatus>();
            this.Time = Time;
        }

        public string MachineName { get; set; }
        public long TotalRam { get; set; }
        public long UsedRam { get; set; }
        public long FreeRam { get; set; }
        public int ProcessorCount { get; set; }
        public DateTime Time { get; set; }
        public List<SystemStatusProcess> SystemStatusProcess { get; set; }
        public List<NetworkStatusData> NetworkStatusData { get; set; }
        public List<MemoryStatus> MemoryStatus { get; set; } 
    }
}
