using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    PlayerInputActions controls;
    Vector2 move;
    CharacterController controller;
    public GameObject model;
    Animator animator;

    float rotateSpeed = 9.0f; // The rotation speed of the model
    float speed = 4f; // The speed of the player

    void Awake()
    {
        controls = new PlayerInputActions();

        controls.Player.Jump.performed += ctx => Grow();

        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;

    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Grow()
    {
        transform.localScale *= 1.1f;
    }

    private void Update()
    {
        Vector3 movement = new Vector3(move.x, 0, move.y) * Time.deltaTime;
        movement = Vector3.ClampMagnitude(movement, 1.0f);

        if (movement.magnitude > 0)
        {
            RotateModelToFaceMovement(movement);
        }

        movement *= speed;
        
        controller.Move(movement);
    }

    void RotateModelToFaceMovement(Vector3 moveDirection)
    {
        Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }
}
