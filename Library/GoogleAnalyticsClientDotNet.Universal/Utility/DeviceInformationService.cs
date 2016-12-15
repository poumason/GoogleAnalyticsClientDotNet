using GoogleAnalyticsClientDotNet.Universal.Utility;
using System;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace GoogleAnalyticsClientDotNet.Utility
{
    public class DeviceInformationService : IDeviceInformationService
    {
        private readonly EasClientDeviceInformation deviceInformation = null;

        private string deviceUniqeId;
        public string UniqueId
        {
            get
            {
                if (deviceUniqeId != null)
                {
                    return deviceUniqeId;
                }

                try
                {
                    if (ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification"))
                    {
                        var packageSpecificToken = HardwareIdentification.GetPackageSpecificToken(null);
                        var hardwareId = packageSpecificToken.Id;

                        var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
                        var hashedHardwareId = hasher.HashData(hardwareId);

                        deviceUniqeId = CryptographicBuffer.EncodeToHexString(hashedHardwareId);
                        return deviceUniqeId;
                    }
                }
                catch (Exception)
                {
                    // XBOX exception
                }

                // support IoT Device
                var networkProfiles = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles();
                var adapter = networkProfiles[0].NetworkAdapter;
                string networkAdapterId = adapter.NetworkAdapterId.ToString();
                deviceUniqeId = networkAdapterId.Replace("-", string.Empty);

                return deviceUniqeId;
            }
        }

        public string OperatingSystem
        {
            get { return deviceInformation.OperatingSystem; }
        }

        private static string operatingSystemVersion;
        public string OperationSystemVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(operatingSystemVersion))
                {
                    return operatingSystemVersion;
                }

                var analyticsInfoUtility = new AnalyticsInfoUtility();
                var deviceFamilyVersion = analyticsInfoUtility.GetDeviceFamilyVersion();

                operatingSystemVersion = $"{deviceFamilyVersion.Major}.{deviceFamilyVersion.Minor}.{deviceFamilyVersion.Build}.{deviceFamilyVersion.Revision}";

                return operatingSystemVersion;
            }
        }

        public string SystemArchitecture
        {
            get { return Package.Current.Id.Architecture.ToString(); }
        }

        public string SystemManufacturer
        {
            get { return deviceInformation.SystemManufacturer; }
        }

        public string Name
        {
            get { return deviceInformation.FriendlyName; }
        }

        public string ModelName
        {
            get { return deviceInformation.SystemProductName; }
        }

        public string DeviceFamily
        {
            get { return AnalyticsInfo.VersionInfo.DeviceFamily; }
        }

        public bool IsMobile
        {
            get { return DeviceFamily?.IndexOf("mobile", StringComparison.OrdinalIgnoreCase) >= 0; }
        }

        public bool IsDesktop
        {
            get { return DeviceFamily?.IndexOf("desktop", StringComparison.OrdinalIgnoreCase) >= 0; }
        }
        
        public bool IsTouchEnabled
        {
            get
            {
                var touch = new Windows.Devices.Input.TouchCapabilities();
                return touch.TouchPresent > 0;
            }
        }

        public DeviceInformationService()
        {
            deviceInformation = new EasClientDeviceInformation();
        }
    }
}