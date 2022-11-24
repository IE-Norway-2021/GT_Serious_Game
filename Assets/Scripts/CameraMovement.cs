using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera gameCamera;
    public float cameraMovementSpeed = 5;

    public Transform Target;

    // private void Start()
    // {
    //     gameCamera = GetComponent<Camera>();
    // }
    public void MoveCamera(Vector3 inputVector)
    {
        // var movementVector = Quaternion.Euler(0, 30, 0) * inputVector;
        // gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
    }

    public Transform cameraOrbit;

    void Start()
    {
        cameraOrbit.position = Target.position;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);

        transform.LookAt(Target.position);
    }
}