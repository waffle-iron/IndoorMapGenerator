using System;
using UnityEngine;

public class IndoorMapGeneratorScript : MonoBehaviour
{
	//inputs:
	public float 		metersInOneUnit = 1;
	public int 			gridSizeX = 10;
	public int 			gridSizeZ = 10;

	//prefabs:
	public GameObject 	floorPlane;
	private Vector3 	floorPlaneOriginalScale;
	public GameObject 	gridCellPlane;
	private Vector3 	gridCellPlaneOriginalScale;
	public GameObject 	gridCellVertex;
	private Vector3 	gridCellVertexOriginalScale;

	//instantiated GameObjects:
	private GameObject 	childHolderObject;
	private GameObject 	floorPlaneObject;

	private String className;
	private String methodName;


	public IndoorMapGeneratorScript()
	{
		Debug.Log("IndoorMapGenScript: constructor", this);
		Utils.Initialize(gridSizeX, gridSizeZ, metersInOneUnit);
		className = this.GetType().Name;
//		floorPlaneOriginalScale = Utils.VECTOR_INVALID_VALUE;
	}

	void Start()
	{
		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.Initialize(gridSizeX, gridSizeZ, metersInOneUnit);
	}

	public void Update()
	{
	}

	public void OnInputUpdate()
	{
		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		Utils.UpdateConstants(gridSizeX, gridSizeZ, metersInOneUnit);
	}

	public void CreateFloorPlane()
	{
		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);

		floorPlaneObject = (GameObject) Instantiate(floorPlane);
		floorPlaneObject.transform.localScale = Utils.GetGridRealSized3D(Utils.PLANE_SIZE_CORRECTION_MULTIPLIER);;
		floorPlaneObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		floorPlaneObject.transform.position = Utils.GetTopLeftCornerXZ(Vector3.zero, floorPlaneObject);
		floorPlaneObject.name = "Floor (" + gridSizeX + ", " + gridSizeZ + ")";
		floorPlaneObject.transform.parent = this.transform;
	}


	public void CreateCellularAutomataBoxes()
	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//		GameObject spawned;
//		Vector3 originalScale = gridCellPlane.transform.localScale;
//		gridCellPlane.transform.localScale = new Vector3(
//			originalScale.x/(float)gridSizeX,
//			originalScale.y,
//			originalScale.z/(float)gridSizeZ);
//
//		Vector3 spawnPosition = Vector3.zero;
//		for (int x= 0; x < gridSizeX; ++x)
//		{
//			for (int z = 0; z < gridSizeZ; ++z)
//			{
//				spawned = (GameObject) Instantiate(gridCellPlane, spawnPosition, Quaternion.identity);
//				spawned.name = "[" + x + ", " + z + "]";
//				spawned.transform.parent = childHolderObject.transform;
//				spawnPosition.z += 1 / (float)gridSizeZ;
//			}
//
//			spawnPosition.z = 0f;
//			spawnPosition.x += 1 / (float)gridSizeX;
//		}
	}

//	public void CreateCellularAutomataVertices()
//	{
//		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
//
//		GameObject spawned;
//		if (gridCellVertexOriginalScale == null)
//		{
//			gridCellVertexOriginalScale = gridCellVertex.transform.localScale;
//		}
//
//		gridCellVertex.transform.localScale = new Vector3(
//
//		);
////		gridCellPlane.transform.localScale = new Vector3(
////			originalScale.x/(float)gridSizeX,
////			originalScale.y,
////			originalScale.z/(float)gridSizeZ);
//
//		Vector3 spawnPosition = Vector3.zero;
//		for (int x= 0; x < gridSizeX; ++x)
//		{
//			for (int z = 0; z < gridSizeZ; ++z)
//			{
//				spawned = (GameObject) Instantiate(gridCellPlane, spawnPosition, Quaternion.identity);
//				spawned.name = "[" + x + ", " + z + "]";
//				spawned.transform.parent = childHolderObject.transform;
//				spawnPosition.z += 1 / (float)gridSizeZ;
//			}
//
//			spawnPosition.z = 0f;
//			spawnPosition.x += 1 / (float)gridSizeX;
//		}
//	}

	public void CreateGridVertices()
	{
		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
	}


	public void ClearObjects()
	{
		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		if (floorPlaneObject != null)
		{
			DestroyFloorPlane();
		}

		if (childHolderObject == null)
		{
			CreateHolder(); //should it be here?
		}
		else
		{
			DestroyChildren();
		}

	}

	private void CreateHolder()
	{
		Debug.Log(className + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name, this);
		childHolderObject = new GameObject("level");
		childHolderObject.transform.parent = this.transform;
	}


	private void DestroyChildren()
	{
		Utils.DestroyAssets(childHolderObject);
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
