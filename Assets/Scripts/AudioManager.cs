using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public AudioMixer mixer;

    public List<AudioSting> audioStings = new List<AudioSting>();

    public AudioSource stingSource;
    public AudioSource mainAmbience;
    public AudioSource waterAmbience;


    public bool underWater;

    private void Start()
    {
        waterAmbience.volume = 0;
        mainAmbience.volume = 0;
    }

    private void Update()
    {
        waterAmbience.volume = underWater ? 1 : 0;
        mainAmbience.volume = underWater ? 0 : 1;
    }

    public void SetVolume(float _sliderValue)
    {
        float dB = Mathf.Lerp(-80f, 0f, _sliderValue);
        mixer.SetFloat("MainVolume", dB);
    }

    public void PlaySting(string _tag)
    {
        for (int i = 0; i < audioStings.Count; i++)
        {
            if (audioStings[i].tag == _tag)
            {
                stingSource.clip = audioStings[i].clip;
                stingSource.volume = audioStings[i].volume;
                stingSource.Play();
            }
        }
    }
}


[System.Serializable]
public class AudioSting
{
    public AudioClip clip;
    public string tag;
    public float volume;
}
