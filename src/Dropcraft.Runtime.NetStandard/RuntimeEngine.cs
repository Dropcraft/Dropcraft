using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Package;
using Dropcraft.Common.Runtime;

namespace Dropcraft.Runtime
{
    public class RuntimeEngine : IRuntimeEngine
    {
        public RuntimeContext RuntimeContext { get; }

        protected IProductConfigurationProvider ConfigurationProvider { get; }

        public RuntimeEngine(RuntimeConfiguration configuration)
        {
            RuntimeContext = configuration.RuntimeContext;
            ConfigurationProvider = configuration
                                    .ProductConfigurationSource
                                    .GetProductConfigurationProvider(RuntimeContext.ProductPath);
        }

        public Task Start()
        {
            return Start(null);
        }

        public Task Start(ICollection<PackageId> packages)
        {
            RaiseRuntimeEvent(new BeforeRuntimeStartedEvent());

            OnInitializePlatformServices();
            var packagesGraph = ConfigurationProvider.GetPackages();

            IEnumerable<PackageId> packagesForLoading = packages != null
                ? packagesGraph.SliceWithDependencies(packages, false).FlattenLeastDependentFirst()
                : packagesGraph.FlattenLeastDependentFirst();

            return OnStart(packagesForLoading);
        }

        public void Stop()
        {
            OnStop();
            RaiseRuntimeEvent(new AfterRuntimeStoppedEvent());
        }

        protected virtual void OnInitializePlatformServices()
        {
            if (EntityActivator.Current == null)
            {
                EntityActivator.InitializeEntityActivator(new ReflectionEntityActivator());
            }
        }

        protected virtual Task OnStart(IEnumerable<PackageId> packages)
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

        protected virtual void OnStop()
        {
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
                    var startupHandler = EntityActivator.Current.GetPackageStartupHandler(startupHandlerInfo);
                    startupHandler.Start(RuntimeContext);
                }
            }

            var runtimeEventsHandlerInfos = configurationProvider.GetRuntimeEventHandlers();
            if (runtimeEventsHandlerInfos != null)
            {
                foreach (var eventsHandlerInfo in runtimeEventsHandlerInfos)
                {
                    var eventsHandler = EntityActivator.Current.GetRuntimeEventsHandler(eventsHandlerInfo);
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
            var extensibilityPoint = EntityActivator.Current.GetExtensibilityPointHandler(info);
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