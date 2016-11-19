using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Security.Policy;

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
	public int 		graphResolutionY = 5;

	public int 		graphKeyPoisPerc = 5;
	public int 		graphNonKeyPoisPerc = 8;


	public float 	perlinNoiseScale = 0.3f;

	[Range(0f, 10f)] public int	blurRadius = 3;
	[Range(0f, 10f)] public int blurIterations = 4;
	[Range(0f, 5f)]  public int	blurSolidification = 2;

	[Range(-200, 100)]public int contrastPercent = 0;

	//TODO: non-square resolutions dont work (like

	private float graphMarkersPositionY;
	private float graphMarkersDistanceX;
	private float graphMarkersDistanceZ;

	//this should be in some model class (Repository pattern?)
	private float[,] mapValuesArray;
//	private 



	//todo: keep reference of ComputationUtils in variable here somewhere
	//(PROS: so that we do not take reflective lookups every function?
	// MAYBE CONS: memory leaks?)


	void Start () {
		Debug.Log ("start");
	}

	void Update () {
		
	}

	void Awake() {
	}

	void OnGUI() {
	}

	void OnRenderObject() {
	}


	//TODO: maybe separate Generators? Creating graph, perlin noise, map etc (this script will be GIGANTIC)
	public void GeneratePerlinNoiseValuesMap() {
		float[,] perlinNoiseMap = GetComponent<ComputationUtils> ().CreatePerlinNoiseValues (
			mapResolutionX, 
			mapResolutionZ, 
			perlinNoiseScale
		);

		mapValuesArray = perlinNoiseMap;
		RenderValuesArray (mapValuesArray);
	}

	public void GenerateTestCrossValuesMap() {
		float[,] testCrossMap = GetComponent<ComputationUtils> ().CreateTestCrossValues (
			mapResolutionX, 
			mapResolutionZ
		);

		mapValuesArray = testCrossMap;
		RenderValuesArray (mapValuesArray);
	}

	public void ApplyGaussianBlur() {
		mapValuesArray = GetComponent<ComputationUtils> ().GaussianBlur (
			mapValuesArray, 
			blurRadius, 
			blurIterations, 
			blurSolidification
		);

		RenderValuesArray (mapValuesArray);
	}

	public void ApplyContrast() {
		if (mapValuesArray == null) {
			GeneratePerlinNoiseValuesMap ();
		}

		mapValuesArray = GetComponent<ComputationUtils> ().ContrastValues (
			mapValuesArray, contrastPercent, 
			1f, 0f
		);
		RenderValuesArray (mapValuesArray);
	}


	public void GenerateGraphMarkers() {

		Vector3[] graphMarkersPositions = new Vector3[graphResolutionX * graphResolutionZ];

		for(int x = 0; x < graphResolutionX; ++x) {
			for(int z = 0; z < graphResolutionZ; ++z) {
				Vector3 position = new Vector3 (x * graphMarkersDistanceX, graphMarkersPositionY, z * graphMarkersDistanceZ);
				//TODO: USE CENTER OBJECT METHOD FROM UTILS CLASS!
				position.x -= mapResolutionX / 2f;
				position.z -= mapResolutionZ / 2f; 
				position.x += graphMarkersDistanceX / 2f;
				position.z += graphMarkersDistanceZ / 2f;
				graphMarkersPositions [x * graphResolutionZ + z] = position;
			}
		}
			
		GetComponent<PerlinNoiseRenderer> ().RenderGraphMarkers (graphMarkersPositions);
	}

	public void GenerateGraphPOIs() {
		
	}

	private void RenderValuesArray(float[,] mapValuesArray) {
		GetComponent<PerlinNoiseRenderer> ().RenderValuesArray (mapValuesArray);
	}

	void OnValidate() {
		Debug.Log ("onvalidate");
	 	graphMarkersPositionY = graphResolutionY / 2f; //so that markers are drawn in between minY and maxY vals
		graphMarkersDistanceX = mapResolutionX / (float)graphResolutionX;
		graphMarkersDistanceZ = mapResolutionZ / (float)graphResolutionZ;
	}

}
