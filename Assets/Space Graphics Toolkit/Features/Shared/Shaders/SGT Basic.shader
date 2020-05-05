Shader "Space Graphics Toolkit/SGT Basic"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB) Smoothness (A)", 2D) = "white" {}
		[Normal]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Map Strength", Range(0,5)) = 1
		_Metallic("Metallic", Range(0,1)) = 0
		_GlossMapScale("Smoothness", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 400
		CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"

			float4    _Color;
			sampler2D _MainTex;
			sampler2D _BumpMap;
			float     _BumpScale;
			float     _Metallic;
			float     _GlossMapScale;

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};

			void surf(Input i, inout SurfaceOutputStandard o)
			{
				float4 texMain = tex2D(_MainTex, i.uv_MainTex);

				o.Albedo     = texMain.rgb * _Color.rgb;
				o.Normal     = UnpackScaleNormal(tex2D(_BumpMap, i.uv_BumpMap), _BumpScale);
				o.Metallic   = _Metallic;
				o.Smoothness = _GlossMapScale * texMain.a;
			}
		ENDCG
	}
	FallBack "Standard"
}