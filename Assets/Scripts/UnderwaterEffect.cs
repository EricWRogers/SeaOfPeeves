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
    private bool previousUnderwater = false;
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
            RenderSettings.fogEndDistance = 50f;

            volume.profile = volumeProfile;
            if(AudioManager.Instance != null)
            {
                AudioManager.Instance.underWater = true;
            }

            isUnderwater = true;
        }
        else
        {
            RenderSettings.fogColor = normalColor;
            RenderSettings.fogDensity = 0.02f;
            RenderSettings.fogEndDistance = 300f;

            volume.profile = null;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.underWater = false;
            }

            isUnderwater = false;
        }

        LazyDetectWaterChange();
    }

    private void LazyDetectWaterChange()
    {
        if(previousUnderwater != isUnderwater)
        {
            //Aye yo, something changed here man
            AudioManager.Instance.PlaySting(isUnderwater ? "enterWater" : "leaveWater");
        }

        previousUnderwater = isUnderwater;
    }
}
