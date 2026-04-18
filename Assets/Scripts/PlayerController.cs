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
        //activate scanning mode and stop ship
        isScanning = true;
        rb.linearVelocity = Vector2.zero;
        //get distance to objective
        GameObject objective = GameObject.FindGameObjectWithTag("Objective");
        float distance = Mathf.Abs((objective.transform.position - transform.position).magnitude);
        //show scan effect based on distance
        Debug.Log("distance: " + distance);
        
        //start tractor beam if within min distance
        if (distance <= tractorDistance)
        {
            Debug.Log("within distance");
            yield return TractorBeam();
            /*//move objective towards ship and show beam effect
            Debug.Log("Beaming up");
            //destroy objective and end scanning mode when done
            yield return new WaitForSeconds(tractorTime);
            Destroy(GameObject.FindGameObjectWithTag("Objective"));
            gameManager.ObjectiveCollected();
            isScanning = false;*/

        }
        else
        {
            //set timeout equal to scan length then turn off scaning
            yield return new WaitForSeconds(scanTime);
            isScanning = false;
        }
    }

    IEnumerator TractorBeam()
    {
        //move objective towards ship and show beam effect
        Debug.Log("Beaming up");
        //destroy objective and end scanning mode when done
        yield return new WaitForSeconds(tractorTime);
        //Destroy(GameObject.FindGameObjectWithTag("Objective"));
        gameManager.ObjectiveCollected();
        isScanning = false;
    }
}
