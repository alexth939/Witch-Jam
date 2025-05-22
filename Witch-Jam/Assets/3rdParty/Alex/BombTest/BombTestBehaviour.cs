using UnityEngine;
using NaughtyAttributes;

public class BombTestBehaviour : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private float _force;
    [SerializeField] private ParticleSystem _boomEffect;

    [Button]
    public void Explode()
    {
        _boomEffect.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, -1, QueryTriggerInteraction.Ignore);

        Debug.Log($"colliders found: {colliders.Length}");

        foreach(Collider collider in colliders)
        {
            if(collider.TryGetComponent<Rigidbody>(out Rigidbody body))
            {
                body.AddExplosionForce(_force, transform.position, _radius);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
