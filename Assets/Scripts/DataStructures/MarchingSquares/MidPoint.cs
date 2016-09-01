using System.Collections;
using UnityEngine;
using UnityEngineInternal;

/**
 * 	Data structure representing a midpoint of a side of a MarchingSquare.
 * 	Data about midPoint's vertexIndex and position is key in
 * 	triangulation process of creating a mesh.
 * 
 * 
 * 	Traversability information is filled dynamically by MarchingSquare and Corner
 * 	objects.
 */ 
public class MidPoint {

	protected int 	vertexIndex = Utils.INTEGER_INVALID_VALUE;
	protected Vector3 unitCoordinates;

	public MidPoint(Vector3 unitCoordinates) {
		this.unitCoordinates = unitCoordinates;
	}

	public void Initialize() {
		
	}

	public int GetVertexIndex() {
		return vertexIndex;
	}

	public int SetVertexIndex(int vertexIndexValue) {
		vertexIndex = vertexIndexValue;
		return vertexIndex;
	}

	public Vector3 GetUnitCoordinates() {
		return unitCoordinates;
	}

}
