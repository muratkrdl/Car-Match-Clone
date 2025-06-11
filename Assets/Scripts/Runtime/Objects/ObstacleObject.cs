using Runtime.Events;
using Runtime.Systems.ObjectPool;
using UnityEngine;
using UnityEngine.Pool;

namespace Runtime.Objects
{
    /// <summary>
    /// Represents an ObstacleObject
    /// </summary>
    public class ObstacleObject : MonoBehaviour, IPoolObject<ObstacleObject>
    {
        private ObjectPool<ObstacleObject> _pool;

        /// <summary>
        /// Initializes the obstacle
        /// </summary>
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
        
        /// <summary>
        /// Sets this pool to his own object pool
        /// </summary>
        public void SetPool(ObjectPool<ObstacleObject> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Releases this obstacle back to the object pool
        /// </summary>
        public void ReleasePool()
        {
            _pool.Release(this);
        }
        
    }
}