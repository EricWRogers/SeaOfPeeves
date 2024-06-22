using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int _width, int _height, int seed, float _scale, int _octaves, float _persistance, float _lacunarity, Vector2 offset)
    {
        float [,] noiseMap = new float[_width,_height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[_octaves];
        for (int i = 0; i < _octaves; i++)
        {
            octaveOffsets[i].x = prng.Next(-100000, 100000) + offset.x;
            octaveOffsets[i].y = prng.Next(-100000, 100000) + offset.y;
        }

        if (_scale <= 0.0f)
        {
            _scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = _width / 2f;
        float halfHeight = _height / 2f;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int o = 0; o < _octaves; o++)
                {
                    float sampleX = (x - halfWidth) / _scale * frequency + octaveOffsets[o].x;
                    float sampleY = (y - halfHeight) / _scale * frequency + octaveOffsets[o].y;

                    float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlin * amplitude;

                    amplitude *= _persistance;
                    frequency *= _lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}
