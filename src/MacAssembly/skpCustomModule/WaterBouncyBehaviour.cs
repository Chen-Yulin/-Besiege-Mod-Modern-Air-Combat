using System;
using System.Collections.Generic;
using Modding.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x0200000D RID: 13
	public class WaterBouncyBehaviour : MonoBehaviour
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002267 File Offset: 0x00000467
		private bool Wenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterEnable;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002273 File Offset: 0x00000473
		private bool WBenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterBouncyEnable;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003C RID: 60 RVA: 0x0000228B File Offset: 0x0000048B
		private float WaterHeight
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterHeight;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000A500 File Offset: 0x00008700
		public void Awake()
		{
			bool flag = base.transform.GetComponent<Rigidbody>();
			if (flag)
			{
				this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
				this.origin_angularDrag = this.BlockRigidbody.angularDrag;
				bool flag2 = this.ObjectBehavior == null;
				if (flag2)
				{
					this.ObjectBehavior = base.gameObject.GetComponent<BlockBehaviour>();
				}
				bool flag3 = WaterBouncyBehaviour.block_buoyancy.ContainsKey((BlockType)this.ObjectBehavior.BlockID);
				if (flag3)
				{
					this.floating = WaterBouncyBehaviour.block_buoyancy[(BlockType)this.ObjectBehavior.BlockID];
				}
				else
				{
					this.floating = 1f;
				}
				bool flag4 = WaterBouncyBehaviour.FlatXBlockList.Contains((BlockType)this.ObjectBehavior.BlockID);
				if (flag4)
				{
					this.isFlatXBlock = true;
				}
				bool flag5 = WaterBouncyBehaviour.FlatYBlockList.Contains((BlockType)this.ObjectBehavior.BlockID);
				if (flag5)
				{
					this.isFlatYBlock = true;
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000A5F8 File Offset: 0x000087F8
		public void FixedUpdate()
		{
			bool flag = !StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim || (StatMaster.isClient && StatMaster.InLocalPlayMode);
			bool flag2 = this.Wenable && this.WBenable && (StatMaster.levelSimulating && flag);
			if (flag2)
			{
				bool flag3 = !this.BlockRigidbody;
				if (flag3)
				{
					this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
				}
				bool flag4 = this.BlockRigidbody;
				if (flag4)
				{
					this.CenterOfComponent = this.BlockRigidbody.worldCenterOfMass;
					float num = this.CenterOfComponent.y - 0.25f;
					float num2 = this.CenterOfComponent.y - 0.5f;
					float num3 = this.floating * 50f;
					bool flag5 = num < this.WaterHeight;
					if (flag5)
					{
						float num4 = Mathf.Clamp01(this.WaterHeight - num);
						this.BlockRigidbody.AddForce(Vector3.up * num3 * num4);
						bool flag6 = this.isFlatXBlock;
						if (flag6)
						{
							Vector3 vector = Vector3.Project(this.BlockRigidbody.velocity, base.transform.forward);
							float num5 = vector.magnitude * 3.5f + vector.sqrMagnitude * 0.05f;
							this.BlockRigidbody.AddForce(-vector.normalized * num5);
						}
						bool flag7 = this.isFlatYBlock;
						if (flag7)
						{
							Vector3 vector2 = Vector3.Project(this.BlockRigidbody.velocity, base.transform.up);
							float num6 = vector2.magnitude * 3.5f + vector2.sqrMagnitude * 0.05f;
							this.BlockRigidbody.AddForce(-vector2.normalized * num6);
						}
						bool waterBouncyDebug = AdCustomModuleMod.mod4.WaterBouncyDebug;
						if (waterBouncyDebug)
						{
							bool flag8 = !base.transform.GetComponent<LineRenderer>();
							if (flag8)
							{
								this.lineRenderer = base.gameObject.AddComponent<LineRenderer>();
								Material material = new Material(Shader.Find("Unlit/Color"));
								material.SetVector("_Color", Color.red);
								this.lineRenderer.material = material;
								this.debugline = true;
							}
							this.lineRenderer.SetVertexCount(2);
							this.lineRenderer.SetWidth(0.2f, 0.2f);
							float num7 = num3 * num4 / 50f;
							Vector3[] positions = new Vector3[]
							{
								this.CenterOfComponent - Vector3.up * num7,
								this.CenterOfComponent
							};
							this.lineRenderer.SetPositions(positions);
						}
						else
						{
							bool flag9 = this.debugline;
							if (flag9)
							{
								bool flag10 = base.transform.GetComponent<LineRenderer>();
								if (flag10)
								{
									this.debugline = false;
									this.lineRenderer = base.transform.GetComponent<LineRenderer>();
									Object.Destroy(this.lineRenderer);
								}
							}
						}
					}
					else
					{
						bool flag11 = this.debugline;
						if (flag11)
						{
							bool flag12 = base.transform.GetComponent<LineRenderer>();
							if (flag12)
							{
								this.debugline = false;
								this.lineRenderer = base.transform.GetComponent<LineRenderer>();
								Object.Destroy(this.lineRenderer);
							}
						}
					}
					bool flag13 = num2 < this.WaterHeight;
					if (flag13)
					{
						float num8 = Mathf.Clamp01(this.WaterHeight - num2);
						this.BlockRigidbody.angularDrag = 3f * num8;
					}
					else
					{
						this.BlockRigidbody.angularDrag = this.origin_angularDrag;
					}
				}
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000AA0C File Offset: 0x00008C0C
		// Note: this type is marked as 'beforefieldinit'.
		static WaterBouncyBehaviour()
		{
			Dictionary<BlockType, float> dictionary = new Dictionary<BlockType, float>();
			dictionary[(BlockType)43] = 6f;
			dictionary[(BlockType)63] = 1.5f;
			dictionary[(BlockType)1] = 1.5f;
			dictionary[(BlockType)15] = 1.5f;
			dictionary[(BlockType)2] = 1.5f;
			dictionary[(BlockType)40] = 1.5f;
			dictionary[(BlockType)46] = 1.5f;
			dictionary[(BlockType)60] = 1.5f;
			dictionary[(BlockType)39] = 1.25f;
			dictionary[(BlockType)38] = 1.25f;
			dictionary[(BlockType)51] = 1.25f;
			dictionary[(BlockType)10] = 1f;
			dictionary[(BlockType)41] = 1f;
			dictionary[(BlockType)26] = 1.25f;
			dictionary[(BlockType)55] = 1f;
			dictionary[(BlockType)25] = 0.3f;
			dictionary[(BlockType)34] = 1f;
			dictionary[(BlockType)14] = 0f;
			dictionary[(BlockType)13] = 0f;
			dictionary[(BlockType)28] = 0f;
			dictionary[(BlockType)19] = 0f;
			dictionary[(BlockType)5] = 0f;
			dictionary[(BlockType)22] = 0f;
			dictionary[(BlockType)16] = 0f;
			dictionary[(BlockType)18] = 0f;
			dictionary[(BlockType)44] = 0f;
			dictionary[(BlockType)18] = 0f;
			dictionary[(BlockType)4] = 0f;
			dictionary[(BlockType)27] = 0f;
			dictionary[(BlockType)9] = 0f;
			dictionary[(BlockType)20] = 0f;
			dictionary[(BlockType)3] = 0f;
			dictionary[(BlockType)17] = 0f;
			dictionary[(BlockType)48] = 0f;
			dictionary[(BlockType)11] = 0f;
			dictionary[(BlockType)53] = 0f;
			dictionary[(BlockType)62] = 0f;
			dictionary[(BlockType)56] = 0f;
			dictionary[(BlockType)47] = 0f;
			dictionary[(BlockType)23] = 0f;
			dictionary[(BlockType)54] = 0f;
			dictionary[(BlockType)31] = 0f;
			dictionary[(BlockType)36] = 0f;
			dictionary[(BlockType)35] = 0f;
			dictionary[(BlockType)32] = 0f;
			dictionary[(BlockType)24] = 0f;
			dictionary[(BlockType)29] = 0f;
			dictionary[(BlockType)33] = 0f;
			dictionary[(BlockType)37] = 0f;
			dictionary[(BlockType)30] = 0f;
			dictionary[(BlockType)6] = 0f;
			dictionary[(BlockType)65] = 0f;
			dictionary[(BlockType)66] = 0f;
			dictionary[(BlockType)67] = 0f;
			dictionary[(BlockType)68] = 0f;
			dictionary[(BlockType)69] = 0f;
			dictionary[(BlockType)70] = 0f;
			dictionary[(BlockType)7] = 0f;
			WaterBouncyBehaviour.block_buoyancy = dictionary;
			WaterBouncyBehaviour.FlatXBlockList = new List<BlockType> { (BlockType)10, (BlockType)32, (BlockType)24, (BlockType)29 };
			WaterBouncyBehaviour.FlatYBlockList = new List<BlockType> { (BlockType)3 };
		}

		// Token: 0x040000D8 RID: 216
		private static readonly Dictionary<BlockType, float> block_buoyancy;

		// Token: 0x040000D9 RID: 217
		private static readonly List<BlockType> FlatXBlockList;

		// Token: 0x040000DA RID: 218
		private static readonly List<BlockType> FlatYBlockList;

		// Token: 0x040000DB RID: 219
		public BlockBehaviour ObjectBehavior;

		// Token: 0x040000DC RID: 220
		public Rigidbody BlockRigidbody;

		// Token: 0x040000DD RID: 221
		public Rigidbody HostRigidbody;

		// Token: 0x040000DE RID: 222
		public Vector3 CenterOfComponent;

		// Token: 0x040000DF RID: 223
		private LineRenderer lineRenderer;

		// Token: 0x040000E0 RID: 224
		private float origin_angularDrag;

		// Token: 0x040000E1 RID: 225
		private bool isFlatXBlock = false;

		// Token: 0x040000E2 RID: 226
		private bool isFlatYBlock = false;

		// Token: 0x040000E3 RID: 227
		private bool debugline = false;

		// Token: 0x040000E4 RID: 228
		public bool isFlatYFloat = false;

		// Token: 0x040000E5 RID: 229
		public bool slaveflag = false;

		// Token: 0x040000E6 RID: 230
		public int OwnerID;

		// Token: 0x040000E7 RID: 231
		public Player PlayerData;

		// Token: 0x040000E8 RID: 232
		public float drag = 0.08f;

		// Token: 0x040000E9 RID: 233
		public float floating = 1.5f;

		// Token: 0x040000EA RID: 234
		public bool Localflag = false;
	}
}
