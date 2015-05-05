using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemStatusData
{
    class SystemStatusData
    {

        public SystemStatusData()
        {
            this.SystemStatusProcess = new List<SystemStatusProcess>();
        }

        public string InterfaceType { get; set; }
        public string PhysicalAddress { get; set; }
        public string OperationalStatus { get; set; }
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }

        public List<SystemStatusProcess> SystemStatusProcess { get; set; }
    }
}
