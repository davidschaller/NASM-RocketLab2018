Shader "Space Graphics Toolkit/SGT Planet"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Albedo (RGB) Smoothness (A)", 2D) = "white" {}
		[NoScaleOffset]_NightTex("Night Lights (RGB)", 2D) = "black" {}
		_NightSharpness("Night Sharpness", Range(1,20)) = 1
		_NightOffset("Night Offset", Range(-1,2)) = 0
		[NoScaleOffset][Normal]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Map Strength", Range(0,5)) = 1
		[NoScaleOffset]_HeightMap("Height (A)", 2D) = "black" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_Metallic("Metallic", Range(0,1)) = 0
		_GlossMapScale("Smoothness", Range(0,1)) = 1

		[NoScaleOffset]_MaskMap("Mask Map", 2D) = "bump" {}
		_DetailTiling("Detail Tiling", Float) = 10
		[NoScaleOffset][Normal]_DetailMapA("Detail Map A", 2D) = "bump" {}
		_DetailScaleA("Detail Scale A", Range(0,5)) = 1
		[NoScaleOffset][Normal]_DetailMapB("Detail Map B", 2D) = "bump" {}
		_DetailScaleB("Detail Scale B", Range(0,5)) = 1

		[Toggle(SGT_A)]_Water("Water", Float) = 0
		[NoScaleOffset]_WaterLevelMap("Water Level Map", 2D) = "black" {}
		_WaterLevelScale("Water Level Scale", Range(0,0.5)) = 0.02

		_WaterLevel("Water Level", Range(0,1.5)) = 0
		_WaterTiling("Water Tiling", Float) = 10
		[NoScaleOffset]_WaterColor("Water Color", 2D) = "white" {}
		_WaterColorScale("Water Color Scale", Float) = 1
		[NoScaleOffset][Normal]_WaterBumpMap("Water Normal Map", 2D) = "bump" {}
		_WaterBumpScale("Water Normal Scale", Range(0,5)) = 1
		_WaterDistortion("Water Distortion", Range(0,5)) = 2
		_WaterSharpness("Water Sharpness", Float) = 1
		_WaterMetallic("Water Metallic", Range(0,1)) = 0
		_WaterSmoothness("Water Smoothness", Range(0,1)) = 1
		_WaterEmission("Water Emission", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 400
		CGPROGRAM
			#pragma surface surf Standard2 fullforwardshadows vertex:vert
			// Water
			#pragma multi_compile __ SGT_A
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"

			sampler2D _MainTex;
			sampler2D _NightTex;
			float     _NightSharpness = 1.0f;
			float     _NightOffset;
			sampler2D _BumpMap;
			float     _BumpScale;
			sampler2D _HeightMap;
			float4    _Color;
			float     _Metallic;
			float     _GlossMapScale;

			sampler2D _MaskMap;
			float     _DetailTiling;
			sampler2D _DetailMapA;
			float     _DetailScaleA;
			sampler2D _DetailMapB;
			float     _DetailScaleB;

			float     _WaterTiling;
			float     _WaterLevel;
			sampler2D _WaterLevelMap;
			float     _WaterLevelScale;
			sampler2D _WaterColor;
			float     _WaterColorScale;
			sampler2D _WaterBumpMap;
			float     _WaterBumpScale;
			float     _WaterDistortion;
			float     _WaterSharpness;
			float     _WaterMetallic;
			float     _WaterSmoothness;
			float     _WaterEmission;

			struct Input
			{
				float4 detail;
				float3 CoordPolar; // xy = coord, z = polar
			};

			float4 sample2(sampler2D tex, float4 coords, float polar)
			{
				float4 tex1 = tex2D(tex, coords.xy);
				float4 tex2 = tex2D(tex, coords.zw);

				return lerp(tex1, tex2, polar);
			}

			inline half4 LightingStandard2(inout SurfaceOutputStandard s, float3 viewDir, UnityGI gi)
			{
				half4 o = LightingStandard(s, viewDir, gi);
				float d = saturate(dot(s.Normal, -gi.light.dir) + _NightOffset);
				s.Emission *= pow(d, _NightSharpness);
				s.Emission += s.Albedo * s.Alpha;
				return o;
			}

			inline void LightingStandard2_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
			{
				LightingStandard_GI(s, data, gi);
			}

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);

				float3 normalA = normalize(mul(unity_WorldToObject, _WorldSpaceCameraPos).xyz - v.vertex);
				float3 normalB = normalize(v.normal);

				o.detail = float4(v.texcoord.xy, v.texcoord1.xy); o.detail.x *= 2.0f;
				o.CoordPolar.xy = v.texcoord.xy;
				o.CoordPolar.z  = saturate((abs(v.texcoord.y - 0.5f) - 0.2f) * 30.0f);
			}

			void surf(Input i, inout SurfaceOutputStandard o)
			{
				float2 coord    = i.CoordPolar.xy;
				float  polar    = i.CoordPolar.z;
				float4 texMain  = tex2D(_MainTex, coord);
				float4 texNight = tex2D(_NightTex, coord);
				float4 texMask  = tex2D(_MaskMap, coord);

				o.Albedo     = texMain.rgb * _Color.rgb;
				o.Normal     = UnpackScaleNormal(tex2D(_BumpMap, coord), _BumpScale);
				o.Emission   = texNight.rgb;
				o.Metallic   = _Metallic;
				o.Smoothness = _GlossMapScale * texMain.a;
				o.Occlusion  = 1.0f;
				o.Alpha      = 0.0f; // Emission

				// Detail normals?
				float4 detailCoord = i.detail * _DetailTiling;

				float3 detailA = UnpackScaleNormal(sample2(_DetailMapA, detailCoord, polar), texMask.r * _DetailScaleA);
				o.Normal = BlendNormals(o.Normal, detailA);

				float3 detailB = UnpackScaleNormal(sample2(_DetailMapB, detailCoord, polar), texMask.g * _DetailScaleB);
				o.Normal = BlendNormals(o.Normal, detailB);
#if SGT_A // Water
				float4 heightMap  = tex2D(_HeightMap, coord);
				float  water      = _WaterLevel - heightMap.a;
				float4 waterCoord = i.detail * _WaterTiling;

				water += (sample2(_WaterLevelMap, waterCoord, polar).a - 0.5f) * _WaterLevelScale;

				float  waterDensity = saturate(water * _WaterSharpness);
				float  waterColor   = saturate(water * _WaterColorScale);
				float4 waterAlbedo  = tex2D(_WaterColor, float2(waterColor, waterColor));
				float3 waterNormal  = UnpackScaleNormal(sample2(_WaterBumpMap, waterCoord, polar), _WaterBumpScale);

				waterNormal.x *= sin(_Time.x * 3 + texMask.r * _WaterDistortion);
				waterNormal.y *= cos(_Time.x * 11 + texMask.g * _WaterDistortion);
				waterNormal = normalize(waterNormal);

				o.Albedo     = lerp(o.Albedo, waterAlbedo, waterDensity);
				o.Metallic   = lerp(o.Metallic, _WaterMetallic, waterDensity);
				o.Smoothness = lerp(o.Smoothness, _WaterSmoothness, waterDensity);
				o.Alpha      = _WaterEmission * waterDensity;
				o.Normal     = normalize(lerp(o.Normal, waterNormal, waterDensity));
#endif
			}
		ENDCG
	}
	FallBack "Standard"
}