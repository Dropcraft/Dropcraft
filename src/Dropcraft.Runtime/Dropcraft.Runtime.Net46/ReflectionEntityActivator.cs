using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Dropcraft.Common;
using Dropcraft.Common.Configuration;

namespace Dropcraft.Runtime
{
    public class ReflectionEntityActivator : EntityActivator
    {
        readonly ReaderWriterLockSlim _listLock = new ReaderWriterLockSlim();
        private readonly List<string> _probePaths = new List<string>();

        public ReflectionEntityActivator()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        private System.Reflection.Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return OnAssemblyResolve(sender, args);
        }

        protected virtual Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                _listLock.EnterReadLock();

                foreach (var path in _probePaths)
                {
                    var assemblyPath = Path.Combine(path, new AssemblyName(args.Name).Name + ".dll");
                    if (!File.Exists(assemblyPath)) continue;

                    return Assembly.LoadFrom(assemblyPath);
                }
            }
            finally
            {
                _listLock.ExitReadLock();
            }

            return null;
        }

        public static T Instantiate<T>(string className)
        {
            var type = Type.GetType(className);
            if (type == null)
            {
                throw new TypeLoadException($"Type not found: {className}");
            }

            return (T)Activator.CreateInstance(type);
        }

        protected override IHandleExtensibilityPoint OnGetExtensibilityPointHandler(ExtensibilityPointInfo info)
        {
            return Instantiate<IHandleExtensibilityPoint>(info.ClassName);
        }

        protected override T OnGetExtension<T>(ExtensionInfo info)
        {
            return Instantiate<T>(info.ClassName);
        }

        protected override IHandlePackageStartup OnGetPackageStartupHandler(PackageStartupHandlerInfo handlerInfo)
        {
            return Instantiate<IHandlePackageStartup>(handlerInfo.ClassName);
        }

        protected override IHandleRuntimeEvents OnGetRuntimeEventsHandler(RuntimeEventsHandlerInfo handlerInfo)
        {
            return Instantiate<IHandleRuntimeEvents>(handlerInfo.ClassName);
        }

        protected override void OnAddPackagePath(string path)
        {
            try
            {
                _listLock.EnterWriteLock();

                if (!_probePaths.Contains(path))
                    _probePaths.Add(path);
            }
            finally
            {
                _listLock.ExitWriteLock();
            }
        }

        protected override void OnRemovePackagePath(string path)
        {
            try
            {
                _listLock.EnterWriteLock();
                _probePaths.Remove(path);
            }
            finally
            {
                _listLock.ExitWriteLock();
            }
        }
    }
}