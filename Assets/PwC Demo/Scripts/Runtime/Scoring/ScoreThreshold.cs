using System;
using UnityEngine;

namespace PWCDemo.Scoring
{
    [Serializable]
    public class ScoreThreshold
    {
        /// <summary>
        /// The distance from the center the shot must be within to qualify for this threshold
        /// </summary>
        [field: SerializeField, Tooltip("The distance from the center the shot must be within to qualify for this threshold")]
        public float DistanceFromCenter { get; set; } = 0f;
        /// <summary>
        /// The amount of points a shot within this threshold is worth
        /// </summary>
        [field: SerializeField, Tooltip("The amount of points a shot within this threshold is worth")]
        public int PointValue { get; set; } = 0;
    }
}