using System;
using System.Collections.Generic;

public class GraphEdgesInfo
{	
	
	private int 		connectionCount = 0;
	private List<float>	edgesThicknesses;
	private GraphEdgeDescription[] edgesDescriptions;


	public GraphEdgesInfo () {
		edgesThicknesses = new List<float> ();
	}


	public int IncrementConnectionCount(float edgeThickness) {
		edgesThicknesses.Add (edgeThickness);
		return IncrementConnectionCount (1);
	}

	public int IncrementConnectionCount(int amount, float[] edgesThicknesses) {
		this.edgesThicknesses.AddRange (edgesThicknesses);
		connectionCount += amount;
		return connectionCount;
	}

	public int SetConnectionCount(int value, float[] edgesThicknesses) {
		this.edgesThicknesses.Clear ();
		this.edgesThicknesses.AddRange (edgesThicknesses);
		connectionCount = value;
		return connectionCount;
	}

}


