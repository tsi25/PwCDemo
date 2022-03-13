using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace PWCDemo
{
    public class RendererController : MonoBehaviour
    {
        [SerializeField]
        private XRRayInteractor _interactor = null;
        [SerializeField]
        private Renderer _renderer = null;
        [SerializeField]
        private InputActionReference _gripAction = null;
        [SerializeField]
        private InputActionReference _triggerAction = null;

        private bool _gripPressed = false;
        private bool _triggerPressed = false;

        public void UpdateRenderer()
        {
            _interactor.lineType = _triggerPressed ? XRRayInteractor.LineType.ProjectileCurve : XRRayInteractor.LineType.StraightLine;
            _renderer.forceRenderingOff = (!_triggerPressed && _gripPressed);
        }

        private void OnGripAction(bool pressed)
        {
            _gripPressed = pressed;
            UpdateRenderer();
        }

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