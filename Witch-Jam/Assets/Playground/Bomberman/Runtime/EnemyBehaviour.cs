using UnityEngine;
using UnityEngine.AI;

namespace Playground.Bomberman
{
    public class EnemyBehaviour : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform _player;

        private void FixedUpdate()
        {
            _agent.SetDestination(_player.position);
        }
    }
}
