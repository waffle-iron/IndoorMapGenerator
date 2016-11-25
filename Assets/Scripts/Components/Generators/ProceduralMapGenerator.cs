﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Security.Policy;
using System.Deployment.Internal;
using UnityEditor;
using System.Linq;

[RequireComponent (typeof (ComputationUtils))]
[RequireComponent (typeof (PerlinNoiseRenderer))]
[ExecuteInEditMode] //https://docs.unity3d.com/ScriptReference/ExecuteInEditMode.html
public class ProceduralMapGenerator : MonoBehaviour {

//	public MapOutputObject mapOutputObject;

	public int 		mapResolutionX = 250;
	public int 		mapResolutionZ = 250;
	public int 		mapResolutionY;
//	public int 		perlinResolutionX = 10;
//	public int 		perlinResolutionZ = 10; //reincorporate this (to make low poly look)
	public int 		graphResolutionX = 25;
	public int 		graphResolutionZ = 25;
	private int 	graphResolutionXZ;
	public int 		graphResolutionY = 5;

	public int 		keyPoisPerc = 5;

	public int 		keyPoisRandomOffsetPerc = 0;
	private int 	keyPoisCount;

	public float 	keyPoisSizePerc = 1;
	public int 		keyPoisSizeRandomOffsetPerc = 0;

	public int		keyPoisConnectionsPerc = 100;

	public int 		nonKeyPoisPerc = 8;


	public float 	perlinNoiseScale = 0.3f;

	[Range(0f, 10f)] public int	blurRadius = 3;
	[Range(0f, 10f)] public int blurIterations = 4;
	[Range(0f, 5f)]  public int	blurSolidification = 2;

	[Range(-200, 100)]public int contrastPercent = 0;


	//this should be in some model class (Repository pattern?)
	private float[,] mapValuesArray;

	private Vector3 mapResolutionVector;
	private Vector3 graphResolutionVector;

//	private Vector3[] graphNodesPositions;
	private Graph 	graph = new Graph();

	private float	keyPoisSize;

	private ComputationUtils 		utils;
	private new PerlinNoiseRenderer renderer;



	void Start () {
		Debug.Log ("start");
	}

	void Update () {
		if (utils == null) {
			utils = GetComponent<ComputationUtils> ();
		}
		if(renderer == null) {
			renderer = GetComponent<PerlinNoiseRenderer> ();
		}
	}

	void Awake() {
	}

	void OnGUI() {
	}

	void OnRenderObject() {
	}


	//TODO: maybe separate Generators? Creating graph, perlin noise, map etc (this script will be GIGANTIC)
	public void GeneratePerlinNoiseValuesMap() {
		float[,] perlinNoiseMap = utils.GetUtilsRandom ().CreatePerlinNoise (
			mapResolutionX, mapResolutionZ, perlinNoiseScale
		);

		mapValuesArray = perlinNoiseMap;
		RenderValuesArray (mapValuesArray);
	}


	public void GenerateTestCrossValuesMap() {
		float[,] testCrossMap = utils.CreateTestCrossValues (mapResolutionX, mapResolutionZ);

		mapValuesArray = testCrossMap;
		RenderValuesArray (mapValuesArray);
	}

	public void ConvertGraphToValues() {
//		float[,] graphValuesAsArray = utils.GetUtilsMath ().ConvertGraphToValueMap (
//			graphNodesPositions, mapResolutionVector, graphResolutionVector
//		);
//
//		mapValuesArray = graphValuesAsArray;
//		RenderValuesArray (graphValuesAsArray);
	}


	public void GenerateVolumeBlock() {
		renderer.RenderVolumeBlock (new Vector3(mapResolutionX, mapResolutionY, mapResolutionZ));
	}

	public void ApplyGaussianBlur() {
		mapValuesArray = utils.GetGaussianBlur ().CreateGaussianBlur (
			mapValuesArray, 
			blurRadius, 
			blurIterations, 
			blurSolidification
		);

		RenderValuesArray (mapValuesArray);
	}

