using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using Unity.VisualScripting;
using UnityEngine;

using static Unity.Mathematics.math;
using float3x3 = Unity.Mathematics.float3x3;
using float4x4 = Unity.Mathematics.float4x4;
using quaternion = Unity.Mathematics.quaternion;

public class FractalJobs : MonoBehaviour
{
    // FloatPrecision: Precision of sin() and cos(), lower precision rise speed of quaternion instructor
    // FloatMode.Fast: reorder mathematical operations to use MADD
    // * MADD: multiply-add—instructions, which faster than separate ADD followed by MUL.
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)] 
    struct UpdateFractalLevelJob : IJobFor // Interface of "Job", using in the FOR loop
    {   
        public float spinAngleDelta;
        public float scale;
        
        public NativeArray<FractalPart> parts;
        [ReadOnly] public NativeArray<FractalPart>  parents;
        [WriteOnly]public NativeArray<float3x4>    matrices;
        public void Execute(int i)
        {   // replace the innermost loop of update()
            FractalPart parent = parents[i / 5];
            FractalPart part = parts[i];
                
            part.spinAngle += spinAngleDelta;
            part.worldRotation = 
                mul(parent.worldRotation, mul(part.rotation, quaternion.RotateY(part.spinAngle)));
            part.worldPosition = parent.worldPosition +
                mul(parent.worldRotation , 1.25f * scale * part.direction);
                
            parts[i] = part;
            
            float3x3 r = float3x3(part.worldRotation) * scale;
            matrices[i] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
        }
    }

    [SerializeField, Range(1, 9)] int depth;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    static float3[] _directions = {up(), right(), left(), forward(), back()};

    static quaternion[] _rotations =
        {
            quaternion.identity, 
            quaternion.RotateZ(-0.5f * PI), quaternion.RotateZ(0.5f * PI), 
            quaternion.RotateX(0.5f * PI),  quaternion.RotateX(-0.5f * PI),
        };

    struct FractalPart
    {
        public float3 direction, worldPosition;
        public quaternion rotation, worldRotation;
        public float spinAngle;
    }
    
    static readonly int matricesId = Shader.PropertyToID("_Matrices");
    static MaterialPropertyBlock propertyBlock;
    
    NativeArray<FractalPart>[] parts;
    NativeArray<float3x4>[] matrices;
    
    FractalPart CreatePart(int childIndex) => new FractalPart
        { direction = _directions[childIndex], rotation = _rotations[childIndex]};
    
    ComputeBuffer[] matricesBuffers;
    
    void OnEnable()
    {
        parts = new NativeArray<FractalPart>[depth];
        matrices = new NativeArray<float3x4>[depth];
        matricesBuffers = new ComputeBuffer[depth];

        int stride = 12 * 4; 
        for (int i = 0, length = 1; i < parts.Length; i++,  length *= 5)
        {
            parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
            matrices[i] = new NativeArray<float3x4>(length, Allocator.Persistent);
            matricesBuffers[i] = new ComputeBuffer(length, stride);
        }

        parts[0][0] =  CreatePart(0);
        for (int li = 1; li < parts.Length; li++)
        {
            NativeArray<FractalPart> levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi+=5)
                for (int ci = 0; ci < 5; ci++)
                    levelParts[fpi + ci] =  CreatePart(ci);
        }
        propertyBlock ??= new MaterialPropertyBlock();
    }

    void OnDisable()
    {
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            matricesBuffers[i].Release();
            parts[i].Dispose();
            matrices[i].Dispose();
        }
        parts = null;
        matrices = null;
        matricesBuffers = null;
    }

    void OnValidate()
    {
        if (parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }
    
    void Update()
    {
        float spinAngleDelta = 0.125f * PI * Time.deltaTime;
        FractalPart rootPart = parts[0][0];
        rootPart.spinAngle += spinAngleDelta;
        rootPart.worldRotation = mul(rootPart.rotation ,quaternion.RotateY(rootPart.spinAngle));
        
        parts[0][0] = rootPart;
        
        float objectScale = transform.lossyScale.x;
        
        float3x3 r = float3x3(rootPart.worldRotation) * objectScale;
        matrices[0][0] = float3x4(r.c0, r.c1, r.c2, rootPart.worldPosition);

        float scale = objectScale;
        JobHandle jHandle = default;
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            
            jHandle = new UpdateFractalLevelJob
            {   
                spinAngleDelta = spinAngleDelta,
                scale = scale,
                parents = parts[li - 1],
                parts = parts[li],
                matrices = matrices[li]
            }.ScheduleParallel(parts[li].Length, 5,jHandle);
            // Schedule(iteration count, sequence pattern)
            // 1.innermost FOR loop count 2.sequential dependency between jobs
        }
        // schedule first, invoke and execute after
        jHandle.Complete();

        var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            ComputeBuffer buffer = matricesBuffers[i];
            buffer.SetData(matrices[i]);
            propertyBlock.SetBuffer(matricesId, buffer);
            Graphics.DrawMeshInstancedProcedural
                (mesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }

    public void OnChangeDepth(float _depth)
    {
        depth = Mathf.RoundToInt(_depth);
        OnValidate();
    }
}
