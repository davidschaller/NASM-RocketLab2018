Shader "CorvoShader/CAtmosphere" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
		_intensity ("Intensity", Range(0,10)) = 1.5
			_maxIntensity("Max Intensity", Range(0,100)) = 4
			_fade("Atmos Fade", Range(1,10000000)) = 1000
			//_rad("Atmos Radius", Range(1,9000000)) = 10000
	}
	SubShader {
			Tags{ "Queue" = "Geometry" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float _fade;
		float _intensity;
		float _maxIntensity;
		float _rad;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
			float3 worldPos;
			INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
			void vert(inout appdata_full v) {
			v.normal.xyz = v.normal * -1;
		}
		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//fade su dstanza
			float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
			float fadeVal = dist / _fade;
			if (fadeVal > 1) fadeVal = 1;

			//fade su angolo
			float x = 1;
			//float4 objectOrigin = mul(_Object2World, float4(0.0, 0.0, 0.0, 1.0));
			//float distFromCenter = distance(objectOrigin, _WorldSpaceCameraPos);
			float3 worldNormal = WorldNormalVector(IN, o.Normal);
			x = saturate(0.5 - (dot(IN.worldNormal, normalize(IN.viewDir))));
			x = pow(x, 6);

			//intensity su distance
			//_intensity = _fade * 10 / dist;

			//max intensity
			if (_intensity > _maxIntensity)
				_intensity = _maxIntensity;


			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb*_intensity;
			o.Metallic = 0;// _Metallic;
			o.Smoothness = 0;// _Glossiness;
			o.Alpha = c.a*fadeVal*x;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
