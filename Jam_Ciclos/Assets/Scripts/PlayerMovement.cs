using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ground Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float airMoveSpeed = 20f;
    private float x;
    private bool facingRight = true;
    private bool isMoving;

    [Header("Jump Movement")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckPosition;
    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] float raySize, rayDistance;
    private bool isGrounded;
    private bool canJump;

    [Header("Wall Movement")]
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;

    [Header("Wall Jump Movement")]
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float wallJumpDirection = -1f;
    [SerializeField] private Vector2 wallJumpAngle;

    [Header("Attack")]
    private bool canAttack = true;

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D box;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        wallJumpAngle.Normalize();
    }

    private void Update()
    {
        Inputs();
        CheckRaycast();
        Attack();
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
        AnimationControl();
        WallSlide();
        WallJump();
    }

    void Inputs()
    {
        x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isWallSliding))
        {
            canJump = true;
        }
    }

    void CheckRaycast()
    {
        isGrounded = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, rayDistance, groundLayer);
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);
    }

    void Movement()
    {
        if (x != 0)
            isMoving = true;
        else
            isMoving = false;

        if (isGrounded)
        {
            rb.velocity = new Vector2(x * speed, rb.velocity.y);
        }
        
        else if (!isGrounded && !isWallSliding && x != 0)
        {
            rb.AddForce(new Vector2(airMoveSpeed * x, 0));

            if (Mathf.Abs(rb.velocity.x) > speed)
            {
                rb.velocity = new Vector2(x * speed, rb.velocity.y);
            }
        }

        if (x < 0 && facingRight && canAttack)
            Flip();
        else if (x > 0 && !facingRight && canAttack)
            Flip();
    }

    void Flip()
    {
        if (!isWallSliding)
        {
            wallJumpDirection *= -1f;
            facingRight = !facingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    void Jump()
    {
        if (canJump && isGrounded)
        {
            Debug.Log("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
        }
    }

    void WallSlide()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
        }

    }

    void WallJump()
    {
        if ((isWallSliding || isTouchingWall) && canJump)
        {
            if (wallJumpDirection > 0)
            {
                if (x < 0)
                {
                    Debug.Log("Esquerda > Esquerda");
                    rb.AddForce(new Vector2((wallJumpForce * 2) * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
                    canJump = false;
                }
                else if (x >= 0)
                {
                    Debug.Log("Esquerda > Direita");
                    rb.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
                    canJump = false;
                }
            }

            else if (wallJumpDirection < 0)
            {
                if (x > 0)
                {
                    Debug.Log("Direita > Direita");
                    rb.AddForce(new Vector2((wallJumpForce * 2) * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
                    canJump = false;
                }
                else if (x <= 0)
                {
                    Debug.Log("Direita > Esquerda");
                    rb.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
                    canJump = false;
                }
            }
        }
    }

    void AnimationControl()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("Y", rb.velocity.y);
        anim.SetBool("isSliding", isWallSliding);
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.C) && canAttack)
        {
            anim.SetTrigger("Attack");
            canAttack = false;
        }
    }

    public void ResetAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(groundCheckPosition.position, groundCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);
    }
}
