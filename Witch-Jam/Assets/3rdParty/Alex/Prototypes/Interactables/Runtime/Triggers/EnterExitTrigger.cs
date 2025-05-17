using System;
using UnityEngine;
using UltEvents;

namespace Prototypes.Interactables.Triggers
{
    public class EnterExitTrigger : MonoBehaviour
    {
        [SerializeField]
        private UltEvent<Transform> _objectEntered;

        [SerializeField]
        private UltEvent<Transform> _objectExited;

#if !UNITY_2022_2_OR_NEWER
        [InfoBox("Only objects from these layers will activate the trigger.")]
        [SerializeField]
        private LayerMask _triggeringLayers;
#endif
        public UltEvent<Transform> ObjectEntered => _objectEntered;

        public UltEvent<Transform> ObjectExited => _objectExited;

        protected virtual void OnTriggerEnter(Collider other)
        {
#if !UNITY_2022_2_OR_NEWER
            if(_triggeringLayers.Contains(other.gameObject.layer) == false)
                return;
#endif
            ValidateObjectEnteredEvent();

            Debug.Log($"{other.name} entered trigger. Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

            _objectEntered.Invoke(other.transform);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
#if !UNITY_2022_2_OR_NEWER
            if (_triggeringLayers.Contains(other.gameObject.layer) == false)
                return;
#endif
            ValidateObjectExitedEvent();

            Debug.Log($"{other.name} exited trigger. Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

            _objectExited.Invoke(other.transform);
        }

        private void ValidateObjectEnteredEvent()
        {
            if(_objectEntered == null)
                throw new NullReferenceException($"A rare internal error occurred: " +
                    $"the '{nameof(_objectEntered)}' event on '{nameof(EnterExitTrigger)}' was null. " +
                    "This means no response could be triggered when an object entered. " +
                    "Please ensure the event is properly assigned in the Inspector or via code.");
        }

        private void ValidateObjectExitedEvent()
        {
            if(_objectExited == null)
                throw new NullReferenceException($"A rare internal error occurred: " +
                    $"the '{nameof(_objectExited)}' event on '{nameof(EnterExitTrigger)}' was null. " +
                    "This means no response could be triggered when an object exited. " +
                    "Please ensure the event is properly assigned in the Inspector or via code.");
        }
    }
}