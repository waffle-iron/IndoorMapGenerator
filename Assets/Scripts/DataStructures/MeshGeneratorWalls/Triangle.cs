using System.Collections;

/**
 * 	Representation of a single geometric triangle in a mesh.
 * 
 * 	Needed during process of determining which of the vertices in a mesh 
 * 	are a part of it's outline edge.
 */ 
public class Triangle {
	public int triangleVertexA;
	public int triangleVertexB;
	public int triangleVertexC;

	public Triangle() {
		
	}

	public Triangle(int vertexA, int vertexB, int vertexC) {
		triangleVertexA = vertexA;
		triangleVertexB = vertexB;
		triangleVertexC = vertexC;
	}

	public int this[int i] {
		get {
			if (i==0){
				return triangleVertexA;
			}
			if (i==1){
				return triangleVertexB;
			}
			if (i==2){
				return triangleVertexC;
			}
			return Utils.INTEGER_INVALID_VALUE;
		}
	}
}
