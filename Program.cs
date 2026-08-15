using SoftEngine;
using SharpDX;

Mesh cube = new Mesh("Cube", 8);

cube.Vertices[0] = new Vector3(-1, 1, 1);
cube.Vertices[1] = new Vector3(1, 1, 1);
cube.Vertices[2] = new Vector3(-1, -1, 1);
cube.Vertices[3] = new Vector3(-1, -1, -1);
cube.Vertices[4] = new Vector3(-1, 1, -1);
cube.Vertices[5] = new Vector3(1, 1, -1);
cube.Vertices[6] = new Vector3(1, -1, 1);
cube.Vertices[7] = new Vector3(1, -1, -1);