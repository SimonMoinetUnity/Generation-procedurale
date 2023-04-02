using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 500;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator;
    int chunckSize;
    int chunckVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunckDictionnary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start() 
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunckSize = MapGenerator.mapChuckSize - 1;
        chunckVisibleInViewDst = Mathf.RoundToInt(maxViewDst/chunckSize);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChuncks();
    }

    void UpdateVisibleChuncks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChuckCoordX = Mathf.RoundToInt(viewerPosition.x / chunckSize);
        int currentChuckCoordY = Mathf.RoundToInt(viewerPosition.y / chunckSize);

        for (int yOffset = -chunckVisibleInViewDst; yOffset <= chunckVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunckVisibleInViewDst; xOffset <= chunckVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunckCoord = new Vector2(currentChuckCoordX + xOffset, currentChuckCoordY + yOffset);

                if(terrainChunckDictionnary.ContainsKey(viewedChunckCoord))
                {
                    terrainChunckDictionnary[viewedChunckCoord].UpdateTerrainChunck();
                    if(terrainChunckDictionnary[viewedChunckCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunckDictionnary[viewedChunckCoord]);
                    }
                }
                else
                {
                    terrainChunckDictionnary.Add(viewedChunckCoord, new TerrainChunk(viewedChunckCoord, chunckSize, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        Vector2 position;
        GameObject meshObject;
        Bounds bounds;

        MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            mapGenerator.RequestMapData(OnMapDataReceived);

        }

        void OnMapDataReceived(MapData mapData)
        {
            mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreatMesh();
        }

        public void UpdateTerrainChunck() 
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public  void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
