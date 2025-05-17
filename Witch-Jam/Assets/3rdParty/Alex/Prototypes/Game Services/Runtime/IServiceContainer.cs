using UnityEngine;

namespace Prototypes.GameServices
{
    public interface IServiceContainer
    {
        /// <summary>
        ///     Command for resolving service.
        /// </summary>
        T Get<T>() where T : class;
    }
}
