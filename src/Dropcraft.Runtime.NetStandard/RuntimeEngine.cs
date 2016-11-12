using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;

namespace Dropcraft.Runtime
{
    public class RuntimeEngine : IRuntimeEngine
    {
        public RuntimeContext RuntimeContext { get; }

        private readonly IProductConfigurationProvider _configurationProvider;

        public RuntimeEngine(RuntimeConfiguration configuration)
        {
            RuntimeContext = configuration.RuntimeContext;
            _configurationProvider = configuration
                                    .ProductConfigurationSource
                                    .GetProductConfigurationProvider(RuntimeContext);
        }

        public Task Start()
        {
            OnInitializePlatformServices();
            return OnStart();
        }

        public void Stop()
        {
            OnStop();
        }

        protected virtual Task OnStart()
        {
            RaiseRuntimeEvent(new RuntimeStartEvent());

            var deferredContext = new DeferredContext();

            var configurations = _configurationProvider.GetPackageConfigurations(DependencyOrdering.BottomToTop);
            foreach (var config in configurations)
            {
                HandlePackage(config, deferredContext, false);
            }

            RaiseRuntimeEvent(new AllRegularPackagesLoadedEvent());

            return Task.Factory.StartNew(HandleDeferredContext, deferredContext);
        }

        protected virtual void OnStop()
        {
            RaiseRuntimeEvent(new RuntimeStopEvent());
        }

        protected virtual void OnInitializePlatformServices()
        {
            if (EntityActivator.Current == null)
            {
                EntityActivator.InitializeEntityActivator(new ReflectionEntityActivator());
            }
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
                    RuntimeContext.RegisterRuntimeEventHandler(eventsHandler);
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