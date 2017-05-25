using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the behavior of a bullet shot by the player or an enemy.
/// </summary>
public class Bullet : MonoBehaviour {

    // The GameObject used for the explosion
    public GameObject explodeParticle;
    
    void Start()
    {
        // Destroy the bullet after 10 seconds
        Destroy(gameObject, 10);
    }

    /// <summary>
    /// Called if the bullet hits an object.
    /// Destroys the bullet game object.
    /// Instantiates the explosion game object and destroys it after 3 seconds.
    /// </summary>
    /// <param name="other">The game object the bullet collided with.</param>
    void OnCollisionEnter(Collision other)
    {
        GameObject explosion = Instantiate(explodeParticle, transform.position, Quaternion.identity);
        Destroy(explosion, 3);
        Destroy(gameObject);
    }
}
