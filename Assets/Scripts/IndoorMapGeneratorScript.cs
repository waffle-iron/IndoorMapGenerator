using System;
using System.CodeDom;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Xml.Schema;

public class IndoorMapGeneratorScript : MonoBehaviour
{
	//inputs:
				   public int 	gridSizeX = 10;
			       public int 	gridSizeZ = 10;
				   public float metersInOneUnit = 1;
				   public int 	regionDensity = 5;

	[Range(1, 25) ]public int 	poiPercentage = 10;
	[Range(1, 100)]public int 	keyPoiPerc = 50;

	[Range(1, 100)]public int 	keyPoiRndOffset = 10;
	[Range(1, 50) ]public int 	keyPoiSizePerc = 25;
	[Range(1, 200)]public int 	keyPoiSizeRndOffset = 10;

	[Range(1, 50) ]public int 	nonKeyPoiSizePerc = 1;
	[Range(1, 50) ]public int 	nonKeyPoiSizeRndOffset = 10;

	[Range(1, 50) ]public int 	noisePercentage = 10;
	[Range(1, 20) ]public int 	noiseBaseSizePerc = 5;
	[Range(1, 50) ]public int 	noiseSizeRandomOffset = 10;
				   public bool 	differentialNoise = true;

	[Range(1, 50) ]public int 	bleedScanRadius = 6;
	[Range(1, 100)]public int 	bleedThresholdPerc = 40;
	[Range(1, 5)  ]public int 	bleedIterations = 2;
	[Range(1, 100)]public int 	bleedChancePercent = 75;

	private GameObject 			objectHolder;
	private GameObject 			graphObjectHolder;

	//prefabs:
	public GameObject 		floorPlanePrefab;
	public GridRegionScript	gridRegionPrefab;
	public GridCellScript 	gridCellPrefab;

	//holders:
	private GameObject 		gridRegionsHolder;
	private GameObject 		gridCellsHolder;

	//instantiated GameObjects:
	private GameObject 		floorPlaneObject;

	private GridRegionScript[,] gridRegionsArray;
	private GridCellScript[,] 	gridCellsArray;

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
//		CalculateValues();
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
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.UpdateConstants(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
		CalculateValues();
	}

