using System;
using Modding.Blocks;
using UnityEngine;

namespace ModernAirCombat
{
	// Token: 0x02000046 RID: 70
	public class AddScriptBase : MonoBehaviour
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00022C00 File Offset: 0x00020E00
		// (set) Token: 0x0600020C RID: 524 RVA: 0x00022C08 File Offset: 0x00020E08
		public Action<XDataHolder> BlockDataSaveEvent { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00022C11 File Offset: 0x00020E11
		// (set) Token: 0x0600020E RID: 526 RVA: 0x00022C19 File Offset: 0x00020E19
		public Action<XDataHolder> BlockDataLoadEvent { get; set; }

		// Token: 0x0600020F RID: 527 RVA: 0x00022C24 File Offset: 0x00020E24
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

		// Token: 0x06000210 RID: 528 RVA: 0x00003D71 File Offset: 0x00001F71
		public virtual void SafeAwake()
		{
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00022C72 File Offset: 0x00020E72
		public void OnEnable()
		{
			this.OwnerID = (int)this.ObjectBehavior.ParentMachine.PlayerID;
			this.SafeEnable();
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00003D71 File Offset: 0x00001F71
		public virtual void SafeEnable()
		{
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00022C94 File Offset: 0x00020E94
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


		// Token: 0x06000215 RID: 533 RVA: 0x00003D71 File Offset: 0x00001F71
		public virtual void SaveConfiguration(XDataHolder BlockData)
		{
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00003D71 File Offset: 0x00001F71
		public virtual void LoadConfiguration(XDataHolder BlockData)
		{
		}

		// Token: 0x040003D6 RID: 982
		public BlockBehaviour ObjectBehavior;

		// Token: 0x040003D7 RID: 983
		public int OwnerID = 0;

		// Token: 0x040003D8 RID: 984
		public NetworkBlock ObjectNetworkBlock;
	}
}
