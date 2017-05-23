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

	void Awake()
	{
		highlight = transform.Find("Highlight").gameObject;
	}

	public int GetMoney() {
		return level;
	}

	public int GetEnemyCount() {
		return Mathf.RoundToInt(Mathf.Ceil(level/3f));
	}

	public override string ToString() {
		return name+" level: "+level+" intimidated: "+intimidated;
	}

}
