using System;
using UnityEngine;

namespace PWCDemo
{
    [Serializable]
    public class RoundData
    {
        /// <summary>
        /// Delay before the round begins
        /// </summary>
        [field: SerializeField, Tooltip("Delay before the round begins")]
        public float CountdownDelay { get; set; } = 5f;
        /// <summary>
        /// Round duration in seconds
        /// </summary>
        [field: SerializeField, Tooltip("Round duration in seconds")]
        public float Duration { get; set; } = 60f;
        /// <summary>
        /// Vector2 representing the min and max spawn frequency
        /// </summary>
        [field: SerializeField, Tooltip("Vector2 representing the min and max spawn frequency")]
        public Vector2 SpawnFrequency { get; set; } = Vector2.one;
        /// <summary>
        /// The curve along which spawning frequency should be evaluated over the duration of the round
        /// </summary>
        [field: SerializeField, Tooltip("The curve along which spawning frequency should be evaluated over the duration of the round")]
        public AnimationCurve SpawnCurve { get; set; } = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
    }
}