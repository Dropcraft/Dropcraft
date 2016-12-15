using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using FluentAssertions;
using Xunit;

namespace Dropcraft.Runtime.Configuration
{
    public class ProductConfigurationTests
    {
        [Fact]
        public void ForEmptyFolderProductIsReportedAsNonConfigured()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var product = new ProductConfigurationProvider(path);
            product.IsProductConfigured.Should().BeFalse();

            Directory.Delete(path, true);
        }

        [Fact]
        public void ForExistingEmptyProductFileProductIsReportedAsConfigured()
        {
            using (var helper = new ProductConfigurationHelper("Product.Empty.json"))
            {
                helper.Configuration.IsProductConfigured.Should().BeTrue();
            }
        }

        [Fact]
        public void ProductWithPackagesIsParsed()
        {
            using (var helper = new ProductConfigurationHelper("Product.PackagesOnly.json"))
            {
                helper.Configuration.IsProductConfigured.Should().BeTrue();
                var packages = helper.Configuration.GetPackages().FlattenLeastDependentFirst();

                packages.Count.Should().Be(2);
                packages[0].ToString().Should().Be("ABC/1.0");
                packages[1].ToString().Should().Be("XYZ/2.0");
            }
        }

        [Fact]
        public void ProductWithAllFeaturesIsParsed()
        {
            using (var helper = new ProductConfigurationHelper("Product.json"))
            {
                helper.Configuration.IsProductConfigured.Should().BeTrue();
                var packages = helper.Configuration.GetPackages().FlattenLeastDependentFirst();

                packages.Count.Should().Be(3);
                packages[0].ToString().Should().Be("ABC/1.0");

                var config = helper.Configuration.GetPackageConfiguration(packages[0]);
                config.IsPackageEnabled.Should().BeTrue();

                packages[1].ToString().Should().Be("QQQ/3.0");

                packages[2].ToString().Should().Be("XYZ/2.0");
                config = helper.Configuration.GetPackageConfiguration(packages[2]);
                config.GetPackageActivationMode().Should().Be(EntityActivationMode.Deferred);

                var files = helper.Configuration.GetInstalledFiles(new PackageId("XYZ/2.0"), true);
                files.Count().Should().Be(2);
            }
        }
    }

    public class ProductConfigurationHelper : IDisposable
    {
        private readonly string _path;

        public ProductConfigurationProvider Configuration { get; set; }

        public ProductConfigurationHelper(string testFile)
        {
            _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Dropcraft.Runtime.Data."+testFile))
            {
                if (stream == null) return;

                string filePath;
                using (var sr = new StreamReader(stream))
                {
                    filePath = Path.Combine(_path, "project.json");
                    using (var sw = File.CreateText(filePath))
                    {
                        sw.Write(sr.ReadToEnd());
                        sw.Flush();
                    }
                }

                Configuration = new ProductConfigurationProvider(filePath);
            }
        }

        public void Dispose()
        {
            Directory.Delete(_path, true);
        }
    }
}
