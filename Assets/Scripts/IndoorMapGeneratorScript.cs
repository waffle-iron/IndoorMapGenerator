using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using Random = System.Random;

public class IndoorMapGeneratorScript : MonoBehaviour
{
	//inputs:
	public int 				gridSizeX = 10;
    public int 				gridSizeZ = 10;
	public float 			metersInOneUnit = 1;
	public int 				regionDensity = 5;
	public int 				pointsOfInterest = 10;


	//prefabs:
	public GameObject 		floorPlanePrefab;
	public GridRegionScript	gridRegionPrefab;
//	public GameObject 	GridCellPlanePrefab;
//	public GameObject 	GridCellVertexPrefab;
//	public  	gridRegionPrefab;

	//holders:
	private GameObject 		gridRegionsHolder;


	//instantiated GameObjects:
	private GameObject floorPlaneObject;


	private LinkedList<GridRegionScript> gridRegionsList = new LinkedList<GridRegionScript>();

	private String methodName;


	public IndoorMapGeneratorScript()
	{
		Debug.Log("IndoorMapGenScript: constructor", this);
		Utils.Initialize(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
	}

	void Start()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.Initialize(gridSizeX, gridSizeZ, regionDensity, metersInOneUnit);
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

	public void CreateRegions()
	{
		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, this);

		gridRegionsList.Clear();
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
				gridRegionsList.AddLast(spawned);
			}
		}
	}

	public void CreatePointsOfInterest(int amount)
	{
		Random random = new Random();
		int listLength = gridRegionsList.Count;
		if (listLength != (gridSizeX * gridSizeZ) - 1)
		{
			Debug.LogError("list length (" + listLength + "!= grid unit dims (" + ((gridSizeX * gridSizeZ) -1) + ")" );
		}
		for (int a = 0; a < amount; a++)
		{
			gridRegionsList.ElementAt(random.Next(0, listLength)).SetRegionState(true);
		}
	}



//	public void CreateCells()
//	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//
//		regionCellList.Clear();
//
//		Vector3 	regionCellLocalScale;
//		GameObject 	regionCell;
//		GameObject 	regionCellHolder;
//		regionCellHolder = new GameObject();
//		regionCellHolder.transform.parent = this.transform;
//
//		regionCellLocalScale = Utils.GetGridRealSize3D(Utils.PLANE_SIZE_CORRECTION_MULTIPLIER);
//		Debug.LogError("scale 1: " + regionCellLocalScale);
//		regionCellLocalScale = Utils.divideXZ(regionCellLocalScale, gridSizeX, gridSizeZ);
//		Debug.LogError("scale 2: " + regionCellLocalScale);
//		regionCellLocalScale = Utils.divideXZ(regionCellLocalScale, regionDensity);
//		Debug.LogError("scale 3: " + regionCellLocalScale);
//
//
//		for (int x = 0; x < (gridSizeX * regionDensity); x++)
//		{
//			for (int z = 0; z < (gridSizeZ * regionDensity); z++)
//			{
//				regionCell = Instantiate(GridCellPlanePrefab);
//
//				regionCell.transform.localScale = regionCellLocalScale;
//				regionCell.transform.position = Utils.GetTopLeftCornerXZ(Utils.GetLength(x), Utils.GetLength(z), regionCell);
//
//				regionCell.transform.parent = regionCellHolder.transform;
//				regionCell.name = "cell (" + x + ", " + z + ")";
//
//				regionCellList.AddLast(regionCell);
//			}
//		}
//
//	}


//	public void CreateGridRegions()
//	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//
//		gridRegionsList.Clear();
//
//		levelHolderObject = new GameObject();
//		levelHolderObject.transform.parent = this.transform;
//		levelHolderObject.name = "level";
//
//		GameObject region;
//
//
//		for (int x = 0; x < gridSizeX; x++)
//		{
//			for (int z = 0; z < gridSizeZ; z++)
//			{
//				region = new GameObject();
//				region.AddComponent<GridCell>().SetGridLocation(x, z);
//				region.transform.position = Utils.GetTopLeftCornerXZ(Utils.GetLength(x), Utils.GetLength(z), region);
//				region.transform.parent = levelHolderObject.transform;
//				region.transform.name = "Region [" + x + ", " + z + "]";
//				gridRegionsList.AddLast(region);
//			}
//		}
//	}

//	public void CreateRegionCells()
//	{
//		for (int x = 0; x < gridSizeX; x++) {
//			for (int z = 0; z < gridSizeZ; z++) {
//				for (int d = 0; d < regionDensity; d++) {
//
//				}
//			}
//		}
//	}

//	public void CreateGridRegionsPlaneCue()
//	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//
//		foreach (var region in gridRegionsList)
//		{
//
//		}
//
//	}


	/*
	public void CreateCellularAutomataBoxes()
	{
		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);

		GameObject 	spawned;
		Vector3 	spawnedScale;
		Vector3 	spawnedPos = Vector3.zero;
		spawnedScale = Utils.GetGridRealSize3D(Utils.PLANE_SIZE_CORRECTION_MULTIPLIER);
		spawnedScale = Utils.divideXZ(spawnedScale, gridSizeX, gridSizeZ);

		gridHolderObject = new GameObject();
		gridHolderObject.transform.parent = this.transform;
        gridHolderObject.name = "grid";

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int z = 0; z < gridSizeZ; z++)
			{
				spawned = Instantiate(GridCellPlanePrefab);
				spawned.transform.localScale = spawnedScale;
				spawned.transform.position = Utils.GetTopLeftCornerXZ(Utils.GetLength(x), Utils.GetLength(z), spawned);
				spawned.transform.parent = gridHolderObject.transform;
				spawned.name = "grid (" + x + ", " + z + ")";
			}
		}

	}
	*/

//	public void CreateCellularAutomataVertices()
//	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//
//		GameObject 	spawned;
//		Vector3 	spawnedScale;
//		Vector3 	spawnedPos = Vector3.zero;
//		spawnedScale = Utils.GetGridRealSize3D(Utils.VERTEX_CUE_SIZE_CORRECTION_MULTIPLIER);
//		spawnedScale = Utils.divideXZ(spawnedScale, gridSizeX, gridSizeZ);
//
//		GameObject 	gridHolder = new GameObject();
//		gridHolder.transform.parent = this.transform;
//        gridHolder.name = "vertexes";
//
//		for (int x = 0; x < gridSizeX; x++)
//		{
//			for (int z = 0; z < gridSizeZ; z++)
//			{
//				spawned = Instantiate(GridCellVertexPrefab);
//				spawned.transform.localScale = spawnedScale;
//
//				spawned.transform.position = Utils.GETTOPCOSTAMTESTETETERATESTETESTETEST(Utils.GetLength(x), Utils.GetLength(z), spawned);
//				spawned.transform.parent = gridHolder.transform;
//				spawned.name = "(" + x + ", " + z + ")";
//			}
//		}
//	}


//	public void ClearObjects()
//	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//		if (floorPlaneObject != null)
//		{
//			DestroyFloorPlane();
//		}
//
//		if (childHolderObject == null)
//		{
//			CreateHolder(); //should it be here?
//		}
//		else
//		{
//			DestroyChildren();
//		}
//
//	}

//	private void CreateHolder()
//	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//		childHolderObject = new GameObject("level");
//		childHolderObject.transform.parent = this.transform;
//	}


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
