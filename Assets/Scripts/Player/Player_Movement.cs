using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    private Rigidbody playerRB;
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float breakAcceleraton = 3f;
    private bool isInMaxSpeed = false;
    private bool isGrounded;
    public bool canAccelerate { get; private set; } = true;

    [Header("Jump")]
    
    private bool canJump; public bool getCanJump() { return canJump; } public void setCanJump(bool canPlayerJump) { canJump = canPlayerJump; }


    public Vector3 currentMoveDirection { get; private set;} = new Vector3(0, 0, 0);
    public Vector3 currentFacingDirection { get; private set;} = new Vector3(0, 0, 0);

    public Vector3 moveDirection { get; private set; } = new Vector3(0, 0,0);

    private Vector2 moveInput = new Vector2(0, 0);

    [Header("Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = .2f;


    void Start()
    {
        //Referência ao rigidbody do player
        playerRB = GetComponent<Rigidbody>();
        isInMaxSpeed = false;
    }


    void Update()
    {
        MyInput();
        Debug.Log(isGrounded);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        Movement();
    }


    public void MyInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal") , Input.GetAxisRaw("Vertical"));

        //Transforma o input do player em um vetor que representa a direção que o player deseja se mover
        moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        moveDirection.Normalize();

        currentFacingDirection = transform.forward;

        isInMaxSpeed = playerRB.velocity.magnitude >= maxSpeed ? true : false;

        canAccelerate = isInMaxSpeed ? false : true;
    }

    public void Movement()
    {
        currentMoveDirection = playerRB.velocity.normalized;

        //Find Vel relative to look
        Vector3 velRelativeToLook = FindVelRelativeToLook();
        
        //limita a velocidade do player a maxSpeed
        if(playerRB.velocity.magnitude >= maxSpeed)
        {
            playerRB.velocity = Vector3.ClampMagnitude(playerRB.velocity, maxSpeed);
        }
        //Acelera o player
        if(canAccelerate)
        {
            playerRB.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
        }

        //Fricção adicional
        if (Mathf.Abs(velRelativeToLook.x) > 0 && Mathf.Abs(moveInput.x) < 0.05f || velRelativeToLook.x > 0 && moveInput.x < 0 || velRelativeToLook.x < 0 && moveInput.x > 0)
        {
            playerRB.AddForce(transform.right * -velRelativeToLook.normalized.x * breakAcceleraton, ForceMode.Acceleration);
        }
        if (Mathf.Abs(velRelativeToLook.z) > 0 && Mathf.Abs(moveInput.y) < 0.05f || velRelativeToLook.z > 0 && moveInput.y < 0 || velRelativeToLook.z < 0 && moveInput.y > 0)
        {
            playerRB.AddForce(transform.forward * -velRelativeToLook.normalized.z * breakAcceleraton, ForceMode.Acceleration);
        }


        //Jump


    }


    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    public Vector3 FindVelRelativeToLook()
    {
        float lookAngle = playerRB.gameObject.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(playerRB.velocity.x, playerRB.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = playerRB.velocity.magnitude;
        float zMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector3(xMag,0,zMag);
    }

    private void CheckGrounded()
    {
        isGrounded = false;

        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                isGrounded = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

}
