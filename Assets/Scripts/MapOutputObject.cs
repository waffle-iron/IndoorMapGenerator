using UnityEngine;
using System.Collections;

public class MapOutputObject : MonoBehaviour {

	public Renderer 	planePrefab;//for now, later it should be private and should be modified by some renderer (like PNoiseRenderer)
	public Renderer		graphMarkerPrefab;

	private Renderer 	planeView;
	private GameObject	graphView;
	private GameObject 	graphMarkersView;

	public void InstantiatePlane() {
		planeView = (Renderer) Instantiate (
			planePrefab, 
			gameObject.GetComponent <Transform> ()
		);
	}

	public void InstantiateGraph() {
		graphView = new GameObject ();
		graphView.transform.parent = gameObject.transform;
		graphView.transform.position = new Vector3 (0, 1, 0);
		graphView.name = "Graph View";

		graphMarkersView = new GameObject ();
		graphMarkersView.transform.parent = graphView.transform;
		graphMarkersView.name ="Markers";

	}


	public void ClearGraph() {
		GameObject.DestroyImmediate (graphView); //todo: DestroyImmediate ONLY IN EDITOR!
		InstantiateGraph (); //temporary
	}

	public Renderer AddGraphMarker(Vector3 markerPosition) {
		Renderer marker = Instantiate (graphMarkerPrefab);
		marker.transform.parent = graphMarkersView.transform;
		marker.transform.position = markerPosition;
		marker.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f); //todo: computUtils.VectorDivide(scale, 2)!
		return marker;
	}

	//TODO: THIS SHOULD BE THE ENTIRE METHOD IN STATIC UTILS CLASS!
	public void AddGraphMarker(Vector3 markerPosition, bool centerObject) {
		Vector3 centeredMarkerPosition = markerPosition;
		Vector3 graphMarkerDimensions = graphMarkerPrefab.bounds.size;
		centeredMarkerPosition.x += graphMarkerDimensions.x; //TODO: SUBTRACT FROM VECTORS METHOD IN STATIC UTILS CLASS!
		centeredMarkerPosition.y += graphMarkerDimensions.y;
		centeredMarkerPosition.z += graphMarkerDimensions.z;
		AddGraphMarker (centeredMarkerPosition);
	}

	public void AddGraphNode() {
		
	}

	public void AddGraphEdge() {
		
	}



	public GameObject GetGraphView() {
		return graphView;
	}

	public Renderer GetPlaneView() {
		return planeView;
	}

}
