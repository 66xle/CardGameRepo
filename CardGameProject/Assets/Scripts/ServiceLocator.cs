using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>
{
    private static readonly Dictionary<System.Type, object> services = new();

    public static void Register<T>(T service)
    {
        services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
            return (T)service;

        Debug.LogWarning($"Service {typeof(T)} not found");
        return default;
    }

    public static void Unregister<T>()
    {
        services.Remove(typeof(T));
    }

    public new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}
