using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPopup : MonoBehaviour {

	public AudioClip spendMoneyClip;
	public AudioClip gainMoneyClip;
	public AudioClip transferMoneyClip;

	public AudioSource audioSource;

	public Animator anim;

	public TextMesh text;

	public void IncreaseMoney(int amount)
	{
		text.text = "+$ " + amount;
		text.color = Color.green;
		audioSource.clip = gainMoneyClip;
		audioSource.Play();
		anim.SetTrigger("Pop");
	}

	public void DecreaseMoney(int amount)
	{
		text.text = "-$ " + amount;
		text.color = Color.red;
		audioSource.clip = spendMoneyClip;
		audioSource.Play();
		anim.SetTrigger("Pop");
	}

	public void ChangeMoney(int amount)
	{
		if (amount > 0)
		{
			IncreaseMoney(Mathf.Abs(amount));
		}
		else if (amount < 0)
		{
			DecreaseMoney(Mathf.Abs(amount));
		}
	}

	public void Transfer()
	{
		audioSource.clip = transferMoneyClip;
		audioSource.Play();
	}
}
