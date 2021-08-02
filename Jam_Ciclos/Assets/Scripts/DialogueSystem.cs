using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
public class DialogueSystem : MonoBehaviour
{
    private int positionInArray = 0;
    private bool canSkipDialogue;
    private bool xKeyPressed;

    public TextMeshProUGUI textGO;
    public GameObject majorDialogueGO;
    public TextVariables[] text;

    public bool activateOnTriggerEnter;
    public bool activateOnTriggerExit;
    public bool activateInSequence;
    public DialogueSystem sequence;
    public float startTimer;
    public float timerToActivate;
    private bool firstActivation = true;

    [SerializeField] private bool playOnAwake;
    [SerializeField] private bool destroyOnEnd;
    [SerializeField] private bool doSomething;

    [SerializeField] private Vector3 respawnPosition;
    public bool isPrefab;

    public State actionOnEnd;

    public enum State
    {
        DO_NOTHING,
        ACTIVATE_MOVEMENT,
        ACTIVATE_JUMP,
        ADJUST_JUMP,
        ACTIVATE_DASH,
        ACTIVATE_WALL,
        ACTIVATE_ATTACK,
        RESPAWN,
        QUIT,
    }

    void Start()
    {
        if (isPrefab)
        {
            majorDialogueGO = GameObject.Find("Canvas/DialogueStart (1)/Dialogue").gameObject;
            textGO = GameObject.Find("Canvas/DialogueStart (1)/Dialogue/Text (TMP)").GetComponent<TextMeshProUGUI>();
        }

        if (playOnAwake)
            StartCoroutine(DisplayText());
        else
            return;

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (canSkipDialogue)
            {
                ShowNextSentence();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
            xKeyPressed = true;
    }

    public IEnumerator DisplayText()
    {
        if (firstActivation)
        {
            yield return new WaitForSeconds(startTimer);
            PlayerMovement player = Utilities.GetPlayerScript();
            player.ChangeState(PlayerMovement.State.DIALOGUE);
            firstActivation = false;
        }

        majorDialogueGO.SetActive(true);
        this.enabled = true;
        canSkipDialogue = false;
        xKeyPressed = false;
        string v = "";

        foreach (char c in text[positionInArray].sentence)
        {
            AudioManager.Instance.PlaySoundEffect("Type");
            v += c.ToString();
            textGO.text = v;
            textGO.fontSize = text[positionInArray].textSize;
            textGO.color = text[positionInArray].textColor;
            if (!xKeyPressed)
                yield return new WaitForSeconds(text[positionInArray].textSpeed / 60);
            else
            {
                v = text[positionInArray].sentence.Substring(0, text[positionInArray].sentence.Length - 1);
                //v = v.Substring(0, v.Length - 1);
            }
        }

        positionInArray++;
        canSkipDialogue = true;

        /*foreach (TextVariables t in text)
        {
            character.sprite = t.imageCharacter;
            string v = "";
            foreach (char c in t.sentence)
            {
                v += c.ToString();
                textGO.text = v;
                textGO.fontSize = t.textSize;
                textGO.color = t.textColor;
                yield return new WaitForSeconds(t.textSpeed / 60);
            }
            yield return new WaitForSeconds(t.delayToNext);
        }*/
    }

    private void ShowNextSentence()
    {
        if (positionInArray < text.Length)
            StartCoroutine(DisplayText());

        else
        {
            positionInArray = 0;
            this.enabled = false;
            firstActivation = true;
            majorDialogueGO.SetActive(false);

            StartCoroutine(Wait(1f));

            if (doSomething)
            {
                PlayerMovement player = Utilities.GetPlayerScript();
                switch (actionOnEnd)
                {
                    case State.DO_NOTHING:
                        player.ChangeState(PlayerMovement.State.NORMAL);
                        break;

                    case State.ACTIVATE_MOVEMENT:
                        player.DialogueFunction("Enable Movement");
                        player.ChangeState(PlayerMovement.State.NORMAL);
                        break;

                    case State.ACTIVATE_JUMP:
                        player.DialogueFunction("Enable Jump");
                        player.ChangeState(PlayerMovement.State.NORMAL);
                        break;

                    case State.ADJUST_JUMP:
                        player.DialogueFunction("Adjust Jump");
                        player.ChangeState(PlayerMovement.State.NORMAL);
                        break;

                    case State.ACTIVATE_DASH:
                        player.DialogueFunction("Enable Dash");
                        player.ChangeState(PlayerMovement.State.NORMAL);
                        break;

                    case State.ACTIVATE_WALL:
                        player.DialogueFunction("Enable Wall");
                        player.ChangeState(PlayerMovement.State.NORMAL);
                        break;

                    case State.ACTIVATE_ATTACK:
                        player.DialogueFunction("Enable Attack");
                        player.ChangeState(PlayerMovement.State.NORMAL);
                        break;

                    case State.RESPAWN:
                        Utilities.RespawnPlayer(respawnPosition);
                        break;

                    case State.QUIT:
                        Application.Quit();
                        break;
                }
            }

            if (activateInSequence)
            {
                sequence.enabled = true;
                sequence.DisplayInTime(timerToActivate);
            }

            if (destroyOnEnd)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activateOnTriggerEnter)
        {
            if (collision.CompareTag("Player"))
            {
                this.enabled = true;
                ShowNextSentence();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activateOnTriggerExit)
        {
            if (collision.CompareTag("Player"))
            {
                this.enabled = true;
                ShowNextSentence();
            }
        }
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(0.1f);
        PlayerMovement player = Utilities.GetPlayerScript();
        player.ChangeState(PlayerMovement.State.NORMAL);
    }

    private IEnumerator TimeToActive(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(DisplayText());
    }

    public void DisplayInTime(float time)
    {
        StartCoroutine(TimeToActive(time));
    }
}


[Serializable]
public struct TextVariables
{
    [TextArea(3, 20)]
    public string sentence;
    public float textSpeed, delayToNext, textSize;
    public Color textColor;
}
