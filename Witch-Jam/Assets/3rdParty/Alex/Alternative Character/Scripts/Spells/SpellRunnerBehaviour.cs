using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class SpellRunnerBehaviour : MonoBehaviour
    {
        [SerializeField] private int _selectedSpellIndex;
        [SerializeField] private List<SpellAsset> _spells;
        [SerializeField] private Transform _aimingIKTarget;
        private Coroutine _loopedCoroutine;
        private float _shootInterval;

        //todo move it to bullet
        private ParticleSystem _shootEffect;
        private BulletBehaviour _bulletPrefab;

        private void Update()
        {
            if(Input.mouseScrollDelta.y > 0)
            {
                _selectedSpellIndex += 1;
                float approxIndex = Mathf.Repeat(_selectedSpellIndex, _spells.Count);
                _selectedSpellIndex = Mathf.RoundToInt(approxIndex);
                SetSpell(_spells[_selectedSpellIndex]);

                Debug.Log($"approxIndex = {approxIndex}");
            }

            if(Input.mouseScrollDelta.y < 0)
            {
                _selectedSpellIndex -= 1;
                float approxIndex = Mathf.Repeat(_selectedSpellIndex, _spells.Count);
                _selectedSpellIndex = Mathf.RoundToInt(approxIndex);
                SetSpell(_spells[_selectedSpellIndex]);

                Debug.Log($"approxIndex = {approxIndex}");
            }
        }

        private void Start()
        {
            SetSpell(_spells[_selectedSpellIndex]);
        }

        public void SetSpell(SpellAsset spellAsset)
        {
            ISpell spell = spellAsset.Spell;

            Action spellSetter = spell switch
            {
                ShootingSpell shootingSpell => () => SetShootingSpell(shootingSpell),
                _ => throw new NotSupportedException($"[{spell.GetType()}] spell ot supported."),
            };

            spellSetter.Invoke();
        }

        private void SetShootingSpell(ShootingSpell spell)
        {
            Debug.Log($"set: {nameof(ShootingSpell)}");

            if(spell.LaunchEffect == null)
                Debug.LogWarning($"[weapon.ShootEffect] is null.");
            else
                _shootEffect = Instantiate(spell.LaunchEffect, _aimingIKTarget);

            _shootInterval = spell.ShootInterval;

            if(spell.Bullet == null)
                Debug.LogWarning($"[weapon.Bullet] is null.");
            else
                _bulletPrefab = spell.Bullet;
        }

        public void Shoot()
        {
            if(_shootEffect == null)
                Debug.LogWarning($"Shoot effect is null!");
            else
                _shootEffect.Play();

            BulletBehaviour bullet = Instantiate(
                _bulletPrefab,
                _aimingIKTarget.transform.position,
                _aimingIKTarget.transform.rotation);

            bullet.Launch(_aimingIKTarget.transform);
        }

        public void ToggleShootingOn()
        {
            if(Mathf.Approximately(_shootInterval, 0))
                Shoot();
            else
                StartShootingLoop();
        }

        public void ToggleShootingOff()
        {
            StopShootingLoop();
        }

        private void StartShootingLoop()
        {
            IEnumerator shootingLoop = CreateShootingLoop(_shootInterval);
            _loopedCoroutine = StartCoroutine(shootingLoop);
        }

        private void StopShootingLoop()
        {
            Debug.Log($"stop");

            if(_loopedCoroutine != null)
            {
                StopCoroutine(_loopedCoroutine);
            }
        }

        private IEnumerator CreateShootingLoop(float interval)
        {
            var waitCommand = new WaitForSeconds(interval);

            while(true)
            {
                Shoot();

                yield return waitCommand;
            }
        }
    }
}
