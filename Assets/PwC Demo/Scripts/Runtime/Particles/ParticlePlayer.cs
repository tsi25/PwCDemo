using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PWCDemo.Pooling;

namespace PWCDemo.Particles
{
    /// <summary>
    /// Convenience class which handles <see cref="ParticleSystem"/> playback
    /// </summary>
    public class ParticlePlayer : MonoBehaviour
    {
        /// <summary>
        /// The root of the managed <see cref="ParticleSystem"/>
        /// </summary>
        [SerializeField, Tooltip("The root of the managed particle system")]
        private ParticleSystem _particleRoot = null;
        /// <summary>
        /// The delay after which the <see cref="ParticleSystem"/> should be despawned
        /// </summary>
        [SerializeField, Tooltip("The delay after which the particle system should be despawned")]
        private float _despawnDelay = 1f;

        /// <summary>
        /// Handles playing the configured <see cref="ParticleSystem"/>
        /// </summary>
        public void Play()
        {
            _particleRoot.Play();
            Invoke("Cleanup", _despawnDelay);
        }

        /// <summary>
        /// Handles cleaning up this Audioplayer
        /// </summary>
        public void Cleanup()
        {
            PoolManager.Recycle(gameObject);
        }
    }
}

