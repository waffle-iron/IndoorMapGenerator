using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor (typeof (ProceduralMapGenerator))]
public class ProceduralMapGeneratorEditor : Editor {

	ProceduralMapGenerator generator;

	private const int buttonPerlin = 1;
	private const int buttonTestCross = 2;
	private const int buttonGaussianBlurPerlin = 3;
	private const int buttonGaussianBlurCross = 4;

	private int lastUsed = 1;
	private long lastUsedTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;

	//TODO: some buttons should be PRESSED DOWN on click, not TAPPED
	//eg: on clicking CONTRAST, more values should pop pup, to fine tune Contrast work

	public override void OnInspectorGUI() {
		generator = (ProceduralMapGenerator)target;

		if (DrawDefaultInspector ()) {
			if (lastUsedTime < DateTime.Now.Ticks / TimeSpan.TicksPerSecond + 1000) {
//				switch (lastUsed) {
//					case 1:
////						ButtonPerlin ();
//						break;
//					case 2:
//						ButtonTestCross ();
//						break;
//					case 3:
//						ButtonGaussianBlurPerlin ();
//						break;
//					case 4:
//						ButtonGaussianBlurCross ();
//						break;
//				}
			}
		}

		if (GUILayout.Button("perlin")) {
			ButtonPerlin ();
		}

		if (GUILayout.Button("test cross")) {
			ButtonTestCross ();
		}

		if (GUILayout.Button("Gaussian Blur (perlin)")) {
			ButtonGaussianBlurPerlin ();
		}

		if (GUILayout.Button("Gaussian Blur (Cross)")) {
			ButtonGaussianBlurCross ();
		}

		if (GUILayout.Button("Apply Contrast")) {
			ButtonApplyContrast ();
		}

		if (GUILayout.Button("graph markers")) {
			generator.GenerateGraphMarkers ();
		}

		if (GUILayout.Button("graph nodes")) {
			generator.GenerateGraphPOIs ();
		}

		if (GUILayout.Button("graph edges")) {
			generator.GenerateGraphEdges ();
		}

		if (GUILayout.Button("mapGraphToValues")) {
			generator.ConvertGraphToValues ();
		}
	}

	private void ButtonPerlin() {
		generator.GeneratePerlinNoiseValuesMap ();
		generator.GenerateVolumeBlock ();
		lastUsed = 1;
	}

	private void ButtonTestCross() {
		generator.GenerateTestCrossValuesMap ();
		lastUsed = 2;
	}

	private void ButtonGaussianBlurPerlin() {
//		generator.GeneratePerlinNoiseValuesMap ();
		generator.ApplyGaussianBlur ();
		lastUsed = 3;
	}

	private void ButtonGaussianBlurCross() {
//		generator.GenerateTestCrossValuesMap ();
		generator.ApplyGaussianBlur ();
		lastUsed = 4;
	}

	private void ButtonApplyContrast() {
//		generator.GeneratePerlinNoiseValuesMap ();
		generator.ApplyContrast ();
	}

}
