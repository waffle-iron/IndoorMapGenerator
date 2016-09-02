using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;

public abstract class MeshGeneratorAbstractComponent : MonoBehaviour {

	protected GameObject 	targetObject;
	protected List<Vector3> vertices;
	protected List<int> 	triangles;


	public void SetTarget(GameObject target) {
		targetObject = target;
	}

	public abstract void GenerateMesh ();

	public abstract void OptimiseMesh ();

	protected abstract void AssignMeshToTarget();
}
