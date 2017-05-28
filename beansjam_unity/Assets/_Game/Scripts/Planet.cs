using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
	public int level;
	public string name;
	public bool intimidated;

    public bool defenseSend;
	public bool gotMoneyInThisRound;

	public GameObject highlight;

    /// <summary>
    /// Is called when the script instance is being loaded.
    /// </summary>
	void Awake()
	{
		highlight = transform.Find("Highlight").gameObject;
	}

    // Determine how much money the player gets.
	public int GetMoney() {
		return level;
	}

    // Determine the number of enemies that spawn around a planet.
	public int GetEnemyCount() {
		return Mathf.RoundToInt(Mathf.Ceil(level/3f));
	}

    /// <summary>
    /// Debug output
    /// </summary>
    /// <returns></returns>
	public override string ToString() {
		return name+" level: "+level+" intimidated: "+intimidated;
	}

}
