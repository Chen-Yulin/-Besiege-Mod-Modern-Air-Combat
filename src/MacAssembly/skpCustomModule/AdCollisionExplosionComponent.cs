using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000039 RID: 57
	public class AdCollisionExplosionComponent : MonoBehaviour, IExplosionEffect
	{
		// Token: 0x06000133 RID: 307 RVA: 0x00016C28 File Offset: 0x00014E28
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

		// Token: 0x06000134 RID: 308 RVA: 0x00016C68 File Offset: 0x00014E68
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
				bool flag2 = (mask & 32) != 0;
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

		// Token: 0x0400028F RID: 655
		public AdProjectileScript projectileScript;
	}
}
