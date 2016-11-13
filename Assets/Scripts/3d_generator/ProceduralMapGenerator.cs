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



	public void GeneratePerlinNoiseValuesMap() {
		float[,] perlinNoiseMap = GetComponent<ComputationUtils> ().CreatePerlinNoiseValues (
			mapTotalDimensionX, 
			mapTotalDimensionZ, 
			perlinNoiseScale
		);

		GetComponent<PerlinNoiseRenderer> ().RenderPerlinNoise (perlinNoiseMap);
	}



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