	public void CreateFloorPlane()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);


		objectHolder = new GameObject ();
		objectHolder.name = "IndoorMapGen map";

		floorPlaneObject = (GameObject) Instantiate(floorPlanePrefab);
		floorPlaneObject.transform.localScale = Utils.GetGridRealSize3D(Utils.PLANE_SIZE_CORRECTION_MULTIPLIER);
		floorPlaneObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		floorPlaneObject.transform.position = Utils.GetTopLeftCornerXZ(Vector3.zero, floorPlaneObject);
		floorPlaneObject.name = "Floor (" + gridSizeX + ", " + gridSizeZ + ")";
		floorPlaneObject.transform.parent = objectHolder.transform;
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
		gridRegionsHolder.transform.parent = objectHolder.transform;


		GridRegionScript 	spawned;
		Vector3 			spawnedScale;
		spawnedScale = Utils.GetGridRealSize3D(Utils.PLANE_SIZE_CORRECTION_MULTIPLIER);
		spawnedScale = Utils.divideXZ(spawnedScale, gridSizeX, gridSizeZ);
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int z = 0; z < gridSizeZ; z++)
			{
				spawned = Instantiate(gridRegionPrefab.gameObject).GetComponent<GridRegionScript>();
				spawned.SetRegionTraversable(false);
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

	public void CreateCells() 
	{
		//creating holder (for gameobjects)
		//creating array of references to cells
		gridCellsArray = new GridCellScript[gridSizeX * regionDensity, gridSizeZ * regionDensity];
		gridCellsHolder = new GameObject ();
		gridCellsHolder.name = "cells (" + gridCellsArray.GetLength(0) + "," + gridCellsArray.GetLength(1) + ")";
		gridCellsHolder.transform.parent = objectHolder.transform;
		gridCellsHolder.transform.position += new Vector3 (0f, 0.2f, 0f);

		//iterating over every region on the map
		GridCellScript 	spawned;
		Vector3 		spawnedPosition;
		bool 			regionTraversable;
		Vector3 		spawnedScale = gridRegionsArray[0,0].gameObject.transform.localScale;
		spawnedScale = Utils.divideXZ (spawnedScale, regionDensity);

		Vector3 		positionOffset = Utils.GetObjectOffsetToCenter (gridCellPrefab.gameObject);
		positionOffset = Utils.multiply (positionOffset, spawnedScale);



//		Debug.Log ("region (1,1): pos: " +  gridRegionsArray[1,1].gameObject.transform.position + ", utilspos: " + Utils.GetBottomLeftCornerXZ(gridRegionsArray[1,1].gameObject));
//		Debug.Log ("region (1,2): pos: " +  gridRegionsArray[1,2].gameObject.transform.position + ", utilspos: " + Utils.GetBottomLeftCornerXZ(gridRegionsArray[1,2].gameObject));
//		Debug.Log ("region (1,3): pos: " +  gridRegionsArray[1,3].gameObject.transform.position + ", utilspos: " + Utils.GetBottomLeftCornerXZ(gridRegionsArray[1,3].gameObject));
		Debug.Log ("regdens: " + regionDensity + " ,scalereg: " + gridRegionsArray[0,0].gameObject.transform.localScale + ", scalecell: " + "(" + spawnedScale.x + "," + spawnedScale.y + "," + spawnedScale.z + ").");

		for (int x = 0; x < gridSizeX; ++x) {
			for (int z = 0; z < gridSizeZ; ++z) {

				regionTraversable = gridRegionsArray [x, z].IsRegionTraversable ();

				//creating cells requires iterating (regionDens x regionDens) for every region
				//and creating cells accordingly
				for (int dx = 0; dx < regionDensity; ++dx) {
					for (int dz = 0; dz <regionDensity; ++dz) {

						spawned = Instantiate (gridCellPrefab.gameObject).GetComponent<GridCellScript> ();
						spawned.cellUnitCoordinates = new Vector2 (x * regionDensity + dx, z * regionDensity + dz);
						spawned.name = "cell (" + spawned.cellUnitCoordinates.x + "," + spawned.cellUnitCoordinates.y + ")";

						spawned.transform.parent = gridCellsHolder.transform;
						spawned.transform.localScale = spawnedScale;

						spawnedPosition = Utils.GetBottomLeftCornerXZ (gridRegionsArray [x, z].gameObject);
						spawnedPosition += positionOffset;
						spawnedPosition.x += (spawnedScale.x  / Utils.PLANE_SIZE_CORRECTION_MULTIPLIER) * dx;
						spawnedPosition.z += (spawnedScale.z  / Utils.PLANE_SIZE_CORRECTION_MULTIPLIER) * dz;
						spawned.transform.position = spawnedPosition;

						if (regionTraversable) {
							spawned.traversable = true;
						} else {
							spawned.traversable = false;
						}
						spawned.ColourTraversability ();

						gridCellsArray [(int)spawned.cellUnitCoordinates.x, (int)spawned.cellUnitCoordinates.y] = spawned;

					}
				}

			}
		}
	}

	public void CreatePointsOfInterest()
	{
//		Random random = new Random();

		//so this is copy-paste of CleanGridRegionsOff() method, but it will produce
		//InvalidArgumentException when is called from here by name.
		//it's like a Voldemort of this class
		for (int x = 0; x < gridRegionsArray.GetLength(0); ++x)
		{
			for (int z = 0; z < gridRegionsArray.GetLength(1); ++z)
			{
				gridRegionsArray[x,z].SetRegionTraversable(false);
			}
		}

		//calculate total number of POIs
		CalculateValues();
		Debug.LogError("COUNT: totalPois: " + totalPOIs + ", keyPois: " + totalPOIsKey + ", nonKeyPois: " + totalPOIsNonKey);
		Debug.Log("SIZES: keyPoi: " + keyPoiSizeVal + ", nonKeyPoi:" + nonKeyPoiSizeVal);


		int[] randomUniqueNums = UtilsMath.GetUniqueRandomNumbers(totalPOIs, 0, gridRegionsArray.GetLength(0) * gridRegionsArray.GetLength(1), true);
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
				UnityEngine.Random.Range(0.0f, 0.8f));

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

			foreach (Vector2 coord in coords)
			{
				gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionTraversable(true);
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
				UnityEngine.Random.Range(0.25f, 0.5f));

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

			foreach (Vector2 coord in coords)
			{
				gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionTraversable(true);
				gridRegionsArray[(int)coord.x, (int)coord.y].SetCustomColour(colour);
			}

		}
	}

	public void CreateEntryPoint()
	{
		Random random = new Random();
		pointEntry = new Vector2(
			Mathf.FloorToInt(gridSizeX * 0.3f),
			Mathf.FloorToInt(gridSizeZ * 0.3f)
		);

//		if (pointEntry != Utils.VECTOR2_INVALID_VALUE)
//		{
			
//		}

		pointEntry.x += random.Next(-gridSizeX / 4, gridSizeX / 4);
		pointEntry.y += random.Next(-gridSizeZ / 4, gridSizeZ / 4);
		pointEntry.x = Mathf.Min(gridSizeX - 1, pointEnd.x);
		pointEntry.y = Mathf.Min(gridSizeZ - 1, pointEnd.y);

		try
		{
			gridRegionsArray[(int)pointEntry.x, (int)pointEntry.y].SetRegionTraversable(false);
		}
		catch (IndexOutOfRangeException indexOutOfRangeException)
		{
			Debug.LogException(indexOutOfRangeException);
			Debug.LogError("out of range: " + "(" + pointEntry.x + ", " + pointEntry.y + ").");
			pointEntry.x = 2 + Math.Min (1, keyPoiSizeVal - 1);
			pointEntry.y = 3 + Math.Min (1, keyPoiSizeVal - 1);
			Debug.LogError ("retrying with values: " + Utils.VectorToString (pointEntry));

			gridRegionsArray[(int)pointEntry.x, (int)pointEntry.y].SetRegionTraversable(false);
		}


		//todo: think of algorithm for this
//		pointEntry.x += random.Next(-gridSizeX / 3, gridSizeX / 5);
//		pointEntry.y += random.Next(-gridSizeZ / 3, gridSizeZ / 5);
//		pointEntry.x = Mathf.Max(0, pointEntry.x);
//		pointEntry.y = Mathf.Max(0, pointEntry.y);

		List<Vector2> coords = UtilsMath.CreateMidPointCircle(
				(int)pointEntry.x,
				(int)pointEntry.y,
				Math.Min(1,keyPoiSizeVal-1),
				0,
				gridSizeX,
				gridSizeZ);

		foreach (Vector2 coord in coords)
		{
			gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionTraversable(true);
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
			gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionTraversable(true);
			gridRegionsArray[(int)coord.x, (int)coord.y].SetCustomColour(Color.black);
		}

//		nonKeyPoisList.AddFirst(gridRegionsArray[(int)pointEnd.x, (int)pointEnd.y]);
		Debug.LogError("entry: " + pointEntry);
		Debug.LogError("end: " + pointEnd);
	}

	public void CreatePathEntryEnd()
	{
		List<Vector2> path = UtilsMath.BresenhamAlgorithmInt(pointEntry, pointEnd);
		String str = "bresenham: ";

		foreach (Vector2 tile in path)
		{
			str += "[" + tile.x + "," + tile.y + "], ";
			gridRegionsArray[(int)tile.x, (int)tile.y].SetRegionTraversable(true);
		}
		Debug.Log(str);
	}
		
	public void ConnectKeyPois()
	{
		//temporary variables definition
		GridRegionScript actualNode;
		GridRegionScript newNode;
		List<GridRegionScript> candidateNodesList = new List<GridRegionScript>();
		List<float> nodesDistancesList = new List<float>();
		float minimumDistanceRangeLower;
		float minimumDistanceRangeUpper;
		bool firstIterPassed = false;

		Vector2 secondPoi = Utils.VECTOR2_INVALID_VALUE;


		//algorithm variables (input)
		float distanceMaxOffsetPercent = 20 / 100f; //0.20f;


		actualNode = keyPoisList.ElementAt(0);
		actualNode.ConnectedEdgesCountIncrement();

		for (int iteration = 0; iteration < 30; ++iteration)
		{

			//clearing and setting new temporary stuff on new loop iteration
			candidateNodesList = new List<GridRegionScript>(keyPoisList);
			nodesDistancesList = new List<float>();

			//removing actual node from calculations (of distance etc)
			bool isRemoved = candidateNodesList.Remove(actualNode);
//			Debug.LogError("removed: " + isRemoved.ToString());

			//creating list of distances from actual node to every other node
			foreach (var node in candidateNodesList)
			{
				nodesDistancesList.Add(
					UtilsMath.VectorLengthToFloat(
						UtilsMath.VectorsDifference(
							actualNode.GetRegionUnitCoords(),
							node.GetRegionUnitCoords())));
			}

			//calculating proper range of distances from actual node
			minimumDistanceRangeLower = Utils.Min(nodesDistancesList);
			minimumDistanceRangeUpper = minimumDistanceRangeLower * (1f + distanceMaxOffsetPercent);

			float distance;
			//removing nodes that are too far away from actual
			for (int d = 0; d < nodesDistancesList.Count; ++d)
			{
				distance = nodesDistancesList.ElementAt(d);
				if (!(distance >= minimumDistanceRangeLower && distance <= minimumDistanceRangeUpper))
				{
					candidateNodesList.RemoveAt(d);
					nodesDistancesList.RemoveAt(d);
				}
			}


			Debug.Log("[" + iteration + "] node " + actualNode.GetRegionUnitCoords() + " ::before: " + Utils.PrintList(candidateNodesList));

			//DEBUG ONLY?
			//removing nodes that were traversed before
//			foreach (var node in candidateNodesList)
			for (int n = 0; n < candidateNodesList.Count; ++n)
			{
				if (candidateNodesList.ElementAt(n).GetConnectedEdgesCount() > 0)
				{
//					candidateNodesList.Remove(node);
					candidateNodesList.RemoveAt(n);
					n = -1;
				}
			}

			Debug.Log("[" + iteration + "] node " + actualNode.GetRegionUnitCoords() + " ::after: " + Utils.PrintList(candidateNodesList));

//		Debug.LogError();

			if (candidateNodesList.Count == 0)
			{
				Debug.LogError("ERROR: no more candiate nodes available");
				CreateFinalConnections(secondPoi, actualNode.GetRegionUnitCoords());
//				List<Vector2> coordsFinal = UtilsMath.BresenhamAlgorithmInt(
////											actualNode.GetRegionUnitCoords(),
////											pointEnd);
	//			foreach (Vector2 coord in coordsFinal)
	//			{
	//				gridRegionsArray[(int)coord.x, (int)coord.y].SetRegionState(true);
	//				gridRegionsArray[(int)coord.x, (int)coord.y].SetCustomColour(Color.red);
	//			}
				return;
			}
			else
			{
//				Debug.LogError("CANDIDATES: count: " + candidateNodesList.Count);
				newNode = candidateNodesList.ElementAt(0);
			}

			Debug.Log("[" + iteration + "] node " + actualNode.GetRegionUnitCoords() + " <-----> " + "node " + newNode.GetRegionUnitCoords());

			if (newNode.GetConnectedEdgesCount() > 0)
			{
				Debug.LogError("ERROR: selected node has been traversed before");
				return;
			}

			//drawing line between nodes
			List<Vector2> coords = UtilsMath.BresenhamAlgorithmInt(
											actualNode.GetRegionUnitCoords(),
											newNode.GetRegionUnitCoords());

			foreach (Vector2 coord in coords)
			{
				gridRegionsArray[(int) coord.x, (int) coord.y].SetRegionTraversable(true);
				gridRegionsArray[(int) coord.x, (int) coord.y].SetCustomColour(new Color(0.95f, 0.25f, 0.20f, 1.00f));
			}
			firstIterPassed = true;

			actualNode = newNode;
			actualNode.ConnectedEdgesCountIncrement();
			if (secondPoi == Utils.VECTOR2_INVALID_VALUE)
			{
				secondPoi = actualNode.GetRegionUnitCoords();
			}
//			Debug.LogError("node " + actualNode.GetRegionUnitCoords() + " incremented, now: " + actualNode.ConnectedEdgesCountIncrement());

		}

	}
		
	private void CreateFinalConnections(Vector2 secondPoi, Vector2 finalPoi)
	{
		List<Vector2> coords;
		coords = UtilsMath.BresenhamAlgorithmInt(pointEntry, secondPoi);
		coords.AddRange(UtilsMath.BresenhamAlgorithmInt(finalPoi, pointEnd));

		foreach (Vector2 coord in coords)
		{
			gridRegionsArray[(int) coord.x, (int) coord.y].SetRegionTraversable(true);
			gridRegionsArray[(int) coord.x, (int) coord.y].SetCustomColour(Color.red);
		}
	}

	//TODO: 
	//	noise SIZE, 
	//	noise size PERCENTAGE OFFSET, 
	//	noise AMOUNT percentage OFFSET
	public void CreateRandomNoise() {
		int randomOffsetPercentage = 10;
		String debugString = "noise: ";

		int maxNoiseAmount = gridCellsArray.GetLength (0) * gridCellsArray.GetLength (1);
		int noiseAmount = maxNoiseAmount;
		debugString += "max: " + noiseAmount;

		noiseAmount = Mathf.CeilToInt(noiseAmount * (noisePercentage / 100f));

		debugString += "start: " + noiseAmount;

//		noiseAmount *= Utils.RandomRangeMiddleVal (
//			(int)(noiseAmount - (noiseAmount * (randomOffsetPercentage / 100f))),
//			(int)(noiseAmount + (noiseAmount * (randomOffsetPercentage / 100f)))
//		);

		debugString += "offset: " + noiseAmount;

		if (noiseAmount < 0) {
			noiseAmount = 0;
		}

		if (noiseAmount > maxNoiseAmount) {
			noiseAmount = maxNoiseAmount;
		}

		Debug.Log (debugString);
		GridCellScript cell;
		for (int n = 0; n < noiseAmount; ++n) {
			cell = gridCellsArray [
				(int)UnityEngine.Random.Range (0, gridCellsArray.GetLength(0)),
				(int)UnityEngine.Random.Range (0, gridCellsArray.GetLength(0))];


			//todo: make this statement prettier
			if (!differentialNoise) {
				cell.traversable = true;
			} else {
				if (cell.traversable) {
					cell.traversable = false;
				} else {
					cell.traversable = true;
				}
			}
			cell.ColourTraversability ();
		}


	}

	//TODO:
	//	bleed neighbour scan RADIUS
	//  bleed SIZE
	// 	bleed size PERCENTAGE OFFSET
	//	bleed noise threshold
	//	bleed noise ratio
	//	int bleedCreationIterations
	public void CreateBleedNoise() {
//		int neighbourScanRadius = 5;
//		int bleedThresholdPercent = 25;
		int bleedThreshold;
//		int bleedCreationIterations; //todo
		int bleedRatio;

		int bleedSize = 1; //todo for more
		int bleedSizePercentageOffset;

		List<Vector2> scanRadiusElements;
		List<Vector2> modifiedElements = new List<Vector2> ();
		Vector2 currentElement = new Vector2 ();
		int traversableElementsInRadius;
		int maxThreshold = UtilsMath.MidPointCircleMaxElements (bleedScanRadius);


		bleedThreshold = Mathf.CeilToInt(maxThreshold * (bleedThresholdPerc / 100f));
		modifiedElements.Clear ();

		for (int iterations = 0; iterations < bleedIterations; ++iterations) {

			for (int dx = 0; dx < gridCellsArray.GetLength (0); ++dx) {
				for (int dz = 0; dz < gridCellsArray.GetLength (1); ++dz) {
				
					currentElement.x = dx;
					currentElement.y = dz;
					traversableElementsInRadius = 0;

					scanRadiusElements = UtilsMath.CreateMidPointCircle (
						dx, 
						dz, 
						bleedScanRadius, 
						0, 
						gridCellsArray.GetLength (0), 
						gridCellsArray.GetLength (1));
				
//				scanRadiusElements.Remove (currentElement);

					for (int e = 0; e < scanRadiusElements.Count; ++e) {
						if (gridCellsArray [(int)scanRadiusElements [e].x, (int)scanRadiusElements [e].y].traversable) {
							++traversableElementsInRadius;
						}
					}

					if (traversableElementsInRadius >= bleedThreshold) {
//					if (!gridCellsArray[dx,dz].traversable) {
						modifiedElements.Add (currentElement);
//					}
					}

				}
			}

			//fix dat
			GridCellScript cell;
			for (int e = 0; e < modifiedElements.Count; ++e) {
				cell = gridCellsArray [(int)modifiedElements [e].x, (int)modifiedElements [e].y];
				if (!cell.traversable && UnityEngine.Random.Range(0, 100) < bleedChancePercent) {
					cell.traversable = true;
					cell.ColourTraversability ();
				}
			}

		}

	}

