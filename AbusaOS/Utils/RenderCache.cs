using Cosmos.System.Graphics;

namespace AbusaOS.Utils
{
    internal static class RenderCache
    {
        public static Bitmap Capture(VBECanvas canvas, int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return null;
            }

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;

            if (x < 0 || y < 0 ||
                x + width > screenWidth ||
                y + height > screenHeight)
            {
                return null;
            }

            Bitmap bitmap = new Bitmap((uint)width, (uint)height, ColorDepth.ColorDepth32);

            for (int srcY = y, destY = 0; destY < height; srcY++, destY++)
            {
                for (int srcX = x, destX = 0; destX < width; srcX++, destX++)
                {
                    bitmap.RawData[destY * width + destX] = canvas.GetPointColor(srcX, srcY).ToArgb();
                }
            }

            return bitmap;
        }
    }
}
