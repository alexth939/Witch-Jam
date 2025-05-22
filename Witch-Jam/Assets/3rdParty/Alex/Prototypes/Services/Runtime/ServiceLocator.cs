using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototypes.Services
{
    /// <summary>
    /// A base generic service locator to manage dependencies within a specific context.
    /// <para>
    /// Implements a simple container with manual service registration and retrieval via interfaces only.
    /// Not thread-safe.
    /// </para>
    /// </summary>
    /// <typeparam name="TImplementor">
    /// The concrete type implementing the service locator. Must derive from <see cref="ServiceLocator{TImplementor}"/> and have a parameterless constructor.
    /// </typeparam>
    public abstract class ServiceLocator<TImplementor> where TImplementor : ServiceLocator<TImplementor>, new()
    {
        private static TImplementor InstanceBackingField = null;
        private static bool IsConstructionAllowed = false;
        private readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// Protected constructor to restrict direct instantiation.
        /// <para>
        /// Throws if not constructed via the <see cref="Instance"/> property.
        /// </para>
        /// </summary>
        protected ServiceLocator()
        {
            if(IsConstructionAllowed is false)
                throw new NotSupportedException("Manual instantiation is not allowed.");
        }

        /// <summary>
        /// Gets the singleton instance of the service locator.
        /// <para>Automatically creates the instance if it does not yet exist.</para>
        /// </summary>
        public static TImplementor Instance
        {
            get
            {
                if(InstanceBackingField == null)
                    InstanceBackingField = CreateInstance();

                return InstanceBackingField;
            }
        }

        protected IEnumerable<KeyValuePair<Type, object>> RegisteredServices => _services;

        /// <summary>
        /// Resolves a registered service by its interface type.
        /// </summary>
        /// <typeparam name="TService">The interface type of the service.</typeparam>
        /// <returns>The registered service instance.</returns>
        /// <exception cref="ArgumentException">Thrown if type is not an interface.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the service is not registered.</exception>
        public TService Get<TService>() where TService : class
        {
            Type serviceType = typeof(TService);

            if(serviceType.IsInterface is false)
                throw new ArgumentException("Generic argument type must be an interface.");

            if(_services.ContainsKey(serviceType) == false)
                throw new KeyNotFoundException($"Service of type ({serviceType.Name}) is not registered in the container.");

            return GetVerifiedService<TService>(serviceType);
        }

        /// <summary>
        /// Resolves a registered service by its interface type at runtime.
        /// </summary>
        /// <param name="serviceType">The interface type of the service.</param>
        /// <returns>The registered service instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="serviceType"/> is not an interface.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the service is not registered.</exception>
        public object Get(Type serviceType)
        {
            if(serviceType is null)
                throw new ArgumentNullException(nameof(serviceType));

            if(serviceType.IsInterface is false)
                throw new ArgumentException("Service may only be resolved by interface.");

            if(_services.ContainsKey(serviceType) == false)
                throw new KeyNotFoundException($"Service of type ({serviceType.Name}) is not registered in the container.");

            return GetVerifiedService<object>(serviceType);
        }

        /// <summary>
        /// Registers a service instance under its interface type.
        /// </summary>
        /// <typeparam name="TService">Service interface type.</typeparam>
        /// <param name="service">The instance to register.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <typeparamref name="TService"/> is not an interface.</exception>
        protected internal void Add<TService>(TService service) where TService : class
        {
            Type serviceType = typeof(TService);

            if(service is null)
                throw new ArgumentNullException(nameof(service), "Cannot add a null service.");

            if(serviceType.IsInterface is false)
                throw new ArgumentException("Generic argument type must be an interface.");

            if(_services.ContainsKey(serviceType))
                Debug.LogWarning($"Service {serviceType.Name} is already registered.");
            else
                AddVerifiedService(serviceType, service);
        }

        /// <summary>
        /// Resets the instance, typically for cleanup or testing.
        /// </summary>
        protected internal void Dispose()
        {
            InstanceBackingField = null;
        }

        /// <summary>
        /// Removes a registered service by its interface type.
        /// </summary>
        /// <typeparam name="TService">The interface type of the service.</typeparam>
        protected internal void Remove<TService>()
        {
            Type serviceType = typeof(TService);

            if(serviceType.IsInterface is false)
                throw new ArgumentException("Generic argument type must be an interface.");

            if(_services.ContainsKey(serviceType))
                RemoveVerifiedService(serviceType);
            else
                Debug.LogWarning($"Service of type ({serviceType.Name}) is not registered, cannot remove.");
        }

        /// <summary>
        /// Removes a registered service by its interface type at runtime.
        /// </summary>
        /// <param name="serviceType">The type of the interface to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="serviceType"/> is not an interface.</exception>
        protected internal void Remove(Type serviceType)
        {
            if(serviceType is null)
                throw new ArgumentNullException(nameof(serviceType));

            if(serviceType.IsInterface is false)
                throw new ArgumentException("Generic argument type must be an interface.");

            if(_services.ContainsKey(serviceType))
                RemoveVerifiedService(serviceType);
            else
                Debug.LogWarning($"Service of type ({serviceType.Name}) is not registered, cannot remove.");
        }

        /// <summary>
        /// Creates an instance of the implementor type using controlled construction.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="TImplementor"/>.</returns>
        private static TImplementor CreateInstance()
        {
            IsConstructionAllowed = true;
            TImplementor instance = new();
            IsConstructionAllowed = false;

            return instance;
        }

        /// <summary>
        /// Internally registers a verified service.
        /// </summary>
        /// <param name="serviceType">The service interface type.</param>
        /// <param name="service">The service instance.</param>
        private void AddVerifiedService(Type serviceType, object service)
        {
            Debug.Log($"Adding Service ({serviceType!.Name})");
            _services.Add(serviceType!, service!);
        }

        /// <summary>
        /// Retrieves a previously verified service.
        /// </summary>
        /// <typeparam name="TService">Service interface type.</typeparam>
        /// <param name="serviceType">The key to retrieve.</param>
        /// <returns>The service instance cast to <typeparamref name="TService"/>.</returns>
        private TService GetVerifiedService<TService>(Type serviceType) => (TService)_services[serviceType!];

        /// <summary>
        /// Internally removes a verified service.
        /// </summary>
        /// <param name="serviceType">The service interface type to remove.</param>
        private void RemoveVerifiedService(Type serviceType)
        {
            Debug.Log($"Removing Service ({serviceType!.Name})");
            _services.Remove(serviceType!);
        }
    }
}
