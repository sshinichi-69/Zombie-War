using UnityEngine;
using UnityEngine.Pool;

namespace ZombieWar
{
    public abstract class ObjectPoolTemplate<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected T m_prefab;
        protected ObjectPool<T> m_pool;

        private void Awake()
        {
            m_pool = new ObjectPool<T>(CreateObject, OnGetFromPool, OnReleaseToPool);
        }

        public T GenerateObject()
        {
            return m_pool.Get();
        }

        public void DestroyObject(T obj)
        {
            m_pool.Release(obj);
        }

        protected virtual T CreateObject()
        {
            GameObject obj = Instantiate(m_prefab.gameObject);
            obj.transform.SetParent(transform);
            return obj.GetComponent<T>();
        }

        protected virtual void OnGetFromPool(T obj)
        {
            obj.gameObject.SetActive(true);
        }

        protected virtual void OnReleaseToPool(T obj)
        {
            obj.transform.SetParent(transform);
            obj.gameObject.SetActive(false);
        }
    }
}
