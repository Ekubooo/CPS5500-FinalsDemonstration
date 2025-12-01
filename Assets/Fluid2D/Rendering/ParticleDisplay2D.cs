using Seb.Fluid2D.Simulation;
using Seb.Helpers;
using UnityEngine;

namespace Seb.Fluid2D.Rendering
{
	public class ParticleDisplay2D : MonoBehaviour
	{
		public FluidSim2D sim;
		public Mesh mesh;
		public Shader shader;
		public float scale;
		public Gradient colourMap;
		public int gradientResolution;
		public float velocityDisplayMax;

		Material material;
		ComputeBuffer argsBuffer;
		Bounds bounds;
		Texture2D gradientTexture;
		bool needsUpdate;

		void Start()
		{
			material = new Material(shader);
            // 关键修复：在开始运行时强制标记为需要更新，确保纹理和Scale被传递给Shader
            needsUpdate = true;
		}

		void LateUpdate()
		{
			if (shader != null)
			{
				UpdateSettings();
				Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
			}
		}

		void UpdateSettings()
		{
			material.SetBuffer("Positions2D", sim.positionBuffer);
			material.SetBuffer("Velocities", sim.velocityBuffer);
			material.SetBuffer("DensityData", sim.densityBuffer);

			ComputeHelper.CreateArgsBuffer(ref argsBuffer, mesh, sim.positionBuffer.count);
			bounds = new Bounds(Vector3.zero, Vector3.one * 10000);

            // 逻辑修改：如果纹理不存在，或者标记为需要更新，则重新生成纹理
			if (needsUpdate || gradientTexture == null)
			{
				needsUpdate = false;
				TextureFromGradient(ref gradientTexture, gradientResolution, colourMap);
				material.SetTexture("ColourMap", gradientTexture);
			}

            // 关键修复：将这些轻量级 float 参数移出 if 块
            // 或者你也可以保留在 if 块里，但必须保证 Start() 里 needsUpdate = true
            // 为了像 PEDemo 一样稳健，建议每帧设置，防止材质丢失状态
            material.SetFloat("scale", scale);
            material.SetFloat("velocityMax", velocityDisplayMax);
		}

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
			ComputeHelper.Release(argsBuffer);
		}
	}
}