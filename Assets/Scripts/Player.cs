using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // dying utility 
    // (before reloading scene there's .5 sec delay and after touching the enemy player can get into the goal
    // just to prevent it
    private bool isDying = false; 

    [SerializeField]
    private Rigidbody2D rb; // player rigid body

    [SerializeField]
    private float forceStrength = 2000f; // start force strength for moving the pluck

    private bool isPushed = false; // has the puck been pushed (1 time per round ONLY)
    private float timePassedSinceLastForce; // time has been passed since last force influence
    [SerializeField] private float maxTimePassedSinceLastForce = 1f; // time to wait before round restart (kind of death/Defeat)

    private Vector2 currentDirection; // current puck direction movement
    private Vector2 lastForceVelocity; // start force velocity from last force influence

    private void Update()
    {
        // check for 'loosing'
        if (isPushed)
        {
            if (timePassedSinceLastForce >= maxTimePassedSinceLastForce)
                GameManager.instance.Defeat();
            else
                timePassedSinceLastForce += Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "Enemy":
            {
                isDying = true;
                GameManager.instance.Defeat();
            }
            break;
            case "Hockey Field":
            {
                // reduce every next force touch of the field side by 2/3
                lastForceVelocity = lastForceVelocity * 2 / 3;

                // push the puck according to collision contact normal vector
                forceStrength = lastForceVelocity.magnitude;
                currentDirection = collision.GetContact(0).normal;
                rb.AddForce(currentDirection * forceStrength, ForceMode2D.Impulse);

                // update pushing utility
                timePassedSinceLastForce = 0;
            } break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal" && !isDying)
            GameManager.instance.Victory();
    }

    // push the puck towards the swipe (Mobile) / click (PC)
    public void Push()
    {
        // prevent multi acceleration (no need bc player can push the puck only 1 time per round)
        if (rb.velocity.magnitude > 0)
            rb.velocity = Vector2.zero;

        // push the puck 
        currentDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(transform.position)).normalized;
        rb.AddForce(currentDirection * forceStrength, ForceMode2D.Impulse);
        lastForceVelocity = rb.velocity;

        // pushing utils
        isPushed = true;
        timePassedSinceLastForce = 0;
    }
    public void Push(Vector2 pointerDown, Vector2 pointerUp)
    {
        // prevent multi acceleration (no need bc player can push the puck only 1 time per round)
        if (rb.velocity.magnitude > 0)
            rb.velocity = Vector2.zero;

        // push the puck 
        currentDirection = (Camera.main.ScreenToWorldPoint(pointerUp) - Camera.main.ScreenToWorldPoint(pointerDown)).normalized;
        rb.AddForce(currentDirection * forceStrength, ForceMode2D.Impulse);
        lastForceVelocity = rb.velocity;

        // pushing utils
        isPushed = true;
        timePassedSinceLastForce = 0;
    }
}
