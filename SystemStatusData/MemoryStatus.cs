using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SystemStatusData
{
    class MemoryStatus
    {
        public MemoryStatus()
        {
            this.PhysicalMemoryUsage = PhysicalMemoryUsage;
            this.BasePriority = BasePriority;
            this.PriorityClass = PriorityClass;
            this.PagedMemorySize = PagedMemorySize;
            this.PagedSystemMemorySize = PagedSystemMemorySize;
            this.VirtualMemorySize = VirtualMemorySize;
            this.NonpagedSystemMemorySize = NonpagedSystemMemorySize;
            this.PeakPagedMemorySize = PeakPagedMemorySize;
            this.PeakVirtualMemorySize = PeakVirtualMemorySize;
            this.PeakWorkingSetMemorySize = PeakWorkingSetMemorySize;
        }

        public long PhysicalMemoryUsage { get; set; }
        public int BasePriority { get; set; }
        public string PriorityClass { get; set; }
        public long PagedMemorySize { get; set; }
        public long VirtualMemorySize { get; set; }
        public long PagedSystemMemorySize { get; set; }
        public long NonpagedSystemMemorySize { get; set; }
        public long PeakPagedMemorySize { get; set; }
        public long PeakVirtualMemorySize { get; set; }
        public long PeakWorkingSetMemorySize { get; set; }
    }
}
