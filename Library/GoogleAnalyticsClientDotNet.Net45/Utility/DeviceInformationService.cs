using System;
using System.Linq;
using System.Management;
using System.Windows.Input;

namespace GoogleAnalyticsClientDotNet.Utility
{
    public class DeviceInformationService : IDeviceInformationService
    {
        public string DeviceFamily
        {
            get
            {
                return string.Empty;
            }
        }

        public bool IsDesktop
        {
            get
            {
                return true;
            }
        }

        public bool IsMobile
        {
            get
            {
                return false;
            }
        }

        public bool IsXBOX
        {
            get
            {
                return false;
            }
        }
        
        public string ModelName
        {
            get; private set;
        }

        public string Name
        {
            get
            {
                return Environment.UserName;
            }
        }

        public string OperatingSystem
        {
            get
            {
                return Environment.OSVersion.Platform.ToString();
            }
        }

        public string OperationSystemVersion
        {
            get
            {
                return Environment.OSVersion.VersionString;
            }
        }

        public string OperationSystemVersionBuild
        {
            get { return string.Empty; }
        }

        public bool IsTouchEnabled
        {
            get
            {
                return Tablet.TabletDevices.Cast<TabletDevice>().Any(dev => dev.Type == TabletDeviceType.Touch);
            }
        }

        public string SystemArchitecture
        {
            get { return Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"); }
        }

        public string SystemManufacturer
        {
            get; private set;
        }

        private static string uniqueId;
        public string UniqueId
        {
            get
            {
                if (string.IsNullOrEmpty(uniqueId) == false)
                {
                    return uniqueId;
                }
                
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (uniqueId == "")
                    {
                        //Get only the first CPU's ID
                        uniqueId = mo.Properties["processorID"].Value.ToString();
                        break;
                    }
                }

                return uniqueId;
            }
        }

        public DeviceInformationService()
        {
            SelectQuery query = new SelectQuery(@"Select * from Win32_ComputerSystem");

            //initialize the searcher with the query it is supposed to execute
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                //execute the query
                foreach (System.Management.ManagementObject process in searcher.Get())
                {
                    //print system info
                    process.Get();
                    SystemManufacturer = $"{process["Manufacturer"]}";
                    ModelName = $"{process["Model"]}";
                }
            }
        }
    }
}