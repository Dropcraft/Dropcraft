using System;
using System.Collections.Generic;
using System.Linq;
using Dropcraft.Common;
using Dropcraft.Common.Package;

namespace Dropcraft.Runtime.Configuration
{
    public class PackageGraph : IPackageGraph
    {
        private readonly List<PackageGraphNode> _packages;

        public IReadOnlyCollection<IPackageGraphNode> Packages => _packages.AsReadOnly();

        public int Count { get; }

        public PackageGraph(List<PackageGraphNode> packages, int count)
        {
            _packages = packages;
            Count = count;
        }

        public static IPackageGraph Empty()
        {
            return new PackageGraph(new List<PackageGraphNode>(), 0);
        }

        public IList<PackageId> FlattenMostDependentFirst()
        {
            var flatList = FlattenLeastDependentFirst();
            return new List<PackageId>(flatList.Reverse());
        }

        public IList<PackageId> FlattenLeastDependentFirst()
        {
            var flatList = new List<PackageId>();

            foreach (var package in _packages)
            {
                FlattenNode(package, flatList);
            }

            return flatList;
        }

        public IPackageGraph SliceWithDependencies(ICollection<PackageId> targetPackages, bool excludeNonExclusiveDependencies)
        {
            var collectedPackages = new List<IPackageGraphNode>();
            var marks = new Dictionary<PackageId, Mark>();

            foreach (var package in _packages)
            {
                MarkPackages(package, targetPackages, collectedPackages, marks, Mark.DoNotInclude,
                    excludeNonExclusiveDependencies);
            }

            var graphBuilder = new PackageGraphBuilder();
            foreach (var collectedPackage in collectedPackages)
            {
                var mark = marks[collectedPackage.Package];
                if (mark == Mark.Include || mark == Mark.Tentative)
                {
                    graphBuilder.Append(collectedPackage.Package,
                        collectedPackage.Dependencies.Where(p =>
                        {
                            Mark packageMark;
                            return marks.TryGetValue(p.Package, out packageMark) &&
                                   (packageMark == Mark.Include || packageMark == Mark.Tentative);
                        }).Select(x => x.Package));
                }
            }

            return graphBuilder.Build();
        }

        public IPackageGraph SliceWithDependents(ICollection<PackageId> packages)
        {
            var nodes = GetNodes(packages);

            var collectedNodes = new List<IPackageGraphNode>();
            foreach (var node in nodes)
            {
                CollectNode(node, collectedNodes);
            }

            var graphBuilder = new PackageGraphBuilder();
            foreach (var collectedNode in collectedNodes)
            {
                graphBuilder.Append(collectedNode.Package,
                    collectedNode.Dependencies.Where(x => collectedNodes.Contains(x)).Select(d => d.Package));
            }
            return graphBuilder.Build();
        }

        private void CollectNode(IPackageGraphNode node, List<IPackageGraphNode> collectedNodes)
        {
            if (!collectedNodes.Contains(node))
                collectedNodes.Add(node);

            foreach (var dependent in node.Dependents)
            {
                CollectNode(dependent, collectedNodes);
            }
        }

        public IList<IPackageGraphNode> GetNodes(ICollection<PackageId> packages)
        {
            var nodes = new List<IPackageGraphNode>();

            foreach (var package in _packages)
            {
                Accumulate(package,
                    p => !packages.Any() || packages.Contains(p.Package),
                    (p, list) =>
                    {
                        if (!list.Contains(p))
                            list.Add(p);
                    },
                    nodes);
            }

            return nodes.ToList();
        }

        private void Accumulate(IPackageGraphNode node, Func<IPackageGraphNode, bool> condition,
            Action<IPackageGraphNode, List<IPackageGraphNode>> action, List<IPackageGraphNode> accu)
        {
            if (condition(node))
                action(node, accu);

            foreach (var nodeDependency in node.Dependencies)
            {
                Accumulate(nodeDependency, condition, action, accu);
            }
        }

        private void MarkPackages(IPackageGraphNode package, ICollection<PackageId> targetPackages,
            ICollection<IPackageGraphNode> collectedPackages, IDictionary<PackageId, Mark> marks, Mark currentMark,
            bool excludeNonExclusiveDependencies)
        {
            if (targetPackages.Contains(package.Package))
            {
                marks[package.Package] = Mark.Include;
                currentMark = Mark.Tentative;
            }
            else if (excludeNonExclusiveDependencies)
            {
                Mark mark;

                if (currentMark == Mark.DoNotInclude)
                {
                    marks[package.Package] = Mark.DoNotInclude;
                }
                else if (marks.TryGetValue(package.Package, out mark))
                {
                    if (mark == Mark.DoNotInclude)
                    {
                        currentMark = Mark.DoNotInclude;
                    }
                }
            }

            if (currentMark == Mark.Tentative && !collectedPackages.Contains(package))
            {
                marks[package.Package] = Mark.Tentative;
                collectedPackages.Add(package);
            }

            foreach (var dependency in package.Dependencies)
            {
                MarkPackages(dependency, targetPackages, collectedPackages, marks, currentMark, excludeNonExclusiveDependencies);
            }
        }

        private void FlattenNode(IPackageGraphNode package, ICollection<PackageId> flatList)
        {
            if (flatList.Contains(package.Package))
                return;

            foreach (var dependency in package.Dependencies)
            {
                FlattenNode(dependency, flatList);
            }

            flatList.Add(package.Package);
        }
    }

    enum Mark
    {
        Include,
        DoNotInclude,
        Tentative
    }

}