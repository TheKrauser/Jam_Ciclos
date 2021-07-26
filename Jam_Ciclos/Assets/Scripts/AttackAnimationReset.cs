using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationReset : MonoBehaviour
{
    private PlayerMovement player;

    private void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
    }

    void ResetAttack()
    {
        player.ResetAttack();
    }
}