//	public void ConnectKeyPois()
//	{
//		GridRegionScript actualNode = keyPoisList.ElementAt(0);
//		GridRegionScript chosenNode;
//		float[] keyPoisDistances = new float[keyPoisList.Count];
//		float minimumDistanceRangeLower;
//		float minimumDistanceRangeUpper;
//
//
//		float distanceMaxOffsetPercent = 25 / 100f; //0.20f
//
//
//		for (int n = 0; n < keyPoisList.Count - 1; ++n)
//		{
////			actualNode = keyPoisList.ElementAt(n);
//
//			for (int i = 0; i < keyPoisList.Count; ++i)
//			{
//				keyPoisDistances[i] = UtilsMath.VectorLengthToFloat(
//					UtilsMath.VectorsDifference(
//						actualNode.GetRegionUnitCoords(),
//						keyPoisList.ElementAt(i).GetRegionUnitCoords()
//					));
//
//				//debug only, should be rewritten asap
//				if (keyPoisDistances[i] == 0)
//					keyPoisDistances[i] = 10000000;
//			}
//
//			minimumDistanceRangeLower = keyPoisDistances.Min();
//			minimumDistanceRangeUpper = minimumDistanceRangeLower * (1f + distanceMaxOffsetPercent);
//
//			List<int> qualifiedMinimumDistances = new List<int>();
//
//			for (int d = 0; d < keyPoisDistances.Length; ++d)
//			{
//				if (keyPoisDistances[d] >= minimumDistanceRangeLower
//				    && keyPoisDistances[d] <= minimumDistanceRangeUpper
//					&& keyPoisList.ElementAt(d).GetConnectedEdgesCount() == 0)
//				{
//					qualifiedMinimumDistances.Add(d);
//				}
//			}
//
//			if (qualifiedMinimumDistances.Count == 0)
//			{
//				Debug.LogError("ERROR: end of qualified distances");
//				return;
//			}
//			else
//			{
//				chosenNode = keyPoisList.ElementAt(qualifiedMinimumDistances.ElementAt(0));
//			}
////			chosenNode = keyPoisList.ElementAt(qualifiedMinimumDistances.ElementAt(UnityEngine.Random.Range(0, qualifiedMinimumDistances.Count)));
////			int count = 0;
////			while (chosenNode.GetConnectedEdgesCount() > 0)
////			{
////				chosenNode = keyPoisList.ElementAt(qualifiedMinimumDistances.ElementAt(UnityEngine.Random.Range(0, qualifiedMinimumDistances.Count)));
////				++count;
////				if (count > 3)
////					break;
////			}
//
//			chosenNode.ConnectedEdgesCountIncrement();
//			Debug.LogError("(" + n + "): node " + actualNode.GetRegionUnitCoords() + " <----> " + "node " + chosenNode.GetRegionUnitCoords());
//
//			//increment region's connectedTo val
//			//check if connection >0
//			//connect actual -> chosennode
//
//			actualNode = chosenNode;
//			if (actualNode.GetConnectedEdgesCount() > 1)
//			{
//				Debug.LogError("ERROR: ACTUAL NODE (chosen) WAS TRAVERSED BEFORE");
//				return;
//			}
//		}
//	}

	private void CleanGridRegionsOff()
	{
		for (int x = 0; x < gridRegionsArray.GetLength(0); ++x)
		{
			for (int z = 0; z < gridRegionsArray.GetLength(1); ++z)
			{
				gridRegionsArray[x,z].SetRegionTraversable(false);
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

	public int GetTotalPOIs(){ return totalPOIs; }
	public int GetTotalPOIsKey(){ return totalPOIsKey; }
	public int GetTotalPOIsNonKey(){ return totalPOIsNonKey; }
	public int GetKeyPoiSizeVal(){ return keyPoiSizeVal; }
	public int GetNonKeyPoiSizeVal(){ return nonKeyPoiSizeVal; }


}
