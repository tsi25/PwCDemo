using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PWCDemo.Pooling;

namespace PWCDemo.Particles
{
    public class ParticlePlayer : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _particleRoot = null;
        [SerializeField]
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

