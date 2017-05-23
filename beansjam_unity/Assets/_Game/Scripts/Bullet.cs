using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject explodeParticle;

    void Start()
    {
        Destroy(gameObject, 10);
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject explosion = Instantiate(explodeParticle, transform.position, Quaternion.identity);
        Destroy(explosion, 3);
        Destroy(gameObject);
    }
}
