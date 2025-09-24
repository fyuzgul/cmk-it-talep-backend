using System.ComponentModel.DataAnnotations;

namespace CMKITTalep.Entities
{
    public class Inventory : BaseEntity
    {
        public string? No { get; set; }
        public string? Department { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Domain { get; set; }
        public string? ComputerName { get; set; }
        public string? ComputerModel { get; set; }
        public string? SerialNumber { get; set; }
        public string? OperatingSystem { get; set; }
        public string? LicenseTag { get; set; }
        public string? Office { get; set; }
        public string? OfficeLicense { get; set; }
        public string? Processor { get; set; }
        public string? Ram { get; set; }
        public string? Ssd { get; set; }
        public string? Hdd { get; set; }
        public string? EthernetIp { get; set; }
        public string? EthernetMac { get; set; }
        public string? WifiIp { get; set; }
        public string? WifiMac { get; set; }
        public string? Antivirus { get; set; }
        public string? InstalledPrograms { get; set; }
        public string? MonitorModel { get; set; }
        public string? MonitorSerialNumber { get; set; }
        public string? Accessories { get; set; }
    }
}
