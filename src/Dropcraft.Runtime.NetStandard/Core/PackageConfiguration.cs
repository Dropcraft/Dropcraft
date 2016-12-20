using System;
using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Core
{
    public class PackageConfiguration : IPackageConfiguration
    {
        private readonly JObject _jsonObject;
        private readonly Func<PackageMetadataInfo> _metadataFunc;

        public PackageId Id { get; }

        public bool IsPackageEnabled
        {
            get
            {
                JToken token;
                return !_jsonObject.TryGetValue("enabled", out token) || token.Value<bool>();
            }
        }

        public PackageConfiguration(PackageId packageId, JObject jsonObject, Func<PackageMetadataInfo> metadataFunc = null)
        {
            Id = packageId;
            _jsonObject = jsonObject;
            _metadataFunc = metadataFunc;
        }

        public PackageMetadataInfo GetPackageMetadata()
        {
            //TODO: read package metadata from NuGet info

            JToken token;

            if (_jsonObject.TryGetValue("metadata", out token))
            {
                return new PackageMetadataInfo(token.Value<string>("title"),
                    token.Value<string>("authors"),
                    token.Value<string>("description"),
                    token.Value<string>("projectUrl"),
                    token.Value<string>("iconUrl"),
                    token.Value<string>("licenseUrl"),
                    token.Value<string>("copyright"));
            }

            return _metadataFunc?.Invoke();
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
                var handlers = array.Select(x => new PackageStartupHandlerInfo(Id, x.ToString()));
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
                var handlers = array.Select(x => new RuntimeEventsHandlerInfo(Id, x.ToString()));
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
                            var jsonConfig = (JObject)x["configuration"];
                            ICustomConfiguration config = jsonConfig == null ? null : new JsonCustomConfiguration(jsonConfig);

                            return new ExtensionInfo(Id, x.Value<string>("class"),
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
                            var jsonConfig = (JObject)x["configuration"];
                            ICustomConfiguration config = jsonConfig == null ? null : new JsonCustomConfiguration(jsonConfig);

                            var activationMode = EntityActivationMode.Immediate;
                            var activationSetting = x.Value<string>("activation");
                            if (!string.IsNullOrWhiteSpace(activationSetting))
                                Enum.TryParse(activationSetting, true, out activationMode);

                            return new ExtensibilityPointInfo(Id, x.Value<string>("class"),
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
                var handlers = array.Select(x => new DeploymentEventsHandlerInfo(Id, x.ToString()));
                result.AddRange(handlers);
            }

            return result;
        }

        public ICustomConfiguration GetCustomConfiguration()
        {
            throw new NotImplementedException();
        }

        public string AsJson()
        {
            return _jsonObject.ToString();
        }
    }
}