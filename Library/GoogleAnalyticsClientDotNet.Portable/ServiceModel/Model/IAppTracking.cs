using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    interface IAppTracking
    {
        string AppName { get; set; }

        string AppId { get; set; }

        string AppVersion { get; set; }

        string AppNamespace { get; set; }
    }
}