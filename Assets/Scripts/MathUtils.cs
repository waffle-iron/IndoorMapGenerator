using System.Collections;
using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

public class MathUtils {

//	http://answers.unity3d.com/questions/805970/intvector2-struct-useful.html
	//todo: make custom Vector2 and Vector3 -- Vector2Int and Vector3Int!!

	public float[,] AddValuesToValueMap(float[,] valueMap, float[,] addValueMap, float rangeMin, float rangeMax) {
		for (int x = 0; x < valueMap.GetLength (0); ++x) {
			for (int z = 0; z < valueMap.GetLength (1); ++z) {	
				valueMap [x, z] = Mathf.Clamp (valueMap [x, z] + addValueMap [x, z], rangeMin, rangeMax);

			}
		}

		return valueMap;
	}

	public float[,] ConvertGraphEdgesToValueMap(Vector3[,] edgesStartEndPositions, Vector3 mapResolutions, Vector3 graphResolutions) {

		float[] asd = RangeEvenSteps (8, 10, 10);
		float[] asd2 = RangeEvenSteps (10, 8, 10);
		float[] asd3 = RangeEvenSteps (-10, -8, 10);
		float[] asd4 = RangeEvenSteps (-8, -10, 10);

		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;

		float[,] valueMap = new float[(int)mapResolutions.x, (int)mapResolutions.z];


		for (int i=0; i < edgesStartEndPositions.GetLength(0); ++i) {
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


			Vector3[] line = BresenhamAlgorithm3DIntLinear (
				startpos,
				endpos,
				0,
				mapResolutions.y
			);

			int boundingBoxRadius = 3;
			float valX = -1;
			float valZ = -1;
			int l = -1;
			try {
			for (l = 0; l < line.Length; ++l) {

				valX = line[l].x;
				valZ = line[l].z;
//				float valX = line[l].x * graphEntitiesDistanceX;
//				float valZ = line[l].z * graphEntitiesDistanceZ;
//
//				valX += graphEntitiesDistanceX / 2f;
//				valZ += graphEntitiesDistanceZ / 2f;

//				for (float x = Mathf.Clamp (valX-boundingBoxRadius, 0, mapResolutions.x); 
//					x < Mathf.Clamp (valX+boundingBoxRadius, 0, mapResolutions.x);
//					++x) {
//					for (float z = Mathf.Clamp (valZ-boundingBoxRadius, 0, mapResolutions.z);
//						z < Mathf.Clamp (valZ+boundingBoxRadius, 0, mapResolutions.z);
//						++z) {
//						valueMap [(int)x, (int)z] = Mathf.Lerp (
//							0f, 
//							1f, 
//							Mathf.InverseLerp (0f, mapResolutions.y, line[l].y)
//						);
//					}
//				}

				valueMap [Mathf.FloorToInt (line[l].x), Mathf.FloorToInt (line[l].z)] = line [l].y;
			}
			} catch (IndexOutOfRangeException exc) {
				Debug.LogError (
					"[i:("+i+"), startpos:" + startpos.ToString () + ", endpos: " + endpos.ToString () +"], " +
					"[l:(" + l + "), " + "(" + (int)valX +"), " + "(" + (int)valZ +"]), \t" + exc.StackTrace);
			}
		}

		return valueMap;
	}

