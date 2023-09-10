using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x0200000F RID: 15
	public class ModuleProjectileBehavior : MonoBehaviour
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000022F1 File Offset: 0x000004F1
		private bool ContinuousDynamicFlag
		{
			get
			{
				return AdCustomModuleMod.mod4.ModuleCollisionModeSwitch_CD;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000AEB8 File Offset: 0x000090B8
		public void OnEnable()
		{
			bool levelSimulating = StatMaster.levelSimulating;
			if (levelSimulating)
			{
				bool continuousDynamicFlag = this.ContinuousDynamicFlag;
				if (continuousDynamicFlag)
				{
					bool flag = base.gameObject.GetComponent<Rigidbody>();
					if (flag)
					{
						this.BlockRigidbody = base.gameObject.GetComponent<Rigidbody>();
						this.BlockRigidbody.collisionDetectionMode = (CollisionDetectionMode)2;
					}
				}
				else
				{
					bool flag2 = base.gameObject.GetComponent<Rigidbody>();
					if (flag2)
					{
						this.BlockRigidbody = base.gameObject.GetComponent<Rigidbody>();
						this.BlockRigidbody.collisionDetectionMode = 0;
					}
				}
				this.changeFlag = this.ContinuousDynamicFlag;
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000AF58 File Offset: 0x00009158
		public void FixedUpadate()
		{
			bool levelSimulating = StatMaster.levelSimulating;
			if (levelSimulating)
			{
				bool flag = this.changeFlag != this.ContinuousDynamicFlag;
				if (flag)
				{
					bool continuousDynamicFlag = this.ContinuousDynamicFlag;
					if (continuousDynamicFlag)
					{
						bool flag2 = base.gameObject.GetComponent<Rigidbody>();
						if (flag2)
						{
							this.BlockRigidbody = base.gameObject.GetComponent<Rigidbody>();
							this.BlockRigidbody.collisionDetectionMode = (CollisionDetectionMode)2;
						}
					}
					else
					{
						bool flag3 = base.gameObject.GetComponent<Rigidbody>();
						if (flag3)
						{
							this.BlockRigidbody = base.gameObject.GetComponent<Rigidbody>();
							this.BlockRigidbody.collisionDetectionMode = 0;
						}
					}
					this.changeFlag = this.ContinuousDynamicFlag;
				}
			}
		}

		// Token: 0x040000F2 RID: 242
		public Rigidbody BlockRigidbody;

		// Token: 0x040000F3 RID: 243
		private bool changeFlag = false;
	}
}
