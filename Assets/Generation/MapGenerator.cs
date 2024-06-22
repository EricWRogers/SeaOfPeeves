using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColorMap, Mesh};
    public DrawMode drawMode;
    public Noise.NormalizeMode normalizeMode;
    public const int mapChuckSize = 241;
    [Range(0,6)]
    public int editorPreviewLOD;
    public float scale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshmapHeightMultiplier;
    public AnimationCurve meshmapHeightCurve;
    public bool autoUpdate;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = gameObject.GetComponent<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChuckSize, mapChuckSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshmapHeightMultiplier, meshmapHeightCurve, editorPreviewLOD);
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChuckSize, mapChuckSize);

            display.DrawMesh(meshData, texture);
        }
    }

    public void RequestMapData(Vector2 _center, Action<MapData> _callback)
    {
        ThreadStart threadStart = delegate {
            MapDataThread(_center, _callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 _center, Action<MapData> _callback)
    {
        MapData mapData = GenerateMapData(_center);
        lock(mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(_callback, mapData));
        }
    }

    public void RequestMeshData(MapData _mapData, int _lod, Action<MeshData> _callback)
    {
        ThreadStart threadStart = delegate {
            MeshDataThread(_mapData, _lod, _callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData _mapData, int _lod, Action<MeshData> _callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(_mapData.heightMap, meshmapHeightMultiplier, meshmapHeightCurve, _lod);
        lock(meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(_callback, meshData));
        }
    }

    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 _center)
    {
        float [,] noiseMap = Noise.GenerateNoiseMap(mapChuckSize, mapChuckSize, seed, scale, octaves, persistance, lacunarity, _center + offset, normalizeMode);

        Color[] colorMap = new Color[mapChuckSize * mapChuckSize];
        for (int y = 0; y < mapChuckSize; y++)
        {
            for (int x = 0; x < mapChuckSize; x++)
            {
                float currentmapChuckSize = noiseMap[x,y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentmapChuckSize >= regions[i].height)
                    {
                        colorMap[y * mapChuckSize + x] = regions[i].color;
                    } else {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
    }

    void OnValidate()
    {
        lacunarity = Mathf.Max(1, lacunarity);
        octaves = Mathf.Max(0, octaves);
    }

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> _callback, T _parameter)
        {
            callback = _callback;
            parameter = _parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public float[,] heightMap;
    public Color[] colorMap;

    public MapData(float[,] _heightMap, Color[] _colorMap)
    {
        heightMap = _heightMap;
        colorMap = _colorMap;
    }
}
