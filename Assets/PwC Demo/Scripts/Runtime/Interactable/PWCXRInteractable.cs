using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace PWCDemo
{
    public class PWCXRInteractable : XRGrabInteractable
    {
        public virtual void ForceDrop(SelectExitEventArgs args)
        {
            OnSelectExited(args);
        }
    }
}

