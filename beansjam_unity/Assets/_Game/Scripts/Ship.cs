using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
	public float radius = 2f;

	public GameObject enemyPrefab;

	private int layerMask;
	[HideInInspector]
	public Planet activePlanet;
	[HideInInspector]
	public int tempMoney = 0;
	[HideInInspector]
	public int safeMoney = 0;

	private Planet attackingPlanet = null;
	private int attackingRemainingEnemies = 0;

	private Planet lastActivePlanet;
	private Text tempMoneyText;
	private Text safeMoneyText;

	public int weapon = 1;
	private Image intimidateText;
	private Image collectText;

    public AudioClip siren;
    public AudioSource audio;
	private MoneyPopup moneyPopup;

	private void Awake()
	{
		layerMask = 1 << LayerMask.NameToLayer("Planet");
		tempMoneyText = GameObject.Find("MoneyTemp").GetComponent<Text>();
		safeMoneyText = GameObject.Find("MoneySafe").GetComponent<Text>();
		intimidateText = GameObject.Find("IntimidateTextLabel").GetComponent<Image>();
		intimidateText.gameObject.SetActive(false);
		collectText = GameObject.Find("CollectTextLabel").GetComponent<Image>();
		collectText.gameObject.SetActive(false);
		moneyPopup = GameObject.Find("MoneyPopup").GetComponent<MoneyPopup>();
	}

	private void Update()
	{
		lastActivePlanet = activePlanet;
		Collider[] planets = Physics.OverlapSphere(transform.position, radius, layerMask);

		if (planets.Length == 0)
			activePlanet = null;
		else if (planets.Length == 1)
			activePlanet = planets[0].GetComponent<Planet>();
		else
		{
			float dist = float.MaxValue;
			Planet planet = null;
			for (int i = 0; i < planets.Length; i++)
			{
				float d = Vector3.Distance(transform.position, planets[i].transform.position);
				if (d < dist)
				{
					dist = d;
					planet = planets[i].GetComponent<Planet>();
				}
			}
			activePlanet = planet;
		}

		if (attackingPlanet == null && activePlanet != null && Input.GetKeyDown(KeyCode.E))
		{
			if (activePlanet.intimidated)
			{
				if (!activePlanet.gotMoneyInThisRound)
				{
					Debug.Log("got money " + activePlanet.GetMoney() + "$ from planet");
					tempMoney += activePlanet.GetMoney();
					moneyPopup.IncreaseMoney(activePlanet.GetMoney());
					activePlanet.gotMoneyInThisRound = true;
				}
				else
				{
					Debug.Log("already got money from planet in this round");
				}
			}
			else
			{
				Debug.Log("attack planet");

                audio.PlayOneShot(siren);

				attackingPlanet = activePlanet;
				attackingRemainingEnemies = activePlanet.GetEnemyCount();
				for (int i = 0; i < activePlanet.GetEnemyCount(); i++)
				{
					var go = Instantiate(enemyPrefab, activePlanet.transform.position + activePlanet.transform.forward * Random.Range(6, 12), Quaternion.identity);
					go.tag = "Enemy";
                    go.GetComponent<Enemy>().SetStrength(activePlanet.level);
                    go.GetComponent<Enemy>().planet = activePlanet;
                    go.GetComponent<Enemy>().ship = this;
				}
			}
		}

		UpdatePlanetHighlight();
 
		UpdateMoney();
	}

	void UpdatePlanetHighlight()
	{

		if (lastActivePlanet != null)
		{
			lastActivePlanet.highlight.SetActive(false);
		}

		if (activePlanet != null)
		{
			activePlanet.highlight.SetActive(true);
			if (activePlanet.intimidated)
			{
				Color col = Color.green;
				col.a = 0.1f;
				foreach (Material m in activePlanet.highlight.GetComponent<Renderer>().materials)
				{
					m.SetColor("_TintColor", col);
				}

				if (!activePlanet.gotMoneyInThisRound)
					collectText.gameObject.SetActive(true);
				else
					collectText.gameObject.SetActive(false);
			}
			else
			{
				Color col = Color.red;
				col.a = 0.1f;
				foreach (Material m in activePlanet.highlight.GetComponent<Renderer>().materials)
				{
					m.SetColor("_TintColor", col);
				}

				intimidateText.gameObject.SetActive(true);
			}
		}

		if (activePlanet == null)
		{
			intimidateText.gameObject.SetActive(false);
			collectText.gameObject.SetActive(false);
		}


	}

	void UpdateMoney()
	{
		tempMoneyText.text = "$" + tempMoney;
		safeMoneyText.text = "$" + safeMoney;
	}

	public void EnemyHit(Enemy enemy)
	{
		if (attackingPlanet == enemy.planet)
		{
			attackingRemainingEnemies--;

			if (attackingRemainingEnemies <= 0)
			{
				Debug.Log("attack won, planet is now intimidated");
				Debug.Log("got money " + attackingPlanet.GetMoney() + "$ from planet");
				attackingPlanet.intimidated = true;
				tempMoney += attackingPlanet.GetMoney();
				GetComponentInChildren<MoneyPopup>().IncreaseMoney(attackingPlanet.GetMoney());
				attackingPlanet.gotMoneyInThisRound = true;
				attackingPlanet = null;
			}
		}
	}

	public void Dead()
	{
		foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
			Destroy(enemy);

		tempMoney = 0;
		attackingPlanet = null;
		attackingRemainingEnemies = 0;
	}
}
