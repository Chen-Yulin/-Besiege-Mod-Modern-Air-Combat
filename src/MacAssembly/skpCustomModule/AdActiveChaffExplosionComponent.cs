using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x0200003A RID: 58
	public class AdActiveChaffExplosionComponent : MonoBehaviour, IExplosionEffect
	{
		// Token: 0x06000136 RID: 310 RVA: 0x00016CDC File Offset: 0x00014EDC
		public void Update()
		{
			bool flag = !StatMaster.levelSimulating;
			if (!flag)
			{
				bool flag2 = this.projectileScript != null;
				if (flag2)
				{
					this.projectileScript = base.gameObject.GetComponent<AdProjectileScript>();
				}
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00016D1C File Offset: 0x00014F1C
		public bool OnExplode(float power, float upPower, float torquePower, Vector3 explosionPos, float radius, int mask)
		{
			bool flag = !StatMaster.levelSimulating;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.projectileScript = base.gameObject.GetComponent<AdProjectileScript>();
				bool flag2 = (mask & 64) != 0;
				if (flag2)
				{
					this.projectileScript.TrailPurge();
					this.projectileScript.Explode(base.transform.position, base.transform.rotation);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04000290 RID: 656
		public AdProjectileScript projectileScript;
	}
}
