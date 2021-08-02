using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationEvents : MonoBehaviour
{
    private BossEnemy bossEnemy;

    private void Start()
    {
        bossEnemy = GetComponentInParent<BossEnemy>();
    }
    public void Disappear()
    {
        transform.parent.position = new Vector2(300, 300);
    }

    public void MeleeRaycast()
    {
        Collider2D hits = Physics2D.OverlapCircle(bossEnemy.meleeAttackPosition.position, 3f, bossEnemy.playerLayer);

        if (hits != null)
        {
            var player = hits.GetComponent<IDamageable>();
            player.Damage(1);
        }
    }
}