	public float[,] ConvertGraphToValueMap(Vector3[] graphNodesPositions, Vector3 mapResolutions, Vector3 graphResolutions) {

		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;

		float[] ranges = FindRangesMinMax (graphNodesPositions);
		Debug.Log (ranges [0] + "|" + ranges [1] + "|" + ranges [2] + "|" + ranges [3]);

		float[,] valueMap = new float[(int)mapResolutions.x, (int)mapResolutions.z];

		int i = 0;
		float valX = 0;
		float valZ = 0;
		int boundingBoxRadius = 3;
//		try {
			for (i = 0; i < graphNodesPositions.Length; ++i) {
				valX = graphNodesPositions[i].x * graphEntitiesDistanceX;
				valZ = graphNodesPositions[i].z * graphEntitiesDistanceZ;

				valX += graphEntitiesDistanceX / 2f;
				valZ += graphEntitiesDistanceZ / 2f;

				//creating box with radius of (boundingBoxRadius)
				for (float x = Mathf.Clamp (valX-boundingBoxRadius, 0, mapResolutions.x); 
						x < Mathf.Clamp (valX+boundingBoxRadius, 0, mapResolutions.x);
						++x) {
					for (float z = Mathf.Clamp (valZ-boundingBoxRadius, 0, mapResolutions.z);
							z < Mathf.Clamp (valZ+boundingBoxRadius, 0, mapResolutions.z);
							++z) {
						valueMap [(int)x, (int)z] = Mathf.Lerp (
							0f, 
							1f, 
							Mathf.InverseLerp (0f, mapResolutions.y, graphNodesPositions[i].y)
						);
					}
				}
					
			}
//		} catch (IndexOutOfRangeException exc) {
////			Debug.LogError ("(" + i + "), " + "(" + (int)valX +"), " + "(" + (int)valZ +"), " + exc.StackTrace);
//		}
			
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

	public Vector3[] BresenhamAlgorithm3DIntLinear(Vector3 lineStart, Vector3 lineEnd, float inputYRangeMin = 0, float inputYRangeMax = 50, float outputYRangeMin = 0, float outputYRangeMax = 1) {
//		float[] rangeEvenSteps = new float[steps + 1];
//
//		float deltaRange = valueB - valueA;
//		float step = deltaRange / (float)steps;
//
//		for (int s = 0; s <= steps; ++s) {
//			rangeEvenSteps [s] = valueA + (s * step);
//		}
//
//		return rangeEvenSteps;

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
			if (x != lineStart.x && z != lineEnd.z) {
				line.Add (tileInLine);
			}

			if (sidesInversion) { z += incrementValue; } 
			else { x += incrementValue; }

			gradientAccumulation += shorterCalculationSide;
			if (gradientAccumulation >= longerCalculationSide) {
				if (sidesInversion) { x += gradientIncrementValue; } 
				else { z += gradientIncrementValue; }
//				dy += dy;
				gradientAccumulation -= longerCalculationSide;
			}

		}

		return line.ToArray ();
	}


