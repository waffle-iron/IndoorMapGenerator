using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtils {

	System.Random random;

	public RandomUtils() {
		random = new System.Random ();
	}

	public float[,] CreatePerlinNoise(int mapDimensionX, int mapDimensionZ, float scaleFactor) {
//		if (mapDimensionX < 1) { mapDimensionX = 1; }
//		if (mapDimensionZ < 1) { mapDimensionZ = 1; }
//		if (scaleFactor < 0.0001f) { scaleFactor = 0.0001f; } 
		return CreatePerlinNoiseValues (mapDimensionX, mapDimensionZ, scaleFactor);
	}

	private float[,] CreatePerlinNoiseValues(int mapDimensionX, int mapDimensionZ, float scaleFactor) {
		float[,] perlinNoiseValues = new float[mapDimensionX, mapDimensionZ];
		for (int x = 0; x < mapDimensionX; ++x) {
			for (int z = 0; z < mapDimensionZ; ++z) {
				perlinNoiseValues [x, z] = UnityEngine.Mathf.PerlinNoise (x / scaleFactor, z / scaleFactor);
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

	public Vector2[] GetUniqueRandomVectors2 (int count, int rangeMinInclusiveX, int rangeMaxNonInclusiveX, int rangeMinInclusiveZ, int rangeMaxNonInclusiveZ) {
		Vector2[] uniqueVectors = new Vector2[count];
		Vector2 vec;
		for (int c = 0; c < count; ++c) {
			vec.x = random.Next (rangeMinInclusiveX, rangeMaxNonInclusiveX);
			vec.y = random.Next (rangeMinInclusiveZ, rangeMaxNonInclusiveZ);
			if (!Contains(uniqueVectors, vec)) {
				uniqueVectors [c] = vec;
			} else {
				--c;
			}
		}

		return uniqueVectors;
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

	private bool Contains(Vector2[] container, Vector2 vector) {
		for (int c = 0; c < container.Length; ++c) {
			if (container[c].x == vector.x && container[c].y == vector.y) {
				return true;
			}
		}
		return false;
	}

}
