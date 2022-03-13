using UnityEngine;
using PWCDemo.Pooling;

namespace PWCDemo
{
    public class ArrowSpawnVolume : MonoBehaviour
    {
        [SerializeField]
        private Arrow _arrowPrefab = null;
        [SerializeField]
        private Transform _spawnTransform = null;

        private Arrow _currentArrow = null;

        public Arrow SpawnArrow()
        {
            Arrow newArrow = PoolManager.Request(_arrowPrefab, _spawnTransform.position, _spawnTransform.rotation);
            newArrow.Initialize();
            return newArrow;
        }

        private void OnTriggerExit(Collider other)
        {
            Arrow arrow = other.GetComponent<Arrow>();
            if(arrow == _currentArrow)
            {
                _currentArrow = SpawnArrow();
            }
        }


        private void Awake()
        {
            SpawnArrow();
        }
    }
}
