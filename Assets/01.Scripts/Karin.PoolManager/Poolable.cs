using Codice.Client.Common;
using UnityEngine;
using UnityEngine.Pool;

namespace Karin.PoolingSystem
{
    public class Poolable : MonoBehaviour
    {
        public IObjectPool<GameObject> pool { get; set; }

        public virtual void Release()
        {
            if (gameObject.activeInHierarchy)
            {
                pool.Release(gameObject);
            }
        }

    }
}
