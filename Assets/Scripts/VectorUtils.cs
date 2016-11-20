using System.Collections;
using UnityEngine;

public class VectorUtils {

	public UnityEngine.Vector3 VectorDivide(UnityEngine.Vector3 input, float subtrahend) {
		input.x = input.x / subtrahend;
		input.y = input.y / subtrahend;
		input.z = input.z / subtrahend;
		return input;
	}
	
}
