using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

public class IndoorMapGeneratorScript : MonoBehaviour
{
	//inputs:
	public int 					gridSizeX = 10;
    public int 					gridSizeZ = 10;
	public float 				metersInOneUnit = 1;
	public int 					regionDensity = 5;
	[Range(1, 100)]public int 	poiPercentage = 10;
	[Range(1, 100)]public int 	keyPoiPerc = 50;
	[Range(1, 100)]public int 	keyPoiRndOffset = 10;
	[Range(1,100) ]public int 	keyPoiSizePerc = 25;
	[Range(1, 50) ]public int 	keyPoiSizeRndOffset = 10;
	[Range(1, 100)]public int 	nonKeyPoiSizePerc = 1;
	[Range(1, 50) ]public int 	nonKeyPoiSizeRndOffset = 10;


	//prefabs:
	public GameObject 		floorPlanePrefab;
	public GridRegionScript	gridRegionPrefab;

	//holders:
	private GameObject 		gridRegionsHolder;

	//instantiated GameObjects:
	private GameObject 		floorPlaneObject;

	private GridRegionScript[,] gridRegionsArray;
	private Vector2 			pointEntry = Utils.VECTOR2_INVALID_VALUE;
	private Vector2 			pointEnd = Utils.VECTOR2_INVALID_VALUE;
	private int 				totalPOIs;
	private int 				totalPOIsKey;
	private int 				totalPOIsNonKey;

	//todo: delete that? its not needed anyway
	private String 				methodName;


	public IndoorMapGeneratorScript()
	{
		Debug.Log("IndoorMapGenScript: constructor", this);
		Utils.Initialize(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
		UtilsMath.Initialize();
	}

	void Start()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.Initialize(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
		UtilsMath.Initialize();
	}

	public void Update()
	{

	}

	public void OnInputUpdate()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.UpdateConstants(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
	}

	public void CreateFloorPlane()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);

		floorPlaneObject = (GameObject) Instantiate(floorPlanePrefab);
		floorPlaneObject.transform.localScale = Utils.GetGridRealSize3D(Utils.PLANE_SIZE_CORRECTION_MULTIPLIER);
		floorPlaneObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		floorPlaneObject.transform.position = Utils.GetTopLeftCornerXZ(Vector3.zero, floorPlaneObject);
		floorPlaneObject.name = "Floor (" + gridSizeX + ", " + gridSizeZ + ")";
		floorPlaneObject.transform.parent = this.transform;
	}

	//todo: regions (this method and CreatePointsOfInterest()) are calibrated to be n x n.
	//investigate and fix dat
	public void CreateRegions()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);

		gridRegionsArray = new GridRegionScript[gridSizeX, gridSizeZ];
