using System;
using UnityEngine;


public class Utils
{

	public static readonly Vector3 	VECTOR_INVALID_VALUE = new Vector3(-1, -1, -1);
	public static readonly float 	PLANE_SIZE_CORRECTION_MULTIPLIER = 0.1f;

	//VARIABLES:
	private static float 	metersInOneUnit;
	private static bool 	initialized = false;

	private static Vector2 	gridSizes;
	private static Vector2 	gridRealSizes2D;
	private static Vector3 	gridRealSizes3D;


	//CONSTRUCTION AND INITIALIZATION METHODS:

	public static void Initialize(int gridSizeXVal, int gridSizeZVal, float metersInOneUnitVal)
	{
		initialized = true;
		metersInOneUnit = metersInOneUnitVal;

		gridSizes = new Vector2(gridSizeXVal, gridSizeZVal);
		gridRealSizes2D = new Vector2(GetLength(gridSizeXVal), GetLength(gridSizeZVal));
		gridRealSizes3D = new Vector3(gridRealSizes2D.x, 1f, gridRealSizes2D.y);

	}

	public static void UpdateConstants(int gridSizeXVal, int gridSizeZVal, float metersInOneUnitVal)
	{
		metersInOneUnit = metersInOneUnitVal;
		gridSizes = new Vector2(gridSizeXVal, gridSizeZVal);
        gridRealSizes2D = new Vector2(GetLength(gridSizeXVal), GetLength(gridSizeZVal));
		gridRealSizes3D = new Vector3(gridRealSizes2D.x, 1f, gridRealSizes2D.y);
	}



	//UTILITY METHODS:

	public static float GetLength(float valueWithoutMeasurementUnit)
	{
		try
		{
			return valueWithoutMeasurementUnit * metersInOneUnit;
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
//		return new Vector3(
//			targetPos.x - gameObject.transform.localScale.x/2f,
//			targetPos.y,
//			targetPos.z - gameObject.transform.localScale.z/2f);
		return new Vector3

	}

	public static Vector3 GetObjectDimensions(GameObject gameObject)
	{
		gameObject.GetComponent<MeshFilter>().mesh.bounds.

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


	public static Vector2 getGridRealSizes2D()
	{
		return gridRealSizes2D;
	}

	public static Vector3 GetGridRealSized3D()
	{
		return gridRealSizes3D;
	}

	public static Vector3 GetGridRealSized3D(float multiplier)
	{
		Vector3 size = gridRealSizes3D;
		size.x *= multiplier;
		size.z *= multiplier;
		return size;
	}

}