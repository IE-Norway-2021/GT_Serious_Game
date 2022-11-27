using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 Target;

    public Transform cameraOrbit;

    void Start()
    {
        cameraOrbit.position = Target;
    }

    void Update()
    {
        // TODO : ajouter le fait de pouvoir zoom sur les bords de la map, et de mettre un zoom minimal par défaut et max
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);

        transform.LookAt(Target);
    }
}