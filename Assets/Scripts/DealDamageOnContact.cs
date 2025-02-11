using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] int damage = 10;

    ulong ownerClientId;

    public void SetOwner(ulong clientId)
    {
        this.ownerClientId = clientId;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.attachedRigidbody == null) { return; }

        if (collider.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
        {
            if (ownerClientId == networkObject.OwnerClientId) { return; }

        }

        if (collider.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
