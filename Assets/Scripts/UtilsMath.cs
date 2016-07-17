﻿using System.Collections;
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

	//todo: refactor to hashmap or whatever
	// (now, cost of checking for existing number is O(n))
	public static int[] GetUniqueRandomNumbers(int count, int rangeMinInclusive, int rangeMaxNonInclusive, bool sortAsc)
	{

		int[] uniqueNums = new int[count];
//		LinkedList<int> uniqueNumbers = new LinkedList<int>();

//		if (count > (rangeMaxNonInclusive - rangeMinInclusive))
//		{
//			return CreateAscendingNumbersArray(rangeMaxNonInclusive - rangeMinInclusive);
//		}

		int num;
		for (int c = 0; c < count; c++)
		{
			num = random.Next(rangeMinInclusive, rangeMaxNonInclusive);
			if (!Contains(uniqueNums, num))
			{
				uniqueNums[c] = num;
//				uniqueNumbers.AddLast(num);
			}
			else
			{
				--c;
			}
		}


		if (sortAsc)
		{
			uniqueNums.OrderBy(x => x);
//			uniqueNumbers.OrderBy(x => x);
		}
		return uniqueNums;
	}

	public static int[] CreateAscendingNumbersArray(int count)
	{
		int[] nums = new int[count];
//		LinkedList<int> numbers = new LinkedList<int>();

		for (int c = 0; c < nums.Length; c++)
		{
			nums[c] = c;
//			numbers.AddLast(c);
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
