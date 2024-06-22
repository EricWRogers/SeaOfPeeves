using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] _heightMap, float _heightMultiplier, AnimationCurve _meshHeightCurve, int _levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_meshHeightCurve.keys);
        int width = _heightMap.GetLength(0);
        int height = _heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSlimplificationIncrement = (_levelOfDetail == 0) ? 1 : _levelOfDetail * 2;
        int verticiesPerLine = (width - 1) / meshSlimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticiesPerLine, verticiesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSlimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSlimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(_heightMap[x,y]) * _heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width, y/(float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticiesPerLine + 1, vertexIndex + verticiesPerLine);
                    meshData.AddTriangle(vertexIndex + verticiesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    int triangleIndex;

    public MeshData(int _width, int _height)
    {
        vertices = new Vector3[_width * _height];
        uvs = new Vector2[_width * _height];
        triangles = new int[(_width-1)*(_height-1)*6];
    }

    public void AddTriangle(int _a, int _b, int _c)
    {
        triangles[triangleIndex] = _a;
        triangles[triangleIndex+1] = _b;
        triangles[triangleIndex+2] = _c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
