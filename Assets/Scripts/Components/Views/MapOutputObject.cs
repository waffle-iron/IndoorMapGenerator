using UnityEngine;
using System.Collections;

public class MapOutputObject : MonoBehaviour {

	public Renderer 	planePrefab;//for now, later it should be private and should be modified by some renderer (like PNoiseRenderer)
	public Renderer		graphMarkerPrefab;
	public Renderer		graphNodePrefab;

	private Renderer 	planeView;

	private GameObject	graphView;
	private GameObject 	graphMarkersView;
	private GameObject	graphNodesView;
	private GameObject	graphEdgesView;

	public void InstantiatePlane() {
		planeView = (Renderer) Instantiate (
			planePrefab, 
			gameObject.GetComponent <Transform> ()
		);
	}

	public void InstantiateGraph() {
		graphView = new GameObject ("Graph");
		graphView.transform.parent = gameObject.transform;
		graphView.transform.position = new Vector3 (0, 1, 0);

		graphMarkersView = new GameObject ("Markers");
		graphMarkersView.transform.parent = graphView.transform;

		graphNodesView = new GameObject ("Nodes");
		graphNodesView.transform.parent = graphView.transform;
	}


	public void ClearGraph() {
		GameObject.DestroyImmediate (graphView); //todo: DestroyImmediate ONLY IN EDITOR!
		InstantiateGraph (); //temporary
	}

	public Renderer AddGraphMarker(Vector3 markerPosition) {
		Renderer marker = Instantiate (graphMarkerPrefab);
		marker.transform.parent = graphMarkersView.transform;
		marker.transform.position = markerPosition;
		marker.transform.localScale = new Vector3 (5f, 5f, 5f); //todo: computUtils.VectorDivide(scale, 2)!
		return marker;
	}

	//TODO: THIS SHOULD BE THE ENTIRE METHOD IN STATIC UTILS CLASS!
	public void AddGraphMarker(Vector3 markerPosition, bool centerObject) {
		if (centerObject) {
			Vector3 centeredMarkerPosition = markerPosition;
			Vector3 graphMarkerDimensions = graphMarkerPrefab.bounds.size;
//			centeredMarkerPosition.x += graphMarkerDimensions.x; //TODO: SUBTRACT FROM VECTORS METHOD IN STATIC UTILS CLASS!
//			centeredMarkerPosition.y += graphMarkerDimensions.y;
//			centeredMarkerPosition.z += graphMarkerDimensions.z;
			centeredMarkerPosition += graphMarkerDimensions;
			AddGraphMarker (centeredMarkerPosition);
		} else {
			AddGraphMarker (markerPosition);
		}
	}
		
	//	TODO: refactor to AddGraphNodeBold / NodeLight etc (this script should not know about POIs and other logic stuff)
	// TODO: node radius
	private Renderer AddGraphNode(Vector3 nodePosition) {
		Renderer poi = Instantiate (graphNodePrefab);
		poi.transform.parent = graphNodesView.transform;
		poi.transform.position = nodePosition;
		poi.transform.localScale = new Vector3 (10f, 10f, 10f);
		return poi;
	}

	//todo: figure out separation into light / bold nodes later
	public void AddGraphNode(Vector3 nodePosition, bool centerObject) {
		if (centerObject) {
			Vector3 centeredNodePosition = nodePosition;
			Vector3 graphNodeDimensions = graphMarkerPrefab.bounds.size;
			centeredNodePosition += graphNodeDimensions;
			AddGraphNode (centeredNodePosition);
		} else {
			AddGraphNode (nodePosition);
		}
//		if (boldNode) {
			
//		}
//		else {
//			AddGraphNode (nodePosition, graphNodePrefab);
//		}
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
