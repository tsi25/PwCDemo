using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using PWCDemo.Audio;
using PWCDemo.Pooling;


namespace PWCDemo
{
    /// <summary>
    /// Class which manages the behavior of the bow
    /// </summary>
    public class Bow : MonoBehaviour
    {
        /// <summary>
        /// Static event that fires any time a nocked <see cref="Arrow"/> is released
        /// </summary>
        public static Action OnArrowReleaseEvent { get; set; } = delegate { };

        /// <summary>
        /// Reference to the <see cref="Trajectory"/> preview component
        /// </summary>
        [Header("General")]
        [SerializeField, Tooltip("Reference to the trajectory preview component")]
        private Trajectory _trajectoryPreview = null;
        /// <summary>
        /// Reference to the <see cref="CollisionVolume"/> that detects <see cref="Arrow"/>s to be nocked
        /// </summary>
        [SerializeField, Tooltip("Reference to the collision volume that detects arrows to be nocked")]
        private CollisionVolume _volume = null;
        /// <summary>
        /// <see cref="Transform"/> under which a nocked <see cref="Arrow"/> should be reparented
        /// </summary>
        [SerializeField, Tooltip("Transform under which a nocked arrow should be reparented")]
        private Transform _arrowParent = null;
        /// <summary>
        /// <see cref="Transform"/> through which a nocked <see cref="Arrow"/> should be released
        /// </summary>
        [SerializeField, Tooltip("Transform through which a nocked arrow should be released")]
        private Transform _arrowPoint = null;
        /// <summary>
        /// Cooldown after which an new <see cref="Arrow"/> can be nocked
        /// </summary>
        [SerializeField, Tooltip("Cooldown after which an new arrow can be nocked")]
        private float _cooldown = 0.5f;

        /// <summary>
        /// <see cref="InputActionReference"/> which can trigger releasing a nocked <see cref="Arrow"/>
        /// </summary>
        [Header("Interaction")]
        [SerializeField, Tooltip("Actions which can trigger releasing a nocked arrow")]
        private InputActionReference[] _releaseActions = null;

        /// <summary>
        /// Force modifier to apply to a released arrow
        /// </summary>
        [Header("Force")]
        [SerializeField, Tooltip("Force modifier to apply to a released arrow")]
        private float _forceModifier = 1f;

        /// <summary>
        /// <see cref="AudioPlayer"/> prefab to be spawned when playing sound effects
        /// </summary>
        [Header("Audio")]
        [SerializeField, Tooltip("Audio player prefab to be spawned when playing sound effects")]
        private AudioPlayer _audioPlayerPrefab = null;
        /// <summary>
        /// Sound to be played when an arrow is released
        /// </summary>
        [SerializeField, Tooltip("Sound to be played when an arrow is released")]
        private AudioClip _releaseSound = null;

        private Transform _cachedHandPosition = null;
        private XRInteractionManager _cachedInteractionManager = null;
        private Arrow _currentArrow = null;
        private Vector3 _defaultArrowParentPosition = Vector3.zero;

        /// <summary>
        /// Reference to the cached <see cref="XRInteractionManager"/>
        /// </summary>
        public XRInteractionManager CachedInteractionManager
        {
            get { return _cachedInteractionManager ?? (_cachedInteractionManager = FindObjectOfType<XRInteractionManager>()); }
        }

        /// <summary>
        /// Handles loading the given <see cref="Arrow"/>
        /// </summary>
        /// <param name="arrow">The <see cref="Arrow"/> to load</param>
        public void NockArrow(Arrow arrow)
        {
            //dont want to nock an arrow if one is already nocked
            if (_currentArrow != null) return;

            //if nothing is interacting with this arrow we don't actually want to load it
            if (arrow.Interactable.interactorsSelecting.Count == 0) return;

            _cachedHandPosition = arrow.Interactable.interactorsSelecting[0].transform;
            _currentArrow = arrow;

            CachedInteractionManager.CancelInteractableSelection((IXRSelectInteractable)_currentArrow.Interactable);
            _currentArrow.Interactable.enabled = false;

            _currentArrow.SetRigidbodyLocked(true);

            _currentArrow.transform.SetParent(_arrowParent);
            _currentArrow.transform.localPosition = Vector3.zero;
            _currentArrow.transform.localRotation = Quaternion.identity;

            _volume.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handles releaseing the <see cref="Arrow"/>, which removes it from the <see cref="Bow"/>s control and fires it forward
        /// </summary>
        public async Task ReleaseArrow()
        {
            //only want to release an arrow if there's an arrow to release
            if (_currentArrow == null) return;
            OnArrowReleaseEvent();
            PoolManager.Request(_audioPlayerPrefab, transform.position, Quaternion.identity).Play(_releaseSound);

            _currentArrow.transform.SetParent(null);
            _arrowParent.localPosition = _defaultArrowParentPosition;

            _currentArrow.SetRigidbodyLocked(false);
            _currentArrow.CachedRigidbody.AddForce(GetAppliedForce(), ForceMode.Impulse);

            _currentArrow.SetInFlight(true);
            _currentArrow = null;
            _trajectoryPreview.Clear();

            float elapsedTime = 0f;
            while(elapsedTime < _cooldown)
            {
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            _volume.gameObject.SetActive(true);
        }

        /// <summary>
        /// Calculates and returns the force applied to the nocked <see cref="Arrow"/> as a factor of
        /// the current draw distance and the configured force modifier
        /// </summary>
        /// <returns>The current applied force</returns>
        private Vector3 GetAppliedForce()
        {
            if (_currentArrow == null) return Vector3.zero;
            return _currentArrow.transform.forward * _forceModifier * Vector3.Distance(_currentArrow.transform.position, _arrowPoint.position);
        }

        /// <summary>
        /// Event which handles intercepting the OnVolumeEnterEvent from the <see cref="CollisionVolume"/>
        /// </summary>
        /// <param name="arrow">The <see cref="Arrow"/> which is to be nocked</param>
        private void OnVolumeEnter(Arrow arrow)
        {
            NockArrow(arrow);
        }

        /// <summary>
        /// Handles the release action interaction from the player
        /// </summary>
        /// <param name="context"></param>
        private void OnReleasePerformed(InputAction.CallbackContext context)
        {
            _ = ReleaseArrow();
        }

        private void Update()
        {
            if(_currentArrow)
            {
                _currentArrow.transform.LookAt(_arrowPoint);
                _arrowParent.position = _cachedHandPosition.transform.position;

                _trajectoryPreview.SimulateTrajectory(
                    _currentArrow.transform.position,
                    _currentArrow.transform.rotation, 
                    GetAppliedForce());
            }
        }

        private void Awake()
        {
            _defaultArrowParentPosition = _arrowParent.localPosition;
            _volume.OnVolumeEnter += OnVolumeEnter;

            foreach (var releaseAction in _releaseActions)
            {
                releaseAction.action.canceled += OnReleasePerformed;
            }
        }
    }
}