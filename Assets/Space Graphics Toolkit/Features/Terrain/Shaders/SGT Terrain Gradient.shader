Shader "Space Graphics Toolkit/Terrain Gradient"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Glossiness("Glossiness", Range(0,1)) = 1.0
		_Metallic("Metallic", Range(0,1)) = 1.0
		_Tiling("Tiling", Float) = 1.0
		_RadiusMin("Radius Min", Float) = 1.0
		_RadiusMax("Radius Max", Float) = 1.1

		[NoScaleOffset]_ColorTex("Color Tex", 2D) = "white" {}

		[NoScaleOffset]_BaseTex("Base Tex", 2D) = "white" {}
		[NoScaleOffset][Normal]_BaseBump("Base Bump", 2D) = "bump" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Geometry" "Queue" = "Geometry" }
		LOD 200

		CGPROGRAM
		#pragma surface Surf Standard fullforwardshadows vertex:Vert
		#include "UnityCG.cginc"

		#pragma target 3.0

		half4 _Color;
		float _Tiling;
		float _RadiusMin;
		float _RadiusMax;
		half  _Glossiness;
		half  _Metallic;

		sampler2D _ColorTex;

		sampler2D _BaseTex;
		sampler2D _BaseBump;
		half      _BaseBumpScale;

		struct Input
		{
			float2 texcoord0 : TEXCOORD0;
			float  texcoord1 : TEXCOORD1;
			float4 color     : COLOR;
		};

		half4 ScaleNormal(half4 packednormal, half bumpScale)
		{
#if defined(UNITY_NO_DXT5nm)
			packednormal.xy = ((packednormal.xy * 2 - 1) * bumpScale + 1) / 2;
#else
			packednormal.wy = ((packednormal.wy * 2 - 1) * bumpScale + 1) / 2;
#endif
			return packednormal;
		}

		void Vert(inout appdata_full v, out Input o)
		{
			o.texcoord0 = v.texcoord.xy;
			o.texcoord1 = (length(v.vertex.xyz) - _RadiusMin) / (_RadiusMax - _RadiusMin);
			o.color     = v.color;
		}

		void Surf(Input i, inout SurfaceOutputStandard o)
		{
			float2 uv = i.texcoord0 * _Tiling;

			half3 baseTex  = tex2D(_BaseTex , uv).rgb;
			half4 colorTex = tex2D(_ColorTex, i.texcoord1.xx);

			half3 albedo = baseTex * colorTex.rgb;

			o.Albedo     = albedo * _Color;
			o.Normal     = UnpackNormal(tex2D(_BaseBump, uv));
			o.Metallic   = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}