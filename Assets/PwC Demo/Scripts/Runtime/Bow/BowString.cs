using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWCDemo
{
    public class BowString : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer _lineRenderer = null;
        [SerializeField]
        private Transform[] _points = new Transform[0];

        private Vector3[] _positions = new Vector3[0];

        public void UpdateBowstringPoints()
        {
            for(int i = 0; i < _points.Length; i++)
            {
                _positions[i] = _points[i].position;
            }

            _lineRenderer.SetPositions(_positions);
        }

        private void Update()
        {
            UpdateBowstringPoints();
        }

        private void Awake()
        {
            _positions = new Vector3[_points.Length];
        }
    }
}
