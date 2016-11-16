using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom;

public class ComputationUtils : MonoBehaviour {

	//1 channel implementation of Gaussian Blur
	public float[,] GaussianBlur(float[,] inputValuesArray, int blurRadius = 5, int blurIterations = 3, int blurSolidifications = 0) {
		float[,] blurredValuesArray = new float[inputValuesArray.GetLength (0), inputValuesArray.GetLength (1)];

		int[] blurBoxes = GaussianBlurBoxSizes (blurRadius, blurIterations);

		for (int i = 0; i < blurIterations; ++i) {
			blurredValuesArray = BoxBlur (
				inputValuesArray,
				inputValuesArray.GetLength (0), 
				inputValuesArray.GetLength (1), 
				(blurBoxes[i]-1)/2,
				blurSolidifications
			);
		}

		return blurredValuesArray;
	}
		

	// source channel, target channel, width, height, radius
	//	scl, tcl, w, h, r
	private float[,] BoxBlur(float[,] inputValuesArray, int boxWidth, int boxHeight, int blurRadius, int solidifyBlurIterations = 0) {
		float[,] blurredValues = new float[boxWidth, boxHeight];
			
		for(var i=0; i<boxHeight; i++) {
			for(var j=0; j<boxWidth; j++) {

				float val = 0;
				for(var iy=i-blurRadius; iy<i+blurRadius+1; iy++) {
					for(var ix=j-blurRadius; ix<j+blurRadius+1; ix++) {
						int x = Mathf.Min(boxWidth-1, Mathf.Max(0, ix));
						int y = Mathf.Min(boxHeight-1, Mathf.Max(0, iy));
						for (int s = 0; s < solidifyBlurIterations+1; ++s) {
							val += inputValuesArray [x, y] * (solidifyBlurIterations==0?1:0.5f);
						}
					}
				}

				blurredValues[i,j] = val/((blurRadius+blurRadius+1)*(blurRadius+blurRadius+1));
			}
		}

		return blurredValues;
	}

//	http://staffhome.ecm.uwa.edu.au/~00011811/research/matlabfns/#integral
	private int[] GaussianBlurBoxSizes(int standardDeviation, int boxCount) {
		List<int> boxSizes = new List<int> ();

		var wIdeal = Mathf.Sqrt((12*standardDeviation*standardDeviation/boxCount)+1);  // Ideal averaging filter width 
		var wl = Mathf.Floor(wIdeal);  
		if (wl % 2 == 0) {
			wl--;
		}
		var wu = wl+2;

		var mIdeal = (12*standardDeviation*standardDeviation - boxCount*wl*wl - 4*boxCount*wl - 3*boxCount)/(-4*wl - 4);
		var m = Mathf.Round(mIdeal);
		// var sigmaActual = Math.sqrt( (m*wl*wl + (n-m)*wu*wu - n)/12 );



		for(var i=0; i<boxCount; i++) {
			boxSizes.Add((int)(i<m?wl:wu));
		}
		return boxSizes.ToArray();
	}

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
