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
	private GFXUtils	utilsGfx = new GFXUtils();


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

	public GaussianBlur GetGaussianBlur() {
		return gaussianBlur;
	}

	public MathUtils GetUtilsMath() {
		return utilsMath;
	}

	public RandomUtils GetUtilsRandom() {
		return utilsRandom;
	}

	public VectorUtils GetUtilsVector() {
		return utilsVector;
	}

	public GFXUtils GetUtilsGFX() {
		return utilsGfx;
	}

}
