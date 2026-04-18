using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2 areaBounds = new Vector2(8.0f, 4.0f); //max x and y of objective spawn area
    [SerializeField] private GameObject objectivePrefab;
    public bool isGameActive = false;
    private GameObject currentObjective;
    private int score = 0;
    private float timer = 10.0f;
    [SerializeField] private TMP_Text timeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGameActive = true;
        PlaceObjective();
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
            currentObjective = Instantiate(objectivePrefab, areaBounds, objectivePrefab.transform.rotation);
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

    private void RoundOver()
    {
        Debug.Log("ended. score: " + score);
        isGameActive = false;
    }
}
