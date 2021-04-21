using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Moving : MonoBehaviour
{
    [SerializeField] float staticSpeed = 1;
    [SerializeField] float dynamicSpeed = 10;
    [SerializeField] Transform cameraPosition;
    [SerializeField] Vector2 horizontalBorders = new Vector2(-5, 5);
    [SerializeField] Vector2 verticalBorders = new Vector2(0, 8);
    Vector3 dynamicVelocity = default;

    public Rigidbody PlayerRigidbody { get; private set; }

    void Start()
    {
        PlayerRigidbody = GetComponent<Rigidbody>();

        if(cameraPosition == null)
        {
            cameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
    }

    public void HandleMoving()
    {
        if (Input.GetButton("Tap"))
        {
            var desiredDynamicVelocity = getUpdatedVelocity();
            dynamicVelocity = smoothMoving(dynamicVelocity, desiredDynamicVelocity, 24f);
        }
        else
        {
            dynamicVelocity = smoothMoving(dynamicVelocity, Vector3.zero, 1f);
        }
    }

    public void UpdatePlayerVelocity()
    {
        var velocity = Vector3.forward * staticSpeed + dynamicVelocity;
        var targetPosition = PlayerRigidbody.position + velocity * Time.deltaTime;
        PlayerRigidbody.MovePosition(limitPosition(targetPosition, new Vector2(horizontalBorders.x , horizontalBorders.y), 
            new Vector2(cameraPosition.position.z + verticalBorders.x, cameraPosition.position.z + verticalBorders.y)));
    }

    Vector3 getUpdatedVelocity()
    {
        var updatedDynamicVelocity = getActualVelocityFromMouseAxis();
        return updatedDynamicVelocity *= dynamicSpeed;
    }

    Vector3 getActualVelocityFromMouseAxis()
    {
        return Input.GetAxis("Mouse X") * Vector3.right + Input.GetAxis("Mouse Y") * Vector3.forward;
    }

    Vector3 smoothMoving(Vector3 startVelocity, Vector3 desiredVelocity, float smoothness)
    {
        return Vector3.Lerp(startVelocity, desiredVelocity, smoothness * Time.deltaTime);
    }

    Vector3 limitPosition(Vector3 position, Vector2 HorizontalBorders, Vector2 VerticalBorders)
    {
        return new Vector3(Mathf.Clamp(position.x, HorizontalBorders.x, HorizontalBorders.y), position.y, 
            Mathf.Clamp(position.z, VerticalBorders.x, VerticalBorders.y));
    }
}
