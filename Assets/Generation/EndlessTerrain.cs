using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EndlessTerrain : MonoBehaviour
{
    const float scale = 1f;
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    public LODInfo[] detailLevels;
    public static float maxViewDistance;
    public Transform viewer;
    public Material mapMaterial;
    public static Vector2 viewerPosition;
    Vector2 lastChunkUpdatePosition;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunkVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    void Start()
    {
        mapGenerator = gameObject.GetComponent<MapGenerator>();

        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        chunkSize = MapGenerator.mapChuckSize - 1;
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);

        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if ((lastChunkUpdatePosition - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            lastChunkUpdatePosition = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLODMesh;

        bool mapDataReceived = false;
        int previousLODIndex = -1;


        public TerrainChunk(Vector2 _coord, int _size, LODInfo[] _detailLevels, Transform _parent, Material _material)
        {
            detailLevels = _detailLevels;
            position = _coord * _size;
            bounds = new Bounds(position, Vector2.one * _size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = _material;

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = _parent;
            meshObject.transform.localScale = Vector3.one * scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);

                if (detailLevels[i].useForCollider)
                {
                    collisionLODMesh = lodMeshes[i];
                }
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData _mapData)
        {
            mapData = _mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChuckSize, MapGenerator.mapChuckSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        Vector3 GetRandomPositionInBounds(Bounds bounds)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = 50f; // Starting from the top of the bounds
            float z = Random.Range(bounds.min.y, bounds.max.y);

            return new Vector3(x, y, z);
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDistance;

                if (visible)
                {
                    if (meshCollider.transform.childCount == 0)
                    {
                        for (int i = 0; i < 400; i++)
                        {
                            Vector3 randomPosition = GetRandomPositionInBounds(bounds);
                            Ray ray = new Ray(randomPosition, Vector3.down);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit, 100))
                            {
                                Renderer renderer = hit.collider.GetComponent<Renderer>();
                                MeshCollider meshCollider = hit.collider as MeshCollider;

                                if (renderer != null && renderer.material != null && renderer.material.mainTexture != null && meshCollider != null)
                                {
                                    Texture2D texture = renderer.material.mainTexture as Texture2D;

                                    // Convert the hit point to texture coordinates
                                    Vector2 pixelUV = hit.textureCoord;
                                    pixelUV.x *= texture.width;
                                    pixelUV.y *= texture.height;

                                    // Ensure the texture is readable
                                    if (texture.isReadable)
                                    {
                                        Color color = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                                        i--;

                                        foreach (TerrainType terrain in MapGenerator.Instance.regions)
                                        {
                                            if (color.r - 0.02 < terrain.color.r && color.r + 0.02 > terrain.color.r &&
                                                color.g - 0.02 < terrain.color.g && color.g + 0.02 > terrain.color.g &&
                                                color.b - 0.02 < terrain.color.b && color.b + 0.02 > terrain.color.b)
                                            {
                                                if (terrain.prefabs.Count != 0)
                                                {
                                                    int index = Random.Range(0, terrain.prefabs.Count - 1);

                                                    if (terrain.prefabs[index].prefab)
                                                    {
                                                        if (terrain.prefabs[index].chance < 1f && terrain.prefabs[index].chance > 0f)
                                                        {
                                                            float chance = Random.Range(0f, 1f);
                                                            if (terrain.prefabs[index].chance >= chance)
                                                                break;
                                                        }
                                                        GameObject go = GameObject.Instantiate(terrain.prefabs[index].prefab, hit.point, terrain.prefabs[index].prefab.transform.rotation, meshObject.transform);
                                                        go.transform.position += new Vector3(0, terrain.prefabs[index].heightOffset, 0);
                                                        go.transform.localScale += new Vector3(
                                                            Random.Range(0, terrain.prefabs[index].addedScale.x),
                                                            Random.Range(0, terrain.prefabs[index].addedScale.y),
                                                            Random.Range(0, terrain.prefabs[index].addedScale.z));
                                                        go.transform.Rotate(new Vector3(
                                                            Random.Range(0, terrain.prefabs[index].addedRotate.x),
                                                            Random.Range(0, terrain.prefabs[index].addedRotate.y),
                                                            Random.Range(0, terrain.prefabs[index].addedRotate.z)));
                                                        Debug.DrawRay(randomPosition, Vector3.down * 100, Color.red, 5f);
                                                        i++;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Texture is not readable.");
                                    }
                                }
                            }
                        }
                    }

                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.Requestmesh(mapData);
                        }
                    }

                    if (lodIndex == 0)
                    {
                        if (collisionLODMesh.hasMesh && meshCollider.sharedMesh == null)
                        {
                            meshCollider.sharedMesh = collisionLODMesh.mesh;
                        }
                        else if (!collisionLODMesh.hasRequestedMesh)
                        {
                            //Debug.Log("Tile Request: " + position);
                            collisionLODMesh.Requestmesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }

                SetVisible(visible);
            }
        }

        public void SetVisible(bool _visible)
        {
            if (meshObject)
                meshObject.SetActive(_visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    public class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int _lod, System.Action _updateCallback)
        {
            lod = _lod;
            updateCallback = _updateCallback;
        }

        void OnMeshDataReceived(MeshData _meshData)
        {
            mesh = _meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }

        public void Requestmesh(MapData _mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(_mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public bool useForCollider;
        public float visibleDstThreshold;
    }
}
