using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class UnderwaterEffect : MonoBehaviour
{
    public Color underwaterColor = new Color(0.0f, 0.4f, 0.7f, 0.6f);

    public Volume volume;

    public VolumeProfile volumeProfile;

    private Color normalColor;

    public float height = 18f;

    private bool isUnderwater = false;

    private void Start()
    {
        normalColor = RenderSettings.fogColor;
    }

    private void Update()
    {
        if (Camera.main.transform.position.y < height)
        {
            RenderSettings.fogColor = underwaterColor;
            RenderSettings.fogDensity = 0.1f;

            volume.profile = volumeProfile;
        }
        else
        {
            RenderSettings.fogColor = normalColor;
            RenderSettings.fogDensity = 0.02f;

            volume.profile = null;
        }
    }
}
