using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace miniRAID.PixelArtRenderer
{
    public class PixelArtUpscalingPass : ScriptableRenderPass
    {
        private Material upscalingMaterial;
        
        internal class PassData
        {
            internal TextureHandle originalTex;
            internal TextureHandle targetRT;

            internal UniversalCameraData cameraData;

            internal Material upscalingMaterial;
        }

        public PixelArtUpscalingPass(Material mat)
        {
            upscalingMaterial = mat;
        }
        
        /// <summary>
        /// Returns the scale bias vector to use for final blits to the backbuffer, based on scaling mode and y-flip platform requirements.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="cameraData"></param>
        /// <returns></returns>
        internal static Vector4 GetFinalBlitScaleBias(RTHandle source, RTHandle destination, UniversalCameraData cameraData)
        {
            Vector2 viewportScale = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
            var yflip = cameraData.IsRenderTargetProjectionMatrixFlipped(destination);
            Vector4 scaleBias = !yflip ? new Vector4(viewportScale.x, -viewportScale.y, 0, viewportScale.y) : new Vector4(viewportScale.x, viewportScale.y, 0, 0);

            return scaleBias;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                builder.AllowPassCulling(false);
                
                passData.originalTex = frameData.Get<UniversalResourceData>().cameraColor;
                passData.targetRT = frameData.Get<UniversalResourceData>().backBufferColor;
                passData.cameraData = frameData.Get<UniversalCameraData>();
                passData.upscalingMaterial = upscalingMaterial;
                
                builder.UseTexture(passData.originalTex, AccessFlags.Read);
                // builder.UseTexture(passData.targetRT, AccessFlags.WriteAll);
                builder.SetRenderAttachment(frameData.Get<UniversalResourceData>().backBufferColor, 0, AccessFlags.WriteAll);
                
                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    // Debug.Log("PixelArtUpscaling: Executing render function");
                    if (data.upscalingMaterial == null)
                    {
                        Debug.LogError("PixelArtUpscaling: Material is null!");
                        return;
                    }

                    data.upscalingMaterial.SetVector("_pixelScaling", new Vector4(4, 4));
                    data.upscalingMaterial.SetVector("_DestinationSize", new Vector4(Screen.width, Screen.height));

                    Vector4 scaleBias = GetFinalBlitScaleBias(data.originalTex, data.targetRT, data.cameraData);
                    Blitter.BlitTexture(ctx.cmd, data.originalTex, scaleBias, data.upscalingMaterial, 0);
                });
            }
        }
    }
}