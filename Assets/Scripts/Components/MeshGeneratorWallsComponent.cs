using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MeshGeneratorWallsComponent : MeshGeneratorAbstractComponent {

	//dependencies:
	private Mesh 			base2dMesh;

	public Material 		debugMaterial;

	private List<List<int>> outlineEdgesAndVertices = new List<List<int>> ();
	private HashSet<int> 	checkedVertices = new HashSet<int> ();
	private Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();


	public void SatisfyDependencies(Mesh base2dMesh) {
		if (base2dMesh == null) {
			throw new NullReferenceException ("Invalid parameter passed as dependency");
		}
		this.base2dMesh = base2dMesh;
	}


	public override void GenerateMesh() {
		triangleDictionary.Clear ();
		outlineEdgesAndVertices.Clear ();
		checkedVertices.Clear ();
		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		GenerateOutlines ();

		float 			wallHeight = 3;

		foreach(List<int> outline in outlineEdgesAndVertices) {
			for (int outlineVertex = 0; outlineVertex < outline.Count-1; ++outlineVertex) {

				//generating wall mesh - calculating starting point
				int startIndex = vertices.Count;

				//generating wall mesh - creating mesh vertices for single wall rectangle (made up from 2 triangles)
				vertices.Add (vertices [outline [outlineVertex]]); 	//left
				vertices.Add (vertices [outline [outlineVertex+1]]); //right
				vertices.Add (vertices [outline [outlineVertex]] - Vector3.up * wallHeight); //bottom left
				vertices.Add (vertices [outline [outlineVertex+1]] - Vector3.up * wallHeight); //bottom right

				//creating triangles for wall rectangle mesh: 1st triangle
				triangles.Add(startIndex + 0);
				triangles.Add(startIndex + 2);
				triangles.Add(startIndex + 3);

				//creating triangles for wall rectangle mesh: 2nd triangle
				triangles.Add(startIndex + 3);
				triangles.Add(startIndex + 1);
				triangles.Add(startIndex + 0);
			}
		}

		//TODO: 
		//	INSTEAD OF WALLTRIANGLES N WALLVERTICES ->
		//	JUST TRIANGLES AND VERTICES AND MESH!
		Mesh mesh = new Mesh();
		try {
			targetObject.GetComponent<MeshFilter> ().mesh = mesh;
			mesh.vertices = vertices.ToArray ();
			mesh.triangles = triangles.ToArray ();
		} catch (NullReferenceException nullRefExc) {
			Debug.LogError ("Null pointer in assigning mesh to target object - target doesn't have MeshFilter / MeshRenderer attached? " +
				"GenerateMesh() was not performed correctly?");
		}

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.Optimize ();


		base2dMesh = null;
	}

	protected override void AssignMeshToTarget() {
//		Mesh wallMesh = new Mesh ();
//		try {
//			targetObject.GetComponent<MeshFilter> ().mesh = wallMesh;
//			wallMesh.vertices = vertices.ToArray ();
//			wallMesh.triangles = triangles.ToArray ();
//		} catch (NullReferenceException nullRefExc) {
//			Debug.LogError ("Null pointer in assigning mesh to target object - target doesn't have MeshFilter / MeshRenderer attached? " +
//				"GenerateMesh() was not performed correctly?");
//		}
//
//		wallMesh.RecalculateNormals ();
//		wallMesh.RecalculateBounds ();
//		wallMesh.Optimize ();
	}

	private void DiscreticiseTriangles() {
		if (base2dMesh.triangles.Length % 3 == 0) {
			Debug.Log ("base2dMesh triangles are divideable by 3");
		} else {
			Debug.LogError ("base2dMesh triangles are NOT divideable by 3, error in provided mesh.");
		}

		int[] baseMeshTriangles = base2dMesh.triangles;
		Triangle triangle = new Triangle ();
		for (int t = 0; t < baseMeshTriangles.Length; t += 3) {
			triangle = new Triangle (
				base2dMesh.triangles[t],
				base2dMesh.triangles[t+1],
				base2dMesh.triangles[t+2]
			);

			SaveTriangle (triangle.triangleVertexA, triangle);
			SaveTriangle (triangle.triangleVertexB, triangle);
			SaveTriangle (triangle.triangleVertexC, triangle);
		}
	}

	private void SaveTriangle(int vertex, Triangle triangle) {
		if (triangleDictionary.ContainsKey(vertex)) {
			triangleDictionary [vertex].Add (triangle);
		} else {
			List<Triangle> list = new List<Triangle>();
			list.Add(triangle);
			triangleDictionary.Add(vertex, list);
		}
	}

	private void GenerateOutlines() {
		for (int vertex = 0; vertex < vertices.Count; ++vertex) {
			if (!checkedVertices.Contains(vertex)) {
				int nextOutlineVertex = NextOutlineVertex (vertex);
				if (nextOutlineVertex != Utils.INTEGER_INVALID_VALUE) {
					checkedVertices.Add (vertex);
					List<int> newOutline = new List<int> ();
					newOutline.Add (vertex);
					outlineEdgesAndVertices.Add (newOutline);
					FollowOutline (nextOutlineVertex, outlineEdgesAndVertices.Count - 1);
					outlineEdgesAndVertices [outlineEdgesAndVertices.Count - 1].Add (vertex);
				}
			}
		}
	}

	private void FollowOutline(int vertex, int outlineVertexIndex) {
		outlineEdgesAndVertices [outlineVertexIndex].Add (vertex);
		checkedVertices.Add (vertex);
		int nextVertex = NextOutlineVertex (vertex);

		if (nextVertex != Utils.INTEGER_INVALID_VALUE) {
			FollowOutline (nextVertex, outlineVertexIndex);
		}
	}

	private int NextOutlineVertex(int vertex) {
		List<Triangle> vertexAdjacentTriangles = triangleDictionary [vertex];

		Triangle triangle;
		for (int t = 0; t < vertexAdjacentTriangles.Count; ++t) {
			triangle = vertexAdjacentTriangles [t];

			for (int corner = 0; corner < 3; ++corner) {
				
				int triangleVertex = triangle[corner];
				if (triangleVertex != vertex && !checkedVertices.Contains(triangleVertex)) {
					if (CheckOutlineEdge(vertex, triangleVertex)) {
						return triangleVertex;
					}
				}

			}
		}

		return Utils.INTEGER_INVALID_VALUE;
	}

	private bool CheckOutlineEdge(int vertex, int vertexB) {
		List<Triangle> vertexAdjacentTriangles = triangleDictionary [vertex];
		int sharedTriangles = 0;

		for (int t = 0; t < vertexAdjacentTriangles.Count; ++t) {
			if (TriangleContainsVertex(vertexAdjacentTriangles[t], vertexB)) {
				++sharedTriangles;
				if (sharedTriangles > 1) {
					break;
				}
			}
		}
		
		if (sharedTriangles == 1) 
			return true;
		return false;
	}



	private bool TriangleContainsVertex(Triangle triangle, int vertex) {
		if (triangle.triangleVertexA == vertex ||
			triangle.triangleVertexB == vertex ||
			triangle.triangleVertexC == vertex) {
			return true;
		}
		return false;
	}

	public override void OptimiseMesh() {

	}

}
