using System;
using Modding.Blocks;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000052 RID: 82
	public class AddScriptBase : MonoBehaviour
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001BF RID: 447 RVA: 0x00002B6E File Offset: 0x00000D6E
		// (set) Token: 0x060001C0 RID: 448 RVA: 0x00002B76 File Offset: 0x00000D76
		public Action<XDataHolder> BlockDataSaveEvent { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x00002B7F File Offset: 0x00000D7F
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x00002B87 File Offset: 0x00000D87
		public Action<XDataHolder> BlockDataLoadEvent { get; set; }

		// Token: 0x060001C3 RID: 451 RVA: 0x0001EF04 File Offset: 0x0001D104
		public virtual void Awake()
		{
			this.ObjectBehavior = base.GetComponent<BlockBehaviour>();
			this.OwnerID = (int)this.ObjectBehavior.ParentMachine.PlayerID;
			bool isMP = StatMaster.isMP;
			if (isMP)
			{
				this.ObjectNetworkBlock = base.GetComponent<NetworkBlock>();
			}
			this.SafeAwake();
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00002191 File Offset: 0x00000391
		public virtual void SafeAwake()
		{
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00002B90 File Offset: 0x00000D90
		public void OnEnable()
		{
			this.OwnerID = (int)this.ObjectBehavior.ParentMachine.PlayerID;
			this.SafeEnable();
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00002191 File Offset: 0x00000391
		public virtual void SafeEnable()
		{
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0001EF54 File Offset: 0x0001D154
		private void SaveConfiguration(PlayerMachineInfo pmi)
		{
			ConsoleController.ShowMessage("On save blockdata");
			bool flag = !(pmi == null);
			if (flag)
			{
				foreach (Modding.Blocks.BlockInfo blockInfo in pmi.Blocks)
				{
					bool flag2 = !(blockInfo.Guid != blockInfo.Guid);
					if (flag2)
					{
						XDataHolder data = blockInfo.Data;
						try
						{
							this.BlockDataSaveEvent(data);
						}
						catch
						{
						}
						this.SaveConfiguration(data);
						break;
					}
				}
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0001F00C File Offset: 0x0001D20C
		private void LoadConfiguration()
		{
			ConsoleController.ShowMessage("On load blockdata");
			bool flag = !(SingleInstance<AdShootingModule>.Instance.PMI == null);
			if (flag)
			{
				foreach (Modding.Blocks.BlockInfo blockInfo in SingleInstance<AdShootingModule>.Instance.PMI.Blocks)
				{
					bool flag2 = !(blockInfo.Guid != blockInfo.Guid);
					if (flag2)
					{
						XDataHolder data = blockInfo.Data;
						try
						{
							this.BlockDataLoadEvent(data);
						}
						catch
						{
						}
						this.LoadConfiguration(data);
						break;
					}
				}
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00002191 File Offset: 0x00000391
		public virtual void SaveConfiguration(XDataHolder BlockData)
		{
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00002191 File Offset: 0x00000391
		public virtual void LoadConfiguration(XDataHolder BlockData)
		{
		}

		// Token: 0x040003F4 RID: 1012
		public BlockBehaviour ObjectBehavior;

		// Token: 0x040003F5 RID: 1013
		public int OwnerID = 0;

		// Token: 0x040003F6 RID: 1014
		public NetworkBlock ObjectNetworkBlock;
	}
}
