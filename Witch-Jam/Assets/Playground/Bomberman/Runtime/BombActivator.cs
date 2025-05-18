using UnityEngine;

namespace Playground.Bomberman
{
    public class BombActivator : MonoBehaviour
    {
        [SerializeField] private uint propogationsCount = 3;
        [SerializeField] private UnityEngine.Events.UnityEvent _dying;

        public void Die()
        {
            _dying.Invoke();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse2))
            {
                Collider[] colliders = Physics.OverlapSphere(
                    transform.position,
                    0.7f,
                    ~0,
                    QueryTriggerInteraction.Collide
                );

                Debug.Log($"{colliders.Length} colliders found.");

                foreach(Collider col in colliders)
                {
                    BombBehaviour bomb = col.GetComponentInParent<BombBehaviour>();

                    if(bomb != null)
                    {
                        bomb.PlantBomb(propogationsCount);
                        Debug.Log($"Activated bomb: {bomb.name}");
                    }
                }
            }
        }
    }

    public static class BombSettings
    {
        public const uint BoomPropogationLength = 3;
    }
}
