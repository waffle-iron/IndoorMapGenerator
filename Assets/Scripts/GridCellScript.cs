using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class GridCellScript : MonoBehaviour {

	public bool traversable = false;

	public Vector2 cellUnitCoordinates;

	public Material material;
	public Material materialOff;

//	private Color colourTraversableTrue;
//	private Color colourTraversableFalse;

	// Use this for initialization
	void Start () {
//		colourTraversableTrue = new Color (1, 1, 1, 0.4f);
//		colourTraversableFalse = new Color (0.05f, 0.05f, 0.05f, 0.4f);
		cellUnitCoordinates = Utils.VECTOR2_INVALID_VALUE;
		material = materialOff;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void InvertTraversability() {
		Material actualMat = new Material (this.GetComponent<Renderer> ().sharedMaterial);

		if(traversable) {
			actualMat = material;
		} else {
			actualMat = materialOff;
		}

		this.GetComponent<Renderer> ().sharedMaterial = actualMat;
	}


}
