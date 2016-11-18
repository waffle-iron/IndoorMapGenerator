using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Linq.Expressions;

public class PerlinNoiseRenderer : MonoBehaviour {

	public Renderer renderer;


	public void RenderValuesArray(float[,] valuesArray, float rangeMin = 0f, float rangeMax = 1f) {
		ValidateRenderer ();
		renderer.sharedMaterial.mainTexture = CreateValuesTexture (valuesArray, rangeMin, rangeMax);
		renderer.transform.localScale = new Vector3 (
			valuesArray.GetLength (0), 
			1, 
			valuesArray.GetLength (1)
		);
	}


	//2D
	private Texture CreateValuesTexture(float[,] valuesArray, float rangeMin, float rangeMax) {

		int mapDimensionX = valuesArray.GetLength (0);
		int mapDimensionZ = valuesArray.GetLength (1);

		Texture2D valueTexture = new Texture2D (mapDimensionX, mapDimensionZ);
		Color[] valueTextureColorMap = new Color[mapDimensionX * mapDimensionZ];

		for(int x = 0; x < mapDimensionX; ++x) {
			for (int z = 0; z < mapDimensionZ; ++z) {
				if (valuesArray[x, z] > rangeMax || valuesArray[x, z] < rangeMin) {
					Debug.LogError ("value in array outside of range (" + valuesArray [x, z] + ") on [x:" + x + ", z:" + z + "].");
					valueTextureColorMap [x * mapDimensionZ + z] = Color.black;
					continue;
				}

				valueTextureColorMap [x * mapDimensionZ + z] = Color.Lerp(
					Color.yellow, 
					Color.red, 
					Mathf.InverseLerp(rangeMin, rangeMax, valuesArray[x, z])
				);
			}
		}
		valueTexture.SetPixels (valueTextureColorMap);
		valueTexture.filterMode = FilterMode.Point;
		valueTexture.Apply ();

		return valueTexture;
	}

	void OnValidate() {
		ValidateRenderer ();
	}

	private void ValidateRenderer() {
		if (renderer == null) {
			Renderer renderObject = gameObject.GetComponent<ProceduralMapGenerator> ().GetMapOutputObject ().GetPlaneObject ();
			if (renderObject != null) {
				renderer = renderObject;
			} 
		}
	}

}
