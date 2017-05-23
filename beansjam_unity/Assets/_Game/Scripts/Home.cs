using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour {
	public float radius = 10f;
	private int layerMask;
	private bool shipInBase;
	private Ship ship;
	public int boostPrice = 4;
	public int shieldPrice = 4;
	public int weaponPrice = 4;
	private Text boostPriceText;
	private Text shieldPriceText;
	private Text weaponPriceText;
	private CanvasGroup canvasGroup;
	private MoneyPopup moneyPopup;

	private void Awake() {
		layerMask = 1 << LayerMask.NameToLayer("Ship");

		boostPriceText = GameObject.Find("BoostPrice").GetComponent<Text>();
		shieldPriceText = GameObject.Find("ShieldPrice").GetComponent<Text>();
		weaponPriceText = GameObject.Find("WeaponPrice").GetComponent<Text>();

		ship = GameObject.Find("PlayerModel").GetComponent<Ship>();
		canvasGroup = GameObject.Find("ShopCanvas").GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;

		moneyPopup = GameObject.Find("MoneyPopup").GetComponent<MoneyPopup>();
	}

	private void Update() {
		ship.safeMoney = Mathf.Max(0, ship.safeMoney);
		RenderShop();

		Collider[] ships = Physics.OverlapSphere(transform.position, radius, layerMask);
		if (ships.Length == 0) {
			if (shipInBase) ShipLeave();
			shipInBase = false;
			return;
		}
			
		if (!shipInBase) ShipEnter();
		shipInBase = true;

		if (ship.tempMoney > 0) {
			moneyPopup.Transfer();
			ship.safeMoney += ship.tempMoney;
			ship.tempMoney = 0;
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
			BuyShield();
		
		if (Input.GetKeyDown(KeyCode.Alpha2))
			BuyBoost();

		if (Input.GetKeyDown(KeyCode.Alpha3))
			BuyWeapon();
	}

	private void ShipEnter() {
		canvasGroup.alpha = 1;

		var planets = GameObject.FindGameObjectsWithTag("Planet");
		foreach (var planet in planets) {
			planet.GetComponent<Planet>().gotMoneyInThisRound = false;
		}
	}

	private void ShipLeave() {
		canvasGroup.alpha = 0;
	}

	private void PlayChaChing(int money) {
		Debug.Log("saved " + money + "$");
	}

	private void NotEnoughMoney() {
		Debug.Log("not enough money");
	}

	private void BuyBoost() {
		if (ship.safeMoney < boostPrice) {
			NotEnoughMoney();
			return;
		}

		ship.safeMoney = ship.safeMoney - boostPrice;
		moneyPopup.DecreaseMoney(boostPrice);
		boostPrice *= 2;

		var control = ship.GetComponent<SpaceShipControll>();
		control.maxFuel *= 2;
		control.fuel = control.maxFuel;
		control.fuelResetPerFrame *= 2;
		Debug.Log("boost upgraded");
	}

	private void BuyShield() {
		if (ship.safeMoney < shieldPrice) {
			NotEnoughMoney();
			return;
		}

		ship.safeMoney = ship.safeMoney - shieldPrice;
		moneyPopup.DecreaseMoney(shieldPrice);
		shieldPrice *= 2;

		var control = ship.GetComponent<SpaceShipControll>();
        control.maxHealth *= 2;
        control.health = control.maxHealth;
        control.UpdateHealthBar();

		Debug.Log("shield upgraded");
	}

	private void BuyWeapon() {
		if (ship.safeMoney < weaponPrice) {
			NotEnoughMoney();
			return;
		}

		ship.safeMoney = ship.safeMoney - weaponPrice;
		moneyPopup.DecreaseMoney(weaponPrice);
		weaponPrice *= 2;

		var control = ship.GetComponent<SpaceShipControll>();
		ship.weapon *= 2;

		Debug.Log("weapon upgraded");
	}

	private void RenderShop() {
		var green = new Color(89/255f, 170/255f, 29/255f);
		var red = new Color(224/255f, 47/255f, 71/255f);

		boostPriceText.text = "$"+boostPrice;
		boostPriceText.color = ship.safeMoney >= boostPrice ? green : red;
		shieldPriceText.text = "$"+shieldPrice;
		shieldPriceText.color = ship.safeMoney >= shieldPrice ? green : red;
		weaponPriceText.text = "$"+weaponPrice;
		weaponPriceText.color = ship.safeMoney >= weaponPrice ? green : red;
	}
}
