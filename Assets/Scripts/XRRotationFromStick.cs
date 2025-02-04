using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.InputSystem;

public class XRRotationFromStick : XRBaseGrabTransformer
{
    [SerializeField] private InputActionReference m_RotationAction;
    [SerializeField] private InputActionReference m_RotationAction2;
    [SerializeField]
    [Tooltip("Defines which rotation axes are allowed when an object is grabbed. Axes not selected will maintain their initial rotation.")]
    XRGeneralGrabTransformer.ManipulationAxes m_PermittedRotationAxis = XRGeneralGrabTransformer.ManipulationAxes.All;

    /// <inheritdoc />
    protected override RegistrationMode registrationMode => RegistrationMode.SingleAndMultiple;

    Vector3 m_InitialEulerRotation;
    Vector3 m_PreviousInput;

    /// <inheritdoc />
    public override void OnLink(XRGrabInteractable grabInteractable)
    {
        base.OnLink(grabInteractable);
        m_InitialEulerRotation = grabInteractable.transform.rotation.eulerAngles;
        m_PreviousInput = Vector3.zero;
    }

    /// <inheritdoc />
    public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
    {
        Vector2 input = m_RotationAction.action.ReadValue<Vector2>();

        // Activer l'action avant de lire sa valeur
        if (!m_RotationAction2.action.enabled)
        {
            m_RotationAction2.action.Enable();
        }

        float input2 = m_RotationAction2.action.ReadValue<float>();

        Debug.Log(input2);

        int stickX = Mathf.Abs(input.x) > 0.8f ? (int)Mathf.Sign(input.x) : 0;
        int stickY = Mathf.Abs(input.y) > 0.8f ? (int)Mathf.Sign(input.y) : 0;
        int stickZ = Mathf.Abs(input2) > 0.8f ? (int)Mathf.Sign(input2) : 0;

        Vector3 newRotationEuler = targetPose.rotation.eulerAngles;

        if (stickX != 0 && m_PreviousInput.x == 0)
        {
            newRotationEuler.x += stickX * 45f;
        }

        if (stickY != 0 && m_PreviousInput.y == 0)
        {
            newRotationEuler.y += stickY * 45f;
        }

        if (stickZ != 0 && m_PreviousInput.z == 0)
        {
            newRotationEuler.z += stickZ * 45f;
        }

        m_PreviousInput = new Vector3(stickX, stickY, stickZ);

        if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.X) == 0)
            newRotationEuler.x = m_InitialEulerRotation.x;

        if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.Y) == 0)
            newRotationEuler.y = m_InitialEulerRotation.y;

        if ((m_PermittedRotationAxis & XRGeneralGrabTransformer.ManipulationAxes.Z) == 0)
            newRotationEuler.z = m_InitialEulerRotation.z;

        targetPose.rotation = Quaternion.Euler(newRotationEuler);
    }
}