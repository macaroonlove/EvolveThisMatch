Shader "UI/2D_Unlit_Flipbook_URP"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        [HDR]_R("R", Color) = (1,0.9719134,0.5896226,0)
        [HDR]_G("G", Color) = (1,0.7230805,0.25,0)
        [HDR]_B("B", Color) = (0.5943396,0.259371,0.09812209,0)
        [HDR]_Outline("Outline", Color) = (0.2169811,0.03320287,0.02354041,0)
        _EmissionColor("Emission", Color) = (0,0,0,0)

        _FlipbookX("Flipbook X", Float) = 1
        _FlipbookY("Flipbook Y", Float) = 1
        _Frame("Frame", Float) = 0
        _DisolveMap("Disolve Map", 2D) = "white" {}
        _DistortionTexture("Distortion Texture", 2D) = "white" {}
        _DistortionSecond("Distortion Second", 2D) = "white" {}
    }

        SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _DisolveMap;
            sampler2D _DistortionTexture;
            sampler2D _DistortionSecond;

            float4 _R;
            float4 _G;
            float4 _B;
            float4 _Outline;
            float4 _EmissionColor;

            float _FlipbookX;
            float _FlipbookY;
            float _Frame;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex); // UI 호환
                o.uv = v.uv;
                return o;
            }

            float4 ApplyHDRColor(float4 texCol)
            {
                // RGB 채널별 HDR 조합
                float3 hdrCol = texCol.r * _R.rgb + texCol.g * _G.rgb + texCol.b * _B.rgb;
                // Outline 섞기
                hdrCol = lerp(hdrCol, _Outline.rgb, 0.15); // 0.15 정도로 살짝 강조
                return float4(hdrCol, texCol.a);
            }

            float4 frag(v2f i) : SV_Target
            {
                // Flipbook UV 계산
                float2 frameUV = float2(
                    fmod(_Frame, _FlipbookX) / _FlipbookX,
                    floor(_Frame / _FlipbookX) / _FlipbookY
                );
                float2 scaleUV = float2(1.0 / _FlipbookX, 1.0 / _FlipbookY);
                float2 uv = i.uv * scaleUV + frameUV;

                // 기본 텍스처 샘플링
                float4 col = tex2D(_MainTex, uv);

                // 디졸브 적용
                float dissolve = tex2D(_DisolveMap, i.uv).r;
                col.a *= dissolve;

                // 왜곡 적용
                float2 distortion = tex2D(_DistortionTexture, i.uv).rg * 0.05;
                float2 distortion2 = tex2D(_DistortionSecond, i.uv).rg * 0.03;
                col.rgb = tex2D(_MainTex, uv + distortion + distortion2).rgb;

                // HDR 컬러 적용
                col = ApplyHDRColor(col);

                // Emission 추가
                col.rgb += _EmissionColor.rgb;

                return col;
            }
            ENDHLSL
        }
    }
}