using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioClip[] soundtracks;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        soundtracks = Resources.LoadAll<AudioClip>("Music");
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    void PlayRandomTrack()
    {
        audioSource.clip = soundtracks[Random.Range(0, soundtracks.Length)];
        audioSource.Play();
    }

}
