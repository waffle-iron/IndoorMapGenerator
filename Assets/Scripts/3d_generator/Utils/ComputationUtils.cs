using UnityEngine;
using System.Collections;

public class ComputationUtils : MonoBehaviour {

	public float[,] CreatePerlinNoiseValues(int mapDimensionX, int mapDimensionZ, float scaleFactor) {
		if (mapDimensionX < 1) {
			mapDimensionX = 1;
		}

		if (mapDimensionZ < 1) {
			mapDimensionZ = 1;
		}

		if (scaleFactor < 0.0001f) {
			scaleFactor = 0.0001f;
		}


		return CreatePerlinNoise (mapDimensionX, mapDimensionZ, scaleFactor);
	}

	private float[,] CreatePerlinNoise(int mapDimensionX, int mapDimensionZ, float scaleFactor) {
		float[,] perlinNoiseValues = new float[mapDimensionX, mapDimensionZ];

		for (int x = 0; x < mapDimensionX; ++x) {
			for (int z = 0; z < mapDimensionZ; ++z) {
				perlinNoiseValues [x, z] = Mathf.PerlinNoise (x / scaleFactor, z / scaleFactor);
			}
		}

		return perlinNoiseValues;
	}

}
