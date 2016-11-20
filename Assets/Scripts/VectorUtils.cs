using System.Collections;
using UnityEngine;

public class VectorUtils {

	public UnityEngine.Vector3 VectorAdd(UnityEngine.Vector3 input, float value) {
		input.x += value;
		input.y += value;
		input.z += value;
		return input;
	}

	public UnityEngine.Vector3 VectorSubtract(UnityEngine.Vector3 input, float subtrahend, bool omitYVal = false) {
		input.x -= subtrahend;
		input.z -= subtrahend;
		if (!omitYVal) {
			input.y -= subtrahend;
		}
		return input;
	}

	public UnityEngine.Vector3 VectorDivide(UnityEngine.Vector3 input, float divident) {
		input.x /= divident;
		input.y /= divident;
		input.z /= divident;
		return input;
	}
	
}
