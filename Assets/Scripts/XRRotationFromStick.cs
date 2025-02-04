using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.InputSystem;

public class XRRotationFromStick : XRBaseGrabTransformer
{
    
    [SerializeField] private InputActionReference m_RotationAction;
    [SerializeField]
    [Tooltip("Defines which rotation axes are allowed when an object is grabbed. Axes not selected will maintain their initial rotation.")]
    XRGeneralGrabTransformer.ManipulationAxes m_PermittedRotationAxis = XRGeneralGrabTransformer.ManipulationAxes.All;

    /// <inheritdoc />
    protected override RegistrationMode registrationMode => RegistrationMode.SingleAndMultiple;

    Vector3 m_InitialEulerRotation;

    /// <inheritdoc />
    public override void OnLink(XRGrabInteractable grabInteractable)
    {
        base.OnLink(grabInteractable);
        m_InitialEulerRotation = grabInteractable.transform.rotation.eulerAngles;
    }

    /// <inheritdoc />
    public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
    {
        int stickX, stickY;
        Vector2 input = m_RotationAction.action.ReadValue<Vector2>();
        if (input.x > 0.8f)
            stickX = 1;
        else if (input.x < -0.8f)
            stickX = -1;
        else
            stickX = 0;
        
        if (input.y > 0.8f)
            stickY = 1;
        else if (input.y < -0.8f)
            stickY = -1;
        else
            stickY = 0;
        
        Vector3 newRotationEuler = targetPose.rotation.eulerAngles;
        Vector3 rot = new Vector3(stickX * 45f, stickY * 45f, 0);
        newRotationEuler += rot;

        if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.X) == 0)
            newRotationEuler.x = m_InitialEulerRotation.x;

        if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.Y) == 0)
            newRotationEuler.y = m_InitialEulerRotation.y;

        if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.Z) == 0)
            newRotationEuler.z = m_InitialEulerRotation.z;

        targetPose.rotation = Quaternion.Euler(newRotationEuler);
    }
}

