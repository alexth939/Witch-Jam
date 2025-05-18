using System;
using UnityEngine;

namespace Playground.Bomberman
{
    public class BombBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _bombVisual;
        [SerializeField] private ParticleSystem _boomEffect;

        [Header("Timing")]
        [SerializeField] private float _boomDelay = 1f;
        [SerializeField] private float _boomDuration = 1f;
        [SerializeField] private float _boomNeighboorDelay = 0.5f;

        [Header("Boom Spread")]
        [SerializeField] private LayerMask _blockMask;

        private State _state = State.Halting;
        private float _stateTime = 0f;
        private uint _remainingPropogationsAmount = 0;

        private BoomDirections _pendingBoomPropagation = BoomDirections.None;

        private enum State
        {
            Halting,
            Ticking,
            Booming
        }

        [Flags]
        private enum BoomDirections
        {
            None = 0,
            Left = 1 << 0,
            Right = 1 << 1,
            Up = 1 << 2,
            Down = 1 << 3,
            All = Left | Right | Up | Down
        }

        private void Update()
        {
            switch(_state)
            {
                case State.Ticking:
                    _stateTime += Time.deltaTime;

                    if(_stateTime >= _boomDelay)
                        StartBoom(BoomDirections.All);
                    break;

                case State.Booming:
                    _stateTime += Time.deltaTime;

                    if(_pendingBoomPropagation != BoomDirections.None &&
                        _remainingPropogationsAmount > 0 &&
                        _stateTime >= _boomNeighboorDelay)
                    {
                        PropagateBoom();
                        _pendingBoomPropagation = BoomDirections.None;
                    }

                    if(_stateTime >= _boomDuration)
                        StartHalting();
                    break;
            }
        }

        public void PlantBomb(uint remainingPropogationsAmount)
        {
            if(_state != State.Halting)
            {
                Debug.Log($"Can't plant bomb because it's busy in [{_state}] state.");
                return;
            }

            _remainingPropogationsAmount = remainingPropogationsAmount;
            _state = State.Ticking;
            _stateTime = 0f;
            _pendingBoomPropagation = BoomDirections.None;
            ShowBomb();
        }

        private void ForceBoom(BoomDirections boomPropogationDirections, uint remainingPropogationsAmount)
        {
            if(_state is State.Halting)
            {
                _remainingPropogationsAmount = remainingPropogationsAmount;
                StartBoom(boomPropogationDirections);
            }
            else if(_state is State.Ticking)
            {
                _remainingPropogationsAmount = BombSettings.BoomPropogationLength;
                StartBoom(BoomDirections.All);
            }
            else if(_state is State.Booming)
            {

            }
        }

        private void StartBoom(BoomDirections directions)
        {
            _state = State.Booming;
            _stateTime = 0f;
            _pendingBoomPropagation = directions;

            HideBomb();
            _boomEffect.Play();

            // Destroy any "Enemy" GameObjects within explosion range
            float radius = 1.5f;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, ~0, QueryTriggerInteraction.Collide);

            foreach(var hit in hitColliders)
            {
                GameObject obj = hit.gameObject;

                // Destroy enemies
                if(obj.layer == LayerMask.NameToLayer("Enemy"))
                {
                    Destroy(obj);
                    continue;
                }

                // Trigger Die() on BombActivator if on Player layer
                if(obj.layer == LayerMask.NameToLayer("Player"))
                {
                    BombActivator activator = obj.GetComponent<BombActivator>();
                    if(activator != null)
                    {
                        activator.Die();
                    }
                }
            }
        }

        private void PropagateBoom()
        {
            _remainingPropogationsAmount -= 1;

            if(_pendingBoomPropagation.HasFlag(BoomDirections.All))
            {
                PropogateBoom(BoomDirections.Left, _remainingPropogationsAmount);
                PropogateBoom(BoomDirections.Right, _remainingPropogationsAmount);
                PropogateBoom(BoomDirections.Up, _remainingPropogationsAmount);
                PropogateBoom(BoomDirections.Down, _remainingPropogationsAmount);
            }
            else if(_pendingBoomPropagation is BoomDirections.None)
            {
                return;
            }
            else
            {
                PropogateBoom(_pendingBoomPropagation, _remainingPropogationsAmount);
            }
        }

        private void PropogateBoom(BoomDirections propogationDirection, uint remainingPropogationsAmount)
        {
            Vector3 origin = transform.position;
            Vector3 direction = propogationDirection switch
            {
                BoomDirections.Left => Vector3.left,
                BoomDirections.Right => Vector3.right,
                BoomDirections.Up => Vector3.forward,
                BoomDirections.Down => Vector3.back,
                BoomDirections.All or BoomDirections.None or _ => throw default,
            };

            Vector3 checkPos = origin + direction * 2f;
            Vector3 rayStart = checkPos + Vector3.up * 1.5f;
            Vector3 rayDir = Vector3.down;

            if(Physics.Raycast(rayStart, rayDir, out RaycastHit hit, 1f, ~0, QueryTriggerInteraction.Collide))
            {
                GameObject target = hit.collider.gameObject;

                if(((1 << target.layer) & _blockMask) != 0)
                    return;

                BombBehaviour other = hit.collider.GetComponentInParent<BombBehaviour>();
                if(other == null)
                    other = hit.collider.GetComponentInChildren<BombBehaviour>();

                if(other != null && other != this)
                    other.ForceBoom(propogationDirection, remainingPropogationsAmount);
            }
        }

        private void StartHalting()
        {
            _state = State.Halting;
            _stateTime = 0f;
            HideBomb();
        }

        private void ShowBomb()
        {
            if(_bombVisual != null)
                _bombVisual.gameObject.SetActive(true);
            else
                Debug.Log($"[_bombVisual] was not assigned.");
        }

        private void HideBomb()
        {
            if(_bombVisual != null)
                _bombVisual.gameObject.SetActive(false);
            else
                Debug.Log($"[_bombVisual] was not assigned.");
        }

        public bool IsEmpty => _state == State.Halting;
    }
}
