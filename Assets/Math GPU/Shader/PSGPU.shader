Shader "Custom/PSGPU"
{
    Properties
    {
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        CGPROGRAM
        
        // 1. 保持 Standard 光照模型
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma target 4.5

        // 2. 保持你原来的 Input 结构 
        struct Input
        {
            float3 worldPos;
        };
        
        float _Step;

        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			StructuredBuffer<float3> _Pos;
		#endif

        // 3. 使用我第一个回复中的 "修复方案 #1" (设置两个矩阵)
        void ConfigureProcedural ()
        {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float3 position = _Pos[unity_InstanceID];
                
                float rcpStep = 1.0 / _Step;

                unity_ObjectToWorld = float4x4(
                    _Step, 0, 0, position.x,
                    0, _Step, 0, position.y,
                    0, 0, _Step, position.z,
                    0, 0, 0, 1
                );
                
                // 这是修复光照的关键
                unity_WorldToObject = float4x4(
                    rcpStep, 0, 0, -position.x * rcpStep,
                    0, rcpStep, 0, -position.y * rcpStep,
                    0, 0, rcpStep, -position.z * rcpStep,
                    0, 0, 0, 1
                );
            #endif
        }
        
        // 4. [诊断测试] 将颜色输出到 Emission (自发光)
        // (注意：我们又回到了 inout SurfaceOutputStandard)
        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
        {
            // 将颜色输出到 Emission。自发光不受光照影响。
            surface.Emission = saturate(input.worldPos * 0.5 + 0.5);
            
            // 将其他光照参数设为0，确保我们只看到自发光
            surface.Albedo = float3(0, 0, 0); 
            surface.Smoothness = 0.0;
            surface.Metallic = 0.0;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}