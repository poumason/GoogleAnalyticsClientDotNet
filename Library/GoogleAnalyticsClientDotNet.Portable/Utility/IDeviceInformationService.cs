namespace GoogleAnalyticsClientDotNet.Utility
{
    public interface IDeviceInformationService
    {
        /// <summary>
        /// 裝置唯一識別
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// 作業系統
        /// </summary>
        string OperatingSystem { get; }

        /// <summary>
        /// 作業系統版本
        /// </summary>
        string OperationSystemVersion { get; }

        /// <summary>
        /// 系統製造商
        /// </summary>
        string SystemManufacturer { get; }

        /// <summary>
        /// 裝置名稱
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 裝置模組名稱
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// OS 類型
        /// </summary>
        string DeviceFamily { get; }

        /// <summary>
        /// 是否為 Mobile
        /// </summary>
        bool IsMobile { get; }

        /// <summary>
        /// 是否為桌機
        /// </summary>
        bool IsDesktop { get; }

        /// <summary>
        /// 是否為 XBOX
        /// </summary>
        bool IsXBOX { get; }

        /// <summary>
        /// 記憶體限制 (MB)
        /// </summary>
        ulong MemoryLimit { get; }
    }
}