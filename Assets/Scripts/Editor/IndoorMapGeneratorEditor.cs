using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IndoorMapGeneratorScript))]
public class IndoorMapGeneratorEditor : Editor
{
	private IndoorMapGeneratorScript script;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (script == null)
		{
			script = (IndoorMapGeneratorScript) target;
		}

		script.Update();

//		if (GUI.changed)
//		{
			script.OnInputUpdate();
//		}


		EditorGUILayout.LabelField(
			new GUIContent(
				"gridX: " + script.gridSizeX +
				", gridZ: " + script.gridSizeZ +
				", density:" + script.regionDensity
			)
		);

		EditorGUILayout.LabelField(
			new GUIContent(
				"cells total: " +
				(script.gridSizeX * script.gridSizeZ * script.regionDensity).ToString()
			)
		);

		if (GUILayout.Button("Create Floor Plane"))
		{
//			script.ClearObjects();
			script.CreateFloorPlane();
			script.CreateRegions();
//			script.CreateGridRegions();
//			script.CreateCells();
//			script.CreateCellularAutomataBoxes();
//			script.CreateCellularAutomataVertices();
		}

		if (GUILayout.Button("POIs"))
		{
			script.CreatePointsOfInterest(script.pointsOfInterest);
		}

  	}


}