	//todo: make this return array, not list (overhead)
	public List<Vector2> BresenhamAlgorithm2DInt (Vector2 lineStart, Vector2 lineEnd) {
		List<Vector2> line = new List<Vector2> ();
		Vector2 tileInLine = Vector2.zero;

		//Vector coordinates (x, y) that we will start computation from
		int x = Mathf.FloorToInt (lineStart.x);
		int y = Mathf.FloorToInt (lineStart.y);

		//difference in length between points in the same dimension (x or y)
		int dx = Mathf.CeilToInt (lineEnd.x - lineStart.x);
		int dy = Mathf.CeilToInt (lineEnd.y - lineStart.y);

		//checking whether we should ADD "+1" (to longerCalculationSide dimension value)
		//in every algorithm iteration or SUBTRACT "-1" from it.
		//(varying depending on lineStart and lineEnd position on a grid)
		int incrementValue = Math.Sign (dx);
		int gradientIncrementValue = Math.Sign (dy);

		int longerCalculationSide, shorterCalculationSide;
		bool sidesInversion;

		//if distance from line starting and ending point on Y-axis is greater than
		//distance on X-axis, then we iterate this algorithm other way around.
		//(incrementing y values and checking for boundary condition for ++x bump,
		// instead of incrementing x values and checking if ++y bump is valid).
		if (Math.Abs (dx) < Math.Abs (dy)) {
			sidesInversion = true;
			longerCalculationSide = Math.Abs (dy);
			shorterCalculationSide = Math.Abs (dx);
			incrementValue = Math.Sign (dy);
			gradientIncrementValue = Math.Sign (dx);
		} else {
			sidesInversion = false;
			longerCalculationSide = Math.Abs (dx);
			shorterCalculationSide = Math.Abs (dy);
		}

		//because reasons, check wiki for algorithm walkthrough
		int gradientAccumulation = longerCalculationSide / 2;

		for (int i = 0; i < longerCalculationSide; ++i) {
			tileInLine.Set (x, y);
			if (x != lineStart.x && y != lineEnd.y) {
				line.Add (tileInLine);
			}

			if (sidesInversion) { y += incrementValue; } 
			else { x += incrementValue; }

			gradientAccumulation += shorterCalculationSide;
			if (gradientAccumulation >= longerCalculationSide) {
				if (sidesInversion) { x += gradientIncrementValue; } 
				else { y += gradientIncrementValue; }
				gradientAccumulation -= longerCalculationSide;
			}

		}
		return line;
	}

//	public List<Vector2> BresenhamAlgorithm2DInt (Vector2 lineStart, Vector2 lineEnd) {
//		List<Vector2> line = new List<Vector2> ();
//		Vector2 tileInLine = Vector2.zero;
//
//		//Vector coordinates (x, y) that we will start computation from
//		int x = Mathf.FloorToInt (lineStart.x);
//		int y = Mathf.FloorToInt (lineStart.y);
//
//		//difference in length between points in the same dimension (x or y)
//		int dx = Mathf.CeilToInt (lineEnd.x - lineStart.x);
//		int dy = Mathf.CeilToInt (lineEnd.y - lineStart.y);
//
//		//checking whether we should ADD "+1" (to longerCalculationSide dimension value)
//		//in every algorithm iteration or SUBTRACT "-1" from it.
//		//(varying depending on lineStart and lineEnd position on a grid)
//		int incrementValue = Math.Sign (dx);
//		int gradientIncrementValue = Math.Sign (dy);
//
//		int longerCalculationSide, shorterCalculationSide;
//		bool sidesInversion;
//
//		//if distance from line starting and ending point on Y-axis is greater than
//		//distance on X-axis, then we iterate this algorithm other way around.
//		//(incrementing y values and checking for boundary condition for ++x bump,
//		// instead of incrementing x values and checking if ++y bump is valid).
//		if (Math.Abs (dx) < Math.Abs (dy)) {
//			sidesInversion = true;
//			longerCalculationSide = Math.Abs (dy);
//			shorterCalculationSide = Math.Abs (dx);
//			incrementValue = Math.Sign (dy);
//			gradientIncrementValue = Math.Sign (dx);
//		} else {
//			sidesInversion = false;
//			longerCalculationSide = Math.Abs (dx);
//			shorterCalculationSide = Math.Abs (dy);
//		}
//
//		//because reasons, check wiki for algorithm walkthrough
//		int gradientAccumulation = longerCalculationSide / 2;
//
//		for (int i = 0; i < longerCalculationSide; ++i) {
//			tileInLine.Set (x, y);
//			if (x != lineStart.x && y != lineEnd.y) {
//				line.Add (tileInLine);
//			}
//
//			if (sidesInversion) { y += incrementValue; } 
//			else { x += incrementValue; }
//
//			gradientAccumulation += shorterCalculationSide;
//			if (gradientAccumulation >= longerCalculationSide) {
//				if (sidesInversion) { x += gradientIncrementValue; } 
//				else { y += gradientIncrementValue; }
//				gradientAccumulation -= longerCalculationSide;
//			}
//
//		}
//		return line;
//	}


