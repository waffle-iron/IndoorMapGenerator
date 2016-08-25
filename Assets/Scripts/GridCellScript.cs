using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;

public class GridCellScript : MonoBehaviour, ICloneable {

	public bool traversable = false;

	public Vector2 cellUnitCoordinates;

	public Material material;
	public Material materialOff;

	public GridCellScript(bool traversable, Vector2 cellUnitCoordinates) {
		this.traversable = traversable;
		this.cellUnitCoordinates = cellUnitCoordinates;
	}

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


	public void ColourTraversability() {
		Material actualMat = new Material (this.GetComponent<Renderer> ().sharedMaterial);

		if(traversable) {
			actualMat = material;
		} else {
			actualMat = materialOff;
		}

		this.GetComponent<Renderer> ().sharedMaterial = actualMat;
	}

	public GridCellScript InvertTraversability() {
		this.traversable = !traversable;
		return this;
	}

	public object Clone() {
//		return new GridCellScript(traversable, cellUnitCoordinates);
		return this.MemberwiseClone();
	}
}
