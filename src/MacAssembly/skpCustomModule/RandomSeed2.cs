using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace skpCustomModule
{
	// Token: 0x0200003E RID: 62
	public class RandomSeed2 : MonoBehaviour
	{
		// Token: 0x06000146 RID: 326 RVA: 0x00002191 File Offset: 0x00000391
		private void Start()
		{
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00002191 File Offset: 0x00000391
		private void Update()
		{
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00016EB8 File Offset: 0x000150B8
		private void OnEnable()
		{
			this.particle = base.GetComponentsInChildren<ParticleSystem>();
			bool flag = this.particle.Length != 0;
			if (flag)
			{
				foreach (ParticleSystem particleSystem in this.particle)
				{
					particleSystem.randomSeed = (uint)Random.Range(0f, 1000f);
				}
			}
		}

		// Token: 0x04000297 RID: 663
		public ParticleSystem[] particle;
	}
}
