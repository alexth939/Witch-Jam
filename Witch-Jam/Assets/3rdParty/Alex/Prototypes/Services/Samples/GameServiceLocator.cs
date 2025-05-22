using UnityEngine;

namespace Prototypes.Services.Samples
{
    /// <summary>
    /// A sample service locator tailored for game context.
    /// Supports both plain C# services and MonoBehaviour-based services.
    /// </summary>
    /// <remarks>
    /// If services are MonoBehaviours, it is your responsibility to manage their lifecycle across scene loads.
    /// </remarks>
    public sealed class GameServiceLocator : ServiceLocator<GameServiceLocator>
    {
        /// <summary>
        /// Registers a service in the locator.
        /// </summary>
        /// <typeparam name="T">The interface type of the service.</typeparam>
        /// <param name="service">The service instance to register.</param>
        internal new void Add<T>(T service) where T : class => base.Add(service);

        /// <summary>
        /// Removes a service and destroys its GameObject if it's a MonoBehaviour.
        /// </summary>
        /// <typeparam name="T">The interface type of the service.</typeparam>
        internal new void Remove<T>() where T : class
        {
            T service = Get<T>();
            base.Remove<T>();

            if(service is MonoBehaviour component)
            {
                Object.Destroy(component.gameObject);
            }
        }

        // what's ur take on that? is it good idea to expose to implementor this way?
        //exposed via:
        // private field: private readonly Dictionary<Type, object> _services = new();
        // exposer: protected IEnumerable<KeyValuePair<Type, object>> ServiceCollection => _services;
        internal new void Dispose()
        {
            foreach(var service in RegisteredServices)
            {
                if(service.Value is MonoBehaviour component)
                    Object.Destroy(component.gameObject);
            }

            base.Dispose();
        }
    }
}
