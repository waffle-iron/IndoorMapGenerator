﻿using System.Collections;
using UnityEngine;

public class MathUtils {

	public float[,] ContrastValues(float[,] inputValuesArray, int percentRatio) {
		return ContrastValues(
			inputValuesArray,
			percentRatio,
			FindRangesMinMax (inputValuesArray)
		);
	}

	public float[,] ContrastValues(float[,] inputValuesArray, int percentRatio, float rangeMin, float rangeMax) {

		if (percentRatio < -100) {
			percentRatio = (int)System.Math.Pow((percentRatio/100),7)*100; // f(x) = x^7
		}

		float contrastDeltaRange = (rangeMax - rangeMin) * (percentRatio / (100f * 2f));
		float contrastRangeMin = rangeMin + contrastDeltaRange;
		float contrastRangeMax = rangeMax - contrastDeltaRange;


		for (int x = 0; x < inputValuesArray.GetLength (0); ++x) {
			for (int z = 0; z < inputValuesArray.GetLength (1); ++z) {
				float contrastedValue = UnityEngine.Mathf.Lerp (contrastRangeMin, contrastRangeMax, inputValuesArray [x, z]);
				float valueAdaptedToOutput = UnityEngine.Mathf.InverseLerp (rangeMin, rangeMax, contrastedValue);
				inputValuesArray [x, z] = valueAdaptedToOutput;
			}
		}

		return inputValuesArray;
	}

	public Vector2 Indexto2dCoordinates(float index, int dimZ) {
		Vector2 coordinate = new Vector2 ();
		coordinate.x = index % dimZ;
		coordinate.y = index / dimZ;
		return coordinate;
	}

//	http://softwareengineering.stackexchange.com/a/212813
	public Vector3 Indexto3dCoordinates(float index, int dimX, int dimZ) {
		Vector3 coordinate = new Vector3 ();
		coordinate.x = index % dimZ;
		coordinate.y = (index / dimZ)%dimX;
		coordinate.z = index / (dimZ*dimX);
		return coordinate;
	}

	public Vector2[] Indexto2dCoordinates(float[] indexes, int dimZ) {
		Vector2[] coordinates = new Vector2[indexes.Length]; 
		for (int i = 0; i < indexes.Length; ++i) {
			coordinates [i] = Indexto2dCoordinates (indexes [i], dimZ);
		}
		return coordinates;
	}

	public Vector3[] Indexto3dCoordinates(float[] indexes, int dimX, int dimZ) {
		Vector3[] coordinates = new Vector3[indexes.Length]; 
		for (int i = 0; i < indexes.Length; ++i) {
			coordinates [i] = Indexto3dCoordinates (indexes [i], dimX, dimZ);
		}
		return coordinates;
	}

	public float[,] ContrastValues(float[,] inputValuesArray, int percentRatio, float[] rangesMinMax) {
		return ContrastValues (inputValuesArray, percentRatio, rangesMinMax [0], rangesMinMax [1]);
	}
	
	public float[] FindRangesMinMax(float[,] inputValuesArray) {
		float[] rangesMinMax = new float[2];
		rangesMinMax [0] = float.MaxValue;
		rangesMinMax [1] = float.MinValue;

		for (int x = 0; x < inputValuesArray.GetLength (0); ++x) {
			for (int z = 0; z < inputValuesArray.GetLength (1); ++z) {
				if (inputValuesArray [x, z] < rangesMinMax [0]) {
					rangesMinMax [0] = inputValuesArray [x, z];
				}
				if (inputValuesArray [x, z] > rangesMinMax [1]) {
					rangesMinMax [1] = inputValuesArray [x, z];
				} 
			}
		}
		return rangesMinMax;
	}
}