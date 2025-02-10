using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

[RequireComponent(typeof(XRGeneralGrabTransformer))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(MeshCollider))]
public class ObjectSnapping : MonoBehaviour
{
    public InputActionProperty snapButton; // Bouton pour snapper l'objet
    public LayerMask wallLayer;            // Couches détectées (mettre "Wall")
    public Transform controllerTransform;  // Transform du contrôleur VR
    public GameObject bigObjectPrefab;     // Prefab de la version big de l'objet
    public List<GameObject> objectsToTrack; // Liste des objets à suivre
    public Boolean isBigObject;

    private XRGrabInteractable grabInteractable;
    private XRGeneralGrabTransformer grabTransformer;

    private Rigidbody rb;
    private Collider col;
    private Dictionary<GameObject, Quaternion> initialRotations; // Dictionnaire pour sauvegarder les rotations initiales

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Écoute quand l'objet est pris ou lâché
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabTransformer = GetComponent<XRGeneralGrabTransformer>();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        grabInteractable.farAttachMode = InteractableFarAttachMode.Near;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        if (GetComponent<MeshCollider>())
        {
            GetComponent<MeshCollider>().convex = true;
        }

        // Initialiser le dictionnaire et sauvegarder les rotations initiales
        initialRotations = new Dictionary<GameObject, Quaternion>();
        foreach (var obj in objectsToTrack)
        {
            initialRotations[obj] = obj.transform.rotation;
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("OnGrab called");

        if (isBigObject)
        {
            Debug.Log("isBigObject is true in OnGrab");
            gameObject.transform.localScale = new Vector3(10, 10, 10);
        }
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Verrouille la rotation
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // Définit la rotation à -90°
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("OnRelease called");

        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        Debug.DrawRay(controllerTransform.position, controllerTransform.forward * 100f, Color.red, 2f); // Visualiser le raycast dans la scène

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            bigObjectPrefab.transform.position = RoundVar(hit.point);
            bigObjectPrefab.transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // Set the rotation to -90, 0, 0

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.None; // Déverrouille la rotation

            if (!isBigObject)
            {
                rb.gameObject.SetActive(false);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(100, 100, 100);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any object in wallLayer");
        }
    }

    public Vector3 RoundVar(Vector3 vectorToRound)
    {
        return new Vector3(Mathf.Round(vectorToRound.x), Mathf.Round(vectorToRound.y), Mathf.Round(vectorToRound.z));
    }

    public Quaternion RoundAngle(Vector3 angle)
    {
        float x = Mathf.Round(angle.x / 90.0f) * 90.0f;
        float y = Mathf.Round(angle.y / 90.0f) * 90.0f;
        float z = Mathf.Round(angle.z / 90.0f) * 90.0f;
        return Quaternion.Euler(x, y, z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controllerTransform.position, controllerTransform.position + controllerTransform.forward * 100);
    }
}