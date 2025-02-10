using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody2D rb2d;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSpeed = 30f;

    Vector2 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.MoveEvent -= HandleMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        float zRotation = previousMovementInput.x * (-turnSpeed) * Time.deltaTime;
        bodyTransform.Rotate(0, 0, zRotation);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
        rb2d.velocity = bodyTransform.up * moveSpeed * previousMovementInput.y;
    }

    void HandleMove(Vector2 movement)
    {
        previousMovementInput = movement;
    }
}
