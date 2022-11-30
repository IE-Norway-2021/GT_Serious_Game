using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    // private Vector2 cameraMovementVector;

    // [SerializeField]
    // Camera mainCamera;

    // public Vector2 CameraMovementVector
    // {
    //     get { return cameraMovementVector; }
    // }


    // // private void Update()
    // // {
    // //     ChechArrowInput();
    // // }


    // private void ChechArrowInput()
    // {
    //     cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    // }


    public GameObject cameraOrbit;

    public Vector3 Target;

    public GameSettings gameSettings;

    void Start()
    {
        cameraOrbit.transform.position = Target;
    }

    void Update()
    {
        // TODO : ajouter le fait de pouvoir zoom sur les bords de la map, et de mettre un zoom minimal par défaut et max

        // Gestion camera
        if (Input.GetMouseButton(0))
        {
            float h = gameSettings.rotateSpeed * Input.GetAxis("Mouse X");
            float v = gameSettings.rotateSpeed * Input.GetAxis("Mouse Y");

            if (cameraOrbit.transform.eulerAngles.z + v <= 0.1f || cameraOrbit.transform.eulerAngles.z + v >= 179.9f)
                v = 0;

            cameraOrbit.transform.eulerAngles = new Vector3(cameraOrbit.transform.eulerAngles.x, cameraOrbit.transform.eulerAngles.y + h, gameSettings.angleView);
        }

        float scrollFactor = Input.GetAxis("Mouse ScrollWheel");

        if (scrollFactor != 0)
        {
            cameraOrbit.transform.localScale = cameraOrbit.transform.localScale * (1f - scrollFactor);
        }
        transform.LookAt(Target);
    }


}
