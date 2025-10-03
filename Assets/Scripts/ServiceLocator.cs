using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator _instance;
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ServiceLocator>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ServiceLocator");
                    _instance = go.AddComponent<ServiceLocator>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterService<T>(T service)
    {
        Type serviceType = typeof(T);

        if (_services.ContainsKey(serviceType))
        {
            Debug.LogWarning($"Service already registered: {serviceType}");  
        }
        
        _services[serviceType] = service;
        Debug.Log($"Service registered: {serviceType}");
    }

    public T GetService<T>() where T : class
    {
        Type serviceType = typeof(T);
        if (_services.TryGetValue(serviceType, out object service))
        {
            return service as T;
        }
        Debug.LogError($"Service not found: {serviceType}");
        return null;
    }
    
    public void UnregisterService<T>()
    {
        Type serviceType = typeof(T);
        if (_services.ContainsKey(serviceType))
        {
            _services.Remove(serviceType);
            Debug.Log($"Service unregistered: {serviceType}"); 
        }
        else
        {
            Debug.LogWarning($"Service is not registered: {serviceType}");
        }
    }

    public bool IsServiceRegistered<T>() where T : class
    {
        return _services.ContainsKey(typeof(T));
    }

    public void ClearAllServices()
    {
        _services.Clear();
        Debug.Log("All services cleared");
    }
}
