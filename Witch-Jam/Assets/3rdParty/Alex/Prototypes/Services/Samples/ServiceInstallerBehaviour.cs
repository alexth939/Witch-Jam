using UnityEngine;

namespace Prototypes.Services.Samples
{
    /// <summary>
    /// Base class for service installers that should be attached to GameObjects.
    /// Use this to declaratively register services during runtime.
    /// </summary>
    [RequireComponent(typeof(GameServicesManipulator))]
    public abstract class ServiceInstallerBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Override to register one or more services into the provided locator.
        /// </summary>
        /// <param name="locator">The GameServiceLocator instance to install services into.</param>
        public abstract void Install(GameServiceLocator locator);
    }
}
