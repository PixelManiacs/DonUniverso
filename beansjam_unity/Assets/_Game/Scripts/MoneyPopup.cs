using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays a popup on the screen showing the amount of money the player 
/// collected from a planet or spent for an upgrade.
/// </summary>
public class MoneyPopup : MonoBehaviour {

    // Played if something was bought
	public AudioClip spendMoneyClip;

    // Played if money was collected
	public AudioClip gainMoneyClip;

    // Played if money is transferred into the bank/home planet.
	public AudioClip transferMoneyClip;

    // The audio source used to play the clips.
	public AudioSource audioSource;

    // Animator object to define a transition between an idle state 
    // (no sound clip playing) and the moneypopup state (playing an audio clip).
    // This allows to have a kind of play once behaviour.
	public Animator anim;

    // GUI Text element to show the amount of money collected/spent
	public TextMesh text;


    /// <summary>
    /// Money was collected. Display the positive amount as a green text.
    /// </summary>
    /// <param name="amount">The collected amount of money.</param>
	public void IncreaseMoney(int amount)
	{
		text.text = "+$ " + amount;
		text.color = Color.green;
		audioSource.clip = gainMoneyClip;
		audioSource.Play();
		anim.SetTrigger("Pop");
	}

    /// <summary>
    /// Money was spent. Display the negative amount as a red text.
    /// </summary>
    /// <param name="amount">The spent amount of money.</param>
	public void DecreaseMoney(int amount)
	{
		text.text = "-$ " + amount;
		text.color = Color.red;
		audioSource.clip = spendMoneyClip;
		audioSource.Play();
		anim.SetTrigger("Pop");
	}

    /// <summary>
    /// Change the money of the player.
    /// Call the regarding increase/decrease methods depending on the sign of 
    /// the amount.
    /// </summary>
    /// <param name="amount">The increased/decreased amount of money.</param>
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

    /// <summary>
    /// Temporary money was transferred into the bank. 
    /// Play the regarding sound clip.
    /// </summary>
	public void Transfer()
	{
		audioSource.clip = transferMoneyClip;
		audioSource.Play();
	}
}
