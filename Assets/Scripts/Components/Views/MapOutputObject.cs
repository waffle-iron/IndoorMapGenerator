using UnityEngine;
using System.Collections;

public class MapOutputObject : MonoBehaviour {

	public Renderer 	planePrefab;//for now, later it should be private and should be modified by some renderer (like PNoiseRenderer)
	public Renderer		volumePrefab;
	public Renderer		graphMarkerPrefab;
	public Renderer		graphNodePrefab;
	public Renderer		graphEdgePrefab;
	public Renderer		meshPrefab;

	private GameObject 	planeView;
	private GameObject	volumeView;

	private GameObject	graphView;
	private GameObject 	graphMarkersView;
	private GameObject	graphNodesView;
	private GameObject	graphEdgesView;
	private GameObject 	meshView;


	//instantiating stuff:
	public void InstantiatePlaneView() {
		planeView = new GameObject ("Bottom Plane");
		planeView.transform.parent = gameObject.transform;
	}

	public void InstantiateGraphView() {
		graphView = new GameObject ("Graph");
		graphView.transform.parent = gameObject.transform;
		graphView.transform.position = new Vector3 (0, 1, 0);

		graphMarkersView = new GameObject ("Markers");
		graphMarkersView.transform.parent = graphView.transform;

		graphNodesView = new GameObject ("Nodes");
		graphNodesView.transform.parent = graphView.transform;

		graphEdgesView = new GameObject ("Edges");
		graphEdgesView.transform.parent = graphView.transform;
	}

	public void InstantiateVolumeView() {
		volumeView = new GameObject ("Volume");
		volumeView.transform.parent = gameObject.transform;
	}

	public void InstantiateMeshView() {
		meshView = new GameObject("Mesh");
		meshView.transform.parent = gameObject.transform;
	}

	//______________________________________________________________________________
	//adding primitives:
	public void AddPlane(Texture texture, Vector3 scale){
		Renderer plane = Instantiate (planePrefab);
		plane.GetComponent <Renderer>().sharedMaterial.mainTexture = texture;
		scale.x *= Constants.PLANE_SIZE_CORRECTION_MULTIPLIER;
		scale.z *= Constants.PLANE_SIZE_CORRECTION_MULTIPLIER;
		plane.GetComponent <Renderer> ().transform.localScale = scale;
		plane.transform.parent = planeView.transform;
	}

	public void ReplacePlane(Texture texture, Vector3 scale) {
		DeleteChildAssets (planeView);
		AddPlane (texture, scale);
	}

	public void AddVolumeBlock(Vector3 addPosition, Vector3 scale) {
		Renderer volumeBlock = Instantiate (volumePrefab);
		volumeBlock.transform.localScale = scale;
		volumeBlock.transform.Translate (addPosition);
		volumeBlock.transform.parent = volumeView.transform;
	}

	public void ReplaceVolumeBlock(Vector3 addPosition, Vector3 scale) {
		DeleteChildAssets (volumeView);
		AddVolumeBlock (addPosition, scale);
	}


	public void AddMesh(Mesh mesh, Texture texture, Vector3 meshScale) {
		Renderer meshInstance = Instantiate (meshPrefab);
		meshInstance.transform.parent = meshView.transform;
		meshInstance.transform.localScale = meshScale;
		meshInstance.GetComponent <MeshFilter>().sharedMesh = mesh;
		meshInstance.GetComponent <MeshRenderer>().sharedMaterial.mainTexture = texture;
//		meshInstance.GetComponent <Transform>().Translate (new Vector3(0f, -meshScale.y/2f + 0.01f, 0f));
		meshInstance.GetComponent<Transform>().position = new Vector3(0, 0.001f, 0);
	}

	public void ReplaceMesh(Mesh mesh, Texture texture, Vector3 meshScale) {
		DeleteChildAssets (meshView);
		AddMesh (mesh, texture, meshScale);
	}


	//TODO: THIS SHOULD BE THE ENTIRE METHOD IN STATIC UTILS CLASS!
	public void AddGraphMarker(Vector3 markerPosition, Vector3 markerScale, bool centerObject) {
		if (centerObject) {
			Vector3 centeredMarkerPosition = markerPosition;
			Vector3 graphMarkerDimensions = graphMarkerPrefab.bounds.size;
			centeredMarkerPosition += graphMarkerDimensions;
			AddGraphMarker (centeredMarkerPosition, markerScale);
		} else {
			AddGraphMarker (markerPosition, markerScale);
		}
	}

	public Renderer AddGraphMarker(Vector3 markerPosition, Vector3 markerScale) {
		Renderer marker = Instantiate (graphMarkerPrefab);
		marker.transform.parent = graphMarkersView.transform;
		marker.transform.position = markerPosition;
		marker.transform.localScale = markerScale;
		return marker;
	}
		
	// TODO: refactor to AddGraphNodeBold / NodeLight etc (this script should not know about POIs and other logic stuff)
	// TODO: node radius
	private Renderer AddGraphNode(Vector3 nodePosition, Vector3 nodeScale) {
		Renderer poi = Instantiate (graphNodePrefab);
		poi.transform.parent = graphNodesView.transform;
		poi.transform.position = nodePosition;
		poi.transform.localScale = nodeScale;
		return poi;
	}

	//todo: figure out separation into light / bold nodes later
	public void AddGraphNode(Vector3 nodePosition, Vector3 nodeScale, bool centerObject) {
		if (centerObject) {
			Vector3 centeredNodePosition = nodePosition;
			Vector3 graphNodeDimensions = graphMarkerPrefab.bounds.size;
			centeredNodePosition += graphNodeDimensions;
			AddGraphNode (centeredNodePosition, nodeScale);
		} else {
			AddGraphNode (nodePosition, nodeScale);
		}
	}

	public void AddGraphEdge(Vector3 position, Vector3 lookat, float scaleY) {
		position += graphMarkerPrefab.bounds.size;
		Renderer edge = Instantiate (graphEdgePrefab);
		edge.transform.parent = graphEdgesView.transform;
		edge.transform.position = position;
		edge.transform.rotation = Quaternion.LookRotation (lookat, Vector3.up);
		edge.transform.Rotate (new Vector3(90,0,0));
		Vector3 scale = edge.transform.localScale;
		scale.y = scaleY/2f;
		edge.transform.localScale = scale;
	}


	//____________________________________________________________________________
	//clearing graph:
	public void ClearPlane() {
		GameObject.DestroyImmediate (planeView);
	}
		
	public void ClearGraph() {
		GameObject.DestroyImmediate (graphView); //todo: DestroyImmediate ONLY IN EDITOR!
//		InstantiateGraphView (); //temporary
	}

	public void ClearGraphMarkers() {
		DeleteChildAssets (graphMarkersView);
	}

	private void DeleteChildAssets(GameObject asset) {
		foreach (Transform child in asset.transform) {
			DestroyImmediate(child.gameObject);
		}
	}

	//____________________________________________________________________________
	//inner view getters:
	public GameObject GetGraphView() {
		return graphView;
	}

	public GameObject GetPlaneView() {
		return planeView;
	}

	public GameObject GetVolumeView() {
		return volumeView;
	}

	public GameObject GetMeshView() {
		return meshView;
	}

	public bool CheckGraphChildrenExistence() {
		if (graphEdgesView == null || graphMarkersView == null || graphNodesView == null) {
			return false;
		}
		return true;
	}

}
