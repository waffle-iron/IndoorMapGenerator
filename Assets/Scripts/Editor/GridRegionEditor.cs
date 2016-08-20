using System.Runtime.Serialization.Formatters;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (GridRegionScript))]
public class GridRegionEditor : Editor
{

	private GridRegionScript script;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (script == null)
		{
			script = (GridRegionScript) target;
		}

		EditorGUILayout.LabelField(
				new GUIContent(
					"Unit Coords: "
				),
				new GUIContent(
					script.GetRegionUnitCoords().ToString()
				)
		);

		EditorGUILayout.LabelField(
				new GUIContent(
					"Region type: "
				),
				new GUIContent(
				script.IsRegionTraversable() ? "On" : "Off"
				));

	}

}
