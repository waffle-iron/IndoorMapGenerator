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

		if (GUI.changed)
		{
			script.OnInputUpdate();
		}

		if (GUILayout.Button("Create Floor Plane"))
		{
//			script.ClearObjects();
			script.CreateFloorPlane();
//			script.CreateCellularAutomataBoxes();
		}

  	}


}
