using System;
using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    public class PackageConfiguration : IPackageConfiguration
    {
        private readonly JObject _jsonObject;
        private readonly PackageInfo _packageInfo;

        public PackageConfiguration(PackageInfo packageInfo, JObject jsonObject)
        {
            _packageInfo = packageInfo;
            _jsonObject = jsonObject;
        }

        public bool IsPackageEnabled()
        {
            JToken token;
            return !_jsonObject.TryGetValue("enabled", out token) || token.Value<bool>();
        }

        public EntityActivationMode GetPackageActivationMode()
        {
            JToken token;
            var result = EntityActivationMode.Immediate;
            if (_jsonObject.TryGetValue("activation", out token))
            {
                Enum.TryParse(token.ToString(), true, out result);
            }

            return result;
        }

        public IEnumerable<PackageStartupHandlerInfo> GetPackageStartupHandlers()
        {
            var result = new List<PackageStartupHandlerInfo>();

            JToken token;
            if (_jsonObject.TryGetValue("startupHandlers", out token) && token.HasValues)
            {
                var array = (JArray)token;
                var handlers = array.Select(x => new PackageStartupHandlerInfo(_packageInfo, x.ToString()));
                result.AddRange(handlers);
            }

            return result;
        }

        public IEnumerable<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers()
        {
            var result = new List<RuntimeEventsHandlerInfo>();

            JToken token;
            if (_jsonObject.TryGetValue("eventHandlers", out token) && token.HasValues)
            {
                var array = (JArray)token;
                var handlers = array.Select(x => new RuntimeEventsHandlerInfo(_packageInfo, x.ToString()));
                result.AddRange(handlers);
            }

            return result;
        }

        public IEnumerable<ExtensionInfo> GetExtensions()
        {
            var result = new List<ExtensionInfo>();

            JToken token;
            if (_jsonObject.TryGetValue("extensions", out token) && token.HasValues)
            {
                var extensionsArray = (JArray)token;
                var extensions =
                    extensionsArray.Select(
                        x =>
                        {
                            var jsonConfig = x["configuration"];
                            ICustomConfiguration config = jsonConfig == null ? null : new JsonCustomConfiguration(jsonConfig);

                            return new ExtensionInfo(_packageInfo, x.Value<string>("class"),
                                x.Value<string>("extensibilityPointId"),
                                x.Value<string>("id"), config);

                        });

                result.AddRange(extensions);
            }

            return result;
        }

        public IEnumerable<ExtensibilityPointInfo> GetExtensibilityPoints()
        {
            var result = new List<ExtensibilityPointInfo>();

            JToken token;
            if (_jsonObject.TryGetValue("extensibilityPoints", out token) && token.HasValues)
            {
                var extArray = (JArray)token;
                var extPoints =
                    extArray.Select(
                        x =>
                        {
                            var jsonConfig = x["configuration"];
                            ICustomConfiguration config = jsonConfig == null ? null : new JsonCustomConfiguration(jsonConfig);

                            var activationMode = EntityActivationMode.Immediate;
                            var activationSetting = x.Value<string>("activation");
                            if (!String.IsNullOrWhiteSpace(activationSetting))
                                Enum.TryParse(activationSetting, true, out activationMode);

                            return new ExtensibilityPointInfo(_packageInfo, x.Value<string>("class"),
                                x.Value<string>("id"), activationMode, config);

                        });

                result.AddRange(extPoints);
            }

            return result;
        }

        public IEnumerable<DeploymentEventsHandlerInfo> GetPackageDeploymentHandlers()
        {
            var result = new List<DeploymentEventsHandlerInfo>();

            JToken token;
            if (_jsonObject.TryGetValue("deploymentHandlers", out token) && token.HasValues)
            {
                var array = (JArray)token;
                var handlers = array.Select(x => new DeploymentEventsHandlerInfo(_packageInfo, x.ToString()));
                result.AddRange(handlers);
            }

            return result;
        }
    }
}