﻿using GoogleAnalyticsClientDotNet.Universal.Utility;
using System;
using Windows.Foundation.Metadata;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
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
                    // XBOX 目前會取失敗
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

                operatingSystemVersion = $"{deviceFamilyVersion.Major}.{deviceFamilyVersion.Minor}.{deviceFamilyVersion.Build}";

                return operatingSystemVersion;
            }
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

        public bool IsXBOX
        {
            get { return DeviceFamily?.IndexOf("xbox", StringComparison.OrdinalIgnoreCase) >= 0; }
        }

        public ulong MemoryLimit
        {
            get
            {
                try
                {
                    var limit = MemoryManager.AppMemoryUsageLimit / 1024 / 1024;
                    return limit;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public DeviceInformationService()
        {
            deviceInformation = new EasClientDeviceInformation();
        }
    }
}