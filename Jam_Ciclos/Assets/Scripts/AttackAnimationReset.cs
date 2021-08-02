using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationReset : MonoBehaviour
{
    [SerializeField] private bool isPlayer, isMelee, isRanged, isBoss;
    private PlayerMovement player;
    private MeleeEnemy melee;
    private FlyEnemy ranged;

    private void Start()
    {
        if (isPlayer)
            player = GetComponentInParent<PlayerMovement>();

        else if (isMelee)
            melee = GetComponentInParent<MeleeEnemy>();

        else if (isRanged)
            ranged = GetComponentInParent<FlyEnemy>();

    }

    void ResetAttack()
    {
        if (isPlayer)
            player.ResetAttack();

        else if (isMelee)
            melee.ResetAttack();
    }

    void AttackRaycast()
    {
        if (isPlayer)
            player.AttackRaycast();

        else if (isMelee)
            melee.AttackRaycast();

        else if (isRanged)
            ranged.Shoot();
    }
}
