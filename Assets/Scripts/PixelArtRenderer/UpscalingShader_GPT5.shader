Shader "Hidden/PixelArtURP/CRT_ScanlineUpscale"
{
    Properties
    {
        // Horizontal scanline shape
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.35    // how dark the valleys are
        _BeamMinWidth     ("Beam Min Width",   Range(0.02,0.50)) = 0.12// width at dark pixels (0..0.5 of a row)
        _BeamMaxWidth     ("Beam Max Width",   Range(0.04,0.60)) = 0.32// width at bright pixels
        _BeamSharpness    ("Beam Sharpness",   Range(1,8)) = 2.0       // curve steepness

        // Horizontal glow/bloom across neighboring source pixels
        _GlowStrength     ("Glow Strength",    Range(0,1)) = 0.25
        _GlowRadius       ("Glow Radius (px)", Range(0.5,3.0)) = 1.25  // in source-pixel units

        // Final gain (no gamma ops performed)
        _Exposure         ("Exposure",         Range(0.5,2.0)) = 1.00
    }

    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

        // Provided by your script (same as in your snippet):
        int2  _pixelScaling;        // integer upscale factor (dst/src)
        float4 _DestinationSize;    // (width, height, 1/width, 1/height)

        CBUFFER_START(UnityPerMaterial)
            float _ScanlineStrength;
            float _BeamMinWidth;
            float _BeamMaxWidth;
            float _BeamSharpness;
            float _GlowStrength;
            float _GlowRadius;   // in source-pixel units
            float _Exposure;
        CBUFFER_END

        // Return uv of the center of the nearest *source* pixel, given integer scale.
        float2 SourcePixelCenterUV(float2 uv, float2 srcTexel)
        {
            // convert to source pixel space, snap to center, go back to UV
            float2 p = uv / srcTexel;
            p = floor(p) + 0.5;
            return p * srcTexel;
        }

        // Simple luminance (no gamma adjustments).
        float Luma(float3 c)
        {
            return dot(c, float3(0.2126, 0.7152, 0.0722));
        }

        half4 FragCRT(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

            // Size relations: one *source* pixel spans this much in UV.
            float2 srcTexel = (float2)_pixelScaling / _DestinationSize.xy;

            // Sample base color from the center of the nearest source pixel (point).
            float2 baseUV   = SourcePixelCenterUV(uv, srcTexel);
            half4  baseCol  = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, baseUV);
            float  lum      = Luma(baseCol.rgb);

            // ---------- Horizontal scanline shaping ----------
            // Phase within the vertical block that corresponds to one source pixel row.
            float rowPhase = frac(uv.y / srcTexel.y);            // [0,1)
            float dist     = abs(rowPhase - 0.5);                // distance from row center (0 at center)
            // Brightness controls beam width -> brighter = thicker
            float beamW = lerp(_BeamMinWidth, _BeamMaxWidth, pow(saturate(lum), 1.0));
            beamW = max(beamW, 1e-4);

            // Smooth, gaussian-like beam profile raised by sharpness for crisper edges.
            float beam = exp(- (dist * dist) / (beamW * beamW));
            beam = pow(saturate(beam), _BeamSharpness);

            // Convert beam profile into scanline multiplier (valleys get darker).
            // When beam==0 -> multiplier = (1 - _ScanlineStrength)
            // When beam==1 -> multiplier = 1
            float scanMul = lerp(1.0 - _ScanlineStrength, 1.0, beam);

            // ---------- Brightness-aware horizontal glow (cheap 5-tap) ----------
            // Measured in *source* pixels, so it stays stable regardless of upscale.
            float sigma = max(0.001, _GlowRadius);
            float w0 = 1.0;
            float w1 = exp(-0.5 * (1.0 / (sigma * sigma)));
            float w2 = exp(-0.5 * (4.0 / (sigma * sigma))); // (2 px offset)^2 = 4
            float norm = w0 + 2.0 * (w1 + w2);

            float2 stepUV = float2(srcTexel.x, 0.0);

            half3 c0 = baseCol.rgb;
            half3 cL1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, baseUV - stepUV).rgb;
            half3 cR1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, baseUV + stepUV).rgb;
            half3 cL2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, baseUV - 2.0 * stepUV).rgb;
            half3 cR2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, baseUV + 2.0 * stepUV).rgb;

            half3 blur = (c0 * w0 + (cL1 + cR1) * w1 + (cL2 + cR2) * w2) / norm;

            // Mix in a bit of horizontal blur as "glow". Stronger on bright pixels.
            float glowAmt = _GlowStrength * pow(saturate(lum), 0.8);
            half3 withGlow = c0 + (blur - c0) * glowAmt;

            // Apply scanline multiplier after glow so the lines remain visible.
            half3 outRGB = withGlow * scanMul;

            // Exposure (linear gain only; no gamma fiddling).
            outRGB *= _Exposure;

            return half4(saturate(outRGB), baseCol.a);
        }
    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "CRT_ScanlineUpscale"
            HLSLPROGRAM
                #pragma vertex   Vert
                #pragma fragment FragCRT
            ENDHLSL
        }
    }
}
