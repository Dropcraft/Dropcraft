using System.Collections.Generic;
using System.Linq;
using Dropcraft.Contracts.Configuration;
using Dropcraft.Runtime.Configuration;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Dropship.Runtime.Configuration
{
    public class ParsedPackageManifestTests
    {
        [Fact]
        public void For_empty_manifest_package_should_be_enabled()
        {
            var obj = JObject.Parse("{}");
            var manifest = new ParsedPackageManifest(null, obj);

            manifest.IsPackageEnabled().Should().BeTrue();
        }

        [Fact]
        public void Enabled_package_state_should_be_parsed()
        {
            var obj = JObject.Parse("{ \"enabled\": \"true\" }");
            var manifest = new ParsedPackageManifest(null, obj);

            manifest.IsPackageEnabled().Should().BeTrue();
        }

        [Fact]
        public void Disabled_package_state_should_be_parsed()
        {
            var obj = JObject.Parse("{ \"enabled\": \"false\" }");
            var manifest = new ParsedPackageManifest(null, obj);

            manifest.IsPackageEnabled().Should().BeFalse();
        }

        [Fact]
        public void For_empty_manifest_package_should_have_immediate_activation()
        {
            var obj = JObject.Parse("{}");
            var manifest = new ParsedPackageManifest(null, obj);

            manifest.GetPackageActivationMode().Should().Be(EntityActivationMode.Immediate);
        }

        [Fact]
        public void Immediate_package_activation_mode_should_be_parsed()
        {
            var obj = JObject.Parse("{ \"activation\": \"immediate\" }");
            var manifest = new ParsedPackageManifest(null, obj);

            manifest.GetPackageActivationMode().Should().Be(EntityActivationMode.Immediate);
        }

        [Fact]
        public void Deferred_package_activation_mode_should_be_parsed()
        {
            var obj = JObject.Parse("{ \"activation\": \"deferred\" }");
            var manifest = new ParsedPackageManifest(null, obj);

            manifest.GetPackageActivationMode().Should().Be(EntityActivationMode.Deferred);
        }

        [Fact]
        public void Missed_startup_handlers_will_return_empty_list()
        {
            var config = JObject.Parse("{}");
            var manifest = new ParsedPackageManifest(null, config);
            var list = manifest.GetPackageStartupHandlers();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void Missed_event_handlers_will_return_empty_list()
        {
            var config = JObject.Parse("{}");
            var manifest = new ParsedPackageManifest(null, config);
            var list = manifest.GetRuntimeEventHandlers();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void Missed_extensibility_points_will_return_empty_list()
        {
            var config = JObject.Parse("{}");
            var manifest = new ParsedPackageManifest(null, config);
            var list = manifest.GetExtensibilityPoints();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void Missed_extensions_will_return_empty_list()
        {
            var config = JObject.Parse("{}");
            var manifest = new ParsedPackageManifest(null, config);
            var list = manifest.GetExtensions();

            list.Count().Should().Be(0);
        }

        [Fact]
        public void Simple_config_to_return_three_startup_handlers()
        {
            var config = JObject.Parse("{\"startupHandlers\": [\"1\",\"2\",\"3\"]}");
            var manifest = new ParsedPackageManifest(null, config);
            var handlers = manifest.GetPackageStartupHandlers();

            var handlersList = new List<PackageStartupHandlerInfo>(handlers);
            handlersList.Count.Should().Be(3);
            handlersList[0].ClassName.Should().Be("1");
            handlersList[1].ClassName.Should().Be("2");
            handlersList[2].ClassName.Should().Be("3");
        }

        [Fact]
        public void Simple_config_to_return_three_event_handlers()
        {
            var config = JObject.Parse("{\"eventHandlers\": [\"1\",\"2\",\"3\"]}");
            var manifest = new ParsedPackageManifest(null, config);
            var handlers = manifest.GetRuntimeEventHandlers();

            var handlersList = new List<RuntimeEventsHandlerInfo>(handlers);
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
        public void Simple_config_to_return_one_extension()
        {
            var config = JObject.Parse("{ \"extensions\": [ {\"id\" : \"id\", \"extensibilityPointId\" : \"extid\", \"class\" : \"test\", \"configuration\": {\"var\" : \"hello\"} } ] }");
            var manifest = new ParsedPackageManifest(null, config);
            var extensions = manifest.GetExtensions();

            var extensionList = new List<ExtensionInfo>(extensions);
            extensionList.Count.Should().Be(1);
            extensionList[0].Id.Should().Be("id");
            extensionList[0].ExtensibilityPointId.Should().Be("extid");
            extensionList[0].ClassName.Should().Be("test");

            var extConfig = extensionList[0].GetConfiguration<ConfigClass>();
            extConfig.Var.Should().Be("hello");
        }

        [Fact]
        public void Simple_config_to_return_one_extensibility_point()
        {
            var config = JObject.Parse("{ \"extensibilityPoints\": [ {\"id\" : \"id\", \"activation\": \"deferred\", \"class\": \"test\", \"configuration\": {\"var\" : \"hello\"} } ] }");
            var manifest = new ParsedPackageManifest(null, config);
            var extensions = manifest.GetExtensibilityPoints();

            var extensionList = new List<ExtensibilityPointInfo>(extensions);
            extensionList.Count.Should().Be(1);
            extensionList[0].Id.Should().Be("id");
            extensionList[0].ActivationMode.Should().Be(EntityActivationMode.Deferred);
            extensionList[0].ClassName.Should().Be("test");

            var extConfig = extensionList[0].GetConfiguration<ConfigClass>();
            extConfig.Var.Should().Be("hello");
        }
    }
}
