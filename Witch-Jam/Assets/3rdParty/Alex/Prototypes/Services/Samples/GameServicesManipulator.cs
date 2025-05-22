using System.Collections.Generic;
using UnityEngine;

namespace Prototypes.Services.Samples
{
    /// <summary>
    /// Executes service installation attached to the GameObject
    /// based on the selected Unity lifecycle phase (Awake or Start).
    /// </summary>
    internal class GameServicesManipulator : MonoBehaviour
    {
        private const string DisposeBeforeInstallTooltip =
            "If enabled, disposes the current service locator instance before installing services. " +
            "Use this to ensure a clean slate in scenes with repeated or overlapping service setups.";

        [SerializeField, Tooltip(DisposeBeforeInstallTooltip)]
        private bool _shouldDisposeBeforeInstall = false;

        [SerializeField]
        private bool _shouldDisposeOnDestroy = false;

        [SerializeField]
        private ServiceInstallationPhase _installationPhase = ServiceInstallationPhase.None;

        private GameServiceLocator Locator => GameServiceLocator.Instance;

        private void Awake()
        {
            if(_installationPhase is ServiceInstallationPhase.Awake)
                InstallAttachedServices();
        }

        private void Start()
        {
            if(_installationPhase is ServiceInstallationPhase.Start)
                InstallAttachedServices();
        }

        private void OnDestroy()
        {
            if(_shouldDisposeOnDestroy)
                Locator.Dispose();
        }

        /// <summary>
        /// Finds all <see cref="ServiceInstallerBehaviour"/> components on the same GameObject
        /// and installs them using the shared GameServiceLocator instance.
        /// </summary>
        private void InstallAttachedServices()
        {
            if(_shouldDisposeBeforeInstall)
                Locator.Dispose();

            List<ServiceInstallerBehaviour> customInstallers = new();
            GetComponents(customInstallers);
            customInstallers.ForEach(installer => installer.Install(Locator));
        }

        /// <summary>
        /// Unity lifecycle moment when services should be installed.
        /// </summary>
        private enum ServiceInstallationPhase
        {
            None,
            Awake,
            Start
        }
    }
}
