﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security;
using System.Globalization;
using System.ComponentModel;
using UnityEngine.Serialization;
using UnityEngine.Networking.NetworkSystem;

public class RandomUtils {

	System.Random random;

	public RandomUtils() {
		random = new System.Random ();
	}

	//todo: fix word DIMENSION to RESOLUTION in the entire project!!
//	public float[,] CreatePerlinNoise(int mapDimensionX, int mapDimensionZ, int perlinDimentionX, int perlinDimensionZ, float scaleFactor) {
//		return CreatePerlinNoiseValues (mapDimensionX, mapDimensionZ, scaleFactor);
//	}

	//PERSISTENCE IN RANGE 0-1
	//LACUNARITY IN RANGE >1
	public float[,] CreatePerlinNoise(int perlinResolutionX, int perlinResolutionZ, 
		float noiseScaleFactorX, float noiseScaleFactorZ, 
		int noiseLayers, float perlinFauxRange, float persistence, float lacunarity,
		bool useFixedPerlinSamplingOffset = true, float perlinSamplingOffsetX = 0, float perlinSamplingOffsetZ = 0) {

		float[,] perlinNoiseValues = new float[perlinResolutionX, perlinResolutionZ];

		if (!useFixedPerlinSamplingOffset) {
			perlinSamplingOffsetX = UnityEngine.Random.Range (0f, 100000f);
			perlinSamplingOffsetZ = UnityEngine.Random.Range (0f, 100000f);
		}

		noiseScaleFactorX *= (perlinResolutionX/10f);
		noiseScaleFactorZ *= (perlinResolutionZ/10f);

		float outputMinVal = float.MaxValue;
		float outputMaxVal = float.MinValue;

		float noiseAmplitude, noiseFrequency;
		float perlinSamplePointX, perlinSamplePointZ;
		float perlinValue;
		for (int x = 0; x < perlinResolutionX; ++x) {
			for (int z = 0; z < perlinResolutionZ; ++z) {
						
				noiseAmplitude = 1;
				noiseFrequency = 1;
				perlinValue = 0;

				for (int l = 0; l < noiseLayers; ++l) {

					perlinSamplePointX = x / (float)noiseScaleFactorX * noiseFrequency + (perlinSamplingOffsetX + (Mathf.PI * l));
					perlinSamplePointZ = z / (float)noiseScaleFactorZ * noiseFrequency + (perlinSamplingOffsetZ + (Mathf.PI * l));

					perlinValue += 
						UnityEngine.Mathf.PerlinNoise (perlinSamplePointX, perlinSamplePointZ) 
						* (perlinFauxRange - perlinFauxRange/2f)
						* noiseAmplitude;

					noiseAmplitude *= persistence;
					noiseFrequency *= lacunarity;
				}

				perlinNoiseValues [x, z] = perlinValue;

				if (perlinValue < outputMinVal) {
					outputMinVal = perlinValue;
				} 
				if (perlinValue > outputMaxVal) {
					outputMaxVal = perlinValue;
				}

			}
		}

		for (int x = 0; x < perlinNoiseValues.GetLength (0); ++x) {
			for (int z = 0; z < perlinNoiseValues.GetLength(1); ++z) {
				perlinNoiseValues [x, z] = Mathf.InverseLerp (outputMinVal, outputMaxVal, perlinNoiseValues [x, z]);
			}
		}

		return perlinNoiseValues;
	}

	public float[,] CreateRandomNoise(int mapDimensionX, int mapDimensionZ, float scaleFactor) {
		float[,] randomNoiseValues = new float[mapDimensionX, mapDimensionZ];
		for (int x = 0; x < mapDimensionX; ++x) {
			for (int z = 0; z < mapDimensionZ; ++z) {
				randomNoiseValues [x, z] = UnityEngine.Random.Range (0f, 1f);
			}
		}
		return randomNoiseValues;
	}


	public int[] GetUniqueRandomNumbersInt(int count, int rangeMinInclusive, int rangeMaxNonInclusive, bool sortAsc = false){
		List<int> uniqueNums = new List<int> (count);
//		int[] uniqueNums = new int[count];
		int num;
		for (int c = 0; c < count; c++) {
			num = random.Next(rangeMinInclusive, rangeMaxNonInclusive);
			if (!Contains (uniqueNums, num)) {
				uniqueNums.Add (num);
			} else {
				--c;
			}
		}

		if (sortAsc) { uniqueNums.OrderBy (x => x); }
		return uniqueNums.ToArray ();
	}

	//TODO: refactor to hashmap or whatever
	// (now, cost of checking for existing number is O(n))
	//FIXME: if count == 0, sometimes app crashes.
	public float[] GetUniqueRandomNumbers (int count, float rangeMinInclusive, float rangeMaxNonInclusive, bool sortAsc = false){
		float[] uniqueNums = new float[count];
		float num;
		for (int c = 0; c < count; c++) {
			num = UnityEngine.Random.Range (rangeMinInclusive, rangeMaxNonInclusive);
			if (!Contains (uniqueNums, num)) {
				uniqueNums [c] = num;
			} else {
				--c;
			}
		}

		if (sortAsc) { uniqueNums.OrderBy (x => x); }
		return uniqueNums;
	}

