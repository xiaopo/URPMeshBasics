Shader "Unlit/CubeGrid"
{
    Properties
    {
        _MainColor("Color",Color) = (1.0,1.0,1.0,1.0)
        [NoScaleOffset]_MainTex("Main Tex",2D) = "white"{}
        [KeywordEnum(X, Y, Z)] _Faces("Faces", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex  Vertex
            #pragma fragment Fragment
            #pragma multi_compile_instancing
            #pragma shader_feature _FACES_X _FACES_Y _FACES_Z
            //导入SRP相关功能
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

             CBUFFER_START(UnityPerDraw)
                //Unity自动赋值
                float4x4 unity_ObjectToWorld;
                float4x4 unity_WorldToObject;
                float4 unity_LODFade;
                real4 unity_WorldTransformParams;
             CBUFFER_END

             float4x4 unity_MatrixVP;


             TEXTURE2D(_MainTex);
             SAMPLER(sampler_MainTex);

             UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
             UNITY_DEFINE_INSTANCED_PROP(float4, _MainColor)
             UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)

             UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

             struct a2v
             {
                 float4 vertex : POSITION;
                 float4 color:COLOR;
                 UNITY_VERTEX_INPUT_INSTANCE_ID

             };

             struct v2f
             {
                 float4 position : SV_Position;
                 float2 cubeuv : TEXCOORD0;

                 UNITY_VERTEX_INPUT_INSTANCE_ID
             };

             v2f Vertex(a2v input)
             {
                 v2f output;
                 UNITY_SETUP_INSTANCE_ID(input);
                 UNITY_TRANSFER_INSTANCE_ID(input, output);

                 float3 wsPos = mul(unity_ObjectToWorld, input.vertex).xyz;
                 output.position = mul(unity_MatrixVP, float4(wsPos, 1.0));

                #if defined(_FACES_X)
                    output.cubeuv = input.color.yz * 255;
                #elif defined(_FACES_Y)
                    output.cubeuv = input.color.xz * 255;
                #elif defined(_FACES_Z)
                    output.cubeuv = input.color.xy * 255;
                #endif
      
                 return output;

             }

             float4 Fragment(v2f input) : SV_TARGET
             {
                 UNITY_SETUP_INSTANCE_ID(input);

                 float4 baseMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.cubeuv);

                 float4 mcolor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _MainColor);

                 float4 color = baseMap * mcolor;

                 return color;

             }

             ENDHLSL
         }
    }
}
