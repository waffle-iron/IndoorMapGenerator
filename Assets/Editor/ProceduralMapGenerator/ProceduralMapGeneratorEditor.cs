using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (ProceduralMapGenerator))]
public class ProceduralMapGeneratorEditor : Editor {

	ProceduralMapGenerator generator;

	public override void OnInspectorGUI() {
		generator = (ProceduralMapGenerator)target;

		DrawDefaultInspector ();
//		if (DrawDefaultInspector ()) {
//			generator.GeneratePerlinNoiseValuesMap ();
//		}

		if (GUILayout.Button("perlin")) {
			generator.GeneratePerlinNoiseValuesMap ();
		}

		if (GUILayout.Button("test cross")) {
			generator.GenerateTestCrossValuesMap ();
		}

		if (GUILayout.Button("Gaussian Blur (perlin)")) {
			generator.GeneratePerlinNoiseValuesMap ();
			generator.ApplyGaussianBlur ();
		}

		if (GUILayout.Button("Gaussian Blur (Cross)")) {
			generator.GenerateTestCrossValuesMap ();
			generator.ApplyGaussianBlur ();
		}
	}

}
