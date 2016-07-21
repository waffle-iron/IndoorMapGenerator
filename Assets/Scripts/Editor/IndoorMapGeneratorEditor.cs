using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(IndoorMapGeneratorScript))]
public class IndoorMapGeneratorEditor : Editor
{
	private IndoorMapGeneratorScript script;


	public override void OnInspectorGUI()
	{
		//'outsource' it to another class
		GUIStyle label = new GUIStyle(GUI.skin.label);

		GUIStyle bold = new GUIStyle(GUI.skin.label);
		bold.fontStyle = FontStyle.Bold;

		GUIStyle left = new GUIStyle(GUI.skin.label);
		left.alignment = TextAnchor.MiddleLeft;
		left.fontStyle = FontStyle.Bold;

		GUIStyle right = new GUIStyle(GUI.skin.label);
		right.alignment = TextAnchor.MiddleRight;


		GUIStyle center = new GUIStyle(GUI.skin.label);
        center.alignment = TextAnchor.UpperCenter;
        center.fontStyle = FontStyle.Italic;

		EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
		EditorGUILayout.LabelField("variables:", center);

		base.OnInspectorGUI();

		if (script == null)
		{
			script = (IndoorMapGeneratorScript) target;
		}

		script.Update();
		script.OnInputUpdate();


		EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);

		if (GUILayout.Button("Create Floor Plane"))
		{
			script.CreateFloorPlane();
			script.CreateRegions();
		}

		if (GUILayout.Button("POIs"))
		{
			script.CreatePointsOfInterest();
		}

		if (GUILayout.Button("entry/exit nodes"))
		{
			script.CreateEntryPoint();
			script.CreateEndPoint();
		}

		if (GUILayout.Button("bresenham"))
		{
			script.CreatePathEntryEnd();
		}


		EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
		EditorGUILayout.LabelField("details:", center);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("grid unit dimensions:"), right);
		GUILayout.Label(new GUIContent("(" + script.gridSizeX + ", " + script.gridSizeZ + ")"), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("grid real dimensions:"), right);
		GUILayout.Label(new GUIContent("(" + Utils.GetGridRealSize2D().x + ", " + Utils.GetGridRealSize2D().y + ")"), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("cells total:"), right);
		GUILayout.Label(new GUIContent((script.gridSizeX * script.gridSizeZ * script.regionDensity).ToString()), left);
		EditorGUILayout.EndHorizontal();

  	}


}
