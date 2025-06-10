using Runtime.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace Runtime.Systems.ObjectPool
{
    public abstract class BaseObjectPool<T1,T2> : MonoSingleton<T2> where T1 : MonoBehaviour, IPoolObject<T1> where T2 : MonoBehaviour
    {
        [SerializeField] protected T1 prefab;

        [SerializeField] private int defaultPoolSize = 10;
        [SerializeField] private int maxPoolSize = 100;

        private ObjectPool<T1> _pool;

        protected override void Awake()
        {
            base.Awake();
            _pool = new ObjectPool<T1> 
            (
                ObCreate,
                OnGet,
                OnRelease,
                OnDestroyForPool,
                true,
                defaultPoolSize,
                maxPoolSize 
            );
        }

        protected virtual T1 ObCreate()
        {
            var obj = Instantiate(prefab, transform);
            obj.SetPool(_pool);
            return obj;
        }

        protected virtual void OnGet(T1 obj)
        {
            obj.gameObject.SetActive(true);
        }

        protected virtual void OnRelease(T1 obj)
        {
            obj.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyForPool(T1 obj)
        {
            Destroy(obj);
        }

        public T1 GetFromPool()
        {
            return _pool.Get();
        }
        
    }
}