using System.Collections;
using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;

public class MathUtils {

	public enum BoundingBoxStyle {
		SQUARE,
		CIRCLE
	}

	public enum PerlinProjectionFunction {
		LINEAR,
		QUADRATIC, 
		QUADRATIC_FROM1
	}


	public float[,] AssignProjection(float[,] inputArray, PerlinProjectionFunction projectionFunction, float projectionActivation = 0f, float projectionDeactivation = 1f) {
		for (int x = 0; x < inputArray.GetLength (0); ++x) {
			for (int z = 0; z < inputArray.GetLength (1); ++z) {	
				if (inputArray[x,z] >= projectionActivation && inputArray[x,z] <= projectionDeactivation) {
					switch (projectionFunction) {
						case PerlinProjectionFunction.QUADRATIC:
							inputArray [x, z] = Mathf.Pow (inputArray [x, z], 2);
							break;
						case PerlinProjectionFunction.QUADRATIC_FROM1:
							inputArray [x, z] = Mathf.Lerp (0f, 1f, Mathf.Pow (inputArray [x, z] * 10, 2));
							break;
						default:
							break;
					}
				}
			}
		}
		return inputArray;
	}

//	http://answers.unity3d.com/questions/805970/intvector2-struct-useful.html
	//todo: make custom Vector2 and Vector3 -- Vector2Int and Vector3Int!!

//	public float[,] AddValuesToValueMap(float[,] valueMap, float[,] addValueMap, float rangeMin, float rangeMax) {
//		for (int x = 0; x < valueMap.GetLength (0); ++x) {
//			for (int z = 0; z < valueMap.GetLength (1); ++z) {	
//				valueMap [x, z] = Mathf.Clamp (valueMap [x, z] + addValueMap [x, z], rangeMin, rangeMax);
//
//			}
//		}
//		return valueMap;
//	

	public float[,] ConvertGraphEdgesToValueMap(Vector3[,] edgesStartEndPositions, Vector3 mapResolutions, Vector3 graphResolutions, float edgeSizeMultiplier) {
		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;

		float[,] valueMap = new float[(int)mapResolutions.x, (int)mapResolutions.z];

		//iterate over every graph node
		for (int i = 0; i < edgesStartEndPositions.GetLength (0); ++i) {
			Vector3 startpos = edgesStartEndPositions [i, 0];
			Vector3 endpos = edgesStartEndPositions [i, 1];

			startpos.x = startpos.x * graphEntitiesDistanceX;
			startpos.z = startpos.z * graphEntitiesDistanceZ;

			startpos.x += graphEntitiesDistanceX / 2f;
			startpos.z += graphEntitiesDistanceZ / 2f;

			endpos.x = endpos.x * graphEntitiesDistanceX;
			endpos.z = endpos.z * graphEntitiesDistanceZ;

			endpos.x += graphEntitiesDistanceX / 2f;
			endpos.z += graphEntitiesDistanceZ / 2f;


			//create a line 
			Vector3[] line = BresenhamAlgorithm3DIntLinear (startpos, endpos, 0, 0f, mapResolutions.y);

			//iterate over every line (create thickness)
			List<Vector3> linePointWithThickess = new List<Vector3> ();
			for (int l = 0; l < line.Length; ++l) {

				linePointWithThickess.Clear ();
				linePointWithThickess.AddRange (
					MidPointCircle3dLinear (
						(int)line[l].x, (int)line[l].z,
						(int)(Mathf.Min (mapResolutions.x / graphResolutions.x, mapResolutions.z / graphResolutions.z) * edgeSizeMultiplier / 2),
						0, 0,
						(int)mapResolutions.x -1, 
						Mathf.InverseLerp (0f, mapResolutions.y, line[l].y * (1 + edgeSizeMultiplier/3f)),
//						line[l].y,
						(int)mapResolutions.z -1,
						false
					)
				);

				for (int t = 0; t < linePointWithThickess.Count; ++t) {
					valueMap [(int)linePointWithThickess [t].x, (int)linePointWithThickess [t].z] += linePointWithThickess [t].y;
				}

			}
		}

		return valueMap;
	}

