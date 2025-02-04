using UnityEngine;

public class GridPlacement : MonoBehaviour
{

    private static Vector3 Floor(Vector3 Vec)
    {
        return new Vector3(Mathf.Floor(Vec.x), Mathf.Floor(Vec.y), Mathf.Floor(Vec.z));
    }
    
    private void Update()
    {
        Vector3 Position = transform.position;
        transform.position = Floor(Position);
    }
}