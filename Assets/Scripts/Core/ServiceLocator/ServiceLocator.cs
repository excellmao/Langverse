using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Core.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private static bool _isShuttingDown = false;

        public static event Action<Type, IService> ServiceRegistered;
        public static event Action<Type> ServiceUnregistered;

        public static void Register<T>(T service, bool initialize = true) where T : class, IService
        {
            if (service == null)
            {
                Debug.LogError($"[ServiceLocator] Cannot register null service for type {typeof(T).Name}.");
                return;
            }

            if (_isShuttingDown)
            {
                Debug.LogWarning($"[ServiceLocator] Cannot register service {typeof(T).Name} while shutting down.");
                return;
            }

            var serviceType = typeof(T);
            IService toCleanup = null;

            _lock.EnterWriteLock();
            try
            {
                if (_services.TryGetValue(serviceType, out var existingService))
                {
                    Debug.LogWarning($"[ServiceLocator] Service {serviceType.Name} already exists. Cleaning up existing instance and replacing.");
                    toCleanup = existingService;
                }

                _services[serviceType] = service;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (toCleanup != null && toCleanup.isInitialized)
            {
                try
                {
                    toCleanup.Cleanup();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ServiceLocator] Error cleaning up existing service {serviceType.Name}: {ex.Message}");
                }
            }

            if (initialize && !service.isInitialized)
            {
                try
                {
                    service.Initialize();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ServiceLocator] Error initializing service {serviceType.Name}: {ex.Message}");
                }
            }

            try
            {
                ServiceRegistered?.Invoke(serviceType, service);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ServiceLocator] Error in ServiceRegistered subscribers for {serviceType.Name}: {ex.Message}");
            }

            Debug.Log($"[ServiceLocator] Service {serviceType.Name} registered.");
        }

        // Resolve service
        public static T Get<T>() where T : class, IService
        {
            _lock.EnterReadLock();
            try
            {
                var serviceType = typeof(T);
                return _services.TryGetValue(serviceType, out var service) ? service as T : null;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        // Try resolve
        public static bool TryGet<T>(out T service) where T : class, IService
        {
            service = Get<T>();
            return service != null;
        }

        // Check if registered
        public static bool isRegistered<T>() where T : class, IService
        {
            _lock.EnterReadLock();
            try
            {
                return _services.ContainsKey(typeof(T));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        // Unregister service
        public static bool Unregister<T>() where T : class, IService
        {
            var serviceType = typeof(T);
            IService toCleanup = null;

            _lock.EnterWriteLock();
            try
            {
                if (_services.TryGetValue(serviceType, out var service))
                {
                    toCleanup = service;
                    _services.Remove(serviceType);
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            if (toCleanup != null && toCleanup.isInitialized)
            {
                try
                {
                    toCleanup.Cleanup();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ServiceLocator] Error cleaning up service {serviceType.Name}: {ex.Message}");
                }
            }

            try
            {
                ServiceUnregistered?.Invoke(serviceType);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ServiceLocator] Error in ServiceUnregistered subscribers for {serviceType.Name}: {ex.Message}");
            }

            Debug.Log($"[ServiceLocator] Service {serviceType.Name} unregistered.");
            return true;
        }

        // List registered service types
        public static Type[] GetRegisteredServiceTypes()
        {
            _lock.EnterReadLock();
            try
            {
                var types = new Type[_services.Count];
                _services.Keys.CopyTo(types, 0);
                return types;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        // Clear all services
        public static void Clear()
        {
            List<IService> toCleanup;

            _lock.EnterWriteLock();
            try
            {
                _isShuttingDown = true;
                toCleanup = new List<IService>(_services.Values);
                _services.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            foreach (var service in toCleanup)
            {
                if (service == null) continue;

                if (service.isInitialized)
                {
                    try
                    {
                        service.Cleanup();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[ServiceLocator] Error cleaning up service {service.GetType().Name}: {ex.Message}");
                    }
                }
            }

            _isShuttingDown = false;
            Debug.Log("[ServiceLocator] All services cleared.");
        }

        // Count services
        public static int ServiceCount
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _services.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
    }
}