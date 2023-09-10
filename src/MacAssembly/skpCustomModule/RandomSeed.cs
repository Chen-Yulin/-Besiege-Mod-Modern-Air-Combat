using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace skpCustomModule
{
	// Token: 0x0200003B RID: 59
	public class RandomSeed : MonoBehaviour
	{
		// Token: 0x06000139 RID: 313 RVA: 0x00002191 File Offset: 0x00000391
		private void Start()
		{
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00002191 File Offset: 0x00000391
		private void Update()
		{
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00016D90 File Offset: 0x00014F90
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
				foreach (ParticleSystem particleSystem2 in this.particle)
				{
					particleSystem2.Play();
				}
				base.StartCoroutine(this.ParticleWorking());
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000027F3 File Offset: 0x000009F3
		private IEnumerator ParticleWorking()
		{
			ParticleSystem particle1 = base.GetComponentInChildren<ParticleSystem>();
			yield return new WaitWhile(() => particle1.IsAlive(true));
			base.gameObject.SetActive(false);
			yield break;
		}

		// Token: 0x04000291 RID: 657
		public ParticleSystem[] particle;
	}
}
