Shader "Custom/VFGpu"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // instancing 01
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

			StructuredBuffer<float3> _Pos;
            float _Step;
            
            struct appdata
            {
                float4 vertex : POSITION;
                // instancing 02
                UNITY_VERTEX_INPUT_INSTANCE_ID
                uint insID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                // instancing 03
                // use this to access instanced properties in the fragment shader.
                UNITY_VERTEX_INPUT_INSTANCE_ID 
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _PosIns)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;
                
                // instancing 04 instance id prop in vert
                UNITY_SETUP_INSTANCE_ID(v);
                // instancing 05 pass instance id to frag
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float3 positionIns = _Pos[v.insID];

                float4x4 o2w= 0.0;
				o2w._m03_m13_m23_m33 = float4(positionIns, 1.0);
				o2w._m00_m11_m22 = _Step;
                
                o.vertex = mul(o2w,v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // instancing 06 instance id prop in frag
                UNITY_SETUP_INSTANCE_ID(i);
                
                // return saturate(i.vertex * 0.5 + 0.5);
                return i.vertex;
            }
            ENDCG
        }
    }
}