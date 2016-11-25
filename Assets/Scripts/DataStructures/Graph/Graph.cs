using System;

//clarification for thesis: why Adjacency Matrix and not Adjacency List?
//http://stackoverflow.com/questions/2218322/what-is-better-adjacency-lists-or-adjacency-matrices-for-graph-problems-in-c

//http://stackoverflow.com/a/9843264/6942800 <- in thesis, for big graphs, using accessors is still almost as fast as direct (JIT inlining)
//http://stackoverflow.com/a/3265377/6942800
using System.Collections.Generic;
using UnityEngine;


public class Graph
{

	private GraphEdgesInfo[,] 	adjacencyMatrix;
	private GraphVertex[] 		verticesList;


	//todo: vertices POSITIONS should be sorted 
	public Graph (Vector3[] verticesPositions, float[] verticesRadiuses, bool[] isKeyPois) {
		ConstructGraph (verticesPositions, verticesRadiuses, isKeyPois);
	}

	public Graph() {}


	public GraphEdgesInfo[,] ConstructGraph(Vector3[] verticesPositions, float[] verticesRadiuses, bool[] isKeyPois) {

		verticesPositions = SortVerticesPositions (verticesPositions);

		verticesList = new GraphVertex[verticesPositions.Length];
		for (int i=0; i <verticesPositions.Length; ++i) {
			verticesList [i] = new GraphVertex (verticesPositions [i], verticesRadiuses [i], isKeyPois [i]);
		}

		adjacencyMatrix = new GraphEdgesInfo[verticesPositions.Length, verticesPositions.Length];

		return adjacencyMatrix;
	}

	public void SetEdge(int vertexAIndex, int vertexBIndex, bool clampValues) {
		if (vertexAIndex < 0 && vertexAIndex < verticesList.Length) {
			if (clampValues)
				vertexAIndex = Mathf.Clamp (vertexAIndex, 0, verticesList.Length);
			else
				return;
		}

		if (vertexBIndex < 0 && vertexBIndex < verticesList.Length) {
			if (clampValues)
				vertexAIndex = Mathf.Clamp (vertexBIndex, 0, verticesList.Length);
			else
				return;
		}

		adjacencyMatrix [vertexAIndex, vertexBIndex].IncrementConnectionCount (1f);
	}

	//here we should check if positions are sorted (if they are, we only do O(n) pass)
	private Vector3[] SortVerticesPositions(Vector3[] verticesPositions) {
		//sorting logic...
		return verticesPositions;
	}


	public Vector3[] GetAllVerticesPositions() {
		Vector3[] verticesPositions = new Vector3[GetVerticesCount ()];

		for (int i=0; i < verticesList.Length; ++i) {
			verticesPositions [i] = verticesList [i].GetPositionsVector ();
		}

		return verticesPositions;
	}

	public int GetVerticesCount() {
		return verticesList.Length;
	}

}