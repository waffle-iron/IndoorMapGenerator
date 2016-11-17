using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Security.Policy;

[RequireComponent (typeof (ComputationUtils))]
[RequireComponent (typeof (PerlinNoiseRenderer))]
public class ProceduralMapGenerator : MonoBehaviour {


	public int 		mapTotalDimensionX = 10;
	public int 		mapTotalDimensionZ = 10;
	public int 		mapTotalDimensionY;
	public float 	perlinNoiseScale = 0.3f;

	[Range(0f, 10f)] public int	blurRadius = 3;

	[Range(0f, 10f)] public int blurIterations = 4;

	[Range(0f, 5f)] public int	blurSolidification = 2;

	public int contrastPercent;

	private float[,] mapValuesArray;


	//todo: keep reference of ComputationUtils in variable here somewhere
	//(PROS: so that we do not take reflective lookups every function?
	// MAYBE CONS: memory leaks?)

	public void ApplyContrast() {
		if (mapValuesArray == null) {
			GeneratePerlinNoiseValuesMap ();
		}

		mapValuesArray = GetComponent<ComputationUtils> ().ContrastValues (mapValuesArray, contrastPercent, new float[]{1f, 0f});

		GetComponent<PerlinNoiseRenderer> ().RenderValuesArray (mapValuesArray);
	}

	public void GeneratePerlinNoiseValuesMap() {
		float[,] perlinNoiseMap = GetComponent<ComputationUtils> ().CreatePerlinNoiseValues (
			mapTotalDimensionX, 
			mapTotalDimensionZ, 
			perlinNoiseScale
		);

		mapValuesArray = perlinNoiseMap;

		GetComponent<PerlinNoiseRenderer> ().RenderValuesArray (mapValuesArray);
	}

	public void GenerateTestCrossValuesMap() {
		float[,] testCrossMap = GetComponent<ComputationUtils> ().CreateTestCrossValues (
			mapTotalDimensionX, 
			mapTotalDimensionZ
		);

		mapValuesArray = testCrossMap;

		GetComponent<PerlinNoiseRenderer> ().RenderValuesArray (mapValuesArray);
	}

	public void ApplyGaussianBlur() {
		mapValuesArray = GetComponent<ComputationUtils> ().GaussianBlur (mapValuesArray, blurRadius, blurIterations, blurSolidification);

		GetComponent<PerlinNoiseRenderer> ().RenderValuesArray (mapValuesArray);
	}


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
