using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour, IDamageable
{
    private Transform player;
    [SerializeField] private GameObject batGO, sphereGO;

    [SerializeField] private Transform[] airPositions;
    [SerializeField] private Transform[] groundPositions;
    [SerializeField] private Transform middleStagePosition;
    [SerializeField] private Transform attackSpawnPosition;
    [SerializeField] private Transform[] downSpawns0, downSpawns1;

    [SerializeField] private Transform teleportPosition;
    private bool canSpawn = false;
    [SerializeField] private LayerMask obstaclesLayer;
    public Transform meleeAttackPosition;
    public LayerMask playerLayer;

    [SerializeField] private int health = 30;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform visuals;

    public GameObject batParticle;

    [SerializeField] private State state;
    public enum State
    {
        INACTIVE,
        MISSING,
        AIR_ATTACK,
        DOWN_ATTACK,
        GROUND_ATTACK,
        MELEE_ATTACK,
        DEAD,
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        visuals = transform.Find("Visuals").transform;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //    AirAttack();
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    StartCoroutine(ChangeStateCooldown(0.1f, State.MISSING));

        switch (state)
        {
            case State.INACTIVE:
                if (SceneControl.Instance.isBossActive)
                    ChangeState(State.MISSING);
                break;

            case State.MISSING:
                break;

            case State.AIR_ATTACK:
                break;

            case State.DOWN_ATTACK:
                break;

            case State.GROUND_ATTACK:
                break;

            case State.MELEE_ATTACK:
                break;

            case State.DEAD:
                break;
        }
    }

    private void AirAttack()
    {
        StartCoroutine(AirAttackCoroutine(2f));
    }

    private void DownAttack()
    {
        StartCoroutine(DownAttackCoroutine());
    }

    private void GroundAttack()
    {

    }

    private void MeleeAttack()
    {
        anim.SetTrigger("Attack02");
    }

    private void Dead()
    {

    }

    private void CheckSpawn()
    {
        Debug.Log("RAYCAST");
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(teleportPosition.position, 1f, obstaclesLayer);

        if (obstacles.Length == 0)
        {
            Debug.Log("MELEE");
            Debug.Log(obstacles.Length);
            Debug.Log(obstacles);
            StartCoroutine(MeleeAttackCoroutine());
        }
        else
        {
            Debug.Log("TRY AGAIN");
            SelectPosition();
        }

    }

    private void SelectPosition()
    {
        Debug.Log("SELECT POSITION");
        var random = Random.Range(0, 2);

        if (random == 0)
        {
            teleportPosition.position = new Vector3(player.position.x - 3f, -7.75f, 0);
        }
        else
        {
            teleportPosition.position = new Vector3(player.position.x + 3f, -7.75f, 0);
        }

        CheckSpawn();
    }

    private IEnumerator AirAttackCoroutine(float time)
    {
        Flip();
        anim.SetTrigger("Appear");
        yield return new WaitForSeconds(1.5f);

        anim.SetTrigger("Attack02");
        yield return new WaitForSeconds(1.2f);

        for (int i = 0; i < 6; i++)
        {
            Instantiate(batGO, attackSpawnPosition.position, Quaternion.identity);
            yield return new WaitForSeconds(time);
        }
        anim.SetBool("isFinished", true);

        StartCoroutine(ChangeStateCooldown(2f, State.MISSING));
    }

    private IEnumerator ChangeStateCooldown(float time, State stateChange)
    {
        yield return new WaitForSeconds(time);

        ChangeState(stateChange);

        if (stateChange == State.MISSING)
        {
            GameObject particle = Instantiate(batParticle, transform.position, Quaternion.identity);
            Destroy(particle, 3);
            AudioManager.Instance.PlaySoundEffect("Disappear");
            anim.SetTrigger("Disappear");
        }
    }

    private IEnumerator DownAttackCoroutine()
    {
        Flip();
        anim.SetTrigger("Appear");
        yield return new WaitForSeconds(1.7f);

        anim.SetTrigger("Attack03");

        for (int i = 0; i < downSpawns0.Length; i++)
        {
            GameObject sphere = Instantiate(sphereGO, downSpawns0[i].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1.7f);

        anim.SetTrigger("Attack03");
        for (int i = 0; i < downSpawns1.Length; i++)
        {
            GameObject sphere = Instantiate(sphereGO, downSpawns1[i].position, Quaternion.identity);
        }

        StartCoroutine(ChangeStateCooldown(2f, State.MISSING));
    }

    private IEnumerator MeleeAttackCoroutine()
    {
        transform.position = teleportPosition.position;
        Flip();
        anim.SetTrigger("Appear");
        yield return new WaitForSeconds(1f);

        anim.SetTrigger("Attack01");

        StartCoroutine(ChangeStateCooldown(1.5f, State.MISSING));
    }

    private void Flip()
    {
        if (player != null)
        {
            Vector3 aimDirection = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            Debug.Log(angle);
            if (angle < -90f || angle > 90f)
            {
                transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
            }
            else
            {
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
        }
    }

    private void ChangeState(State stateS)
    {
        anim.SetBool("isFinished", false);
        state = stateS;

        switch (state)
        {
            case State.INACTIVE:
                break;

            case State.MISSING:
                var random = Random.Range(0, 3);
                if (random == 0)
                {
                    StartCoroutine(ChangeStateCooldown(3f, State.AIR_ATTACK));
                }
                else if (random == 1)
                {
                    StartCoroutine(ChangeStateCooldown(2.5f, State.DOWN_ATTACK));
                }
                else
                {
                    StartCoroutine(ChangeStateCooldown(2.5f, State.MELEE_ATTACK));
                }
                break;

            case State.AIR_ATTACK:
                transform.position = airPositions[Random.Range(0, airPositions.Length)].position;
                StartCoroutine(AirAttackCoroutine(1.5f));
                break;

            case State.DOWN_ATTACK:
                transform.position = middleStagePosition.position;
                StartCoroutine(DownAttackCoroutine());
                break;

            case State.GROUND_ATTACK:
                transform.position = groundPositions[Random.Range(0, airPositions.Length)].position;
                break;

            case State.MELEE_ATTACK:
                SelectPosition();
                break;

            case State.DEAD:
                //Dead();
                break;
        }
    }

    public void Damage(int damage)
    {
        SceneControl.Instance.DamageBlink(visuals.GetComponent<SpriteRenderer>());
        health -= damage;

        if (health <= 0)
        {
            SceneControl.Instance.isBossDead = true;
            AudioManager.Instance.PlaySoundEffect("Death2");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(teleportPosition.position, 1f);
    }
}
