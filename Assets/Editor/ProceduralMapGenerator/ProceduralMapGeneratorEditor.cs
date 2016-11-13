using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (ProceduralMapGenerator))]
public class ProceduralMapGeneratorEditor : Editor {

	ProceduralMapGenerator generator;

	public override void OnInspectorGUI() {
		generator = (ProceduralMapGenerator)target;

		if (DrawDefaultInspector ()) {
			generator.GeneratePerlinNoiseValuesMap ();
		}

		if (GUILayout.Button("perlin")) {
			generator.GeneratePerlinNoiseValuesMap ();
		}
	}

}
