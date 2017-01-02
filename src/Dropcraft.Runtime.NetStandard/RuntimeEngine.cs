using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Dropcraft.Common.Runtime;

namespace Dropcraft.Runtime
{
    /// <summary>
    /// Class RuntimeEngine.
    /// </summary>
    /// <seealso cref="Dropcraft.Common.Runtime.IRuntimeEngine" />
    public class RuntimeEngine : IRuntimeEngine
    {
        /// <summary>
        /// Current context
        /// </summary>
        /// <value>The runtime context.</value>
        public RuntimeContext RuntimeContext { get; }
        /// <summary>
        /// Gets the configuration provider.
        /// </summary>
        /// <value>The configuration provider.</value>
        public IProductConfigurationProvider ConfigurationProvider { get; }
        /// <summary>
        /// Gets the entity activator.
        /// </summary>
        /// <value>The entity activator.</value>
        public IEntityActivator EntityActivator { get; }

        internal RuntimeEngine(RuntimeContext runtimeContext, IProductConfigurationProvider configurationProvider, IEntityActivator entityActivator)
        {
            RuntimeContext = runtimeContext;
            ConfigurationProvider = configurationProvider;
            EntityActivator = entityActivator;
        }

        /// <summary>
        /// Starts execution
        /// </summary>
        /// <returns>Task</returns>
        public Task Start()
        {
            return Start(null);
        }

        /// <summary>
        /// Starts execution for the selected packages and their dependencies
        /// </summary>
        /// <param name="packages">Packages to load</param>
        /// <returns>Task</returns>
        public Task Start(ICollection<PackageId> packages)
        {
            RaiseRuntimeEvent(new BeforeRuntimeStartedEvent());

            var packagesGraph = ConfigurationProvider.GetPackages();

            IEnumerable<PackageId> packagesForLoading = packages != null
                ? packagesGraph.SliceWithDependencies(packages, false).FlattenLeastDependentFirst()
                : packagesGraph.FlattenLeastDependentFirst();

            return Execute(packagesForLoading);
        }

        /// <summary>
        /// Stops execution. The main purpose is to notify all components about the application shutdown.
        /// </summary>
        public void Stop()
        {
            RaiseRuntimeEvent(new AfterRuntimeStoppedEvent());
        }

        private Task Execute(IEnumerable<PackageId> packages)
        {
            var deferredContext = new DeferredContext();

            foreach (var package in packages)
            {
                var config = ConfigurationProvider.GetPackageConfiguration(package);
                HandlePackage(config, deferredContext, false);
            }

            RaiseRuntimeEvent(new AllRegularPackagesLoadedEvent());

            return Task.Factory.StartNew(HandleDeferredContext, deferredContext);
        }

        private void HandleDeferredContext(object obj)
        {
            var deferredContext = (DeferredContext)obj;

            foreach (var config in deferredContext.PackageConfigurations)
            {
                HandlePackage(config, deferredContext, true);
            }

            foreach (var extensibilityPointInfo in deferredContext.ExtensibilityPoints)
            {
                ActivateAndRegisterExtensibilityPoint(extensibilityPointInfo);
            }

            RaiseRuntimeEvent(new AllDeferredPackagesLoadedEvent());
        }

        private void HandlePackage(IPackageConfiguration config, DeferredContext deferredContext, bool ignoreEntityActivationMode)
        {
            if (!config.IsPackageEnabled)
                return;

            if (config.GetPackageActivationMode() == EntityActivationMode.Immediate)
            {
                RaiseRuntimeEvent(new BeforePackageLoadedEvent {Package = config.Id});
                LoadPackageInfo(config, deferredContext, ignoreEntityActivationMode);
                RaiseRuntimeEvent(new AfterPackageLoadedEvent { Package = config.Id });
            }
            else
            {
                deferredContext.PackageConfigurations.Add(config);
            }
        }

        private void LoadPackageInfo(IPackageConfiguration configurationProvider, DeferredContext deferredContext, bool ignoreEntityActivationMode)
        {
            var packageStartupHandlerInfos = configurationProvider.GetPackageStartupHandlers();
            if (packageStartupHandlerInfos != null)
            {
                foreach (var startupHandlerInfo in packageStartupHandlerInfos)
                {
                    var startupHandler = EntityActivator.GetPackageStartupHandler(startupHandlerInfo);
                    startupHandler.Start(RuntimeContext);
                }
            }

            var runtimeEventsHandlerInfos = configurationProvider.GetRuntimeEventHandlers();
            if (runtimeEventsHandlerInfos != null)
            {
                foreach (var eventsHandlerInfo in runtimeEventsHandlerInfos)
                {
                    var eventsHandler = EntityActivator.GetRuntimeEventsHandler(eventsHandlerInfo);
                    eventsHandler.RegisterEventHandlers(RuntimeContext);
                }
            }

            var extensions = configurationProvider.GetExtensions();
            if (extensions != null)
            {
                foreach (var extension in extensions)
                {
                    RuntimeContext.RegisterExtension(extension);
                }
            }

            var extensibilityPoints = configurationProvider.GetExtensibilityPoints();
            foreach (var extensibilityPointInfo in extensibilityPoints)
            {
                if (extensibilityPointInfo.ActivationMode == EntityActivationMode.Immediate || ignoreEntityActivationMode)
                {
                    ActivateAndRegisterExtensibilityPoint(extensibilityPointInfo);
                }
                else
                {
                    deferredContext.ExtensibilityPoints.Add(extensibilityPointInfo);
                }
            }
        }

        private void ActivateAndRegisterExtensibilityPoint(ExtensibilityPointInfo info)
        {
            var extensibilityPoint = EntityActivator.GetExtensibilityPointHandler(info);
            extensibilityPoint.Initialize(info, RuntimeContext);

            RuntimeContext.RegisterExtensibilityPoint(info.Id, extensibilityPoint);
        }

        private void RaiseRuntimeEvent(RuntimeEvent runtimeEvent)
        {
            runtimeEvent.Context = RuntimeContext;
            RuntimeContext.RaiseRuntimeEvent(runtimeEvent);
        }
    }
}