using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Security.Policy;
using System.Deployment.Internal;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

[RequireComponent (typeof (ComputationUtils))]
[RequireComponent (typeof (PerlinNoiseRenderer))]
[ExecuteInEditMode] //https://docs.unity3d.com/ScriptReference/ExecuteInEditMode.html
public class ProceduralMapGenerator : MonoBehaviour {

//	public MapOutputObject mapOutputObject;

	public int 		mapResolutionX = 150;
	public int 		mapResolutionZ = 150;
	public int 		mapResolutionY;
	public int 		perlinResolutionX = 50;
	public int		perlinResolutionZ = 50;
	public float	perlinScaleX = 1;
	public float 	perlinScaleZ = 1;
	public float 	perlinNoiseScale = 0.3f;
	public int 		perlinLayers = 3;
	[Range(0,1)] public float perlinPersistence = 0.5f;
	[Range(1, 10)] public float perlinLacunarity = 1.5f;

//	public int 		perlinResolutionX = 10;
//	public int 		perlinResolutionZ = 10; //reincorporate this (to make low poly look)
	public int 		graphResolutionX = 20;
	public int 		graphResolutionZ = 20;
	private int 	graphResolutionXZ;
	public int 		graphResolutionY = 5;

	public int 		meshResolutionX = 75;
	public int 		meshResolutionZ = 75;

	public float 	vertexSize = 1;
	[Range(-200,200)] public int vertexSizeRndOffset = 0;
	public int		edgeThickness = 1;

	public int 		keyPoisPerc = 5;

	public int 		keyPoisRandomOffsetPerc = 0;
	private int 	keyPoisCount;
	public float 	keyPoisSizePerc = 1;
	public int 		keyPoisSizeRandomOffsetPerc = 0;
	public int		keyPoisConnectionsPerc = 100;

	public int 		nonKeyPoisPerc = 8;

	[Range(0f, 10f)] public int	blurRadius = 3;
	[Range(0f, 10f)] public int blurIterations = 4;
	[Range(0f, 5f)]  public int	blurSolidification = 2;

	[Range(-200, 100)]public int contrastPercent = 0;


	//this all should be in some model class (Repository pattern?) !!!!!!!!!!!!!!!!!
	private float[,] noiseValuesArray;
	private float[,] graphValuesArray;
	private float[,] finalValuesArray;
	private int activeValuesArray = 0;
	private Graph 	graph = new Graph();
	private MeshWrapper meshWrapper = new MeshWrapper ();


	private Vector3 mapResolutionVector;
	private Vector3 graphResolutionVector;

//	private Vector3[] graphNodesPositions;


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
		//		finalValuesArray = new float[,];
		float[,] perlinNoiseMap = utils.GetUtilsRandom ().CreatePerlinNoise (
			mapResolutionX, mapResolutionZ, perlinResolutionX, perlinResolutionZ, perlinScaleX, perlinScaleZ,
			perlinLayers, perlinPersistence, perlinLacunarity
		);

