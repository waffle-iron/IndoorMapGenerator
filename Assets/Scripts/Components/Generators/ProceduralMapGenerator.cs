﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Security.Policy;
using System.Deployment.Internal;
using UnityEditor;

[RequireComponent (typeof (ComputationUtils))]
[RequireComponent (typeof (PerlinNoiseRenderer))]
[ExecuteInEditMode] //https://docs.unity3d.com/ScriptReference/ExecuteInEditMode.html
public class ProceduralMapGenerator : MonoBehaviour {

//	public MapOutputObject mapOutputObject;

	public int 		mapResolutionX = 250;
	public int 		mapResolutionZ = 250;
	public int 		mapResolutionY;
//	public int 		perlinResolutionX = 10;
//	public int 		perlinResolutionZ = 10;
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


	private float graphMarkersPositionY;
	private float graphMarkersDistanceX;
	private float graphMarkersDistanceZ;

	//this should be in some model class (Repository pattern?)
	private float[,] mapValuesArray;

	private Vector3[] graphNodesPositions;
	private float	keyPoisSize;

	private ComputationUtils 		utils;
	private new PerlinNoiseRenderer renderer;


	//todo: keep reference of ComputationUtils in variable here somewhere
	//(PROS: so that we do not take reflective lookups every function?
	// MAYBE CONS: memory leaks?)


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
		float[,] graphValuesAsArray = utils.GetUtilsMath ().ConvertGraphToValues (
			mapResolutionX, mapResolutionY, mapResolutionZ,
			graphResolutionX, graphResolutionY, graphResolutionZ,
			graphNodesPositions
		);

		RenderValuesArray (graphValuesAsArray);
	}


	public void GenerateVolumeBlock() {
		renderer.RenderVolumeBlock (new Vector3(mapResolutionX, mapResolutionY, mapResolutionZ));
	}

	public void ApplyGaussianBlur() {
		mapValuesArray = utils.GetGaussianBlur ().CreateGaussianBlur (
			mapValuesArray, 
			blurRadius, blurIterations, blurSolidification
		);

		RenderValuesArray (mapValuesArray);
	}

	public void ApplyContrast() {
		if (mapValuesArray == null) {
			GeneratePerlinNoiseValuesMap ();
		}

		mapValuesArray = utils.GetUtilsMath ().ContrastValues (mapValuesArray, contrastPercent, 1f, 0f);
		RenderValuesArray (mapValuesArray);
	}


	public void GenerateGraphMarkers() {
		graphResolutionXZ = graphResolutionX * graphResolutionZ;
		Vector3[] graphMarkersPositions = new Vector3[graphResolutionXZ];

		for(int x = 0; x < graphResolutionX; ++x) {
			for(int z = 0; z < graphResolutionZ; ++z) {
				Vector3 position = new Vector3 (
					x * graphMarkersDistanceX, 
					graphMarkersPositionY, 
					z * graphMarkersDistanceZ
				);
				//TODO: USE CENTER OBJECT METHOD FROM UTILS CLASS!
				position.x -= mapResolutionX / 2f;
				position.z -= mapResolutionZ / 2f; 
				position.x += graphMarkersDistanceX / 2f;
				position.z += graphMarkersDistanceZ / 2f;
				graphMarkersPositions [x * graphResolutionZ + z] = position;
			}
		}
			
		renderer.RenderGraphMarkers (graphMarkersPositions);
	}

	public void GenerateGraphPOIs() {

		GenerateGraphMarkers ();

		keyPoisCount = (int) (graphResolutionXZ * (keyPoisPerc / 100f));
//			(utils.GetUtilsRandom ().RandomRangeMiddleVal (keyPoisPerc, keyPoisRandomOffsetPerc) / 100);

		Vector3[] graphKeyPoisPositions = new Vector3[keyPoisCount];

		Vector2[] graphKeyHorizontalPositions = utils.GetUtilsRandom ().GetUniqueRandomVectors2 (
			keyPoisCount, 
			0, graphResolutionX,
			0, graphResolutionZ,
			true
		);

//		Vector2[] graphKeyHorizontalPositions = new Vector2[keyPoisCount * 2 + 4];
//
//		float graphx = graphResolutionX / keyPoisCount;
//		float graphz = graphResolutionZ / keyPoisCount;
//
//		graphKeyHorizontalPositions [0] = new Vector2 (0f, 0f);
//		graphKeyHorizontalPositions [1] = new Vector2 (0f, graphResolutionZ);
//		graphKeyHorizontalPositions [2] = new Vector2 (graphResolutionX, graphResolutionZ);
//		graphKeyHorizontalPositions [3] = new Vector2 (graphResolutionX, 0f);
//
//
//		for (int i=4; i < keyPoisCount; i=i+2) {
//			graphKeyHorizontalPositions [i].x = graphx * i;
//			graphKeyHorizontalPositions [i].y = graphz * i;
//
//			if (!(i == 0)) {
//				graphKeyHorizontalPositions [i + 1].x = graphx * i;
//				graphKeyHorizontalPositions [i + 1].y = graphResolutionZ - (graphz * i);
//			}
//		}

		for (int i = 0; i < graphKeyPoisPositions.Length; ++i) {
			graphKeyPoisPositions [i] = new Vector3(
				graphKeyHorizontalPositions[i].x * graphMarkersDistanceX,
				utils.GetUtilsRandom ().GetUniqueRandomNumbers (1, 0f, graphResolutionY)[0],
				graphKeyHorizontalPositions[i].y * graphMarkersDistanceZ
			);

			graphKeyPoisPositions [i].x -= mapResolutionX / 2f;
			graphKeyPoisPositions [i].z -= mapResolutionZ / 2f; 
			graphKeyPoisPositions [i].x += graphMarkersDistanceX / 2f;
			graphKeyPoisPositions [i].z += graphMarkersDistanceZ / 2f;
		}

		graphNodesPositions = graphKeyPoisPositions;
		renderer.RenderGraphKeyPois (graphKeyPoisPositions);
	}

	public void GenerateGraphEdges() {
		for (int i = 0; i < graphNodesPositions.Length-1; ++i) {
			Vector3 graphEdgeStart = graphNodesPositions [i];
			Vector3 graphEdgeEnd = graphNodesPositions [i + 1];
			renderer.RenderGraphEdge (graphEdgeStart, graphEdgeEnd);
		}
	}


	private void RenderValuesArray(float[,] mapValuesArray) {
		renderer.RenderValuesArray (mapValuesArray);
	}

	void OnValidate() {
		Debug.Log ("onvalidate");
	 	graphMarkersPositionY = graphResolutionY / 2f; //so that markers are drawn in between minY and maxY vals
		graphMarkersDistanceX = mapResolutionX / (float)graphResolutionX;
		graphMarkersDistanceZ = mapResolutionZ / (float)graphResolutionZ;
	}

}
