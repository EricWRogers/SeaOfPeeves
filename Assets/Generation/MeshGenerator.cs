using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] _heightMap, float _heightMultiplier, AnimationCurve _meshHeightCurve, int _levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_meshHeightCurve.keys);
        int meshSlimplificationIncrement = (_levelOfDetail == 0) ? 1 : _levelOfDetail * 2;
        int borderedSize = _heightMap.GetLength(0);
        int meshSize = borderedSize - 2 * meshSlimplificationIncrement;
        int meshSizeUnsimplified = borderedSize - 2;

        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

        int verticiesPerLine = (meshSize - 1) / meshSlimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticiesPerLine);
        
        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for (int y = 0; y < borderedSize; y += meshSlimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSlimplificationIncrement)
            {
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                if (isBorderVertex)
                {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < borderedSize; y += meshSlimplificationIncrement)
        {
            for (int x = 0; x < borderedSize; x += meshSlimplificationIncrement)
            {
                int vertexIndex = vertexIndicesMap[x, y];
                Vector2 percent = new Vector2((x-meshSlimplificationIncrement)/(float)meshSize, (y-meshSlimplificationIncrement)/(float)meshSize);
                float height = heightCurve.Evaluate(_heightMap[x,y]) * _heightMultiplier;
                Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

                meshData.AddVertex(vertexPosition, percent, vertexIndex);                

                if (x < borderedSize - 1 && y < borderedSize - 1)
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + meshSlimplificationIncrement, y];
                    int c = vertexIndicesMap[x, y + meshSlimplificationIncrement];
                    int d = vertexIndicesMap[x + meshSlimplificationIncrement, y + meshSlimplificationIncrement];
                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
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

    Vector3[] borderVertices;
    int[] borderTriangles;
    int triangleIndex;
    int borderTrianglesIndex;

    public MeshData(int _verticesPerLine)
    {
        vertices = new Vector3[_verticesPerLine * _verticesPerLine];
        uvs = new Vector2[_verticesPerLine * _verticesPerLine];
        triangles = new int[(_verticesPerLine-1)*(_verticesPerLine-1)*6];

        borderVertices = new Vector3[_verticesPerLine * 4 + 4];
        borderTriangles = new int[24 * _verticesPerLine];
    }

    public void AddVertex(Vector3 _vertexPosition, Vector2 _uv, int _vertexIndex)
    {
        if (_vertexIndex < 0)
        {
            borderVertices[-_vertexIndex - 1] = _vertexPosition;
        }
        else
        {
            vertices[_vertexIndex] = _vertexPosition;
            uvs[_vertexIndex] = _uv;
        }
    }

    public void AddTriangle(int _a, int _b, int _c)
    {
        if (_a < 0 || _b < 0 || _c < 0)
        {
            borderTriangles[borderTrianglesIndex] = _a;
            borderTriangles[borderTrianglesIndex+1] = _b;
            borderTriangles[borderTrianglesIndex+2] = _c;
            borderTrianglesIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = _a;
            triangles[triangleIndex+1] = _b;
            triangles[triangleIndex+2] = _c;
            triangleIndex += 3;
        }
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = borderTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0)
                vertexNormals[vertexIndexA] += triangleNormal;
            
            if (vertexIndexB >= 0)
                vertexNormals[vertexIndexB] += triangleNormal;
            
            if (vertexIndexC >= 0)
                vertexNormals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }
        
        return vertexNormals;
    }

    Vector3 SurfaceNormalFromIndices(int _indexA, int _indexB, int _indexC)
    {
        Vector3 pointA = (_indexA < 0) ? borderVertices[-_indexA-1] : vertices[_indexA];
        Vector3 pointB = (_indexB < 0) ? borderVertices[-_indexB-1] : vertices[_indexB];
        Vector3 pointC = (_indexC < 0) ? borderVertices[-_indexC-1] : vertices[_indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC);
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = CalculateNormals();
        return mesh;
    }
}
