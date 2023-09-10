using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x02000040 RID: 64
	public class LifeTimeP : MonoBehaviour
	{
		// Token: 0x06000151 RID: 337 RVA: 0x00002863 File Offset: 0x00000A63
		private void OnEnable()
		{
			base.StartCoroutine(this.ParticleWorking());
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00002873 File Offset: 0x00000A73
		private IEnumerator ParticleWorking()
		{
			ParticleSystem particle = base.GetComponentInChildren<ParticleSystem>();
			yield return new WaitWhile(() => particle.IsAlive(true));
			Object.Destroy(base.gameObject);
			yield break;
		}
	}
}
