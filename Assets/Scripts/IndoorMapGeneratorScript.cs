using UnityEngine;
using System.Collections;

public class IndoorMapGeneratorScript : MonoBehaviour
{

	public float 		oneUnitToMeters = 1;
	public int 			gridSizeX = 10;
	public int 			gridSizeZ = 10;
	public GameObject 	floorPlane;
	public GameObject 	gridCellPlane;
	public GameObject 	gridCellVertex;

	//instantiated GameObjects
	private GameObject 	childHolderObject;
	private GameObject 	floorPlaneObject;

	public IndoorMapGeneratorScript()
	{
		Debug.Log("IndoorMapGenScript constructor", this);
		Utils.Initialize(oneUnitToMeters);
	}

	void Start()
	{
		Debug.Log("IndoorMapGenScript Start()", this);
		Utils.Initialize(oneUnitToMeters);
	}

	public void Update()
	{

	}

	public void OnInputUpdate()
	{
		Utils.UpdateConstants(oneUnitToMeters);
	}

	public void CreateFloorPlane()
	{
		Vector2 gridDimensions = new Vector2(
			Utils.GetLength(gridSizeX),
			Utils.GetLength(gridSizeZ));

//		floorPlane.transform.localScale = gridDimensions;
//		floorPlane.transform.rotation = Quaternion

		floorPlaneObject = (GameObject) Instantiate(floorPlane);
		floorPlaneObject.transform.position = Utils.GetOriginCornerXz(Vector3.zero, floorPlaneObject);
		floorPlaneObject.transform.localScale = gridDimensions;
		floorPlaneObject.transform.rotation = Quaternion.Euler(270, 0, 0);

	}


	public void CreateCellularAutomataBoxes()
	{
		GameObject spawned;
		Vector3 originalScale = gridCellPlane.transform.localScale;
		gridCellPlane.transform.localScale = new Vector3(
			originalScale.x/(float)gridSizeX,
			originalScale.y,
			originalScale.z/(float)gridSizeZ);

		Vector3 spawnPosition = Vector3.zero;
		for (int x= 0; x < gridSizeX; ++x)
		{
			for (int z = 0; z < gridSizeZ; ++z)
			{
				spawned = (GameObject) Instantiate(gridCellPlane, spawnPosition, Quaternion.identity);
				spawned.name = "[" + x + ", " + z + "]";
				spawned.transform.parent = childHolderObject.transform;
				spawnPosition.z += 1 / (float)gridSizeZ;
			}
			spawnPosition.z = 0f;
			spawnPosition.x += 1 / (float)gridSizeX;
		}
	}

	public void CreateGridVertices()
	{

	}


	public void ClearObjects()
	{
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
