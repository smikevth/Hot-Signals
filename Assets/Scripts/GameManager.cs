using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2 areaBounds = new Vector2(8.0f, 4.0f); //max x and y of objective spawn area
    [SerializeField] private GameObject objectivePrefab;
    public bool isGameActive = false;
    private GameObject currentObjective;
    private int score = 0;
    private float timer = 60.0f;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject captainDialogue;
    [SerializeField] private GameObject admiralDialogue;
    [HideInInspector] public bool isDialogueOpen;
    private int dialogueIndex = 0;

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
                RoundOver();
            }
        }
    }

    private void PlaceObjective()
    {
        if(isGameActive)
        {
            //place object in random position !!! NEED TO CHANGE THIS TO BE RANDOM POS
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
        captainDialogue.SetActive(true);
        isDialogueOpen = true;
        
    }

    public void AdvanceDialogue()
    {
        switch(dialogueIndex)
        {
            case 0:
                captainDialogue.SetActive(false);
                admiralDialogue.SetActive(true);
                dialogueIndex++;
                break;
            case 1:
                captainDialogue.SetActive(true);
                admiralDialogue.SetActive(false);
                dialogueIndex++;
                break;
            case 2:
                captainDialogue.SetActive(false);
                admiralDialogue.SetActive(true);
                dialogueIndex++;
                break;
            case 3:
                captainDialogue.SetActive(false);
                admiralDialogue.SetActive(false);
                dialogueIndex++;
                StartRound();
                break;
        }
    }

    private void StartRound()
    {
        isGameActive = true;
        isDialogueOpen = false;
        PlaceObjective();
    }

    private void RoundOver()
    {
        Debug.Log("ended. score: " + score);
        isGameActive = false;
    }
}
