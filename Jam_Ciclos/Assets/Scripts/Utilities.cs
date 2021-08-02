using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static PlayerMovement player;

    public static PlayerMovement GetPlayerScript()
    {
        PlayerMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        return player;
    }

    public static void RespawnPlayer(Vector3 respawnPoint)
    {
        var player = GetPlayerScript();

        player.Respawn(respawnPoint, 3);
    }
}
