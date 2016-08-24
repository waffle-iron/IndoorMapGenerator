using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class UtilsMath
{

	private static bool 	initialized = false;
	private static Random 	random;

	public static void Initialize()
	{
		initialized = true;
		random = new Random();
	}

	/**
		Cool algorithm for 'drawing' a line (starting from point lineStart and ending on lineEnd)
		on a unified grid of integer 2D values.
		caveat in Bresenham's algorithm is no support for antialiasing of a line.

		inputs:		(lineStart) - starting position for the line.
					(lineEnd) - ending position for the line.
		returning: 	list of tiles that must be traversed / coloured / drawed / whatever in order to
					travel from point (lineStart) to (lineEnd).
	*/
	public static List<Vector2> BresenhamAlgorithmInt(Vector2 lineStart, Vector2 lineEnd)
	{
		List<Vector2> line = new List<Vector2>();
		Vector2 tileInLine = Vector2.zero;

		//Vector coordinates (x, y) that we will start computation from
		int x = Mathf.FloorToInt(lineStart.x);
		int y = Mathf.FloorToInt(lineStart.y);

		//difference in length between points in the same dimension (x or y)
		int dx = Mathf.CeilToInt(lineEnd.x - lineStart.x);
		int dy = Mathf.CeilToInt(lineEnd.y - lineStart.y);

		//checking whether we should ADD "+1" (to longerCalculationSide dimension value)
		//in every algorithm iteration or SUBTRACT "-1" from it.
		//(varying depending on lineStart and lineEnd position on a grid)
		int incrementValue = Math.Sign(dx);
		int gradientIncrementValue = Math.Sign(dy);

		int  longerCalculationSide, shorterCalculationSide;
		bool sidesInversion;

		//if distance from line starting and ending point on Y-axis is greater than
		//distance on X-axis, then we iterate this algorithm other way around.
		//(incrementing y values and checking for boundary condition for ++x bump,
		// instead of incrementing x values and checking if ++y bump is valid).
		if (Math.Abs(dx) < Math.Abs(dy))
		{
			sidesInversion = true;
			longerCalculationSide = Math.Abs(dy);
			shorterCalculationSide = Math.Abs(dx);
			incrementValue = Math.Sign(dy);
			gradientIncrementValue = Math.Sign(dx);
		}
		else
		{
			sidesInversion = false;
			longerCalculationSide = Math.Abs(dx);
			shorterCalculationSide = Math.Abs(dy);
		}

		//because reasons, check wiki for algorithm walkthrough
		int gradientAccumulation = longerCalculationSide / 2;

		for (int i = 0; i < longerCalculationSide; ++i)
		{
			tileInLine.Set(x, y);
			if (x != lineStart.x && y != lineEnd.y)
			{
				line.Add(tileInLine);
			}

			if (sidesInversion)
			{
				y += incrementValue;
			}
			else
			{
				x += incrementValue;
			}

			gradientAccumulation += shorterCalculationSide;
			if (gradientAccumulation >= longerCalculationSide)
			{
				if (sidesInversion)
				{
					x += gradientIncrementValue;
				}
				else
				{
					y += gradientIncrementValue;
				}
				gradientAccumulation -= longerCalculationSide;
			}

		}
		return line;
	}

	/**
	  Returning list (list) of square elements in 2d space, which are creating a 'circle' of given radius
	  (circleRadius) given certain 'Mid-Point', that is center of the circle of (X, Z) coordinates of
	  (midPointX, midPointZ).
	 */
	public static List<Vector2> MidPointCircle(int midPointX, int midPointZ, int circleRadius)
	{
		List<Vector2> list = new List<Vector2>();
		Vector2 listElement = new Vector2();

		for (int x = midPointX - circleRadius; x <= midPointX + circleRadius; ++x)
		{
			for (int y = midPointZ - circleRadius; y <= midPointZ + circleRadius; ++y)
			{
				listElement = UtilsMath.VectorsDifference(midPointX, midPointZ, x, y);
				if (Mathf.Pow(listElement.x, 2) + Mathf.Pow(listElement.y, 2) <= Mathf.Pow(circleRadius, 2))
				{
					listElement.Set(x, y);
					list.Add(listElement);
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
	public static List<Vector2> MidPointCircle(int midPointX, int midPointZ, int circleRadius, int min, int maxX, int maxZ)
	{
		List<Vector2> coords = MidPointCircle(midPointX, midPointZ, circleRadius);
		Vector2 coord;
		for (int c = 0; c < coords.Count; c++)
		{
			coord = coords.ElementAt(c);
			if (coord.x < min || coord.x >= maxX || coord.y < min || coord.y >= maxZ)
			{
				coords.RemoveAt(c);
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
	public static List<Vector2> MidPointCircle(int midPointX, int midPointZ, int circleRadius, int min, int maxX, int maxZ, bool excludeMidPoint) {
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
	public static List<Vector2> MidPointSquare(int midPointX, int midPointZ, int squareRadius) {
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
	public static List<Vector2> MidPointSquare(int midPointX, int midPointZ, int squareRadius, int min, int maxX, int maxZ) {
		List<Vector2> coords = MidPointSquare (midPointX, midPointZ, squareRadius);
		Vector2 coord;
		for (int c = 0; c < coords.Count; c++) {
			coord = coords.ElementAt (c);
			if (coord.x < min || coord.x >= maxX || coord.y < min || coord.y >= maxZ)
			{
				coords.RemoveAt(c);
				--c;
			}
		}

		return coords;
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
	public static List<Vector2> MidPointSquare(int midPointX, int midPointZ, int squareRadius, int min, int maxX, int maxZ, bool excludeMidPoint) {
		if (!excludeMidPoint) {
			return MidPointSquare (midPointX, midPointZ, squareRadius, min, maxX, maxZ);
		}
		List<Vector2> coords = MidPointSquare (midPointX, midPointZ, squareRadius, min, maxX, maxZ);
		coords.Remove (new Vector2 (midPointX, midPointZ));
		return coords;
	}

	/** 
	  I may have trouble adding 2+2 sometimes, but I figured out that if you want to get
	  MAXIMUM number of grid elements inside circle of (circleRadius)'s radius (eg. 3),
	  then you have to go like this: (4*3 + 4*2 + 4*1).
	 */
	public static int MidPointCircleMaxElements(int circleRadius) {
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
	public static int MidPointSquareMaxElements(int squareRadius) {
		int maxElements = 0;
		for (int i = squareRadius; i >= 1; --i) {
			maxElements += 8 * i;
		}

		return maxElements;
	}

	/**
	 * Returning vector length as double precision float type value.
	 */ 
	public static double VectorLength(Vector2 vector)
	{
		return Math.Sqrt(Math.Pow(vector.x, 2) + Math.Pow(vector.y, 2));
	}

	/**
	 * Returning vector length as double precision float type value.
	 * 
	 * Returning value is cast to float type value.
	 */ 
	public static float VectorLengthToFloat(Vector2 vector)
	{
		return (float)VectorLength(vector);
	}

	/**
	 * Returning vector length as double precision float type value.
	 * 
	 * Returning value is cast to integer type value (with appropriate rounding).
	 */ 
	public static float VectorLengthToInt(Vector2 vector)
	{
		return Mathf.RoundToInt(VectorLengthToFloat(vector));
	}

	/**
	 * Returns difference between two vectors' coordinates in form of a vector.
	 * Takes a set of 2 coordinates (X1, Z1, X2, Z2) as a parameters.
	 */
	public static Vector2 VectorsDifference(float vecAX, float vecAY, float vecBX, float vecBY)
	{
		return new Vector2(Mathf.Abs(vecAX - vecBX), Mathf.Abs(vecAY - vecBY));
	}

	/**
	 * Returns difference between two vectors' coordinates in form of a vector.
	 * Takes a set of 2 vectors (vectorA, vectorB) as a parameters.
	 */
	public static Vector2 VectorsDifference(Vector2 vectorA, Vector2 vectorB)
	{
		return VectorsDifference(vectorA.x, vectorA.y, vectorB.x, vectorB.y);
	}

	//todo: refactor to hashmap or whatever
	// (now, cost of checking for existing number is O(n))
	//FIXME: if count == 0, sometimes app crashes.
	public static int[] GetUniqueRandomNumbers(int count, int rangeMinInclusive, int rangeMaxNonInclusive, bool sortAsc)
	{
		int[] uniqueNums = new int[count];
		int num;
		for (int c = 0; c < count; c++)
		{
			num = random.Next(rangeMinInclusive, rangeMaxNonInclusive);
			if (!Contains(uniqueNums, num))
			{
				uniqueNums[c] = num;
			}
			else
			{
				--c;
			}
		}

		if (sortAsc)
		{
			uniqueNums.OrderBy(x => x);
		}
		return uniqueNums;
	}

	/*
	 * Returning array of numbers ranging from 0 (inclusive) to (count) (inclusive).
	 */ 
	public static int[] CreateAscendingNumbersArray(int count)
	{
		int[] nums = new int[count];

		for (int c = 0; c < nums.Length; c++)
		{
			nums[c] = c;
		}
		return nums;
	}

	/*
	 * Returning list of numbers ranging from 0 (inclusive) to (count) (inclusive).
	 */ 
	public static LinkedList<int> CreateAscendingNumbers(int count)
	{
		LinkedList<int> numbers = new LinkedList<int>();

		for (int c = 0; c < count; c++)
		{
			numbers.AddLast(c);
		}

		return numbers;
	}

	/**
	 * Utility method for checking if (number) is inside the (container). Comparision by value.
	 */ 
	private static bool Contains(int[] container, int number)
	{
		for (int c = 0; c < container.Length; ++c)
		{
			if (container[c] == number)
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * Utility method for checking if (number) is inside the (container). Comparision by value.
	 */ 
	private static bool Contains(LinkedList<int> container, int number)
	{
		for (int c = 0; c < container.Count; ++c)
		{
			if (container.ElementAt(c) == number)
			{
				return true;
			}
		}
		return false;
	}
}
