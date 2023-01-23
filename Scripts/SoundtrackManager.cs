using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundtrackManager : MonoBehaviour {

    public GameObject[] soundtrackObjects;
    private AudioSource soundtrack;
    private int currentProgress;
    [Range(0,1)]
    public float maxVolume;
    [Range(1,10)]
    [Tooltip("Keep Between 1 to 10 Seconds")]
    public float fadeTime;
    private float lengthOfTrack;
   // public Slider mainMenuVolumeSlider;
    public bool noFadeIn;
    int currentSound = 0;

    //This function kicks off the first fade in.
    void Start()
    {
        StartCoroutine(FadeIn(2));
        //mainMenuVolumeSlider.value = maxVolume;
    }
    //This function keeps the volume within its desired threshold.
    void Update()
    {
        if (soundtrack != null)
        {
            soundtrack.volume = Mathf.Clamp(soundtrack.volume, 0, maxVolume);
        }
    }
	
    //This coroutine handles the fading in of the track.
    public IEnumerator FadeIn(int soundToPlay)
    {
        currentSound = soundToPlay;
        soundtrack = Instantiate(soundtrackObjects[soundToPlay].GetComponent<AudioSource>());
        lengthOfTrack = soundtrack.clip.length;
        StartCoroutine(currentTrackProgress());
        float interval = (maxVolume / fadeTime) / 10;
        for(int i = 0; i < fadeTime * 10;i++)
        {
            soundtrack.volume += interval;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitUntil(() => soundtrack.volume == maxVolume);
        Debug.Log("Stopped Fade In");
        StopCoroutine("FadeIn");
    }

    //This coroutine handles the actual fading out of the track.
    public IEnumerator FadeOut(int soundToPlay)
    {
        
        float interval = (maxVolume / fadeTime) / 10;
        for (int i = 0; i < fadeTime * 10; i++)
        {
            soundtrack.volume -= interval;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(soundtrack.gameObject);
        if (!noFadeIn)
        {
            StartCoroutine(FadeIn(soundToPlay));
        }
        StopCoroutine("FadeOut");
    }

    //This coroutine handles the time at which the fading out should be triggered.
    public IEnumerator currentTrackProgress()
    {
        float timeToBeginFade = (soundtrack.clip.length - fadeTime);
        currentProgress = 0;
        for(int i = 0;i < timeToBeginFade - 5;i++)
        {
            currentProgress++;
            yield return new WaitForSeconds(1);
        }
        StartCoroutine(FadeOut(currentSound));
        StopCoroutine(currentTrackProgress());
    }
	
    public void SetVolume()
    {
        //maxVolume = mainMenuVolumeSlider.value;
        soundtrack.volume = maxVolume;
    }
    
}
