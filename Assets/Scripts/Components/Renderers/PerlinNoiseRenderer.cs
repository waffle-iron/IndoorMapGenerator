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
		view.ReplaceVolumeBlock (new Vector3(0f, scale.y/2f, 0f), scale);
	}


	public void RenderGraphMarkers(Vector3[] graphMarkersPositions, Vector3 mapResolutions, Vector3 graphResolutions) {
		ValidateGraphView ();
		view.ClearGraphMarkers ();
		Vector3 graphMarkerPosition;
//		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
//		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;

		for (int m = 0; m < graphMarkersPositions.Length; ++m) {
//			graphMarkerPosition = graphMarkersPositions[m];

//			graphMarkerPosition.x *= graphEntitiesDistanceX;
//			graphMarkerPosition.z *= graphEntitiesDistanceZ; 

//			graphMarkerPosition.x -= mapResolutions.x / 2f; 
//			graphMarkerPosition.z -= mapResolutions.z / 2f;   	//todo: this code snipped is duplicated 3x already, make utils method or sth
//			graphMarkerPosition.x += graphEntitiesDistanceX / 2f;
//			graphMarkerPosition.z += graphEntitiesDistanceZ / 2f;

			view.AddGraphMarker (
				AlignToResolutons (graphMarkersPositions[m], mapResolutions, graphResolutions), 
				true
			);
		}
	}

	public void RenderGraphKeyPois(Vector3[] graphKeyPoisPositions, Vector3 mapResolutions, Vector3 graphResolutions) {

		Vector3 graphKeyPoiPosition;
//		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
//		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;

		for (int p = 0; p < graphKeyPoisPositions.Length; ++p) {
//			graphKeyPoiPosition = graphKeyPoisPositions [p];

//			graphKeyPoiPosition.x *= graphEntitiesDistanceX;
//			graphKeyPoiPosition.z *= graphEntitiesDistanceZ;
//
//			graphKeyPoiPosition.x -= mapResolutions.x / 2f;
//			graphKeyPoiPosition.z -= mapResolutions.z / 2f; 
//			graphKeyPoiPosition.x += graphEntitiesDistanceX / 2f;
//			graphKeyPoiPosition.z += graphEntitiesDistanceZ / 2f;

//			view.AddGraphNode (graphKeyPoiPosition, true);
			view.AddGraphNode (
				AlignToResolutons (graphKeyPoisPositions [p], mapResolutions, graphResolutions),
				true
			);
		}
	}
		
	public void RenderGraphEdge(Vector3 positionA, Vector3 positionB, Vector3 mapResolutions, Vector3 graphResolutions) {

		Vector3 prevPosA = positionA;
		Vector3 prevPosB = positionB;

		Vector3 deltaPosition = positionB - positionA;
		deltaPosition.x /= 2f;
		deltaPosition.y /= 2f;
		deltaPosition.z /= 2f;

		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
		float graphEntitiesDistanceY = mapResolutions.y / (float)graphResolutions.y;
		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;


		positionA.x *= graphEntitiesDistanceX;
		positionA.y *= graphEntitiesDistanceY;
		positionA.z *= graphEntitiesDistanceZ;

		positionA.x -= mapResolutions.x / 2f;
		positionA.y -= mapResolutions.y / 2f;
		positionA.z -= mapResolutions.z / 2f;

		positionA.x += graphEntitiesDistanceX / 2f;
		positionA.y += graphEntitiesDistanceY / 2f;
		positionA.z += graphEntitiesDistanceZ / 2f;

		positionB.x *= graphEntitiesDistanceX;
		positionB.y *= graphEntitiesDistanceY;
		positionB.z *= graphEntitiesDistanceZ;

		positionB.x -= mapResolutions.x / 2f;
		positionB.y -= mapResolutions.y / 2f;
		positionB.z -= mapResolutions.z / 2f;

		positionB.x += graphEntitiesDistanceX / 2f;
		positionB.y += graphEntitiesDistanceY / 2f;
		positionB.z += graphEntitiesDistanceZ / 2f;


		view.AddGraphEdge (
			positionA + deltaPosition, 
			positionA - positionB,
			Vector3.Distance (positionA, positionB)
		);
	}


	//2D
	private Texture CreateValuesTexture(float[,] valuesArray, float rangeMin, float rangeMax) {

		int mapDimensionX = valuesArray.GetLength (0);
		int mapDimensionZ = valuesArray.GetLength (1); 

		Texture2D valueTexture = new Texture2D (mapDimensionX, mapDimensionZ);
		Color[] valueTextureColorMap = new Color[mapDimensionX * mapDimensionZ];

		for (int z = mapDimensionZ-1; z >= 0; --z) {
			for(int x = mapDimensionX-1; x >= 0; --x) {
					valueTextureColorMap [((mapDimensionX) * (mapDimensionZ))-1 -  z * (mapDimensionX) - x] = Color.Lerp(
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

	private Vector3 AlignToResolutons(Vector3 value, Vector3 mapResolutions, Vector3 graphResolutions) {
		float graphEntitiesDistanceX = mapResolutions.x / (float)graphResolutions.x;
		float graphEntitiesDistanceZ = mapResolutions.z / (float)graphResolutions.z;

		value.x *= graphEntitiesDistanceX;
		value.z *= graphEntitiesDistanceZ;

		value.x -= mapResolutions.x / 2f;
		value.z -= mapResolutions.z / 2f; 

		value.x += graphEntitiesDistanceX / 2f;
		value.z += graphEntitiesDistanceZ / 2f;

		return value;
	}

}
