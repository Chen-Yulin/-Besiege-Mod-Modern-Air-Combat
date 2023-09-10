using System;
using System.Collections.Generic;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000014 RID: 20
	public class AdNetworkBlock : NetworkBlock
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002366 File Offset: 0x00000566
		public bool ExExpand
		{
			get
			{
				return AdCustomModuleMod.mod4.ExExpandFloor;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002372 File Offset: 0x00000572
		public bool networkCheck
		{
			get
			{
				return AdCustomModuleMod.skyboxcheckerCallBack;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000B880 File Offset: 0x00009A80
		protected override void AwakeBase()
		{
			bool flag = !this.isAwake;
			if (flag)
			{
				this.isAwake = true;
				this.isTracking = false;
				this.isInitialized = false;
				this.posHolder = default(Vector3);
				this.rotHolder = default(Quaternion);
				this.sendEntity = new SendEntity(true);
				this.posTracker = new NetworkInterpolation();
				this.rotTracker = new NetworkInterpolation();
				this.UpdateTransforms();
				this.UpdateBaseInterval();
				this.blockBehaviour = base.GetComponent<BlockBehaviour>();
				this.blockBehaviour.NetBlock = this;
			}
			bool isMP = StatMaster.isMP;
			if (isMP)
			{
				this.isBlock = true;
				this.FetchComponents();
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000B92C File Offset: 0x00009B2C
		public void FixedUpdate()
		{
			bool flag = this.debug;
			if (flag)
			{
				bool flag2 = base.transform.GetComponent<Rigidbody>();
				if (flag2)
				{
					Rigidbody component = base.transform.GetComponent<Rigidbody>();
					Debug.Log("BlockRigidbody.maxAngularVelocity:" + component.maxAngularVelocity.ToString());
					this.debug = false;
				}
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000B990 File Offset: 0x00009B90
		public override void UpdateBaseInterval()
		{
			bool isBaseBlock = this.isBaseBlock;
			if (isBaseBlock)
			{
				base.UpdateBaseInterval();
			}
			else
			{
				this.baseInterval = NetworkScene.ServerSettings.sendRate * (float)(NetworkScene.ServerSettings.skipChildCount + 1);
			}
			bool flag = this.isInitialized && !this.isTracking;
			if (flag)
			{
				this.posTracker.OverrideInterval(this.baseInterval);
				this.rotTracker.OverrideInterval(this.baseInterval);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000BA14 File Offset: 0x00009C14
		public override void UpdateTransforms()
		{
			bool flag = !this.isBlock || !this.blockBehaviour.isSimulating;
			if (flag)
			{
				base.UpdateTransforms();
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000BA48 File Offset: 0x00009C48
		public override void Init(uint blockIdentifier, NetworkController controller, Transform baseEnt, bool track)
		{
			this.AwakeBase();
			this.baseBlock = baseEnt;
			this.isBaseBlock = this.baseBlock == base.transform;
			this.isClusterBase = this.isBaseBlock;
			this.isEssential = this.isBaseBlock;
			this.hasChildManager = false;
			this.BreakCount = 0;
			this.UpdateBaseInterval();
			bool flag = !this.isBaseBlock;
			if (flag)
			{
				this.baseAdNetworkBlock = this.baseBlock.GetComponent<AdNetworkBlock>();
			}
			this.Init(blockIdentifier, controller, track);
			this.ResetTransformData();
			bool isBlock = this.isBlock;
			if (isBlock)
			{
				bool hasParentMachine = this.blockBehaviour.HasParentMachine;
				if (hasParentMachine)
				{
					this.serverMachine = this.blockBehaviour.ParentMachine as ServerMachine;
				}
				BlockType type = this.blockBehaviour.Prefab.Type;
				BlockType blockType = type;
				if ((int)blockType <= 15)
				{
					if ((int)blockType <= 7)
					{
						if ((int)blockType != 1)
						{
							if ((int)blockType != 7)
							{
								goto IL_318;
							}
							bool stripped = this.blockBehaviour.stripped;
							if (stripped)
							{
								goto IL_318;
							}
							BraceCode braceCode = this.blockBehaviour as BraceCode;
							Vector3 localScale = braceCode.transform.localScale;
							Vector3 localScale2 = braceCode.cylinder.localScale;
							bool flag2 = BraceCode.BraceType(localScale, localScale2.y) == 0;
							if (flag2)
							{
								base.SetupBrace(braceCode);
								this.posTracker.SetData(this.baseInterval, this.GetPos(this.position));
								this.rotTracker.SetData(this.baseInterval, this.GetRot(this.rotation));
								bool flag3 = !this.isBaseBlock;
								if (flag3)
								{
									this.basePos = this.posTracker.lastVec;
								}
								else
								{
									this.position = this.myTransform.position;
									this.rotation = this.myTransform.rotation;
									this.transformRotation = Quaternion.Inverse(this.rotation);
									this.transformMatrix = this.myTransform.worldToLocalMatrix;
								}
							}
							goto IL_318;
						}
					}
					else
					{
						if ((int)blockType == 9)
						{
							goto IL_1F6;
						}
						if ((int)blockType != 15)
						{
							goto IL_318;
						}
					}
				}
				else if ((int)blockType <= 41)
				{
					if ((int)blockType == 36)
					{
						BreakOnForceBoulder component = base.GetComponent<BreakOnForceBoulder>();
						bool flag4 = component.BreakInto;
						if (flag4)
						{
							this.childManager = new EntityChildManager(this, controller, this.isTracking);
							this.hasChildManager = true;
							NetworkBlock component2 = component.BreakInto.GetComponent<NetworkBlock>();
							this.BreakCount = component2.children.Length;
						}
						goto IL_318;
					}
					if ((int)blockType != 41)
					{
						goto IL_318;
					}
				}
				else
				{
					if ((int)blockType == 45)
					{
						goto IL_1F6;
					}
					if ((int)blockType != 63)
					{
						if ((int)blockType != 73)
						{
							goto IL_318;
						}
						this.fireTag = this.blockBehaviour.GetComponent<FireTag>();
						this.hasFireController = this.fireTag != null;
						this.fireController = ((!this.hasFireController) ? null : this.fireTag.fireControllerCode);
						goto IL_318;
					}
				}
				(this.blockBehaviour.VisualController as FragmentVisualController).onVisualBreak += this.OnVisualBreak;
				goto IL_318;
				IL_1F6:
				this.SetupSpring(this.blockBehaviour as SpringCode);
				IL_318:
				bool flag5 = !this.serverMachine.SimPhysics;
				if (flag5)
				{
					this.SetupClientBlock();
				}
				else
				{
					bool flag6 = !this.isBaseBlock;
					if (flag6)
					{
						this.baseDist = (this.baseAdNetworkBlock.position - this.trackTransform.position).sqrMagnitude;
						this.baseSqrThreshold = this.baseDist * this.baseThreshold;
						this.partOfCluster = true;
					}
					else
					{
						this.partOfCluster = false;
					}
					this.serverMachine.RegisterIntact(this.blockBehaviour);
				}
			}
			bool flag7 = track && this.pollTransform;
			if (flag7)
			{
				this.pollFrame = (int)((ulong)blockIdentifier % (ulong)((long)(NetworkScene.ServerSettings.skipChildCount + 1)));
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000BE30 File Offset: 0x0000A030
		private BlockBehaviour GetConnectedBlock(bool isOwn, bool isDynamic, bool dynamicToggle)
		{
			List<BlockLink> blockNeighbours = this.serverMachine.GetBlockNeighbours(this.blockBehaviour.NodeIndex);
			bool flag = blockNeighbours != null;
			if (flag)
			{
				for (int i = 0; i < blockNeighbours.Count; i++)
				{
					BlockLink blockLink = blockNeighbours[i];
					for (int j = 0; j < blockLink.Triggers.Count; j++)
					{
						BlockTrigger blockTrigger = blockLink.Triggers[j];
						bool flag2 = blockTrigger.isOwnLink == isOwn && (!dynamicToggle || blockTrigger.isDynamic == isDynamic);
						if (flag2)
						{
							return this.serverMachine.GetSimBlock(blockLink.Other.Block);
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000BEFC File Offset: 0x0000A0FC
		public override void BreakIntoChildren(Transform breakInstance)
		{
			NetworkBlock component = breakInstance.GetComponent<NetworkBlock>();
			this.childManager.InitBlockChildren(component);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000BF20 File Offset: 0x0000A120
		private void SetupSpring(SpringCode springCode)
		{
			Transform startPoint = springCode.startPoint;
			Transform endPoint = springCode.endPoint;
			List<BlockLink> blockNeighbours = this.serverMachine.GetBlockNeighbours(this.blockBehaviour.NodeIndex);
			bool flag = false;
			bool flag2 = false;
			BlockBehaviour blockBehaviour = null;
			BlockBehaviour blockBehaviour2 = null;
			for (int i = 0; i < blockNeighbours.Count; i++)
			{
				BlockLink blockLink = blockNeighbours[i];
				foreach (BlockTrigger blockTrigger in blockLink.Triggers)
				{
					bool isOwnLink = blockTrigger.isOwnLink;
					if (isOwnLink)
					{
						bool isDynamic = blockTrigger.isDynamic;
						if (isDynamic)
						{
							flag = true;
							blockBehaviour = blockLink.Other.Block;
						}
						else
						{
							flag2 = true;
							blockBehaviour2 = blockLink.Other.Block;
						}
					}
				}
			}
			bool flag3 = false;
			bool flag4 = false;
			bool isTracking = this.isTracking;
			if (isTracking)
			{
				bool flag5 = flag && flag2;
				if (flag5)
				{
					this.pollTransform = false;
				}
				else
				{
					bool flag6 = !flag && flag2;
					if (flag6)
					{
						this.trackTransform = endPoint;
						flag3 = true;
					}
					else
					{
						this.trackTransform = startPoint;
						flag4 = true;
					}
				}
			}
			else
			{
				bool flag7 = flag;
				if (flag7)
				{
					BlockBehaviour simBlock = this.serverMachine.GetSimBlock(blockBehaviour);
					endPoint.SetParent(simBlock.transform, true);
				}
				bool flag8 = flag2;
				if (flag8)
				{
					BlockBehaviour simBlock2 = this.serverMachine.GetSimBlock(blockBehaviour2);
					startPoint.SetParent(simBlock2.transform, true);
				}
				bool flag9 = !flag && flag2;
				if (flag9)
				{
					flag3 = true;
				}
				else
				{
					bool flag10 = flag && !flag2;
					if (flag10)
					{
						flag4 = true;
					}
				}
			}
			bool flag11 = flag4 || flag3;
			if (flag11)
			{
				bool isTracking2 = this.isTracking;
				Vector3 vector;
				Quaternion quaternion;
				if (isTracking2)
				{
					vector = (this.position = this.trackTransform.position);
					quaternion = (this.rotation = this.trackTransform.rotation);
				}
				else
				{
					bool flag12 = !flag2;
					if (flag12)
					{
						startPoint.SetParent(base.transform.parent, true);
					}
					bool flag13 = !flag;
					if (flag13)
					{
						endPoint.SetParent(base.transform.parent, true);
					}
					vector = ((!flag4) ? endPoint.position : startPoint.position);
					quaternion = ((!flag4) ? endPoint.rotation : startPoint.rotation);
					this.myTransform.position = vector;
					this.myTransform.rotation = quaternion;
					bool flag14 = !flag2;
					if (flag14)
					{
						startPoint.SetParent(this.myTransform, true);
					}
					bool flag15 = !flag;
					if (flag15)
					{
						endPoint.SetParent(this.myTransform, true);
					}
				}
				this.posTracker.SetData(this.baseInterval, this.GetPos(vector));
				this.rotTracker.SetData(this.baseInterval, this.GetRot(quaternion));
				this.ResetTransformData();
			}
			bool flag16 = !this.blockBehaviour.SimPhysics && springCode.winchMode;
			if (flag16)
			{
				bool flag17 = flag2;
				if (flag17)
				{
					BlockBehaviour simBlock3 = this.serverMachine.GetSimBlock(blockBehaviour2);
					simBlock3.CreateSimLists();
					//simBlock3.visAddedToMe.Add(springCode.winchRotateVis.GetComponent<Renderer>());
				}
				bool flag18 = flag;
				if (flag18)
				{
					BlockBehaviour simBlock4 = this.serverMachine.GetSimBlock(blockBehaviour);
					simBlock4.CreateSimLists();
					//simBlock4.visAddedToMe.Add(springCode.winchRotateVis2.GetComponent<Renderer>());
				}
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000C2B8 File Offset: 0x0000A4B8
		private void ResetTransformData()
		{
			bool flag = !this.isBaseBlock;
			if (flag)
			{
				this.basePos = this.posTracker.lastVec;
			}
			else
			{
				this.position = this.myTransform.position;
				this.rotation = this.myTransform.rotation;
				this.transformRotation = Quaternion.Inverse(this.rotation);
				this.transformMatrix = this.myTransform.worldToLocalMatrix;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000C32C File Offset: 0x0000A52C
		public void SetupClientBlock()
		{
			BlockType type = this.blockBehaviour.Prefab.Type;
			bool flag = this.blockBehaviour.Prefab.hasBVC && this.blockBehaviour.Prefab.hasFragment;
			bool flag2 = (int)type == 73;
			if (flag2)
			{
				BuildSurface buildSurface = this.blockBehaviour as BuildSurface;
				bool isValid = buildSurface.isValid;
				if (isValid)
				{
					Collider[] componentsInChildren = buildSurface.simColliderParent.GetComponentsInChildren<Collider>();
					int j;
					int i;
					for (i = 0; i < componentsInChildren.Length; i = j + 1)
					{
						componentsInChildren[i].enabled = false;
						j = i;
					}
					Dictionary<int, List<BlockBehaviour>> jointDict = new Dictionary<int, List<BlockBehaviour>>();
					List<BlockLink> blockNeighbours = this.serverMachine.GetBlockNeighbours(this.blockBehaviour.NodeIndex);
					blockNeighbours.ForEach(delegate(BlockLink x)
					{
						BlockBehaviour block = x.Other.Block;
						int i2;
						for (i = 0; i < x.Triggers.Count; i = i2 + 1)
						{
							BlockTrigger blockTrigger = x.Triggers[i];
							bool isOwnLink = blockTrigger.isOwnLink;
							if (isOwnLink)
							{
								List<BlockBehaviour> list = new List<BlockBehaviour>();
								bool flag6 = !jointDict.TryGetValue(blockTrigger.Index, out list);
								if (flag6)
								{
									list = new List<BlockBehaviour>();
									jointDict.Add(blockTrigger.Index, list);
								}
								list.Add(block.SimBlock);
							}
							i2 = i;
						}
					});
					foreach (KeyValuePair<int, List<BlockBehaviour>> keyValuePair in jointDict)
					{
						buildSurface.FragmentController.OnConnectionEstablished(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			else
			{
				bool flag3 = !flag && type != (BlockType)43;
				if (!flag3)
				{
					BlockBehaviour connectedBlock = this.GetConnectedBlock(true, false, false);
					bool flag4 = connectedBlock != null;
					if (flag4)
					{
						bool flag5 = flag;
						if (flag5)
						{
							FilterRendererPair filterRendererPair = (this.blockBehaviour.VisualController as FragmentVisualController).brokenVis[0];
							Renderer renderer = filterRendererPair.renderer;
							renderer.transform.parent = connectedBlock.transform;
							connectedBlock.visAddedToMe.Add(renderer);
							filterRendererPair.active = false;
						}
						else
						{
							(this.blockBehaviour as BalloonController).endPoint.parent = connectedBlock.transform;
						}
					}
				}
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000C540 File Offset: 0x0000A740
		public override bool UpdateEntity(float delta)
		{
			this.changedPos = false;
			this.changedRot = false;
			bool isHeadless = StatMaster.isHeadless;
			if (isHeadless)
			{
				bool isActive = this.posTracker.isActive;
				if (isActive)
				{
					this.posTracker.Update(delta);
					this.changedPos = true;
				}
				bool isActive2 = this.rotTracker.isActive;
				if (isActive2)
				{
					this.rotTracker.Update(delta);
					this.changedRot = true;
				}
				bool flag = this.isClusterBase && (this.changedPos || this.changedRot);
				if (flag)
				{
					bool changedPos = this.changedPos;
					if (changedPos)
					{
						this.position = this.posTracker.Vector;
					}
					bool changedRot = this.changedRot;
					if (changedRot)
					{
						this.rotation = this.rotTracker.Rotation;
					}
					this.transformMatrix = this.myTransform.localToWorldMatrix;
				}
			}
			else
			{
				bool flag2 = this.isClusterBase;
				if (flag2)
				{
					bool isActive3 = this.posTracker.isActive;
					if (isActive3)
					{
						this.posTracker.Update(delta);
						this.position = this.posTracker.Vector;
						this.trackTransform.position = this.posTracker.Vector;
						this.changedPos = true;
					}
					bool isActive4 = this.rotTracker.isActive;
					if (isActive4)
					{
						this.rotTracker.Update(delta);
						this.rotation = this.rotTracker.Rotation;
						this.trackTransform.rotation = this.rotation;
						this.changedRot = true;
					}
					bool flag3 = this.changedPos || this.changedRot;
					if (flag3)
					{
						this.transformMatrix = this.myTransform.localToWorldMatrix;
					}
				}
				else
				{
					bool isBaseBlock = this.isBaseBlock;
					if (isBaseBlock)
					{
						bool isActive5 = this.posTracker.isActive;
						if (isActive5)
						{
							this.posTracker.Update(delta);
							this.trackTransform.position = this.posTracker.Vector;
						}
						bool isActive6 = this.rotTracker.isActive;
						if (isActive6)
						{
							this.rotTracker.Update(delta);
							this.trackTransform.rotation = this.rotTracker.Rotation;
						}
					}
					else
					{
						bool isActive7 = this.posTracker.isActive;
						if (isActive7)
						{
							this.posTracker.Update(delta);
							this.posActive = true;
						}
						bool isActive8 = this.rotTracker.isActive;
						if (isActive8)
						{
							this.rotTracker.Update(delta);
							this.rotActive = true;
						}
						bool flag4 = this.posActive;
						if (flag4)
						{
							this.trackTransform.position = this.baseAdNetworkBlock.transformMatrix.MultiplyPoint3x4(this.posTracker.Vector);
						}
						bool flag5 = this.rotActive;
						if (flag5)
						{
							this.trackTransform.localRotation = this.rotTracker.Rotation;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000C838 File Offset: 0x0000AA38
		public override int GetDataSize()
		{
			return NetworkEntity.GetDataSize((this.isBaseBlock ? ((this.hasChangedPos ? 1 : 0) | (this.hasChangedRot ? 2 : 0)) : 0) | (this.hasChangedState ? 4 : 0));
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000C880 File Offset: 0x0000AA80
		public override int EncodeState(byte[] buffer, int offset)
		{
			int num = offset;
			bool isBaseBlock = this.isBaseBlock;
			if (isBaseBlock)
			{
				buffer[offset] = (byte)((this.hasChangedPos ? 1 : 0) | (this.hasChangedRot ? 2 : 0) | (this.hasChangedState ? 4 : 0));
				offset++;
				bool hasChangedState = this.hasChangedState;
				if (hasChangedState)
				{
					buffer[offset] = this.blockState;
					offset++;
				}
				bool hasChangedPos = this.hasChangedPos;
				if (hasChangedPos)
				{
					if (ExExpand)
					{
						AdNetworkCompression.CompressPosition(this.posTracker.lastVec, buffer, offset);
						offset += 12;
					}
					else
					{
                        NetworkCompression.CompressPosition(this.posTracker.lastVec, buffer, offset);
                        offset += 6;
                    }
				}
				bool hasChangedRot = this.hasChangedRot;
				if (hasChangedRot)
				{
					NetworkCompression.CompressRotation(this.rotTracker.lastRot, buffer, offset);
					offset += 7;
				}
			}
			else
			{
				buffer[offset] = (byte)(this.hasChangedState ? 4 : 0);
				offset++;
				bool hasChangedState2 = this.hasChangedState;
				if (hasChangedState2)
				{
					buffer[offset] = this.blockState;
					offset++;
				}
			}
			return offset - num;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000C978 File Offset: 0x0000AB78
		public override int DecodeState(byte[] data, int offset)
		{
			int num = offset;
			int num2 = (int)data[offset];
			offset++;
			bool flag = NetworkEntity.StateChanged(num2);
			if (flag)
			{
				this.ApplyState(data[offset]);
				offset++;
			}
			this.changedPos = (this.changedRot = false);
			bool flag2 = false;
			bool flag3 = NetworkEntity.PosChanged(num2);
			if (flag3)
			{
				if (ExExpand) {
					AdNetworkCompression.DecompressPosition(data, offset, out this.posHolder);
					offset += 12;
				} else
				{
                    NetworkCompression.DecompressPosition(data, offset, out this.posHolder);
                    offset += 6;
                }
				this.position = this.GetPos(this.posHolder);
				this.posTracker.Override(this.position, this.position);
				flag2 = true;
				this.trackTransform.position = this.position;
			}
			bool flag4 = NetworkEntity.RotChanged(num2);
			if (flag4)
			{
				NetworkCompression.DecompressRotation(data, offset, out this.rotHolder);
				this.rotation = this.GetRot(this.rotHolder);
				this.rotTracker.Override(this.rotation, this.rotation);
				this.trackTransform.rotation = this.rotation;
				flag2 = true;
				offset += 7;
			}
			bool flag5 = this.isBaseBlock && flag2;
			if (flag5)
			{
				this.transformMatrix = this.myTransform.localToWorldMatrix;
			}
			return offset - num;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000CAB0 File Offset: 0x0000ACB0
		protected override void ApplyState(byte state)
		{
			NetworkBlock.applyingState = true;
			bool flag = base.ContainsState(state, (NetworkBlock.BlockState)1);
			if (flag)
			{
				this.SetEvent(0U, (EntityEvent)1);
			}
			bool flag2 = base.ContainsState(state, (NetworkBlock.BlockState)4);
			if (flag2)
			{
				this.SetEvent(0U, (EntityEvent)4, (byte)0);
			}
			bool flag3 = base.ContainsState(state, (NetworkBlock.BlockState)64);
			if (flag3)
			{
				this.SetEvent(0U, (EntityEvent)11);
			}
			bool flag4 = base.ContainsState(state, (NetworkBlock.BlockState)2);
			if (flag4)
			{
				this.SetEvent(0U, (EntityEvent)0);
			}
			bool flag5 = base.ContainsState(state, (NetworkBlock.BlockState)8);
			if (flag5)
			{
				this.SetEvent(0U, (EntityEvent)6);
			}
			bool flag6 = base.ContainsState(state, (NetworkBlock.BlockState)32);
			if (flag6)
			{
				this.SetEvent(0U, (EntityEvent)8);
			}
			NetworkBlock.applyingState = false;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000CB5C File Offset: 0x0000AD5C
		public void RegisterJointDamage(ConfigurableJoint[] configJoints, HingeJoint[] hingeJoints)
		{
			for (int i = 0; i < configJoints.Length; i++)
			{
				bool flag = configJoints[i].connectedBody != null;
				if (flag)
				{
					this.serverMachine.DamageController.AddTotalDamage(AdNetworkBlock.jointDamageValue);
					this.totalDamageAdded += AdNetworkBlock.jointDamageValue;
				}
			}
			for (int i = 0; i < hingeJoints.Length; i++)
			{
				bool flag2 = hingeJoints[i].connectedBody != null;
				if (flag2)
				{
					this.serverMachine.DamageController.AddTotalDamage(AdNetworkBlock.jointDamageValue);
					this.totalDamageAdded += AdNetworkBlock.jointDamageValue;
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000CC10 File Offset: 0x0000AE10
		protected override void OnDestroy()
		{
			this.isDestroyed = true;
			bool flag = this.isTracking && this.isBlock;
			if (flag)
			{
				this.serverMachine.DamageController.RemoveTotalDamage(this.totalDamageAdded);
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000CC54 File Offset: 0x0000AE54
		public override int PollObject(bool fullUpdate, byte[] data, int offset)
		{
			int num = offset;
			offset++;
			bool flag = this.isDestroyed || !this.pollTransform;
			int result;
			if (flag)
			{
				int eventCount = this.sendEntity.eventCount;
				bool flag2 = eventCount > 0;
				if (flag2)
				{
					data[num] = (byte)(eventCount << 3);
					Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount);
					this.sendEntity.eventCount = 0;
				}
				result = offset - num + eventCount;
			}
			else
			{
				bool flag3 = false;
				bool isBaseBlock = this.isBaseBlock;
				Vector3 vector;
				Quaternion quaternion;
				if (isBaseBlock)
				{
					this.position = this.trackTransform.position;
					vector = NetworkEntity.ClampPosition(this.position);
					this.rotation = this.trackTransform.rotation;
					quaternion = this.rotation;
					bool flag4 = this.isClusterBase;
					if (flag4)
					{
						this.transformRotation = Quaternion.Inverse(this.rotation);
						this.transformMatrix = this.trackTransform.worldToLocalMatrix;
					}
					bool flag5 = this.partOfCluster && this.blockBehaviour.isIntact;
					if (flag5)
					{
						float sqrMagnitude = (this.baseAdNetworkBlock.position - this.position).sqrMagnitude;
						bool flag6 = sqrMagnitude > this.baseSqrThreshold;
						if (flag6)
						{
							this.serverMachine.ApplyDamage(this.blockBehaviour, (MachineDamageType)4);
							this.partOfCluster = false;
						}
					}
				}
				else
				{
					bool flag7 = this.SkipFrame();
					if (flag7)
					{
						int eventCount2 = this.sendEntity.eventCount;
						bool flag8 = eventCount2 > 0;
						if (flag8)
						{
							data[num] = (byte)(eventCount2 << 3);
							Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount2);
							this.sendEntity.eventCount = 0;
						}
						return offset - num + eventCount2;
					}
					this.position = this.trackTransform.position;
					vector = NetworkEntity.ClampPosition(this.baseAdNetworkBlock.transformMatrix.MultiplyPoint3x4(this.position));
					bool flag9 = !fullUpdate;
					if (flag9)
					{
						float num2 = vector.x - this.basePos.x;
						float num3 = vector.y - this.basePos.y;
						float num4 = vector.z - this.basePos.z;
						float num5 = ((num2 >= 0f) ? num2 : (0f - num2)) + ((num3 >= 0f) ? num3 : (0f - num3)) + ((num4 >= 0f) ? num4 : (0f - num4));
						bool flag10 = num5 <= this.BASE_THRESHOLD;
						if (flag10)
						{
							int eventCount3 = this.sendEntity.eventCount;
							bool flag11 = eventCount3 > 0;
							if (flag11)
							{
								data[num] = (byte)(eventCount3 << 3);
								Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount3);
								this.sendEntity.eventCount = 0;
							}
							return offset - num + eventCount3;
						}
						this.BreakOff(0U);
						flag3 = true;
						vector = NetworkEntity.ClampPosition(this.position);
						this.rotation = this.trackTransform.rotation;
						quaternion = this.rotation;
					}
					else
					{
						this.rotation = this.trackTransform.rotation;
						quaternion = this.baseAdNetworkBlock.transformRotation * this.rotation;
					}
				}
				int num6 = 0;
				bool flag12 = this.hasWheelSmoke && this.smokeActive != this.wheelSmoke.smokeActive;
				if (flag12)
				{
					this.smokeActive = this.wheelSmoke.smokeActive;
					this.Event((EntityEvent)9, (byte)(this.smokeActive ? 1 : 0));
				}
				bool flag13 = this.hasCogMotorDamage && this.cogMotorDamage.hasEmitted;
				if (flag13)
				{
					bool flag14 = (int)this.blockBehaviour.Prefab.Type == 17;
					float num7;
					if (flag14)
					{
						num7 = ((this.cogMotorDamage.eulerX >= 0f) ? this.cogMotorDamage.eulerX : (360f + this.cogMotorDamage.eulerX));
						num7 = num7 / 360f * 255f;
					}
					else
					{
						num7 = Mathf.Clamp(this.cogMotorDamage.drillDistance * 100f, 0f, 255f);
					}
					this.Event((EntityEvent)31, (byte)num7);
					this.cogMotorDamage.hasEmitted = false;
				}
				bool flag15 = false;
				int num8 = this.lastSentPos + 1;
				this.lastSentPos = num8;
				bool flag16 = num8 > OptionsMaster.resendTransformFrames;
				bool flag17 = flag3 || flag16 || !this.posTracker.WithinThreshold(vector);
				if (flag17)
				{
					bool flag18 = !this.isBaseBlock && fullUpdate;
					if (flag18)
					{
						float num9 = vector.x - this.basePos.x;
						float num10 = vector.y - this.basePos.y;
						float num11 = vector.z - this.basePos.z;
						float num12 = ((num9 >= 0f) ? num9 : (0f - num9)) + ((num10 >= 0f) ? num10 : (0f - num10)) + ((num11 >= 0f) ? num11 : (0f - num11));
						bool flag19 = num12 > this.BASE_THRESHOLD;
						if (flag19)
						{
							this.BreakOff(0U);
							flag3 = true;
							vector = NetworkEntity.ClampPosition(this.position);
							quaternion = this.rotation;
						}
					}
					bool flag20 = !flag3 && this.isBreakingOff;
					if (flag20)
					{
						bool flag21 = this.baseCurrentFrame < OptionsMaster.baseEventFrames;
						if (flag21)
						{
							this.Event((EntityEvent)1);
							this.baseCurrentFrame++;
						}
						else
						{
							this.isBreakingOff = false;
						}
					}
					int eventCount4 = this.sendEntity.eventCount;
					int num13 = 6;
					int num14 = eventCount4 + num13;
					bool exExpand = this.ExExpand;
					if (exExpand)
					{
						bool flag22 = eventCount4 > 0;
						if (flag22)
						{
							Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount4);
							this.sendEntity.eventCount = 0;
							offset += eventCount4;
							num6 |= num14 << 3;
						}
						else
						{
							num6 |= num13 << 3;
						}
						flag15 = true;
						this.lastSentPos = 0;
						AdNetworkCompression.CompressPosition(vector, data, offset);
						offset += 6 + num13;
						num6 |= 1;
					}
					else
					{
						bool flag23 = eventCount4 > 0;
						if (flag23)
						{
							Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount4);
							this.sendEntity.eventCount = 0;
							offset += eventCount4;
							num6 |= eventCount4 << 3;
						}
						flag15 = true;
						this.lastSentPos = 0;
						NetworkCompression.CompressPosition(vector, data, offset);
						offset += 6;
						num6 |= 1;
					}
					this.posTracker.Store(vector);
					this.hasChangedPos = true;
				}
				num8 = this.lastSentRot + 1;
				this.lastSentRot = num8;
				bool flag24 = num8 > OptionsMaster.resendTransformFrames;
				bool flag25 = flag3 || flag24 || !this.rotTracker.WithinThreshold(quaternion);
				if (flag25)
				{
					bool flag26 = !flag15;
					if (flag26)
					{
						int eventCount5 = this.sendEntity.eventCount;
						bool flag27 = eventCount5 > 0;
						if (flag27)
						{
							Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount5);
							this.sendEntity.eventCount = 0;
							offset += eventCount5;
							num6 |= eventCount5 << 3;
						}
						flag15 = true;
					}
					this.lastSentRot = 0;
					NetworkCompression.CompressRotation(quaternion, data, offset);
					offset += 7;
					num6 |= 4;
					this.rotTracker.Store(quaternion);
					this.hasChangedRot = true;
				}
				bool flag28 = !flag15;
				if (flag28)
				{
					int eventCount6 = this.sendEntity.eventCount;
					bool flag29 = eventCount6 > 0;
					if (flag29)
					{
						Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount6);
						this.sendEntity.eventCount = 0;
						offset += eventCount6;
						num6 |= eventCount6 << 3;
					}
				}
				data[num] = (byte)num6;
				bool turningOff = this.turningOff;
				if (turningOff)
				{
					this.pollTransform = false;
					this.turningOff = false;
				}
				result = offset - num;
			}
			return result;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000D44C File Offset: 0x0000B64C
		public override void SetData(uint frame, byte[] data, int offset, bool hasPos, bool hasRot, int eventCount)
		{
			offset++;
			int num = offset;
			int num2 = 6;
			bool flag = !hasPos || !this.ExExpand;
			if (flag)
			{
				num2 = 0;
			}
			int num3 = eventCount - num2;
			for (int i = 0; i < num3; i++)
			{
				NetworkEntity.EntityEvent entityEvent = (EntityEvent)data[num + i];
				bool flag2 = (int)entityEvent == 1;
				if (flag2)
				{
					this.SetEvent(frame, entityEvent);
				}
				i += base.DataLength(entityEvent);
			}
			offset += num3;
			if (hasPos)
			{
				bool flag3 = frame >= this.lastPosFrame;
				if (flag3)
				{
					bool exExpand = this.ExExpand;
					if (exExpand)
					{
						AdNetworkCompression.DecompressPosition(data, offset, out this.posHolder);
					}
					else
					{
						NetworkCompression.DecompressPosition(data, offset, out this.posHolder);
					}
					bool flag4 = !this.isBaseBlock && (ulong)(frame - this.lastPosFrame) >= (ulong)((long)OptionsMaster.resendTransformFrames);
					if (flag4)
					{
						float num4 = this.posHolder.x - this.basePos.x;
						float num5 = this.posHolder.y - this.basePos.y;
						float num6 = this.posHolder.z - this.basePos.z;
						float num7 = ((num4 >= 0f) ? num4 : (0f - num4)) + ((num5 >= 0f) ? num5 : (0f - num5)) + ((num6 >= 0f) ? num6 : (0f - num6));
						bool flag5 = num7 > this.BASE_THRESHOLD;
						if (flag5)
						{
							this.SetEvent(frame, (EntityEvent)1);
						}
					}
					bool networkCheck = this.networkCheck;
					if (networkCheck)
					{
						this.posTracker.Set(this.posHolder);
					}
					this.hasChangedPos = true;
					this.lastPosFrame = frame;
				}
				offset += 6 + num2;
			}
			if (hasRot)
			{
				bool flag6 = frame >= this.lastRotFrame;
				if (flag6)
				{
					NetworkCompression.DecompressRotation(data, offset, out this.rotHolder);
					bool networkCheck2 = this.networkCheck;
					if (networkCheck2)
					{
						this.rotTracker.Set(this.rotHolder);
					}
					this.hasChangedRot = true;
					this.lastRotFrame = frame;
				}
				offset += 7;
			}
			for (int i = 0; i < num3; i++)
			{
				NetworkEntity.EntityEvent entityEvent2 = (EntityEvent)data[num + i];
				int num8 = base.DataLength(entityEvent2);
				bool flag7 = num8 > 0;
				if (flag7)
				{
					int num9 = num + (i + 1);
					bool flag8 = num8 == 1;
					if (flag8)
					{
						this.SetEvent(frame, entityEvent2, data[num9]);
					}
					else
					{
						byte[] array = new byte[num8];
						Buffer.BlockCopy(data, num9, array, 0, num8);
						this.SetEvent(frame, entityEvent2, array);
					}
					i += num8;
				}
				else
				{
					bool flag9 = entityEvent2 != (EntityEvent)1;
					if (flag9)
					{
						this.SetEvent(frame, entityEvent2);
					}
				}
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000D720 File Offset: 0x0000B920
		protected override Vector3 GetPos(Vector3 pos)
		{
			bool flag = !this.isBaseBlock;
			Vector3 result;
			if (flag)
			{
				result = this.baseAdNetworkBlock.transformMatrix.MultiplyPoint3x4(pos);
			}
			else
			{
				result = pos;
			}
			return result;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000D758 File Offset: 0x0000B958
		protected override Quaternion GetRot(Quaternion rot)
		{
			bool flag = !this.isBaseBlock;
			Quaternion result;
			if (flag)
			{
				result = this.baseAdNetworkBlock.transformRotation * rot;
			}
			else
			{
				result = rot;
			}
			return result;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000D790 File Offset: 0x0000B990
		protected bool SkipFrame()
		{
			int num = this.skipFrames + 1;
			this.skipFrames = num;
			bool flag = num > NetworkScene.ServerSettings.skipChildCount;
			if (flag)
			{
				this.skipFrames = 0;
			}
			return this.skipFrames != this.pollFrame;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000D7E0 File Offset: 0x0000B9E0
		public override void NewFrame(uint frame)
		{
			bool flag = this.isBaseBlock || !this.SkipFrame();
			if (flag)
			{
				base.NewFrame(frame);
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000D810 File Offset: 0x0000BA10
		public override void Event(NetworkEntity.EntityEvent evt, byte[] eventData)
		{
			bool flag = base.DataLength(evt) < 2;
			if (flag)
			{
				Debug.LogWarning("Event " + evt.ToString() + " should be a 1 byte event!");
			}
			bool flag2 = this.sendEntity.eventCount + 1 + eventData.Length < SendEntity.MAX_EVENTS;
			if (flag2)
			{
				this.sendEntity.AddEvent(((byte)evt));
				for (int i = 0; i < eventData.Length; i++)
				{
					this.sendEntity.AddEvent(eventData[i]);
				}
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000D8A0 File Offset: 0x0000BAA0
		public override void Event(NetworkEntity.EntityEvent evt, byte eventData)
		{
			bool flag = base.DataLength(evt) != 1;
			if (flag)
			{
				Debug.LogWarning("Event " + evt.ToString() + " is incompatible with single byte events!");
			}
			int num = -1;
			bool flag2 = (int)evt == 4;
			if (flag2)
			{
				num = 4;
			}
			bool flag3 = num != -1;
			if (flag3)
			{
				this.isEssential = true;
				this.blockState |= (byte)num;
				this.hasChangedState = true;
			}
			bool flag4 = this.sendEntity.eventCount + 2 < SendEntity.MAX_EVENTS;
			if (flag4)
			{
				this.sendEntity.AddEvent((byte)evt);
				this.sendEntity.AddEvent(eventData);
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000D954 File Offset: 0x0000BB54
		public override void Event(NetworkEntity.EntityEvent evt)
		{
			int num = -1;
			if ((int)evt <= 1)
			{
				if (evt == null)
				{
					num = 2;
					bool isBlock = this.isBlock;
					if (isBlock)
					{
						bool flag = (int)this.blockBehaviour.Prefab.Type == 36;
						if (flag)
						{
							BreakOnForceBoulder component = base.GetComponent<BreakOnForceBoulder>();
							component.Break();
							this.BreakIntoChildren(component.BrokenInstance);
						}
						this.serverMachine.ApplyDamage(this.blockBehaviour, (MachineDamageType)5);
					}
					goto IL_D1;
				}
				if ((int)evt == 1)
				{
					num = 1;
					goto IL_D1;
				}
			}
			else
			{
				if ((int)evt == 6)
				{
					num = 8;
					goto IL_D1;
				}
				if ((int)evt == 8)
				{
					num = 32;
					this.pollTransform = false;
					goto IL_D1;
				}
				if ((int)evt == 11)
				{
					num = 64;
					goto IL_D1;
				}
			}
			bool flag2 = evt == null || (int)evt == 3;
			if (flag2)
			{
				num = 2;
				bool flag3 = evt == 0;
				if (flag3)
				{
					this.pollTransform = false;
				}
			}
			IL_D1:
			bool flag4 = num != -1;
			if (flag4)
			{
				this.isEssential = true;
				this.blockState |= (byte)num;
				this.hasChangedState = true;
			}
			bool flag5 = (int)evt == 7;
			if (flag5)
			{
				int num2 = -5;
				byte b = (byte)num2;
				this.blockState &= b;
			}
			bool flag6 = this.sendEntity.eventCount + 1 < SendEntity.MAX_EVENTS;
			if (flag6)
			{
				this.sendEntity.AddEvent((byte)evt);
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000DAB0 File Offset: 0x0000BCB0
		protected void BreakOff(uint frame)
		{
			bool flag = !this.isBaseBlock;
			if (flag)
			{
				bool flag2 = !this.isBaseBlock && !this.isTracking;
				if (flag2)
				{
					base.transform.SetParent(base.transform.parent.parent, true);
				}
				this.isBreakingOff = true;
				this.isBaseBlock = true;
				this.baseBlock = base.transform;
				this.baseAdNetworkBlock = this;
				this.blockBehaviour = base.GetComponent<BlockBehaviour>();
				this.blockBehaviour.ClusterIndex = -1;
				bool flag3 = !this.isTracking;
				if (flag3)
				{
					this.posTracker.prevVec = ((this.lastPosFrame <= frame) ? this.baseBlock.TransformPoint(this.posTracker.lastVec) : this.posTracker.lastVec);
					this.posTracker.Vector = this.trackTransform.position;
					this.rotTracker.prevRot = ((this.lastRotFrame <= frame) ? (this.baseAdNetworkBlock.rotation * this.rotTracker.lastRot) : this.posTracker.lastRot);
					this.rotTracker.Rotation = this.trackTransform.rotation;
					this.UpdateBaseInterval();
				}
				else
				{
					this.Event((EntityEvent)1);
				}
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000DC08 File Offset: 0x0000BE08
		protected void OnJointBreak(float breakForce)
		{
			bool flag = this.isTracking && this.isBlock;
			if (flag)
			{
				this.serverMachine.DamageController.ApplyJointDamage(AdNetworkBlock.jointDamageValue);
				this.serverMachine.ApplyDamage(this.blockBehaviour, (MachineDamageType)1);
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000DC58 File Offset: 0x0000BE58
		private void OnVisualBreak()
		{
			bool flag = !StatMaster.isClient && this.isBlock;
			if (flag)
			{
				this.Event((EntityEvent)3);
				this.serverMachine.ApplyDamage(this.blockBehaviour, (MachineDamageType) 5);
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00002191 File Offset: 0x00000391
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt, byte[] data)
		{
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000DC98 File Offset: 0x0000BE98
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt, byte data)
		{
			bool flag = !this.networkCheck;
			if (!flag)
			{
				if ((int)evt <= 9)
				{
					if ((int)evt != 4)
					{
						if ((int)evt == 9)
						{
							bool hasWheelSmoke = this.hasWheelSmoke;
							if (hasWheelSmoke)
							{
								this.wheelSmoke.ToggleSmoke(data == 1);
							}
						}
					}
					else
					{
						bool flag2 = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 59;
						if (flag2)
						{
							bool activeInHierarchy = base.gameObject.activeInHierarchy;
							if (activeInHierarchy)
							{
								(this.blockBehaviour as TimedRocket).Fire(0f);
							}
						}
						else
						{
							bool flag3 = this.fireController != null;
							if (flag3)
							{
								float fireDuration = (float)data / 255f * this.fireController.randomAmount * 2f - this.fireController.randomAmount;
								this.fireController.SetFireDuration(fireDuration);
							}
							bool flag4 = this.fireTag != null;
							if (flag4)
							{
								this.fireTag.Ignite();
							}
						}
					}
				}
				else if ((int)evt != 14)
				{
					if ((int)evt != 24)
					{
						switch ((int)evt)
						{
						case 31:
						{
							bool hasCogMotorDamage = this.hasCogMotorDamage;
							if (hasCogMotorDamage)
							{
								bool flag5 = (int)this.blockBehaviour.Prefab.Type == 17;
								if (flag5)
								{
									this.cogMotorDamage.EmitSparksClient((float)data / 255f * 360f);
								}
								else
								{
									this.cogMotorDamage.EmitDrillSparksClient((float)data / 100f);
								}
							}
							break;
						}
						case 32:
						{
							bool flag6 = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 62;
							if (flag6)
							{
								(this.blockBehaviour as VacuumBlock).ToggleParticles(data == 1);
							}
							break;
						}
						case 33:
						{
							bool flag7 = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 27;
							if (flag7)
							{
								(this.blockBehaviour as GrabberBlock).joinOnTriggerBlock.PlayGrabSound(data == 1);
							}
							break;
						}
						case 34:
						{
							bool flag8 = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 73;
							if (flag8)
							{
								(this.blockBehaviour as BuildSurface).OnRemoteFragmentBreak((int)data);
							}
							break;
						}
						}
					}
					else
					{
						RandomSoundController componentInChildren = base.GetComponentInChildren<RandomSoundController>();
						bool flag9 = componentInChildren != null;
						if (flag9)
						{
							float num = (float)data / 255f;
							componentInChildren.Play2(num);
						}
					}
				}
				else
				{
					bool flag10 = this.isBlock && this.blockBehaviour.Prefab.hasBVC;
					if (flag10)
					{
						float damageLevel = (float)data / 255f;
						this.blockBehaviour.VisualController.SetDamageLevel(damageLevel);
					}
				}
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000DF84 File Offset: 0x0000C184
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt)
		{
			bool flag = !this.networkCheck;
			if (!flag)
			{
				bool isHosting = StatMaster.isHosting;
				if (isHosting)
				{
					this.Event(evt);
					this.sendEntity.eventCount = 0;
				}
				switch (evt)
				{
				case 0:
				{
					bool isBlock = this.isBlock;
					if (isBlock)
					{
						BlockType type = this.blockBehaviour.Prefab.Type;
						BlockType blockType = type;
						if ((int)blockType <= 36)
						{
							if ((int)blockType != 9)
							{
								if ((int)blockType != 36)
								{
									break;
								}
								BreakOnForceBoulder component = base.GetComponent<BreakOnForceBoulder>();
								component.Break();
								this.BreakIntoChildren(component.BrokenInstance);
								break;
							}
						}
						else
						{
							if ((int)blockType == 43)
							{
								(this.blockBehaviour as BalloonController).Pop();
								break;
							}
							if ((int)blockType != 45)
							{
								if ((int)blockType != 73)
								{
									break;
								}
								(this.blockBehaviour as BuildSurface).OnRemoteBreak();
								break;
							}
						}
						(this.blockBehaviour as SpringCode).Snap();
					}
					break;
				}
				case (EntityEvent)1:
					this.BreakOff(frame);
					break;
				case (EntityEvent)3:
				{
					bool isBlock2 = this.isBlock;
					if (isBlock2)
					{
						bool hasFragment = this.blockBehaviour.Prefab.hasFragment;
						if (hasFragment)
						{
							(this.blockBehaviour.VisualController as FragmentVisualController).OnJointBreak(0f);
						}
						else
						{
							bool flag2 = (int)this.blockBehaviour.Prefab.Type == 43;
							if (flag2)
							{
								(this.blockBehaviour as BalloonController).Snap();
							}
						}
					}
					break;
				}
				case (EntityEvent)5:
				{
					bool flag3 = this.fireTag != null;
					if (flag3)
					{
						this.fireTag.Ignite();
					}
					break;
				}
				case (EntityEvent)6:
				{
					bool flag4 = this.iceTag != null;
					if (flag4)
					{
						this.iceTag.Freeze();
					}
					else
					{
						Debug.LogWarning("Freezing '" + Machine.GetObjectPath(base.gameObject) + "', but no IceTag found!", base.gameObject);
					}
					break;
				}
				case (EntityEvent)7:
				{
					bool flag5 = this.fireController != null;
					if (flag5)
					{
						this.fireController.DouseFire();
					}
					break;
				}
				case (EntityEvent)8:
				{
					bool flag6 = !base.gameObject.activeInHierarchy;
					if (!flag6)
					{
						bool isBlock3 = this.isBlock;
						if (isBlock3)
						{
							TimedRocket timedRocket = this.blockBehaviour as TimedRocket;
							bool flag7 = timedRocket != null;
							if (flag7)
							{
								timedRocket.OnExplode();
							}
							ControllableBomb controllableBomb = this.blockBehaviour as ControllableBomb;
							bool flag8 = controllableBomb != null;
							if (flag8)
							{
								controllableBomb.ExplodeMessage();
							}
							ExplodeOnCollideBlock explodeOnCollideBlock = this.blockBehaviour as ExplodeOnCollideBlock;
							bool flag9 = explodeOnCollideBlock != null;
							if (flag9)
							{
								explodeOnCollideBlock.Explodey();
							}
						}
						else
						{
							ExplodeOnCollide componentInChildren = base.GetComponentInChildren<ExplodeOnCollide>();
							bool flag10 = componentInChildren != null;
							if (flag10)
							{
								componentInChildren.Explodey();
							}
							SmallExplosion componentInChildren2 = base.GetComponentInChildren<SmallExplosion>();
							bool flag11 = componentInChildren2 != null;
							if (flag11)
							{
								componentInChildren2.StartCoroutine(componentInChildren2.Explode());
							}
						}
					}
					break;
				}
				case (EntityEvent)10:
				{
					SoundOnCollide component2 = base.GetComponent<SoundOnCollide>();
					bool flag12 = component2 != null;
					if (flag12)
					{
						component2.PlaySound();
					}
					break;
				}
				case (EntityEvent)11:
				{
					bool isBlock4 = this.isBlock;
					if (isBlock4)
					{
						bool flag13 = (int)this.blockBehaviour.Prefab.Type == 7;
						if (flag13)
						{
							(this.blockBehaviour as BraceCode).RemoveBrace();
						}
						else
						{
							Debug.LogWarning("Trying to remove braces on block " + this.blockBehaviour.Prefab.Type.ToString() + "!");
						}
					}
					break;
				}
				case (EntityEvent)12:
				{
					ParticleOnCollide componentInChildren3 = base.GetComponentInChildren<ParticleOnCollide>();
					bool flag14 = componentInChildren3 != null;
					if (flag14)
					{
						componentInChildren3.PlayParticles();
					}
					break;
				}
				case (EntityEvent)20:
				{
					bool flag15 = this.isBlock && this.blockBehaviour.Prefab.hasBVC;
					if (flag15)
					{
						this.blockBehaviour.VisualController.SetBloodyLevel(1f);
					}
					break;
				}
				case (EntityEvent)23:
				{
					RandomSoundController componentInChildren4 = base.GetComponentInChildren<RandomSoundController>();
					bool flag16 = componentInChildren4 != null;
					if (flag16)
					{
						componentInChildren4.Play();
					}
					break;
				}
				case (EntityEvent)25:
				{
					RandomSoundController componentInChildren5 = base.GetComponentInChildren<RandomSoundController>();
					bool flag17 = componentInChildren5 != null;
					if (flag17)
					{
						componentInChildren5.Play3();
					}
					break;
				}
				case (EntityEvent)26:
				{
					RandomSoundController componentInChildren6 = base.GetComponentInChildren<RandomSoundController>();
					bool flag18 = componentInChildren6 != null;
					if (flag18)
					{
						componentInChildren6.Stop();
					}
					break;
				}
				}
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000E460 File Offset: 0x0000C660
		public static int GetMaxDataSize()
		{
			return 21;
		}

		// Token: 0x0400010C RID: 268
		private static float jointDamageValue = 4f;

		// Token: 0x0400010D RID: 269
		public AdNetworkBlock baseAdNetworkBlock;

		// Token: 0x0400010E RID: 270
		private Vector3 basePos;

		// Token: 0x0400010F RID: 271
		private float BASE_THRESHOLD = 1f;

		// Token: 0x04000110 RID: 272
		private float totalDamageAdded;

		// Token: 0x04000111 RID: 273
		private bool smokeActive;

		// Token: 0x04000112 RID: 274
		private int skipFrames;

		// Token: 0x04000113 RID: 275
		private int pollFrame;

		// Token: 0x04000114 RID: 276
		private bool posActive;

		// Token: 0x04000115 RID: 277
		private bool rotActive;

		// Token: 0x04000116 RID: 278
		private Matrix4x4 transformMatrix;

		// Token: 0x04000117 RID: 279
		private ServerMachine serverMachine;

		// Token: 0x04000118 RID: 280
		private bool isClusterBase;

		// Token: 0x04000119 RID: 281
		private int baseCurrentFrame;

		// Token: 0x0400011A RID: 282
		private bool isBreakingOff;

		// Token: 0x0400011B RID: 283
		private float baseThreshold = 1.5f;

		// Token: 0x0400011C RID: 284
		private float baseSqrThreshold;

		// Token: 0x0400011D RID: 285
		private float baseDist;

		// Token: 0x0400011E RID: 286
		private bool partOfCluster;

		// Token: 0x0400011F RID: 287
		private int lastSentPos;

		// Token: 0x04000120 RID: 288
		private int lastSentRot;

		// Token: 0x04000121 RID: 289
		private bool debug = false;
	}
}
