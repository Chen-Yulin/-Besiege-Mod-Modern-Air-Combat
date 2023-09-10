using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000010 RID: 16
	public class WaterProjectileBehaviour : MonoBehaviour
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002267 File Offset: 0x00000467
		public bool Wenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterEnable;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002273 File Offset: 0x00000473
		public bool WBenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterBouncyEnable;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600004D RID: 77 RVA: 0x0000228B File Offset: 0x0000048B
		public float WaterHeight
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterHeight;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000B014 File Offset: 0x00009214
		public void Awake()
		{
			bool flag = base.gameObject.GetComponent<Rigidbody>();
			if (flag)
			{
				this.BlockRigidbody = base.gameObject.GetComponent<Rigidbody>();
				this.origin_angularDrag = this.BlockRigidbody.angularDrag;
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000B05C File Offset: 0x0000925C
		public void OnEnable()
		{
			bool levelSimulating = StatMaster.levelSimulating;
			if (levelSimulating)
			{
				bool flag = this.thisAdprojectile;
				if (flag)
				{
					bool flag2 = base.gameObject.GetComponent<AdProjectileScript>();
					if (flag2)
					{
						AdProjectileScript component = base.gameObject.GetComponent<AdProjectileScript>();
						this.Blocktransform = component.gyro;
						this.CenterOfMass = this.Blocktransform.position;
						this.enable = true;
					}
				}
				bool flag3 = base.gameObject.GetComponent<Rigidbody>();
				if (flag3)
				{
					this.BlockRigidbody = base.gameObject.GetComponent<Rigidbody>();
					this.CenterOfMass = this.BlockRigidbody.worldCenterOfMass;
					this.attachRigidbody = true;
					this.enable = true;
				}
				else
				{
					this.Collidertransform = base.gameObject.GetComponentInChildren<Collider>().transform;
					this.CenterOfMass = this.Blocktransform.position;
					this.attachRigidbody = false;
					this.enable = true;
				}
				bool flag4 = this.enable;
				if (flag4)
				{
					float num = this.CenterOfMass.y - 0.25f;
					bool flag5 = num < this.WaterHeight;
					if (flag5)
					{
						this.IntoWater = true;
					}
					else
					{
						this.IntoWater = false;
					}
				}
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x0000B190 File Offset: 0x00009390
		public void FixedUpdate()
		{
			bool flag = StatMaster.isMP || StatMaster.isLocalSim;
			bool flag2 = this.Wenable && this.WBenable && StatMaster.levelSimulating && this.enable;
			if (flag2)
			{
				bool flag3 = this.thisAdprojectile;
				if (flag3)
				{
					this.CenterOfMass = this.Blocktransform.position;
				}
				else
				{
					bool flag4 = this.attachRigidbody;
					if (flag4)
					{
						this.CenterOfMass = this.BlockRigidbody.worldCenterOfMass;
					}
					else
					{
						this.CenterOfMass = this.Collidertransform.position;
					}
				}
				float num = this.CenterOfMass.y - 0.25f;
				float num2 = this.CenterOfMass.y - 0.5f;
				float num3 = this.floating * 50f;
				bool flag5 = num < this.WaterHeight;
				if (flag5)
				{
					bool flag6 = this.attachRigidbody;
					if (flag6)
					{
						float num4 = Mathf.Clamp01(this.WaterHeight - num);
						this.BlockRigidbody.AddForce(Vector3.up * num3 * num4);
						float magnitude = this.BlockRigidbody.velocity.magnitude;
						Vector3 vector = -this.BlockRigidbody.velocity.normalized;
						vector *= magnitude;
						vector *= this.drag;
						vector *= num4;
						this.BlockRigidbody.velocity += vector;
					}
					bool flag7 = !this.IntoWater;
					if (flag7)
					{
						Transform transform = AdCustomModuleMod.mod6.ProjectileWaterEffectList[AdCustomModuleMod.mod6.ProjectileEffectCount];
						transform.position = new Vector3(this.CenterOfMass.x, this.WaterHeight, this.CenterOfMass.z);
						transform.gameObject.SetActive(true);
						this.IntoWater = true;
						bool flag8 = AdCustomModuleMod.mod6.ProjectileEffectCount < AdCustomModuleMod.mod6.MAX_Effect_POOL_SIZE;
						if (flag8)
						{
							AdCustomModuleMod.mod6.ProjectileEffectCount++;
						}
						else
						{
							AdCustomModuleMod.mod6.ProjectileEffectCount = 0;
						}
					}
				}
				else
				{
					this.IntoWater = false;
				}
				bool flag9 = this.attachRigidbody;
				if (flag9)
				{
					bool flag10 = num2 < this.WaterHeight;
					if (flag10)
					{
						float num5 = Mathf.Clamp01(this.WaterHeight - num2);
						this.BlockRigidbody.angularDrag = 3f * num5;
					}
					else
					{
						this.BlockRigidbody.angularDrag = this.origin_angularDrag;
					}
				}
			}
		}

		// Token: 0x040000F4 RID: 244
		public Rigidbody BlockRigidbody;

		// Token: 0x040000F5 RID: 245
		public Collider BlockCollider;

		// Token: 0x040000F6 RID: 246
		public Transform Blocktransform;

		// Token: 0x040000F7 RID: 247
		public Transform Collidertransform;

		// Token: 0x040000F8 RID: 248
		private float cap = 500f;

		// Token: 0x040000F9 RID: 249
		public float resistance = 10f;

		// Token: 0x040000FA RID: 250
		public float DelayTime = 0f;

		// Token: 0x040000FB RID: 251
		public bool isFlatXBlock = false;

		// Token: 0x040000FC RID: 252
		public bool isFlatYBlock = false;

		// Token: 0x040000FD RID: 253
		private Vector3 CenterOfMass;

		// Token: 0x040000FE RID: 254
		private float drag = 0.05f;

		// Token: 0x040000FF RID: 255
		private float origin_angularDrag;

		// Token: 0x04000100 RID: 256
		private bool IntoWater = false;

		// Token: 0x04000101 RID: 257
		private bool attachRigidbody = false;

		// Token: 0x04000102 RID: 258
		private bool enable = false;

		// Token: 0x04000103 RID: 259
		public float floating = 0f;

		// Token: 0x04000104 RID: 260
		public bool thisAdprojectile = false;
	}
}
