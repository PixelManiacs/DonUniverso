using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    float health = 3;

    GameObject target;
    public GameObject bulletPrefab;
    public Rigidbody rigid;
	public Planet planet;
	public Ship ship;

    public GameObject explodeParticle;

    public void SetStrength(int level)
    {
        health *= level *0.5f;
    }

    void Start()
    {
        target = GameObject.FindWithTag("Player");
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
			var strength = GameObject.Find("PlayerModel").GetComponent<Ship>().weapon;
			health = Mathf.Max(0, health - strength);
            if (health <= 0)
            {
				if (ship != null)
					ship.EnemyHit(this);
				
				GameObject explosion = Instantiate(explodeParticle, transform.position, Quaternion.identity);
				target.GetComponent<Oneliners>().PlayRandomClip();
                //explosion.transform.localScale = Vector3.one * 7;
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            float attration = dir.magnitude > 20 ? 1 : -1;

            //Debug.Log(">> " + dir.magnitude);

            if (dir.magnitude > 6)
            {
                rigid.AddForce(dir * 1);
            }
            else
            {
                rigid.AddForce(Quaternion.AngleAxis(90, Vector3.up) * dir * 3 - dir * 3);
            }

            if (Time.frameCount % 120 == 0)
            {
                ShootAtPlayer();
            }
        }
    }

	void LateUpdate()
	{
        if(target != null)
        {
            transform.LookAt(target.transform);
        }
	}

    void ShootAtPlayer()
    {
        Vector3 dir = target.transform.position - transform.position;

        GameObject goButtlet = Instantiate(bulletPrefab, transform.position + dir * 0.3f, Quaternion.identity);
        Rigidbody rig = goButtlet.GetComponent<Rigidbody>();
        rig.velocity = dir * 1 + rigid.velocity;

    }
}
