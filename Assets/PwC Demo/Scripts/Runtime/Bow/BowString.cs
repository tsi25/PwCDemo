using UnityEngine;

namespace PWCDemo
{
    /// <summary>
    /// Class which manages rendering the bow string
    /// </summary>
    public class BowString : MonoBehaviour
    {
        /// <summary>
        /// Reference to the <see cref="LineRenderer"/> being used
        /// </summary>
        [SerializeField, Tooltip("Reference to the line renderer being used")]
        private LineRenderer _lineRenderer = null;
        /// <summary>
        /// The points along which the <see cref="BowString"/> should be strung
        /// </summary>
        [SerializeField, Tooltip("The points along which the bow string should be strung")]
        private Transform[] _points = new Transform[0];

        /// <summary>
        /// Handles updating the bowstring points
        /// </summary>
        public void UpdateBowstringPoints()
        {
            for(int i = 0; i < _points.Length; i++)
            {
                _lineRenderer.SetPosition(i, _points[i].position);
            }
        }

        private void Update()
        {
            UpdateBowstringPoints();
        }
    }
}
