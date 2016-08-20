using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class GridCellScript : MonoBehaviour {

	public bool traversable = false;

	public Vector2 cellUnitCoordinates;

	private Color colourTraversableTrue;
	private Color colourTraversableFalse;

	// Use this for initialization
	void Start () {
		colourTraversableTrue = new Color (1, 1, 1, 0.4f);
		colourTraversableFalse = new Color (0.05f, 0.05f, 0.05f, 0.4f);
		cellUnitCoordinates = Utils.VECTOR2_INVALID_VALUE;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void ChangeTraversableColour() {
		if (traversable) {
			this.GetComponent<Renderer> ().sharedMaterial.color = colourTraversableTrue;
		} else {
			this.GetComponent<Renderer> ().sharedMaterial.color = colourTraversableFalse;
		}
	}


}
