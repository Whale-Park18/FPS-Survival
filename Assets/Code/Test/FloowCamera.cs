using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloowCamera : MonoBehaviour
{
    public Transform target;
    public float offsetY;

    private void LateUpdate()
    {
        var newPosition = target.position;
        newPosition.y += offsetY;

        transform.position = newPosition;
    }
}
