using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;

public class PerlinNoiseRenderer : MonoBehaviour {

	public Renderer renderer;


	public void RenderPerlinNoise(float[,] perlinNoiseValues) {
		renderer.sharedMaterial.mainTexture = CreatePerlinNoiseTexture (perlinNoiseValues);
		renderer.transform.localScale = new Vector3 (
			perlinNoiseValues.GetLength (0), 
			1, 
			perlinNoiseValues.GetLength (1)
		);
	}


	//2D
	private Texture CreatePerlinNoiseTexture(float[,] perlinNoiseValues) {

		int mapDimensionX = perlinNoiseValues.GetLength (0);
		int mapDimensionZ = perlinNoiseValues.GetLength (1);

		Texture2D perlinNoiseTexture = new Texture2D (mapDimensionX, mapDimensionZ);
		Color[] perlinNoiseTextureColourMap = new Color[mapDimensionX * mapDimensionZ];

		for(int x = 0; x < mapDimensionX; ++x) {
			for (int z = 0; z < mapDimensionZ; ++z) {
				if (perlinNoiseValues[x, z] > 1 || perlinNoiseValues[x, z] < 0) {
					Debug.LogError ("perlin value outside of range (" + perlinNoiseValues [x, z] + ") on [x:" + x + ", z:" + z + ").");
					perlinNoiseTextureColourMap [x * mapDimensionZ + z] = Color.black;
					continue;
				}

				perlinNoiseTextureColourMap [x * mapDimensionZ + z] = Color.Lerp(Color.yellow, Color.red, perlinNoiseValues[x, z]);
			}
		}
		perlinNoiseTexture.SetPixels (perlinNoiseTextureColourMap);
		perlinNoiseTexture.filterMode = FilterMode.Point;
		perlinNoiseTexture.Apply ();

		return perlinNoiseTexture;
	}



}
