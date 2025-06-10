using Runtime.Events;
using Runtime.Systems.ObjectPool;
using UnityEngine;
using UnityEngine.Pool;

namespace Runtime.Objects
{
    public class ObstacleObject : MonoBehaviour, IPoolObject<ObstacleObject>
    {
        private ObjectPool<ObstacleObject> _pool;

        public void Initialize(Vector3 getWorldPosition, Transform parent)
        {
            transform.localPosition = getWorldPosition;
            transform.SetParent(parent);
        }
        
        private void OnEnable()
        {
            CoreGameEvents.Instance.onResetLevel += ReleasePool;
        }

        private void OnDisable()
        {
            CoreGameEvents.Instance.onResetLevel -= ReleasePool;
        }
        
        public void SetPool(ObjectPool<ObstacleObject> pool)
        {
            _pool = pool;
        }

        public void ReleasePool()
        {
            _pool.Release(this);
        }
        
    }
}