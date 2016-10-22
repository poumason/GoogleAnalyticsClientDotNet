using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ulong MemoryLimit
        {
            get
            {
                return 0;
            }
        }

        public string ModelName
        {
            get
            {
                return Environment.MachineName;
            }
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

        public string SystemManufacturer
        {
            get
            {
                return Environment.SystemDirectory;
            }
        }

        public string UniqueId
        {
            get
            {
                return string.Empty;
            }
        }
    }
}