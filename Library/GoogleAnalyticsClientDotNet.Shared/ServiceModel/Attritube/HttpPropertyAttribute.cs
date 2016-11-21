using System;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class HttpPropertyAttribute : Attribute
    {
        public string Name { get; private set; }

        public Type ConveterType { get; private set; }

        public bool IgnoreNullEmpty { get; private set; }

        public HttpPropertyFor For { get; private set; }

        public HttpPropertyAttribute(string name, HttpPropertyFor propertyFor, Type converterType, bool ignore)
        {
            Name = name;
            For = propertyFor;
            ConveterType = converterType;
            IgnoreNullEmpty = ignore;
        }

        public HttpPropertyAttribute(string name)
            : this(name, HttpPropertyFor.GET, null, true)
        {
        }

        public HttpPropertyAttribute(string name, Type converterType)
            : this(name, HttpPropertyFor.GET, converterType, true)
        {
        }

        public HttpPropertyAttribute(string name, HttpPropertyFor propertyFor)
            : this(name, propertyFor, null, true)
        {
        }

        public HttpPropertyAttribute(string name, HttpPropertyFor propertyFor, Type converterType)
            : this(name, propertyFor, converterType, true)
        {
        }

        public HttpPropertyAttribute(string name, HttpPropertyFor propertyFor, bool ignore)
            : this(name, propertyFor, null, ignore)
        {
        }
    }
}