using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dropcraft.Contracts;
using Dropcraft.Contracts.Configuration;

namespace Dropcraft.Runtime
{
    public partial class DropcraftEngine : IDropcraftEngine
    {
        public RuntimeContext RuntimeContext { get; }

        private readonly List<IPackageConfigurationParser> _packageConfigurationParsers;
        private readonly List<PackageInfo> _packageSources;

        DropcraftEngine(EngineConfiguration configuration)
        {
            RuntimeContext = configuration.RuntimeContext;
            _packageSources = new List<PackageInfo>(configuration.PackageSources);
            _packageConfigurationParsers = new List<IPackageConfigurationParser>(configuration.PackageConfigurationParsers);
        }

        public Task Start()
        {
            InitializePlatformServices();
            return StartEngine();
        }

        public void Stop()
        {
            StopEngine();
        }

        protected virtual Task StartEngine()
        {
            RaiseRuntimeEvent(new RuntimeStartEvent());

            var deferredContext = new DeferredContext();
            foreach (var package in _packageSources)
            {
                if (!String.IsNullOrWhiteSpace(package.InstallPath))
                    EntityActivator.Current.AddPackagePath(package.InstallPath);

                HandlePackage(package, deferredContext, false);
            }

            RaiseRuntimeEvent(new AllRegularPackagesLoadedEvent());

            return Task.Factory.StartNew(HandleDeferredContext, deferredContext);
        }

        protected virtual void StopEngine()
        {
            RaiseRuntimeEvent(new RuntimeStopEvent());
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

        private void HandlePackage(PackageInfo package, DeferredContext deferredContext, bool ignoreEntityActivationMode)
        {
            IParsedPackageConfiguration configuration = null;
            foreach (var configurationParser in _packageConfigurationParsers)
            {
                configuration = configurationParser.Parse(package);
                if (configuration != null)
                    break;
            }

            if (configuration == null || !configuration.IsPackageEnabled())
                return;

            if (configuration.GetPackageActivationMode() == EntityActivationMode.Immediate)
            {
                RaiseRuntimeEvent(new BeforePackageLoadedEvent {PackageInfo = package});
                LoadPackageInfo(configuration, deferredContext, ignoreEntityActivationMode);
                RaiseRuntimeEvent(new AfterPackageLoadedEvent { PackageInfo = package });
            }
            else
            {
                deferredContext.Packages.Add(package);
            }
        }

        private void LoadPackageInfo(IParsedPackageConfiguration configurationProvider, DeferredContext deferredContext, bool ignoreEntityActivationMode)
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
                    RuntimeContext.StashExtension(extension);
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