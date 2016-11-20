using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom;
using System;

public class ComputationUtils : MonoBehaviour {

	private GaussianBlur gaussianBlur = new GaussianBlur (); 
	private MathUtils 	utilsMath = new MathUtils();
	private RandomUtils utilsRandom = new RandomUtils();
	private VectorUtils utilsVector = new VectorUtils();


	//dev only
	public float[,] CreateTestCrossValues(int mapDimensionX, int mapDimensionZ) {
		float[,] testCrossValues = new float[mapDimensionX, mapDimensionZ];

		for (int x = 0; x < mapDimensionX; ++x) {
			if (x <= mapDimensionZ - 1) {
				testCrossValues [x, x] = 1; 
			}
		}

		return testCrossValues;
	}

}
