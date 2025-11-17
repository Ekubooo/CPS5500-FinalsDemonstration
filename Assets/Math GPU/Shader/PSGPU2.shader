Shader "Custom/PSGPU2"
{
    Properties
    {
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        // 添加一个属性来控制“保底”亮度
        _MinBrightness ("Min Brightness (Emission)", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        CGPROGRAM
        
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma target 4.5

        struct Input
        {
            float3 worldPos;
        };
        
        float _Step;
        float _MinBrightness; // 对应上面 Properties 中的属性

        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			StructuredBuffer<float3> _Pos;
		#endif

        // 我们使用在 "发光" 测试中被证明可以正确计算 worldPos 的矩阵
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
                
                unity_WorldToObject = float4x4(
                    rcpStep, 0, 0, -position.x * rcpStep,
                    0, rcpStep, 0, -position.y * rcpStep,
                    0, 0, rcpStep, -position.z * rcpStep,
                    0, 0, 0, 1
                );
            #endif
        }
        
        // [混合方案]
        void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
        {
            // 1. 计算出基础颜色
            float3 worldColor = saturate(input.worldPos * 0.5 + 0.5);

            // 2. 将它同时赋给 Albedo (用于接收光照)
            surface.Albedo = worldColor;

            // 3. 将它的一部分赋给 Emission (用于"保底"，防止变黑)
            // _MinBrightness 可以在材质面板上调节，建议 0.1 或 0.2
            surface.Emission = worldColor * _MinBrightness;

            // 4. 设置PBR属性
            surface.Smoothness = 0.5; // 你原始的值
            surface.Metallic = 0.0;   // 确保它不是金属
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}