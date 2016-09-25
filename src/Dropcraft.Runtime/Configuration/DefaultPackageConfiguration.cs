using System;
using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Configuration
{
    public class DefaultPackageConfiguration : PackageConfiguration
    {
        private readonly JObject _jsonObject;
        private readonly PackageInfo _packageInfo;

        public DefaultPackageConfiguration(PackageInfo packageInfo, JObject jsonObject)
        {
            _packageInfo = packageInfo;
            _jsonObject = jsonObject;
        }

        protected override bool OnIsPackageEnabled()
        {
            JToken token;
            return !_jsonObject.TryGetValue("enabled", out token) || token.Value<bool>();
        }

        protected override EntityActivationMode OnGetPackageActivationMode()
        {
            JToken token;
            var result = EntityActivationMode.Immediate;
            if (_jsonObject.TryGetValue("activation", out token))
            {
                Enum.TryParse(token.ToString(), true, out result);
            }

            return result;
        }

        protected override IEnumerable<PackageStartupHandlerInfo> OnGetPackageStartupHandlers()
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

        protected override IEnumerable<RuntimeEventsHandlerInfo> OnGetRuntimeEventHandlers()
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

        protected override IEnumerable<ExtensionInfo> OnGetExtensions()
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
                            var cfg = x["configuration"];

                            Func<Type, object> func;
                            if (cfg != null)
                                func = t => cfg.ToObject(t);
                            else
                                func = t => null;

                            return new ExtensionInfo(_packageInfo, x.Value<string>("class"),
                                x.Value<string>("extensibilityPointId"),
                                x.Value<string>("id"), func);

                        });

                result.AddRange(extensions);
            }

            return result;
        }

        protected override IEnumerable<ExtensibilityPointInfo> OnGetExtensibilityPoints()
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
                            var cfg = x["configuration"];

                            Func<Type, object> func;
                            if (cfg != null)
                                func = t => cfg.ToObject(t);
                            else
                                func = t => null;

                            var activationMode = EntityActivationMode.Immediate;
                            var activationSetting = x.Value<string>("activation");
                            if (!String.IsNullOrWhiteSpace(activationSetting))
                                Enum.TryParse(activationSetting, true, out activationMode);

                            return new ExtensibilityPointInfo(_packageInfo, x.Value<string>("class"),
                                x.Value<string>("id"), activationMode, func);

                        });

                result.AddRange(extPoints);
            }

            return result;
        }

        protected override IEnumerable<PackageDeploymentHandlerInfo> OnGetPackageDeploymentHandlers()
        {
            throw new System.NotImplementedException();
        }
    }
}