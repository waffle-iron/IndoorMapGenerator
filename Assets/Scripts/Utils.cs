using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;


public class Utils
{

	//CONSTANTS:

	private static bool initialized = false;
	private static float oneUnitToMeters;


	//CONSTRUCTION AND INITIALIZATION METHODS:

	public static void Initialize(float oneUnitToMetersVal)
	{
		initialized = true;
		oneUnitToMeters = oneUnitToMetersVal;
	}

	public static void UpdateConstants(float oneUnitToMetersVal)
	{
		oneUnitToMeters = oneUnitToMetersVal;
	}


	//UTILITY METHODS:

	public static float GetLength(float valueWithoutMeasurementUnit)
	{
		try
		{
			return valueWithoutMeasurementUnit * oneUnitToMeters;
		}
		catch (Exception exception)
		{
			if (exception is ArgumentNullException)
			{
				Debug.Log("Utils.GetLength(): argument of method was null.");
			} else if (exception is NullReferenceException)
			{
				if (initialized == false)
				{
					Debug.Log("Utils.GetLength(): Utils class is not initialized. Dumbass.");
				}
				else
				{
					Debug.Log("Utils.GetLength(): null in expression (used constant or argument is null).");
				}
			}
		}

		return -1f;
	}

	public static Vector3 GetOriginCornerXz(Vector3 targetPos, GameObject gameObject)
	{
		return new Vector3(
			targetPos.x - gameObject.transform.localScale.x/2f,
			targetPos.y,
			targetPos.z - gameObject.transform.localScale.z/2f);
	}

	public static bool DestroyAsset(GameObject asset)
	{
		if (Application.isEditor)
		{
			GameObject.DestroyImmediate(asset);
			return true;
		}

		if (Application.isPlaying)
		{
			GameObject.Destroy(asset);
			return true;
		}

		return false;
	}

	public static bool DestroyAssets(GameObject parent)
	{
		bool returnValue = true;
		foreach (Transform child in parent.transform)
		{
			if (DestroyAsset(child.gameObject) == false)
			{
				returnValue = false;
			}
		}

		return returnValue;
	}


}