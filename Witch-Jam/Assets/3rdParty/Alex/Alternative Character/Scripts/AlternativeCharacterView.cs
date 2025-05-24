using System;
using UnityEngine;

namespace Runtime
{
    public class AlternativeCharacterView : MonoBehaviour
    {
        [SerializeField] private float _aimingBlendSpeed = 3f;
        [SerializeField] private AlternativeCharacterAnimatorStuff _animationStuff;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _aimingIKTarget;
        [SerializeField, Range(0f, 1f)] private float _maxHandIKWeight;
        [SerializeField, Range(0f, 1f)] private float _maxHeadIKWeight;
        [SerializeField] private ParticleSystem _sprintEffect;
        [SerializeField] private float _turningSpeed = 500f;

        private Vector3 _movingDirection = Vector3.zero;
        private float _movingSpeed;
        private Vector3 _targetLookDirection = Vector3.zero;
        private Vector3 _aimingDirection = Vector3.zero;
        private float _aimingWeight = 0;
        private float _aimingWeightTarget = 0;

        public event Action FullyAimed;

        public event Action FullyUnaimed;

        public float AimingBlendSpeed
        {
            get => _aimingBlendSpeed;
            set => _aimingBlendSpeed = value;
        }

        public float GravityValue { get; set; } = 9f;

        public bool IsAiming => _aimingWeight > 0f;

        public bool IsGravityApplied { get; set; } = true;

        //public float MoveSpeed
        //{
        //    get => _moveSpeed;
        //    set => _moveSpeed = value;
        //}

        public float MovingSpeed
        {
            get => _movingSpeed;
            set => _movingSpeed = value;
        }

        public float TurningSpeed
        {
            get => _turningSpeed;
            set => _turningSpeed = value;
        }

        private Animator Animator => _animationStuff.Animator;

        /// <summary>
        ///     Sets target look rotation.
        /// </summary>
        public void LookAt(Vector3 direction) => _targetLookDirection = direction;

        public void PerformBombPlant() => Animator.SetTrigger(_animationStuff.PlantBombParameter);

        public void PerformMeleeAttack() => Animator.SetTrigger(_animationStuff.MeleeAttackParameter);

        public void PlaySprintEffect()
        {
            if(_sprintEffect != null)
                _sprintEffect.Play();
            else
                Debug.LogWarning("Sprint effect was not assigned.");
        }

        public void SetAimingDirection(Vector3 direction)
        {
            _aimingDirection = direction;
            _aimingIKTarget.transform.position = transform.position + Vector3.up * 1f + _aimingDirection;
            _aimingIKTarget.LookAt(_aimingIKTarget.transform.position + _aimingDirection, Vector3.up);
        }

        public void SetMovingDirection(Vector3 direction) => _movingDirection = direction;

        public void SetMovingSpeed(float speed) => _movingSpeed = speed;

        public void StopSprintEffect()
        {
            if(_sprintEffect != null)
                _sprintEffect.Stop();
            else
                Debug.LogWarning("Sprint effect was not assigned.");
        }

        /// <summary>
        ///     Starts blending the aiming IKs toward no aiming (weight = 0).
        /// </summary>
        public void ToggleAimingOff() => _aimingWeightTarget = 0f;

        /// <summary>
        ///     Starts blending the aiming IKs toward full aiming (weight = 1).
        /// </summary>
        public void ToggleAimingOn() => _aimingWeightTarget = 1f;

        private void OnAnimatorIK(int layerIndex)
        {
            if(IsAiming)
            {
                float globalWeight = _maxHeadIKWeight * _aimingWeight;
                float headWeight = _maxHeadIKWeight * _aimingWeight;
                float handWeight = _maxHandIKWeight * _aimingWeight;

                Animator.SetLookAtWeight(globalWeight, headWeight);
                Animator.SetLookAtPosition(_aimingIKTarget.position);

                Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handWeight);
                Animator.SetIKPosition(AvatarIKGoal.RightHand, _aimingIKTarget.position);
            }
        }

        private void Update()
        {
            float lastFrameDuration = Time.deltaTime;

            UpdateAiming(lastFrameDuration);
            UpdateRotation(lastFrameDuration);
            UpdateCharacterMovement(lastFrameDuration);
            UpdateAnimatorMovement();
        }

        private void UpdateAiming(float lastFrameDuration)
        {
            float previousAimingWeight = _aimingWeight;
            float blendDelta = _aimingBlendSpeed * lastFrameDuration;
            _aimingWeight = Mathf.MoveTowards(_aimingWeight, _aimingWeightTarget, blendDelta);

            if(_aimingWeight > previousAimingWeight && Mathf.Approximately(_aimingWeight, 1f))
                FullyAimed?.Invoke();

            if(_aimingWeight < previousAimingWeight && Mathf.Approximately(_aimingWeight, 0f))
                FullyUnaimed?.Invoke();
        }

        private void UpdateAnimatorMovement()
        {
            float idleToRunBlendValue = _movingDirection.magnitude;
            Animator.SetFloat(_animationStuff.RunBlendParameter, idleToRunBlendValue);
        }

        private void UpdateCharacterMovement(float lastFrameDuration)
        {
            Vector3 movementDelta = _movingDirection * (_movingSpeed * lastFrameDuration);

            if(IsGravityApplied)
                movementDelta += Vector3.down * (GravityValue * lastFrameDuration);

            _ = _controller.Move(movementDelta);
        }

        private void UpdateRotation(float lastFrameDuration)
        {
            if(Mathf.Approximately(_targetLookDirection.sqrMagnitude, 0f) == false)
            {
                Quaternion finalLookRotation = Quaternion.LookRotation(_targetLookDirection, Vector3.up);
                Quaternion rotationA = transform.rotation;
                Quaternion rotationB = finalLookRotation;
                float turningDelta = _turningSpeed * lastFrameDuration;

                transform.rotation = Quaternion.RotateTowards(rotationA, rotationB, turningDelta);
            }
        }
    }
}
