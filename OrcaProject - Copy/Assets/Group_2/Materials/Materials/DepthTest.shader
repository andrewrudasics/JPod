Shader "Lightweight MultPass Test"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_BumpMap("Bumpmap", 2D) = "bump" {}
	}
		SubShader
		{
			Tags{ "RenderPipeline" = "LightweightPipeline" "Queue" = "Transparent" }

			Pass
			{
				Name "Depth Fill"
				Tags{"LightMode" = "SRPDefaultUnlit"}

				Cull Off
				ZTest LEqual
				ZWrite On
				ColorMask 0

				HLSLPROGRAM
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0

				#pragma multi_compile_instancing

				#pragma vertex vert
				#pragma fragment frag

				#include "Packages\com.unity.render-pipelines.lightweight\ShaderLibrary/Core.hlsl"

				struct VertexInput
				{
					float4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct VertexOutput
				{
					float4 position : POSITION;
				};

				VertexOutput vert(VertexInput v)
				{
					VertexOutput o = (VertexOutput)0;
					UNITY_SETUP_INSTANCE_ID(v);

					o.position = TransformObjectToHClip(v.vertex.xyz);

					return o;
				}

				void frag(VertexOutput IN) {}
				ENDHLSL
			}

			Pass
			{
				Name "Color Fill"
				Tags{"LightMode" = "LightweightForward" }

				Blend SrcAlpha OneMinusSrcAlpha
				Cull Off
				ZTest LEqual
				ZWrite Off

				HLSLPROGRAM
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0

				#pragma multi_compile_fog
				
				#pragma multi_compile_instancing

				#pragma vertex vert
				#pragma fragment frag

				#include "Packages\com.unity.render-pipelines.lightweight\ShaderLibrary\Core.hlsl"

				TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
				half4 _Color;

				struct VertexInput
				{
					float4 vertex : POSITION;
					float4 texcoord1 : TEXCOORD1;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct VertexOutput
				{
					float4 position : POSITION;
					float4 uv0AndFog : TEXCOORD8;

					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				VertexOutput vert(VertexInput v)
				{
					VertexOutput o = (VertexOutput)0;

					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);

					float4 clipPos = TransformObjectToHClip(v.vertex.xyz);
					half fogFactor = ComputeFogFactor(clipPos.z);

					o.position = clipPos;
					o.uv0AndFog = float4(v.texcoord1.xy, fogFactor, 0.0);

					return o;
				}

				half4 frag(VertexOutput IN) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID(IN);

					float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0AndFog.xy) * _Color;
					//UNITY_APPLY_FOG(color.rgb, IN.uv0AndFog.z);
					return color;
				}
				ENDHLSL
			}
		}
}