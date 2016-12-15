using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    public static class HttpParameterPacker
    {
        public static HttpParameterPackResult CreatePackedParameterResult(object parameter)
        {
            var packResult = new HttpParameterPackResult();

            var propertyArray = parameter.GetType().GetRuntimeProperties();
            var getParameters = new StringBuilder();

            foreach (var property in propertyArray)
            {
                if (CheckIsIgnoreProperty(property))
                {
                    continue;
                }

                var normalAttribute = property.GetCustomAttribute<HttpPropertyAttribute>();
                if (normalAttribute != null)
                {
                    CreateNormalParameter(parameter, property, normalAttribute, packResult, getParameters);
                    continue;
                }

                var rawStringAttribute = property.GetCustomAttribute<HttpRawStringPropertyAttribute>();
                if (rawStringAttribute != null)
                {
                    CreateRawStringParameter(parameter, property, rawStringAttribute, packResult, getParameters);
                    continue;
                }

                var arrayAttribute = property.GetCustomAttribute<HttpArrayPropertyAttribute>();
                if (arrayAttribute != null)
                {
                    CreateArrayParameter(parameter, property, arrayAttribute, packResult, getParameters);
                }
            }

            if (getParameters.Length > 0)
            {
                getParameters.Remove(0, 1);
            }
            packResult.GetCombindedString = getParameters.ToString();

            return packResult;
        }

        private static bool CheckIsIgnoreProperty(PropertyInfo property)
        {
            var ignoreAttribute = property.GetCustomAttribute<HttpIgnoreAttribute>();
            return ignoreAttribute != null;
        }

        #region 產生一般的參數

        private static void CreateNormalParameter(object parameter, PropertyInfo property, HttpPropertyAttribute attribute, HttpParameterPackResult packResult, StringBuilder getParameters)
        {
            string propertyName;
            string propertyValue;
            GetPropertyNameAndValue(parameter, property, attribute, out propertyName, out propertyValue);

            if ((string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue)) && attribute.IgnoreNullEmpty)
            {
                return;
            }

            switch (attribute.For)
            {
                case HttpPropertyFor.GET:
                    var urlEncodedValue = System.Net.WebUtility.UrlEncode(propertyValue);
                    getParameters.Append($"&{propertyName}={urlEncodedValue}");
                    break;
                case HttpPropertyFor.POST:
                    packResult.PostParameterMap.Add(propertyName, propertyValue);
                    break;
                case HttpPropertyFor.HEADER:
                    packResult.HeaderParameterMap.Add(propertyName, propertyValue);
                    break;
            }
        }

        private static void GetPropertyNameAndValue(object parameter, PropertyInfo property, HttpPropertyAttribute attribute, out string propertyName, out string propertyValue)
        {
            propertyName = null;
            propertyValue = null;

            if (attribute == null)
            {
                propertyName = property.Name;
                propertyValue = string.Format("{0}", property.GetValue(parameter, null));
                return;
            }

            propertyName = attribute.Name;
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (attribute.ConveterType == null)
            {
                propertyValue = string.Format("{0}", property.GetValue(parameter, null));
                return;
            }

            var converter = Activator.CreateInstance(attribute.ConveterType) as IServiceParameterConveter;
            propertyValue = converter.Convert(property.GetValue(parameter, null));
        }

        #endregion

        #region 產生 Raw String 的參數

        private static void CreateRawStringParameter(object parameter, PropertyInfo property, HttpRawStringPropertyAttribute rawStringAttribute, HttpParameterPackResult packResult, StringBuilder getParameters)
        {
            var value = property.GetValue(parameter);
            if (value == null)
            {
                return;
            }

            packResult.PostRawStringList.Add(value.ToString());
        }

        #endregion        

        private static void CreateArrayParameter(object parameter, PropertyInfo property, HttpArrayPropertyAttribute attribute, HttpParameterPackResult packResult, StringBuilder getParameters)
        {
            var value = property.GetValue(parameter, null);

            if (value == null || value is Dictionary<string, string> == false)
            {
                return;
            }

            var dictionary = value as Dictionary<string, string>;

            switch (attribute.For)
            {
                case HttpPropertyFor.GET:
                    foreach (KeyValuePair<string, string> item in dictionary)
                    {
                        if (string.IsNullOrEmpty(item.Value) || string.IsNullOrEmpty(item.Key))
                        {
                            continue;
                        }

                        var urlEncodedValue = WebUtility.UrlEncode(item.Value);
                        getParameters.Append($"&{item.Key}={urlEncodedValue}");
                    }
                    break;
                case HttpPropertyFor.POST:
                    foreach (KeyValuePair<string, string> item in dictionary)
                    {
                        if (string.IsNullOrEmpty(item.Value) || string.IsNullOrEmpty(item.Key))
                        {
                            continue;
                        }

                        packResult.PostParameterMap.Add(item.Key, item.Value);
                    }
                    break;
            }
        }
    }
}