using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security;
using System.Globalization;
using System.ComponentModel;

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
	public float[,] CreatePerlinNoise(int mapDimensionX, int mapDimensionZ, int perlinResolutionX, int perlinResolutionZ, 
		float noiseScaleFactorX, float noiseScaleFactorZ, 
		int noiseLayers, float persistence, float lacunarity) {

		float[,] perlinNoiseValues = new float[mapDimensionX, mapDimensionZ];



		int resolutionsScaleOffsetX = Mathf.CeilToInt (mapDimensionX / perlinResolutionX);
		int resolutionsScaleOffsetZ = Mathf.CeilToInt (mapDimensionZ / perlinResolutionZ);

		float noiseAmplitude, noiseFrequency;
		float noiseSamplePointX, noiseSamplePointZ;
		float perlinValue;
		for (int x = 0; x < perlinResolutionX; ++x) {
			for (int z = 0; z < perlinResolutionZ; ++z) {



				for (int scaleX = 0; scaleX < resolutionsScaleOffsetX; ++scaleX) { //so that we can create a map with data resolution of PerlinRes with array resolution of MapRes
					for (int scaleZ = 0; scaleZ < resolutionsScaleOffsetZ; ++scaleZ) {
						
						perlinValue = 0;
						noiseAmplitude = 1;
						noiseFrequency = 1;

						for (int l = 0; l < noiseLayers; ++l) {
							noiseAmplitude = Mathf.Pow (persistence, l);
							noiseFrequency = Mathf.Pow (lacunarity, l);
							noiseSamplePointX = x / noiseScaleFactorX * noiseFrequency;
							noiseSamplePointZ = z / noiseScaleFactorZ * noiseFrequency;

							perlinValue += UnityEngine.Mathf.PerlinNoise (noiseSamplePointX, noiseSamplePointZ) * noiseAmplitude;
						}

						perlinNoiseValues [x * resolutionsScaleOffsetX + scaleX, z * resolutionsScaleOffsetZ + scaleZ] = perlinValue;

						//	perlinNoiseValues [x, z] = UnityEngine.Mathf.PerlinNoise (x / scaleFactorX, z / scaleFactorZ);
//							perlinNoiseValues [x * resolutionsScaleOffsetX + scaleX, z * resolutionsScaleOffsetZ + scaleZ] = UnityEngine.Mathf.PerlinNoise (
//								x / noiseScaleFactorX,
//								z / noiseScaleFactorZ
//							);
					}
				}

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
