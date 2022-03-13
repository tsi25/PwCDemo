using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace PWCDemo.Teleportation
{
    public class TeleportationManager : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _teleportationAction = null;
        [SerializeField]
        private XRRayInteractor _interactor = null;
        [SerializeField]
        private TeleportationProvider _provider = null;

        private void OnTeleportationActionCanceled(InputAction.CallbackContext context)
        {
            if (_interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                TeleportRequest request = new TeleportRequest() { destinationPosition = hit.point };
                _provider.QueueTeleportRequest(request);
            }
        }

        private void Start()
        {
            _teleportationAction.action.canceled += OnTeleportationActionCanceled;
        }
    }
}