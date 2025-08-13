using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace miniRAID.PixelArtRenderer
{
    public class PixelArtRendererFeature : ScriptableRendererFeature
    {
        public Material upscalingMaterial;
        
        private PixelArtUpscalingPass _upscalingPass;
        
        public override void Create()
        {
            _upscalingPass = new PixelArtUpscalingPass(upscalingMaterial)
            {
                renderPassEvent = RenderPassEvent.AfterRendering
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_upscalingPass == null || upscalingMaterial == null)
            {
                Debug.LogWarning("PixelArtRendererFeature: Pass or material is null");
                return;
            }
            
            renderer.EnqueuePass(_upscalingPass);
        }
    }
}