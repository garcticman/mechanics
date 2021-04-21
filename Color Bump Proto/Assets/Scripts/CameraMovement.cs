using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    //TODO targer
    Vector3 target;
    [SerializeField]
    float speed = 1;

    Vector3 startPosition;

    void FixedUpdate()
    {
        moveForward();
    }

    void moveForward()
    {
        transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed, Space.World);
    }
}
