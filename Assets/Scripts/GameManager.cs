using UnityEngine;
using TMPro;
using System.Collections;


public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2 areaBounds = new Vector2(8.0f, 4.0f); //max x and y of objective spawn area
    [SerializeField] private GameObject objectivePrefab;
    public bool isGameActive = false;
    private GameObject currentObjective;
    private int score = 0;
    private float timerInit = 10.0f;
    private float timer;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject[] dialogueBoxes;
    [SerializeField] private TMP_Text[] dialogueTextAreas; 
    [HideInInspector] public bool isDialogueOpen;
    private int dialogueIndex = 0;
    [SerializeField] private Dialogue[] introDialogues;
    [SerializeField] private Dialogue[] postRoundDialogues;
    private bool isIntro = false;
    private bool isPostRound = false;
    private bool isRoundSummary = false;
    [SerializeField] private GameObject roundSummaryBox;
    [SerializeField] private TMP_Text scoreText;
    private float textPause = 1.0f; //time to wait for text to be passable

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartIntro();
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameActive)
        {
            timer -= Time.deltaTime;
            timeText.text = "Time left: " + Mathf.Ceil(timer);
            if(timer <= 0.0f)
            {
                StartCoroutine(RoundOver());
            }
        }
    }

    private void PlaceObjective()
    {
        if(isGameActive)
        {
            //place object in random position
            Vector2 randomPosition = new Vector2(Random.Range(-areaBounds.x, areaBounds.x), Random.Range(-areaBounds.y, areaBounds.y));
            currentObjective = Instantiate(objectivePrefab, randomPosition, objectivePrefab.transform.rotation);
            Debug.Log("Objective placed");
        }
    }

    public void ObjectiveCollected()
    {
        //increment score !!! LATER WILL CHANGE TO COLLECT SPECIFIC COLLECTIBLE/CREW MEMEBER FROM POOL
        score++;
        // destroy objective
        if(currentObjective != null)
        {
            Destroy(currentObjective);
        }
        //place new objective
        PlaceObjective();
    }

    private void StartIntro()
    {
        isDialogueOpen = true;
        isIntro = true;
        SetDialogue(introDialogues[dialogueIndex]);
    }

    public void AdvanceDialogue()
    {
        dialogueIndex++;
        Dialogue[] currentDialogues = null;
        if(isIntro)
        {
            currentDialogues = introDialogues;
        }
        else if(isPostRound)
        {
            currentDialogues = postRoundDialogues;
        }
        if(currentDialogues != null)
        {
            if (dialogueIndex < currentDialogues.Length)
            {
                SetDialogue(currentDialogues[dialogueIndex]);
            }
            else
            {
                CloseDialogues();
                if(isIntro)
                {
                    isIntro = false;
                    StartRound();
                }
                else if(isPostRound)
                {
                    //will add post round summary here
                    if(isRoundSummary)
                    {
                        isPostRound = false;
                        roundSummaryBox.SetActive(false);
                        isRoundSummary = false;
                        StartRound();

                    }
                    else
                    {
                        isDialogueOpen = false;
                        StartCoroutine(RoundSummary());                     
                    }
                }
            }
        }
        else
        {
            Debug.Log("currentDialogues is null for some reason");
        }
    }

    //closes all dialogue windows
    private void CloseDialogues()
    {
        for (int i = 0; i < dialogueBoxes.Length; i++)
        {
            dialogueBoxes[i].SetActive(false);
        }
    }

    private IEnumerator RoundSummary()
    {
        //post score
        scoreText.text = score.ToString();
        //later will list various collectibles the player picked up.
        roundSummaryBox.SetActive(true);
        isRoundSummary = true;
        yield return new WaitForSeconds(textPause);
        isDialogueOpen = true;
    }

    private void SetDialogue(Dialogue dialogue)
    {
        //Dialogue currentDialogue = introDialogues[dialogueIndex];
        for (int i=0; i<dialogueBoxes.Length; i++)
        {
            if(i != dialogue.Speaker)
            {
                dialogueBoxes[i].SetActive(false);
            }
            else
            {
                dialogueBoxes[i].SetActive(true);
            }
        }
        dialogueTextAreas[dialogue.Speaker].SetText(dialogue.Text);
    }

    private void StartRound()
    {
        score = 0;
        timer = timerInit;
        timeText.gameObject.SetActive(true);
        isGameActive = true;
        isDialogueOpen = false;
        PlaceObjective();

    }

    private IEnumerator RoundOver()
    {
        Debug.Log("ended. score: " + score);
        isGameActive = false;
        //destroy objective
        if (currentObjective != null)
        {
            Destroy(currentObjective);
        }
        //dialogue from admiral
        isPostRound = true;
        dialogueIndex = 0;
        SetDialogue(postRoundDialogues[dialogueIndex]);
        //wait a sec to enable skipping dialogue
        yield return new WaitForSeconds(textPause);
        isDialogueOpen = true;
        //move ship off screen?

        //show results screen

        //upgrades? new crew?

        //next round

    }
}
