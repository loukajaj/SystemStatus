using System.Net.NetworkInformation;

namespace SystemStatusData
{
    class NetworkStatusData
    {
        public NetworkStatusData()
        {
            this.InterfaceType = InterfaceType;
            this.PhysicalAddress = PhysicalAddress;
            this.OperationalStatus = OperationalStatus;
            this.BytesSent = BytesSent;
            this.BytesReceived = BytesReceived;
        }

        public string InterfaceType { get; set; }
        public string PhysicalAddress { get; set; }
        public string OperationalStatus { get; set; }
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }
    }
}