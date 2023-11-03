// Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable

using System.Threading;
using UnityEngine;

namespace Essentials
{
    public static class TextureUtility
    {
        /// <summary>
        /// Returns a scaled copy of given texture. 
        /// </summary>
        /// <param name="tex">Source texure to scale</param>
        /// <param name="width">Destination texture width</param>
        /// <param name="height">Destination texture height</param>
        /// <param name="mode">Filtering mode</param>
        public static Texture2D CreateScaledCopy(this Texture2D tex, int width, int height,
            FilterMode mode = FilterMode.Trilinear)
        {
            //Get rendered data back to a new texture
            Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
            _gpu_scale(tex, result, width, height, mode);

            return result;
        }

        /// <summary>
        /// Scales the texture data of the given texture.
        /// </summary>
        /// <param name="tex">Texure to scale</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Filtering mode</param>
        public static void Scale(this Texture2D tex, int width, int height, FilterMode mode)
        {
            _gpu_scale(tex, tex, width, height, mode);
        }

        /// <summary>
        /// Scales the texture data of the given texture.
        /// </summary>
        /// <param name="tex">Texure to scale</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        public static void Bilinear(this Texture2D tex, int width, int height)
        {
            Scale(tex, width, height, FilterMode.Bilinear);
        }

        /// <summary>
        /// Scales the texture data of the given texture.
        /// </summary>
        /// <param name="tex">Texure to scale</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        public static void Trilinear(this Texture2D tex, int width, int height)
        {
            Scale(tex, width, height, FilterMode.Trilinear);
        }

        // Internal unility that renders the source texture into the RTT - the scaling method itself.
        static void _gpu_scale(Texture2D src, Texture2D target, int width, int height, FilterMode fmode)
        {
            //We need the source texture in VRAM because we render with it
            src.filterMode = fmode;
            src.Apply(true);

            //Using RTT for best quality and performance. Thanks, Unity 5
            var renderTexture = RenderTexture.GetTemporary(width, height, 32);

            //Set the RTT in order to render to it
            RenderTexture.active = renderTexture;

            //Setup 2D matrix in range 0..1, so nobody needs to care about sized
            GL.LoadPixelMatrix(0, 1, 1, 0);

            //Then clear & draw the texture to fill the entire RTT.
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            Graphics.Blit(src, renderTexture, new Vector2(1, 1), Vector2.zero);
            var texR = new Rect(0, 0, width, height);
            if (target.width != width || target.height != height) target.Reinitialize(width, height);
            target.ReadPixels(texR, 0, 0, true);
            target.Apply(true);

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }
}