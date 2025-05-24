using UnityEngine;

namespace Runtime
{
    public class ConstantSpeedBullet : BulletBehaviour
    {
        [SerializeField] private ParticleSystem _movingEffect;
        [SerializeField] private float _movingSpeed = 1f;
        [SerializeField] private float _lifeDuration = 1.0f;
        private Vector3 _movingDirection;

        public override void Launch(Transform aimingPoint)
        {
            _movingDirection = aimingPoint.forward;
            _movingEffect.Play();
            Destroy(gameObject, _lifeDuration);
        }

        private void FixedUpdate()
        {
            transform.position += _movingDirection * _movingSpeed;
        }
    }
}
