using UnityEngine;

namespace Runtime.Extensions
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad;
        
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;
                
                _instance = FindAnyObjectByType<T>();
                if (_instance) return _instance;
                
                GameObject obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            var instance = Instance;

            if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        
    }
}