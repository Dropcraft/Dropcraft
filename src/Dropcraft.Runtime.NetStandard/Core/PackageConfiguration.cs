using System;
using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Newtonsoft.Json.Linq;

namespace Dropcraft.Runtime.Core
{
    /// <summary>
    /// Provides JSON-based implementation of the <see cref="IPackageConfiguration"/>
    /// </summary>
    public class PackageConfiguration : IPackageConfiguration
    {
        private readonly JObject _jsonObject;

        /// <summary>
        /// Package ID
        /// </summary>
        /// <value>The identifier.</value>
        public PackageId Id { get; }

        /// <summary>
        /// Indicates whether the package is enabled in the current configuration
        /// </summary>
        /// <value><c>true</c> if this instance is package enabled; otherwise, <c>false</c>.</value>
        public bool IsPackageEnabled
        {
            get
            {
                JToken token;
                return !_jsonObject.TryGetValue("enabled", out token) || token.Value<bool>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageConfiguration"/> class.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="jsonObject">The JSON object.</param>
        public PackageConfiguration(PackageId packageId, JObject jsonObject)
        {
            Id = packageId;
            _jsonObject = jsonObject;
        }

        /// <summary>
        /// Returns the package's metadata
        /// </summary>
        /// <returns>Metadata</returns>
        public PackageMetadataInfo GetPackageMetadata()
        {
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

            return null;
        }

        /// <summary>
        /// Returns the package's activation mode to use during the runtime
        /// </summary>
        /// <returns>Activation mode</returns>
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

        /// <summary>
        /// Returns a list of the package startup event handlers
        /// </summary>
        /// <returns>Event handlers</returns>
        public IReadOnlyCollection<PackageStartupHandlerInfo> GetPackageStartupHandlers()
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

        /// <summary>
        /// Returns a list of the runtime events handlers
        /// </summary>
        /// <returns>Event handlers</returns>
        public IReadOnlyCollection<RuntimeEventsHandlerInfo> GetRuntimeEventHandlers()
        {
            var result = new List<RuntimeEventsHandlerInfo>();

            JToken token;
            if (_jsonObject.TryGetValue("runtimeEventHandlers", out token) && token.HasValues)
            {
                var array = (JArray)token;
                var handlers = array.Select(x => new RuntimeEventsHandlerInfo(Id, x.ToString()));
                result.AddRange(handlers);
            }

            return result;
        }

        /// <summary>
        /// Returns a list of the extensions implemented in the package
        /// </summary>
        /// <returns>Extension definitions</returns>
        public IReadOnlyCollection<ExtensionInfo> GetExtensions()
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

        /// <summary>
        /// Returns a list of the extensibility points exposed by the package
        /// </summary>
        /// <returns>Extensibility points definitions</returns>
        public IReadOnlyCollection<ExtensibilityPointInfo> GetExtensibilityPoints()
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

        /// <summary>
        /// Returns a list the deployment events handlers.
        /// </summary>
        /// <returns>Event handlers</returns>
        public IReadOnlyCollection<DeploymentEventsHandlerInfo> GetDeploymentEventHandlers()
        {
            var result = new List<DeploymentEventsHandlerInfo>();

            JToken token;
            if (_jsonObject.TryGetValue("deploymentEventHandlers", out token) && token.HasValues)
            {
                var array = (JArray)token;
                var handlers = array.Select(x => new DeploymentEventsHandlerInfo(Id, x.ToString()));
                result.AddRange(handlers);
            }

            return result;
        }

        /// <summary>
        /// Returns custom configuration for the package
        /// </summary>
        /// <param name="configurationTag">The configuration tag.</param>
        /// <returns>Custom configuration</returns>
        public ICustomConfiguration GetCustomConfiguration(string configurationTag)
        {
            JToken token;
            if (_jsonObject.TryGetValue(configurationTag, out token))
            {
                var jobject = token as JObject;
                return jobject == null ? null : new JsonCustomConfiguration(jobject);
            }

            return null;
        }

        /// <summary>
        /// Converts configuration into JSON
        /// </summary>
        /// <returns>JSON representation of the package configuration</returns>
        public string AsJson()
        {
            return _jsonObject.ToString();
        }
    }
}