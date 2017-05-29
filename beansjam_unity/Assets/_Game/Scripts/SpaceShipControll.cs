using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipControll : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    public float health = 100;
    public GameObject healthBarPrefab;

    [Header("Camera")]
    public float minCameraDistance;
    public float maxCameraDistance;
    public float speedFactor;

    [Header("Speed")]
    public float maxSpeed;
    public float boostAmount = 5f;
    public float boostBreakAmount = 10f;

    [Header("Audio")]
    public AudioSource pewpewAudio;
    public AudioSource boostAudio;

    [Header("Boost")]
    public float maxFuel = 100f;
    public float fuel = 100f;
    public float fuelConsumptionPerFrame = 1f;
    public float fuelResetPerFrame = 0.3f;
    public float boostParticleSize = 4f;

    [Header("Particles")]
    public ParticleSystem particlesBootsLeft;
    public ParticleSystem particlesBootsRight;

    [Header("Background Renderer")]
    public Renderer backgroundRenderer;
    public Renderer backgroundRenderer2;
    public Renderer backgroundRenderer3;

    [Header("Shooting")]
    public Transform background;
    public Transform markerObject;
    public Transform thomson;
    public Transform carLookAt;
    public GameObject bullet;

    Plane groundPlane;
    bool miniMap;
    bool bootsActivated;
    bool bootsBrake;
    float gas = 10;
    float boost = 1;

    Rigidbody spaceShipRigidBody;
    private Transform needle;
    GameObject healthBar;

    /// <summary>
    /// Is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        needle = GameObject.Find("Needle").transform;
    }

    /// <summary>
    /// Is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // Create the healthbar.
        healthBar = Instantiate(healthBarPrefab);

        groundPlane = new Plane(Vector3.up, Vector3.zero);

        spaceShipRigidBody = GetComponent<Rigidbody>();

        backgroundRenderer.material.mainTextureScale = new Vector2(52.5f, 52.5f);
        backgroundRenderer2.material.mainTextureScale = new Vector2(132.5f, 132.5f);
        backgroundRenderer3.material.mainTextureScale = new Vector2(252.75f, 252.75f);

        // Start time controlled Coroutine.
        StartCoroutine(RefillHealth());
    }

    /// <summary>
    /// Refills the health slowly
    /// </summary>
    /// <returns></returns>
    IEnumerator RefillHealth()
    {
        while (true)
        {
            // Adding 0,25% to the player's health.
            health += maxHealth * 0.0025f;
            // If health is more than max health set it to the value of max health.
            health = Mathf.Min(maxHealth, health);

            UpdateHealthBar();

            // Coroutine returns after half a second.
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Is called when this collider has begun touching another collider.
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        // If the other Gameobject is the home planet, get outta here.
        if (other.gameObject.layer == LayerMask.NameToLayer("Home"))
            return;

        Debug.Log("AUAAAAAAA!");
        health -= 15;

        // If player hs no health anymore.
        if (health < 0)
        {
            // Reset the position of the ship.
            transform.position = new Vector3(10, 0, 0);
            // Reset the health.
            health = maxHealth;
            Ship ship = GetComponent<Ship>();
            // Tell the ship that it is dead.
            ship.Dead();
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// Updates the health bar.
    /// </summary>
    public void UpdateHealthBar()
    {
        healthBar.GetComponentInChildren<Image>().fillAmount = health / maxHealth;
    }

    /// <summary>
    /// Is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = Input.mousePosition;
        Vector3 breakForce;
        float rayDistance;
        bool boosting;
        float b, rot;

        // Zoom out when 'F' is pressed.
        miniMap = Input.GetKey(KeyCode.F);
        // Activate boost when 'LeftShift' is pressed.
        bootsActivated = Input.GetKey(KeyCode.LeftShift);

        // Raycast for aiming.
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // Set the position of the "Crosshair".
            markerObject.position = ray.GetPoint(rayDistance);
            thomson.LookAt(markerObject);

            // If 'LeftMosuseButton' is pressed.
            if (Input.GetMouseButtonDown(0))
            {
                // Create the bullet.
                GameObject goButtlet = Instantiate(bullet, thomson.position + thomson.forward * 2, Quaternion.identity);
                Rigidbody rig = goButtlet.GetComponent<Rigidbody>();
                // Play the 'pew pew' sound.
                pewpewAudio.Play();
                // Give the bullet a velocity.
                rig.velocity = thomson.forward * 20;// + spaceShipRigidBody.velocity; 

                // Knockback when firing.
                spaceShipRigidBody.AddForce(-thomson.forward * 80);
            }
        }

        var vertical = Input.GetAxis("Vertical");
        var forward = vertical > 0.1f;

        // Only boost if 'LeftShift' is pressed and the ship flies forward and when the ship has enough fuel.
        boosting = bootsActivated && forward && fuel > 0;
        b = boosting ? boostAmount : 1f;

        //Debug.Log("fuel: "+fuel);
        if (boosting)
        {
            // Remove the fuel consumption.
            fuel -= fuelConsumptionPerFrame;

            // Set the particle size of the boost.
            SetParticleSystemSize(particlesBootsLeft, boostParticleSize);
            SetParticleSystemSize(particlesBootsRight, boostParticleSize);

            // Play the boost sound if it isn't already playing.
            if (!boostAudio.isPlaying)
                boostAudio.Play();
        }
        else {
            // Set the particle size of the boost to normal.
            SetParticleSystemSize(particlesBootsLeft, 1f);
            SetParticleSystemSize(particlesBootsRight, 1f);
            
            // Decrease the velocity of the ship.
            breakForce = -spaceShipRigidBody.velocity;
            spaceShipRigidBody.AddForce(breakForce);

            // Stop the boost sound if it's playing.
            if (boostAudio.isPlaying)
                boostAudio.Stop();
        }

        // If 'LeftShit' is not pressed, refill the fuel.
        if (!bootsActivated)
            fuel = Mathf.Min(fuel + fuelResetPerFrame, maxFuel);
        
        // Calculate the rotation of the needle.
        rot = Map(fuel, 0f, maxFuel, 0f, 242f) - 122f;
        // Update the rotation of the needle.
        needle.rotation = Quaternion.Euler(new Vector3(0, 0, -rot));

        // Add force to the ship so it's dragging.
        spaceShipRigidBody.AddForce(transform.forward * vertical * gas * b, ForceMode.Force);

        // If the player controls the ship to fly forward.
        if (forward)
        {
            particlesBootsLeft.Emit(1);
            particlesBootsRight.Emit(1);
        }
        
        // Steering the ship.
        var horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0.1f)
            particlesBootsRight.Emit(1);
        else if (horizontal < -0.1f)
            particlesBootsLeft.Emit(1);

        // Rotate the ship.
        spaceShipRigidBody.AddTorque(Vector3.up * horizontal * 50);

        backgroundRenderer.material.mainTextureOffset = new Vector2(-transform.position.x * 0.05f, -transform.position.z * 0.05f);
        backgroundRenderer2.material.mainTextureOffset = new Vector2(-transform.position.x * 0.75f * 0.05f, -transform.position.z * 0.75f * 0.05f);
        backgroundRenderer3.material.mainTextureOffset = new Vector2(-transform.position.x * 0.5f * 0.05f, -transform.position.z * 0.5f * 0.05f);
    }

    /// <summary>
    /// Is called every frame after all Update functions.
    /// </summary>
    void LateUpdate()
    {
        // Calculate the distance of the camera to the ship.
        float dist = minCameraDistance * Mathf.Max(1, spaceShipRigidBody.velocity.magnitude * 0.25f) * speedFactor;
        // Zoom out when 'F' is pressed.
        float yOffset = miniMap ? maxCameraDistance : dist;
        // Move the camera.
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                                                      transform.position + Vector3.up * yOffset,
                                                      Time.deltaTime * 3);
        // Look at the ship.
        Camera.main.transform.LookAt(carLookAt);
        Vector3 backgroundPos = transform.position;

        backgroundPos.y = -10;
        background.position = backgroundPos;
    }

    /// <summary>
    /// Sets the particle size
    /// </summary>
    /// <param name="system">Particle system.</param>
    /// <param name="size">New size of the particles.</param>
    private void SetParticleSystemSize(ParticleSystem system, float size)
    {
        var x = particlesBootsRight.main;
        x.startSizeMultiplier = size;
    }

    private static float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
