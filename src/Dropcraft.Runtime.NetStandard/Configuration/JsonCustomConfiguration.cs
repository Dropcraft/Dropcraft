using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common.Package;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    public class JsonCustomConfiguration : ICustomConfiguration
    {
        private readonly CustomConfigurationValue _value;

        public JsonCustomConfiguration(JObject jsonObject)
        {
            _value = new CustomConfigurationValue();
            Parse(jsonObject, _value);
        }

        public ICustomConfigObject Get()
        {
            return _value;
        }

        private static void Parse(JToken jsonToken, CustomConfigurationValue customConfiguration)
        {
            if (jsonToken is JObject)
            {
                ParseObject((JObject) jsonToken, customConfiguration);
            }
            else if (jsonToken is JArray)
            {
                ParseArray((JArray) jsonToken, customConfiguration);
            }
            else
            {
                ParseToken(jsonToken, customConfiguration);
            }
        }

        private static void ParseObject(JObject json, CustomConfigurationValue customConfiguration)
        {
            foreach (var child in json.Children())
            {
                var property = (JProperty) child;
                var configValue = new CustomConfigurationValue {Key = property.Name};
                customConfiguration.Values.Add(configValue);

                Parse(property.Value, configValue);
            }
        }

        private static void ParseArray(JArray json, CustomConfigurationValue customConfiguration)
        {
            foreach (var element in json)
            {
                var configValue = new CustomConfigurationValue();
                customConfiguration.Values.Add(configValue);

                Parse(element, configValue);
            }
        }

        private static void ParseToken(JToken json, CustomConfigurationValue customConfiguration)
        {
            customConfiguration.Value = json.Value<string>();
        }
    }

    internal class CustomConfiguration : ICustomConfigObject
    {
        public List<ICustomConfigValue> Values { get; } = new List<ICustomConfigValue>();

        public IEnumerable<ICustomConfigValue> GetChildren()
        {
            return Values;
        }

        public ICustomConfigValue GetChild(string key)
        {
            return Values.FirstOrDefault(x => x.Key == key);
        }

        public bool HasChildren()
        {
            return Values.Any();
        }
    }

    internal class CustomConfigurationValue : CustomConfiguration, ICustomConfigValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}