using System.Collections.Generic;
using UnityEngine;
using PWCDemo.Pooling;

namespace PWCDemo
{
    /// <summary>
    /// Convenience which handles spawning <see cref="Target"/>s in a configurable area
    /// </summary>
    public class TargetSpawnVolume : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Target"/> prefab to be spawned within this <see cref="TargetSpawnVolume"/>
        /// </summary>
        [SerializeField, Tooltip("The target prefab to be spawned within this volume")]
        private Target _targetPrefab = null;
        /// <summary>
        /// Max number of <see cref="Target"/>s that can be spawned at once
        /// </summary>
        [SerializeField, Tooltip("Max number of targets that can be spawned at once")]
        private int _targetCap = 5;
        /// <summary>
        /// <see cref="Color"/> the preview spawn area gizmo should render as when selected
        /// </summary>
        [SerializeField, Tooltip("Color the preview spawn area gizmo should render as when selected")]
        private Color _gizmoColor = new Color(1, 0, 0, 0.5f);

        private List<Target> _spawnedTargets = new List<Target>();

        /// <summary>
        /// Handles initializing a new <see cref="Target"/> within the configured volume
        /// </summary>
        /// <returns>A new <see cref="Target"/></returns>
        public Target GenerateTarget()
        {
            if (_spawnedTargets.Count >= _targetCap) return null;

            Target newTarget = PoolManager.Request(_targetPrefab, GetRandomPosition(), Quaternion.identity);
            newTarget.transform.SetParent(transform, true);

            newTarget.Initialize();
            newTarget.OnTargetCleanupEvent += CleanupTarget;
            _spawnedTargets.Add(newTarget);

            return newTarget;
        }

        /// <summary>
        /// Handles cleaning up all <see cref="Target"/>s tracked by this spawn bolume
        /// </summary>
        public void CleanupTargets()
        {
            for(int i = _spawnedTargets.Count - 1; i >= 0; i--)
            {
                _spawnedTargets[i].Cleanup();
            }
        }

        /// <summary>
        /// Handles cleaning up the given <see cref="Target"/>
        /// </summary>
        /// <param name="target">The <see cref="Target"/> to clean up</param>
        private void CleanupTarget(Target target)
        {
            target.OnTargetCleanupEvent -= CleanupTarget;
            _spawnedTargets.Remove(target);
        }

        /// <summary>
        /// Gets a random position within the spawn volume's bounds
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRandomPosition()
        {
            return transform.position + new Vector3(
                (Random.value - 0.5f) * transform.localScale.x,
                (Random.value - 0.5f) * transform.localScale.y,
                (Random.value - 0.5f) * transform.localScale.z
             );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawCube(transform.position, transform.localScale);
        }

#if UNITY_EDITOR
        [ContextMenu("Generate Target")]
        private void EditorGenerateTarget()
        {
            GenerateTarget();
        }
#endif
    }
}