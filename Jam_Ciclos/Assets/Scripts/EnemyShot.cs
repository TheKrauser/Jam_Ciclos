using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EnemyShot : MonoBehaviour
{

    [Tooltip("Position we want to hit")]
    public Vector3 targetPos;
    public Transform target;

    [Tooltip("Horizontal speed, in units/sec")]
    public float speed = 10;

    [Tooltip("How high the arc should be, in units")]
    public float arcHeight = 1;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject endParticle;


    Vector3 startPos;

    void Start()
    {
        // Cache our start position, which is really the only thing we need
        // (in addition to our current position, and the target).
        startPos = transform.position;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetPos = target.position;
    }

    void Update()
    {
        // Compute the next position, with arc added in
        float x0 = startPos.x;
        float x1 = targetPos.x;
        float dist = x1 - x0;
        float nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
        float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
        float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
        var nextPos = new Vector3(nextX, baseY + arc, transform.position.z);

        // Rotate to face the next position, and then move there
        transform.rotation = LookAt2D(nextPos - transform.position);
        transform.position = nextPos;

        // Do something when we reach the target
        if (nextPos == targetPos) Arrived();
    }

    void Arrived()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 2f, playerLayer);

        if (hit != null)
        {
            var p = hit.GetComponent<IDamageable>();
            p.Damage(1);
        }

        GameObject particle = Instantiate(endParticle, transform.position, Quaternion.identity);
        Destroy(particle, 3);
        AudioManager.Instance.PlaySoundEffect("Explosion2");
        gameObject.SetActive(false);
        Destroy(gameObject, 3);
    }

    /// 
    /// This is a 2D version of Quaternion.LookAt; it returns a quaternion
    /// that makes the local +X axis point in the given forward direction.
    /// 
    /// forward direction
    /// Quaternion that rotates +X to align with forward
    static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
}