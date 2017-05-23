using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipControll : MonoBehaviour
{

    public float maxHealth = 100;

    public float minCameraDistance;
    public float maxCameraDistance;
    public float speedFactor;

    public float maxSpeed;
    public float boostAmount = 5f;
    public float boostBreakAmount = 10f;

	public AudioSource pewpewAudio;

	public AudioSource boostAudio;

    float gas = 10;
    float boost = 1;
    public float health = 100; 

	public float maxFuel = 100f;
	public float fuel = 100f;
	public float fuelConsumptionPerFrame = 1f;
	public float fuelResetPerFrame = 0.3f;
	public float boostParticleSize = 4f;
    
    Rigidbody spaceShipRigidBody;

    public ParticleSystem particlesBootsLeft;
    public ParticleSystem particlesBootsRight;

    public Renderer backgroundRenderer;
    public Renderer backgroundRenderer2;
    public Renderer backgroundRenderer3;

    public Transform background;

    public Transform markerObject;

    public Transform thomson;

    public GameObject bullet;

    Plane groundPlane;

    bool miniMap;

    bool bootsActivated;
    bool bootsBrake;

    public Transform carLookAt;

	private Transform needle;
    public GameObject healthBarPrefab;
    GameObject healthBar;

    private void Awake()
    {
		needle = GameObject.Find("Needle").transform;
    }

    // Use this for initialization
    void Start()
    {
        healthBar = Instantiate(healthBarPrefab);

        groundPlane = new Plane(Vector3.up, Vector3.zero);

        spaceShipRigidBody = GetComponent<Rigidbody>();

        backgroundRenderer.material.mainTextureScale = new Vector2(52.5f, 52.5f);
        backgroundRenderer2.material.mainTextureScale = new Vector2(132.5f, 132.5f);
        backgroundRenderer3.material.mainTextureScale = new Vector2(252.75f, 252.75f);

        StartCoroutine(RefillHealth());
    }

    IEnumerator RefillHealth()
    {
        while(true)
        {
            health += maxHealth * 0.0025f;

            health = Mathf.Min(maxHealth, health);

            UpdateHealthBar();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Home")) 
        {
            return;
        }

        Debug.Log( "AUAAAAAAA! " );
        health -= 15;

        if(health < 0)
        {
            transform.position = new Vector3(10, 0, 0);
            health = maxHealth;
            Ship ship = GetComponent<Ship>();
			ship.Dead();
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.GetComponentInChildren<Image>().fillAmount = health / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
		miniMap = Input.GetKey(KeyCode.F);
		bootsActivated = Input.GetKey(KeyCode.LeftShift);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = Input.mousePosition;
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            markerObject.position = ray.GetPoint(rayDistance);
            thomson.LookAt(markerObject);
            if (Input.GetMouseButtonDown(0))
            {
                GameObject goButtlet = Instantiate(bullet, thomson.position + thomson.forward * 2, Quaternion.identity);
				pewpewAudio.Play();
                Rigidbody rig = goButtlet.GetComponent<Rigidbody>();
                rig.velocity = thomson.forward * 20;// + spaceShipRigidBody.velocity; 

                spaceShipRigidBody.AddForce(-thomson.forward * 80);
            }
        }
			
		var vertical = Input.GetAxis("Vertical");
		var forward = vertical > 0.1f;

		bool boosting = bootsActivated && forward && fuel > 0;
		float b = boosting ? boostAmount : 1f;

		//Debug.Log("fuel: "+fuel);
		if (boosting) {
			fuel -= fuelConsumptionPerFrame;
			SetParticleSystemSize(particlesBootsLeft, boostParticleSize);
			SetParticleSystemSize(particlesBootsRight, boostParticleSize);
		} else {
			SetParticleSystemSize(particlesBootsLeft, 1f);
			SetParticleSystemSize(particlesBootsRight, 1f);
		}

		if (!bootsActivated) {
			fuel = Mathf.Min(fuel + fuelResetPerFrame, maxFuel);
		}

		float rot = Map(fuel, 0f, maxFuel, 0f, 242f) - 122f;
		needle.rotation = Quaternion.Euler(new Vector3(0, 0, -rot));

        spaceShipRigidBody.AddForce(transform.forward * vertical * gas * b, ForceMode.Force);

        if (forward)
        {
            particlesBootsLeft.Emit(1);
            particlesBootsRight.Emit(1);
        }

		if (!boosting)
		{
			Vector3 breakForce = -spaceShipRigidBody.velocity;
			spaceShipRigidBody.AddForce(breakForce);
		}

		if (boosting)
		{
			if (!boostAudio.isPlaying)
			{
				boostAudio.Play();
			}
		}

		if (!boosting)
		{
			if (boostAudio.isPlaying)
			{
				boostAudio.Stop();
			}
		}


		var horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0.1f)
            particlesBootsRight.Emit(1);
        else if (horizontal < -0.1f)
            particlesBootsLeft.Emit(1);

        spaceShipRigidBody.AddTorque(Vector3.up * horizontal * 50);

        backgroundRenderer.material.mainTextureOffset = new Vector2(-transform.position.x * 0.05f, -transform.position.z * 0.05f);
        backgroundRenderer2.material.mainTextureOffset = new Vector2(-transform.position.x * 0.75f * 0.05f, -transform.position.z * 0.75f * 0.05f);
        backgroundRenderer3.material.mainTextureOffset = new Vector2(-transform.position.x * 0.5f * 0.05f, -transform.position.z * 0.5f * 0.05f);
    }

    void LateUpdate()
    {
        float dist = minCameraDistance * Mathf.Max(1, spaceShipRigidBody.velocity.magnitude * 0.25f) * speedFactor;
        float yOffset = miniMap ? maxCameraDistance : dist;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                                                      transform.position + Vector3.up * yOffset,
                                                      Time.deltaTime * 3);
        Camera.main.transform.LookAt(carLookAt);

        Vector3 backgroundPos = transform.position;
        backgroundPos.y = -10;
        background.position = backgroundPos;



    }

	private void SetParticleSystemSize(ParticleSystem system, float size) {
		var x = particlesBootsRight.main;
		x.startSizeMultiplier = size;
	}

	private static float Map(float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
