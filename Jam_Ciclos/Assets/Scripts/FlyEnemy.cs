using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float attackCooldown;

    private Transform player;

    private Transform visuals;

    [SerializeField] private int health;
    private int maxHealth;
    [SerializeField] private Image bar;

    private float distance;
    private bool playerFound = false;

    private Animator anim;

    private State state;
    private enum State
    {
        FOUND_PLAYER,
        NOT_FOUND_PLAYER
    }
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        bar = transform.Find("Canvas/Bar").GetComponent<Image>();
        visuals = transform.Find("Visuals").transform;

        maxHealth = health;

        //StartCoroutine(Attack());
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);

        if (!playerFound && distance <= 15f)
        {
            ChangeState(State.FOUND_PLAYER);
        }
    }


    private IEnumerator Attack()
    {
        Flip();
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCooldown);

        if (distance <= 15f)
            StartCoroutine(Attack());
        else
            ChangeState(State.NOT_FOUND_PLAYER);
    }

    public void Shoot()
    {
        GameObject shoot = Instantiate(projectile, transform.position, Quaternion.identity);
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
                visuals.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                visuals.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    private void ChangeState(State stateS)
    {
        state = stateS;
        switch (state)
        {
            case State.NOT_FOUND_PLAYER:
                playerFound = false;
                break;

            case State.FOUND_PLAYER:
                playerFound = true;
                StartCoroutine(Attack());
                break;
        }
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
}
