using System;
using UnityEngine;

namespace PWCDemo
{
    /// <summary>
    /// Class which handles detecting <see cref="ArrowNock"/>s
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CollisionVolume : MonoBehaviour
    {
        /// <summary>
        /// Fires when a collider enters the <see cref="CollisionVolume"/>
        /// </summary>
        public Action<Arrow> OnVolumeEnter { get; set; } = delegate { };
        /// <summary>
        /// Fires when a collider exits the <see cref="CollisionVolume"/>
        /// </summary>
        public Action<Arrow> OnVolumeExit { get; set; } = delegate { };

        private void OnTriggerEnter(Collider other)
        {
            ArrowNock nock = other.GetComponent<ArrowNock>();
            if(nock) OnVolumeEnter(nock.CachedArrow);
        }

        private void OnTriggerExit(Collider other)
        {
            ArrowNock nock = other.GetComponent<ArrowNock>();
            if (nock) OnVolumeExit(nock.CachedArrow);
        }
    }
}

