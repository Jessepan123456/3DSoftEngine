using Raylib_cs;
using SharpDX;

namespace SoftEngine
{
    public class Device
    {
        private byte[] backBuffer;
        private int width;
        private int height;
        private Texture2D texture;

        public Device(int width, int height)
        {
            this.width = width;
            this.height = height;

            // Back buffer size is equal to the number of pixels to draw
            // on screen (width * height) * 4 ( Values )
            backBuffer = new byte[width * height * 4];

            // Create texture used to display our back buffer
            Image image = Raylib.GenImageColor(width, height, Raylib_cs.Color.Blank);
            texture = Raylib.LoadTextureFromImage(image);
            Raylib.UnloadImage(image);
        }

        // This method is called to clear the back buffer with a specific color
        public void Clear(byte r, byte g, byte b, byte a)
        {
            for (var index = 0; index < backBuffer.Length; index += 4)
            {
                backBuffer[index] = b;
                backBuffer[index + 1] = g;
                backBuffer[index + 2] = r;
                backBuffer[index + 3] = a;
            }
        }

        // Once everything is ready, we can flush the back buffer
        // into the front buffer.
        public void Present()
        {
            Raylib.UpdateTexture(texture, backBuffer);
            Raylib.DrawTexture(texture, 0, 0, Raylib_cs.Color.White);
        }

        // Called to put a pixel on screen at a specific X,Y coor
        public void PutPixel(int x, int y, Color4 color)
        {
            // As we have a 1-D Array for our back buffer
            // we need to know hte equivalent cell in 1-D based
            // on the 2D coor on screen
            var index = ( x+ y * width) * 4;

            backBuffer[index] = (byte)(color.Blue * 255);
            backBuffer[index + 1] = (byte)(color.Green * 255);
            backBuffer[index + 2] = (byte)(color.Red * 255);
            backBuffer[index + 3] = (byte)(color.Alpha * 255);
        }

        // Project takes some 3D coor and transform them
        // in 2D coor using the transformation matrix
        public Vector2 Project(Vector3 coord, Matrix transMat)
        {
            // transforming the coor
            var point = Vector3.TransformCoordinate(coord, transMat);

            // The transformed coor will be based on coor system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * width + width / 2.0f;
            var y = -point.Y * height + height / 2.0f;
            
            return (new Vector2(x, y));
        }

        // Draw Point calls PutPixel but does the clipping operation beofre
        public void DrawPoint(Vector2 point)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < width && point.Y < height)
            {
                // Drawing a yellow point
                PutPixel((int)point.X, (int)point.Y, new Color4(1.0f, 1.0f, 0.0f, 1.0f));
            }
        }

        public void DrawLine(Vector2 point0, Vector2 point1)
        {
            var dist = (point1 - point0).Length();

            // If the distance btween the 2 points is less than 2 pixels
            // We're exiting
            if (dist < 2) 
                return;

            // Find the middle point between frist & second point
            Vector2 middlePoint = point0 + (point1 - point0)/2;
            // We draw this point on screen 
            DrawPoint(middlePoint);
            // Recursive algorithm launched between first & middle point
            // and between middle & second point
            DrawLine(point0, middlePoint);
            DrawLine(middlePoint, point1);
        }

        public void DrawBline(Vector2 point0, Vector2 point1)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                DrawPoint(new Vector2(x0, y0));

                // If it hits the other point
                if ((x0 == x1) && (y0 == y1)) break;

                // If it has a err, how should it move
                var e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx ) { err += dx; y0 += sy; }
            }
        }

        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render(Camera camera, params Mesh[] meshes)
        {
            // To understand this part, please read the prerequisites resources
            // What the camera located and what it looking at
            var viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, Vector3.UnitY);
            // perspective
            var projectionMatrix = Matrix.PerspectiveFovRH(0.78f, (float)width / height, 0.01f, 1.0f);

            foreach(Mesh mesh in meshes)
            {
                // Beware to apply rotation before translation
                // Where is the object and how is it rotated
                var worldMatrix = Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) * Matrix.Translation(mesh.Position);
                    
                // Combine world, view, projection
                var transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                foreach (var face in mesh.Faces)
                {
                    var vertexA = mesh.Vertices[face.A];
                    var vertexB = mesh.Vertices[face.B];
                    var vertexC = mesh.Vertices[face.C];

                    // First, we project the 3D coor into the 2D space
                    var pixelA = Project(vertexA, transformMatrix);
                    var pixelB = Project(vertexB, transformMatrix);
                    var pixelC = Project(vertexC, transformMatrix);

                    // Then we can draw on screen
                    DrawBline(pixelA, pixelB);
                    DrawBline(pixelB, pixelC);
                    DrawBline(pixelC, pixelA);
                }
            }
        }
    }
}