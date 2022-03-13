using System.Collections.Generic;
using UnityEngine;


namespace PWCDemo.Pooling
{
    /// <summary>
    /// Class which handles object pooling generically
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        private static Dictionary<int, Queue<GameObject>> _objectPools = new Dictionary<int, Queue<GameObject>>();

        private static PoolManager _instance = null;
        /// <summary>
        /// Global instance of the <see cref="PoolManager"/> singleton
        /// </summary>
        public static PoolManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<PoolManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("PoolManager").AddComponent<PoolManager>();
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Handles requesting the given prefab from the pool manager as a <see cref="GameObject"/>. 
        /// Will create a new instance if none are available.
        /// </summary>
        /// <param name="prefab">The prefab instance we are requesting from the pool</param>
        /// <returns>An instance of the given prefab</returns>
        public static GameObject Request(GameObject prefab)
        {
            GameObject instance = null;
            int id = prefab.GetInstanceID();

            if(!_objectPools.ContainsKey(id))
            {
                _objectPools.Add(id, new Queue<GameObject>());
            }

            if(_objectPools[id].Count > 0)
            {
                //A prefab instance is available, activate and return it
                instance = _objectPools[id].Dequeue();
                instance.transform.SetParent(null);
                instance.SetActive(true);
            }
            else
            {
                //No prefab instance is available, create a new one and initialize its PoolId
                instance = Instantiate(prefab);
                PoolId poolId = instance.AddComponent<PoolId>();
                poolId.Id = id;
            }

            return instance;
        }

        /// <summary>
        /// Handles requesting the given prefab from the pool manager as the given type. 
        /// Will create a new instance if none are available.
        /// </summary>
        /// <typeparam name="T">The type of object we want back from the pool</typeparam>
        /// <param name="prefab">The prefab instance we are requesting from the pool</param>
        /// <returns>An instance of the given prefab</returns>
        public static T Request<T>(T prefab) where T : Component
        {
            T instance = Request(prefab.gameObject).GetComponent<T>();
            return instance;
        }

        /// <summary>
        /// Handles requesting the given prefab from the pool manager as the given type. 
        /// Will create a new instance if none are available.
        /// </summary>
        /// <typeparam name="T">The type of object we want back from the pool</typeparam>
        /// <param name="prefab">The prefab instance we are requesting from the pool</param>
        /// <param name="position">The position to initialize the requested prefab at representated as a <see cref="Vector3"/></param>
        /// <param name="rotation">The rotation to initialize the requested prefab at represented as a <see cref="Quaternion"/></param>
        /// <returns>An instance of the given prefab</returns>
        public static T Request<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            var instance = Request(prefab);

            instance.transform.SetPositionAndRotation(position, rotation);

            return instance;
        }

        /// <summary>
        /// Handles requesting the given prefab from the pool manager as the given type. 
        /// Will create a new instance if none are available.
        /// </summary>
        /// <typeparam name="T">The type of object we want back from the pool</typeparam>
        /// <param name="prefab">The prefab instance we are requesting from the pool</param>
        /// <param name="parent">The <see cref="Transform"/> to parent the requested prefab to</param>
        /// <returns>An instance of the given prefab</returns>
        public static T Request<T>(T prefab, Transform parent) where T : Component
        {
            var instance = Request(prefab);

            instance.transform.SetParent(parent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            return instance;
        }

        /// <summary>
        /// Handles recycling the given object instance
        /// </summary>
        /// <param name="instance">The instance to be recycled</param>
        public static void Recycle(GameObject instance)
        {
            PoolId poolId = instance.GetComponent<PoolId>();
            if(instance == null)
            {
                Debug.LogError("Could not recycle object, not part of any active pool");
                return;
            }

            if(!_objectPools.ContainsKey(poolId.Id))
            {
                Debug.LogWarning("No pool exists for object, but it has an active pool id. Creating new pool and recycling...");
                _objectPools.Add(poolId.Id, new Queue<GameObject>());
            }

            _objectPools[poolId.Id].Enqueue(instance);
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(Instance.transform);
        }

        /// <summary>
        /// Handles recycling the given object instance
        /// </summary>
        /// <typeparam name="T">The type of object we want to return to the pool</typeparam>
        /// <param name="instance">The instance to be recycled</param>
        public static void Recycle<T>(T instance) where T : Component
        {
            Recycle(instance.gameObject);
        }
    }
}
