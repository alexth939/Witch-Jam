using UnityEngine;

namespace Runtime
{
    [System.Serializable]
    public abstract class BulletBehaviour : MonoBehaviour
    {
        public abstract void Launch(Transform aimingPoint);
    }
}
