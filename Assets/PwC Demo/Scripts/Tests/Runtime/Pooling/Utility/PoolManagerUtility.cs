using UnityEngine;

namespace PWCDemo.UnitTests.Pooling
{
    [CreateAssetMenu(fileName =nameof(PoolManagerUtility), menuName ="PWCDemo/UnitTests/"+ nameof(PoolManagerUtility))]
    public class PoolManagerUtility : UnitTestUtility
    {
        private static PoolManagerUtility _instance = null;

        public static PoolManagerUtility Instance
        {
            get { return _instance ? _instance : _instance = GetInstance<PoolManagerUtility>(); }
        }

        [field: SerializeField]
        public GameObject GameObjectPrefab { get; set; } = null;

        [field: SerializeField]
        public Transform ComponentPrefab { get; set; } = null;
    }
}