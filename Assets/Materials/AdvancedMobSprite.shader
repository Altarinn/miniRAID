Shader "PixelArtURP/3DSprites"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BehindWallColor ("BehindWallColor", Color) = (1,1,1,1)
        _Scale ("Scale", Int) = 3
        _PixelSize ("PixelSize", Int) = 32
        _SortingOrder ("Sorting Order", Float) = 0
    }
    
    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        struct Attributes
        {
            float3 positionOS   : POSITION;
            float4 color        : COLOR;
            float2 uv           : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        struct Varyings
        {
            float4  positionCS  : SV_POSITION;
            half4   color       : COLOR;
            float2  uv          : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };
        
        CBUFFER_START(UnityPerMaterial)
            half4 _Color, _BehindWallColor;
            int _Scale;
            int _PixelSize;
            float _SortingOrder;
            float4 _MainTex_ST;
        CBUFFER_END

        static float sortingEps = 0.000030517578125f;
        
        Varyings SpriteVertex(Attributes v)
        {
            Varyings o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            
            // Get the world position of the sprite's origin (pivot point)
            float4 originCS = TransformObjectToHClip(float3(0, 0, 0));

            // For constant screen size, we need to bypass perspective division
            // We keep the depth from the projected origin but use screen-space offsets
            // float2 osOffset = float2(UNITY_MATRIX_M[0].x * v.positionOS.x, UNITY_MATRIX_M[1].y * v.positionOS.y); 
            float2 osOffset = v.positionOS.xy;
            float2 screenOffset = osOffset * _Scale * _PixelSize * 2.0; // Adjust multiplier for desired size
            
            // Apply screen-space offset while preserving depth
            float4 positionCS;
            positionCS.x = originCS.x + screenOffset.x * originCS.w / _ScreenParams.x;
            positionCS.y = originCS.y + screenOffset.y * (-_ProjectionParams.x) * originCS.w / _ScreenParams.y;
            positionCS.z = originCS.z;
            positionCS.w = originCS.w;
            
            // Move sprites with higher sorting order a little bit front towards to the camera
            positionCS.z -= _SortingOrder * _ProjectionParams.x * sortingEps * positionCS.w;
            
            o.positionCS = positionCS;
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.color = v.color * _Color;
            
            return o;
        }
    ENDHLSL
    
    SubShader
    {
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        
        Cull Off
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex SpriteVertex
            #pragma fragment SpriteFragment
            
            half4 SpriteFragment(Varyings i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                if(texColor.a < 0.5f)
                {
                    clip(-1);
                }
                return texColor * i.color;
            }
            
            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex SpriteVertex
            #pragma fragment SpriteFragment_BehindWall
            
            half4 SpriteFragment_BehindWall(Varyings i) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                if(texColor.a < 0.5f)
                {
                    clip(-1);
                }
                return _BehindWallColor;
            }
            
            ENDHLSL
        }
    }

//    SubShader
//    {
//        Tags {"Queue" = "Geometry" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
//        
//        Cull Off
//        
//    }
}