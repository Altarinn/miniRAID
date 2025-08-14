Shader "Hidden/PixelArtURP/RetroScanlineUpscaling" 
{ 
    HLSLINCLUDE 
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl" 
 
        int2 _pixelScaling; 
        float4 _DestinationSize; 
        
        // Customizable parameters (you can expose these in your renderer feature)
        #define SCANLINE_STRENGTH 0.35    // How dark the scanlines are (0-1)
        #define SCANLINE_FREQUENCY 2.0     // Scanline spacing
        #define GLOW_STRENGTH 0.4          // Bloom/glow intensity
        #define GLOW_RADIUS 1.5            // Glow spread radius
        #define PHOSPHOR_STRENGTH 0.15     // RGB phosphor separation effect
        #define BRIGHTNESS_BOOST 1.15      // Overall brightness multiplier
        
        // Helper function to get luminance
        float GetLuminance(float3 color)
        {
            return dot(color, float3(0.299, 0.587, 0.114));
        }
        
        // Smooth scanline function
        float ScanlineEffect(float2 uv, float frequency)
        {
            float scanline = sin(uv.y * _DestinationSize.y * 3.14159 / frequency);
            scanline = scanline * 0.5 + 0.5; // Remap to 0-1
            return lerp(1.0 - SCANLINE_STRENGTH, 1.0, scanline);
        }
        
        // Phosphor RGB mask (subtle CRT color separation)
        float3 PhosphorMask(float2 uv)
        {
            float3 mask;
            int x = int(uv.x * _DestinationSize.x);
            
            // RGB phosphor pattern
            if (x % 3 == 0)
                mask = float3(1.0, 0.7, 0.7);
            else if (x % 3 == 1)
                mask = float3(0.7, 1.0, 0.7);
            else
                mask = float3(0.7, 0.7, 1.0);
                
            return lerp(1.0, mask, PHOSPHOR_STRENGTH);
        }
        
        // Glow/bloom sampling for bright pixels
        float3 GetGlow(float2 uv, float radius)
        {
            float3 glow = 0.0;
            float totalWeight = 0.0;
            
            // Sample in a small radius for glow effect
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    float2 offset = float2(x, y) * radius / _DestinationSize.xy;
                    float3 sample = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv + offset).rgb;
                    
                    // Weight by brightness and distance
                    float brightness = GetLuminance(sample);
                    float dist = length(float2(x, y)) / 2.0;
                    float weight = brightness * exp(-dist * dist);
                    
                    glow += sample * weight;
                    totalWeight += weight;
                }
            }
            
            return (totalWeight > 0.0) ? glow / totalWeight : 0.0;
        }
 
        half4 RetroScanlinePost(Varyings input) : SV_Target 
        { 
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input); 
 
            float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord); 
            
            // Get base pixel color
            half3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv).rgb;
            
            // Calculate luminance for glow effect
            float luminance = GetLuminance(color);
            
            // Add glow for bright pixels
            if (luminance > 0.5)
            {
                float3 glow = GetGlow(uv, GLOW_RADIUS);
                float glowStrength = smoothstep(0.5, 1.0, luminance) * GLOW_STRENGTH;
                color = lerp(color, color + glow, glowStrength);
            }
            
            // Apply phosphor mask for RGB separation
            color *= PhosphorMask(uv);
            
            // Apply scanline effect
            float scanlineIntensity = ScanlineEffect(uv, SCANLINE_FREQUENCY);
            
            // Modulate scanline strength based on pixel brightness
            // (brighter pixels "burn through" scanlines more)
            float brightnessModulation = smoothstep(0.0, 0.8, luminance);
            scanlineIntensity = lerp(scanlineIntensity, 1.0, brightnessModulation * 0.3);
            
            color *= scanlineIntensity;
            
            // Boost overall brightness slightly for that CRT glow
            color *= BRIGHTNESS_BOOST;
            
            // Add subtle vignette for retro feel
            float2 vignetteCoord = (uv - 0.5) * 2.0;
            float vignette = 1.0 - dot(vignetteCoord, vignetteCoord) * 0.15;
            color *= vignette;
            
            // Clamp to prevent over-bright pixels
            color = clamp(color, 0.0, 1.5);
 
            return half4(color, 1.0);
        } 
    ENDHLSL 
 
    SubShader 
    { 
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"} 
        LOD 100 
        ZTest Always ZWrite Off Cull Off 
         
        Pass 
        { 
            Name "RetroScanlineUpscaling" 
             
            HLSLPROGRAM 
                #pragma vertex Vert 
                #pragma fragment RetroScanlinePost 
            ENDHLSL 
        } 
    } 
}