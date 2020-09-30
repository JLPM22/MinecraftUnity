using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float RotationSpeed;

    private Vector3 Rotation;

    private void Awake()
    {
        Rotation = transform.localRotation.eulerAngles;
    }

    void Update()
    {
        float mouseY = -Input.GetAxis("Mouse Y");
        Rotation.x += mouseY * Time.deltaTime * RotationSpeed;
        Rotation.x = Mathf.Clamp(Rotation.x, -85.0f, 85.0f);
        transform.localRotation = Quaternion.Euler(Rotation);
    }
}
