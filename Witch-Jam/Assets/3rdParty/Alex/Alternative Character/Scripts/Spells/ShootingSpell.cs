using UnityEngine;

namespace Runtime
{
    public class ShootingSpell : ISpell
    {
        [SerializeField] private ParticleSystem _bulletLaunchEffect;
        [SerializeField, Range(0f, 5f)] private float _shootInterval = 0.5f;
        [SerializeField] private BulletBehaviour _bullet;

        public ParticleSystem LaunchEffect => _bulletLaunchEffect;

        public float ShootInterval => _shootInterval;

        public BulletBehaviour Bullet => _bullet;
    }
}
