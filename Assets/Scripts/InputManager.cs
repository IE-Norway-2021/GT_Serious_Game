using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public Action<Vector3Int> OnMouseClick, OnMouseHold;
    public Action OnMouseUp;
    private Vector2 cameraMovementVector;

    [SerializeField]
    Camera mainCamera;

    public LayerMask groundMask;

    public Vector2 CameraMovementVector
    {
        get { return cameraMovementVector; }
    }


    private void Update()
    {
        ChechArrowInput();
    }


    private void ChechArrowInput()
    {
        cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
