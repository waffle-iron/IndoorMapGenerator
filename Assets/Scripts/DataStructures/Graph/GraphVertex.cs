using System;
using UnityEngine;


public class GraphVertex
{
	private bool 	keyPoi;
	private float 	positionX;
	private float 	positionY;
	private float 	positionZ;
	private Vector3 positionVector;
	private float	radius;

	public GraphVertex (Vector3 vertexPosition, float radius, bool isKeyPoi) {
		ConstructVertex (vertexPosition.x, vertexPosition.y, vertexPosition.z, radius, isKeyPoi); 
	}

//	public GraphVertex (float positionX, float positionY, float positionZ, float radius, bool isKeyPoi) {
//		ConstructVertex (positionX, positionY, positionZ, radius, isKeyPoi);
//	}
//
//	public GraphVertex (int positionX, int positionY, int positionZ, float radius, bool isKeyPoi) {
//		ConstructVertex (positionX, positionY, positionZ, radius, isKeyPoi);
//	}


	public GraphVertex ConstructVertex(float positionX, float positionY, float positionZ, float radius, bool isKeyPoi) {
		this.positionX = positionX;
		this.positionY = positionY;
		this.positionZ = positionZ;
		positionVector = new Vector3 (this.positionX, this.positionY, this.positionZ);
		this.radius = radius;
		this.keyPoi = isKeyPoi;

		return this;
	}


	public Vector3 GetPositionsVector() {
		return new Vector3 (positionX, positionY, positionZ);
	}

	public Vector3 GetPositionsVectorInt() {
		return new Vector3 ((int)positionX, (int)positionY, (int)positionZ);
	}

	public int GetPositionXInt() {
		return (int)positionX;
	}

	public int GetPositionYInt() {
		return (int)positionY;
	}

	public int GetPositionZInt() {
		return (int)positionZ;
	}
		
}


