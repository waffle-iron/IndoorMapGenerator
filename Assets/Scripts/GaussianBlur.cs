using System.Collections;
using System.Collections.Generic;

public class GaussianBlur {

	//1 channel implementation of Gaussian Blur
	public float[,] CreateGaussianBlur(float[,] inputValuesArray, int blurRadius = 5, int blurIterations = 3, int blurSolidifications = 0) {
		
		float[,] blurredValuesArray = new float[inputValuesArray.GetLength (0), inputValuesArray.GetLength (1)];
		int[] blurBoxes = GaussianBlurBoxSizes (blurRadius, blurIterations);

		for (int i = 0; i < blurIterations; ++i) {
			blurredValuesArray = BoxBlur (
				inputValuesArray, inputValuesArray.GetLength (0), inputValuesArray.GetLength (1), 
				(blurBoxes[i]-1)/2, blurSolidifications
			);
		}

		return blurredValuesArray;
	}


	private float[,] BoxBlur(float[,] inputValuesArray, int boxWidth, int boxHeight, int blurRadius, int solidifyBlurIterations = 0) {
		float[,] blurredValues = new float[boxWidth, boxHeight];

		for(var i=0; i<boxHeight; i++) {
			for(var j=0; j<boxWidth; j++) {
				
				float val = 0;
				for(var iy=i-blurRadius; iy<i+blurRadius+1; iy++) {
					for(var ix=j-blurRadius; ix<j+blurRadius+1; ix++) {
						
						int x = UnityEngine.Mathf.Min(boxWidth-1, UnityEngine.Mathf.Max(0, ix));
						int y = UnityEngine.Mathf.Min(boxHeight-1, UnityEngine.Mathf.Max(0, iy));
						if (x < boxWidth && x >= 0 && y < boxHeight && y >= 0) {
							for (int s = 0; s < solidifyBlurIterations + 1; ++s) {
								val += inputValuesArray [x, y] * (solidifyBlurIterations == 0 ? 1 : 0.5f);
							}
						}
					}
				}

				//todo: Mathf.Clamp(rangeMin, rangeMax)
				blurredValues[i,j] = val/((blurRadius+blurRadius+1)*(blurRadius+blurRadius+1));
			}
		}

		return blurredValues;
	}

	//	http://staffhome.ecm.uwa.edu.au/~00011811/research/matlabfns/#integral
	private int[] GaussianBlurBoxSizes(int standardDeviation, int boxCount) {
		List<int> boxSizes = new List<int> ();

		var wIdeal = UnityEngine.Mathf.Sqrt((12*standardDeviation*standardDeviation/boxCount)+1);  // Ideal averaging filter width 
		var wl = UnityEngine.Mathf.Floor(wIdeal);  
		if (wl % 2 == 0) {
			wl--;
		}
		var wu = wl+2;

		var mIdeal = (12*standardDeviation*standardDeviation - boxCount*wl*wl - 4*boxCount*wl - 3*boxCount)/(-4*wl - 4);
		var m = UnityEngine.Mathf.Round(mIdeal);
		// var sigmaActual = Math.sqrt( (m*wl*wl + (n-m)*wu*wu - n)/12 );

		for(var i=0; i<boxCount; i++) {
			boxSizes.Add((int)(i<m?wl:wu));
		}
		return boxSizes.ToArray();
	}
}
