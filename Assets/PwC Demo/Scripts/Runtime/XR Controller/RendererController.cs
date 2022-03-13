using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace PWCDemo
{
    /// <summary>
    /// Convenience class which handles controlling the rendered behavior of the user's <see cref="XRRayInteractor"/>
    /// </summary>
    public class RendererController : MonoBehaviour
    {
        /// <summary>
        /// Reference to the managed <see cref="XRRayInteractor"/>
        /// </summary>
        [SerializeField, Tooltip("Reference to the managed ray interactor")]
        private XRRayInteractor _interactor = null;
        /// <summary>
        /// <see cref="Renderer"/> used by the <see cref="XRRayInteractor"/>
        /// </summary>
        [SerializeField, Tooltip("Line renderer used by the ray interactor")]
        private Renderer _renderer = null;
        /// <summary>
        /// Reference to the grip <see cref="InputActionReference"/>
        /// </summary>
        [SerializeField, Tooltip("Reference to the grip action")]
        private InputActionReference _gripAction = null;
        /// <summary>
        /// Reference to the trigger <see cref="InputActionReference"/>
        /// </summary>
        [SerializeField, Tooltip("Reference to the trigger action")]
        private InputActionReference _triggerAction = null;

        private bool _gripPressed = false;
        private bool _triggerPressed = false;

        /// <summary>
        /// Handles updating the rendering behavior of the managed <see cref="XRRayInteractor"/> and its <see cref="Renderer"/>
        /// based on the current state of the grip and trigger being pressed
        /// </summary>
        public void UpdateRenderer()
        {
            _interactor.lineType = _triggerPressed ? XRRayInteractor.LineType.ProjectileCurve : XRRayInteractor.LineType.StraightLine;
            _renderer.forceRenderingOff = (!_triggerPressed && _gripPressed);
        }

        /// <summary>
        /// Handles processing the grip being pressed and released 
        /// </summary>
        /// <param name="pressed">Whether or not the grip is pressed or released</param>
        private void OnGripAction(bool pressed)
        {
            _gripPressed = pressed;
            UpdateRenderer();
        }

        /// <summary>
        /// Handles processing the trigger being pressed and released 
        /// </summary>
        /// <param name="pressed">Whether or not the trigger is pressed or released</param>
        private void OnTriggerAction(bool pressed)
        {
            _triggerPressed = pressed;
            UpdateRenderer();
        }

        public void Awake()
        {
            _gripAction.action.started += (x) => { OnGripAction(true); };
            _gripAction.action.canceled += (x) => { OnGripAction(false); };

            _triggerAction.action.started += (x) => { OnTriggerAction(true); };
            _triggerAction.action.canceled += (x) => { OnTriggerAction(false); };
        }
    }
}
