using System.Collections;
using UnityEngine;
using System;

/**
 * 
 */ 
public class MarchingSquare {


	private int 	squareConfig;
	public Corner 	cornerTopLeft, cornerTopRight, cornerBottomRight, cornerBottomLeft;
	public MidPoint midPointTop, midPointRight, midPointBottom, midPointLeft; 


	public MarchingSquare (Corner cornerTopLeft, Corner cornerTopRight, Corner cornerBottomRight, Corner cornerBottomLeft) {
		this.cornerTopLeft = cornerTopLeft;
		this.cornerTopRight = cornerTopRight;
		this.cornerBottomRight = cornerBottomRight;
		this.cornerBottomLeft = cornerBottomLeft;

		midPointTop = cornerTopLeft.GetRightMidPoint ();
		midPointRight = cornerBottomRight.GetUpperMidPoint ();
		midPointBottom = cornerBottomLeft.GetRightMidPoint ();
		midPointLeft = cornerBottomLeft.GetUpperMidPoint ();

		CalculateSquareConfig ();
	}

	public int CalculateSquareConfig() {
		squareConfig = 0;

		if (cornerTopLeft.GetTraversable()) {
			squareConfig += (int) Mathf.Pow (2, 3);
		}

		if (cornerTopRight.GetTraversable()) {
			squareConfig += (int) Mathf.Pow (2, 2);
		}

		if (cornerBottomRight.GetTraversable()) {
			squareConfig += (int) Mathf.Pow (2, 1);
		}

		if (cornerBottomLeft.GetTraversable()) {
			squareConfig += (int) Mathf.Pow (2, 0);
		}

		return squareConfig;
	}

	public int GetSquareConfig() {
		return squareConfig;
	}


}
