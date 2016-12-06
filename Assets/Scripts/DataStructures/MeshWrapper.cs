using System;
using UnityEngine;


public class MeshWrapper {

	private Vector3[] 	meshVertices;
	private int[]	  	meshTriangles;
	private Vector2[] 	meshUvs;
	private Mesh 		mapMesh;

	private int triangleIndex;
	private int triangleCount;
	private int verticeCount;


	public MeshWrapper() {
	}

	//we are making a 2d mesh and then extrude some verts from it (so no need for specifying Y range/res/values)
	public MeshWrapper (int meshDimensionX, int meshDimensionZ) {
		InitializeMeshWrapper (meshDimensionX, meshDimensionZ);
	}

	public MeshWrapper InitializeMeshWrapper(int meshDimensionX, int meshDimensionZ) {
		meshVertices = new Vector3[(meshDimensionX * meshDimensionZ)];
		meshTriangles = new int[(meshDimensionX - 1) * (meshDimensionZ - 1)*6];
		meshUvs = new Vector2[meshDimensionX * meshDimensionZ];
		triangleIndex = 0;
		triangleCount = 0;
		verticeCount = 0;
		return this;
	}


	public void AddTriangle(int triangleCornerA, int triangleCornerB, int triangleCornerC) {
		meshTriangles [triangleIndex + 0] = triangleCornerA;
		meshTriangles [triangleIndex + 1] = triangleCornerB;
		meshTriangles [triangleIndex + 2] = triangleCornerC;

		triangleCount++;
		triangleIndex += 3;
	}

	public void AddVertex(int index, float valueX, float valueY, float valueZ) {
		meshVertices [index].x = valueX;
		meshVertices [index].y = valueY;
		meshVertices [index].z = valueZ;
	}

	public void AddVertex(int index, Vector3 values) {
		meshVertices [index] = values;
	}
		
	public void AddVertex(Vector3 values) {
		AddVertex (verticeCount, values);
		++verticeCount;
	}

	public void AddVertex(float valueX, float valueY, float valueZ) {
		AddVertex (verticeCount, valueX, valueY, valueZ);
		verticeCount++;
	}

	public void AddUV(int index, float valueX, float valueY) {
		meshUvs [index].x = valueX;
		meshUvs [index].y = valueY;
	}

	public void AddUV(float valueX, float valueY) {
		AddUV(verticeCount, valueX, valueY);
	}

	public Mesh GenerateMesh() {
		mapMesh = new Mesh ();

		mapMesh.vertices = meshVertices;
		mapMesh.triangles = meshTriangles;
		mapMesh.uv = meshUvs;
		mapMesh.RecalculateNormals ();
		mapMesh.RecalculateBounds ();
		mapMesh.Optimize ();

		return mapMesh;
	}

}
