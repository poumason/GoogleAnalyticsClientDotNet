namespace GoogleAnalyticsClientDotNet.Utility
{
    public interface IDeviceInformationService
    {
        /// <summary>
        /// Device unique Id.
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// Operation system
        /// </summary>
        string OperatingSystem { get; }

        /// <summary>
        /// Operation system version.
        /// </summary>
        string OperationSystemVersion { get; }

        /// <summary>
        /// SystemManu facturer
        /// </summary>
        string SystemManufacturer { get; }

        /// <summary>
        /// Device name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Device model (product) name
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// Device family
        /// </summary>
        string DeviceFamily { get; }

        /// <summary>
        /// Is mobile device
        /// </summary>
        bool IsMobile { get; }

        /// <summary>
        /// Is desktop device
        /// </summary>
        bool IsDesktop { get; }

        /// <summary>
        /// System architecture, such as: x64, x86, arm.
        /// </summary>
        string SystemArchitecture { get; }

        /// <summary>
        /// Is supported Touch.
        /// </summary>
        bool IsTouchEnabled { get; }
    }
}