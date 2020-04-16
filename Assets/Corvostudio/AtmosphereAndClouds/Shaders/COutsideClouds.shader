Shader "CorvoShader/OutsideClouds" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Threshold("Cutout threshold", Range(0,1)) = 0.1
		_Softness("Cutout softness", Range(0,0.5)) = 0.0
		_fade("Cloud Fade", Range(1,100000)) = 1000
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM
	#pragma surface surf Lambert alpha

	sampler2D _MainTex;
	float _Threshold;
	float _Softness;
	float _fade;

	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex);

		float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
		float fadeVal = dist / _fade;
		if (fadeVal > 1) fadeVal = 1;

		o.Albedo = c.rgb;
		o.Alpha = smoothstep(_Threshold, _Threshold + _Softness, 0.333 * (c.r + c.g + c.b))*fadeVal;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
