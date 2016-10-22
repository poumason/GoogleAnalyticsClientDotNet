using System;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class HttpIgnoreAttribute : Attribute
    {
    }
}