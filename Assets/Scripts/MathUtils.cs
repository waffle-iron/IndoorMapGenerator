﻿using System.Collections;
using UnityEngine;
using System;
using UnityEditor;

public class MathUtils {

	public float[,] ConvertGraphToValues(
		int mapResolutionX, int mapResolutionY, int mapResolutionZ,  //todo: make vector out of this X,Y,Z stuff
		int graphResolutionX, int graphResolutionY, int graphResolutionZ, 
		Vector3[] graphNodes) {

		float[] ranges = FindRangesMinMax (graphNodes);
		Debug.Log (ranges [0] + "|" + ranges [1] + "|" + ranges [2] + "|" + ranges [3]);

//		float resolutionScaleOffsetX = mapResolutionX / graphResolutionX;
//		float resolutionScaleOffsetZ = mapResolutionZ / graphResolutionZ;

		float[,] valueMap = new float[mapResolutionX, mapResolutionZ];
		for (int x = 0; x < mapResolutionX; ++x) {
			for (int z = 0; z < mapResolutionZ; ++z) {
				valueMap [x, z] = 0f;
			}
		}

		int i = 0;
		int valX = 0;
		int valZ = 0;
		try {
		for (i = 0; i < graphNodes.Length; ++i) {
//				valX = mapResolutionZ - (int)(graphNodes[i].z + mapResolutionZ/2);
//				valZ = mapResolutionX - (int)(graphNodes[i].x + mapResolutionX/2); //works with square grid
				valX = (int)(graphNodes[i].x + mapResolutionX/2);
				valZ = (int)(graphNodes[i].z + mapResolutionZ/2);

			valueMap [
					valX, valZ
			] = 1f;
		}

		} catch (IndexOutOfRangeException exc) {
			Debug.LogError ("(" + i +"), " + "(" + valX +"), " + "(" + valZ +"), " + exc.StackTrace);
		}
			
		return valueMap;
	}

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

	public float[] FindRangesMinMax(Vector3[] inputValuesArray) {
		float[] rangesMinMaxX = new float[2] {float.MaxValue, float.MinValue};
		float[] rangesMinMaxY = new float[2] {float.MaxValue, float.MinValue};

		for (int x = 0; x < inputValuesArray.GetLength (0); ++x) {
			if (inputValuesArray[x].x < rangesMinMaxX[0]) {rangesMinMaxX [0] = inputValuesArray [x].x;}

			if (inputValuesArray[x].x > rangesMinMaxX[1]) {rangesMinMaxX [1] = inputValuesArray [x].x;}

			if (inputValuesArray[x].z < rangesMinMaxY[0]) {rangesMinMaxY [0] = inputValuesArray [x].z;}

			if (inputValuesArray[x].z > rangesMinMaxY[1]) {rangesMinMaxY [1] = inputValuesArray [x].z;}
		}
		return new float[4]{rangesMinMaxX[0], rangesMinMaxX[1], rangesMinMaxY[0], rangesMinMaxY[1]};
	}
}
