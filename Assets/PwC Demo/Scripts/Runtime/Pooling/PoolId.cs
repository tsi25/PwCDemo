using UnityEngine;

namespace PWCDemo.Pooling
{
    /// <summary>
    /// ID used to associated spawned prefabs with a particular object pool
    /// </summary>
    public class PoolId : MonoBehaviour
    {
        public int Id { get; set; } = -1;
    }
}