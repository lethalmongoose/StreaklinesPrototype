using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Tooltip("The force applied every physics update.")]
    public float initialMovementImpulse = 1.0f;
    public float movementInpulseIncreaseRatio = 1.01f;

    [Header("Referenced Components")]
    public Rigidbody2D playerRigidBody;

    #region Private Variables

    private float currentPlayerMovementImpulse = 0.0f;
    private Vector2 playerMovementForwardDirection = Vector2.right;

    private float currentVelocityMagnitude = 0f;
    private bool bounceOccurredThisFrame = false;

    #endregion

    #region MonoBehaviour

    private void FixedUpdate()
    {
        PlayerMovementUpdate();
    }

    private void Start()
    {
        if(playerRigidBody == null)
        {
            Debug.LogError("playerRigidBody must not be null", this.gameObject);
        }
        currentPlayerMovementImpulse = initialMovementImpulse;
        playerMovementForwardDirection = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
        
    }

    private void Update()
    {

        //Save current velocity in case Unity physics collision changes it
        currentVelocityMagnitude = playerRigidBody.velocity.magnitude;

        Debug.DrawRay(transform.position, (Vector2)transform.position + playerMovementForwardDirection);
    }

    private void LateUpdate()
    {
        if(bounceOccurredThisFrame)
        {
            playerMovementForwardDirection = playerRigidBody.velocity.normalized;

            bounceOccurredThisFrame = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().Die();
            Die();
        }
        else if(collision.gameObject.tag == "Wall Bounce")
        {
            bounceOccurredThisFrame = true;
        }
    }
    #endregion

    private void PlayerMovementUpdate()
    {
        playerRigidBody.AddForce(playerMovementForwardDirection * currentPlayerMovementImpulse);
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
