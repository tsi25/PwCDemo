using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;


namespace PWCDemo
{
    public class Bow : MonoBehaviour
    {
        [SerializeField]
        private CollisionVolume _volume = null;
        [SerializeField]
        private Transform _arrowParent = null;
        [SerializeField]
        private Transform _arrowPoint = null;
        [SerializeField]
        private float _cooldownDuration = 0.5f;

        [Header("Interaction")]
        [SerializeField]
        private InputActionReference[] _releaseActions = null;

        [Header("Force")]
        [SerializeField]
        private float _forceModifier = 1f;

        private Transform _cachedHandPosition = null;
        private XRInteractionManager _cachedInteractionManager = null;
        private Arrow _currentArrow = null;

        public XRInteractionManager CachedInteractionManager
        {
            get { return _cachedInteractionManager ?? (_cachedInteractionManager = FindObjectOfType<XRInteractionManager>()); }
        }

        public void LoadArrow(Arrow arrow)
        {
            //if nothing is interacting with this arrow we don't actually want to load it
            if (arrow.Interactable.interactorsSelecting.Count == 0) return;

            _cachedHandPosition = arrow.Interactable.interactorsSelecting[0].transform;
            _currentArrow = arrow;

            CachedInteractionManager.CancelInteractableSelection((IXRSelectInteractable)_currentArrow.Interactable);
            _currentArrow.Interactable.enabled = false;

            _currentArrow.Rigidbody.isKinematic = true;
            _currentArrow.Rigidbody.useGravity = false;

            _currentArrow.transform.SetParent(_arrowParent);

            _currentArrow.transform.localPosition = Vector3.zero;
            _currentArrow.transform.localRotation = Quaternion.identity;

            _volume.gameObject.SetActive(false);
        }


        public async Task ReleaseArrow()
        {
            //only want to release an arrow if there's an arrow to release
            if (_currentArrow == null) return;

            Vector3 force = _currentArrow.transform.forward * _forceModifier * Vector3.Distance(_currentArrow.transform.position, _arrowPoint.position);

            _currentArrow.Rigidbody.isKinematic = false;
            _currentArrow.Rigidbody.useGravity = true;
            
            _currentArrow.transform.SetParent(null);

            _currentArrow.Rigidbody.AddForce(force, ForceMode.Impulse);
            _currentArrow.SetInFlight(true);
            _currentArrow = null;

            float elapsedTime = 0f;
            while(elapsedTime < _cooldownDuration)
            {
                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }

            _volume.gameObject.SetActive(true);
        }


        private void OnVolumeEnter(GameObject other)
        {
            if (_currentArrow) return;

            Arrow arrow = other.GetComponent<Arrow>();
            if (arrow) LoadArrow(arrow);
        }

        private void OnReleasePerformed(InputAction.CallbackContext context)
        {
            float grip = context.action.ReadValue<float>();
            if (grip < 1f) _ = ReleaseArrow();
        }

        private void Update()
        {
            if(_currentArrow)
            {
                _currentArrow.transform.LookAt(_arrowPoint);
                _arrowParent.position = _cachedHandPosition.transform.position;
            }
        }

        private void Awake()
        {
            _volume.OnVolumeEnter += OnVolumeEnter;

            foreach (var releaseAction in _releaseActions)
            {
                releaseAction.action.performed += OnReleasePerformed;
            }
        }
    }
}