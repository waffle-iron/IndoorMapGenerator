﻿using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PerlinNoiseRenderer : MonoBehaviour {

	public  MapOutputObject viewPrefab;
	private MapOutputObject view;

	public Texture 			mapTexture = null;

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

	private void ValidateMeshView() {
		if (view.GetMeshView() == null) {
			view.InstantiateMeshView ();
		}
	}


	public void RenderMesh(Mesh mesh, float[,] heightMap, Vector3 meshScale) {
		ValidateMeshView ();
//		mapTexture = CreateValuesTexture (heightMap, 0f, 1f);
		view.ReplaceMesh (mesh, mapTexture, meshScale);
	}

	public void RenderValuesArray(float[,] valuesArray, float outputRenderSizeX = 1f, float outputRenderSizeZ = 1f, float rangeMin = 0f, float rangeMax = 1f, Constants.TextureType textureType = Constants.TextureType.TEXTURE_DEBUG_HEIGHT, Constants.TerrainSet terrainSet = null) {
		ValidateView ();
		ValidatePlaneView ();
		mapTexture = CreateValuesTexture (valuesArray, rangeMin, rangeMax, textureType, terrainSet);
		view.ReplacePlane (
			mapTexture,
			new Vector3(valuesArray.GetLength (0) * (float)outputRenderSizeX, 1, valuesArray.GetLength (1) * (float)outputRenderSizeZ)
		);
	}

	public void RenderVolumeBlock(Vector3 scale) {
		ValidateVolumeView ();
		view.ReplaceVolumeBlock (new Vector3(0f, scale.y/2f, 0f), scale);
	}


	//todo: renderer classes should get high level data info (like mapResVector and graphResVector) and then
	//calculating low level graphical info (like scale of objects, based on vectors difference) and passing that into views
	public void RenderGraphMarkers(Vector3[] graphMarkersPositions, Vector3 mapResolutions, Vector3 graphResolutions) {
		ValidateGraphView ();
		view.ClearGraphMarkers ();

		for (int m = 0; m < graphMarkersPositions.Length; ++m) {
			view.AddGraphMarker (
				AlignToResolutons (graphMarkersPositions[m], mapResolutions, graphResolutions), 
				new Vector3(mapResolutions.x/(graphResolutions.x * 1.5f), mapResolutions.y / 1.5f, mapResolutions.z/(graphResolutions.z * 1.5f)),
				true
			);
		}
	}

	public void RenderGraphKeyPois(Vector3[] graphKeyPoisPositions, Vector3 mapResolutions, Vector3 graphResolutions) {
		float nodeScaleValue = Mathf.Min (mapResolutions.x / graphResolutions.x, mapResolutions.z / graphResolutions.z);
		for (int p = 0; p < graphKeyPoisPositions.Length; ++p) {
			view.AddGraphNode (
				AlignToResolutons (graphKeyPoisPositions [p], mapResolutions, graphResolutions),
				new Vector3(nodeScaleValue, nodeScaleValue, nodeScaleValue),
				true
			);
		}
	}
		
	public void RenderGraphEdge(Vector3 positionA, Vector3 positionB, Vector3 mapResolutions, Vector3 graphResolutions) {

		positionA = AlignToResolutons (positionA, mapResolutions, graphResolutions);
		positionB = AlignToResolutons (positionB, mapResolutions, graphResolutions);

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

	private Texture CreateValuesTexture(float[,] valuesArray, float rangeMin = 0f, float rangeMax = 1f, Constants.TextureType textureType = Constants.TextureType.TEXTURE_DEBUG_HEIGHT, Constants.TerrainSet terrainSet = null) {
		if (textureType == Constants.TextureType.TEXTURE_MAP_STANDARD) {
			return CreateValuesTextureMap (valuesArray, terrainSet, rangeMin, rangeMax);
		}
		return CreateValuesTextureDebug (valuesArray, rangeMin, rangeMax);
	}


	private Texture CreateValuesTextureMap(float[,] valuesArray, Constants.TerrainSet terrainSet, float rangeMin = 0f, float rangeMax = 1f) {

		int mapDimensionX = valuesArray.GetLength (0);
		int mapDimensionZ = valuesArray.GetLength (1); 

		Texture2D valueTexture = new Texture2D (mapDimensionX, mapDimensionZ);
		Color[] valueTextureColorMap = new Color[mapDimensionX * mapDimensionZ];

		for (int z = mapDimensionZ-1; z >= 0; --z) {
			for(int x = mapDimensionX-1; x >= 0; --x) {

				for (int l = 0; l < terrainSet.terrainTypes.Length; ++l) {
					float actualVal = Mathf.InverseLerp (rangeMin, rangeMax, valuesArray [x, z]);
					float terrainMaxVal = terrainSet.terrainTypes [l].rangeMaxHeight;

					bool statement = actualVal < terrainMaxVal;

					if (statement) {
						valueTextureColorMap [((mapDimensionX) * (mapDimensionZ)) - 1 - z * (mapDimensionX) - x] = Color.Lerp (
							terrainSet.terrainTypes[l].rangeMinColour, 
							terrainSet.terrainTypes[l].rangeMaxColour,
							Mathf.InverseLerp (
								l-1>=0f? terrainSet.terrainTypes[l-1].rangeMaxHeight : 0f, terrainSet.terrainTypes[l].rangeMaxHeight, valuesArray [x, z])
						);
						break;
					}
				}

			}
		}

		valueTexture.SetPixels (valueTextureColorMap);

		valueTexture.filterMode = FilterMode.Point;
		valueTexture.Apply ();

		return valueTexture;
	}


	private Texture CreateValuesTextureDebug(float[,] valuesArray, float rangeMin = 0f, float rangeMax = 1f) {

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