	public Vector2[] GetUniqueRandomVectors2 (int count, int rangeMinInclusiveX, int rangeMaxNonInclusiveX, int rangeMinInclusiveZ, int rangeMaxNonInclusiveZ, bool sortAsc = false) {
		List<Vector2> uniqueVectors = new List<Vector2>(count);
		Vector2 vec;
		Vector2 invVec = new Vector2 ();
		for (int c = 0; c < count; ++c) {
			vec.x = random.Next (rangeMinInclusiveX, rangeMaxNonInclusiveX);
			vec.y = random.Next (rangeMinInclusiveZ, rangeMaxNonInclusiveZ);
//			invVec.x = vec.y;
//			invVec.y = vec.x;
//			if (!Contains(uniqueVectors, vec) && vec.x != vec.y && !Contains(uniqueVectors, invVec)) {
			if (!Contains(uniqueVectors, vec)) {
				uniqueVectors.Add(vec);
			} else {
				--c;
			}
		}

		if (sortAsc) { uniqueVectors.OrderBy (v => v.x * Math.Abs(rangeMaxNonInclusiveZ - rangeMinInclusiveZ)+ v.y); }

		return uniqueVectors.ToArray ();
	}

	public Vector2[] GetUniqueRandomEdges(int count, int rangeMinInclusiveX, int rangeMaxNonInclusiveX, int rangeMinInclusiveZ, int rangeMaxNonInclusiveZ, bool sortAsc = false) {
		//x:
		int[] uniqueEdgesStart = GetUniqueRandomNumbersInt (count, rangeMinInclusiveX, rangeMaxNonInclusiveX);

		//y:
		List<int> uniqueEdgesEnd = new List<int>(count);
		int num;
		for (int c = 0; c < count; ++c) {
			num = random.Next (rangeMinInclusiveX, rangeMaxNonInclusiveX);
			if (!Contains(uniqueEdgesEnd, num) 
				&& num != uniqueEdgesStart[c] 
				&& !ContainsInvertedPair (uniqueEdgesStart, uniqueEdgesEnd.ToArray (), uniqueEdgesStart[c], num)
			) {
				--c;
			}
		}

		Vector2[] uniqueEdgesBorderPoints = new Vector2 [count];
		for (int c = 0; c < count; ++c) {
			uniqueEdgesBorderPoints [c].x = uniqueEdgesStart [c];
			uniqueEdgesBorderPoints [c].y = uniqueEdgesEnd[c];
		}

		return uniqueEdgesBorderPoints;
	}

	private bool ContainsInvertedPair(int[] containerX, int[] containerY, int x, int y)  {
		for (int i=0; i < containerY.Length; ++i) {
			if (containerX [i] == y && containerY [i] == x)
				return true;
		}

		return false;
	}

	public int RandomRangeMiddleVal (int min, int max) {
		//		if (min > max) {
		//			max += min;
		//			min = max - min;
		//			max -= min;
		//		}

		int delta = max - min;
		delta /= 2;
		return UnityEngine.Random.Range (min - delta, min + delta);
	}

	/*
	 * Returning array of numbers ranging from 0 (inclusive) to (count) (inclusive).
	 */
	public int[] CreateAscendingNumbersArray (int count){
		int[] nums = new int[count];
		for (int c = 0; c < nums.Length; c++) {
			nums [c] = c;
		}
		return nums;
	}

	/*
	 * Returning list of numbers ranging from 0 (inclusive) to (count) (inclusive).
	 */
	public LinkedList<int> CreateAscendingNumbers (int count){
		LinkedList<int> numbers = new LinkedList<int> ();
		for (int c = 0; c < count; c++) {
			numbers.AddLast (c);
		}
		return numbers;
	}

	private bool Contains (int[] container, int number){
		for (int c = 0; c < container.Length; ++c) {
			if (container [c] == number) {
				return true;
			}
		}
		return false;
	}

	/**
	 * Utility method for checking if (number) is inside the (container). Comparision by value.
	 */ 
	private bool Contains (float[] container, float number){
		for (int c = 0; c < container.Length; ++c) {
			if (container [c] == number) {
				return true;
			}
		}
		return false;
	}

	/**
	 * Utility method for checking if (number) is inside the (container). Comparision by value.
	 */ 
	private bool Contains (LinkedList<float> container, float number){
		for (int c = 0; c < container.Count; ++c) {
			if (container.ElementAt (c) == number) {
				return true;
			}
		}
		return false;
	}

	private bool Contains (List<int> container, int number){
		for (int c = 0; c < container.Count; ++c) {
			if (container.ElementAt (c) == number) {
				return true;
			}
		}
		return false;
	}


	//todo: make ONE Contains class (List<Object>, Object)
	private bool Contains (List<Vector2> container, Vector2 number){
		for (int c = 0; c < container.Count; ++c) {
			if (container.ElementAt (c) == number) {
				return true;
			}
		}
		return false;
	}

	private bool Contains(Vector2[] container, Vector2 vector) {
		for (int c = 0; c < container.Length; ++c) {
			if (container[c].x == vector.x && container[c].y == vector.y) {
				return true;
			}
		}
		return false;
	}

}
