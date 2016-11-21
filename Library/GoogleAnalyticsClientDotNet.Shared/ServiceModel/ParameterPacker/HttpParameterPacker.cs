using System;
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
    }
}