using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UnderwaterEffect : MonoBehaviour
{
    public Color underwaterColor = new Color(0.0f, 0.4f, 0.7f, 0.6f);

    public Volume volume;

    public VolumeProfile volumeProfile;

    private Color normalColor;

    private bool isUnderwater = false;

    private void Start()
    {
        normalColor = RenderSettings.fogColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RenderSettings.fogColor = underwaterColor;
            RenderSettings.fogDensity = 0.1f;

            volume.profile = volumeProfile;

            isUnderwater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RenderSettings.fogColor = normalColor;
            RenderSettings.fogDensity = 0.02f;

            volume.profile = null;

            isUnderwater = false;
        }
    }

    private void Update()
    {
        if (isUnderwater)
        {
            // Other underwater effects if need be?
        }
    }
}
