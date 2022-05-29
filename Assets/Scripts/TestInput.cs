using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInput : MonoBehaviour
{
    // References
    PlayerInputActions controls;
    CharacterController controller;
    public GameObject model;
    Animator animator;

    // Movement
    Vector3 movement;
    Vector2 move;

    // Values
    float rotateSpeed = 9.0f; // The rotation speed of the model
    float speed = 4f; // The speed of the player
    float maxSpeed = 4f; // The maximum speed of the player
    float speedWhenCharging = 2f; // The speed of the player when charging a sword attack
    float speedWhenImpulse = 8f; // The speed of the player when the sword attack is released
    float attackRate = 2f; // How many times the player can attack per second
    float nextAttackTime = 0f; // Counter for the attackRate

    // State
    bool attacking = false;

    void Awake()
    {
        controls = new PlayerInputActions();

        controls.Player.Attack.performed += ctx => SwordAttack();

        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;

    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("Velocity", controller.velocity.magnitude);
    }

    private void Movement()
    {
        movement = new Vector3(move.x, 0, move.y) * Time.deltaTime;
        movement = Vector3.ClampMagnitude(movement, 1.0f);

        if (movement.magnitude > 0)
        {
            RotateModelToFaceMovement(movement);
        }
        
        movement *= speed;
        controller.Move(movement);
    }

    private void SwordAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + 1f / attackRate;
            speed = speedWhenCharging;
            attacking = true;
            StartCoroutine(SwordSpeedCountdown(4));
        }
    }

    void RotateModelToFaceMovement(Vector3 moveDirection)
    {
        Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
    }

    private IEnumerator SwordSpeedCountdown(int cooldownDuration)
    {
        int counter = cooldownDuration;
        while (counter > 0)
        {
            yield return new WaitForSeconds(0.1f);
            counter--;
            if(counter == 3)
            {
                speed = speedWhenImpulse;
            }
        }
        speed = maxSpeed;
        attacking = false;
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
