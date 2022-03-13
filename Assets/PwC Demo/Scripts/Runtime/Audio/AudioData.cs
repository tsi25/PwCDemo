using System;
using UnityEngine;

namespace PWCDemo.Audio
{
    /// <summary>
    /// Serializable class which tracks configuration data for audio sources
    /// </summary>
    [Serializable]
    public class AudioData
    {
        /// <summary>
        /// The range in which pitch can be randomly selected with x being the min pitch and y being the max pitch
        /// </summary>
        [field: SerializeField, Tooltip("The range in which pitch can be randomly selected with x being the min pitch and y being the max pitch")]
        public Vector2 PitchRange { get; set; } = Vector2.one;
        
        /// <summary>
        /// Convenience property which returns a randomized pitch from the pitch range
        /// </summary>
        public float RandomPitch
        {
            get { return UnityEngine.Random.Range(PitchRange.x, PitchRange.y); }
        }
    }
}
