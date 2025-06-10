using Runtime.Events;
using Runtime.Systems.ObjectPool;
using UnityEngine;
using UnityEngine.Pool;

namespace Runtime.Objects
{
    public class CarPlaceObject : MonoBehaviour, IPoolObject<CarPlaceObject>
    {
        private ObjectPool<CarPlaceObject> _pool;
        
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
        
        public void SetPool(ObjectPool<CarPlaceObject> pool)
        {
            _pool = pool;
        }

        public void ReleasePool()
        {
            _pool.Release(this);
        }
        
    }
}