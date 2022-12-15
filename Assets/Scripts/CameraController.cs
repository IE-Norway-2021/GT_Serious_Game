using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraController : MonoBehaviour
{

    public GameSettings gameSettings;

    public Transform cameraTransform;

    public float moveSpeed;
    public float movementTime;

    public Vector3 newPosition;
    public Vector3 newZoom;

    public Vector3 zoomAmount;

    public Quaternion newRotation;

    public float rotationAmount;


    // mouse movement
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;


    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newZoom = cameraTransform.localPosition;
        newRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    void HandleMouseInput()
    {
        // 0 : left click
        // 1 : right click
        // 2 : middle click

        if (Input.GetMouseButtonDown(1))
        {
            rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }

    }


    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            newPosition += (transform.forward * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition += (transform.forward * -moveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition += (transform.right * -moveSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition += (transform.right * moveSpeed);
        }

        // zoom in when mouse rolls up
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            // check if new zoom is within the limits
            if (newZoom.z + zoomAmount.z < gameSettings.zoomMax.x && newZoom.z + zoomAmount.z > gameSettings.zoomMin.z)
                newZoom += zoomAmount;
        }

        // zoom out when mouse rolls down
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // check if new zoom is within the limits
            if (newZoom.z - zoomAmount.z < gameSettings.zoomMax.x && newZoom.z - zoomAmount.z > gameSettings.zoomMin.z)
                newZoom -= zoomAmount;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, movementTime);

    }
}
