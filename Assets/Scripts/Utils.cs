using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Utils
{
	public static readonly Vector2 	VECTOR2_INVALID_VALUE = new Vector2(-1, -1);
	public static readonly Vector3 	VECTOR3_INVALID_VALUE = new Vector3(-1, -1, -1);
	public static readonly float 	PLANE_SIZE_CORRECTION_MULTIPLIER = 0.1f;
	public static readonly float 	VERTEX_CUE_SIZE_CORRECTION_MULTIPLIER = 0.3f;

	//VARIABLES:
	private static float 	metersInOneUnit;
	private static float 	regionDensity;
	private static bool 	initialized = false;

//	private static Vector2 	gridSizes;
//	private static Vector2 	gridRealSizes2D;
//	private static Vector3 	gridRealSizes3D;

	private static Vector2 	gridUnitSize2D;
	private static Vector2 	gridRealSize2D;
	private static Vector2 	regionRealSize2D;
	private static Vector2 	regionCellRealSize2D;
	private static int 		totalCells;
	private static int 		totalRegions;
	private static int 		minPOIRadius;
	public static int 		totalPOIs;
	public static int 		totalPOIsKey;
	public static int 		totalPOIsNonKey;
	public static int 		keyPoiSizeVal;
	public static int 		nonKeyPoiSizeVal;


	//CONSTRUCTION AND INITIALIZATION METHODS:

	public static void Initialize(int gridSizeXVal, int gridSizeZVal, int regionDensityVal, float metersInOneUnitVal)
	{
		initialized = true;
		UpdateConstants(gridSizeXVal, gridSizeZVal, regionDensityVal, metersInOneUnitVal);
	}

	public static void UpdateConstants(int gridSizeXVal, int gridSizeZVal, int regionDensityVal, float metersInOneUnitVal)
	{
		metersInOneUnit = metersInOneUnitVal;
		regionDensity = regionDensityVal;

		gridUnitSize2D = new Vector2(gridSizeXVal, gridSizeZVal);
		gridRealSize2D = multiply(gridUnitSize2D, metersInOneUnit);

		regionRealSize2D = new Vector2(
			gridRealSize2D.x / gridUnitSize2D.x,
			gridRealSize2D.y / gridUnitSize2D.y
		);

		regionCellRealSize2D = new Vector2(
			regionRealSize2D.x / regionDensity,
			regionRealSize2D.y / regionDensity
		);

		totalRegions = (int)(gridUnitSize2D.x * gridUnitSize2D.y);
        totalCells = totalRegions * (int)regionDensity;

		//todo: ?
//		minPOIRadius = (int)((1 / (float) gridUnitSize2D.x) * 100f);
		minPOIRadius = 1; //lol



	}



	//UTILITY METHODS:

	public static float Min(List<float> list)
	{
		return list.Select((t, i) => list.ElementAt(i)).Concat(new[] {float.MaxValue}).Min();
	}



	public static int RandomRangeMiddleVal(int min, int max)
	{
		if (min > max)
		{
			max += min;
			min = max - min;
			max -= min;
		}

		int delta = max - min;
		delta /= 2;
		return UnityEngine.Random.Range(min-delta, min+delta);
	}

	public static String PrintList(List<GridRegionScript> list)
	{
		StringBuilder builder = new StringBuilder();
		foreach (GridRegionScript element in list)
		{
			Vector2 coords = element.GetRegionUnitCoords();
			builder.Append("(" + coords.x + "," + coords.y + ")[" + element.GetConnectedEdgesCount() + "]");
			builder.Append(" ");
		}
		return builder.ToString();
	}

	public static String PrintList(LinkedList<int> list)
	{
		StringBuilder builder = new StringBuilder();
		foreach (int element in list)
		{
			builder.Append(element);
			builder.Append(" ");
		}
		return builder.ToString();
	}

	public static String PrintList(List<Vector2> list)
	{
		StringBuilder builder = new StringBuilder();
		foreach (Vector2 element in list)
		{
			builder.Append("(" + element.x + "," + element.y + ")");
			builder.Append(" ");
		}
		return builder.ToString();
	}

	public static String PrintList(List<int> list)
	{
		StringBuilder builder = new StringBuilder();
		foreach (int element in list)
		{
			builder.Append(element);
			builder.Append(" ");
		}
		return builder.ToString();
	}

	public static String PrintList(List<float> list)
	{
		StringBuilder builder = new StringBuilder();
		foreach (float element in list)
		{
			builder.Append(element);
			builder.Append(" ");
		}
		return builder.ToString();
	}

	public static String PrintList(int[] list)
	{
		StringBuilder builder = new StringBuilder();
		foreach (int element in list)
		{
			builder.Append(element);
			builder.Append(" ");
		}
		return builder.ToString();
	}

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

	public static Vector3 GetTopLeftCornerXZ(GameObject gameObject) {
		return GetTopLeftCornerXZ (0f, 0f, gameObject);
	}

	public static Vector3 GetTopLeftCornerXZ(Vector3 targetPos, GameObject gameObject)
	{
		Vector3 offset = GetObjectOffsetToCenter(gameObject);
		offset = multiply(offset, gameObject.transform.localScale);

		return new Vector3(
				targetPos.x + offset.x,
				targetPos.y,
				targetPos.z + offset.z);
	}

	public static Vector3 GetTopLeftCornerXZ(float targetPosX, float targetPosZ, GameObject gameObject)
	{
		Vector3 offset = GetObjectOffsetToCenter(gameObject);
		offset = multiply(offset, gameObject.transform.localScale);

		//TODO this (y=1f) is bad (sometimes you'd want to have y=0f), should be fixd, i guess
		return new Vector3(
			targetPosX + offset.x,
			1f,
			targetPosZ + offset.z);
	}

	public static Vector3 GetBottomLeftCornerXZ(GameObject gameObject) {
		Vector3 offset = GetObjectOffsetToCenter (gameObject);
		offset = multiply (offset, gameObject.transform.localScale);

		Vector3 point = gameObject.transform.position;
		point.x -= offset.x;
		point.z -= offset.z;

		return point;
	}

	public static Vector3 GETTOPCOSTAMTESTETETERATESTETESTETEST(float targetPosX, float targetPosZ, GameObject gameObject)
	{
		
//    		Vector3 offset = GetObjectOffsetToCenter(gameObject);
	    Vector3 offset = gameObject.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.extents;
		offset = multiply(offset, gameObject.transform.localScale);

		return new Vector3(
				targetPosX + offset.x,
				1f,
				targetPosZ + offset.z);
	}



	private static Vector3 GetObjectOffsetToCenter(GameObject gameObject)
	{
		Mesh gameObjectMesh;

		try
		{
			if (inEditor())
			{
//			return gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
				gameObjectMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			}
			else
			{
				gameObjectMesh = gameObject.GetComponent<MeshFilter>().mesh;
			}
		}
		catch (MissingComponentException noMeshException)
		{
			Debug.Log("no mesh attached to gameobject, setting offset as 0. ");
			return Vector3.zero;
		}

		return gameObjectMesh.bounds.extents;
//		return gameObject.GetComponent<MeshFilter>().mesh.bounds.extents;

	}

	public static Vector3 GetObjectDimensions(GameObject gameObject)
	{
		if (inEditor())
		{
			return gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.size;
		}
		return gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
	}

	public static bool DestroyAsset(GameObject asset)
	{
		if (inEditor())
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


	public static Vector2 GetGridRealSize2D()
	{
		return gridRealSize2D;
	}

	public static Vector3 GetGridRealSize3D()
	{
		return new Vector3(
				gridRealSize2D.x,
				1f,
				gridRealSize2D.y
		);
	}

	public static Vector3 GetGridRealSize3D(float multiplier)
	{
		Vector3 size = GetGridRealSize3D();
		size.x *= multiplier;
		size.z *= multiplier;
		return size;
	}

	public static Vector2 GetGridUnitSize2D()
	{
		return gridUnitSize2D;
	}

	public static Vector3 multiply(Vector3 vector, Vector3 multiplier)
	{
		vector.x *= multiplier.x;
		vector.y *= multiplier.y;
		vector.z *= multiplier.z;
		return vector;
	}

	public static Vector2 multiply(Vector2 vector, float multiplier)
	{
		vector.x *= multiplier;
		vector.y *= multiplier;
		return vector;
	}

	public static Vector3 multiplyXZ(Vector3 vector, float mulXZ)
	{
		vector.x *= mulXZ;
		vector.z *= mulXZ;
		return vector;
	}

	public static Vector3 divide(Vector3 vector, Vector3 divider)
	{
		vector.x /= divider.x;
		vector.y /= divider.y;
		vector.z /= divider.z;
		return vector;
	}

	public static Vector3 divideXZ(Vector3 vector, float divX, float divZ)
	{
		vector.x /= divX;
		vector.z /= divZ;
		return vector;
	}

	public static Vector3 divideXZ(Vector3 vector, float divXZ)
	{
		vector.x /= divXZ;
		vector.z /= divXZ;
		return vector;
	}

	public static bool inEditor()
	{
		return Application.isEditor;
	}

	public static int GetTotalRegions()
	{
		return totalRegions;
	}

	public static int GetTotalCells()
	{
		return totalCells;
	}

	public static int GetMinPOIRadius()
	{
		return minPOIRadius;
	}

}