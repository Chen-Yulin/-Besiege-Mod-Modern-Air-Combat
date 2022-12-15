using UnityEngine;

namespace ModernAirCombat
{
    public class ThermalVision:MonoBehaviour
    {
		public bool ThermalOn = true;
		public bool IsInverse = true;

		public RenderTexture OtherTex;
		private Material material0;
		private Material material1;
		private Material material2;
		// Creates a private material used to the effect
		void Awake()
		{
			material0 = new Material(AssetManager.Instance.Shader.GrayShader);
			material1 = new Material(AssetManager.Instance.Shader.Thermal1Shader);
			material2 = new Material(AssetManager.Instance.Shader.Thermal2Shader);
		}

		// Postprocess the image
		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (ThermalOn)
			{
				if (IsInverse)
				{
					material1.SetTexture("_OtherTex", OtherTex);

					Graphics.Blit(source, destination, material1);
				}
				else
				{
					material2.SetTexture("_OtherTex", OtherTex);

					Graphics.Blit(source, destination, material2);
				}
			}
			else
			{
				material0.SetTexture("_OtherTex", OtherTex);

				Graphics.Blit(source, destination, material0);
			}

		}
	}


}
