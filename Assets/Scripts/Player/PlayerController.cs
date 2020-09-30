using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float Speed = 1.0f;
    public float RotationSpeed = 1.0f;
    public float JumpSpeed = 1.0f;

    private CharacterController CharacterController;

    private float FallVelocity;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Gravity
        if (!CharacterController.isGrounded)
        {
            FallVelocity += 9.81f * Time.deltaTime;
        }
        else if (CharacterController.isGrounded && Input.GetKey(KeyCode.Space)) // Jump
        {
            FallVelocity = -JumpSpeed;
        }
        else
        {
            FallVelocity = 0.0f;
        }

        CharacterController.Move(transform.TransformVector(new Vector3(horizontal, -FallVelocity, vertical) * Time.deltaTime * Speed));

        // Camera
        float mouseX = Input.GetAxis("Mouse X");

        transform.Rotate(new Vector3(0.0f, mouseX, 0.0f) * Time.deltaTime * RotationSpeed);
    }
}
