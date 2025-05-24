using System.Collections;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(AlternativeCharacterView))]
    public class AlternativeCharacterController : MonoBehaviour
    {
        private const string BaseSpeedTooltip =
            "Base walking speed of the character without sprinting.";

        private const string SprintPotentialTooltip =
            "Maximum speed boost provided by sprinting, " +
            "gradually added on top of base speed based on sprint effectiveness.";

        private const string SprintEaseTooltip =
            "Time (in seconds) it takes to fully reach or " +
            "drop sprint speed when starting or stopping sprinting.";

        [SerializeField]
        private AlternativeCharacterView _view;

        [SerializeField]
        private SpellRunnerBehaviour _weapon;

        [SerializeField, Tooltip(BaseSpeedTooltip)]
        private float _baseMovingSpeed = 5f;

        [SerializeField, Tooltip(SprintPotentialTooltip)]
        private float _sprintSpeedPotential = 5f;

        [SerializeField, Tooltip(SprintEaseTooltip)]
        private float _sprintEaseDuration = 1f;

        private bool _isSprinting = false;
        private float _sprintEffectiveness = 0;

        private float FinalMovingSpeed => _baseMovingSpeed + SprintSpeedContribution;

        private float SprintSpeedContribution => 1f * _sprintEffectiveness * _sprintSpeedPotential;

        private Vector3 GetPointerRelativeToPlayerDirection()
        {
            Vector3 pointerDirection = default;
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            Plane plane = new(Vector3.up, transform.position);

            if(plane.Raycast(ray, out float distance))
            {
                Vector3 targetPoint = ray.GetPoint(distance);
                Vector3 flatTargetPoint = new(targetPoint.x, transform.position.y, targetPoint.z);

                pointerDirection = (flatTargetPoint - transform.position).normalized;
            }
            else
            {
                Debug.LogWarning($"Failed to resolve pointer position!\n" +
                    $"mouse:[{mousePosition}]\n" +
                    $"ray:[{ray}]\n" +
                    $"position:[{transform.position}]");
            }

            return pointerDirection;
        }

        //private void ShootOnce() => _view.Shoot();
        //private void ShootOnce() => _weapon.Shoot();

        private void Update()
        {
            UpdateMeleeAttack();
            UpdateBombPlant();
            UpdateSprintControl();
            UpdateMovementAndLooking();
            UpdateAimingShooting();
        }

        private void UpdateAimingShooting()
        {
            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                _view.ToggleAimingOn();
                _view.FullyAimed += _weapon.ToggleShootingOn;
            }
            else if(Input.GetKeyUp(KeyCode.Mouse1))
            {
                _view.ToggleAimingOff();
                _view.FullyAimed -= _weapon.ToggleShootingOn;
                _weapon.ToggleShootingOff();
            }

            if(_view.IsAiming)
            {
                Vector3 aimingDirection = GetPointerRelativeToPlayerDirection();
                _view.SetAimingDirection(aimingDirection);
            }
        }

        private void UpdateBombPlant()
        {
            if(Input.GetKeyDown(KeyCode.Mouse2))// MMB
            {
                _view.PerformBombPlant();
            }
        }

        private void UpdateMeleeAttack()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))// LMB
            {
                _view.PerformMeleeAttack();
            }
        }

        private void UpdateMovementAndLooking()
        {
            Vector3 movingDirection = new()
            {
                x = Input.GetAxis("Horizontal"),// Reminder: it is animated value! (0f to 1f)
                z = Input.GetAxis("Vertical"),// Reminder: it is animated value! (0f to 1f)
            };

            bool shouldMove = Mathf.Approximately(movingDirection.sqrMagnitude, 0f) == false;

            if(shouldMove)
            {
                movingDirection = Vector3.ClampMagnitude(movingDirection, 1f);
                float finalMovingSpeed = FinalMovingSpeed * movingDirection.magnitude;

                _view.SetMovingDirection(movingDirection);
                _view.SetMovingSpeed(finalMovingSpeed);
                _view.LookAt(movingDirection);
            }
        }

        private void UpdateSprintControl()
        {
            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                _view.PlaySprintEffect();
                _isSprinting = true;
            }

            if(Input.GetKeyUp(KeyCode.LeftShift))
            {
                _view.StopSprintEffect();
                _isSprinting = false;
            }

            float sprintEffectivenessTarget = _isSprinting ? 1f : 0f;

            if(_sprintEaseDuration > 0f)
            {
                _sprintEffectiveness = Mathf.MoveTowards(
                    _sprintEffectiveness,
                    sprintEffectivenessTarget,
                    Time.deltaTime / _sprintEaseDuration);
            }
            else
            {
                _sprintEffectiveness = 1f;
            }
        }
    }
}
