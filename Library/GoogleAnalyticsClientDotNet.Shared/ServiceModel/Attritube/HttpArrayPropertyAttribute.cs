using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    internal class HttpArrayPropertyAttribute : Attribute
    {
        public HttpPropertyFor For { get; private set; }

        public HttpArrayPropertyAttribute()
        {
            For = HttpPropertyFor.GET;
        }

        public HttpArrayPropertyAttribute(HttpPropertyFor propertyFor)
        {
            For = propertyFor;
        }
    }
}