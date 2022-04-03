using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    [SerializeField]
    AudioSource source1;
    [SerializeField]
    AudioSource source2;

    public AudioClip nextMusic = null;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.gameManager.musicHandler = this;
    }

    public void SetMusic(bool enabled)
    {
        source1.enabled = enabled;
        source2.enabled = enabled;
        source2.Stop();
    }

    public void SetTrack(AudioClip track)
    {
        if (source1.clip != track)
        {
            nextMusic = track;
            StartCoroutine(SwapTrack());
        }
    }
    public void SetTrackInstant(AudioClip track)
    {
        source1.clip = track;
        source1.Play();
    }
    
    IEnumerator SwapTrack()
    {
        source2.clip = nextMusic;
        Debug.Log("Perparing to swap music in " + (source1.clip.length - source1.time) + "seconds");
        yield return new WaitForSeconds(source1.clip.length - source1.time);
        Debug.Log("swapping music");
        source2.Play();
        source1.Stop();
        AudioSource temp = source1;
        source1 = source2;
        source2 = temp;
    }
}