	public float[,] ConvertGraphNodesToValueMap(Vector3[] graphNodesPositions, Vector3 mapResolutions, Vector3 graphResolutions, float sizeMultiplier, BoundingBoxStyle boundingBoxStyle = BoundingBoxStyle.CIRCLE) {

		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;

		float[] ranges = FindRangesMinMax (graphNodesPositions);
		Debug.Log (ranges [0] + "|" + ranges [1] + "|" + ranges [2] + "|" + ranges [3]);

		float[,] valueMap = new float[(int)mapResolutions.x, (int)mapResolutions.z];


		float valX = 0;
		float valZ = 0;
		List<Vector3> nodesBoundingBoxValues = new List<Vector3>();
		for (int i = 0; i < graphNodesPositions.Length; ++i) {
			valX = graphNodesPositions [i].x * graphEntitiesDistanceX;
			valZ = graphNodesPositions [i].z * graphEntitiesDistanceZ;

			valX += graphEntitiesDistanceX / 2f;
			valZ += graphEntitiesDistanceZ / 2f;

			nodesBoundingBoxValues.Clear ();

			if (boundingBoxStyle == BoundingBoxStyle.CIRCLE) {
				nodesBoundingBoxValues.AddRange (
					MidPointCircle3dLinear (
						(int)valX, (int)valZ,
						(int)(Mathf.Min (mapResolutions.x / graphResolutions.x, mapResolutions.z / graphResolutions.z) * sizeMultiplier / 2),
						0,
						0f,
						(int)mapResolutions.x - 1,
						Mathf.InverseLerp (0f, mapResolutions.y, graphNodesPositions [i].y),
						(int)mapResolutions.z - 1,
						false
					)
				);
			} else if (boundingBoxStyle == BoundingBoxStyle.SQUARE) {
								
				//	TODO!

				}


			for (int n = 0; n < nodesBoundingBoxValues.Count; ++n) {
				valueMap [(int)nodesBoundingBoxValues [n].x, (int)nodesBoundingBoxValues [n].z] += nodesBoundingBoxValues [n].y;
			}

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

	//	//todo: make this return array, not list (overhead)
	public Vector3[] BresenhamAlgorithm3DIntLinear(Vector3 lineStart, Vector3 lineEnd, int lineThickness = 1, float inputYRangeMin = 0, float inputYRangeMax = 50, float outputYRangeMin = 0, float outputYRangeMax = 1) {

		List<Vector3> line = new List<Vector3> ();
		Vector3 tileInLine = Vector3.zero;

		float properOutputYRangeMin = Mathf.Lerp(
			outputYRangeMin, 
			outputYRangeMax, 
			Mathf.InverseLerp (inputYRangeMin, inputYRangeMax, lineStart.y) 
		);

		float properOutputYRangeMax = Mathf.Lerp(
			outputYRangeMin, 
			outputYRangeMax, 
			Mathf.InverseLerp (inputYRangeMin, inputYRangeMax, lineEnd.y) 
		);

		int x = Mathf.FloorToInt (lineStart.x);
		int z = Mathf.FloorToInt (lineStart.z);
		float y = lineStart.y;

		int dx = Mathf.CeilToInt (lineEnd.x - lineStart.x);
		int dz = Mathf.CeilToInt (lineEnd.z - lineStart.z);
		float dy = lineEnd.y - lineStart.y;
	

		int incrementValue = Math.Sign (dx);
		int gradientIncrementValue = Math.Sign (dz);

		int longerCalculationSide, shorterCalculationSide;
		bool sidesInversion;

		if (Math.Abs (dx) < Math.Abs (dz)) {
			sidesInversion = true;
			longerCalculationSide = Math.Abs (dz);
			shorterCalculationSide = Math.Abs (dx);
			incrementValue = Math.Sign (dz);
			gradientIncrementValue = Math.Sign (dx);
			y = lineEnd.y;
			dy = -dy;
			float temp = properOutputYRangeMin;
			properOutputYRangeMin = outputYRangeMax;
			properOutputYRangeMax = temp;
		} else {
			sidesInversion = false;
			longerCalculationSide = Math.Abs (dx);
			shorterCalculationSide = Math.Abs (dz);
		}

		dy /= (float)longerCalculationSide;
		int gradientAccumulation = longerCalculationSide / 2;

		for (int i = 0; i < longerCalculationSide; ++i) {
			float tempY = Mathf.Lerp (
				properOutputYRangeMin, 
				properOutputYRangeMax, 
				Mathf.InverseLerp (lineStart.y, lineEnd.y, y + (i*dy)) //probably here is inverted something
			);

			tileInLine.Set (x, tempY, z);
			line.Add (tileInLine);

			for (int t = (x-lineThickness); t < (x+lineThickness); ++t) {
				tileInLine.Set (t, tempY, t);
				line.Add (tileInLine);
			}

			if (sidesInversion) { z += incrementValue; } 
			else { x += incrementValue; }

			gradientAccumulation += shorterCalculationSide;
			if (gradientAccumulation >= longerCalculationSide) {

				if (sidesInversion) { x += gradientIncrementValue; } 
				else { z += gradientIncrementValue; }
				gradientAccumulation -= longerCalculationSide;
			}

		}

		return line.ToArray ();
	}

//	//todo: make this return array, not list (overhead)


	private float[] RangeEvenSteps(float valueA, float valueB, int steps) {
		float[] rangeEvenSteps = new float[steps + 1];

		float deltaRange = valueB - valueA;
		float step = deltaRange / (float)steps;

		for (int s = 0; s <= steps; ++s) {
			rangeEvenSteps [s] = valueA + (s * step);
		}

		return rangeEvenSteps;
	}

	private float[] RangeEvenSteps(float valueA, float valueB, int steps, float outputValueMin, float outputValueMax) {
		float[] rangeEvenSteps = new float[steps + 1];

		float deltaRange = valueB - valueA;
		float step = deltaRange / (float)steps;

		for (int s = 0; s <= steps; ++s) {
			rangeEvenSteps [s] = Mathf.Lerp (
				outputValueMin, 
				outputValueMax, 
				Mathf.InverseLerp (valueA, valueB, valueA + (s * step))
			);
		}

		return rangeEvenSteps;
	}


	/**
	  Returning list (list) of square elements in 2d space, which are creating a 'circle' of given radius
	  (circleRadius) given certain 'Mid-Point', that is center of the circle of (X, Z) coordinates of
	  (midPointX, midPointZ).
	 */
	public Vector2[] MidPointCircle (int midPointX, int midPointZ, int circleRadius, bool excludeMidPoint = false) {
		List<Vector2> list = new List<Vector2> ();
		Vector2 listElement = new Vector2 ();

		for (int x = midPointX - circleRadius; x <= midPointX + circleRadius; ++x) {
			for (int z = midPointZ - circleRadius; z <= midPointZ + circleRadius; ++z) {
				listElement = VectorsDifference (midPointX, midPointZ, x, z);
				if (Mathf.Pow (listElement.x, 2) + Mathf.Pow (listElement.y, 2) <= Mathf.Pow (circleRadius, 2)) {
					listElement.Set (x, z);
					list.Add (listElement);
				}
			}
		}

		if (excludeMidPoint) { list.Remove (new Vector2 (midPointX, midPointZ)); }
		return list.ToArray ();
	}
		
	public Vector2[] MidPointCircle (int midPointX, int midPointZ, int circleRadius, int min, int maxX, int maxZ, bool excludeMidPoint = false){
		List<Vector2> list = new List<Vector2> ();
		Vector2 listElement = new Vector2 ();

		for (int x = midPointX - circleRadius; x <= midPointX + circleRadius; ++x) {
			for (int z = midPointZ - circleRadius; z <= midPointZ + circleRadius; ++z) {
				listElement = VectorsDifference (midPointX, midPointZ, x, z);
				if (Mathf.Pow (listElement.x, 2) + Mathf.Pow (listElement.y, 2) <= Mathf.Pow (circleRadius, 2)) {
					listElement.Set (Mathf.Clamp (x, min, maxX),Mathf.Clamp (z, min, maxZ));
					list.Add (listElement);
				}
			}
		}

		if (excludeMidPoint) { list.Remove (new Vector2 (midPointX, midPointZ)); }
		return list.ToArray ();
	}

	public Vector3[] MidPointCircle3d (int midPointX, int midPointZ, float circleYVal, int circleRadius, int min, int maxX, int maxZ, bool excludeMidPoint = false) {
		List<Vector3> list = new List<Vector3> ();
		Vector3 listElement = new Vector3 ();

		for (int x = midPointX - circleRadius; x <= midPointX + circleRadius; ++x) {
			for (int z = midPointZ - circleRadius; z <= midPointZ + circleRadius; ++z) {
				listElement = VectorsDifference3d (midPointX, midPointZ, x, z);
				if (Mathf.Pow (listElement.x, 2) + Mathf.Pow (listElement.y, 2) <= Mathf.Pow (circleRadius, 2)) {
					if (x >= min && x < maxX && z >= min && z < maxZ) {
						listElement.Set (x, circleYVal, z);
					}
					list.Add (listElement);
				}
			}
		}

		if (excludeMidPoint) { list.Remove (new Vector2 (midPointX, midPointZ)); }
		return list.ToArray ();
	}

	public Vector3[] MidPointCircle3dRandom (int midPointX, int midPointZ, int circleRadius, int minXZ, float minY, int maxX, float maxY, int maxZ, bool excludeMidPoint = false) {
		List<Vector3> list = new List<Vector3> ();
		Vector3 listElement = new Vector3 ();

		for (int x = midPointX - circleRadius; x <= midPointX + circleRadius; ++x) {
			for (int z = midPointZ - circleRadius; z <= midPointZ + circleRadius; ++z) {
				listElement = VectorsDifference3d (midPointX, midPointZ, x, z);
				if (Mathf.Pow (listElement.x, 2) + Mathf.Pow (listElement.y, 2) <= Mathf.Pow (circleRadius, 2)) {
					if (x >= minXZ && x < maxX && z >= minXZ && z < maxZ) {
						listElement.Set (x, UnityEngine.Random.Range (minY, maxY), z);
					}
					list.Add (listElement);
				}
			}
		}

		if (excludeMidPoint) { list.Remove (new Vector2 (midPointX, midPointZ)); }
		return list.ToArray ();
	}

	public Vector3[] MidPointCircle3dLinear (int midPointX, int midPointZ, int circleRadius, int minXZ, float minY, int maxX, float maxY, int maxZ, bool excludeMidPoint = false) {
		List<Vector3> list = new List<Vector3> ();
		Vector3 listElement = new Vector3 ();


		for (int x = midPointX - circleRadius; x <= midPointX + circleRadius; ++x) {
			for (int z = midPointZ - circleRadius; z <= midPointZ + circleRadius; ++z) {
				listElement = VectorsDifference3d (midPointX, midPointZ, x, z);
				if (Mathf.Pow (listElement.x, 2) + Mathf.Pow (listElement.y, 2) <= Mathf.Pow (circleRadius, 2)) {
					if (x >= minXZ && x < maxX && z >= minXZ && z < maxZ) {
						listElement.Set (x, Mathf.Lerp(maxY, minY, Mathf.InverseLerp (0, Mathf.Pow(circleRadius, 2), listElement.sqrMagnitude)), z);
					}
					list.Add (listElement);
				}
			}
		}

		if (excludeMidPoint) { list.Remove (new Vector2 (midPointX, midPointZ)); }
		return list.ToArray ();
	}

	/**
	  Returning list (list) of square elements in 2d space, which are creating a square of given radius
	  (squareRadius) given certain 'Mid-Point', that is center of the square of (X, Z) coordinates of
	  (midPointX, midPointZ).
	 */
	public List<Vector2> MidPointSquare (int midPointX, int midPointZ, int squareRadius){
		List<Vector2> list = new List<Vector2> ();
		Vector2 listElement = new Vector2 ();

		for (int x = midPointX - squareRadius; x <= midPointX + squareRadius; ++x) {
			for (int y = midPointZ - squareRadius; y <= midPointZ + squareRadius; ++y) {
				listElement.Set (x, y);
				list.Add (listElement);
			}
		}

		return list;
	}

	/**
	  Returning list (list) of square elements in 2d space, which are creating a square of given radius
	  (squareRadius) given certain 'Mid-Point', that is center of the square of (X, Z) coordinates of
	  (midPointX, midPointZ).
	  List of coordinates is cropped when conditions for minimum and maximum index are not met, preventing 
	  IndexOutOfRange exception. 
	  Minimum allowed coordinate pair for any given element in 2d space (X, Z) is determined by (min, min) parameter,
	  and maximum allowed coordinate pair (X, Z) is (maxX, maxZ)
	 */
	public List<Vector2> MidPointSquare (int midPointX, int midPointZ, int squareRadius, int min, int maxX, int maxZ){
		List<Vector2> coords = MidPointSquare (midPointX, midPointZ, squareRadius);
		Vector2 coord;
		for (int c = 0; c < coords.Count; c++) {
			coord = coords[c];
			if (coord.x < min || coord.x >= maxX || coord.y < min || coord.y >= maxZ) {
				coords.RemoveAt (c);
				--c;
			}
		}

		return coords;
	}

	/** 
	  I may have trouble adding 2+2 sometimes, but I figured out that if you want to get
	  MAXIMUM number of grid elements inside circle of (circleRadius)'s radius (eg. 3),
	  then you have to go like this: (4*3 + 4*2 + 4*1).
	 */
	public int MidPointCircleMaxElements (int circleRadius){
		int elements = 0;

		for (int i = circleRadius; i >= 1; --i) {
			elements += 4 * i;
		}
		return elements;
	}

	/**
	 * Likewise for square based area, but base number for calculations is number 8.
	 * So, maximum number of grid elements inside square of (squareRadius)'s radius (eg. 4)
	 * is (8*4 + 8*3 + 8*2 + 8*1).
	 */ 
	public int MidPointSquareMaxElements (int squareRadius){
		int maxElements = 0;
		for (int i = squareRadius; i >= 1; --i) {
			maxElements += 8 * i;
		}

		return maxElements;
	}

	/**
	 * Returning vector length as double precision float type value.
	 */ 
	public double VectorLength (Vector2 vector){
		return Math.Sqrt (Math.Pow (vector.x, 2) + Math.Pow (vector.y, 2));
	}

	/**
	  Returning list (list) of square elements in 2d space, which are creating a square of given radius
	  (squareRadius) given certain 'Mid-Point', that is center of the square of (X, Z) coordinates of
	  (midPointX, midPointZ).
	  List of coordinates is cropped when conditions for minimum and maximum index are not met, preventing 
	  IndexOutOfRange exception. 
	  Minimum allowed coordinate pair for any given element in 2d space (X, Z) is determined by (min, min) parameter,
	  and maximum allowed coordinate pair (X, Z) is (maxX, maxZ)
	  Additionally, if (excludeMidPoint) flag is 'true', then center of the square is not present in output coordinate list.
	 */
	public List<Vector2> MidPointSquare (int midPointX, int midPointZ, int squareRadius, int min, int maxX, int maxZ, bool excludeMidPoint){
		if (!excludeMidPoint) {
			return MidPointSquare (midPointX, midPointZ, squareRadius, min, maxX, maxZ);
		}
		List<Vector2> coords = MidPointSquare (midPointX, midPointZ, squareRadius, min, maxX, maxZ);
		coords.Remove (new Vector2 (midPointX, midPointZ));
		return coords;
	}

	public Vector2 VectorsDifference (float vecAX, float vecAY, float vecBX, float vecBY) {
		return new Vector2 (Mathf.Abs (vecAX - vecBX), Mathf.Abs (vecAY - vecBY));
	}

	public Vector3 VectorsDifference3d (float vecAX, float vecAY, float vecBX, float vecBY) {
		return new Vector3 (Mathf.Abs (vecAX - vecBX), 0f, Mathf.Abs (vecAY - vecBY));
	}

	//todo: this in another class
	public enum MergeArrayMode {
		ADD,
		ADD_MULTIPLIER,
		ADD_MULTIPLIER_LIMIT,
		SUBTRACT,
		SUBTRACT_MULTIPLIER,
		SUBTRACT_MULTIPLIER_LIMIT,
		XOR, 
	}


//	public float[,] ResizeArray(float[,] inputArray, int outputArrayDimensionX, int outputArrayDimensionZ) {
//		
//	}

	public float[,] MergeArrays(
		float[,] baseLayer, float[,] topAlphaLayer, 
		float rangeYMin = 0f, float rangeYMax = 1f, 
		MergeArrayMode mergeMode = MergeArrayMode.XOR, float mergeModeMultiplier = 1f, bool graphMergeLimitLand = true, float graphMergeLimitLandValue = 0f) {


		int layersScaleDifferenceX = Mathf.RoundToInt (topAlphaLayer.GetLength (0) / (float)baseLayer.GetLength (0));
		int layersScaleDifferenceZ = Mathf.RoundToInt (topAlphaLayer.GetLength (1) / (float)baseLayer.GetLength (1));

		float topAlphaLayerValueSum;
		float topAlphaLayerValue;

		for(int x = 0; x < baseLayer.GetLength (0); ++x){
			for(int z = 0; z < baseLayer.GetLength (1); ++z){

				topAlphaLayerValue = 0;
				topAlphaLayerValueSum = 0;
//				topAlphaLayerValue = topAlphaLayer [x, z];

				for (int scalingX = 0; scalingX < layersScaleDifferenceX; ++scalingX) {
					for (int scalingZ = 0; scalingZ < layersScaleDifferenceZ; ++scalingZ) {
						int scalingSampleX = x * layersScaleDifferenceX + scalingX;
						int scalingSampleZ = z * layersScaleDifferenceZ + scalingZ;
						if (scalingSampleX < topAlphaLayer.GetLength (0) && scalingSampleZ < topAlphaLayer.GetLength (1)) {
							try {
								topAlphaLayerValueSum += topAlphaLayer [scalingSampleX, scalingSampleZ];
							} catch(IndexOutOfRangeException exc) {
								topAlphaLayerValue += 0f;
							}
						} 

					}
				}

				topAlphaLayerValue = topAlphaLayerValueSum;
				topAlphaLayerValue /= (float) (layersScaleDifferenceX * layersScaleDifferenceZ);
//
//				Debug.Log ("topalphalayer val: " + topAlphaLayerValue);
				
				switch(mergeMode) {
					case MergeArrayMode.XOR:
						if (baseLayer[x,z] == 0) {
							baseLayer [x, z] = Mathf.Lerp(
								rangeYMin, 
								rangeYMax, 
								topAlphaLayerValue
							);
						}
						break;

					case MergeArrayMode.ADD: 
					case MergeArrayMode.ADD_MULTIPLIER:
						baseLayer [x, z] = Mathf.Lerp (
							rangeYMin, 
							rangeYMax, 
							baseLayer [x, z] + (mergeModeMultiplier * topAlphaLayerValue)
						);
						break;

					case MergeArrayMode.SUBTRACT:
					case MergeArrayMode.SUBTRACT_MULTIPLIER:
						baseLayer [x, z] = Mathf.Lerp (
							rangeYMin, 
							rangeYMax, 
							baseLayer [x, z] - (mergeModeMultiplier * topAlphaLayerValue)
						);
						break;

					case MergeArrayMode.ADD_MULTIPLIER_LIMIT:
						baseLayer [x, z] = Mathf.Lerp (
							rangeYMin, 
							rangeYMax, 
							baseLayer [x, z] + (mergeModeMultiplier * topAlphaLayerValue)
						);
						baseLayer [x, z] = Mathf.Clamp (baseLayer [x, z], graphMergeLimitLandValue, rangeYMax);
						break;

					case MergeArrayMode.SUBTRACT_MULTIPLIER_LIMIT:
						baseLayer [x, z] = Mathf.Lerp (
							rangeYMin, 
							rangeYMax, 
							baseLayer [x, z] - (mergeModeMultiplier * topAlphaLayerValue)
						);
						baseLayer [x, z] = Mathf.Clamp (baseLayer [x, z], graphMergeLimitLandValue, rangeYMax);
						break;
				}

			}
		}

		return baseLayer;
	}

	public bool ContainsXZ(List<Vector3> container, Vector3 value) {
		for (int i = 0;i < container.Count; ++i) {
			if (container[i].x == value.x && container[i].z == value.z) {
				return true;
			}
		}

		return false;
	}

}
