Shader "Hidden/PixelArtURP/UpscalingShader"
{
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

        int2 _pixelScaling;
        float4 _DestinationSize;

        half4 PAFinalPost(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

            half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uv);

            // if(int2(uv * _DestinationSize.xy).y % 4 == 0)
            // {
            //     color.rgb = 0.0f;
            // }

            return color;
        }
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off
        
        Pass
        {
            Name "ScanlineUpscaling"
            
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment PAFinalPost
            ENDHLSL
        }
    }
}
