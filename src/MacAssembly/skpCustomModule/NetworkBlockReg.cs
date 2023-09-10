using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000013 RID: 19
	public class NetworkBlockReg : AddScriptBase
	{
		// Token: 0x06000057 RID: 87 RVA: 0x0000232A File Offset: 0x0000052A
		public override void SafeAwake()
		{
			this.PlayerMachine = this.ObjectBehavior.ParentMachine;
			this.PlayerLinkManager = this.PlayerMachine.LinkManager;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000B550 File Offset: 0x00009750
		public void Update()
		{
			bool flag = !StatMaster.levelSimulating || !this.ObjectBehavior.isSimulating;
			if (flag)
			{
				this.Init = false;
				this.countFixedupdate = 0;
			}
			else
			{
				bool flag2 = !this.Init;
				if (flag2)
				{
					Debug.Log("Init");
					ServerMachine component = this.PlayerMachine.transform.GetComponent<ServerMachine>();
					NetworkController component2 = this.PlayerMachine.transform.GetComponent<NetworkController>();
					this.Init = true;
					this.adNetworkBlocks = new AdNetworkBlock[this.PlayerLinkManager.Nodes.Count - this.PlayerLinkManager.IgnoredNodes.Count];
					int count = this.PlayerLinkManager.Clusters.Count;
					uint num = 0U;
					Debug.Log("count : " + count.ToString());
					for (int i = 0; i < count; i++)
					{
						BlockCluster blockCluster = this.PlayerLinkManager.Clusters[i];
						BlockBehaviour block = blockCluster.Base.Block;
						BlockBehaviour simBlock = this.PlayerMachine.GetSimBlock(block);
						Machine.SimCluster simCluster = new Machine.SimCluster(simBlock, blockCluster.CenterOffset, blockCluster.BlockWeight, blockCluster.Blocks.Count);
						AdNetworkBlock component3 = simBlock.transform.GetComponent<AdNetworkBlock>();
						component3.Init(num, component2, simCluster.BaseTransform, this.PlayerMachine.SimPhysics);
						component2.Add(simBlock.transform.GetComponent<AdNetworkBlock>());
						this.adNetworkBlocks[(int)num++] = simBlock.transform.GetComponent<AdNetworkBlock>();
						int count2 = blockCluster.Blocks.Count;
						for (int j = 0; j < count2; j++)
						{
							BlockBehaviour block2 = blockCluster.Blocks[j].Block;
							bool flag3 = block2.NodeIndex != block.NodeIndex;
							if (flag3)
							{
								simBlock = this.PlayerMachine.GetSimBlock(block2);
								component3 = simBlock.transform.GetComponent<AdNetworkBlock>();
								component3.Init(num, component2, simCluster.BaseTransform, this.PlayerMachine.SimPhysics);
								component2.Add(simBlock.transform.GetComponent<AdNetworkBlock>());
								this.adNetworkBlocks[(int)num++] = simBlock.transform.GetComponent<AdNetworkBlock>();
							}
						}
					}
					for (int k = 0; k < this.PlayerLinkManager.IgnoredNodes.Count; k++)
					{
						BlockBehaviour block3 = this.PlayerLinkManager.IgnoredNodes[k].Block;
						BlockBehaviour simBlock2 = this.PlayerMachine.GetSimBlock(block3);
						bool flag4 = simBlock2 != null;
						if (flag4)
						{
							AdNetworkBlock component4 = simBlock2.transform.GetComponent<AdNetworkBlock>();
							component4.Init(num++, component2, simBlock2.transform, this.PlayerMachine.SimPhysics);
							component2.Add(simBlock2.transform.GetComponent<AdNetworkBlock>());
						}
					}
					Debug.Log("num: " + num.ToString());
					ServerMachine serverMachine = component;
					NetworkBlock[] networkBlocks = this.adNetworkBlocks;
					serverMachine.networkBlocks = networkBlocks;
				}
			}
		}

		// Token: 0x04000107 RID: 263
		public Machine PlayerMachine;

		// Token: 0x04000108 RID: 264
		public BlockLinkManager PlayerLinkManager;

		// Token: 0x04000109 RID: 265
		public AdNetworkBlock[] adNetworkBlocks;

		// Token: 0x0400010A RID: 266
		public bool Init = false;

		// Token: 0x0400010B RID: 267
		public int countFixedupdate = 0;
	}
}
