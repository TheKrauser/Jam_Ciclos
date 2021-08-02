using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    public static SceneControl Instance;

    private PlayerMovement player;
    public static Transform lastCheckpoint;

    public static bool isPlayerDead;
    public GameObject deathDialogue;
    public GameObject bossDeathDialogue;
    public GameObject bossEndDialogue;
    public GameObject death2Dialogue;
    public bool death01Activated = false;
    public bool death02Activated = false;
    public bool bossDeathActivated = false;

    public GameObject camera1, camera2;

    public int deathCount;
    public bool isBossActive = false;
    public bool isBossDead = false;

    private State state;
    public enum State
    {
        DEATH01,
        DEATH02,
        BOSSDEATH,
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        player = Utilities.GetPlayerScript();
        isBossActive = false;
        deathCount = 0;
    }

    private void Update()
    {
        CheckPlayerDead();
        CheckBossDead();
    }

    public static void SetCheckpoint(Vector2 position)
    {
        lastCheckpoint.position = position;
    }

    public void CheckBossDead()
    {
        if (isBossDead)
            bossEndDialogue.SetActive(true);
        else
            bossEndDialogue.SetActive(false);
    }
    public void CheckPlayerDead()
    {
        if (player.IsPlayerDead() && !isBossActive)
        {
            if (deathCount == 1 && !death01Activated)
            {
                ChangeState(State.DEATH01);
                death01Activated = true;
            }
            else if (deathCount > 1 && !death02Activated)
            {
                ChangeState(State.DEATH02);
                death02Activated = true;
            }
        }
        else if (player.IsPlayerDead() && isBossActive && !bossDeathActivated)
        {
            ChangeState(State.BOSSDEATH);
            bossDeathActivated = true;
        }
        else if (!player.IsPlayerDead())
        {
            death01Activated = false;
            death02Activated = false;
            bossDeathActivated = false;
        }
    }

    public void DamageBlink(SpriteRenderer render)
    {
        StartCoroutine(DamageBlinkCoroutine(render));
    }

    public IEnumerator DamageBlinkCoroutine(SpriteRenderer render)
    {
        if (render != null)
            render.color = Color.black;

        yield return new WaitForSeconds(0.05f);

        if (render != null)
            render.color = Color.white;
    }

    public void ChangeCamera()
    {
        camera1.SetActive(false);
        camera2.SetActive(true);
    }

    private void ChangeState(State stateS)
    {
        state = stateS;

        switch (state)
        {
            case State.DEATH01:
                Instantiate(deathDialogue, transform.position, Quaternion.identity);
                break;

            case State.DEATH02:
                Instantiate(death2Dialogue, transform.position, Quaternion.identity);
                break;

            case State.BOSSDEATH:
                Instantiate(bossDeathDialogue, transform.position, Quaternion.identity);
                break;
        }
    }
}
