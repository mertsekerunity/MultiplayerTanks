using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{

    [SerializeField] InputReader inputReader;
    [SerializeField] Transform towerTransform;

    // Update is called once per frame
    void LateUpdate()
    {
        if (!IsOwner) {  return; }

        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        towerTransform.up = new Vector2(aimWorldPosition.x - towerTransform.position.x,
                                aimWorldPosition.y - towerTransform.position.y);
    }
}
