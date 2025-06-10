using UnityEngine.Pool;

namespace Runtime.Systems.ObjectPool
{
    public interface IPoolObject<T> where T : class
    {
        void SetPool(ObjectPool<T> pool);
        void ReleasePool();
    }
}