using Windows.System.Profile;

namespace GoogleAnalyticsClientDotNet.Universal.Utility
{
    public class DeviceFamilyVersion
    {
        public ulong Major { get; set; }

        public ulong Minor { get; set; }

        public ulong Build { get; set; }

        public ulong Revision { get; set; }
    }

    public class AnalyticsInfoUtility
    {
        public DeviceFamilyVersion GetDeviceFamilyVersion()
        {
            string versionString = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong versionNumber = ulong.Parse(versionString);
            ulong major = (versionNumber & 0xFFFF000000000000L) >> 48;
            ulong minor = (versionNumber & 0x0000FFFF00000000L) >> 32;
            ulong build = (versionNumber & 0x00000000FFFF0000L) >> 16;
            ulong revesion = (versionNumber & 0x000000000000FFFFL);

            return new DeviceFamilyVersion
            {
                Major = major,
                Minor = minor,
                Build = build,
                Revision = revesion,
            };
        }
    }
}