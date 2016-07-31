using UnityEngine;
using System.Collections;

public class GridRegionScript : MonoBehaviour
{

	private bool regionOn = false;
	private Vector2 regionUnitCoordinates = new Vector2(-1f, -1f);
	private int connectedEdgesCount = 0;

	public Material materialOn;
	public Material materialOff;

	//todo: 2 materials as drag-and-drops
	// OR have 2 colours (white-ish and gray) and set material colour accordingly to 'regionOn'


	void Start () {

	}

	void Update () {
	
	}

	private void UpdateStateColour()
	{
		//todo: make Utility method to choose between referencing
		//sharedMaterial vs material
//		Material mat = this.GetComponent<Renderer>().sharedMaterial;

		if (regionOn)
		{
			this.GetComponent<Renderer>().sharedMaterial = materialOn;
		}
		else
		{
			this.GetComponent<Renderer>().sharedMaterial = materialOff;
		}
	}

	public void SetCustomColour(Color colour)
	{
		SetCustomColour(colour, 50);
	}

	public void SetCustomColour(Color colour, int influencePercentage)
	{
		Material material = new Material(this.GetComponent<Renderer>().sharedMaterial);
		material.color = colour;
		this.GetComponent<Renderer>().sharedMaterial = material;
//			Color.Lerp(
//				this.GetComponent<Renderer>().sharedMaterial.color,
//				colour,
//				influencePercentage / 100
//		);
	}

	public void SetRegionState(bool regionOn)
	{
		this.regionOn = regionOn;
		UpdateStateColour();
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
		UpdateStateColour();
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

	public int GetConnectedEdgesCount()
	{
		return connectedEdgesCount;
	}

	public int ConnectedEdgesCountIncrement(int value)
	{
		connectedEdgesCount += value;
		return connectedEdgesCount;
	}

	public int ConnectedEdgesCountIncrement()
	{
		return ConnectedEdgesCountIncrement(1);
	}

}
