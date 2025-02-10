using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLifetime : MonoBehaviour
{
    [SerializeField] float lifetime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    } 
}
