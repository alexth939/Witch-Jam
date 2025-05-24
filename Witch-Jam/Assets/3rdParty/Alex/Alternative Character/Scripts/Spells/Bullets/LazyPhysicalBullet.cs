using UnityEngine;

namespace Runtime
{
    public class LazyPhysicalBullet : BulletBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _initialForce;
        [SerializeField] private ForceMode _forceMode;
        [SerializeField] private float _lifeDuration = 1.0f;

        public override void Launch(Transform aimingPoint)
        {
            _rigidbody.AddForce(aimingPoint.forward * _initialForce, _forceMode);
            Destroy(gameObject, _lifeDuration);
        }
    }
}
