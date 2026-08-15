using SharpDX;

namespace SoftEngine
{
    public class Device
    {
        private byte[] backBuffer;
        private int width;
        private int height;

        public Device(int width, int height)
        {
            this.width = width;
            this.height = height;

            // Back buffer size is equal to the number of pixels to draw
            // on screen (width * height) * 4 ( Values )
            backBuffer = new byte[width * height * 4];
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

        // Once everything is ready, we can fluush the back buffer
        // into the front buffer.
        public void Present()
        {

        }

        // Called to put a pixel on screen at a specific X,Y coor
        public void PutPixel(int x, int y, Color4 color)
        {
            
        }

        // Project takes some 3D coor and transform them
        // in 2D coor using the transformation matrix
        public Vector2 Project(Vector3 coord, Matrix transMat)
        {
            var x;
            var y;
            
            return (new Vector2(x, y));
        }

        // Draw Point calls PutPixel but does the clipping operation beofre
        public void DrawwPoint(Vector2 point)
        {
            
        }

        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render(Camera camera, params Mesh[] meshes)
        {
            
        }
    }
}