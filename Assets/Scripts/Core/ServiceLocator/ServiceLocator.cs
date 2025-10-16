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
        
        // Bat tat event khi dich vu dc dang ki / xoa
        public static event Action<Type, IService> serviceRegistered;
        public static event Action<Type> serviceUnregistered;
        
        //Dang ki dich vu voi locator
        public static void Register<T>(T service, bool initialize = true) where T : class, IService
        {
            if (service == null)
            {
                Debug.LogError($"Dich vu {typeof(T).Name} khong duoc null.");
                return;
            }

            if (_isShuttingDown)
            {
                Debug.LogWarning($"Khong the dang ki dich vu {typeof(T).Name} khi dang tat.");
                return;
            }

            var serviceType = typeof(T);
            IService toCleanup = null;
            _lock.EnterWriteLock();

            try
            {
                if (_services.TryGetValue(serviceType, out var existingService))
                {
                    Debug.LogWarning($"Dich vu {serviceType.Name} da ton tai.");
                    toCleanup = existingService;
                }

                _services[serviceType] = service;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            //Cleanup ben ngoai lock
            if (toCleanup != null && toCleanup.isInitialized)
            {
                try
                {
                    toCleanup.Cleanup();
                }
                catch (Exception ex)
                {
                    Debug.LogError(
                        $"[ServiceLocator] Error cleaning up existing service {serviceType.Name}: {ex.Message}");
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
                serviceRegistered?.Invoke(serviceType, service);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ServiceLocator] Error in ServiceRegistered subscribers for {serviceType.Name}: {ex.Message}");
            }

            Debug.Log($"[ServiceLocator] Service {serviceType.Name} registered.");
        }

        //Lay dich vu ra de su dung
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
        
        //Test get
        public static bool TryGet<T>(out T service) where T : class, IService
        {
            service = Get<T>();
            return service != null;
        }
        
        //Kiem tra xem dich vu da duoc dang ky chua
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
        
        //Xoa dich vu
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

            // Cleanup outside lock
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

            // Event outside lock
            try
            {
                serviceUnregistered?.Invoke(serviceType);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ServiceLocator] Error in ServiceUnregistered subscribers for {serviceType.Name}: {ex.Message}");
            }

            Debug.Log($"[ServiceLocator] Service {serviceType.Name} unregistered.");
            return true;
        }
        
        //Lay danh sach dich vu da dang ky
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
        
        //Xoa toan bo dich vu
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
        //Dem so dich vu da dang ky (bruh tai sao no khuyen nen co cai nay tu dem di vro)
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