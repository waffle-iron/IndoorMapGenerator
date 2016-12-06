using System;
using UnityEngine;

public class GFXUtils {
	
	//todo: refactor this
	//todo; fix PerlinRenderer (similar method with 2 fors)
	public MeshWrapper GenerateMesh(float[,] heightMap) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);
		int verticeCount = 0;

		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		MeshWrapper mesh = new MeshWrapper ();
		mesh.InitializeMeshWrapper (width, height);


		for (int y = 0; y < height; ++y) {
			for (int x = 0; x < width; ++x) {
				//todo: implement SMALL fluctuations in positions here (to make mesh less rectangly)
//				mesh.AddVertex (verticeCount, topLeftX + x, heightMap[x, y], topLeftZ -y);
//				mesh.AddVertex (verticeCount, x, heightMap[x, y], topLeftZ -y);

//				mesh.AddVertex (verticeCount, topLeftX + x, heightMap[x, y], y);

				mesh.AddVertex (verticeCount, topLeftX + x, heightMap[width -1 - x, height-1 - y], -topLeftZ + y);

				mesh.AddUV (verticeCount, x / (float)width, y / (float)height);

				if (x < width - 1 && 0 < x
					&& y < height - 1 && 0 < y) {
//					mesh.AddTriangle (verticeCount, verticeCount + width + 1, verticeCount + width);
//					mesh.AddTriangle (verticeCount + width + 1, verticeCount, verticeCount + 1);
					mesh.AddTriangle (verticeCount + width, verticeCount + width + 1, verticeCount);
					mesh.AddTriangle (verticeCount + 1, verticeCount, verticeCount + width + 1);

				}

				++verticeCount;
			}
		}
			
		return mesh;
	}

}

