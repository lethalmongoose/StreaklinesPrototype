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
    private float currentPlayerMovementImpulse = 0.0f;
    private Vector2 playerMovementForwardDirection = Vector2.right;

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
        playerMovementForwardDirection = (new Vector2(Random.RandomRange(-1f, 1f), Random.RandomRange(-1f, 1f))).normalized;
        
    }

    private void Update()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y), playerMovementForwardDirection);
    }

    private void PlayerMovementUpdate()
    {
        playerRigidBody.AddForce(playerMovementForwardDirection * currentPlayerMovementImpulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag == "Wall Bounce")
        {
            Debug.Log("Change direction");
            ContactPoint2D hitContactPoint = collision.contacts[0];
            Vector2 impactDirection = hitContactPoint.point - (Vector2)transform.position;
            playerMovementForwardDirection = Vector2.Reflect(impactDirection, hitContactPoint.normal);
        }
    }
    #endregion
}
