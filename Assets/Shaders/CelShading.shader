Shader "GAT350/CelShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
        _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Gloss("Gloss", Range(0,32)) = 32
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 0.716
        _RimThreshold("Rim Threshold", Range(0, 1)) = 0.1

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.05)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }
        LOD 100

        // =========================
        // OUTLINE PASS (Inverted Hull)
        // =========================
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
            };

            half4 _OutlineColor;
            float _OutlineWidth;

            v2f vertOutline(Attributes v)
            {
                v2f o;

                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);

                // Expand along world-space normal
                positionWS += normalWS * _OutlineWidth;

                o.positionHCS = TransformWorldToHClip(positionWS);
                return o;
            }

            half4 fragOutline(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // =========================
        // CEL SHADING PASS (Your original)
        // =========================
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            half4 _Color;
            half4 _AmbientColor;
            half4 _SpecularColor;
            half4 _RimColor;
            float _Gloss;
            float _RimAmount;
            float _RimThreshold;

            v2f vert(Attributes v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.positionWS = TransformObjectToWorld(v.positionOS);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.positionWS);

                VertexPositionInputs positions = GetVertexPositionInputs(v.positionOS);
                o.shadowCoord = GetShadowCoord(positions);

                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                albedo *= _Color;

                // NOTE: In URP you typically fetch main light with GetMainLight(),
                // but I'm leaving your logic intact as requested.
                half3 lightDirection = normalize(_MainLightPosition);
                half3 viewDir = normalize(i.viewDir);

                half3 normalWS = normalize(i.normalWS);

                half NdotL = saturate(dot(normalWS, lightDirection));

                half shadow = MainLightRealtimeShadow(i.shadowCoord);
                float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
                float4 light = lightIntensity * _MainLightColor;

                half spectularIntensity = pow(NdotL * lightIntensity, _Gloss * _Gloss);
                half spectularIntensitySmooth = smoothstep(0.005, 0.01, spectularIntensity);
                half4 specular = spectularIntensitySmooth * _SpecularColor;

                half4 rimDot = 1 - dot(viewDir, normalWS);
                float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
                rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
                half4 rim = rimIntensity * _RimColor;

                return albedo * (_AmbientColor + light + specular + rim);
            }
            ENDHLSL
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
