using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ObjectSnapping : MonoBehaviour
{
    public InputActionProperty snapButton; // Bouton pour snapper l'objet
    public LayerMask wallLayer;            // Couches détectées (mettre "Wall")
    public Transform controllerTransform;  // Transform du contrôleur VR

    private XRGrabInteractable grabInteractable;
    private XRGeneralGrabTransformer grabTransformer;

    private Rigidbody rb;
    private Collider col;
    
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Écoute quand l'objet est pris ou lâché
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabTransformer = GetComponent<XRGeneralGrabTransformer>();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    
    private void OnGrab(SelectEnterEventArgs args)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, wallLayer))
        {
            
            transform.position = RoundVar(hit.point);
            transform.rotation = RoundAngle(transform.rotation.eulerAngles);
            rb.isKinematic = true;
            rb.useGravity = false;
            
        } else {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public Vector3 RoundVar(Vector3 vectorToRound)
    {
        return new Vector3(Mathf.Round(vectorToRound.x), Mathf.Round(vectorToRound.y), Mathf.Round(vectorToRound.z));
    }

    public Quaternion RoundAngle(Vector3 angle)
    {
        float x =  Mathf.Round(angle.x / 90.0f) * 90.0f;
        float y =  Mathf.Round(angle.y / 90.0f) * 90.0f;
        float z =  Mathf.Round(angle.z / 90.0f) * 90.0f;
        return Quaternion.Euler(x, y, z);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controllerTransform.position, controllerTransform.position + controllerTransform.forward * 100);
    }
}