using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

namespace Prototypes.Interactables.Triggers
{
    public class EnterExitStayTrigger : EnterExitTrigger
    {
        private readonly Dictionary<Collider, Coroutine> _objectsInside = new();

        [SerializeField]
        private UltEvent<Transform> _objectStayed;

        [SerializeField] private float _stayNotificationInterval = 1.0f;

        public UltEvent<Transform> ObjectStayed => _objectStayed;

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            RegisterNewStayNotificationLoop(other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            RemoveStayingObject(other);
        }

        private IEnumerator CreateStayNotificationLoop(Collider stayingObject)
        {
            while(true)
            {
                yield return new WaitForSeconds(_stayNotificationInterval);

                ValidateObjectStayedEvent();

                _objectStayed.Invoke(stayingObject.transform);

                if(_objectsInside.Count > 0)
                    Debug.Log($"Objects stayed: {_objectsInside.Count}");
            }
        }

        private void RegisterNewStayNotificationLoop(Collider stayingObject)
        {
            ValidateStayingObjectRegistration(stayingObject);

            IEnumerator loop = CreateStayNotificationLoop(stayingObject);
            Coroutine stayNotificationCoroutine = StartCoroutine(loop);
            _objectsInside.Add(stayingObject, stayNotificationCoroutine);
        }

        private void RemoveStayingObject(Collider stayingObject)
        {
            if(_objectsInside.Remove(stayingObject, out Coroutine stayNotificationCoroutine))
            {
                StopCoroutine(stayNotificationCoroutine);
            }
            else
            {
                Debug.LogWarning($"attempted to remove an unregistered object from '{nameof(_objectsInside)}'.");
            }
        }

        private void ValidateObjectStayedEvent()
        {
            if(_objectStayed == null)
                throw new NullReferenceException($"A rare internal error occurred: " +
                    $"the '{nameof(_objectStayed)}' event on '{nameof(EnterExitStayTrigger)}' was null. " +
                    "This means no response could be triggered when an object stayed within the trigger. " +
                    "Please ensure the event is properly assigned in the Inspector or via code.");
        }

        private void ValidateStayingObjectRegistration(Collider stayingObject)
        {
            if(_objectsInside.ContainsKey(stayingObject))
                throw new InvalidOperationException($"A rare internal error occurred: " +
                    $"the object '{stayingObject}' was already registered as inside in '{nameof(EnterExitStayTrigger)}'. " +
                    "This means it triggered enter while already tracked. " +
                    "Please ensure trigger enter/exit events are correctly handled.");
        }
    }
}
