using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer; // Permet d'accéder au Renderer du plan
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void  DrawTexture(Texture2D texture) // Fonction prenant comme argument la noise map (array de type float) qui contient les valeurs perlinNoise
    {
        textureRenderer.sharedMaterial.mainTexture = texture; // Applique la texture 2D au texture Renderer (sans passer par le run time)
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height); // Attribue au plan les dimensions de la noise map
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreatMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
