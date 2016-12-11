using System;
using UnityEngine;

public class GFXUtils {
	
	//todo: refactor this
	//todo; fix PerlinRenderer (similar method with 2 fors)
	//todo: random fluctuation percent (USE MATH METHOD - RANDOM MIDDLE RANGE!)
	public MeshWrapper GenerateMesh(float[,] heightMap, int magnitudeDividerX = 1, int magnitudeDividerZ = 1) {
		int dimensionX = heightMap.GetLength (0);
		int dimensionZ = heightMap.GetLength (1);
		int verticeCount = 0;

		float topLeftX = (dimensionX - 1) / -2f;
		float topLeftZ = (dimensionZ - 1) / 2f;

		MeshWrapper mesh = new MeshWrapper ();
		mesh.InitializeMeshWrapper (dimensionX, dimensionZ);


		for (int z = 0; z < dimensionZ; z+=magnitudeDividerZ) {
			for (int x = 0; x < dimensionX; x+=magnitudeDividerX) {
			
				//todo: implement SMALL fluctuations in positions here (to make mesh less rectangly)
//				mesh.AddVertex (verticeCount, topLeftX + x, heightMap[x, y], topLeftZ -y);
//				mesh.AddVertex (verticeCount, x, heightMap[x, y], topLeftZ -y);

//				mesh.AddVertex (verticeCount, topLeftX + x, heightMap[x, y], y);
//				float tempX = topLeftX + x + UnityEngine.Random.Range (0, 1f);
//				float tempZ = -topLeftZ + z + UnityEngine.Random.Range (0, 1f);
				float tempX = topLeftX + x;
				float tempZ = -topLeftZ + z;

				mesh.AddVertex (verticeCount, tempX, heightMap[dimensionX -1 - x, dimensionZ-1 - z], tempZ);

				mesh.AddUV (verticeCount, x / (float)dimensionX, z / (float)dimensionZ);



				if (x < dimensionX - magnitudeDividerX && magnitudeDividerX <= x
					&& z < dimensionZ - magnitudeDividerX  && magnitudeDividerZ <= z) {

//					mesh.AddTriangle (verticeCount, verticeCount + width + 1, verticeCount + width);
//					mesh.AddTriangle (verticeCount + width + 1, verticeCount, verticeCount + 1);
					mesh.AddTriangle (verticeCount + dimensionX, verticeCount + dimensionX + 1, verticeCount);
					mesh.AddTriangle (verticeCount + 1, verticeCount, verticeCount + dimensionX + 1);

				}
				++verticeCount;

			}
		}
			
		return mesh;
	}

}

