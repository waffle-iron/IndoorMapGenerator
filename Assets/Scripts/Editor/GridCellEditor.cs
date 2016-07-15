using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GridCell))]
public class GridCellEditor : Editor {

	public override void OnInspectorGUI()
	{
//		[HeaderAttribute ("asd")]
		EditorGUILayout.LabelField(
			new GUIContent("Grid Position X: "),
			new GUIContent(((GridCell)target).GetGridLocationX().ToString())
		);

		EditorGUILayout.LabelField(
			new GUIContent("Grid Position Z: "),
			new GUIContent(((GridCell)target).GetGridLocationZ().ToString())
		);


	}

}
