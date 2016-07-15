using UnityEngine;
using System.Collections;

public class GridCell : MonoBehaviour
{

	[SerializeField]
	private int gridLocationX = -1;
	[SerializeField]
	private int gridLocationZ = -1;


	void Start () {
	
	}

	void Update () {
	
	}

	public void SetGridLocation(int gridLocationX, int gridLocationZ)
	{
		this.gridLocationX = gridLocationX;
		this.gridLocationZ = gridLocationZ;
	}

	public int GetGridLocationX()
	{
		return gridLocationX;
	}

	public int GetGridLocationZ()
	{
		return gridLocationZ;
	}

	public Vector2 GetGridLocations()
	{
		return new Vector2(gridLocationX, gridLocationZ);
	}
}
