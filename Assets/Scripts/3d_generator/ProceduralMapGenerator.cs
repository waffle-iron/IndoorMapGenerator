using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

[RequireComponent (typeof (ComputationUtils))]
[RequireComponent (typeof (PerlinNoiseRenderer))]
public class ProceduralMapGenerator : MonoBehaviour {


	public int 		mapTotalDimensionX = 10;
	public int 		mapTotalDimensionZ = 10;
	public int 		mapTotalDimensionY;
	public float 	perlinNoiseScale = 0.3f;
	public int		blurRadius = 3;
	public int 		blurIterations = 4;
	public int		blurSolidification = 2;

	float[,] mapValuesArray;


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
