using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 offset;
    public float radius;
    public LayerMask mask;

    private void Update()
    {
        Vector3 point0 = transform.position;
        Vector3 point1 = transform.position + offset;
        Collider[] overlap = Physics.OverlapCapsule(point0, point1, radius, mask);

        foreach(Collider elelment in overlap)
        {
            print(elelment.name);
        }
    }
}
