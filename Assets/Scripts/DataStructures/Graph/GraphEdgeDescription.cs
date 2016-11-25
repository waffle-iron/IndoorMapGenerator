using System;
using UnityEngine;


public class GraphEdgeDescription
{

	enum DescriptionFunction {
		LINEAR,
		HALF_CIRCULAR,
		BEZIER_QUADRATIC,
		BEZIER_CUBIC
	}


	//TODO: ACTUALLY IMPLEMENT TYPES OF DESCRIPTION FUNC IN CODE (first LINEAR, then EVERY OTHER)


	private DescriptionFunction descriptionFunction = DescriptionFunction.LINEAR;

	//	extra param for HALF_CIRCULAR function (radius, default is half of the distance between start and end point of the edge)
	private int parameterHalfCircularRadius = -1; 

	//	extra param for BEZIERs (quadratic and cubic) functions
	private Vector3 parameterBezierPoint2 = Vector3.zero;

	//	extra param for CUBIC BEZIER function
	private Vector3 parameterBezierPoint3 = Vector3.zero;


//	public GraphEdgeDescription (DescriptionFunction edgeDescriptionFunction, int parame)
//	{
//		descriptionFunction = edgeDescriptionFunction;
//	}
//
//	public GraphEdgeDescription SetEdgeDescriptionFunction(DescriptionFunction edgeDescriptionFunction) {
//		descriptionFunction = edgeDescriptionFunction;
//		return this;
//	}
//
//	public DescriptionFunction GetEdgeDescriptionFunction() {
//		return descriptionFunction;
//	}
}