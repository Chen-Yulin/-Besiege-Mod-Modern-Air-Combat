using System;
using System.Collections.Generic;
using Modding;
using Modding.Blocks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x02000008 RID: 8
	public class AdWaterModule : SingleInstance<AdWaterModule>
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001C RID: 28 RVA: 0x0000221F File Offset: 0x0000041F
		public override string Name
		{
			get
			{
				return "AdWaterModule";
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002191 File Offset: 0x00000391
		private void Awake()
		{
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00005398 File Offset: 0x00003598
		private void AddScript(Block block)
		{
			BlockBehaviour internalObject = block.BuildingBlock.InternalObject;
			BlockType blockID = (BlockType)internalObject.BlockID;
			bool flag = internalObject.GetComponent<UWaterBehaviourScript>() == null;
			if (flag)
			{
				bool flag2 = !AdWaterModule.ExclusionBlockList.Contains(blockID);
				if (flag2)
				{
					internalObject.gameObject.AddComponent<UWaterBehaviourScript>();
				}
			}
			bool flag3 = internalObject.GetComponent<NetworkBlock>();
			if (flag3)
			{
				bool flag4 = internalObject.GetComponent<AdNetworkBlock>() == null;
				if (flag4)
				{
					Object.Destroy(internalObject.GetComponent<NetworkBlock>());
					internalObject.gameObject.AddComponent<AdNetworkBlock>();
				}
			}
			bool flag5 = this.ScriptAttachContainer.ContainsKey(internalObject.BlockID);
			if (flag5)
			{
				Type type = this.ScriptAttachContainer[internalObject.BlockID];
				try
				{
					bool flag6 = internalObject.GetComponent(type) == null;
					if (flag6)
					{
						internalObject.gameObject.AddComponent(type);
					}
				}
				catch
				{
					ModConsole.Log("AddScript Error");
				}
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000054A0 File Offset: 0x000036A0
		private void Update()
		{
			bool flag = StatMaster.isMainMenu || StatMaster.inMenu;
			if (!flag)
			{
				bool flag2 = !this.Init;
				if (flag2)
				{
					this.Init = true;
					Events.OnBlockInit += this.AddScript;

				}
			}
		}

		// Token: 0x04000056 RID: 86
		private static readonly List<BlockType> ExclusionBlockList = new List<BlockType> { (BlockType)58, (BlockType)57, (BlockType)7, (BlockType)54, (BlockType)23 };

		// Token: 0x04000057 RID: 87
		public Dictionary<int, Type> ScriptAttachContainer = new Dictionary<int, Type>
		{
			{
				73,
				typeof(FireKillScript)
			},
			{
				48,
				typeof(DrillEmulateFix)
			}
		};

		// Token: 0x04000058 RID: 88
		public Dictionary<string, UWaterBehaviourScript> BlockObjectContainer = new Dictionary<string, UWaterBehaviourScript>();

		// Token: 0x04000059 RID: 89
		public Dictionary<int, string> NetworkIdContainer = new Dictionary<int, string>();

		// Token: 0x0400005A RID: 90
		public int BlockCount = 0;

		// Token: 0x0400005B RID: 91
		public GameObject ProjectileWaterEffect;

		// Token: 0x0400005C RID: 92
		public GameObject BlockWaterSplashEffect;

		// Token: 0x0400005D RID: 93
		public GameObject BlockWaterWaveEffect;

		// Token: 0x0400005E RID: 94
		public Material BlockWaterSplashMaterial;

		// Token: 0x0400005F RID: 95
		public List<Transform> ProjectileWaterEffectList = new List<Transform>();

		// Token: 0x04000060 RID: 96
		public int ProjectileEffectCount = 0;

		// Token: 0x04000061 RID: 97
		public int MAX_Effect_POOL_SIZE = 1000;

		// Token: 0x04000062 RID: 98
		public List<AdWaterModule.IventInfo> BlockIventsPool;

		// Token: 0x04000063 RID: 99
		public List<AdWaterModule.InitInfo> BlockInitsPool;

		// Token: 0x04000064 RID: 100
		private bool Init = false;

		// Token: 0x02000009 RID: 9
		public class IventInfo
		{
			// Token: 0x06000022 RID: 34 RVA: 0x0000571C File Offset: 0x0000391C
			public int Size()
			{
				return 13;
			}

			// Token: 0x04000065 RID: 101
			public ushort NetworkId;

			// Token: 0x04000066 RID: 102
			public Vector3 BlockDirection;

			// Token: 0x04000067 RID: 103
			public bool SplashEffectFlag;

			// Token: 0x04000068 RID: 104
			public float BlockSpeed;
		}

		// Token: 0x0200000A RID: 10
		public class InitInfo
		{
			// Token: 0x06000024 RID: 36 RVA: 0x00005730 File Offset: 0x00003930
			public int Size()
			{
				return 44;
			}

			// Token: 0x04000069 RID: 105
			public string GuidString;

			// Token: 0x0400006A RID: 106
			public ushort NetworkId;

			// Token: 0x0400006B RID: 107
			public Vector3 BlockBoundsCenter;
		}
	}
}
