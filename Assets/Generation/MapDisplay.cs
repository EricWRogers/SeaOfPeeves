using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;

    public void DrawTexture(Texture2D _texture)
    {
        textureRender.sharedMaterial.mainTexture = _texture;
        textureRender.transform.localScale = new Vector3(_texture.width, 1, _texture.height);
    }
}
