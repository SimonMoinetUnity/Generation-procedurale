using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator 
{
   public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
   {
      Texture2D texture = new Texture2D(width, height);
      texture.filterMode = FilterMode.Point; // Permet de ne pas avoir de floux au niveau de la transition entre 2 couleurs
      texture.wrapMode = TextureWrapMode.Clamp;
      texture.SetPixels(colourMap);
      texture.Apply();
      return texture;
   }

   public static Texture2D TextureFromHeightMap(float[,] heightMap)
   {
      int width = heightMap.GetLength(0); // Récupère la longueur de la noise map (0 = l'index 0 de l'array : x)
      int height = heightMap.GetLength(1); // Récupère la hauteur de la noise map (1 = l'index 1 de l'array : y)

      Color[] colourMap = new Color[width*height]; // Créer une array de type Color contenant width*height éléments correspondants aux teintes de chaque pixels

      for (int y = 0; y < height; y++)
      {
         for (int x = 0; x < width; x++)
         {
             colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]); // Assigne à chaque pixel de la colour map la teinte correspondant (interpolation linéaire entre blanc et noir)
         }
      }

      return TextureFromColourMap(colourMap, width, height);
   }


}
