using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColourMap, Mesh}
    public DrawMode drawMode;

    public const int mapChuckSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octave;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public float meshHeightMultiplier;

    public AnimationCurve meshHeightCurve;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>>mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>>meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();


    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();

        MapDisplay display = FindObjectOfType<MapDisplay>(); // Récupère le script "MapDisplay"
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap)); // Attribue les "couleurs" à la noise map
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChuckSize, mapChuckSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChuckSize, mapChuckSize));
        }
    }
    
    public void RequestMapData(Action<MapData> callBack)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callBack);
        };

        new Thread (threadStart).Start();
    }

    public void MapDataThread(Action<MapData> callBack)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callBack, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callBack)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callBack);
        };

        new Thread (threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callBack)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callBack, meshData));
        }
    }

    private void Update() 
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callBack(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callBack(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChuckSize, mapChuckSize, seed, noiseScale, octave, persistance, lacunarity, offset); // Crée une array de type float à partir de la fonction "GenerateNoiseMap" du script "Noise"

        Color[] colourMap = new Color[mapChuckSize * mapChuckSize];

        for (int y = 0; y < mapChuckSize; y++)
        {
            for (int x = 0; x < mapChuckSize; x++)
            {
                float currentHeight = noiseMap[x,y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChuckSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
        
    }

    private void OnValidate() 
    {

        if(lacunarity < 1)
        {
            lacunarity = 1;
        }

        if(octave < 0)
        {
            octave = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callBack;
        public readonly T parameter;
        public MapThreadInfo(Action<T> callBack, T parameter)
        {
            this.callBack = callBack;
            this.parameter = parameter;
        }
    }
}

[System.Serializable] // Permet de faire apparaitre TerrainType dans l'inspector
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData (float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
