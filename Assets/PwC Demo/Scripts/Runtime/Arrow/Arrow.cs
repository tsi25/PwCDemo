using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using PWCDemo.Pooling;

namespace PWCDemo
{
    public class Arrow : MonoBehaviour
    {
        public Action OnImpactEvent { get; set; } = delegate { };

        [field: SerializeField]
        public XRGrabInteractable Interactable { get; private set; } = null;
        [field: SerializeField]
        public Rigidbody Rigidbody { get; private set; } = null;

        [field: SerializeField]
        public bool InFlight { get; private set; } = false;

        [field: SerializeField]
        public TrailRenderer TrailRenderer { get; set; } = null;

        [SerializeField]
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
            Interactable.enabled = true;
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
            TrailRenderer.gameObject.SetActive(inFlight);
        }

        /// <summary>
        /// Handles locking and unlocking rigidbody physics simulation based on the given locked parameter
        /// </summary>
        /// <param name="locked">Whether or not physics simulation should be locked</param>
        public void SetRigidbodyLocked(bool locked)
        {
            Rigidbody.useGravity = !locked;
            Rigidbody.isKinematic = locked;
        }

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
                _flightRotation.SetLookRotation(Rigidbody.velocity);

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
                Rigidbody.velocity = Vector3.zero;
                OnImpactEvent();

                transform.SetPositionAndRotation(_positionLastFrame, _rotationLastFrame);
                Cleanup(_despawnDelay);
            }
        }
    }
}