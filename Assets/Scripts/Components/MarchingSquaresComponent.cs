using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarchingSquaresComponent : MonoBehaviour {

	private MarchingSquare[,] marchingSquaresMap;

	public bool debugGizmos = false;
	private Vector3 debugCornerCubeScale;
	private Vector3 debugMidPointCubeScale;

//	void Start () {
//	
//	}

	public void GenerateMarchingSquaresMap(GridCellScript[,] cellMap, float squareScale, float height) {

		squareScale *= 10f;

		debugCornerCubeScale = Vector3.one;
		debugCornerCubeScale *= (float)(0.32f * squareScale);
		debugMidPointCubeScale = Vector3.one;
		debugMidPointCubeScale *= (float)(0.20f * squareScale);

//		debugCornerCubeScale = (float)(0.8f * squareScale);
//		debugMidPointCubeScale = (float)(0.5f * squareScale);

		int cellsUnitSizeX = cellMap.GetLength (0);
		int cellsUnitSizeZ = cellMap.GetLength (1);

		float cellsRealSizeX = cellsUnitSizeX * squareScale;
		float cellsRealSizeZ = cellsUnitSizeZ * squareScale;

		Corner[,] squareCorners = new Corner[cellsUnitSizeX, cellsUnitSizeZ];
		marchingSquaresMap = new MarchingSquare[cellsUnitSizeX - 1, cellsUnitSizeZ - 1];

		Vector3 coordinates;
		for (int x = 0; x < cellsUnitSizeX; ++x) {
			for (int z = 0; z < cellsUnitSizeZ; ++z) {
				//todo: refactor + create midpoints in some double for loop above
				coordinates = new Vector3 (
					x * squareScale,
					height,
					z * squareScale);
				if (cellMap [x, z].traversable) {
					squareCorners [x, z] = new Corner (coordinates, true, squareScale);
				} else {
					squareCorners [x, z] = new Corner (coordinates, false, squareScale);
				}
			}
		}

		for (int x = 0; x < cellsUnitSizeX-1; ++x) {
			for (int z = 0; z < cellsUnitSizeZ-1; ++z) {
				marchingSquaresMap[x,z] = new MarchingSquare(
					squareCorners[x, z+1], 
					squareCorners[x+1, z+1], 
					squareCorners[x+1, z], 
					squareCorners[x, z]
				);
			}
		}
			
	}

	public void GenerateMarchingSquaresMap(GridCellScript[,] cellMap, float squareScale) {
		GenerateMarchingSquaresMap (cellMap, squareScale, 0f);
	}
		
	public MarchingSquare[,] GetMarchingSquaresMap() {
		return marchingSquaresMap;
	}

	/**
	 * Debug drawing of marching squares (remember to turn on Gizmos' rendering in Unity Editor).
	 */ 
	void OnDrawGizmos() {
		if (marchingSquaresMap != null && debugGizmos == true) {
			
			for (int x = 0; x < marchingSquaresMap.GetLength(0); ++x) {
				for (int z = 0; z < marchingSquaresMap.GetLength (1); ++z) {	

					//drawing square cross lines:
//					Gizmos.color = Utils.white;
//					Gizmos.DrawLine (marchingSquaresMap [x, z].cornerBottomLeft.GetUnitCoordinates (), marchingSquaresMap [x, z].cornerTopRight.GetUnitCoordinates ());
//					Gizmos.DrawLine (marchingSquaresMap [x, z].cornerTopLeft.GetUnitCoordinates (), marchingSquaresMap [x, z].cornerBottomRight.GetUnitCoordinates ());
//
					//drawing square sides:
					Gizmos.color = Utils.greyDark;
					Gizmos.DrawLine (marchingSquaresMap [x, z].cornerTopLeft.GetUnitCoordinates (), marchingSquaresMap [x, z].cornerTopRight.GetUnitCoordinates ());
					Gizmos.DrawLine (marchingSquaresMap [x, z].cornerTopRight.GetUnitCoordinates (), marchingSquaresMap [x, z].cornerBottomRight.GetUnitCoordinates ());
					Gizmos.DrawLine (marchingSquaresMap [x, z].cornerBottomRight.GetUnitCoordinates (), marchingSquaresMap [x, z].cornerBottomLeft.GetUnitCoordinates ());
					Gizmos.DrawLine (marchingSquaresMap [x, z].cornerBottomLeft.GetUnitCoordinates (), marchingSquaresMap [x, z].cornerTopLeft.GetUnitCoordinates ());


					//Drawing midpoints:
					Gizmos.color = Utils.greyDark;
					Gizmos.DrawCube (marchingSquaresMap [x, z].midPointTop.GetUnitCoordinates (), debugMidPointCubeScale);
					Gizmos.DrawCube (marchingSquaresMap [x, z].midPointRight.GetUnitCoordinates (), debugMidPointCubeScale);
					Gizmos.DrawCube (marchingSquaresMap [x, z].midPointBottom.GetUnitCoordinates (), debugMidPointCubeScale);
					Gizmos.DrawCube (marchingSquaresMap [x, z].midPointLeft.GetUnitCoordinates (), debugMidPointCubeScale);

					//Drawing corners:
					Gizmos.color = (marchingSquaresMap [x, z].cornerTopLeft.GetTraversable ()) ? Color.white : Utils.white;
					Gizmos.DrawCube (marchingSquaresMap [x, z].cornerTopLeft.GetUnitCoordinates (), debugCornerCubeScale);

					Gizmos.color = (marchingSquaresMap [x, z].cornerTopRight.GetTraversable ()) ? Color.white : Utils.white;
					Gizmos.DrawCube (marchingSquaresMap [x, z].cornerTopRight.GetUnitCoordinates (), debugCornerCubeScale);

					Gizmos.color = (marchingSquaresMap [x, z].cornerBottomLeft.GetTraversable ()) ? Color.white : Utils.white;
					Gizmos.DrawCube (marchingSquaresMap [x, z].cornerBottomLeft.GetUnitCoordinates (), debugCornerCubeScale);

					Gizmos.color = (marchingSquaresMap [x, z].cornerBottomRight.GetTraversable ()) ? Color.white : Utils.white;
					Gizmos.DrawCube (marchingSquaresMap [x, z].cornerBottomRight.GetUnitCoordinates (), debugCornerCubeScale);

				}
			}

		}
	}


}
