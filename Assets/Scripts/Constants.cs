using System;
using UnityEngine;
using UnityEditor;

public class Constants
{
	public static float PLANE_SIZE_CORRECTION_MULTIPLIER = 0.1f;
	public static Vector3 PLANE_SIZE_CORRECTION_MULTIPLIER_VECTOR3 = new Vector3(0.1f, 1f, 0.1f);

//	private readonly Color WATER_COLOR_LOW_DEEPBLUE;
//	private readonly Color WATER_COLOR_LOW_PURPLISH;

//	https://www.sessions.edu/color-calculator-results/?colors=7344ff,ffe944,ff8c44
//	https://www.sessions.edu/color-calculator-results/?colors=8bb0ff,eaff8b,ffa08b

//	https://www.sessions.edu/color-calculator-results/?colors=ffb97f,b27fff,7fffe8

//	https://www.sessions.edu/color-calculator-results/?colors=48703d,70453d,703d62

//	https://www.sessions.edu/color-calculator-results/?colors=a38d89,8993a3,8ea389

	public static Color WATER_COLOR_LOW = new Color (Mathf.InverseLerp (0, 255, 115), Mathf.InverseLerp (0, 255, 68), Mathf.InverseLerp (0, 255, 255));
	public static Color WATER_COLOR_HIGH = new Color (Mathf.InverseLerp (0, 255, 139), Mathf.InverseLerp (0, 255, 176), Mathf.InverseLerp (0, 255, 255));
	public static float WATER_RANGE_MAX_HEIGHT = 0.15f;

	public static Color SHORE_COLOR_LOW = new Color (Mathf.InverseLerp (0, 255, 255), Mathf.InverseLerp (0, 255, 245), Mathf.InverseLerp (0, 255, 150)); 
	public static Color SHORE_COLOR_HIGH = new Color (Mathf.InverseLerp (0, 255, 255), Mathf.InverseLerp (0, 255, 185), Mathf.InverseLerp (0, 255, 127)); 
	public static float SHORE_RANGE_MAX_HEIGHT = 0.30f;

	public static Color GRASS_COLOR_LOW = new Color (Mathf.InverseLerp (0, 255, 234), Mathf.InverseLerp (0, 255, 255), Mathf.InverseLerp (0, 255, 139));
	public static Color GRASS_COLOR_HIGH = new Color (Mathf.InverseLerp (0, 255, 137), Mathf.InverseLerp (0, 255, 207), Mathf.InverseLerp (0, 255, 94));
	public static float GRASS_RANGE_MAX_HEIGHT = 0.49f;

	public static Color BOREAL_COLOR_LOW = new Color (Mathf.InverseLerp (0, 255, 95), Mathf.InverseLerp (0, 255, 161), Mathf.InverseLerp (0, 255, 76));
	public static Color BOREAL_COLOR_HIGH = new Color (Mathf.InverseLerp (0, 255, 142), Mathf.InverseLerp (0, 255, 163), Mathf.InverseLerp (0, 255, 137));
	public static float BOREAL_RANGE_MAX_HEIGHT = 0.80f;

	public static Color MOUNTAINS_COLOR_LOW = new Color (Mathf.InverseLerp (0, 255, 163), Mathf.InverseLerp (0, 255, 141), Mathf.InverseLerp (0, 255, 137));
	public static Color MOUNTAINS_COLOR_HIGH = new Color (Mathf.InverseLerp (0, 255, 137), Mathf.InverseLerp (0, 255, 147), Mathf.InverseLerp (0, 255, 163));
	public static float MOUNTAINS_RANGE_MAX_HEIGHT = 0.90f;

	public static Color TOP_COLOR_LOW = new Color (Mathf.InverseLerp (0, 255, 250), Mathf.InverseLerp (0, 255, 255), Mathf.InverseLerp (0, 255, 245));
	public static Color TOP_COLOR_HIGH = Color.white;
	public static float TOP_RANGE_MAX_HEIGHT = 1.15f;



	public static TerrainType TERRAINTYPE_STANDARD_WATER = new TerrainType ("TERRAINTYPE_STANDARD_WATER", WATER_RANGE_MAX_HEIGHT, WATER_COLOR_LOW, WATER_COLOR_HIGH);
	public static TerrainType TERRAINTYPE_STANDARD_SHORE = new TerrainType ("TERRAINTYPE_STANDARD_SHORE", SHORE_RANGE_MAX_HEIGHT, SHORE_COLOR_LOW, SHORE_COLOR_HIGH);
	public static TerrainType TERRAINTYPE_STANDARD_GRASS = new TerrainType ("TERRAINTYPE_STANDARD_GRASS", GRASS_RANGE_MAX_HEIGHT, GRASS_COLOR_LOW, GRASS_COLOR_HIGH);
	public static TerrainType TERRAINTYPE_STANDARD_BOREAL = new TerrainType ("TERRAINTYPE_STANDARD_BOREAL", BOREAL_RANGE_MAX_HEIGHT, BOREAL_COLOR_LOW, BOREAL_COLOR_HIGH);
	public static TerrainType TERRAINTYPE_STANDARD_MOUNTAINS = new TerrainType ("TERRAINTYPE_STANDARD_MOUNTAINS", MOUNTAINS_RANGE_MAX_HEIGHT, MOUNTAINS_COLOR_LOW, MOUNTAINS_COLOR_HIGH);
	public static TerrainType TERRAINTYPE_STANDARD_TOP = new TerrainType ("TERRAINTYPE_STANDARD_TOP", TOP_RANGE_MAX_HEIGHT, TOP_COLOR_LOW, TOP_COLOR_HIGH);

	public enum TextureType {
		TEXTURE_DEBUG_HEIGHT,
		TEXTURE_MAP_STANDARD
	}

	public static TerrainSet TERRAINSET_STANDARD = new TerrainSet ("TERRAINSET_STANDARD", new TerrainType[] {
		TERRAINTYPE_STANDARD_WATER, 
		TERRAINTYPE_STANDARD_SHORE, 
		TERRAINTYPE_STANDARD_GRASS, 
		TERRAINTYPE_STANDARD_BOREAL, 
		TERRAINTYPE_STANDARD_MOUNTAINS, 
		TERRAINTYPE_STANDARD_TOP
	});
		

	public class TerrainSet {
		public String name;
		public TerrainType[] terrainTypes;

		public TerrainSet(String name, TerrainType[] terrainTypes) {
			Initialize(name, terrainTypes);
		}

		public void Initialize(String name, TerrainType[] terrainTypes) {
			this.name = name;
			this.terrainTypes = terrainTypes;
		}
	}
		

	//convert to JSON:
	public class TerrainType {
		public String 	name;
		public float 	rangeMaxHeight;
		public Color	rangeMinColour;
		public Color	rangeMaxColour;

		public TerrainType(String name, float rangeMaxHeight, Color rangeMinColour, Color rangeMaxColour) {
			Initialize(name, rangeMaxHeight, rangeMinColour, rangeMaxColour);
		}

		public void Initialize(String name, float rangeMaxHeight, Color rangeMinColour, Color rangeMaxColour) {
			this.name = name;
			this.rangeMaxHeight = rangeMaxHeight;
			this.rangeMinColour = rangeMinColour;
			this.rangeMaxColour = rangeMaxColour;
		}
	}

}

