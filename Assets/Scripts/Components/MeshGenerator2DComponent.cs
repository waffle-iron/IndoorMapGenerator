using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MeshGenerator2DComponent : MonoBehaviour {

	//refactor lists to arrays of fixed size
	//(because it's possible (?) to know vertex count beforehand).
	private List<Vector3> 	vertices;
	private List<int> 		triangles;
//	private Mesh 			mesh; //isnt it a duplicate?
	private GameObject 		targetMapObject;

	public Material 		debugMaterial;




	public void SetTarget(GameObject target) {
		targetMapObject = target;
	}

	public void GenerateMesh(MarchingSquare[,] marchingSquaresMap) {
		
		vertices = new List<Vector3> ();
		triangles = new List<int> ();
		Mesh mapMesh = new Mesh ();


		for (int x = 0; x < marchingSquaresMap.GetLength (0); ++x) {
			for (int z = 0; z < marchingSquaresMap.GetLength (1); ++z) {
				CreateTrianglesForSquare (marchingSquaresMap [x, z]);
			}
		}

		targetMapObject.GetComponent<MeshFilter> ().mesh = mapMesh;
//		GetComponent<MeshFilter> ().mesh = mesh;

		mapMesh.vertices = vertices.ToArray ();
		mapMesh.triangles = triangles.ToArray ();
//		mesh.RecalculateBounds ();  ?
		mapMesh.RecalculateNormals ();
	}


	private void CreateTrianglesForSquare(MarchingSquare square) {
		switch(square.GetSquareConfig()) {
			case 0:
				break;

			case 1: 
				MeshifySquare (square.midPointBottom, square.cornerBottomLeft, square.midPointLeft);
				break;
			case 2: 
				MeshifySquare (square.midPointRight, square.cornerBottomRight, square.midPointBottom);
				break;
			case 4: 
				MeshifySquare (square.midPointTop, square.cornerTopRight, square.midPointRight);
				break;
			case 8: 
				MeshifySquare (square.cornerTopLeft, square.midPointTop, square.midPointLeft);
				break;

			case 3:
				MeshifySquare (square.midPointRight, square.cornerBottomRight, square.cornerBottomLeft, square.midPointLeft);
				break;
			case 6:
				MeshifySquare (square.midPointTop, square.cornerTopRight, square.cornerBottomRight, square.midPointBottom);
				break;
			case 9:
				MeshifySquare (square.cornerTopLeft, square.midPointTop, square.midPointBottom, square.cornerBottomLeft);
				break;
			case 12:
				MeshifySquare (square.cornerTopLeft, square.cornerTopRight, square.midPointRight, square.midPointLeft);
				break;
			case 5:
				MeshifySquare (square.midPointTop, square.cornerTopRight, square.midPointRight, square.midPointBottom, square.cornerBottomLeft, square.midPointLeft);
				break;
			case 10:
				MeshifySquare (square.cornerTopLeft, square.midPointTop, square.midPointRight, square.cornerBottomRight, square.midPointBottom, square.midPointLeft);
				break;

			case 7:
				MeshifySquare (square.midPointTop, square.cornerTopRight, square.cornerBottomRight, square.cornerBottomLeft, square.midPointLeft);
				break;
			case 11:
				MeshifySquare (square.cornerTopLeft, square.midPointTop, square.midPointRight, square.cornerBottomRight, square.cornerBottomLeft);
				break;
			case 13:
				MeshifySquare (square.cornerTopLeft, square.cornerTopRight, square.midPointRight, square.midPointBottom, square.cornerBottomLeft);
				break;
			case 14:
				MeshifySquare (square.cornerTopLeft, square.cornerTopRight, square.cornerBottomRight, square.midPointBottom, square.midPointLeft);
				break;

			case 15:
				MeshifySquare (square.cornerTopLeft, square.cornerTopRight, square.cornerBottomRight, square.cornerBottomLeft);
				break;
			
			default:
				break;
		}
	}

	private void MeshifySquare(params MidPoint[] points) {
		CreateVertices (points);

		if (points.Length >= 3) {
			CreateTriangleFromPoints (points [0], points [1], points [2]);
		}
		if (points.Length >= 4) {
			CreateTriangleFromPoints (points [0], points [2], points [3]);
		}
		if (points.Length >= 5) {
			CreateTriangleFromPoints (points [0], points [3], points [4]);
		}
		if (points.Length >= 6) {
			CreateTriangleFromPoints (points [0], points [4], points [5]);
		}
	}
		

	private void CreateVertices(MidPoint[] points) {
		for (int p = 0; p < points.GetLength(0); ++p) {
			if (points[p].GetVertexIndex() == Utils.INTEGER_INVALID_VALUE) {
				points [p].SetVertexIndex (vertices.Count);
				vertices.Add (points [p].GetUnitCoordinates ());
			}
		}
	}

	private void CreateTriangleFromPoints(MidPoint a, MidPoint b, MidPoint c) {
		triangles.Add (a.GetVertexIndex ());
		triangles.Add (b.GetVertexIndex ());
		triangles.Add (c.GetVertexIndex ());
	}
		
}
