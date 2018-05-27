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

    #region MonoBehaviour

    private void FixedUpdate()
    {
        PlayerMovementUpdate();
    }

    private void PlayerMovementUpdate()
    {

    }

#endregion
}
