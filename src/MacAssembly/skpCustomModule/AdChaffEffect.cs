using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000055 RID: 85
	public class AdChaffEffect : SimBehaviour
	{
		// Token: 0x060001DB RID: 475 RVA: 0x00002C01 File Offset: 0x00000E01
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00002C0B File Offset: 0x00000E0B
		public void OnEnable()
		{
			this.Chaffing = true;
			this.ChaffingStart();
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0001F564 File Offset: 0x0001D764
		public void FixedUpdate()
		{
			bool flag = !base.SimPhysics;
			if (!flag)
			{
				bool chaffing = this.Chaffing;
				if (chaffing)
				{
					this.explosionPos = base.transform.position;
					this.hitColliders = Physics.OverlapSphere(this.explosionPos, this.radius);
					Collider[] array = this.hitColliders;
					foreach (Collider collider in array)
					{
						bool flag2 = collider.attachedRigidbody && !this.prevRigidbodies.Contains(collider.attachedRigidbody) && collider.attachedRigidbody.gameObject.layer != 20 && collider.attachedRigidbody.gameObject.layer != 22 && collider.attachedRigidbody.tag != "KeepConstraintsAlways";
						if (flag2)
						{
							this.colAttachedRigidbody = collider.attachedRigidbody;
							this.colAttachedRigidbody.WakeUp();
							this.colAttachedRigidbody.constraints = 0;
							this.prevRigidbodies.Add(this.colAttachedRigidbody);
							int num = 64;
							IExplosionEffect[] interfaces = (IExplosionEffect[])ReferenceMaster.GetInterfaces<IExplosionEffect>(this.colAttachedRigidbody.gameObject);
							IExplosionEffect[] array3 = interfaces;
							foreach (IExplosionEffect explosionEffect in array3)
							{
								explosionEffect.OnExplode(this.power, this.upPower, 0f, this.explosionPos, this.radius, num);
							}
						}
					}
				}
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00002C1C File Offset: 0x00000E1C
		private void ChaffingStart()
		{
			base.StartCoroutine(this.ExplodeMessage());
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00002C2C File Offset: 0x00000E2C
		public IEnumerator ExplodeMessage()
		{
			yield return new WaitForSeconds(this.lifetime);
			base.gameObject.SetActive(false);
			this.Chaffing = false;
			yield break;
		}

		// Token: 0x0400040A RID: 1034
		public float radius = 5f;

		// Token: 0x0400040B RID: 1035
		public float power = 0f;

		// Token: 0x0400040C RID: 1036
		public float upPower = 0f;

		// Token: 0x0400040D RID: 1037
		public float lifetime = 1f;

		// Token: 0x0400040E RID: 1038
		public string BlockName;

		// Token: 0x0400040F RID: 1039
		public bool Chaffing = true;

		// Token: 0x04000410 RID: 1040
		private Vector3 explosionPos;

		// Token: 0x04000411 RID: 1041
		private Collider[] hitColliders;

		// Token: 0x04000412 RID: 1042
		private Rigidbody colAttachedRigidbody;

		// Token: 0x04000413 RID: 1043
		private List<Rigidbody> prevRigidbodies = new List<Rigidbody>();
	}
}
