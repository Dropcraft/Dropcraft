using Dropcraft.Runtime.Core;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Dropcraft.Runtime.Configuration
{
    public class JsonCustomConfigurationTests
    {
        [Fact]
        public void EmptyJsonParsedAndReturnsEmptyConfig()
        {
            var obj = JObject.Parse("{}");
            var config = new JsonCustomConfiguration(obj);

            var customConfig = config.Get();
            customConfig.HasChildren().Should().BeFalse();
        }

        [Fact]
        public void JsonWithPropertiesOnlyIsParsed()
        {
            var obj = JObject.Parse("{\"x1\":\"1\", \"x2\":\"2\", \"x3\":\"3\"}");
            var config = new JsonCustomConfiguration(obj);

            var customConfig = config.Get();
            customConfig.HasChildren().Should().BeTrue();

            customConfig.GetChild("x1").Key.Should().Be("x1");
            customConfig.GetChild("x1").Value.Should().Be("1");

            customConfig.GetChild("x3").Key.Should().Be("x3");
            customConfig.GetChild("x3").Value.Should().Be("3");
        }

        [Fact]
        public void JsonWithObjectsIsParsed()
        {
            var obj = JObject.Parse("{\"x1\" : { \"x2\" : { \"x3\" : \"3\", \"x4\" : \"4\" } } }");
            var config = new JsonCustomConfiguration(obj);

            var customConfig = config.Get();
            customConfig.HasChildren().Should().BeTrue();

            var x1 = customConfig.GetChild("x1");
            x1.Key.Should().Be("x1");
            x1.Value.Should().BeNullOrEmpty();

            var x2 = x1.GetChild("x2");
            x2.Key.Should().Be("x2");
            x2.Value.Should().BeNullOrEmpty();

            x2.GetChild("x3").Key.Should().Be("x3");
            x2.GetChild("x3").Value.Should().Be("3");

            x2.GetChild("x4").Key.Should().Be("x4");
            x2.GetChild("x4").Value.Should().Be("4");
        }

        [Fact]
        public void JsonWithArrayIsParsed()
        {
            var obj = JObject.Parse("{\"x1\" : { \"x2\" : [ \"3\", \"4\", \"5\" ] } }");
            var config = new JsonCustomConfiguration(obj);

            var customConfig = config.Get();
            customConfig.HasChildren().Should().BeTrue();

            var x1 = customConfig.GetChild("x1");
            x1.Key.Should().Be("x1");
            x1.Value.Should().BeNullOrEmpty();

            var x2 = x1.GetChild("x2");
            x2.Key.Should().Be("x2");
            x2.Value.Should().BeNullOrEmpty();

            var i = 3;
            foreach (var child in x2.GetChildren())
            {
                child.Key.Should().BeNullOrEmpty();
                child.Value.Should().Be(i.ToString());
                i++;
            }
        }

        [Fact]
        public void JsonWithAllFeaturesIsParsed()
        {
            var obj = JObject.Parse("{\"p1\":\"1\", \"x1\":{\"x2\":{\"x3\":\"3\"}, \"x4\":[\"5\", \"6\"]} }");
            var config = new JsonCustomConfiguration(obj);

            var customConfig = config.Get();
            customConfig.HasChildren().Should().BeTrue();

            var p1 = customConfig.GetChild("p1");
            p1.Key.Should().Be("p1");
            p1.Value.Should().Be("1");

            var x1 = customConfig.GetChild("x1");
            x1.Key.Should().Be("x1");
            x1.Value.Should().BeNullOrEmpty();

            var x2 = x1.GetChild("x2");
            x2.Key.Should().Be("x2");
            x2.Value.Should().BeNullOrEmpty();
            x2.GetChild("x3").Key.Should().Be("x3");
            x2.GetChild("x3").Value.Should().Be("3");

            var x4 = x1.GetChild("x4");
            x4.Key.Should().Be("x4");

            var i = 5;
            foreach (var child in x4.GetChildren())
            {
                child.Key.Should().BeNullOrEmpty();
                child.Value.Should().Be(i.ToString());
                i++;
            }

        }
    }
}
