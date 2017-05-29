using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for the home planet.
/// It defines the upgrading system and stores the collected money in the "bank".
/// </summary>
public class Home : MonoBehaviour {

    // The radius in which the script searches for the players ship
	public float radius = 10f;

    // Prices for the ship upgrades
	public int boostPrice = 4;
	public int shieldPrice = 4;
	public int weaponPrice = 4;

    // The GUI Text Elements to display the prices
	public Text boostPriceText;
	public Text shieldPriceText;
	public Text weaponPriceText;

	// The GUI Canvas element for the shop. This contains the Text elements
	// for the ship upgrades
	public CanvasGroup canvasGroup;

	// Reference to the player game object to retrieve the ship script component
	public GameObject goPlayer;

    // Reference to the money popup game object to retrieve the money popup
    // script
    public GameObject goMoneyPopup;

    // A layer mask, which is specified to check only for the players ship
	private int layerMask;

    // Indicates if the ship is currently in the base i.e. in the radius 
    // defined above around the home planet.
	private bool shipInBase;

    // The player ship
	private Ship ship;
	
    // The popup text element which is shown if money is collected or spent
    private MoneyPopup moneyPopup;


    /// <summary>
    /// Awake the instance i.e. initialize the class attributes.
    /// - Set up the layer mask
    /// - Retrieve the needed scripts from the gameobjects
    /// </summary>
	private void Awake() {
		layerMask = 1 << LayerMask.NameToLayer("Ship");

		ship = goPlayer.GetComponent<Ship>();
        moneyPopup = goMoneyPopup.GetComponent<MoneyPopup>();

		canvasGroup.alpha = 0;
	}

    /// <summary>
    /// Update this instance:
    /// - Handle money transaction: If ship is in range store money in the bank
    /// - Render the shop
    /// - Check keyboard inputs
    /// </summary>
	private void Update() {
        HandleMoneyTransaction();
		RenderShop();
        CheckInputs();
	}

    /// <summary>
    /// Handles the money transaction.
    /// </summary>
    private void HandleMoneyTransaction() {
        // check for a ship collider in the radius around the planet
		Collider[] ships = Physics.OverlapSphere(transform.position, radius, 
                                                 layerMask);

        // no ships in range
		if (ships.Length == 0)
		{
            // if the ship was in base before --> it left
            if (shipInBase)
                ShipLeave();
            
			shipInBase = false;
			return;
		}

        // if we found a ship collider and it was not in base before
        // --> it entered
		if (!shipInBase) 
            ShipEnter();
        
		shipInBase = true;

        // if it has temporary money, then transfer it into the bank
		if (ship.tempMoney > 0)
		{
			moneyPopup.Transfer();
			ship.safeMoney += ship.tempMoney;
			ship.tempMoney = 0;
		}
    }

    /// <summary>
    /// The ship entered the radius of the home planet.
    /// </summary>
	private void ShipEnter() {
        // show the shop
		canvasGroup.alpha = 1;

        // reset all planets, i.e. allow the player to collect money again
		var planets = GameObject.FindGameObjectsWithTag("Planet");
		foreach (var planet in planets) {
			planet.GetComponent<Planet>().gotMoneyInThisRound = false;
		}
	}

    /// <summary>
    /// The ship has left the influence area of the home planet.
    /// </summary>
	private void ShipLeave() {
        // Hide the shop
		canvasGroup.alpha = 0;
	}

    /// <summary>
    /// The player has not enough money to buy an upgrade.
    /// Currently it only shows a debug message.
    /// This could be the place for your advertising banner :P or just a neat 
    /// sound or something else.
    /// </summary>
	private void NotEnoughMoney() {
		Debug.Log("not enough money");
	}

    /// <summary>
    /// Buys a boost upgrade if the player has enough money in the bank.
    /// </summary>
	private void BuyBoost() {
        // Not enough money to buy --> return
		if (ship.safeMoney < boostPrice) {
			NotEnoughMoney();
			return;
		}

        // buy the boost, double the cost for the next boost upgrade
		ship.safeMoney = ship.safeMoney - boostPrice;
		moneyPopup.DecreaseMoney(boostPrice);
		boostPrice *= 2;

        // upgrade the ship by doubling the boost fuel
		var control = ship.GetComponent<SpaceShipControll>();
		control.maxFuel *= 2;
		control.fuel = control.maxFuel;
		control.fuelResetPerFrame *= 2;

		Debug.Log("boost upgraded");
	}

    /// <summary>
    /// Buys a shield upgrade if the player has enough money in the bank.
    /// </summary>
	private void BuyShield() {
        // Not enough money to buy --> return
		if (ship.safeMoney < shieldPrice) {
			NotEnoughMoney();
			return;
		}

        // buy the shield, double the cost for the next shield upgrade
		ship.safeMoney = ship.safeMoney - shieldPrice;
		moneyPopup.DecreaseMoney(shieldPrice);
		shieldPrice *= 2;

        // upgrade the ship by doubling the maximum health
		var control = ship.GetComponent<SpaceShipControll>();
        control.maxHealth *= 2;
        control.health = control.maxHealth;
        control.UpdateHealthBar();

		Debug.Log("shield upgraded");
	}

    /// <summary>
    /// Buys a weapon upgrade if the player has enough money in the bank.
    /// </summary>
	private void BuyWeapon() {
        // Not enough money to buy --> return
		if (ship.safeMoney < weaponPrice) {
			NotEnoughMoney();
			return;
		}

        // buy the weapons, double the cost for the next weapon upgrade
		ship.safeMoney = ship.safeMoney - weaponPrice;
		moneyPopup.DecreaseMoney(weaponPrice);
		weaponPrice *= 2;

        // upgrade the ship by doubling the weapon power
		var control = ship.GetComponent<SpaceShipControll>();
		ship.weapon *= 2;

		Debug.Log("weapon upgraded");
	}

    /// <summary>
    /// Renders the shop.
    /// The prices for items which can be bought are displayed in green.
    /// The prices for items for which the player has not enough money are 
    /// shown in red.
    /// </summary>
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


    /// <summary>
    /// Checks if the player pressed the key for the regarding shop item.
    /// </summary>
    private void CheckInputs() {
		if (Input.GetKeyDown(KeyCode.Alpha1))
			BuyShield();

		if (Input.GetKeyDown(KeyCode.Alpha2))
			BuyBoost();

		if (Input.GetKeyDown(KeyCode.Alpha3))
			BuyWeapon();
    }
}
