using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelArtRenderer.Editor
{
    public class PixelArtRendererUtils : OdinEditorWindow
    {
        [MenuItem("miniRAID/Rendering/Utils")]
        private static void OpenWindow()
        {
            GetWindow<PixelArtRendererUtils>().Show();
        }

        public RenderTexture targetRT;
        public Texture2D targetTexture;

        [Button("Save Texture To Disk")]
        public void SaveTextureToDisk()
        {
            if (targetTexture == null)
            {
                if (targetRT != null && targetRT.dimension == TextureDimension.Tex2D)
                {
                    targetTexture = new Texture2D(targetRT.width, targetRT.height);
                    RenderTexture.active = targetRT;
                    targetTexture.ReadPixels(new Rect(0, 0, targetRT.width, targetRT.height), 0, 0);
                    targetTexture.Apply();
                    RenderTexture.active = null;
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Select Texture",
                        "You Must Select a Texture first!",
                        "Ok");
                    return;
                }
            }

            var path = EditorUtility.SaveFilePanel(
                "Save texture as PNG",
                "",
                targetTexture.name + ".png",
                "png");

            if (path.Length != 0)
            {
                var pngData = targetTexture.EncodeToPNG();
                if (pngData != null)
                    File.WriteAllBytes(path, pngData);
            }
        }
    }
}