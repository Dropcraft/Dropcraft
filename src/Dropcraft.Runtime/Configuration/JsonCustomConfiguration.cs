using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common.Configuration;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    public class JsonCustomConfiguration : ICustomConfiguration
    {
        private readonly ICustomConfigObject _value;

        public JsonCustomConfiguration(JToken jsonObject)
        {
            _value = ParseToken(jsonObject, new CustomConfiguration());
        }

        private ICustomConfigObject ParseToken(JToken jsonToken, CustomConfiguration customConfiguration)
        {
            return customConfiguration; //TODO
        }

        public ICustomConfigObject Get()
        {
            return _value;
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
    }

    internal class CustomConfigurationValue : CustomConfiguration, ICustomConfigValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}