		SetNoiseValuesArray (perlinNoiseMap);
		RenderValuesArray ();
	}


	public void GenerateMesh() {
		meshWrapper = utils.GetUtilsGFX ().GenerateMesh (
//			finalValuesArray != null ? finalValuesArray : GetActiveValuesArray (), 
			GetActiveValuesArray (),
			mapResolutionX/meshResolutionX, 
			mapResolutionZ/meshResolutionZ
		);
		renderer.RenderMesh (
			meshWrapper.GenerateMesh (), 
			GetActiveValuesArray (),
			mapResolutionY
		);
	}

	public void GenerateTestCrossValuesMap() {
		float[,] testCrossMap = utils.CreateTestCrossValues (mapResolutionX, mapResolutionZ);

		SetNoiseValuesArray (testCrossMap);
		RenderValuesArray ();
	}

	public void ConvertGraphToValues() {
		ConvertGraphVerticesToValues ();
		ConvertGraphEdgesToValues ();
	}

	private void ConvertGraphVerticesToValues() {
		float[,] graphValuesAsArray = utils.GetUtilsMath ().ConvertGraphToValueMap (
			graph.GetAllVerticesPositions (), mapResolutionVector, graphResolutionVector
		);

		SetGraphValuesArray (graphValuesAsArray);
		RenderValuesArray ();
	}

	private void ConvertGraphEdgesToValues() {
		float[,] graphEdgeValuesAsArray = utils.GetUtilsMath ().ConvertGraphEdgesToValueMap (
			graph.GetEdgesStartEndPositions (),
			mapResolutionVector,
			graphResolutionVector,
			edgeThickness
		);

		SetGraphValuesArray (
			utils.GetUtilsMath ().MergeArrays (
				graphValuesArray, 
				graphEdgeValuesAsArray, 
				0f, 1f, 
				MathUtils.MergeArrayMode.XOR)
		);
		RenderValuesArray ();
	}


	public void GenerateVolumeBlock() {
		renderer.RenderVolumeBlock (new Vector3(mapResolutionX, mapResolutionY, mapResolutionZ));
	}

	public void ApplyGaussianBlur() {
		SetActiveValuesArray (utils.GetGaussianBlur ().CreateGaussianBlur (
			GetActiveValuesArray (), 
			blurRadius, 
			blurIterations, 
			blurSolidification
		)
		);
		RenderValuesArray ();
	}

	public void ApplyContrast() {
		SetActiveValuesArray (utils.GetUtilsMath ().ContrastValues (GetActiveValuesArray (), contrastPercent, 1f, 0f));
//		mapValuesArray = utils.GetUtilsMath ().ContrastValues (mapValuesArray, contrastPercent, 1f, 0f);
		RenderValuesArray ();
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
		GenerateGraphKeyPOIs ();
		RenderGraphPOIs (); //todo: should it be separated or not? rest of the methods are not separated like that...
	}


	private void GenerateGraphKeyPOIs() {
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
			
		graph.ConstructGraph (
			graphKeyPoisPositions, 
			Enumerable.Repeat (1f, graphKeyPoisPositions.Length).ToArray (),
			Enumerable.Repeat (true, graphKeyPoisPositions.Length).ToArray ()
		);
	}

	//todo; check hardcoded values that go into some math methods - maybe they can be parameters

	//todo: thicker bresenham line (for every iteration colour position (x,z) and (x+1,z) and (x+2,z) or add to Z instead of X, depends on inversion)

	//todo: calculate base size of a graph vertex (1)
	//todo: make editor variable 'rand size perc' etc

	public void MergeIntoFinalArray() {
		SetFinalValuesArray (utils.GetUtilsMath ().MergeArrays (
			noiseValuesArray, graphValuesArray,
			0f, 1f,
			MathUtils.MergeArrayMode.SUBTRACT,
			2f
		));
//		SetFinalValuesArray (new float[mapResolutionX, mapResolutionZ]);

		RenderValuesArray ();
	}


	public void GenerateGraphEdges() {
		for (int i = 0; i < graph.GetVerticesCount ()-1; ++i) {
			int graphEdgeStartVertexIndex = i;
			int graphEdgeEndVertexIndex = i+1;
			graph.AddEdge (graphEdgeStartVertexIndex, graphEdgeEndVertexIndex, true);
		}


		Vector2[] edgeVerticesIndexes = graph.GetEdgesStartEndIndexes ();
		for (int i=0; i < edgeVerticesIndexes.Length; ++i) {
			renderer.RenderGraphEdge (
				graph.GetVertexPosition (Mathf.RoundToInt (edgeVerticesIndexes[i].x)),
				graph.GetVertexPosition (Mathf.RoundToInt (edgeVerticesIndexes[i].y)),
				mapResolutionVector,
				graphResolutionVector
			);
		}
	}

	private void RenderGraphPOIs() {
		renderer.RenderGraphKeyPois (graph.GetAllVerticesPositions (), mapResolutionVector, graphResolutionVector);
	}

	private void RenderValuesArray() {
		renderer.RenderValuesArray (GetActiveValuesArray ());
	}

	public void RenderNoiseValuesArray() {
		renderer.RenderValuesArray (noiseValuesArray);
	}

	public void RenderGraphValuesArray() {
		renderer.RenderValuesArray (graphValuesArray);
	}

	public void RenderFinalValuesArray() {
		renderer.RenderValuesArray (finalValuesArray);
	}

//	private void RenderValuesArray(float[,] mapValuesArray) {
//		renderer.RenderValuesArray (mapValuesArray);
//	}

	private float[,] GetActiveValuesArray() {
		switch(activeValuesArray) {
			case 0:
				return noiseValuesArray;
			case 1: 
				return graphValuesArray;
			default:
				return finalValuesArray;
		}
	}

	private void SetNoiseValuesArray(float[,] values) {
		noiseValuesArray = values;
		activeValuesArray = 0;
	}

	private void SetGraphValuesArray(float[,] values) {
		graphValuesArray = values;
		activeValuesArray = 1;
	}

	private void SetFinalValuesArray(float[,] values) {
		finalValuesArray = values;
		activeValuesArray = 2;
	}

	private void SetActiveValuesArray(float[,] values) {
		switch(activeValuesArray) {
			case 0:
				SetNoiseValuesArray (values);
				break;
			case 1: 
				SetGraphValuesArray (values);
				break;
			default:
				SetFinalValuesArray (values);
				break;
		}
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
