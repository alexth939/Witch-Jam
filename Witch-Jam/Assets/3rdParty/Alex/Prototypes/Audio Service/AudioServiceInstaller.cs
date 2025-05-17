using Prototypes.GameServices;
using UnityEngine;

namespace Runtime.Services
{
    public class AudioServiceInstaller : CustomServiceInstaller
    {
        [SerializeField] private AudioServiceBehaviour _notDestroyableService;

        public override void Install(ServiceContainer container)
        {
            container.Add<IAudioService>(_notDestroyableService);
        }
    }
}
