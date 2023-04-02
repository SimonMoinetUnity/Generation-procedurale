using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) // Méthode qui renvoie une array de type float : les cooredonnées (x,y) du noise
    {
        float[,] noiseMap = new float [mapWidth, mapHeight]; // Crée la variable correspondant à la noise map (array de type float à deux paramètres)

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2f;
        float halfHeight = mapHeight/2f;


        for (int y = 0; y < mapHeight; y++) // Boucle pour chaque pixel de la noise map
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequancy = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequancy + octaveOffsets[i].x + offset.x; // Donne une teinte de gris
                    float sampleY = (y - halfHeight) / scale * frequancy + octaveOffsets[i].y + offset.y; // Donne une teinte de gris

                    float perlinValue = Mathf.PerlinNoise(sampleX,sampleY) * 2 - 1; // La fonction "PerlinNoise()" permet d'avoir une répartition progressive des teintes de gris (entre 0 et 1) (*2 -1 permet d'avoir des valeurs négatives)
                    //noiseMap[x,y] = perlinValue; // Assigne la valeur perlinValue au pixel de coordonnées (x,y) de la noise map
                    noiseHeight += perlinValue * amplitude;
                    amplitude *=  persistance;
                    frequancy *= lacunarity;
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

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}
