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

	[Range(1, 25)]public int 	poiPercentage = 10;
	[Range(1, 100)]public int 	keyPoiPerc = 50;

	[Range(1, 100)]public int 	keyPoiRndOffset = 10;
	[Range(1, 50) ]public int 	keyPoiSizePerc = 25;
	[Range(1, 200)]public int 	keyPoiSizeRndOffset = 10;

	[Range(1, 50) ]public int 	nonKeyPoiSizePerc = 1;
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

	//todo: make this shit private
	private int 				totalPOIs;
	private int 				totalPOIsKey;
	private int 				totalPOIsNonKey;
	private int 				keyPoiSizeVal;
	private int 				nonKeyPoiSizeVal;

	private LinkedList<GridRegionScript> keyPoisList = new LinkedList<GridRegionScript>();
	private LinkedList<GridRegionScript> nonKeyPoisList = new LinkedList<GridRegionScript>();

	//todo: delete that? its not needed anyway
	private String 				methodName;



	//todo: make controller - model infrastructure




	public IndoorMapGeneratorScript()
	{
		Debug.Log("IndoorMapGenScript: constructor", this);
		Utils.Initialize(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
		UtilsMath.Initialize();
		CalculateValues();
	}

	void Start()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.Initialize(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
		UtilsMath.Initialize();
		CalculateValues();
	}

	private void CalculateValues()
	{
		totalPOIs = Mathf.FloorToInt(gridSizeX * gridSizeZ * (poiPercentage / 100f));

		totalPOIsKey = Mathf.CeilToInt(totalPOIs * (keyPoiPerc / 100f));
		totalPOIsKey += Utils.RandomRangeMiddleVal(0, Mathf.FloorToInt(totalPOIsKey * (keyPoiRndOffset/100f)));

		totalPOIsNonKey = totalPOIs - totalPOIsKey;


		keyPoiSizeVal = Mathf.Clamp(
				Mathf.CeilToInt(Mathf.Min(gridSizeX, gridSizeZ) * (keyPoiSizePerc / 100f)),
				Utils.GetMinPOIRadius(),
				Mathf.Min(gridSizeX, gridSizeZ));

		nonKeyPoiSizeVal = Mathf.Clamp(
				Mathf.FloorToInt(Mathf.Min(gridSizeX, gridSizeZ) * (nonKeyPoiSizePerc / 100f)),
				Utils.GetMinPOIRadius(),
				Mathf.Min(gridSizeX, gridSizeZ));
	}

	public void Update()
	{

	}

	public void OnInputUpdate()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.UpdateConstants(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
		CalculateValues();
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

		//so this is copy-paste of CleanGridRegionsOff() method, but it will produce
		//InvalidArgumentException when is called from here by name.
		//it's like a Voldemort of this class
		for (int x = 0; x < gridRegionsArray.GetLength(0); ++x)
		{
			for (int z = 0; z < gridRegionsArray.GetLength(1); ++z)
			{
				gridRegionsArray[x,z].SetRegionState(false);
			}
		}

		//calculate total number of POIs
		CalculateValues();


		Debug.Log("POISizeKEY: " + keyPoiSizeVal + ", POISizeNONKEY:" + nonKeyPoiSizeVal);

		int[] randomUniqueNums = UtilsMath.GetUniqueRandomNumbers(totalPOIs, 0, gridRegionsArray.GetLength(0) * gridRegionsArray.GetLength(1), true);

		Debug.Log("nums: " + Utils.PrintList(randomUniqueNums.ToList()));

		//BUG: 0X0 IS ALWAYS (sometimes) ON!
		int row;
		int column;
		Color colour;

		//create KEY points of interest
		for (int l = 0; l < totalPOIsKey; ++l)
		{
			row = Mathf.FloorToInt(randomUniqueNums[l] / (float)gridSizeZ);
			column = randomUniqueNums[l] - (row * gridSizeX);
			colour = new Color(
				UnityEngine.Random.Range(0.0f, 0.8f),
				1f,
				UnityEngine.Random.Range(0.0f, 0.8f)
			);

			keyPoisList.AddLast(gridRegionsArray[row, column]);

			int radius = keyPoiSizeVal;
			radius += Utils.RandomRangeMiddleVal(0, Mathf.FloorToInt(radius * keyPoiSizeRndOffset/100f));

			List<Vector2> coords = UtilsMath.CreateMidPointCircle(
				row,
				column,
				radius,
				0,
				gridSizeX,
				gridSizeZ);

			Debug.Log("KEY (" + l + "), R: " + radius);

			foreach (Vector2 coord in coords)
			{
				gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionState(true);
				gridRegionsArray[(int)coord.x, (int)coord.y].SetCustomColour(colour);
			}
		}

		//NON-KEY points of interest
		for (int l = totalPOIsKey; l < totalPOIs; ++l)
		{
			row = Mathf.FloorToInt(randomUniqueNums[l] / (float)gridSizeX);
			column = randomUniqueNums[l] - (row * gridSizeX);
			colour = new Color(
				1f,
				UnityEngine.Random.Range(0f, 0.3f),
				UnityEngine.Random.Range(0.25f, 0.5f)
			);

			nonKeyPoisList.AddLast(gridRegionsArray[row, column]);

			int radius = nonKeyPoiSizeVal;
			radius += Utils.RandomRangeMiddleVal(0, Mathf.FloorToInt(radius * nonKeyPoiSizeRndOffset/100f));

			List<Vector2> coords = UtilsMath.CreateMidPointCircle(
				row,
				column,
				radius,
				0,
				gridSizeX,
				gridSizeZ);

			Debug.Log("nonkey (" + l + "), R: " + radius);

			foreach (Vector2 coord in coords)
			{
				gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionState(true);
				gridRegionsArray[(int)coord.x, (int)coord.y].SetCustomColour(colour);
			}

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

		//todo: think of algorithm for this
		pointEntry.x += random.Next(-gridSizeX / 3, gridSizeX / 5);
		pointEntry.y += random.Next(-gridSizeZ / 3, gridSizeZ / 5);
		pointEntry.x = Mathf.Max(0, pointEntry.x);
		pointEntry.y = Mathf.Max(0, pointEntry.y);

		List<Vector2> coords = UtilsMath.CreateMidPointCircle(
				(int)pointEntry.x,
				(int)pointEntry.y,
				Math.Min(1,keyPoiSizeVal-1),
				0,
				gridSizeX,
				gridSizeZ);

		foreach (Vector2 coord in coords)
		{
			gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionState(true);
			gridRegionsArray[(int)coord.x, (int)coord.y].SetCustomColour(Color.black);
		}

		keyPoisList.AddFirst(gridRegionsArray[(int)pointEntry.x, (int)pointEntry.y]);
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

		List<Vector2> coords = UtilsMath.CreateMidPointCircle(
				(int)pointEnd.x,
				(int)pointEnd.y,
				keyPoiSizeVal+1,
				0,
				gridSizeX,
				gridSizeZ);

		foreach (Vector2 coord in coords)
		{
			gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionState(true);
			gridRegionsArray[(int)coord.x, (int)coord.y].SetCustomColour(Color.black);
		}

//		nonKeyPoisList.AddFirst(gridRegionsArray[(int)pointEnd.x, (int)pointEnd.y]);
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

	public void ConnectKeyPois()
	{
		GridRegionScript actualNode = keyPoisList.ElementAt(0);
		GridRegionScript chosenNode;
		float[] keyPoisDistances = new float[keyPoisList.Count];
		float minimumDistanceRangeLower;
		float minimumDistanceRangeUpper;


		float distanceMaxOffsetPercent = 25 / 100f; //0.20f


		for (int n = 0; n < keyPoisList.Count - 1; ++n)
		{
//			actualNode = keyPoisList.ElementAt(n);

			for (int i = 0; i < keyPoisList.Count; ++i)
			{
				keyPoisDistances[i] = UtilsMath.VectorLengthToFloat(
					UtilsMath.VectorsDifference(
						actualNode.GetRegionUnitCoords(),
						keyPoisList.ElementAt(i).GetRegionUnitCoords()
					));

				//debug only, should be rewritten asap
				if (keyPoisDistances[i] == 0)
					keyPoisDistances[i] = 10000000;
			}

			minimumDistanceRangeLower = keyPoisDistances.Min();
			minimumDistanceRangeUpper = minimumDistanceRangeLower * (1f + distanceMaxOffsetPercent);

			List<int> qualifiedMinimumDistances = new List<int>();

			for (int d = 0; d < keyPoisDistances.Length; ++d)
			{
				if (keyPoisDistances[d] >= minimumDistanceRangeLower
				    && keyPoisDistances[d] <= minimumDistanceRangeUpper)
				{
					qualifiedMinimumDistances.Add(d);
				}
			}


			chosenNode = keyPoisList.ElementAt(qualifiedMinimumDistances.ElementAt(UnityEngine.Random.Range(0, qualifiedMinimumDistances.Count)));
			int count = 0;
			while (chosenNode.GetConnectedEdgesCount() > 0)
			{
				chosenNode = keyPoisList.ElementAt(qualifiedMinimumDistances.ElementAt(UnityEngine.Random.Range(0, qualifiedMinimumDistances.Count)));
				++count;
				if (count > 3)
					break;
			}
			chosenNode.ConnectedEdgesCountIncrement();
			Debug.LogError("(" + n + "): node " + actualNode.GetRegionUnitCoords() + " <----> " + "node " + chosenNode.GetRegionUnitCoords());

			//increment region's connectedTo val
			//check if connection >0
			//connect actual -> chosennode

			actualNode = chosenNode;
			if (actualNode.GetConnectedEdgesCount() > 1)
			{
				Debug.LogError("ERROR: ACTUAL NODE (chosen) WAS TRAVERSED BEFORE");
				return;
			}
		}


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

	public int GetTotalPOIs(){ return totalPOIs; }
	public int GetTotalPOIsKey(){ return totalPOIsKey; }
	public int GetTotalPOIsNonKey(){ return totalPOIsNonKey; }
	public int GetKeyPoiSizeVal(){ return keyPoiSizeVal; }
	public int GetNonKeyPoiSizeVal(){ return nonKeyPoiSizeVal; }


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
