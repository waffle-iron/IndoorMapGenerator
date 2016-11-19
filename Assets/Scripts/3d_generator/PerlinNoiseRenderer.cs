using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Linq.Expressions;

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


	public void RenderGraphMarkers(int mapResolutionX, int mapResolutionZ, int graphResolutionX, int graphResolutionY, int graphResolutionZ) {
		view.ClearGraph ();

		float graphMarkersPositionY = graphResolutionY / 2; //so that markers are drawn in between minY and maxY vals
		float graphMarkersDistanceX = mapResolutionX / graphResolutionX;
		float graphMarkersDistanceZ = mapResolutionZ / graphResolutionZ;

		for(int x = 0; x < graphResolutionX; ++x) {
			for(int z = 0; z < graphResolutionZ; ++z) {
				Vector3 position = new Vector3 (x * graphMarkersDistanceX, graphMarkersPositionY, z * graphMarkersDistanceZ);
				//TODO: USE CENTER OBJECT METHOD FROM UTILS CLASS!
				position.x -= mapResolutionX / 2f;
				position.z -= mapResolutionZ / 2f; 
				view.AddGraphMarker (
						position,
						true
				);
			}
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
