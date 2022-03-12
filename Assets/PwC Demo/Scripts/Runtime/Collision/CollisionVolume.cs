using System;
using UnityEngine;

namespace PWCDemo
{
    [RequireComponent(typeof(Collider))]
    public class CollisionVolume : MonoBehaviour
    {
        /// <summary>
        /// Fires when a collider enters the <see cref="CollisionVolume"/>
        /// </summary>
        public virtual Action<GameObject> OnVolumeEnter { get; set; } = delegate { };
        /// <summary>
        /// Fires when a collider exits the <see cref="CollisionVolume"/>
        /// </summary>
        public virtual Action<GameObject> OnVolumeExit { get; set; } = delegate { };

        protected void OnTriggerEnter(Collider other)
        {
            OnVolumeEnter(other.gameObject);
        }

        protected void OnTriggerExit(Collider other)
        {
            OnVolumeExit(other.gameObject);
        }
    }
}

