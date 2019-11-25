// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Puffy_Smoke/FakedVolumetric" {

Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.0,1)
	_ShadowColor ("Shadow Color", Color) = (0.0,0.5,0.5,1)
	
	_MainTex ("Particle Texture", 2D) = "white" {}
	_DetailTex ("Particle Details Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	
	_Opacity ("Opacity", Range(0.0,1)) = 0.5
	
	_Scattering ("Scattering", Range(0.0,1.0)) = 1
	
	_Density ("Density", Range(0.0,1.0)) = 0
	_Sharpness ("Sharpness", Range(0.0,5.0)) = 0
	_DetailsSpeed ("Details Speed", Range(0.01,1.0)) = 0.2
	
	
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "texcoord", texcoord0
		Bind "texcoord1", texcoord1
	}
	
	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
// Upgrade NOTE: excluded shader from Xbox360; has structs without semantics (struct appdata_t members worldPos)
#pragma exclude_renderers xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _DetailTex;
			fixed4 _TintColor;
	      	fixed4 _ShadowColor;
	      	float _Density;
        	float _DetailsSpeed;
        	float _Sharpness;
        	float _Scattering;
        	float _Opacity;
        	
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
			};
			
			float4 _MainTex_ST;
			float4 _DetailTex_ST;
			
			v2f vert (appdata_t v)
			{
				//float3 worldPos = mul(_Object2World, v.vertex).xyz;
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord1,_DetailTex);
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : COLOR
			{
								
				float _old = i.color.a; // particle age is stored in color the alpha channel
				float _young = 1f - _old;
				
				// get main texture color and alpha
				float4 _sampled = tex2D(_MainTex, i.texcoord);

				// mix light color and shadow color
				float4 _finalcolor = lerp(_ShadowColor ,_TintColor ,_sampled.r);  
				
				// older particles gets more light color
				_finalcolor = lerp(_finalcolor * i.color , (_TintColor * _old + _finalcolor * _young) * i.color, _Scattering);
				
				if(_Density < 1f){
					// animated noise
					float _details = 1f;
				
					_details = tex2D(_DetailTex, i.texcoord1 * (1f + _young * _DetailsSpeed)).r;
					_details = max(0f,min(1f , lerp((0.5f+_details) * 0.5f, _details , _old*_Sharpness) + _Density));
					
					_sampled.a *= _details;
				}
				
				// main alpha
				_finalcolor.a = _Opacity * _sampled.a * _young;
				
				
				
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				_finalcolor.a *= fade;
				#endif
				
																
				return _finalcolor;
			}
			
			
			ENDCG 
		}	
	} 	
	
	// ---- Dual texture cards
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				Combine texture * constant DOUBLE, texture * primary
			}
			SetTexture [_DetailTex] {
				Combine previous,previous * texture
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}

}

}
