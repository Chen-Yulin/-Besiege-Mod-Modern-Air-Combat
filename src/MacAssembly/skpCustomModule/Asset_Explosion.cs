using System;
using Modding;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000032 RID: 50
	public class Asset_Explosion
	{
		// Token: 0x060000FC RID: 252 RVA: 0x000026EE File Offset: 0x000008EE
		public Asset_Explosion(ModAssetBundle modAssetBundle)
		{
			this.explosionEffect = modAssetBundle.LoadAsset<GameObject>("ExplodePrefab");
			this.explosionEffect.AddComponent<AdExplosionEffect>();
		}

		// Token: 0x04000239 RID: 569
		public GameObject explosionEffect;
	}
}
