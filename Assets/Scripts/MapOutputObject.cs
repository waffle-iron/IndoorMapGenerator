using UnityEngine;
using System.Collections;

public class MapOutputObject : MonoBehaviour {

	public Renderer 	planePrefab;//for now, later it should be private and should be modified by some renderer (like PNoiseRenderer)

	private Renderer 	planeView;
	private GameObject	graphView;

	public void InstantiatePlane() {
		planeView = (Renderer) Instantiate (
			planePrefab, 
			gameObject.GetComponent <Transform> ()
		);
	}

	public void InstantiateGraph() {
		graphView = new GameObject ();

	}

	public void ClearGraph() {
		
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
