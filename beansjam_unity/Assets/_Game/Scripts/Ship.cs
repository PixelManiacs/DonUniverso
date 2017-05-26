using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    // Radius of the circle around the ship in which the player can intimidate/collect.
    public float radius = 2f;
    public GameObject enemyPrefab;
    // Nearest planet (highlighted in green or red).
    [HideInInspector]
    public Planet activePlanet;
    // Money stored in the ship.
    [HideInInspector]
    public int tempMoney = 0;
    // Money stored in the home planet.
    [HideInInspector]
    public int safeMoney = 0;
    // Strength of the player's weapon.
    public int weapon = 1;
    // Audio clip for the siren of the enemies.
    public AudioClip siren;
    public AudioSource audio;

    // The planet the player is intimidating.
    private Planet attackingPlanet = null;
    // Remaining enemies for the planet the player is intimidating.
    private int attackingRemainingEnemies = 0;
    // Last active planet to "unhighlight" it.
    private Planet lastActivePlanet;
    // UI text for the money stored in the ship.
    private Text tempMoneyText;
    // UI text for the money stored in the home planet.
    private Text safeMoneyText;
    // UI text which tells the player he can intimidate.
    private Image intimidateText;
    // UI text which tells the player he can collect money.
    private Image collectText;
    private MoneyPopup moneyPopup;
    private int layerMask;

    /// <summary>
    /// Is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Initialize
        layerMask = 1 << LayerMask.NameToLayer("Planet");
        tempMoneyText = GameObject.Find("MoneyTemp").GetComponent<Text>();
        safeMoneyText = GameObject.Find("MoneySafe").GetComponent<Text>();
        intimidateText = GameObject.Find("IntimidateTextLabel").GetComponent<Image>();
        intimidateText.gameObject.SetActive(false);
        collectText = GameObject.Find("CollectTextLabel").GetComponent<Image>();
        collectText.gameObject.SetActive(false);
        moneyPopup = GameObject.Find("MoneyPopup").GetComponent<MoneyPopup>();
    }

    /// <summary>
    /// Is called every frame.
    /// </summary>
	private void Update()
    {
        Collider[] planets = Physics.OverlapSphere(transform.position, radius, layerMask);

        // Save the active planet to "unhighlight" him later.
        lastActivePlanet = activePlanet;

        // If there is no planet.
        if (planets.Length == 0)
            // There is no active planet.
            activePlanet = null;
        // If there is only one planet.
        else if (planets.Length == 1)
            // Set this planet as active
            activePlanet = planets[0].GetComponent<Planet>();
        // When there are more planets
        else
        {
            // Distance to the actual nearest planet
            float dist = float.MaxValue;
            // Actual nearest planet.
            Planet planet = null;

            // Walk through all planets.
            for (int i = 0; i < planets.Length; i++)
            {
                // Distance to the actual planet.
                float d = Vector3.Distance(transform.position, planets[i].transform.position);

                // If the distance to the actual planet is less than the distance to the actual NEAREST planet.
                if (d < dist)
                {
                    dist = d;
                    planet = planets[i].GetComponent<Planet>();
                }
            }

            // Set the actual nearest planet active.
            activePlanet = planet;
        }

        // If the player is not attacking a planet and there is an active planet and the player is pressing 'E'.
        if (attackingPlanet == null && activePlanet != null && Input.GetKeyDown(KeyCode.E))
        {
            // If the active planet is already intimidated.
            if (activePlanet.intimidated)
            {
                // And the player didn't get money from this planet in this round.
                if (!activePlanet.gotMoneyInThisRound)
                {
                    // Debug output.
                    Debug.Log("got money " + activePlanet.GetMoney() + "$ from planet");

                    // Get the money of the planet into the ship.
                    tempMoney += activePlanet.GetMoney();
                    // Show the money popup for increasing money.
                    moneyPopup.IncreaseMoney(activePlanet.GetMoney());
                    // Save that the player already got money from this planet in this round.
                    activePlanet.gotMoneyInThisRound = true;
                }
                else
                {
                    // Debug output.
                    Debug.Log("already got money from planet in this round");
                }
            }
            else
            {
                // Debug output.
                Debug.Log("attack planet");
                
                // Play the siren.
                audio.PlayOneShot(siren);

                // The active planet is now attacking.
                attackingPlanet = activePlanet;
                // Determine the number of enemies.
                attackingRemainingEnemies = activePlanet.GetEnemyCount();

                Debug.Log("Level: " + activePlanet.level + ", Enemies: " + attackingRemainingEnemies);

                // Walk through the loop to spawn the enemies.
                for (int i = 0; i < attackingPlanet.GetEnemyCount(); i++)
                {
                    // Spawn the enemy randomly around the planet.
                    var go = Instantiate(enemyPrefab, attackingPlanet.transform.position + attackingPlanet.transform.forward * Random.Range(6, 12), Quaternion.identity);

                    // Initialize the Enemy Gameobject.
                    go.tag = "Enemy";
                    // Set the strength of the enemy depending on the level of his planet.
                    go.GetComponent<Enemy>().SetStrength(attackingPlanet.level);
                    go.GetComponent<Enemy>().planet = attackingPlanet;
                    go.GetComponent<Enemy>().ship = this;
                }
            }
        }

        // Update the highlighting of the active and last active planet.
        UpdatePlanetHighlight();
        // Update the money.
        UpdateMoney();
    }

    void UpdatePlanetHighlight()
    {
        // Unhighlight the last active planet to avoid a second highlight.
        if (lastActivePlanet != null)
            lastActivePlanet.highlight.SetActive(false);

        // If there is an active planet.
        if (activePlanet != null)
        {
            // Highlight the active planet.
            activePlanet.highlight.SetActive(true);

            // If the active planet is already intimidated
            if (activePlanet.intimidated)
            {
                Color col = Color.green;

                col.a = 0.1f;

                // Give the highlight the corresponding color.
                foreach (Material m in activePlanet.highlight.GetComponent<Renderer>().materials)
                {
                    m.SetColor("_TintColor", col);
                }

                // Remove the intimidate text to prevent overlap.
                intimidateText.gameObject.SetActive(false);

                if (!activePlanet.gotMoneyInThisRound)
                    collectText.gameObject.SetActive(true);
                else
                    collectText.gameObject.SetActive(false);
            }
            else
            {
                Color col = Color.red;

                col.a = 0.1f;

                // Give the highlight the corresponding color.
                foreach (Material m in activePlanet.highlight.GetComponent<Renderer>().materials)
                {
                    m.SetColor("_TintColor", col);
                }

                // Remove the collect text to prevent overlap.
                collectText.gameObject.SetActive(false);
                // Tell the player that he is able to intimidate the planet.
                intimidateText.gameObject.SetActive(true);
            }
        }

        // If the player can't intimidate or collect, remove the text.
        if (activePlanet == null)
        {
            intimidateText.gameObject.SetActive(false);
            collectText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the money if necessary.
    /// </summary>
	void UpdateMoney()
    {
        tempMoneyText.text = "$" + tempMoney;
        safeMoneyText.text = "$" + safeMoney;
    }

    public void EnemyHit(Enemy enemy)
    {
        if (attackingPlanet == enemy.planet)
        {
            // Decrease the remaining enemies for the planet by 1.
            attackingRemainingEnemies--;

            // If there are no more enemies.
            if (attackingRemainingEnemies <= 0)
            {
                // Some debug output.
                Debug.Log("attack won, planet is now intimidated");
                Debug.Log("got money " + attackingPlanet.GetMoney() + "$ from planet");

                // Set the planet as intimidated.
                attackingPlanet.intimidated = true;
                // Give the player the money.
                tempMoney += attackingPlanet.GetMoney();
                // Show the Money-Popup.
                GetComponentInChildren<MoneyPopup>().IncreaseMoney(attackingPlanet.GetMoney());
                // Did the player get money from this planet in this round?
                attackingPlanet.gotMoneyInThisRound = true;
                // Reset the attacking planet.
                attackingPlanet = null;
            }
        }
    }

    /// <summary>
    /// player is dead
    /// </summary>
	public void Dead()
    {
        // Destroy all enemies.
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(enemy);

        // Reset the money stored in the ship.
        tempMoney = 0;
        // Reset the attacking planet.
        attackingPlanet = null;
        // Reset the enemies counter.
        attackingRemainingEnemies = 0;
    }
}
