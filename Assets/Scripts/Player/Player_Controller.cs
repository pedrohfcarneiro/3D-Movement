using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Movement")]
    private CharacterController PlayerController;
    private Vector2 moveInput = new Vector2(0,0);
    private Vector3 velocity;
    public Vector3 moveDirection { get; private set; }
    public Vector3 currentFacingDirection { get; private set; }
    [SerializeField] private float Speed = 2f;
    public float Gravity = -9.81f;
    private bool isGrounded;

    [Header("Detection")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .2f;
    public bool isPlayerGrounded { get; private set; }

    void Start()
    {
        PlayerController = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckForGround();
        MyInput();
        Movement();
    }


    private void MyInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //transform input into movement direction.
        moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        moveDirection.Normalize();

        currentFacingDirection = transform.forward;
    }

    public void Movement()
    {
        //adding horizontal movement
        PlayerController.Move(moveDirection * Speed * Time.deltaTime);

        if(isPlayerGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //adding gravity effect
        velocity.y += Gravity * Time.deltaTime;
        PlayerController.Move(velocity * Time.deltaTime);

    }

    private void CheckForGround()
    {
        isPlayerGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

}