//		gridRegionsList.Clear();

		gridRegionsHolder = new GameObject();
		gridRegionsHolder.name = "Grid Regions";
		gridRegionsHolder.transform.parent = this.transform;


		GridRegionScript 	spawned;
		Vector3 spawnedScale;
		spawnedScale = Utils.GetGridRealSize3D(Utils.PLANE_SIZE_CORRECTION_MULTIPLIER);
		spawnedScale = Utils.divideXZ(spawnedScale, gridSizeX, gridSizeZ);
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int z = 0; z < gridSizeZ; z++)
			{
				spawned = Instantiate(gridRegionPrefab.gameObject).GetComponent<GridRegionScript>();
				spawned.SetRegionState(false);
				spawned.SetRegionUnitCoords(x, z);
				spawned.transform.localScale = spawnedScale;
				spawned.transform.position = Utils.GetTopLeftCornerXZ(Utils.GetLength(x), Utils.GetLength(z), spawned.gameObject);
				spawned.transform.name = "(" + x + ", " + z + ")";
				spawned.transform.parent = gridRegionsHolder.transform;

				gridRegionsArray[x, z] = spawned;
//				gridRegionsList.AddLast(spawned);
			}
		}
	}

	public void CreatePointsOfInterest()
	{
		Random random = new Random();

		CleanGridRegionsOff();

		//calculate total number of POIs
		totalPOIs = (int)(gridSizeX * gridSizeZ * poiPercentage / 100f);
		int[] randomUniqueNums = UtilsMath.GetUniqueRandomNumbers(pointsOfInterestCount, 0, gridRegionsArray.GetLength(0) * gridRegionsArray.GetLength(1), true);

		Debug.Log("nums: " + Utils.PrintList(randomUniqueNums.ToList()));

		//BUG: 0X0 IS ALWAYS (sometimes) ON!

		int row;
		int column;
		//create NON-KEY points of interest
		for (int l = 0; l < randomUniqueNums.Length; ++l)
		{
			row = Mathf.FloorToInt(randomUniqueNums[l] / (float)gridSizeX);
			column = randomUniqueNums[l] - (row * gridSizeX);
			gridRegionsArray[row, column].SetRegionState(true);
		}

		//create KEY points of interest
		for (int l = 0; l < randomUniqueNums.Length; ++l)
		{
			row = Mathf.FloorToInt(randomUniqueNums[l] / (float)gridSizeX);
			column = randomUniqueNums[l] - (row * gridSizeX);
			gridRegionsArray[row, column].SetRegionState(true);
			gridRegionsArray[row, column].SetCustomColour(Color.yellow);
		}
	}

	public void CreateEntryPoint()
	{
		Random random = new Random();
		if (pointEntry != Utils.VECTOR2_INVALID_VALUE)
		{
			gridRegionsArray[(int)pointEntry.x, (int)pointEntry.y].SetRegionState(false);
		}

		pointEntry = new Vector2(
			Mathf.FloorToInt(gridSizeX * 0.3f),
			Mathf.FloorToInt(gridSizeZ * 0.3f)
		);

		pointEntry.x += random.Next(-gridSizeX / 3, gridSizeX / 5);
		pointEntry.y += random.Next(-gridSizeZ / 3, gridSizeZ / 5);
		pointEntry.x = Mathf.Max(0, pointEntry.x);
		pointEntry.y = Mathf.Max(0, pointEntry.y);

		Debug.LogError("entrypoint: x:" + pointEntry.x + ", z:" + pointEntry.y);
		gridRegionsArray[(int)pointEntry.x, (int)pointEntry.y].SetRegionState(true);
		gridRegionsArray[(int)pointEntry.x, (int)pointEntry.y].SetCustomColour(Color.green, 75);
	}

	public void CreateEndPoint()
	{

		Random random = new Random();

//		if (pointEnd != Utils.VECTOR2_INVALID_VALUE)
//		{
//			gridRegionsArray[(int)pointEnd.x, (int)pointEnd.y].SetRegionState(false);
//		}

		pointEnd = new Vector2(
			Mathf.FloorToInt(gridSizeX * 0.9f),
			Mathf.FloorToInt(gridSizeZ * 0.9f)
		);
		try
		{
			pointEnd.x += random.Next(-gridSizeX / 4, gridSizeX / 4);
			pointEnd.y += random.Next(-gridSizeZ / 4, gridSizeZ / 4);
			pointEnd.x = Mathf.Min(gridSizeX - 1, pointEnd.x);
			pointEnd.y = Mathf.Min(gridSizeZ - 1, pointEnd.y);
		}
		catch (IndexOutOfRangeException indexOutOfRangeException)
		{
			Debug.LogException(indexOutOfRangeException);
			Debug.LogError("out of range: " + "(" + pointEnd.x + ", " + pointEnd.y + ").");
		}

		Debug.LogError("endpoint: x:" + pointEnd.x + ", z:" + pointEnd.y);
		gridRegionsArray[(int)pointEnd.x, (int)pointEnd.y].SetRegionState(true);
		gridRegionsArray[(int)pointEnd.x, (int)pointEnd.y].SetCustomColour(Color.red, 75);
	}

	public void CreatePathEntryEnd()
	{
		List<Vector2> path = UtilsMath.BresenhamAlgorithmInt(pointEntry, pointEnd);
		String str = "bresenham: ";

		foreach (Vector2 tile in path)
		{
			str += "[" + tile.x + "," + tile.y + "], ";
			gridRegionsArray[(int)tile.x, (int)tile.y].SetRegionState(true);
		}
		Debug.Log(str);

	}

	private void CleanGridRegionsOff()
	{
		for (int x = 0; x < gridRegionsArray.GetLength(0); ++x)
		{
			for (int z = 0; z < gridRegionsArray.GetLength(1); ++z)
			{
				gridRegionsArray[x,z].SetRegionState(false);
			}
		}
	}


	private void DestroyChildren()
	{
//		Utils.DestroyAssets(childHolderObject);
	}

	private void DestroyFloorPlane()
	{
		Utils.DestroyAsset(floorPlaneObject);
	}


	//input validation method
	void OnValidate()
	{
		if (gridSizeX < 10)
		{
			gridSizeX = 10;
		}

		if (gridSizeZ < 10)
		{
			gridSizeZ = 10;
		}
	}


}
