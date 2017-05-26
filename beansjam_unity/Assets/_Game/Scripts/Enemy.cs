using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the behavior of an enemy.
/// An enemy follows the player and shoots a bullet every 120 frames.
/// </summary>
public class Enemy : MonoBehaviour
{
    // Prefab for the bullet
    public GameObject bulletPrefab;
    
    // The rigidbody component of the enemy game object
    public Rigidbody rigid;

    // Reference of the planet this enemy belongs to
	public Planet planet;

    // The ship script of the player ship
	public Ship ship;

    // Prefab for the explosion particles
    public GameObject explodeParticle;

    // The health of the enemy
    float health = 3;

    // The target of the enemy i.e. the player
    GameObject target;

    // The strength of the players weapon
    int weaponStrength;

    // multiply the health by the half the level
    public void SetStrength(int level)
    {
        health *= level * 0.5f;
    }

    void Start()
    {
        // initialize the target reference aka the player
        target = GameObject.FindWithTag("Player");
        weaponStrength = target.GetComponent<Ship>().weapon;
    }

    void OnCollisionEnter(Collision other)
    {
        // if the enemy was hit by a bullet
        if(other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            // decrease the health by the strength of the players weapon
			health -= weaponStrength;

            // if the enemy has no health left
            if (health <= 0)
            {
                // tell the player ship that the enemy was hit/destroyed
				if (ship != null)
					ship.EnemyHit(this);
				
                // instantiate the explosion particles
				GameObject explosion = Instantiate(explodeParticle, transform.position, Quaternion.identity);

                // play a one liner sound
				target.GetComponent<Oneliners>().PlayRandomClip();

                // destroy the enemy game object
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // calculate the direction vector to the players ship
            Vector3 dir = target.transform.position - transform.position;

            // if the player is more than 6 units away
            if (dir.magnitude > 6)
            {
                // fly towards the player
                rigid.AddForce(dir * 1);
            }
            else
            {
                // circle around the player
                rigid.AddForce(Quaternion.AngleAxis(90, Vector3.up) * dir * 3 - dir * 3);
            }

            // every 120th frame shoot at the player
            if (Time.frameCount % 120 == 0)
            {
                ShootAtPlayer(dir);
            }
        }
    }

	void LateUpdate()
	{
        if(target != null)
        {
            // turn towards the player
            transform.LookAt(target.transform);
        }
	}

    void ShootAtPlayer(Vector3 dir)
    {
        // instantiate a bullet and accelerate it
        GameObject goButtlet = Instantiate(bulletPrefab, transform.position + dir * 0.3f, Quaternion.identity);
        Rigidbody rig = goButtlet.GetComponent<Rigidbody>();
        rig.velocity = dir * 1;
    }
}
