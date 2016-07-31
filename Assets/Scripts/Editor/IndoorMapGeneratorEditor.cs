using UnityEngine;
using System.Collections;
using System.Linq;
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


		if (GUILayout.Button("connect key POIs"))
		{
			script.ConnectKeyPois();
		}

		if (GUILayout.Button("circletest"))
		{
			int min = 0; int max = 10;
			Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 0; max = 20;
			Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 0; max = 4;
			Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 0; max = 5;
            Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 5; max = 10;
            Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 0; max = 0;
           	Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 0; max = 1;
            Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 0; max = 1;
			Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 0; max = 1;
            Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
			min = 1; max = 1;
			Debug.LogError("rand("+min+","+max+")=>" + Utils.RandomRangeMiddleVal(min, max));
		}


		EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);
		EditorGUILayout.LabelField("details:", center);

		Vector2 vec = Utils.GetGridUnitSize2D();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("total POIs:"), right);
		GUILayout.Label(new GUIContent(script.GetTotalPOIs().ToString()), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("key POI count:"), right);
		GUILayout.Label(new GUIContent(	"~" + script.GetTotalPOIsKey() +
										" (" + (script.GetTotalPOIsKey()- Mathf.FloorToInt(script.GetTotalPOIsKey() * (script.keyPoiRndOffset/100f))) +
		                               	"-" + (script.GetTotalPOIsKey() + Mathf.FloorToInt(script.GetTotalPOIsKey() * (script.keyPoiRndOffset/100f))) + ")"
										), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("nonkey POI count:"), right);
		GUILayout.Label(new GUIContent(	"~" + script.GetTotalPOIsNonKey()), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("key POI size:"), right);
		GUILayout.Label(new GUIContent(	"~" + script.GetKeyPoiSizeVal() +
										" (" + (script.GetKeyPoiSizeVal() - Mathf.FloorToInt(script.GetKeyPoiSizeVal() * (script.keyPoiSizeRndOffset/100f))) +
		                               	"-"  + (script.GetKeyPoiSizeVal() + Mathf.FloorToInt(script.GetKeyPoiSizeVal() * (script.keyPoiSizeRndOffset/100f))) + ")"
										), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("non key POI size (base):"), right);
		GUILayout.Label(new GUIContent(script.GetNonKeyPoiSizeVal().ToString()), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("grid real dimensions:"), right);
		GUILayout.Label(new GUIContent("(" + vec.x + ", " + vec.y + ")"), left);
		EditorGUILayout.EndHorizontal();

		vec = Utils.GetGridRealSize2D();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("grid real dimensions:"), right);
		GUILayout.Label(new GUIContent("(" + vec.x + ", " + vec.y + ")"), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("total regions:"), right);
		GUILayout.Label(new GUIContent(Utils.GetTotalRegions().ToString()), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("total cells:"), right);
		GUILayout.Label(new GUIContent(Utils.GetTotalCells().ToString()), left);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent("min POI radius:"), right);
		GUILayout.Label(new GUIContent("~" + Utils.GetMinPOIRadius().ToString() + "%"), left);
		EditorGUILayout.EndHorizontal();

  	}


}
