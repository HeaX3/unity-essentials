using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/*
    This class hooks into Unity's asset import process to combine "Metallic" and "Smoothness" textures
    into a single "Metallic-Smoothness" texture to be used in the "Metallic" texture slot of the standard shader.
    Editing the "smoothness" part of the combined texture is very difficult to do since it lives in the alpha channel.
    This lets you keep the smoothness texture separate, for ease of editing.
    For example, you may want to save it as a PSD file with a UV Map layer that you turn on when working on it,
    then turn back off when saving, or have multiple layers of elements that are more flexible to adjust dynamically.

    Written by Todd Gillissie of Gilligames.
    Feel free to use and distribute freely.

    Edited by HeaX to include an auto converter for Roughness input textures.

    Instructions: Put two textures into the same folder, one with a -metallic suffix in the name, and one with either a -roughness or -smoothness suffix.
    Roughness textures represent rough areas as white, where smoothness textures represent smooth areas as white.
    */

namespace Essentials
{
    public class ImportMetallicSmoothness : AssetPostprocessor
    {
        private static readonly NamePattern[] patterns =
        {
            new("metallic", "smoothness", "roughness", "-"),
            new("Metallic", "Smoothness", "Roughness", "_"),
        };

        void OnPreprocessTexture()
        {
            if (patterns.All(p => !isMetallicOrSmoothness(p)))
            {
                return;
            }

            // Sets some required import values for reading the texture's pixels for combining.
            var textureImporter = (TextureImporter)assetImporter;

            textureImporter.isReadable = true;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = false;
        }

        // We need to do the actual combining of textures in the Postprocessor, since the original texture needs to be finished processing first.
        void OnPostprocessTexture(Texture2D texture)
        {
            foreach (var p in patterns)
            {
                Handle(texture, p);
            }
        }

        private void Handle(Texture2D texture, NamePattern pattern)
        {
            if (!isMetallicOrSmoothness(pattern))
            {
                return;
            }

            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var combinedPath = "";

            Texture2D metallic = null;
            Texture2D smoothness = null;
            Texture2D roughness = null;
            var smoothnessIsDerived = false;

            if (filename.EndsWith($"{pattern.concatenation}{pattern.metallic}"))
            {
                metallic = texture;

                var smoothnessPath =
                    convertMetallicSmoothnessPath(pattern, pattern.metallic, pattern.smoothness, out combinedPath);

                if (File.Exists(smoothnessPath))
                {
                    smoothness = AssetDatabase.LoadAssetAtPath<Texture2D>(smoothnessPath);
                }

                var roughnessPath =
                    convertMetallicSmoothnessPath(pattern, pattern.metallic, pattern.roughness, out combinedPath);

                if (File.Exists(roughnessPath))
                {
                    roughness = AssetDatabase.LoadAssetAtPath<Texture2D>(roughnessPath);
                }
            }
            else if (filename.EndsWith($"{pattern.concatenation}{pattern.smoothness}"))
            {
                smoothness = texture;

                var metallicPath =
                    convertMetallicSmoothnessPath(pattern, pattern.smoothness, pattern.metallic, out combinedPath);

                if (File.Exists(metallicPath))
                {
                    metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(metallicPath);
                }
            }
            else if (filename.EndsWith($"{pattern.concatenation}{pattern.roughness}"))
            {
                roughness = texture;

                var metallicPath =
                    convertMetallicSmoothnessPath(pattern, pattern.roughness, pattern.metallic, out combinedPath);

                if (File.Exists(metallicPath))
                {
                    metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(metallicPath);
                }
            }

            if (metallic == null)
            {
                // Debug.LogWarningFormat("Associated Metallic texture not found for: {0}", filename);
                return;
            }

            if (smoothness == null && roughness != null)
            {
                smoothness = new Texture2D(metallic.width, metallic.height, TextureFormat.ARGB32, false);
                var pixels = roughness.GetPixels32();
                for (var i = 0; i < smoothness.width * smoothness.height; i++)
                {
                    pixels[i] = Color.white - pixels[i];
                }

                smoothness.SetPixels32(pixels);
                smoothness.Apply(false, false);
                smoothnessIsDerived = true;
            }

            if (smoothness == null)
            {
                // Debug.LogWarningFormat("Associated Smoothness texture not found for: {0}", filename);
                return;
            }

            if (metallic.width != smoothness.width || metallic.height != smoothness.height)
            {
                Debug.LogWarningFormat(
                    "Metallic and Smoothness textures must be the same size in order to combine: {0}", assetPath);
                return;
            }

            var metallicPixels = metallic.GetPixels32();
            var smoothnessPixels = smoothness.GetPixels32();

            var combined = new Texture2D(metallic.width, metallic.height, TextureFormat.ARGB32, false);

            // Use the red channel info from smoothness for the alpha channel of the combined texture.
            // Since the smoothness should be grayscale, we just use the red channel info.
            for (var i = 0; i < metallicPixels.Length; i++)
            {
                metallicPixels[i].a = smoothnessPixels[i].r;
            }

            combined.SetPixels32(metallicPixels);

            if (smoothnessIsDerived)
            {
                Object.DestroyImmediate(smoothness);
            }

            // Save the combined data.
            var png = combined.EncodeToPNG();
            File.WriteAllBytes(combinedPath, png);

            AssetDatabase.ImportAsset(combinedPath);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // Helper functions.
        ////////////////////////////////////////////////////////////////////////////////////////////////

        // Returns true if the texture being processed ends with " Metallic" or " Smoothness",
        // since we only want to work with those.
        private bool isMetallicOrSmoothness(NamePattern pattern)
        {
            var filename = Path.GetFileNameWithoutExtension(assetPath);

            return !filename.EndsWith(
                       $"{pattern.concatenation}{pattern.metallic}{pattern.concatenation}{pattern.smoothness}") &&
                   (filename.EndsWith($"{pattern.concatenation}{pattern.metallic}") ||
                    filename.EndsWith($"{pattern.concatenation}{pattern.smoothness}") ||
                    filename.EndsWith($"{pattern.concatenation}{pattern.roughness}"));
        }

        private string convertMetallicSmoothnessPath(NamePattern pattern, string from, string to,
            out string combinedPath)
        {
            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var extension = Path.GetExtension(assetPath);
            var pathWithoutFilename = Path.GetDirectoryName(assetPath);
            var baseFilename = filename[..^$"{pattern.concatenation}{from}".Length];

            var newPath = Path.Combine($"{pathWithoutFilename}",
                $"{baseFilename}{pattern.concatenation}{to}{extension}");

            combinedPath = Path.Combine($"{pathWithoutFilename}",
                $"{baseFilename}{pattern.concatenation}{pattern.metallic}{pattern.concatenation}{pattern.smoothness}.png");

            return newPath;
        }

        private readonly struct NamePattern
        {
            public readonly string metallic;
            public readonly string smoothness;
            public readonly string roughness;
            public readonly string concatenation;

            public NamePattern(string metallic, string smoothness, string roughness, string concatenation)
            {
                this.metallic = metallic;
                this.smoothness = smoothness;
                this.roughness = roughness;
                this.concatenation = concatenation;
            }
        }
    }
}