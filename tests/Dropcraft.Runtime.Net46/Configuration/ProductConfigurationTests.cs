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
                var packages = helper.Configuration.GetPackageConfigurations(DependencyOrdering.BottomToTop).ToList();

                packages.Count.Should().Be(2);
                packages[0].Id.ToString().Should().Be("ABC/1.0");
                packages[1].Id.ToString().Should().Be("XYZ/2.0");
            }
        }

        [Fact]
        public void ProductWithAllFeaturesIsParsed()
        {
            using (var helper = new ProductConfigurationHelper("Product.json"))
            {
                helper.Configuration.IsProductConfigured.Should().BeTrue();
                var packages = helper.Configuration.GetPackageConfigurations(DependencyOrdering.BottomToTop).ToList();

                packages.Count.Should().Be(3);
                packages[0].Id.ToString().Should().Be("ABC/1.0");
                packages[0].IsPackageEnabled.Should().BeTrue();

                packages[1].Id.ToString().Should().Be("XYZ/2.0");
                packages[1].GetPackageActivationMode().Should().Be(EntityActivationMode.Deferred);

                packages[2].Id.ToString().Should().Be("QQQ/3.0");

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
            string filePath;
            _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Dropcraft.Runtime.Data."+testFile))
            {
                using (var sr = new StreamReader(stream))
                {
                    filePath = Path.Combine(_path, "project.json");
                    using (var sw = File.CreateText(filePath))
                    {
                        sw.Write(sr.ReadToEnd());
                        sw.Flush();
                    }
                }
            }

            Configuration = new ProductConfigurationProvider(filePath);
        }

        public void Dispose()
        {
            Directory.Delete(_path, true);
        }
    }
}
