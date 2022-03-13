using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace PWCDemo.Teleportation
{
    /// <summary>
    /// Class which handles teleport controls and input from the player
    /// </summary>
    public class TeleportationManager : MonoBehaviour
    {
        /// <summary>
        /// <see cref="InputActionReference"/> user can use to begin and end teleporting
        /// </summary>
        [SerializeField, Tooltip("Action user can use to begin and end teleporting")]
        private InputActionReference _teleportationAction = null;
        /// <summary>
        /// Reference to the <see cref="XRRayInteractor"/> used to calculate teleportation
        /// </summary>
        [SerializeField, Tooltip("Reference to the ray interactor used to calculate teleportation")]
        private XRRayInteractor _interactor = null;
        /// <summary>
        /// Reference to the <see cref="TeleportationProvider"/> which handles performing a teleport
        /// </summary>
        [SerializeField, Tooltip("Reference to the provider which handles performing a teleport")]
        private TeleportationProvider _provider = null;
        /// <summary>
        /// Reference to the reticle used to preview the resulting teleport position
        /// </summary>
        [SerializeField, Tooltip("Reference to the reticle used to preview the resulting teleport position")]
        private GameObject _teleportReticle = null; 

        private bool _isActive = false;

        /// <summary>
        /// Whether or not a teleportation is actively being calculated
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                _teleportReticle.gameObject.SetActive(_isActive);
            }
        }

        /// <summary>
        /// Handles user input which begins a teleport action
        /// </summary>
        /// <param name="context">The <see cref="InputAction.CallbackContext"/> of the configured <see cref="InputActionReference"/></param>
        private void OnTeleportationActionStarted(InputAction.CallbackContext context)
        {
            IsActive = true;
        }

        /// <summary>
        /// Handles user input which completes a teleport action
        /// </summary>
        /// <param name="context">The <see cref="InputAction.CallbackContext"/> of the configured <see cref="InputActionReference"/></param>
        private void OnTeleportationActionCanceled(InputAction.CallbackContext context)
        {
            if (_interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                TeleportRequest request = new TeleportRequest() { destinationPosition = hit.point };
                _provider.QueueTeleportRequest(request);
            }

            IsActive = false;
        }

        private void Update()
        {
            if (!_isActive) return;

            if (_interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                //render the reticle at the given point if its valid
                if (!_teleportReticle.activeSelf) _teleportReticle.SetActive(true);
                _teleportReticle.transform.position = hit.point;
            }
            else if (_teleportReticle.activeSelf)
            {
                //otherwise hide the reticle
                _teleportReticle.SetActive(false);
            }
        }

        private void Start()
        {
            _teleportationAction.action.started += OnTeleportationActionStarted;
            _teleportationAction.action.canceled += OnTeleportationActionCanceled;

            _teleportReticle.transform.SetParent(null);
        }
    }
}