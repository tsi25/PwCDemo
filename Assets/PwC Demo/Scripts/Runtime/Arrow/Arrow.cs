using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
        private TrailRenderer TrailRenderer { get; set; } = null;

        private Quaternion _flightRotation = Quaternion.identity;

        public void Initialize()
        {

        }

        public void SetInFlight(bool inFlight)
        {
            InFlight = inFlight;
            TrailRenderer.gameObject.SetActive(inFlight);
        }

        private void Update()
        {
            if(InFlight)
            {
                _flightRotation.SetLookRotation(Rigidbody.velocity);
                transform.rotation = _flightRotation;
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if(InFlight)
            {
                SetInFlight(false);
                Rigidbody.isKinematic = true;
                Rigidbody.useGravity = false;
                Rigidbody.velocity = Vector3.zero;
                OnImpactEvent();
            }
        }
    }
}