using System;

using System.Collections.Generic;
using UnityEngine;


//clarification for thesis: why Adjacency Matrix and not Adjacency List?
//http://stackoverflow.com/questions/2218322/what-is-better-adjacency-lists-or-adjacency-matrices-for-graph-problems-in-c
//http://stackoverflow.com/a/9843264/6942800 <- in thesis, for big graphs, using accessors is still almost as fast as direct (JIT inlining)
//http://stackoverflow.com/a/3265377/6942800
using System.Runtime.Remoting.Messaging;


public class Graph
{

	private GraphEdgesInfo[,] 	adjacencyMatrix;
	private GraphVertex[] 		verticesList;
	private int 				edgesCount;

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
		for (int x = 0; x < adjacencyMatrix.GetLength (0); ++x) {
			for (int z = 0; z < adjacencyMatrix.GetLength (1); ++z) {	
				adjacencyMatrix [x, z] = new GraphEdgesInfo ();
			}
		}

		edgesCount = 0;

		return adjacencyMatrix;
	}

	public void AddEdge(int vertexAIndex, int vertexBIndex, bool clampValues) {
		if (vertexAIndex < 0 && vertexAIndex < verticesList.Length) {
			if (clampValues)
				vertexAIndex = Mathf.Clamp (vertexAIndex, 0, verticesList.Length-1);
			else
				return;
		}

		if (vertexBIndex < 0 && vertexBIndex < verticesList.Length) {
			if (clampValues)
				vertexAIndex = Mathf.Clamp (vertexBIndex, 0, verticesList.Length-1);
			else
				return;
		}

		adjacencyMatrix [vertexAIndex, vertexBIndex].IncrementConnectionCount (1f);
		adjacencyMatrix [vertexBIndex, vertexAIndex].IncrementConnectionCount (1f);
		++edgesCount;
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

	public int GetEdgesCount() {
		return edgesCount;
	}

	public Vector2[] GetEdgesStartEndIndexes() {

		List<Vector2> indexPairs = new List<Vector2> (edgesCount); 

		Vector2 pair = new Vector2 ();
		for (int x = 0; x < verticesList.Length; ++x) {
			for (int z = 0; z <= x; ++z) {
				for (int i=0; i < adjacencyMatrix[x, z].GetConnectionCount (); ++i) {
					pair.x = x;
					pair.y = z;
					indexPairs.Add (pair);
				}
			}
		}

		return indexPairs.ToArray ();
	}

	public Vector3[,] GetEdgesStartEndPositions() {
		Vector2[] edgesStartEndIndexes = GetEdgesStartEndIndexes ();

		Vector3[,] edgesStartEndPositions = new Vector3[edgesStartEndIndexes.Length, 2];

		for (int i=0 ; i < edgesStartEndIndexes.Length; ++i) {
			edgesStartEndPositions [i, 0] = GetVertexPosition((int)edgesStartEndIndexes [i].x);
			edgesStartEndPositions [i, 1] = GetVertexPosition((int)edgesStartEndIndexes [i].y);
		}

		return edgesStartEndPositions;
	}

	public Vector3 GetVertexPosition(int vertexIndex) {
		return verticesList [vertexIndex].GetPositionsVector ();
	}
}