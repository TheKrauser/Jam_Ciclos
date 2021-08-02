using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed;
    private float moveDirection = 1;
    private bool facingRight = true;
    private Transform visuals;
    private Transform checks;
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private float radius;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask groundLayer, wallLayer;
    private bool checkingGround;
    private bool checkingWall;
    private bool checkingPlayer;
    private bool canAttack = true;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private int health;
    private int maxHealth;
    [SerializeField] private Image bar;

    private Rigidbody2D rb;
    private Animator anim;

    private State state;

    public enum State
    {
        PATROL,
        ATTACKING,
        DEAD,
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        bar = transform.Find("Canvas/Bar").GetComponent<Image>();
        visuals = transform.Find("Visuals").transform;
        checks = transform.Find("Checks").transform;

        maxHealth = health;
    }

    private void Update()
    {
        switch (state)
        {
            case State.PATROL:
                if (checkingPlayer)
                {
                    state = State.ATTACKING;
                }
                break;

            case State.ATTACKING:
                Attack();
                break;

            case State.DEAD:
                break;
        }
    }

    private void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheckPosition.position, radius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheckPosition.position, radius, wallLayer);
        checkingPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        switch (state)
        {
            case State.PATROL:
                Patrol();
                break;

            case State.ATTACKING:
                break;

            case State.DEAD:
                break;
        }
    }

    void Patrol()
    {
        if ((!checkingGround || checkingWall) && !checkingPlayer)
        {
            if (facingRight)
            {
                Flip();
            }
            else if (!facingRight)
            {
                Flip();
            }
        }

        rb.velocity = new Vector2(speed * moveDirection, rb.velocity.y);
    }

    void Flip()
    {
        moveDirection *= -1;
        facingRight = !facingRight;
        visuals.transform.Rotate(0, 180, 0);
        checks.transform.Rotate(0, 180, 0);
    }

    void Attack()
    {
        if (canAttack)
        {
            anim.SetTrigger("Attack");
            canAttack = false;
        }
    }

    public void AttackRaycast()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hits)
        {
            var p = player.GetComponent<IDamageable>();
            p.Damage(1);
        }
    }
    public void ResetAttack()
    {
        canAttack = true;
        state = State.PATROL;
    }

    public void Damage(int damage)
    {
        health -= damage;
        bar.fillAmount = (float)health / (float)maxHealth;
        SceneControl.Instance.DamageBlink(visuals.GetComponent<SpriteRenderer>());

        if (health <= 0)
        {
            AudioManager.Instance.PlaySoundEffect("Explosion");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheckPosition.position, radius);
        Gizmos.DrawWireSphere(groundCheckPosition.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
