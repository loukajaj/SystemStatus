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
            this.NetworkStatusData = new List<NetworkStatusData>();
        }

        public List<SystemStatusProcess> SystemStatusProcess { get; set; }
        public List<NetworkStatusData> NetworkStatusData { get; set; }
    }
}
