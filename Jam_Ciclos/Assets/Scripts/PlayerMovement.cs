using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDamageable
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
    [SerializeField] private bool jumpEnabled = false;

    [Header("Wall Movement")]
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    [SerializeField] private bool wallSlideEnabled = false;

    [Header("Wall Jump Movement")]
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float wallJumpForceMultiplier;
    [SerializeField] private float wallJumpDirection = -1f;
    [SerializeField] private Vector2 wallJumpAngle;
    [SerializeField] private bool wallJumpEnabled = false;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    private bool canAttack = true;
    [SerializeField] private LayerMask enemiesLayer;
    [SerializeField] private bool attackEnabled = false;
    private bool canTakeDamage = true;

    [Header("Dash Movement")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;
    private Vector2 dashDir;
    private bool isDashing = false;
    [SerializeField] private bool dashEnabled = false;

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D box;

    [Header("Vida Placeholder")]
    [SerializeField] private int health = 3;
    private bool isDead = false;
    private Transform visuals;

    [SerializeField] private GameObject jumpParticle, hitParticle, damageParticle;

    private State state;
    public enum State
    {
        NORMAL,
        DASH,
        DIALOGUE,
        DEAD,
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        visuals = transform.Find("Visuals").transform;

        wallJumpAngle.Normalize();

        AudioManager.Instance.PlaySoundtrack("Start");

Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            speed = 5f;
            airMoveSpeed = 15f;
            dashEnabled = true;
            jumpEnabled = true;
            attackEnabled = true;
            wallJumpEnabled = true;
            wallSlideEnabled = true;
        }
        switch (state)
        {
            case State.NORMAL:
                Inputs();
                CheckRaycast();
                Attack();
                break;

            case State.DASH:
                isDashing = true;
                float speedDrop = 5f;
                dashSpeed -= dashSpeed * speedDrop * Time.deltaTime;

                float dashSpeedMinimum = 4f;
                if (dashSpeed <= dashSpeedMinimum)
                {
                    isDashing = false;
                    state = State.NORMAL;
                }
                break;

            case State.DIALOGUE:
                CheckRaycast();
                AnimationControl();
                break;

            case State.DEAD:
                AnimationControl();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.NORMAL:
                Movement();
                Jump();
                AnimationControl();
                WallSlide();
                WallJump();
                break;

            case State.DASH:
                AnimationControl();
                rb.velocity = dashDir * dashSpeed;
                break;

            case State.DIALOGUE:
                break;

            case State.DEAD:
                break;
        }
    }

    void Inputs()
    {
        x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.X) && (isGrounded || isWallSliding) && jumpEnabled)
        {
            canJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && !isWallSliding && canAttack && dashEnabled && canDash)
        {
            Dash();
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
            GameObject particle = Instantiate(jumpParticle, new Vector3(transform.position.x, box.bounds.min.y, 0), Quaternion.identity);
            Destroy(particle, 3);
            AudioManager.Instance.PlaySoundEffect("Jump2");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
        }
    }

    void WallSlide()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0 && wallSlideEnabled)
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
        if ((isWallSliding || isTouchingWall) && canJump && wallJumpEnabled)
        {
            if (wallJumpDirection > 0)
            {
                if (x < 0)
                {
                    Debug.Log("Esquerda > Esquerda");
                    AudioManager.Instance.PlaySoundEffect("Jump2");
                    rb.AddForce(new Vector2((wallJumpForce * wallJumpForceMultiplier) * wallJumpDirection * wallJumpAngle.x, (wallJumpForce * wallJumpForceMultiplier) * wallJumpAngle.y), ForceMode2D.Impulse);
                    canJump = false;
                }
                else if (x >= 0)
                {
                    Debug.Log("Esquerda > Direita");
                    AudioManager.Instance.PlaySoundEffect("Jump2");
                    rb.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
                    canJump = false;
                }
            }

            else if (wallJumpDirection < 0)
            {
                if (x > 0)
                {
                    Debug.Log("Direita > Direita");
                    AudioManager.Instance.PlaySoundEffect("Jump2");
                    rb.AddForce(new Vector2((wallJumpForce * wallJumpForceMultiplier) * wallJumpDirection * wallJumpAngle.x, (wallJumpForce * wallJumpForceMultiplier) * wallJumpAngle.y), ForceMode2D.Impulse);
                    canJump = false;
                }
                else if (x <= 0)
                {
                    Debug.Log("Direita > Esquerda");
                    AudioManager.Instance.PlaySoundEffect("Jump2");
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
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isDead", isDead);
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.C) && canAttack && attackEnabled)
        {
            anim.SetTrigger("Attack");
            canAttack = false;
        }
    }

    public void AttackRaycast()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemiesLayer);

        foreach (Collider2D enemy in hits)
        {
            AudioManager.Instance.PlaySoundEffect("Hit");
            GameObject particle = Instantiate(hitParticle, enemy.transform.position, Quaternion.identity);
            Destroy(particle, 3);

            var enemies = enemy.GetComponent<IDamageable>();
            enemies.Damage(1);
        }
    }

    void Dash()
    {
        canDash = false;
        dashDir = new Vector2(x, 0);
        if (dashDir.x == 0 && facingRight)
            dashDir.x = 1;
        else if (dashDir.x == 0 && !facingRight)
            dashDir.x = -1;

        dashSpeed = 22f;
        ChangeState(State.DASH);
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void ResetAttack()
    {
        canAttack = true;
    }

    public void ChangeJump(float value)
    {
        jumpForce = value;
    }

    public void Damage(int damage)
    {
        if (canTakeDamage)
        {
            AudioManager.Instance.PlaySoundEffect("Death");
            health -= damage;
            GameObject particle = Instantiate(damageParticle, transform.position, Quaternion.identity);
            Destroy(particle, 3);

            SceneControl.Instance.DamageBlink(visuals.GetComponent<SpriteRenderer>());
            if (health <= 0)
            {
                isDead = true;
                canTakeDamage = false;
                rb.bodyType = RigidbodyType2D.Static;
                ChangeState(State.DEAD);
                anim.SetBool("isDead", isDead);
            }
        }
    }

    private IEnumerator DashInvTime(float invTime)
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invTime);
        canTakeDamage = true;
    }

    public void Respawn(Vector3 respawnPoint, int newHealth)
    {
        transform.position = respawnPoint;

        if (isDead)
        {
            canTakeDamage = true;
            isDead = !isDead;
            rb.bodyType = RigidbodyType2D.Dynamic;
            ChangeState(State.NORMAL);
            health = newHealth;
        }
    }

    public bool IsPlayerDead()
    {
        if (isDead)
            return true;
        else
            return false;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        //Respawn
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(groundCheckPosition.position, groundCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void ChangeState(State stateS)
    {
        state = stateS;
        switch (stateS)
        {
            case State.NORMAL:
                break;

            case State.DASH:
                AudioManager.Instance.PlaySoundEffect("Dash");
                StartCoroutine(DashInvTime(1.5f));
                StartCoroutine(DashCooldown());
                break;

            case State.DEAD:
                SceneControl.Instance.deathCount++;
                x = 0;
                isMoving = false;
                isDashing = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;

            case State.DIALOGUE:
                x = 0;
                isMoving = false;
                isDashing = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
        }
    }

    public void DialogueFunction(string functionName)
    {
        if (functionName == "Enable Movement")
        {
            speed = 5f;
            airMoveSpeed = 15f;
        }

        if (functionName == "Enable Jump")
        {
            jumpForce = 100f;
            jumpEnabled = true;
        }

        if (functionName == "Adjust Jump")
        {
            jumpForce = 10f;
        }

        if (functionName == "Enable Dash")
        {
            dashEnabled = true;
        }

        if (functionName == "Enable Wall")
        {
            wallSlideEnabled = true;
            wallJumpEnabled = true;
        }

        if (functionName == "Enable Attack")
        {
            attackEnabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StartBoss"))
        {
            AudioManager.Instance.StopSoundtrack("Start");
            AudioManager.Instance.PlaySoundtrack("Boss");
            SceneControl.Instance.isBossActive = true;
            SceneControl.Instance.ChangeCamera();
            Destroy(other.gameObject);
        }
    }
}
