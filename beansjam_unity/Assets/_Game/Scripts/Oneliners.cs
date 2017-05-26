using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oneliners : MonoBehaviour
{

    public AudioSource audioSource;
    public List<AudioClip> allClips = new List<AudioClip>();

    private int previousClipNumber = 99;

    /// <summary>
    /// Plays a random audio clip
    /// </summary>
    public void PlayRandomClip()
    {
        int randomClipNumber;

        // if the audio source is not playing
        if (!audioSource.isPlaying)
        {
            // avoid playing the same clip two times in a row
            do
            {
                // pick a random audio clip
                randomClipNumber = Random.Range(0, allClips.Count);
            } while (randomClipNumber == previousClipNumber);

            previousClipNumber = randomClipNumber;

            // assign the clip to the audio source
            audioSource.clip = allClips[randomClipNumber];
            // play the audio source
            audioSource.Play();
        }
    }
}