	//todo: does this do anything?
	public List<Vector2> BresenhamAlgorithm2DFloat (Vector2 lineStart, Vector2 lineEnd) {
		List<Vector2> line = new List<Vector2> ();
		Vector2 tileInLine = Vector2.zero;

		//Vector coordinates (x, y) that we will start computation from
		float x = Mathf.FloorToInt (lineStart.x);
		float y = Mathf.FloorToInt (lineStart.y);

		//difference in length between points in the same dimension (x or y)
		float dx = Mathf.CeilToInt (lineEnd.x - lineStart.x);
		float dy = Mathf.CeilToInt (lineEnd.y - lineStart.y);

		//checking whether we should ADD "+1" (to longerCalculationSide dimension value)
		//in every algorithm iteration or SUBTRACT "-1" from it.
		//(varying depending on lineStart and lineEnd position on a grid)
		float incrementValue = Math.Sign (dx);
		float gradientIncrementValue = Math.Sign (dy);

		float longerCalculationSide, shorterCalculationSide;
		bool sidesInversion;

		//if distance from line starting and ending point on Y-axis is greater than
		//distance on X-axis, then we iterate this algorithm other way around.
		//(incrementing y values and checking for boundary condition for ++x bump,
		// instead of incrementing x values and checking if ++y bump is valid).
		if (Math.Abs (dx) < Math.Abs (dy)) {
			sidesInversion = true;
			longerCalculationSide = Math.Abs (dy);
			shorterCalculationSide = Math.Abs (dx);
			incrementValue = Math.Sign (dy);
			gradientIncrementValue = Math.Sign (dx);
		} else {
			sidesInversion = false;
			longerCalculationSide = Math.Abs (dx);
			shorterCalculationSide = Math.Abs (dy);
		}

		//because reasons, check wiki for algorithm walkthrough
		float gradientAccumulation = longerCalculationSide / 2;

		for (int i = 0; i < longerCalculationSide; ++i) {
			tileInLine.Set (x, y);
			if (x != lineStart.x && y != lineEnd.y) {
				line.Add (tileInLine);
			}

			if (sidesInversion) { y += incrementValue; } 
			else { x += incrementValue; }

			gradientAccumulation += shorterCalculationSide;
			if (gradientAccumulation >= longerCalculationSide) {
				if (sidesInversion) { x += gradientIncrementValue; } 
				else { y += gradientIncrementValue; }
				gradientAccumulation -= longerCalculationSide;
			}

		}
		return line;
	}

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
	public List<Vector2> MidPointCircle (int midPointX, int midPointZ, int circleRadius) {
		List<Vector2> list = new List<Vector2> ();
		Vector2 listElement = new Vector2 ();

		for (int x = midPointX - circleRadius; x <= midPointX + circleRadius; ++x) {
			for (int y = midPointZ - circleRadius; y <= midPointZ + circleRadius; ++y) {
				listElement = VectorsDifference (midPointX, midPointZ, x, y);
				if (Mathf.Pow (listElement.x, 2) + Mathf.Pow (listElement.y, 2) <= Mathf.Pow (circleRadius, 2)) {
					listElement.Set (x, y);
					list.Add (listElement);
				}
			}
		}

		return list;
	}

	/**
	  Returning list (list) of square elements in 2d space, which are creating a 'circle' of given radius
	  (circleRadius) given certain 'Mid-Point', that is center of the circle of (X, Z) coordinates of
	  (midPointX, midPointZ).
	  List of coordinates is cropped when conditions for minimum and maximum index are not met, preventing 
	  IndexOutOfRange exception. 
	  Minimum allowed coordinate pair for any given element in 2d space (X, Z) is determined by (min, min) parameter,
	  and maximum allowed coordinate pair (X, Z) is (maxX, maxZ)
	 */
	public List<Vector2> MidPointCircle (int midPointX, int midPointZ, int circleRadius, int min, int maxX, int maxZ){
		List<Vector2> coords = MidPointCircle (midPointX, midPointZ, circleRadius);
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
	  Returning list (list) of square elements in 2d space, which are creating a 'circle' of given radius
	  (circleRadius) given certain 'Mid-Point', that is center of the circle of (X, Z) coordinates of
	  (midPointX, midPointZ).
	  List of coordinates is cropped when conditions for minimum and maximum index are not met, preventing 
	  IndexOutOfRange exception. 
	  Minimum allowed coordinate pair for any given element in 2d space (X, Z) is determined by (min, min) parameter,
	  and maximum allowed coordinate pair (X, Z) is (maxX, maxZ)
	  Additionally, if (excludeMidPoint) flag is 'true', then center of the circle is not present in output coordinate list.
	 */
	public List<Vector2> MidPointCircle (int midPointX, int midPointZ, int circleRadius, int min, int maxX, int maxZ, bool excludeMidPoint){
		if (!excludeMidPoint) {
			return MidPointCircle (midPointX, midPointZ, circleRadius, min, maxX, maxZ);
		}
		List<Vector2> coords = MidPointCircle (midPointX, midPointZ, circleRadius, min, maxX, maxZ);
		coords.Remove (new Vector2 (midPointX, midPointZ));
		return coords;
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

}
