using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oneliners : MonoBehaviour
{

	public AudioSource audioSource;
	public List<AudioClip> allClips = new List<AudioClip>();

	private List<AudioClip> clipsToUse = new List<AudioClip>();
	private int previousClip = 99;

	public void PlayRandomClip()
	{
		if (audioSource.isPlaying)
		{
			return;
		}

		clipsToUse.Clear();
		clipsToUse = new List<AudioClip>(allClips);

		if (previousClip < allClips.Count)
		{
			clipsToUse.RemoveAt(previousClip);
		}

		int randomClip = Random.Range(0, clipsToUse.Count);
		previousClip = randomClip;

		audioSource.clip = clipsToUse[randomClip];
		audioSource.Play();
	}
}
