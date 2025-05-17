using System.Collections.Generic;
using UnityEngine;

namespace Prototypes.GameServices
{
    public class ServiceContainerInstaller : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _servicesToRegister;
        [SerializeField] private MonoBehaviour[] _servicesToUnregister;
        [SerializeField] private bool _shouldDisposeWhenDestroyed;

        private ServiceContainer Container => (ServiceContainer)ServiceContainer.Instance;

        public void DisposeContainer() => Container.Dispose();

        private void OnDestroy()
        {
            if(_shouldDisposeWhenDestroyed)
                DisposeContainer();
        }

        private void RegisterServices()
        {
            foreach(MonoBehaviour service in _servicesToRegister)
            {
                Container.Add(service);
            }

            List<CustomServiceInstaller> customInstallers = new();
            GetComponents<CustomServiceInstaller>(customInstallers);
            customInstallers.ForEach(installer => installer.Install(Container));
        }

        private void Start()
        {
            RegisterServices();
            UnregisterServices();
        }

        private void UnregisterServices()
        {
            foreach(MonoBehaviour service in _servicesToUnregister)
            {
                System.Type serviceType = service.GetType();
                Container.Remove(serviceType);
            }
        }
    }
}
