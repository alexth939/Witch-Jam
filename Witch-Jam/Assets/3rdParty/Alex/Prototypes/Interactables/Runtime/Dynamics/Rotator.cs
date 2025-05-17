using UnityEngine;

namespace Prototypes.Interactables.Dynamics
{
    public sealed class Rotator : MonoBehaviour
    {
        private bool _isRotating;

        [SerializeField]
        private bool _shouldWorkAtStart = true;

        [SerializeField]
        private Vector3 _rotationSpeed = new Vector3(0, 90f, 0);

        [SerializeField]
        private Space _space = Space.Self;

        public void StartRotation() => _isRotating = true;

        public void StopRotation() => _isRotating = false;

        public void ToggleRotation() => _isRotating = !_isRotating;

        private void Start() => _isRotating = _shouldWorkAtStart;

        private void Update()
        {
            if(_isRotating)
            {
                transform.Rotate(_rotationSpeed * Time.deltaTime, _space);
            }
        }
    }
}
