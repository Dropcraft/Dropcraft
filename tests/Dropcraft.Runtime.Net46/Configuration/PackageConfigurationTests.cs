using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common.Package;
using Dropcraft.Runtime.Core;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Dropcraft.Runtime.Configuration
{
    public class PackageConfigurationTests
    {
        [Fact]
        public void PackageIsEnabledForEmptyManifest()
        {
            var obj = JObject.Parse("{}");
            var manifest = new PackageConfiguration(null, obj);

            manifest.IsPackageEnabled.Should().BeTrue();
        }

        [Fact]
        public void EnabledPackageStateIsParsed()
        {
            var obj = JObject.Parse("{ \"enabled\": \"true\" }");
            var manifest = new PackageConfiguration(null, obj);

            manifest.IsPackageEnabled.Should().BeTrue();
        }

        [Fact]
        public void DisabledPackageStateIsParsed()
        {
            var obj = JObject.Parse("{ \"enabled\": \"false\" }");
            var manifest = new PackageConfiguration(null, obj);

            manifest.IsPackageEnabled.Should().BeFalse();
        }

        [Fact]
        public void ActivationModeIsImmediateFoEmptyManifest()
        {
            var obj = JObject.Parse("{}");
            var manifest = new PackageConfiguration(null, obj);

            manifest.GetPackageActivationMode().Should().Be(EntityActivationMode.Immediate);
        }

        [Fact]
        public void ImmediateActivationModeIsParsed()
        {
            var obj = JObject.Parse("{ \"activation\": \"immediate\" }");
            var manifest = new PackageConfiguration(null, obj);

            manifest.GetPackageActivationMode().Should().Be(EntityActivationMode.Immediate);
        }

        [Fact]
        public void DeferredActivationModeIsParsed()
        {
            var obj = JObject.Parse("{ \"activation\": \"deferred\" }");
            var manifest = new PackageConfiguration(null, obj);

            manifest.GetPackageActivationMode().Should().Be(EntityActivationMode.Deferred);
        }

        [Fact]
        public void EmptyStartupHandlersListReturnForEmptyManifest()
        {
            var config = JObject.Parse("{}");
            var manifest = new PackageConfiguration(null, config);
            var list = manifest.GetPackageStartupHandlers();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void EmptyEventHandlersListReturnForEmptyManifest()
        {
            var config = JObject.Parse("{}");
            var manifest = new PackageConfiguration(null, config);
            var list = manifest.GetRuntimeEventHandlers();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void EmptyDeploymentHandlersListReturnForEmptyManifest()
        {
            var config = JObject.Parse("{}");
            var manifest = new PackageConfiguration(null, config);
            var list = manifest.GetDeploymentEventHandlers();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void EmptyExtensibilityPointsListReturnForEmptyManifest()
        {
            var config = JObject.Parse("{}");
            var manifest = new PackageConfiguration(null, config);
            var list = manifest.GetExtensibilityPoints();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void EmptyExtensionsListReturnForEmptyManifest()
        {
            var config = JObject.Parse("{}");
            var manifest = new PackageConfiguration(null, config);
            var list = manifest.GetExtensions();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void ConfigWithThreeStartupHandlersParsed()
        {
            var config = JObject.Parse("{\"startupHandlers\": [\"1\",\"2\",\"3\"]}");
            var manifest = new PackageConfiguration(null, config);
            var handlers = manifest.GetPackageStartupHandlers();

            var handlersList = new List<PackageStartupHandlerInfo>(handlers);
            handlersList.Count.Should().Be(3);
            handlersList[0].ClassName.Should().Be("1");
            handlersList[1].ClassName.Should().Be("2");
            handlersList[2].ClassName.Should().Be("3");
        }

        [Fact]
        public void ConfigWithThreeEventHandlersParsed()
        {
            var config = JObject.Parse("{\"runtimeEventHandlers\": [\"1\",\"2\",\"3\"]}");
            var manifest = new PackageConfiguration(null, config);
            var handlers = manifest.GetRuntimeEventHandlers();

            var handlersList = new List<RuntimeEventsHandlerInfo>(handlers);
            handlersList.Count.Should().Be(3);
            handlersList[0].ClassName.Should().Be("1");
            handlersList[1].ClassName.Should().Be("2");
            handlersList[2].ClassName.Should().Be("3");
        }

        [Fact]
        public void ConfigWithThreeDeploymentHandlersParsed()
        {
            var config = JObject.Parse("{\"deploymentEventHandlers\": [\"1\",\"2\",\"3\"]}");
            var manifest = new PackageConfiguration(null, config);
            var handlers = manifest.GetDeploymentEventHandlers();

            var handlersList = new List<DeploymentEventsHandlerInfo>(handlers);
            handlersList.Count.Should().Be(3);
            handlersList[0].ClassName.Should().Be("1");
            handlersList[1].ClassName.Should().Be("2");
            handlersList[2].ClassName.Should().Be("3");
        }

        public class ConfigClass
        {
            public string Var { get; set; }
        }

        [Fact]
        public void ConfigWithOneExtensionParsed()
        {
            var config = JObject.Parse("{ \"extensions\": [ {\"id\" : \"id\", \"extensibilityPointId\" : \"extid\", \"class\" : \"test\", \"configuration\": {\"var\" : \"hello\"} } ] }");
            var manifest = new PackageConfiguration(null, config);
            var extensions = manifest.GetExtensions();

            var extensionList = new List<ExtensionInfo>(extensions);
            extensionList.Count.Should().Be(1);
            extensionList[0].Id.Should().Be("id");
            extensionList[0].ExtensibilityPointId.Should().Be("extid");
            extensionList[0].ClassName.Should().Be("test");

            var customConfig = extensionList[0].CustomConfiguration.Get();
            customConfig.HasChildren().Should().BeTrue();
            customConfig.GetChild("var").Value.Should().Be("hello");
        }

        [Fact]
        public void ConfigWithOneExtensibilityPointParsed()
        {
            var config = JObject.Parse("{ \"extensibilityPoints\": [ {\"id\" : \"id\", \"activation\": \"deferred\", \"class\": \"test\", \"configuration\": {\"var\" : \"hello\"} } ] }");
            var manifest = new PackageConfiguration(null, config);
            var extensions = manifest.GetExtensibilityPoints();

            var extensionList = new List<ExtensibilityPointInfo>(extensions);
            extensionList.Count.Should().Be(1);
            extensionList[0].Id.Should().Be("id");
            extensionList[0].ActivationMode.Should().Be(EntityActivationMode.Deferred);
            extensionList[0].ClassName.Should().Be("test");

            var customConfig = extensionList[0].CustomConfiguration.Get();
            customConfig.HasChildren().Should().BeTrue();
            customConfig.GetChild("var").Value.Should().Be("hello");
        }
    }
}
