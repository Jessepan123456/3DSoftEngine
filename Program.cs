using Raylib_cs;
using SharpDX;

namespace SoftEngine
{
    public class Program
    {
        private static Device device;
        private static Mesh mesh = new Mesh("Cube", 8, 12);
        private static Camera camera = new Camera();

        public static void Main()
        {
            // Choose the back buffer resolution
            int width = 640;
            int height = 480;

            // Create the Raylib window
            Raylib.InitWindow(width, height, "SoftEngine");

            // Create the device
            device = new Device(width, height);

            // Create the cube vertices
            mesh.Vertices[0] = new Vector3(-1, 1, 1);
            mesh.Vertices[1] = new Vector3(1, 1, 1);
            mesh.Vertices[2] = new Vector3(-1, -1, 1);
            mesh.Vertices[3] = new Vector3(1, -1, 1);
            mesh.Vertices[4] = new Vector3(-1, 1, -1);
            mesh.Vertices[5] = new Vector3(1, 1, -1);
            mesh.Vertices[6] = new Vector3(1, -1, -1);
            mesh.Vertices[7] = new Vector3(-1, -1, -1);

            // Mesh faces
            mesh.Faces[0] = new Face { A = 0, B = 1, C = 2 };
            mesh.Faces[1] = new Face { A = 1, B = 2, C = 3 };
            mesh.Faces[2] = new Face { A = 1, B = 3, C = 6 };
            mesh.Faces[3] = new Face { A = 1, B = 5, C = 6 };
            mesh.Faces[4] = new Face { A = 0, B = 1, C = 4 };
            mesh.Faces[5] = new Face { A = 1, B = 4, C = 5 };

            mesh.Faces[6] = new Face { A = 2, B = 3, C = 7 };
            mesh.Faces[7] = new Face { A = 3, B = 6, C = 7 };
            mesh.Faces[8] = new Face { A = 0, B = 2, C = 7 };
            mesh.Faces[9] = new Face { A = 0, B = 4, C = 7 };
            mesh.Faces[10] = new Face { A = 4, B = 5, C = 6 };
            mesh.Faces[11] = new Face { A = 4, B = 6, C = 7 };

            // Set up camera
            camera.Position = new Vector3(0, 0, 10.0f);
            camera.Target = Vector3.Zero;

            // Rendering loop
            while (!Raylib.WindowShouldClose())
            {
                Render();
            }

            Raylib.CloseWindow();
        }

        // Rendering loop handler
        private static void Render()
        {
            // Clear to black
            device.Clear(0, 0, 0, 255);

            // Rotate the cube slightly
            mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);

            // Render the mesh
            device.Render(camera, mesh);

            // Display the back buffer
            Raylib.BeginDrawing();

            device.Present();

            Raylib.EndDrawing();
        }
    }
}

