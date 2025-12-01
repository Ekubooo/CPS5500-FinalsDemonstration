Shader "Instanced/Particle2D" {
	Properties {
		
	}
	SubShader {

		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5
            // 必须添加：启用过程化实例
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

			#include "UnityCG.cginc"
			
			StructuredBuffer<float2> Positions2D;
			StructuredBuffer<float2> Velocities;
			StructuredBuffer<float2> DensityData;
			float scale;
			float4 colA;
			Texture2D<float4> ColourMap;
			SamplerState linear_clamp_sampler;
			float velocityMax;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 colour : TEXCOORD1;
			};

            // 必须添加：WebGPU/Vulkan 需要此函数来正确设置每个实例的矩阵
            void setup()
            {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                float2 pos2D = Positions2D[unity_InstanceID];
                float3 pos = float3(pos2D.x, pos2D.y, 0);

                unity_ObjectToWorld._11_21_31_41 = float4(scale, 0, 0, 0);
                unity_ObjectToWorld._12_22_32_42 = float4(0, scale, 0, 0);
                unity_ObjectToWorld._13_23_33_43 = float4(0, 0, scale, 0);
                unity_ObjectToWorld._14_24_34_44 = float4(pos, 1);

                unity_WorldToObject = unity_ObjectToWorld;
                unity_WorldToObject._14_24_34 *= -1;
                unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
            #endif
            }

			v2f vert (appdata_full v)
			{
				v2f o;
                // 必须添加：初始化实例ID
                UNITY_SETUP_INSTANCE_ID(v);

				o.uv = v.texcoord;
                // 使用标准转换，因为矩阵已在 setup() 中设置
				o.pos = UnityObjectToClipPos(v.vertex);

                // 颜色逻辑
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				    float speed = length(Velocities[unity_InstanceID]);
				    float speedT = saturate(speed / velocityMax);
				    float colT = speedT;
				    o.colour = ColourMap.SampleLevel(linear_clamp_sampler, float2(colT, 0.5), 0);
                #else
                    o.colour = float3(1,1,1);
                #endif

				return o;
			}


			float4 frag (v2f i) : SV_Target
			{
				float2 centreOffset = (i.uv.xy - 0.5) * 2;
				float sqrDst = dot(centreOffset, centreOffset);
				float delta = fwidth(sqrt(sqrDst));
				float alpha = 1 - smoothstep(1 - delta, 1 + delta, sqrDst);

				float3 colour = i.colour;
				return float4(colour, alpha);
			}

			ENDCG
		}
	}
}