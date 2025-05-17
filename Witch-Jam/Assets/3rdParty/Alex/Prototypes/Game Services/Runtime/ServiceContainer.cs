using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototypes.GameServices
{
    public sealed class ServiceContainer : IServiceContainer
    {
        private static ServiceContainer ContainerInstance = null;

        private Dictionary<Type, object> _services = new();

        private ServiceContainer()
        {
        }

        public void Dispose()
        {
            _services = null;
            ContainerInstance = null;
        }

        /// <summary>
        ///     Lazily initializes a new service container instance on first access.<br/>
        ///     Do not cache this reference — always access via this property.
        /// </summary>
        public static IServiceContainer Instance => ContainerInstance ??= new ServiceContainer();

        /// <summary>
        ///     Designed to bind service by interface.
        /// </summary>
        /// <typeparam name="T">Service's interface.</typeparam>
        public void Add<T>(object service) where T : class
        {
            Type serviceType = typeof(T);

            if(service is null)
                throw new ArgumentNullException(nameof(service), "Cannot add a null service.");

            if(service is not T)
                throw new InvalidCastException($"Service does not implement or derive from {typeof(T)}.");

            if(_services.ContainsKey(serviceType))
                Debug.LogWarning($"Service of type ({serviceType.Name}) already registered in the container.");
            else
                AddVerifiedService(serviceType, service);
        }

        /// <summary>
        ///     Designed to bind service by its concrete type.
        /// </summary>
        public void Add(object service)
        {
            Type serviceType = service.GetType();

            if(service is null)
                throw new ArgumentNullException(nameof(service), "Cannot add a null service.");

            if(_services.ContainsKey(serviceType))
                Debug.LogWarning($"Service of type ({serviceType.Name}) already registered in the container.");
            else
                AddVerifiedService(serviceType, service);
        }

        private void AddVerifiedService(Type serviceType, object service)
        {
            Debug.Log($"Adding Service ({serviceType.Name})");
            _services.Add(serviceType, service);
        }

        public T Get<T>() where T : class
        {
            Type serviceType = typeof(T);

            if(_services.ContainsKey(serviceType) == false)
                throw new KeyNotFoundException($"Service of type ({serviceType.Name}) is not registered in the container.");

            return GetVerifiedService<T>(serviceType);
        }

        public object Get(Type serviceType)
        {
            if(_services.ContainsKey(serviceType) == false)
                throw new KeyNotFoundException($"Service of type ({serviceType.Name}) is not registered in the container.");

            return GetVerifiedService<object>(serviceType);
        }

        private T GetVerifiedService<T>(Type serviceType) => (T)_services[serviceType];

        public void Remove<T>()
        {
            Type serviceType = typeof(T);

            if(_services.ContainsKey(serviceType))
                RemoveVerifiedService(serviceType);
            else
                Debug.LogWarning($"Service of type ({serviceType.Name}) is not registered, cannot remove.");
        }

        private void RemoveVerifiedService(Type serviceType)
        {
            Debug.Log($"Removing Service ({serviceType.Name})");
            _services.Remove(serviceType);
        }

        public void Remove(Type serviceType)
        {
            if(_services.ContainsKey(serviceType))
                RemoveVerifiedService(serviceType);
            else
                Debug.LogWarning($"Service of type ({serviceType.Name}) is not registered, cannot remove.");
        }
    }
}
