using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWCDemo.Audio
{
    [Serializable]
    public class AudioData
    {
        [field: SerializeField]
        public Vector2 PitchRange { get; set; } = Vector2.one;

        public float RandomPitch
        {
            get { return UnityEngine.Random.Range(PitchRange.x, PitchRange.y); }
        }
    }
}
