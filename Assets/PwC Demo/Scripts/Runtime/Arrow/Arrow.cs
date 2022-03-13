using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using PWCDemo.Pooling;

namespace PWCDemo
{
    /// <summary>
    /// <see cref="Arrow"/> class responsible for managing <see cref="Arrow"/> behavior
    /// </summary>
    public class Arrow : MonoBehaviour
    {
        /// <summary>
        /// Event which fires when the <see cref="Arrow"/> impacts something after being released
        /// </summary>
        public Action OnImpactEvent { get; set; } = delegate { };

        /// <summary>
        /// Reference to the <see cref="XRGrabInteractable"/> component
        /// </summary>
        [field: SerializeField, Tooltip("Reference to the interactable component")]
        public XRGrabInteractable Interactable { get; private set; } = null;
        /// <summary>
        /// Reference to the <see cref="Arrow"/>'s <see cref="Rigidbody"/>
        /// </summary>
        [field: SerializeField, Tooltip("Reference to the arrow's rigidbody")]
        public Rigidbody CachedRigidbody { get; private set; } = null;
        /// <summary>
        /// Boolean describing whether or not the <see cref="Arrow"/> is currently in flight
        /// </summary>
        [field: SerializeField, Tooltip("Boolean describign whether or not the arrow is currently in flight")]
        public bool InFlight { get; private set; } = false;
        /// <summary>
        /// Reference to the <see cref="Arrow"/>'s <see cref="TrailRenderer"/>
        /// </summary>
        [field: SerializeField, Tooltip("Reference to the arrow's trail renderer")]
        public TrailRenderer CachedTrailRenderer { get; set; } = null;

        /// <summary>
        /// Delay in seconds after which the arrow will be despawned once it's hit a surface
        /// </summary>
        [SerializeField, Tooltip("Delay in seconds after which the arrow will be despawned once it's hit a surface")]
        private float _despawnDelay = 10f;

        private Quaternion _rotationLastFrame = Quaternion.identity;
        private Vector3 _positionLastFrame = Vector3.zero;
        private Quaternion _flightRotation = Quaternion.identity;
        private Coroutine _cleanupCoroutine = null;

        /// <summary>
        /// Handles initializing the <see cref="Arrow"/>, primarily resetting physics and reenabling interactions
        /// </summary>
        public void Initialize()
        {
            SetRigidbodyLocked(false);
            if(Interactable) Interactable.enabled = true;
        }

        /// <summary>
        /// Cleans up the <see cref="Arrow"/> after the given delay delay, or immediately if no delay is given
        /// </summary>
        /// <param name="delay">The delay after which the <see cref="Arrow"/> should be cleaned up</param>
        public void Cleanup(float delay = 0f)
        {
            if (_cleanupCoroutine != null) StopCoroutine(_cleanupCoroutine);
            _cleanupCoroutine = null;

            if (delay > 0f)
            {
                _cleanupCoroutine = StartCoroutine(CleanupAfterDelay(delay));
                return;
            }

            if (gameObject.activeSelf) PoolManager.Recycle(gameObject);
        }

        /// <summary>
        /// Handles setting the arrow to the in-flight state
        /// </summary>
        /// <param name="inFlight"></param>
        public void SetInFlight(bool inFlight)
        {
            InFlight = inFlight;
            if(CachedTrailRenderer) CachedTrailRenderer.gameObject.SetActive(inFlight);
        }

        /// <summary>
        /// Handles locking and unlocking rigidbody physics simulation based on the given locked parameter
        /// </summary>
        /// <param name="locked">Whether or not physics simulation should be locked</param>
        public void SetRigidbodyLocked(bool locked)
        {
            CachedRigidbody.useGravity = !locked;
            CachedRigidbody.isKinematic = locked;
        }

        /// <summary>
        /// Handles cleaning up the <see cref="Arrow"/> after the given delay
        /// </summary>
        /// <param name="delay">Amount of time in seconds after which to despawn the <see cref="Arrow"/></param>
        private IEnumerator CleanupAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _cleanupCoroutine = null;
            Cleanup();
        }

        private void Update()
        {
            if(InFlight)
            {
                _flightRotation.SetLookRotation(CachedRigidbody.velocity);

                _rotationLastFrame = transform.rotation;
                _positionLastFrame = transform.position;

                transform.rotation = _flightRotation;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(InFlight)
            {
                SetInFlight(false);
                SetRigidbodyLocked(true);
                CachedRigidbody.velocity = Vector3.zero;
                OnImpactEvent();

                transform.SetPositionAndRotation(_positionLastFrame, _rotationLastFrame);
                Cleanup(_despawnDelay);
            }
        }
    }
}