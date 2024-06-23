using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public AudioMixer mixer;

    public List<AudioSting> audioStings = new List<AudioSting>();

    private AudioSource stingSource;

    private void Start()
    {
        stingSource = GetComponent<AudioSource>();
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
