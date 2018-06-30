using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Inspector Variables

    [Tooltip("The force applied every physics update.")]
    public float initialMovementImpulse = 1.0f;
    public float movementInpulseIncreaseSpeed= 1.01f;

    public Color activePlayerColor = Color.green;

    [Header("Referenced Components")]
    public Rigidbody2D playerRigidBody;
    public SpriteRenderer playerSpriteRenderer;

    #endregion

    #region Public Static Variables

    [HideInInspector]
    public static GameObject activePlayer = null;
    #endregion

    #region Private Variables

    private float currentPlayerMovementImpulse = 0.0f;
    private Vector2 playerMovementForwardDirection = Vector2.right;

    private bool bounceOccurredThisFrame = false;
    [SerializeField]
    private bool isActivePlayerCharacter = false;
    [SerializeField]
    private float playerCurrentSpeed = 0f;
    [SerializeField]
    private float playerMaxSpeed = 0f;

    private Vector3 previousPosition;

    private GameManager gameManagerInstance;


    #endregion

    #region MonoBehaviour

    private void FixedUpdate()
    {
        PlayerMovementUpdate();
    }

    private void Awake()
    {
        if(playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if(playerRigidBody == null)
        {
            Debug.LogError("playerRigidBody must not be null", this.gameObject);
        }
        currentPlayerMovementImpulse = initialMovementImpulse;
        playerMovementForwardDirection = (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))).normalized;
        
        if(activePlayer == null)
        {
            SetupPlayerCharacter();
        }
    }

    private void SetupPlayerCharacter()
    {
        activePlayer = this.gameObject;
        isActivePlayerCharacter = true;

        if(playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = activePlayerColor;
        }

        this.gameObject.name = "ActivePlayer";

        previousPosition = transform.position;
    }

    private void LateUpdate()
    {
        if(bounceOccurredThisFrame)
        {
            playerMovementForwardDirection = playerRigidBody.velocity.normalized;

            bounceOccurredThisFrame = false;
        }

        if (isActivePlayerCharacter)
        {
            playerCurrentSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;

            previousPosition = transform.position;

            if (playerCurrentSpeed > playerMaxSpeed)
            {
                playerMaxSpeed = playerCurrentSpeed;
            }

            if(gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
            {
                gameManagerInstance.UpdateScore(playerCurrentSpeed, playerMaxSpeed);
                Debug.LogFormat("Current Speed: {0}\nMax Speed: {1}", playerCurrentSpeed, playerMaxSpeed);
            }
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
        currentPlayerMovementImpulse += movementInpulseIncreaseSpeed * Time.deltaTime;

        if (isActivePlayerCharacter)
        {
            Vector2 mousePointingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            Debug.DrawRay(transform.position, mousePointingDirection.normalized);

            float mouseAngle = Mathf.Clamp(Vector2.SignedAngle(playerMovementForwardDirection, mousePointingDirection), -90f, 90f);
            //Debug.LogFormat("Mouse angle = {0}", mouseAngle);
            float turnAngleImpulseRatio = Mathf.Abs(mouseAngle / 90f);

            Vector2 turnForceDirection = Vector2Extension.Rotate(playerMovementForwardDirection, mouseAngle);

            Debug.DrawRay(transform.position, turnForceDirection.normalized * currentPlayerMovementImpulse * Time.deltaTime, Color.red);

            playerRigidBody.AddForce(turnForceDirection.normalized * turnAngleImpulseRatio * currentPlayerMovementImpulse * Time.deltaTime);
            playerMovementForwardDirection = playerRigidBody.velocity.normalized;
            Debug.DrawRay(transform.position, playerMovementForwardDirection);

            float leftOverForwardImpulse = currentPlayerMovementImpulse * (1 - turnAngleImpulseRatio);

            playerRigidBody.AddForce(playerMovementForwardDirection.normalized * leftOverForwardImpulse * currentPlayerMovementImpulse * Time.deltaTime);
            Debug.DrawRay(transform.position, playerMovementForwardDirection.normalized * leftOverForwardImpulse * currentPlayerMovementImpulse * Time.deltaTime, Color.green);
        }
        else
        {
            playerRigidBody.AddForce(playerMovementForwardDirection.normalized * currentPlayerMovementImpulse * Time.deltaTime);
            Debug.DrawRay(transform.position, playerMovementForwardDirection.normalized * currentPlayerMovementImpulse * Time.deltaTime, Color.green);
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
