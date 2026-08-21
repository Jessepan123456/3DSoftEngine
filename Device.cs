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

        // Clamping vlaues to keep them between 0 and 1
        float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        // Interpolating the value between 2 vertices
        // min is the starting point, max the ending point
        // and gradient the % between 2 points
        float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render(Camera camera, params Mesh[] meshes)
        {
            // To understand this part, please read the prerequisites resources
            // What the camera located and what it looking at
            var viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, Vector3.UnitY);
            // perspective
            var projectionMatrix = Matrix.PerspectiveFovLH(0.78f, (float)width / height, 0.01f, 1.0f);

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

        // Loading the JSON file in an asynchronous manner
        public Mesh[] LoadJSONFile(string fileName)
        {
           var meshes = new List<Mesh>();

           var data = File.ReadAllText(fileName);

           dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(data); 

           for ( var meshIndex = 0; meshIndex < jsonObject.meshes.Count; meshIndex++)
            {
                var verticesArray = jsonObject.meshes[meshIndex].positions;

                // Faces
                var indicesArray = jsonObject.meshes[meshIndex].indices;

                // Tells you which location on the texture should be mapped to that vertex
                // var uvCount = jsonObject.meshes[meshIndex].uvCount.Value ?? 0;
                var verticesStep = 3;

                // Depending of the number of texture's coor per vertex
                // we're jumping in hte vertices array by 6, 8 & 10 windows frame
                // switch ((int)uvCount)
                // {
                //     case 0:
                //         verticesStep = 6;
                //         break;
                //     case 1:
                //         verticesStep = 8;
                //         break;
                //     case 2:
                //         verticesStep = 10;
                //         break;
                // }

                // the number of interesting vertices info for us
                var verticesCount = verticesArray.Count / verticesStep;
                // number of faces is logically the size of the array divided by 3 (A, B, C)
                var facesCount = indicesArray.Count / 3;
                var mesh = new Mesh(jsonObject.meshes[meshIndex].name.Value, verticesCount, facesCount);

                // Filling the Vertices array of our mesh first
                for (var index = 0; index < verticesCount; index++)
                {
                    var x = (float)verticesArray[index * verticesStep].Value;
                    var y = (float)verticesArray[index * verticesStep + 1].Value;
                    var z = (float)verticesArray[index * verticesStep + 2].Value;
                    mesh.Vertices[index] = new Vector3(x, y, z);
                }

                // Then filling the Faces array
                for (var index = 0; index < facesCount; index++)
                {
                    var a = (int)indicesArray[index * 3].Value;
                    var b = (int)indicesArray[index * 3 + 1].Value;
                    var c = (int)indicesArray[index * 3 + 2].Value;
                    mesh.Faces[index] = new Face { A = a, B = b, C = c };
                }

                // Getting the position you've set in Blender
                var position = jsonObject.meshes[meshIndex].position;
                mesh.Position = new Vector3((float)position[0].Value, (float)position[1].Value, (float)position[2].Value);
                meshes.Add(mesh);
            }
            return meshes.ToArray();
        }
    }
}