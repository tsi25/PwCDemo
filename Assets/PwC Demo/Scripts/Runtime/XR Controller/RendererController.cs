using UnityEngine;
using UnityEngine.InputSystem;

namespace PWCDemo
{
    public class RendererController : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer = null;
        [SerializeField]
        private InputActionReference _actionReference = null;

        public void SetRendererEnabled(bool enabled)
        {
            _renderer.forceRenderingOff = !enabled;
        }

        public void Awake()
        {
            _actionReference.action.started += (x) => { SetRendererEnabled(false); };
            _actionReference.action.canceled += (x) => { SetRendererEnabled(true); };
        }
    }
}