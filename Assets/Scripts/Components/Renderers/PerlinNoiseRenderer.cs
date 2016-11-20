using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

public class PerlinNoiseRenderer : MonoBehaviour {

	public  MapOutputObject viewPrefab;
	private MapOutputObject view;

	void Reset() {
		ValidateView ();
	}

	public void ValidateView() {
		if (view == null) {
			view = (MapOutputObject) Instantiate (viewPrefab);
		}
		ValidatePlaneView ();
		ValidateGraphView ();
	}

	private void ValidatePlaneView() {
		if (view.GetPlaneView () == null) {
			view.InstantiatePlane ();
		}
	}

	private void ValidateGraphView() {
		if (view.GetGraphView () == null) {
			view.InstantiateGraph ();
		}
	}


	public void RenderGraphMarkers(Vector3[] graphMarkersPositions) {
		view.ClearGraph ();
		for (int m = 0; m < graphMarkersPositions.Length; ++m) {
			view.AddGraphMarker (graphMarkersPositions [m], true);
		}
	}

	public void RenderGraphKeyPois(Vector3[] graphKeyPoisPositions) {
		for (int p = 0; p < graphKeyPoisPositions.Length; ++p) {
			view.AddGraphNode (graphKeyPoisPositions [p], true);
		}
	}


	public void RenderValuesArray(float[,] valuesArray, float rangeMin = 0f, float rangeMax = 1f) {
		ValidateView ();
		view.GetPlaneView ().sharedMaterial.mainTexture = CreateValuesTexture (
			valuesArray, 
			rangeMin, 
			rangeMax
		);
		view.GetPlaneView().GetComponent<Renderer> ().transform.localScale = new Vector3 (
			valuesArray.GetLength (0), 
			1, 
			valuesArray.GetLength (1)
		);
	}


	//2D
	private Texture CreateValuesTexture(float[,] valuesArray, float rangeMin, float rangeMax) {
		int mapDimensionX = valuesArray.GetLength (0);
		int mapDimensionZ = valuesArray.GetLength (1);

		Texture2D valueTexture = new Texture2D (mapDimensionX, mapDimensionZ);
		Color[] valueTextureColorMap = new Color[mapDimensionX * mapDimensionZ];

		for(int x = 0; x < mapDimensionX; ++x) {
			for (int z = 0; z < mapDimensionZ; ++z) {
				valueTextureColorMap [x * mapDimensionZ + z] = Color.Lerp(
					Color.yellow, 
					Color.red, 
					Mathf.InverseLerp(rangeMin, rangeMax, valuesArray[x, z])
				);
			}
		}

		valueTexture.SetPixels (valueTextureColorMap);
		valueTexture.filterMode = FilterMode.Point;
		valueTexture.Apply ();

		return valueTexture;
	}

}
