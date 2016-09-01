using System.Collections;
using UnityEngine;

/**
 * 	Data structure representing a corner of a MarchingSquare.
 * 	Data about Corner's vertexIndex, position and traversability is key in
 * 	triangulation process of creating a mesh.
 * 
 * 	Holds reference to two MidPoint elements, the one directly above the corner and 
 * 	directly to the right of it. In consequence, if we are inspecting top left corner of a square, 
 * 	the upperMidPoint actually points to a midPoint in another (upper) square.
 */ 
public class Corner : MidPoint {

//	private int 	vertexIndex;
//	private Vector3 unitCoordinates;
	private bool 	traversable;

	private MidPoint upperMidPoint;
	private MidPoint rightMidPoint;


	/**
	 * Constructor for the specific Corner of marching square's Square.
	 * Two midpoints needed for mesh triangulation will be generated based on
	 * (squareScale) parameter.
	 */
	public Corner(Vector3 unitCoordinates, bool traversable, float squareScale) : base (unitCoordinates) {
//		this.unitCoordinates = unitCoordinates;
		this.traversable = traversable;
		upperMidPoint = new MidPoint (unitCoordinates + Vector3.forward * squareScale / 2f);
		rightMidPoint = new MidPoint (unitCoordinates + Vector3.right * squareScale / 2f);
	}

	/**
	 * Constructor for the specific Corner of marching square's Square.
	 * Two midpoints needed for mesh triangulation will be referenced based on 
	 * (upperMidPoint) and (rightMidPoint) parameters.
	 */
	public Corner(Vector3 unitCoordinates, bool traversable, MidPoint upperMidPoint, MidPoint rightMidPoint) : base(unitCoordinates) {
//		this.unitCoordinates = unitCoordinates;
		this.traversable = traversable;
		this.upperMidPoint = upperMidPoint;
		this.rightMidPoint = rightMidPoint;
	}
		

//	public int GetVertexIndex() {
//		return vertexIndex;
//	}
//
//	public Vector3 GetUnitCoordinates() {
//		return unitCoordinates;
//	}

	public bool GetTraversable() {
		return traversable;
	}

	public MidPoint GetUpperMidPoint() {
		return upperMidPoint;
	}

	public MidPoint GetRightMidPoint() {
		return rightMidPoint;
	}

}
