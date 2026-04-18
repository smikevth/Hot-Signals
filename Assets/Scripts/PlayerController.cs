using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private Rigidbody2D rb;
    [SerializeField] private float moveForce = 1.0f;
    [SerializeField] private float maxSpeed = 5f;
    private bool isScanning = false;
    [SerializeField] private float scanTime = 1.0f;
    [SerializeField] private float tractorDistance = 1.0f; //max distance to be able to pick up objective
    [SerializeField] private float tractorTime = 2.0f;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject signal;
    [SerializeField] private float farDistance = 10.0f;
    [SerializeField] private float distanceRange = 9.0f;
    [SerializeField] private float closeDistance = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveShip();
    }

    void MoveShip()
    {
        //only move if scan/tractor is not active
        if(gameManager.isGameActive && !isScanning)
        {
            //read move action
            Vector2 moveDirection = moveAction.ReadValue<Vector2>().normalized;
            //rotate
            if(moveAction.IsPressed())
            {
                transform.up = moveDirection;
            }
            //move
            rb.AddForce(moveDirection * moveForce);

            //limit speed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    IEnumerator OnScan()
    {
        if (gameManager.isGameActive)
        {
            //activate scanning mode and stop ship
            isScanning = true;
            rb.linearVelocity = Vector2.zero;
            //get distance to objective
            GameObject objective = GameObject.FindGameObjectWithTag("Objective");
            float distance = Mathf.Abs((objective.transform.position - transform.position).magnitude);
            //show scan effect based on distance
            //Debug.Log("distance: " + distance);


            if (distance <= tractorDistance)
            {
                yield return TractorBeam(objective);
            }
            else
            {
                SetRingColor(distance);
                //set timeout equal to scan length then turn off scaning
                yield return new WaitForSeconds(scanTime);
                isScanning = false;
                signal.SetActive(false);
            }
        }
    }

    IEnumerator TractorBeam(GameObject objective)
    {
        //show objective
        objective.transform.GetChild(0).gameObject.SetActive(true);
        //move objective towards ship and show beam effect
        Debug.Log("Beaming up");
        //destroy objective and end scanning mode when done
        yield return new WaitForSeconds(tractorTime);
        //Destroy(GameObject.FindGameObjectWithTag("Objective"));
        gameManager.ObjectiveCollected();
        isScanning = false;
    }

    void SetRingColor(float distance)
    {
        //has to set the color of the ring based on how far the objective is, further away more red, closer more green. So there should be a value for red from 255 to 0 that goes from far to close and one for green from 0 to 255 close to far and the two values should overlap in the middle to make orange/yellow (hopefully).
        float red = 1.0f;
        if(distance < farDistance)
        {
            if (distance < (farDistance - distanceRange))
            {
                red = 0.0f;
            }
            else
            {
                red = distance / farDistance;
            }
        }
        //float red = Mathf.Lerp(0, 255, farness);
        float green = 1.0f;
        if (distance > closeDistance)
        {
            if (distance > (closeDistance + distanceRange))
            {
                green = 0.0f;
            }
            else
            {
                green = 1 - ((distance - closeDistance)/(distanceRange - closeDistance));
            }
        }
        //float green = Mathf.Lerp(0, 255, closeness);
        Color signalColor = new Color(red, green, 0.0f);
        //Debug.Log("Red: " + red + " Green: " + green + " Distance: " + distance);
        signal.GetComponent<SpriteRenderer>().color = signalColor;
        signal.SetActive(true);
    }
}
