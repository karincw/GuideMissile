using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


namespace Karin.PoolingSystem
{
    public class PoolManager : MonoBehaviour
    {
        [System.Serializable]
        class PoolObjectInfo
        {
            public string objectName;
            public GameObject prefab;
            public int count;
        }

        public bool IsReady { get; private set; }

        [SerializeField]
        private PoolObjectInfo[] objectInfos = null;

        private string objectName;

        private Dictionary<string, IObjectPool<GameObject>> objectPoolDictionary = new Dictionary<string, IObjectPool<GameObject>>();
        private Dictionary<string, GameObject> gameObjectDictionary = new Dictionary<string, GameObject>();

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            IsReady = false;

            for (int idx = 0; idx < objectInfos.Length; idx++)
            {
                IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreateNewObject, OnGetPoolObject, OnReleasePoolObject, OnDestroyPoolObject, true, objectInfos[idx].count, objectInfos[idx].count);

                if (gameObjectDictionary.ContainsKey(objectInfos[idx].objectName))
                {
                    Debug.LogFormat("{0} : Already assigned.", objectInfos[idx].objectName);
                }

                gameObjectDictionary.Add(objectInfos[idx].objectName, objectInfos[idx].prefab);
                objectPoolDictionary.Add(objectInfos[idx].objectName, pool);

                // count만큼 생성해놓고 일단 회수
                for (int i = 0; i < objectInfos[idx].count; i++)
                {
                    objectName = objectInfos[idx].objectName;
                    Poolable poolable = CreateNewObject().GetComponent<Poolable>();
                    poolable.pool.Release(poolable.gameObject);
                }
            }

            Debug.Log("[PoolManager] Ready to Pool");
            IsReady = true;
        }

        private void OnDestroyPoolObject(GameObject obj)
        {
            Destroy(obj);
        }

        private void OnReleasePoolObject(GameObject obj)
        {
            obj.SetActive(false);
        }

        private void OnGetPoolObject(GameObject obj)
        {
            obj.SetActive(true);
        }

        private GameObject CreateNewObject()
        {
            GameObject newObject = Instantiate(gameObjectDictionary[objectName]);
            newObject.GetComponent<Poolable>().pool = objectPoolDictionary[objectName];
            return newObject;
        }

        public GameObject Spawn(string name)
        {
            objectName = name;
            if (!gameObjectDictionary.ContainsKey(name))
            {
                Debug.LogFormat("{0} : Key not found in Pool.", name);
                return null;
            }

            return objectPoolDictionary[name].Get();
        }
    }
}