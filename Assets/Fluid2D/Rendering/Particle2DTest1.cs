using Seb.Fluid2D.Simulation;
using Seb.Helpers;
using UnityEngine;

namespace Seb.Fluid2D.Rendering
{
    public class Particle2DTest1 : MonoBehaviour
    {
        public FluidSim2D sim;
        public Mesh mesh;
        public Shader shader;
        public float scale;
        public Gradient colourMap;
        public int gradientResolution;
        public float velocityDisplayMax;

        Material material;
        // [修改 1] 将 ComputeBuffer 改为 GraphicsBuffer
        GraphicsBuffer argsBuffer; 
        Bounds bounds;
        Texture2D gradientTexture;
        bool needsUpdate;

        void Start()
        {
            material = new Material(shader);
        }

        void LateUpdate()
        {
            if (shader != null)
            {
                UpdateSettings();
                // Unity 6 会自动处理 GraphicsBuffer 到绘制命令的转换
                Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
            }
        }

        void UpdateSettings()
        {
            // 这里 sim.positionBuffer 依然是 ComputeBuffer，这是可以的，只有 argsBuffer 必须严格是 GraphicsBuffer
            material.SetBuffer("Positions2D", sim.positionBuffer);
            material.SetBuffer("Velocities", sim.velocityBuffer);
            material.SetBuffer("DensityData", sim.densityBuffer);

            // [修改 2] 手动创建 GraphicsBuffer，替代原本的 ComputeHelper.CreateArgsBuffer
            int instanceCount = sim.positionBuffer.count;
            if (argsBuffer == null || !argsBuffer.IsValid() || argsBuffer.count != 5) 
            {
                if (argsBuffer != null) argsBuffer.Release();
                // 关键：必须指定 Target.IndirectArguments
                argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 5, sizeof(uint));
            }

            // 设置绘制参数：[0]索引数, [1]实例数, [2]索引开始, [3]顶点开始, [4]实例开始
            var args = new uint[5] { 0, 0, 0, 0, 0 };
            args[0] = (uint)mesh.GetIndexCount(0);
            args[1] = (uint)instanceCount; // 粒子数量
            args[2] = (uint)mesh.GetIndexStart(0);
            args[3] = (uint)mesh.GetBaseVertex(0);
            
            argsBuffer.SetData(args);

            bounds = new Bounds(Vector3.zero, Vector3.one * 10000);

            if (needsUpdate)
            {
                needsUpdate = false;
                TextureFromGradient(ref gradientTexture, gradientResolution, colourMap);
                material.SetTexture("ColourMap", gradientTexture);

                material.SetFloat("scale", scale);
                material.SetFloat("velocityMax", velocityDisplayMax);
            }
        }

        // ... TextureFromGradient 方法保持不变 ...

        public static void TextureFromGradient(ref Texture2D texture, int width, Gradient gradient, FilterMode filterMode = FilterMode.Bilinear)
        {
            if (texture == null)
            {
                texture = new Texture2D(width, 1);
            }
            else if (texture.width != width)
            {
                texture.Reinitialize(width, 1);
            }

            if (gradient == null)
            {
                gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.black, 1) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
                );
            }

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = filterMode;

            Color[] cols = new Color[width];
            for (int i = 0; i < cols.Length; i++)
            {
                float t = i / (cols.Length - 1f);
                cols[i] = gradient.Evaluate(t);
            }

            texture.SetPixels(cols);
            texture.Apply();
        }

        void OnValidate()
        {
            needsUpdate = true;
        }

        void OnDestroy()
        {
            // [修改 3] 使用标准的 Release，ComputeHelper 可能不支持 GraphicsBuffer
            if (argsBuffer != null) argsBuffer.Release();
        }
    }
}