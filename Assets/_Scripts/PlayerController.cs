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
        Vector2 mousePointingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Debug.DrawRay(transform.position, mousePointingDirection.normalized);

        float mouseAngle = Mathf.Clamp(Vector2.SignedAngle(playerMovementForwardDirection, mousePointingDirection), -90f, 90f);
        Debug.LogFormat("Mouse angle = {0}", mouseAngle);
        float turnAngleImpulseRatio = Mathf.Abs(mouseAngle / 90f);

        Vector2 turnForceDirection = Vector2Extension.Rotate(playerMovementForwardDirection, mouseAngle);

        Debug.DrawRay(transform.position, turnForceDirection.normalized * currentPlayerMovementImpulse * Time.deltaTime, Color.red);

        playerRigidBody.AddForce(turnForceDirection.normalized * turnAngleImpulseRatio * currentPlayerMovementImpulse * Time.deltaTime);
        playerMovementForwardDirection = playerRigidBody.velocity.normalized;
        Debug.DrawRay(transform.position, playerMovementForwardDirection);

        float leftOverForwardImpulse = currentPlayerMovementImpulse * (1 - turnAngleImpulseRatio) * 0.5f + 0.5f;

        playerRigidBody.AddForce(playerMovementForwardDirection.normalized * leftOverForwardImpulse * currentPlayerMovementImpulse * Time.deltaTime);
        Debug.DrawRay(transform.position, playerMovementForwardDirection.normalized * leftOverForwardImpulse * currentPlayerMovementImpulse * Time.deltaTime, Color.green);
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
