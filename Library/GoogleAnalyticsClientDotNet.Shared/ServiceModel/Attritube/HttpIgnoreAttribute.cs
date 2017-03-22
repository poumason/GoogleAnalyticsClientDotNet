using System;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    internal class HttpIgnoreAttribute : Attribute
    {
    }
}