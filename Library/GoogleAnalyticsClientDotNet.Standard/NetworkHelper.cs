
#if !WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GoogleAnalyticsClientDotNet.Utility
{
    public class NetworkHelper : INetworkHelper, IDisposable
    {
        [DllImport("wininet")]
        public static extern bool InternetGetConnectedState(
            ref uint lpdwFlags,
            uint dwReserved
            );

        public bool IsNetworkAvailable
        {
            get
            {
                uint flags = 0x0;
                return InternetGetConnectedState(ref flags, 0);
            }
        }

        public NetworkHelper()
        {
        }

        public void Dispose()
        {
        }
    }
}
#endif