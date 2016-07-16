using System.Collections;
using System.Collections.Generic;
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
	public static LinkedList<int> GetUniqueRandomNumbers(int count, int rangeMinInclusive, int rangeMaxNonInclusive, bool sortAsc)
	{
		LinkedList<int> uniqueNumbers = new LinkedList<int>();

		if (count > (rangeMaxNonInclusive - rangeMinInclusive))
		{
			return GetAscendingNumbers(rangeMaxNonInclusive - rangeMinInclusive);
		}

		int num;
		for (int c = 0; c < count; c++)
		{
			num = random.Next(rangeMinInclusive, rangeMaxNonInclusive);
			if (!Contains(uniqueNumbers, num))
			{
				uniqueNumbers.AddLast(num);
			}
			else
			{
				--c;
			}
		}


		if (sortAsc)
		{
			uniqueNumbers.OrderBy(x => x);
		}
		return uniqueNumbers;
	}

	public static LinkedList<int> GetAscendingNumbers(int count)
	{
		LinkedList<int> numbers = new LinkedList<int>();

		for (int c = 0; c < count; c++)
		{
			numbers.AddLast(c);
		}

		return numbers;
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
