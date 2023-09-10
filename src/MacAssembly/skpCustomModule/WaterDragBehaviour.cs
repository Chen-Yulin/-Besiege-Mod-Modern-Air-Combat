using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x0200000E RID: 14
	public class WaterDragBehaviour : MonoBehaviour
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002267 File Offset: 0x00000467
		public bool Wenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterEnable;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002273 File Offset: 0x00000473
		public bool WBenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterBouncyEnable;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000043 RID: 67 RVA: 0x0000228B File Offset: 0x0000048B
		public float WaterHeight
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterHeight;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000022A5 File Offset: 0x000004A5
		public void Awake()
		{
			this.myRigibody = base.transform.GetComponent<Rigidbody>();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000AD88 File Offset: 0x00008F88
		public void FixedUpdate()
		{
			bool flag = !this.myRigibody;
			if (flag)
			{
				this.myRigibody = base.transform.GetComponent<Rigidbody>();
			}
			bool flag2 = this.myRigibody;
			if (flag2)
			{
				this.CenterOfMass = this.myRigibody.worldCenterOfMass;
				float num = this.CenterOfMass.y - 0.25f;
				bool flag3 = num < this.WaterHeight;
				if (flag3)
				{
					bool flag4 = this.isFlatXBlock;
					if (flag4)
					{
						Vector3 vector = Vector3.Project(this.myRigibody.velocity, base.transform.forward);
						float num2 = vector.magnitude * 2.5f;
						this.myRigibody.AddForce(-vector.normalized * num2);
					}
					bool flag5 = this.isFlatYBlock;
					if (flag5)
					{
						Vector3 vector2 = Vector3.Project(this.myRigibody.velocity, base.transform.up);
						float num3 = vector2.magnitude * 2.5f;
						this.myRigibody.AddForce(-vector2.normalized * num3);
					}
				}
			}
		}

		// Token: 0x040000EB RID: 235
		public Rigidbody myRigibody;

		// Token: 0x040000EC RID: 236
		public float cap = 500f;

		// Token: 0x040000ED RID: 237
		public float resistance = 10f;

		// Token: 0x040000EE RID: 238
		public float DelayTime = 0f;

		// Token: 0x040000EF RID: 239
		public bool isFlatXBlock = false;

		// Token: 0x040000F0 RID: 240
		public bool isFlatYBlock = false;

		// Token: 0x040000F1 RID: 241
		private Vector3 CenterOfMass;
	}
}
