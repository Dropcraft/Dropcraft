using System.Threading.Tasks;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;
using Dropcraft.Common.Handler;

namespace Dropcraft.Runtime
{
    public class RuntimeEngine : IRuntimeEngine
    {
        public IRuntimeContext RuntimeContext { get; }

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

            var packages = _configurationProvider.GetPackages();
            foreach (var package in packages)
            {
                HandlePackage(package, deferredContext, false);
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

            foreach (var package in deferredContext.Packages)
            {
                HandlePackage(package, deferredContext, true);
            }

            foreach (var extensibilityPointInfo in deferredContext.ExtensibilityPoints)
            {
                ActivateAndRegisterExtensibilityPoint(extensibilityPointInfo);
            }

            RaiseRuntimeEvent(new AllDeferredPackagesLoadedEvent());
        }

        private void HandlePackage(PackageId package, DeferredContext deferredContext, bool ignoreEntityActivationMode)
        {
            var configuration = _configurationProvider.GetPackageConfiguration(package);
            if (configuration == null || !configuration.IsPackageEnabled())
                return;

            if (configuration.GetPackageActivationMode() == EntityActivationMode.Immediate)
            {
                RaiseRuntimeEvent(new BeforePackageLoadedEvent {Package = package});
                LoadPackageInfo(configuration, deferredContext, ignoreEntityActivationMode);
                RaiseRuntimeEvent(new AfterPackageLoadedEvent { Package = package });
            }
            else
            {
                deferredContext.Packages.Add(package);
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