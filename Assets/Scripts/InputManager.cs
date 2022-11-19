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


    private void Update() {
        CheckClickDownnEvent();
        CheckClickUpEvent();
        CheckClickHoldEvent();
        ChechArrowInput();
    }

    private Vector3Int? RaycastGround() {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask)) {
            return new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
        }
        return null;
    }

    private void CheckClickDownnEvent() {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false) {
            var position = RaycastGround();
            if (position != null) {
                OnMouseClick?.Invoke(position.Value);
            }
        }
    }
    
    private void CheckClickUpEvent() {
        if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false) {
            OnMouseUp?.Invoke();
        }
    }

    private void CheckClickHoldEvent() {
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false) {
            var position = RaycastGround();
            if (position != null) {
                OnMouseHold?.Invoke(position.Value);
            }
        }
    }

    private void ChechArrowInput() {
        cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
