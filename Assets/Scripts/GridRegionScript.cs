using UnityEngine;
using System.Collections;

public class GridRegionScript : MonoBehaviour
{

	private bool regionOn = false;
	private Vector2 regionUnitCoordinates = new Vector2(-1f, -1f);

	public Material materialOn;
	public Material materialOff;

	//todo: 2 materials as drag-and-drops
	// OR have 2 colours (white-ish and gray) and set material colour accordingly to 'regionOn'


	void Start () {

	}

	void Update () {
	
	}

	private void updateStateColour()
	{
		//todo: make Utility method to choose between referencing
		//sharedMaterial vs material
		Material mat = this.GetComponent<Renderer>().sharedMaterial;

		if (regionOn)
		{
			mat = materialOn;
		}
		else
		{
			mat = materialOff;
		}
	}

	public void SetRegionState(bool regionOn)
	{
		this.regionOn = regionOn;
		updateStateColour();
	}

	public void SetRegionUnitCoords(Vector2 unitCoordinates)
	{
		SetRegionUnitCoords((int)unitCoordinates.x, (int)unitCoordinates.y);
	}

	public void SetRegionUnitCoords(int unitCoordinateX, int unitCoordinateZ)
	{
		regionUnitCoordinates.x = unitCoordinateX;
		regionUnitCoordinates.y = unitCoordinateZ;
	}

	public bool SwitchRegionState()
	{
		regionOn = !regionOn;
		updateStateColour();
		return regionOn;
	}

	public bool IsRegionOn()
	{
		return regionOn;
	}

	public Vector2 GetRegionUnitCoords()
	{
		return regionUnitCoordinates;
	}



}
