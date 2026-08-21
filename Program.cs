using System.Threading.Tasks;
using Raylib_cs;
using SharpDX;

namespace SoftEngine
{
    public class Program
    {
        private static Device device;
        // private static Mesh mesh = new Mesh("Cube", 8, 12);
        private static Mesh[] meshes;
        private static Camera camera = new Camera();

        public static void Main()
        {
            // Choose the back buffer resolution
            int width = 640;
            int height = 480;

            // Create the Raylib window
            Raylib.InitWindow(width, height, "SoftEngine");

            // // Create the device
            device = new Device(width, height);

            meshes = device.LoadJSONFile("monkey.babylon");

            // // // Set up camera
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
            foreach (var mesh in meshes)
            {
                mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);

            }

            // Render the mesh
            device.Render(camera, meshes);

            // Display the back buffer
            Raylib.BeginDrawing();

            device.Present();

            Raylib.EndDrawing();
        }
     }
}

