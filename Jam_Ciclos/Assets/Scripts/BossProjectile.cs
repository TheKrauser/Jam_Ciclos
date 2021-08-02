using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private bool followThePlayer;
    public bool goUp;

    private Transform player;

    public GameObject electricParticle;

    private Rigidbody2D rb;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        if (followThePlayer)
        {
            Vector3 playerPos = new Vector2(player.position.x, player.position.y);
            Vector3 aimDirection = (transform.position - playerPos).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        if (followThePlayer)
            rb.AddForce((transform.right * -1) * 4f, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (followThePlayer)
            rb.AddForce((transform.right * -1) * 0.25f, ForceMode2D.Impulse);
        else
        {
            if (goUp)
                rb.velocity = new Vector2(0, 2 * 3.5f);
            else
                rb.velocity = new Vector2(0, -2 * 3.5f);
        }

        var time = 10f;
        time -= Time.deltaTime;
        if (time <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && followThePlayer)
        {
            GameObject particle = Instantiate(electricParticle, transform.position, Quaternion.identity);
            Destroy(particle, 3);

            var player = other.GetComponent<IDamageable>();
            player.Damage(1);
            Destroy(gameObject);
        }

        if ((other.CompareTag("Ground") || other.CompareTag("Walls")) && followThePlayer)
        {
            GameObject particle = Instantiate(electricParticle, transform.position, Quaternion.identity);
            Destroy(particle, 3);
            gameObject.SetActive(false);
            Destroy(gameObject, 3);
        }
    }
}
