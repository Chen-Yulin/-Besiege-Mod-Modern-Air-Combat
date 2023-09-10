using System;
using System.Collections;
using System.Collections.Generic;
using Modding.Blocks;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;
namespace skpCustomModule
{
	// Token: 0x0200001A RID: 26
	public class TriggerSetSubVis : TriggerSetJointBase
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00002441 File Offset: 0x00000641
		private void Start()
		{
			this.isInitialized = true;
			this.blockType = this.block.Prefab.Type;
			this.canParentBlock = true;
			this.SkipCount = 0;
			base.StartCoroutine(this.CheckAllJoints());
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000FD9C File Offset: 0x0000DF9C
		private void FixedUpdate()
		{
			bool flag = this.setSubVis;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			bool flag2 = this.SkipCount < 1;
			if (flag2)
			{
				this.SkipCount++;
			}
			else
			{
				bool isSimulating = this.block.isSimulating;
				if (isSimulating)
				{
					bool flag3 = this.colliders != null;
					if (flag3)
					{
						for (int i = 0; i < this.colliders.Count; i++)
						{
							Collider collider = this.colliders[i];
							bool flag4 = collider != null;
							if (flag4)
							{
								this.SetSubVistoparent(collider);
							}
						}
					}
				}
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000FE50 File Offset: 0x0000E050
		private void OnTriggerEnter(Collider other)
		{
			bool isSimulating = this.block.isSimulating;
			if (isSimulating)
			{
				bool flag = this.colliders == null;
				if (flag)
				{
					this.colliders = new List<Collider>();
				}
				bool flag2 = false;
				for (int i = 0; i < this.colliders.Count; i++)
				{
					Collider collider = this.colliders[i];
					bool flag3 = collider == other;
					if (flag3)
					{
						flag2 = true;
					}
				}
				bool flag4 = !flag2;
				if (flag4)
				{
					this.colliders.Add(other);
				}
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000FEE4 File Offset: 0x0000E0E4
		private void SetSubVistoparent(Collider other)
		{
			GameObject gameObject = other.gameObject;
			int layer = gameObject.layer;
			Transform parent = other.transform.parent;
			bool flag = other.name == "StartingBlock";
			if (flag)
			{
				parent = other.transform;
			}
			bool flag2 = (layer == 12 || layer == 14) && !this.setSubVis;
			if (flag2)
			{
				this.parentToJoinTo = parent;
				bool flag3 = this.parentToJoinTo != null;
				if (flag3)
				{
					this.SubVis.transform.SetParent(parent);
				}
				this.setSubVis = true;
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000247C File Offset: 0x0000067C
		private IEnumerator CheckAllJoints()
		{
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			Object.Destroy(base.gameObject);
			yield break;
		}

		// Token: 0x0400015C RID: 348
		public BlockBehaviour block;

		// Token: 0x0400015D RID: 349
		public PlayerMachine machine;

		// Token: 0x0400015E RID: 350
		public GameObject SubVis;

		// Token: 0x0400015F RID: 351
		private bool setSubVis = false;

		// Token: 0x04000160 RID: 352
		private bool otherMechJoint;

		// Token: 0x04000161 RID: 353
		private Collider colliderToJoinTo;

		// Token: 0x04000162 RID: 354
		private Transform parentToJoinTo;

		// Token: 0x04000163 RID: 355
		private bool canParentBlock;

		// Token: 0x04000164 RID: 356
		private bool mechJointTag;

		// Token: 0x04000165 RID: 357
		private BlockType blockType;

		// Token: 0x04000166 RID: 358
		private float prevTimeSliderValue;

		// Token: 0x04000167 RID: 359
		private bool isInitialized;

		// Token: 0x04000168 RID: 360
		private List<Transform> otherMechJointsParents;

		// Token: 0x04000169 RID: 361
		private List<Collider> colliders;

		// Token: 0x0400016A RID: 362
		private int SkipCount = 0;
	}
}
