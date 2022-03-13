using UnityEngine;
using PWCDemo.Pooling;

namespace PWCDemo
{
    /// <summary>
    /// Class which handles spawning <see cref="Arrow"/>s as they are fired from the <see cref="Bow"/>
    /// </summary>
    public class ArrowSpawnVolume : MonoBehaviour
    {
        /// <summary>
        /// <see cref="Arrow"/> prefab to spawn
        /// </summary>
        [SerializeField,Tooltip("Arrow prefab to spawn")]
        private Arrow _arrowPrefab = null;
        /// <summary>
        /// Position at which to spawn the <see cref="Arrow"/> prefab
        /// </summary>
        [SerializeField, Tooltip("Position at which to spawn the arrow prefab")]
        private Transform _spawnTransform = null;

        /// <summary>
        /// Handles spawning a new <see cref="Arrow"/>
        /// </summary>
        /// <returns>The spawned <see cref="Arrow"/></returns>
        public Arrow SpawnArrow()
        {
            Arrow newArrow = PoolManager.Request(_arrowPrefab, _spawnTransform.position, _spawnTransform.rotation);
            newArrow.Initialize();
            return newArrow;
        }

        /// <summary>
        /// Handles intercepting the OnArrowReleasedEvent from the <see cref="Bow"/>
        /// </summary>
        private void OnArrowReleased()
        {
            SpawnArrow();
        }


        private void Awake()
        {
            SpawnArrow();
            Bow.OnArrowReleaseEvent += OnArrowReleased;
        }
    }
}
