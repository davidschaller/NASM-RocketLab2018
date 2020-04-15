Shader "Starfield/Star Particle (Gamma)"
{
	Properties
	{
		[HideInInspector] _ParticleSize("ParticleSize", Float) = 500
		[HideInInspector] _ClosestParticleDistance("ClosestParticleDistance", Float) = 0
		[HideInInspector] _FarthestParticleDistance("FarthestParticleDistance", Float) = 2000

		_DotSizeMultiplier("DotSizeMultiplier", Range(1, 5)) = 1
		_FarthestDotAlpha("FarthestDotAlpha", Range(0, 1)) = 0.3
		_AlphaPow("AlphaPow", Range(0, 10)) = 1
		//_DebugBackground("DebugBackground", Color) = (0,0,0,0)

		_CoreColorMult("CoreColorMultiplier", Range(0, 5)) = 1
		_GlowColorMult("GlowColorMultiplier", Range(0, 5)) = 1

		_Core1Size("Core1Size", Range(0, 1)) = 0.005
		_Core1Pow("Core1Pow", Range(0, 10)) = 10
		_Core2Size("Core2Size", Range(0, 1)) = 0.01
		_Core2Pow("Core2Pow", Range(0, 10)) = 0.22
		_Glow1Size("Glow1Size", Range(0, 2)) = 0.43
		_Glow1Pow("Glow1Pow", Range(0, 10)) = 0.21
		_Glow2Size("Glow2Size", Range(0, 2)) = 0.73
		_Glow2Pow("Glow2Pow", Range(0, 10)) = 0.15

		_ScintillationSpeed("ScintillationSpeed", Range(0, 10)) = 0.5
		_ScintillationStrength("ScintillationStrength", Range(0, 1)) = 0.5
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off
		BlendOp Add

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag	
			#pragma multi_compile __ ENABLE_SCINTILLATION
			
			#include "UnityCG.cginc"
			
			float _ParticleSize; // Diameter
			float _FarthestParticleDistance;
			float _ClosestParticleDistance;

			float _DotSizeMultiplier;
			float _FarthestDotAlpha;
			float _AlphaPow;
			//float4 _DebugBackground;
			float _CoreColorMult;
			float _GlowColorMult;
			float _Core1Size;
			float _Core1Pow;
			float _Core2Size;
			float _Core2Pow;
			float _Glow1Size;
			float _Glow1Pow;
			float _Glow2Size;
			float _Glow2Pow;
			float _ScintillationSpeed;
			float _ScintillationStrength;

			struct vertexInput 
			{
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
				float4 color : COLOR;
			};

			struct fragmentInput 
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float4 uv : TEXCOORD0;
				float4 offset : TEXCOORD1;
			};
			
			fragmentInput vert(vertexInput i) 
			{
				fragmentInput o;
				o.pos = UnityObjectToClipPos(i.vertex);

				float4 vertexPosCamSpace = float4(UnityObjectToViewPos(i.vertex), 0.0);
				float4 vertexPosCamSpaceWithOffset = vertexPosCamSpace + float4(_ParticleSize, 0.0, 0.0, 0.0);
				vertexPosCamSpace = mul(UNITY_MATRIX_P, vertexPosCamSpace);
				vertexPosCamSpaceWithOffset = mul(UNITY_MATRIX_P, vertexPosCamSpaceWithOffset);
				
				float4 screenPos = ComputeScreenPos(vertexPosCamSpace);
				screenPos.xy = screenPos.xy / screenPos.w;

				float4 screenPos2 = ComputeScreenPos(vertexPosCamSpaceWithOffset);
				screenPos2.xy = screenPos2.xy / screenPos2.w;

				o.offset = screenPos2 - screenPos;
				o.offset.w = distance(mul(unity_ObjectToWorld, i.vertex), _WorldSpaceCameraPos);

				// Passing the z depth of the vertex for use in randomizing the stars
				o.offset.z = abs(vertexPosCamSpaceWithOffset.z / 100);

				o.uv = i.texcoord0;
				o.color = i.color;				
				
				return o;
			}
			
			fixed4 frag (fragmentInput i) : SV_Target
			{
				float uvDist = length(i.uv - float2(0.5, 0.5));

				float particleScreenSize = i.offset.x;
				float particleSizeInPixels = floor(particleScreenSize * _ScreenParams.x);

				float twinkleFactor = 1; // No twinkle
			#if ENABLE_SCINTILLATION
				// Twinkle
				float starRandomizer = i.offset.z;
				float time = (starRandomizer + _Time.y) * _ScintillationSpeed;
				float twinkle = max(sin(time * 2), sin(time * 3 + 1)); // The numbers are pretty much random, what matters is that the sinusoids have different periods and offsets, so maxing them makes the star scintillation pattern less obvious to the eye
				twinkle = max(twinkle, sin(time * 10));
				twinkleFactor = (1 - _ScintillationStrength) + saturate(twinkle) * _ScintillationStrength;
			#endif

				// 1 pixel * _DotSizeMultiplier
				float minSizeUV = (_DotSizeMultiplier / particleSizeInPixels) * 0.7; // This 0.7 should be 0.5, but then in some situations all four pixels around the UV center could fail to activate... So I use the diagonal of 0.7 instead, but now in some situations there can be more pixels.
				minSizeUV = max(minSizeUV, 0.001); // To fix precision problem on closest particles, that makes them flicker more than distant ones
				
				float4 dotColor = float4(1.0, 1.0, 1.0, 1.0) * ceil(max(minSizeUV - uvDist, 0));
				// Equivalent to:
				/*if (uvDist <= minSizeUV)
					return dotColor = fixed4(1.0, 1.0, 1.0, 1.0);*/
				
				// Gamma space: linearize the color before processing it
				float4 inputColor = i.color;
				inputColor.rgb = pow(i.color.rgb, 2.2);
				
				float worldDistanceFromCam = i.offset.w;
				float distanceRatio = saturate((worldDistanceFromCam - _ClosestParticleDistance) / (_FarthestParticleDistance - _ClosestParticleDistance));
				dotColor.a = dotColor.r * lerp(1, _FarthestDotAlpha, pow(distanceRatio, 2));
				dotColor.a *= twinkleFactor;
				dotColor.rgb *= dotColor.a;

				float core = pow(uvDist / _Core1Size, _Core1Pow);
				float core2 = pow(uvDist / _Core2Size, _Core2Pow);
				float glow1 = pow(uvDist / _Glow1Size, _Glow1Pow);
				float glow2 = pow(uvDist / _Glow2Size, _Glow2Pow);

				float glowFactor = min(glow1, glow2);
				float coreFactor = min(core, core2);
				float factor = min(coreFactor, glowFactor);

				float4 color = saturate(inputColor * (1 - glowFactor) * _GlowColorMult) + saturate(inputColor * _CoreColorMult * (1-coreFactor));
				color = saturate(color);
				color.a = pow(color.a, _AlphaPow);
				color.a *= twinkleFactor;
				
				// Gamma space: now that the color has been processed, restore the gamma correction
				color.rgb = pow(color.rgb, 0.45);

				return dotColor + color; // +_DebugBackground;
			}

			ENDCG
		}
	}
}
