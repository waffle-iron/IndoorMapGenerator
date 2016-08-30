using System.Collections;


public class MarchingSquare {


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
	}


}
