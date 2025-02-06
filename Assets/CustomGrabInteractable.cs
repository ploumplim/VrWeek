using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CustomGrabInteractable : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [SerializeField]
    private XRInputModalityManager handManager;
}
