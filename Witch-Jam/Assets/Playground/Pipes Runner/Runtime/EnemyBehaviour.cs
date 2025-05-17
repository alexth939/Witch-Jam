using UnityEngine;

namespace Playground.PipesRunner
{
    public class EnemyBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform _chasingTarget;
        [SerializeField] private float _chasingSpeed = 1f;

        private void Update()
        {
            Vector3 pointA = transform.position;
            Vector3 pointB = _chasingTarget.position;
            float movementDelta = _chasingSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(pointA, pointB, movementDelta);
        }
    }
}