	public void ApplyContrast() {
//		if (mapValuesArray == null) {
//			GeneratePerlinNoiseValuesMap ();
//		}

		mapValuesArray = utils.GetUtilsMath ().ContrastValues (mapValuesArray, contrastPercent, 1f, 0f);
		RenderValuesArray (mapValuesArray);
	}


	public void GenerateGraphMarkers() {
		graphResolutionXZ = graphResolutionX * graphResolutionZ;
		Vector3[] graphMarkersPositions = new Vector3[graphResolutionXZ];

		for(int x = 0; x < graphResolutionX; ++x) {
			for(int z = 0; z < graphResolutionZ; ++z) {
				Vector3 position = new Vector3 (x, mapResolutionY / 2f, z);
				graphMarkersPositions [x * graphResolutionZ + z] = position;
			}
		}
			
		renderer.RenderGraphMarkers (graphMarkersPositions, mapResolutionVector, graphResolutionVector);
	}

	//todo: some clever alrorithm here (different for CONNECTING all nodes ONCE (for 100%), twice (for 200%) etc)
	// 		also, some sort of shortest path between critical pois
//	https://en.wikipedia.org/wiki/Breadth-first_search
//	https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
//	https://en.wikipedia.org/wiki/Prim's_algorithm 			!!!
//	https://en.wikipedia.org/wiki/Shortest_path_problem
//	https://en.wikipedia.org/wiki/Minimum_spanning_tree
//	https://en.wikipedia.org/wiki/Eulerian_path#Constructing_Eulerian_trails_and_circuits
//	http://www.graph-magics.com/articles/euler.php
	public void GenerateGraphPOIs() {
		CreateGraphPOIs ();
		RenderGraphPOIs ();
	}

	private void CreateGraphPOIs() {
		GenerateGraphMarkers ();
		keyPoisCount = (int) (graphResolutionXZ * (keyPoisPerc / 100f));

		Vector3[] graphKeyPoisPositions = new Vector3[keyPoisCount];
		Vector2[] graphKeyHorizontalPositions = utils.GetUtilsRandom ().GetUniqueRandomVectors2 (
			keyPoisCount, 
			0, graphResolutionX,
			0, graphResolutionZ,
			true
		);

		for (int i = 0; i < graphKeyPoisPositions.Length; ++i) {
			graphKeyPoisPositions [i] = new Vector3(
				graphKeyHorizontalPositions[i].x,
				utils.GetUtilsRandom ().GetUniqueRandomNumbers (1, 0f, graphResolutionY)[0],
				graphKeyHorizontalPositions[i].y
			);
		}

		//		graphNodesPositions = graphKeyPoisPositions;
		graph.ConstructGraph (
			graphKeyPoisPositions, 
			Enumerable.Repeat (1f, graphKeyPoisPositions.Length).ToArray (),
			Enumerable.Repeat (true, graphKeyPoisPositions.Length).ToArray ()
		);
	}

	private void RenderGraphPOIs() {
		renderer.RenderGraphKeyPois (graph.GetAllVerticesPositions (), mapResolutionVector, graphResolutionVector);
	}

	public void GenerateGraphEdges() {
//		for (int i = 0; i < graphNodesPositions.Length-1; ++i) {
//			Vector3 graphEdgeStart = graphNodesPositions [i];
//			Vector3 graphEdgeEnd = graphNodesPositions [i + 1];
//			renderer.RenderGraphEdge (graphEdgeStart, graphEdgeEnd);
//		}
	}


	private void RenderValuesArray(float[,] mapValuesArray) {
		renderer.RenderValuesArray (mapValuesArray);
	}

	void OnValidate() {
		Debug.Log ("onvalidate");

		graphResolutionVector.x = graphResolutionX;
		graphResolutionVector.y = graphResolutionY;
		graphResolutionVector.z = graphResolutionZ;

		mapResolutionVector.x = mapResolutionX;
		mapResolutionVector.y = mapResolutionY;
		mapResolutionVector.z = mapResolutionZ;
	}

}
