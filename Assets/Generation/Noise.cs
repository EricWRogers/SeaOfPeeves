using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode {Local, Global}
    public static float[,] GenerateNoiseMap(int _width, int _height, int seed, float _scale, int _octaves, float _persistance, float _lacunarity, Vector2 offset, NormalizeMode _normalizeMode)
    {
        float [,] noiseMap = new float[_width,_height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[_octaves];

        float maxPossibleHeight = 0f;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < _octaves; i++)
        {
            octaveOffsets[i].x = prng.Next(-100000, 100000) + offset.x;
            octaveOffsets[i].y = prng.Next(-100000, 100000) - offset.y;

            maxPossibleHeight += amplitude;
            amplitude *= _persistance;
        }

        if (_scale <= 0.0f)
        {
            _scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = _width / 2f;
        float halfHeight = _height / 2f;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int o = 0; o < _octaves; o++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[o].x) / _scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[o].y) / _scale * frequency;

                    float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlin * amplitude;

                    amplitude *= _persistance;
                    frequency *= _lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (NormalizeMode.Local == _normalizeMode)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x,y]);
                }
                else if (NormalizeMode.Global == _normalizeMode)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 1.75f);
                    noiseMap[x,y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
