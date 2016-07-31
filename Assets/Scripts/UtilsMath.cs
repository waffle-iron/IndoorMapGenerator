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
	
	public static List<Vector2> CreateMidPointCircle(int midPointX, int midPointZ, int circleRadius)
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

	public static List<Vector2> CreateMidPointCircle(int midPointX, int midPointZ, int circleRadius, int min, int maxX, int maxZ)
	{
		List<Vector2> coords = CreateMidPointCircle(midPointX, midPointZ, circleRadius);
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

	public static double VectorLength(Vector2 vector)
	{
		return Math.Sqrt(Math.Pow(vector.x, 2) + Math.Pow(vector.y, 2));
	}

	public static float VectorLengthToFloat(Vector2 vector)
	{
		return (float)VectorLength(vector);
	}

	public static float VectorLengthToInt(Vector2 vector)
	{
		return Mathf.RoundToInt(VectorLengthToFloat(vector));
	}

//
//	public static List<Vector2> CreateMidPointCircle(Vector2 midPoint, int circleRadius)
//	{
//		return CreateMidPointCircle((int)midPoint.x, (int)midPoint.y, circleRadius);
//	}
//
//	public static List<Vector2> CreateMidPointCircle(Vector2 midPoint, int circleRadius, int min, int maxX, int maxZ)
//	{
//		return CreateMidPointCircle((int) midPoint.x, (int) midPoint.y, circleRadius, min, maxX, maxZ);
//	}

	public static Vector2 VectorsDifference(float vecAX, float vecAY, float vecBX, float vecBY)
	{
		return new Vector2(Mathf.Abs(vecAX - vecBX), Mathf.Abs(vecAY - vecBY));
	}

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

	public static int[] CreateAscendingNumbersArray(int count)
	{
		int[] nums = new int[count];

		for (int c = 0; c < nums.Length; c++)
		{
			nums[c] = c;
		}
		return nums;
	}

	public static LinkedList<int> CreateAscendingNumbers(int count)
	{
		LinkedList<int> numbers = new LinkedList<int>();

		for (int c = 0; c < count; c++)
		{
			numbers.AddLast(c);
		}

		return numbers;
	}

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
