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
        public static event Action<Type, IService> serviceRegitered;
        public static event Action<Type> serviceUnregitered;
        
        //Dang ki dich vu voi locator
        public static void Register<T>(T service, bool initialize = true) where T : class, IService
        {
            if (_isShuttingDown)
            {
                Debug.LogWarning($"Khong the dang ki dich vu {typeof(T).Name} khi dang tat.");
                return;
            }
            _lock.EnterWriteLock();
            try
            {
                var serviceType = typeof(T);
                
                if (_services.ContainsKey(serviceType))
                {
                    Debug.LogWarning($"Dich vu {serviceType.Name} da ton tai.");
                    var existingService = _services[serviceType];
                    if (existingService.isInitialized)
                    {
                        existingService.Cleanup();
                    }
                }

                _services[serviceType] = service;
                if (initialize && !service.isInitialized)
                {
                    service.Initialize();
                }
                Debug.Log($"Dich vu {serviceType.Name} dang ki thanh cong.");
                serviceRegitered?.Invoke(serviceType, service);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        //Lay dich vu ra de su dung
        public static T Get<T>() where T : class
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
        public static bool Unregistered<T>() where T : class, IService
        {
            _lock.EnterWriteLock();
            try
            {
                var serviceType = typeof(T);
                if (_services.TryGetValue(serviceType, out var service))
                {
                    if (service.isInitialized)
                    {
                        service.Cleanup();
                    }
                    _services.Remove(serviceType);
                    Debug.Log($"Dich vu {serviceType.Name} da xoa thanh cong.");
                    serviceUnregitered?.Invoke(serviceType);
                    return true;
                }
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
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
            _lock.EnterWriteLock();
            try
            {
                _isShuttingDown = true;
                
                foreach (var service in _services.Values)
                {
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
                
                _services.Clear();
                Debug.Log("[ServiceLocator] All services cleared.");
            }
            finally
            {
                _isShuttingDown = false;
                _lock.ExitWriteLock();
            }
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