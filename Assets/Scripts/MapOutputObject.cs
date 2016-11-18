using UnityEngine;
using System.Collections;

public class MapOutputObject : MonoBehaviour {

	public Renderer planePrefab;//for now, later it should be private and should be modified by some renderer (like PNoiseRenderer)

	private Renderer planeObject;

	public void InstantiatePlanePrefab() {
		planeObject = (Renderer) Instantiate (
			planePrefab, 
			gameObject.GetComponent <Transform> ()
		);
	}

	public Renderer GetPlaneObject() {
		if (planeObject == null) {
			InstantiatePlanePrefab ();
		}
		return planeObject;
	}

}
