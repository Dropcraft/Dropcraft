using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using FluentAssertions;
using Xunit;

namespace Dropcraft.Runtime.Configuration
{
    public class PackageGraphTests
    {
        private readonly PackageId _packageA = new PackageId("A/1.0.0");
        private readonly PackageId _packageB = new PackageId("B/1.0.0");
        private readonly PackageId _packageC = new PackageId("C/1.0.0");
        private readonly PackageId _packageD = new PackageId("D/1.0.0");
        private readonly PackageId _packageE = new PackageId("E/1.0.0");
        private readonly PackageId _packageF = new PackageId("F/1.0.0");
        private readonly PackageId _packageG = new PackageId("G/1.0.0");
        private readonly PackageId _packageH = new PackageId("H/1.0.0");
        private readonly PackageId _packageI = new PackageId("I/1.0.0");
        private readonly PackageId _packageJ = new PackageId("J/1.0.0");

        [Fact]
        public void OneRootPackageTest()
        {
            var builder = new PackageGraphBuilder();
            builder.Append(_packageA, new PackageId[] { });
            var graph = builder.Build();

            graph.Count.Should().Be(1);

            var list = graph.FlattenLeastDependentFirst();
            list.Count.Should().Be(1);

            list = graph.FlattenMostDependentFirst();
            list.Count.Should().Be(1);
        }

        [Fact]
        public void TwoRootPackagesTest()
        {
            var builder = new PackageGraphBuilder();
            builder.Append(_packageA, new PackageId[] { });
            builder.Append(_packageB, new PackageId[] { });
            var graph = builder.Build();

            graph.Count.Should().Be(2);
            graph.Packages.Count(x => x.Package.Id == "A").Should().Be(1);
            graph.Packages.Count(x => x.Package.Id == "B").Should().Be(1);

            var list = graph.FlattenLeastDependentFirst();
            list.Count.Should().Be(2);

            list = graph.FlattenMostDependentFirst();
            list.Count.Should().Be(2);
        }

        [Fact]
        public void TwoRootPackagesAndSharedDependencyTest()
        {
            var builder = new PackageGraphBuilder();

            builder.Append(_packageA, new[] { _packageC });
            builder.Append(_packageB, new[] { _packageC });
            var graph = builder.Build();

            graph.Count.Should().Be(3);
            graph.Packages.Count.Should().Be(2);

            var packageA = graph.Packages.First(x => x.Package.Id == "A");
            packageA.Dependencies.First().Package.Id.Should().Be("C");

            var packageB = graph.Packages.First(x => x.Package.Id == "B");
            packageB.Dependencies.First().Package.Id.Should().Be("C");

            var list = graph.FlattenLeastDependentFirst();
            list[0].Id.Should().Be("C");

            list = graph.FlattenMostDependentFirst();
            list[2].Id.Should().Be("C");
        }

        [Fact]
        public void PackageHierarhyTest()
        {
            var builder = new PackageGraphBuilder();
            builder.Append(_packageA, new[] { _packageB, _packageC });
            builder.Append(_packageB, new[] { _packageD, _packageE });
            builder.Append(_packageC, new[] { _packageF });
            builder.Append(_packageD, new PackageId[]{ });
            builder.Append(_packageE, new[] { _packageF });
            builder.Append(_packageF, new[] { _packageG });
            builder.Append(_packageG, new PackageId[] { });
            builder.Append(_packageH, new PackageId[] { });
            var graph = builder.Build();

            graph.Count.Should().Be(8);
            graph.Packages.Count.Should().Be(2);

            var packageA1 = graph.Packages.First(x=>x.Package.Id == "A");
            packageA1.Dependencies.Count.Should().Be(2);

            packageA1.Dependencies.First(x => x.Package.Id == "B").Dependencies.Count.Should().Be(2);
            packageA1.Dependencies.First(x => x.Package.Id == "C").Dependencies.Count.Should().Be(1);

            var list = graph.FlattenMostDependentFirst();
            list[0].Id.Should().BeOneOf("A", "H");
            list[1].Id.Should().BeOneOf("A", "H");
            list[2].Id.Should().BeOneOf("B", "C");
            list[3].Id.Should().BeOneOf("B", "C");
            list[4].Id.Should().Be("E");
            list[5].Id.Should().Be("F");

            list = graph.FlattenLeastDependentFirst();
            list[0].Id.Should().BeOneOf("D", "G");
            list[1].Id.Should().BeOneOf("D", "G");
            list[2].Id.Should().Be("F");
            list[3].Id.Should().Be("E");
        }

        private IPackageGraph BuildGraphForSlicing()
        {
            var builder = new PackageGraphBuilder();
            builder.Append(_packageA, new[] { _packageC, _packageD });
            builder.Append(_packageB, new[] { _packageE, _packageJ });
            builder.Append(_packageC, new[] { _packageI });
            builder.Append(_packageD, new[] { _packageF });
            builder.Append(_packageE, new[] { _packageF, _packageG });
            builder.Append(_packageF, new[] { _packageJ, _packageH });
            builder.Append(_packageG, new[] { _packageH });
            builder.Append(_packageJ, new PackageId[] { });
            builder.Append(_packageI, new PackageId[] { });

            return builder.Build();
        }

        [Fact]
        public void SliceAllTest()
        {
            var graph = BuildGraphForSlicing();
            graph.Count.Should().Be(10);
            graph.Packages.Count.Should().Be(2);

            var slice = graph.SliceWithDependencies(new[] {_packageA, _packageB}, true);
            slice.Count.Should().Be(10);
            slice.Packages.Count.Should().Be(2);
        }

        [Fact]
        public void SliceAPackageWithoutSharedPackagesTest()
        {
            var graph = BuildGraphForSlicing();

            var slice = graph.SliceWithDependencies(new[] { _packageA }, true);
            slice.Count.Should().Be(4);
            slice.Packages.Count.Should().Be(1);
        }

        [Fact]
        public void SliceAPackageWithSharedPackagesTest()
        {
            var graph = BuildGraphForSlicing();

            var slice = graph.SliceWithDependencies(new[] { _packageA }, false);
            slice.Count.Should().Be(7);
            slice.Packages.Count.Should().Be(1);
        }

        [Fact]
        public void SliceBPackageWithoutSharedPackagesTest()
        {
            var graph = BuildGraphForSlicing();

            var slice = graph.SliceWithDependencies(new[] { _packageB }, true);
            slice.Count.Should().Be(3);
            slice.Packages.Count.Should().Be(1);
        }

        [Fact]
        public void SliceBPackageWithSharedPackagesTest()
        {
            var graph = BuildGraphForSlicing();

            var slice = graph.SliceWithDependencies(new[] { _packageB }, false);
            slice.Count.Should().Be(6);
            slice.Packages.Count.Should().Be(1);
        }

        [Fact]
        public void SliceFPackageWithoutSharedPackagesTest()
        {
            var graph = BuildGraphForSlicing();

            var slice = graph.SliceWithDependencies(new[] { _packageF }, true);
            slice.Count.Should().Be(1);
            slice.Packages.Count.Should().Be(1);
        }

        [Fact]
        public void SliceFPackageWithSharedPackagesTest()
        {
            var graph = BuildGraphForSlicing();

            var slice = graph.SliceWithDependencies(new[] { _packageF }, false);
            slice.Count.Should().Be(3);
            slice.Packages.Count.Should().Be(1);
        }
    }
}