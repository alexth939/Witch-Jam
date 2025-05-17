using UnityEngine;

namespace Prototypes.GameServices
{
    [RequireComponent(typeof(ServiceContainerInstaller))]
    public abstract class CustomServiceInstaller : MonoBehaviour
    {
        public abstract void Install(ServiceContainer container);
    }
}