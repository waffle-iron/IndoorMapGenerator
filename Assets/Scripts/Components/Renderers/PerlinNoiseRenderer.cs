using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PerlinNoiseRenderer : MonoBehaviour {

	public  MapOutputObject viewPrefab;
	private MapOutputObject view;

	void Reset() {
//		ValidateView ();
	}

	void Update() {
//		ValidateView ();
	}


	private void ValidateView() {
		if (view == null) {
			view = (MapOutputObject) Instantiate (viewPrefab);
		}
	}

	private void ValidatePlaneView() {
		if (view.GetPlaneView () == null) {
			view.InstantiatePlaneView ();
		}
	}

	private void ValidateVolumeView() {
		if (view.GetVolumeView() == null) {
			view.InstantiateVolumeView ();
		}
	}

	private void ValidateGraphView() {
		if (view.GetGraphView () == null) {
			view.InstantiateGraphView ();
		} else if (!view.CheckGraphChildrenExistence ()) {
			view.InstantiateGraphView ();
		}
	}

	public void RenderValuesArray(float[,] valuesArray, float rangeMin = 0f, float rangeMax = 1f) {
		ValidateView ();
		ValidatePlaneView ();
		view.ReplacePlane (
			CreateValuesTexture (valuesArray, rangeMin, rangeMax),
			new Vector3(valuesArray.GetLength(0), 1, valuesArray.GetLength(1))
		);
	}

	public void RenderVolumeBlock(Vector3 scale) {
		ValidateVolumeView ();
//		scale.y /= 2f;
		view.ReplaceVolumeBlock (new Vector3(0f, scale.y/2f, 0f), scale);
	}


	public void RenderGraphMarkers(Vector3[] graphMarkersPositions) {
		ValidateGraphView ();
		view.ClearGraphMarkers ();
		for (int m = 0; m < graphMarkersPositions.Length; ++m) {
			view.AddGraphMarker (graphMarkersPositions [m], true);
		}
	}

	public void RenderGraphKeyPois(Vector3[] graphKeyPoisPositions) {
		for (int p = 0; p < graphKeyPoisPositions.Length; ++p) {
			view.AddGraphNode (graphKeyPoisPositions [p], true);
		}
	}
		
	public void RenderGraphEdge(Vector3 positionA, Vector3 positionB) {
		Vector3 deltaPosition = positionB - positionA;
		deltaPosition.x /= 2f;
		deltaPosition.y /= 2f;
		deltaPosition.z /= 2f;

		view.AddGraphEdge (
			positionA + deltaPosition, 
			positionA - positionB,
			Vector3.Distance (positionA, positionB)
		);
	}


	//2D
	private Texture CreateValuesTexture(float[,] valuesArray, float rangeMin, float rangeMax) {
		int mapDimensionX = valuesArray.GetLength (0);

		Texture2D valueTexture = new Texture2D (mapDimensionX, mapDimensionZ);
		Color[] valueTextureColorMap = new Color[mapDimensionX * mapDimensionZ];

		for(int x = 0; x < mapDimensionX; ++x) {
			for (int z = 0; z < mapDimensionZ; ++z) {
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
