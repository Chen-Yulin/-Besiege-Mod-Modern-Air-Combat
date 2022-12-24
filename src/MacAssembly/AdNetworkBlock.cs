using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModernAirCombat
{
	// Token: 0x0200001C RID: 28
	public class AdNetworkBlock : NetworkBlock
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000042AF File Offset: 0x000024AF
		public bool ExExpand
		{
			get
			{
				return ModController.Instance.BoundaryOff;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x000042BB File Offset: 0x000024BB
		public bool networkCheck
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00010184 File Offset: 0x0000E384
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

		// Token: 0x060000F5 RID: 245 RVA: 0x00010230 File Offset: 0x0000E430
		//public void FixedUpdate()
		//{
		//	bool flag = this.debug;
		//	if (flag)
		//	{
		//		bool flag2 = base.transform.GetComponent<Rigidbody>();
		//		if (flag2)
		//		{
		//			Rigidbody component = base.transform.GetComponent<Rigidbody>();
		//			AdCustomModuleMod.Log("BlockRigidbody.maxAngularVelocity:" + component.maxAngularVelocity.ToString());
		//			this.debug = false;
		//		}
		//	}
		//}

		// Token: 0x060000F6 RID: 246 RVA: 0x00010294 File Offset: 0x0000E494
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

		// Token: 0x060000F7 RID: 247 RVA: 0x00010318 File Offset: 0x0000E518
		public override void UpdateTransforms()
		{
			bool flag = !this.isBlock || !this.blockBehaviour.isSimulating;
			if (flag)
			{
				base.UpdateTransforms();
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0001034C File Offset: 0x0000E54C
		public override void Init(uint blockIdentifier, NetworkController controller, Transform baseEnt, bool track)
		{
			this.AwakeBase();
			this.baseBlock = baseEnt;
			this.isBaseBlock = (this.baseBlock == base.transform);
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
					this.serverMachine = (this.blockBehaviour.ParentMachine as ServerMachine);
				}
				BlockType type = this.blockBehaviour.Prefab.Type;
				BlockType blockType = type;
				if (blockType <= BlockType.SingleWoodenBlock)
				{
					if (blockType <= BlockType.Brace)
					{
						if (blockType != BlockType.DoubleWoodenBlock)
						{
							if (blockType != BlockType.Brace)
							{
								goto IL_336;
							}
							bool stripped = this.blockBehaviour.stripped;
							if (stripped)
							{
								goto IL_336;
							}
							BraceCode braceCode = this.blockBehaviour as BraceCode;
							Vector3 localScale = braceCode.transform.localScale;
							Vector3 localScale2 = braceCode.cylinder.localScale;
							bool flag2 = BraceCode.BraceType(localScale, localScale2.y) == BraceState.Regular;
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
							goto IL_336;
						}
					}
					else
					{
						if (blockType == BlockType.Spring)
						{
							goto IL_214;
						}
						if (blockType != BlockType.SingleWoodenBlock)
						{
							goto IL_336;
						}
					}
				}
				else if (blockType <= BlockType.WoodenPole)
				{
					if (blockType == BlockType.Boulder)
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
						goto IL_336;
					}
					if (blockType != BlockType.WoodenPole)
					{
						goto IL_336;
					}
				}
				else
				{
					if (blockType == BlockType.RopeWinch)
					{
						goto IL_214;
					}
					if (blockType != BlockType.Log)
					{
						if (blockType != BlockType.BuildSurface)
						{
							goto IL_336;
						}
						this.fireTag = this.blockBehaviour.GetComponent<FireTag>();
						this.hasFireController = (this.fireTag != null);
						this.fireController = ((!this.hasFireController) ? null : this.fireTag.fireControllerCode);
						goto IL_336;
					}
				}
				FragmentVisualController fragmentVisualController = this.blockBehaviour.VisualController as FragmentVisualController;
				fragmentVisualController.onVisualBreak = (Action)Delegate.Combine(fragmentVisualController.onVisualBreak, new Action(this.OnVisualBreak));
				goto IL_336;
			IL_214:
				this.SetupSpring(this.blockBehaviour as SpringCode);
			IL_336:
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

		// Token: 0x060000F9 RID: 249 RVA: 0x00010750 File Offset: 0x0000E950
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

		// Token: 0x060000FA RID: 250 RVA: 0x0001081C File Offset: 0x0000EA1C
		public override void BreakIntoChildren(Transform breakInstance)
		{
			NetworkBlock component = breakInstance.GetComponent<NetworkBlock>();
			this.childManager.InitBlockChildren(component);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00010840 File Offset: 0x0000EA40
		private void SetupSpring(SpringCode springCode)
		{
			Transform startPoint = springCode.startPoint;
			Transform endPoint = springCode.endPoint;
			List<BlockLink> blockNeighbours = this.serverMachine.GetBlockNeighbours(this.blockBehaviour.NodeIndex);
			bool flag = false;
			bool flag2 = false;
			BlockBehaviour block = null;
			BlockBehaviour block2 = null;
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
							block = blockLink.Other.Block;
						}
						else
						{
							flag2 = true;
							block2 = blockLink.Other.Block;
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
					BlockBehaviour simBlock = this.serverMachine.GetSimBlock(block);
					endPoint.SetParent(simBlock.transform, true);
				}
				bool flag8 = flag2;
				if (flag8)
				{
					BlockBehaviour simBlock2 = this.serverMachine.GetSimBlock(block2);
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
					BlockBehaviour simBlock3 = this.serverMachine.GetSimBlock(block2);
					simBlock3.CreateSimLists();
					simBlock3.visAddedToMe.Add(springCode.winchRotateVis.GetComponent<Renderer>());
				}
				bool flag18 = flag;
				if (flag18)
				{
					BlockBehaviour simBlock4 = this.serverMachine.GetSimBlock(block);
					simBlock4.CreateSimLists();
					simBlock4.visAddedToMe.Add(springCode.winchRotateVis2.GetComponent<Renderer>());
				}
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00010BD8 File Offset: 0x0000EDD8
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

		// Token: 0x060000FD RID: 253 RVA: 0x00010C4C File Offset: 0x0000EE4C
		public new void SetupClientBlock()
		{
			BlockType type = this.blockBehaviour.Prefab.Type;
			bool flag = this.blockBehaviour.Prefab.hasBVC && this.blockBehaviour.Prefab.hasFragment;
			bool flag2 = type == BlockType.BuildSurface;
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
					blockNeighbours.ForEach(delegate (BlockLink x)
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
				bool flag3 = !flag && type != BlockType.Balloon;
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

		// Token: 0x060000FE RID: 254 RVA: 0x00010E60 File Offset: 0x0000F060
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

		// Token: 0x060000FF RID: 255 RVA: 0x00011158 File Offset: 0x0000F358
		public override int GetDataSize()
		{
			return NetworkEntity.GetDataSize((this.isBaseBlock ? ((this.hasChangedPos ? 1 : 0) | (this.hasChangedRot ? 2 : 0)) : 0) | (this.hasChangedState ? 4 : 0));
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000111A0 File Offset: 0x0000F3A0
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
					NetworkCompression.CompressPosition(this.posTracker.lastVec, buffer, offset);
					offset += 6;
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
				buffer[offset] = ((byte)(this.hasChangedState ? 4 : 0));
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

		// Token: 0x06000101 RID: 257 RVA: 0x00011290 File Offset: 0x0000F490
		public override int DecodeState(byte[] data, int offset)
		{
			int num = offset;
			int changed = (int)data[offset];
			offset++;
			bool flag = NetworkEntity.StateChanged(changed);
			if (flag)
			{
				this.ApplyState(data[offset]);
				offset++;
			}
			this.changedPos = (this.changedRot = false);
			bool flag2 = false;
			bool flag3 = NetworkEntity.PosChanged(changed);
			if (flag3)
			{
				NetworkCompression.DecompressPosition(data, offset, out this.posHolder);
				offset += 6;
				this.position = this.GetPos(this.posHolder);
				this.posTracker.Override(this.position, this.position);
				flag2 = true;
				this.trackTransform.position = this.position;
			}
			bool flag4 = NetworkEntity.RotChanged(changed);
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

		// Token: 0x06000102 RID: 258 RVA: 0x000113C0 File Offset: 0x0000F5C0
		protected override void ApplyState(byte state)
		{
			NetworkBlock.applyingState = true;
			bool flag = base.ContainsState(state, NetworkBlock.BlockState.BaseBlock);
			if (flag)
			{
				this.SetEvent(0U, NetworkEntity.EntityEvent.Base);
			}
			bool flag2 = base.ContainsState(state, NetworkBlock.BlockState.Burned);
			if (flag2)
			{
				this.SetEvent(0U, NetworkEntity.EntityEvent.Ignite, 0);
			}
			bool flag3 = base.ContainsState(state, NetworkBlock.BlockState.Killed);
			if (flag3)
			{
				this.SetEvent(0U, NetworkEntity.EntityEvent.Kill);
			}
			bool flag4 = base.ContainsState(state, NetworkBlock.BlockState.Broken);
			if (flag4)
			{
				this.SetEvent(0U, NetworkEntity.EntityEvent.Break);
			}
			bool flag5 = base.ContainsState(state, NetworkBlock.BlockState.Frozen);
			if (flag5)
			{
				this.SetEvent(0U, NetworkEntity.EntityEvent.Freeze);
			}
			bool flag6 = base.ContainsState(state, NetworkBlock.BlockState.Exploded);
			if (flag6)
			{
				this.SetEvent(0U, NetworkEntity.EntityEvent.Explode);
			}
			NetworkBlock.applyingState = false;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0001146C File Offset: 0x0000F66C
		public new void RegisterJointDamage(ConfigurableJoint[] configJoints, HingeJoint[] hingeJoints)
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

		// Token: 0x06000104 RID: 260 RVA: 0x00011520 File Offset: 0x0000F720
		protected override void OnDestroy()
		{
			this.isDestroyed = true;
			bool flag = this.isTracking && this.isBlock;
			if (flag)
			{
				this.serverMachine.DamageController.RemoveTotalDamage(this.totalDamageAdded);
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00011564 File Offset: 0x0000F764
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
				Quaternion rot;
				if (isBaseBlock)
				{
					this.position = this.trackTransform.position;
					vector = NetworkEntity.ClampPosition(this.position);
					this.rotation = this.trackTransform.rotation;
					rot = this.rotation;
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
							this.serverMachine.ApplyDamage(this.blockBehaviour, MachineDamageType.ClusterLeave);
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
						rot = this.rotation;
					}
					else
					{
						this.rotation = this.trackTransform.rotation;
						rot = this.baseAdNetworkBlock.transformRotation * this.rotation;
					}
				}
				int num6 = 0;
				bool flag12 = this.hasWheelSmoke && this.smokeActive != this.wheelSmoke.smokeActive;
				if (flag12)
				{
					this.smokeActive = this.wheelSmoke.smokeActive;
					this.Event(NetworkEntity.EntityEvent.ToggleSmoke, (byte)(this.smokeActive ? 1 : 0));
				}
				bool flag13 = this.hasCogMotorDamage && this.cogMotorDamage.hasEmitted;
				if (flag13)
				{
					bool flag14 = this.blockBehaviour.Prefab.Type == BlockType.CircularSaw;
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
					this.Event(NetworkEntity.EntityEvent.EmitSparks, (byte)num7);
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
							rot = this.rotation;
						}
					}
					bool flag20 = !flag3 && this.isBreakingOff;
					if (flag20)
					{
						bool flag21 = this.baseCurrentFrame < OptionsMaster.baseEventFrames;
						if (flag21)
						{
							this.Event(NetworkEntity.EntityEvent.Base);
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
				bool flag25 = flag3 || flag24 || !this.rotTracker.WithinThreshold(rot);
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
					NetworkCompression.CompressRotation(rot, data, offset);
					offset += 7;
					num6 |= 4;
					this.rotTracker.Store(rot);
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

		// Token: 0x06000106 RID: 262 RVA: 0x00011D5C File Offset: 0x0000FF5C
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
				NetworkEntity.EntityEvent entityEvent = (NetworkEntity.EntityEvent)data[num + i];
				bool flag2 = entityEvent == NetworkEntity.EntityEvent.Base;
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
							this.SetEvent(frame, NetworkEntity.EntityEvent.Base);
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
				NetworkEntity.EntityEvent entityEvent2 = (NetworkEntity.EntityEvent)data[num + i];
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
					bool flag9 = entityEvent2 != NetworkEntity.EntityEvent.Base;
					if (flag9)
					{
						this.SetEvent(frame, entityEvent2);
					}
				}
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00012030 File Offset: 0x00010230
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

		// Token: 0x06000108 RID: 264 RVA: 0x00012068 File Offset: 0x00010268
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

		// Token: 0x06000109 RID: 265 RVA: 0x000120A0 File Offset: 0x000102A0
		protected new bool SkipFrame()
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

		// Token: 0x0600010A RID: 266 RVA: 0x000120F0 File Offset: 0x000102F0
		public override void NewFrame(uint frame)
		{
			bool flag = this.isBaseBlock || !this.SkipFrame();
			if (flag)
			{
				base.NewFrame(frame);
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00012120 File Offset: 0x00010320
		public override void Event(NetworkEntity.EntityEvent evt, byte[] eventData)
		{
			bool flag = base.DataLength(evt) < 2;
			if (flag)
			{
				//AdCustomModuleMod.Warning("Event " + evt.ToString() + " should be a 1 byte event!");
			}
			bool flag2 = this.sendEntity.eventCount + 1 + eventData.Length < SendEntity.MAX_EVENTS;
			if (flag2)
			{
				this.sendEntity.AddEvent((byte)evt);
				for (int i = 0; i < eventData.Length; i++)
				{
					this.sendEntity.AddEvent(eventData[i]);
				}
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000121B0 File Offset: 0x000103B0
		public override void Event(NetworkEntity.EntityEvent evt, byte eventData)
		{
			bool flag = base.DataLength(evt) != 1;
			if (flag)
			{
				//AdCustomModuleMod.Warning("Event " + evt.ToString() + " is incompatible with single byte events!");
			}
			int num = -1;
			bool flag2 = evt == NetworkEntity.EntityEvent.Ignite;
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

		// Token: 0x0600010D RID: 269 RVA: 0x00012264 File Offset: 0x00010464
		public override void Event(NetworkEntity.EntityEvent evt)
		{
			int num = -1;
			if (evt <= NetworkEntity.EntityEvent.Base)
			{
				if (evt == NetworkEntity.EntityEvent.Break)
				{
					num = 2;
					bool isBlock = this.isBlock;
					if (isBlock)
					{
						bool flag = this.blockBehaviour.Prefab.Type == BlockType.Boulder;
						if (flag)
						{
							BreakOnForceBoulder component = base.GetComponent<BreakOnForceBoulder>();
							component.Break();
							this.BreakIntoChildren(component.BrokenInstance);
						}
						this.serverMachine.ApplyDamage(this.blockBehaviour, MachineDamageType.Break);
					}
					goto IL_D1;
				}
				if (evt == NetworkEntity.EntityEvent.Base)
				{
					num = 1;
					goto IL_D1;
				}
			}
			else
			{
				if (evt == NetworkEntity.EntityEvent.Freeze)
				{
					num = 8;
					goto IL_D1;
				}
				if (evt == NetworkEntity.EntityEvent.Explode)
				{
					num = 32;
					this.pollTransform = false;
					goto IL_D1;
				}
				if (evt == NetworkEntity.EntityEvent.Kill)
				{
					num = 64;
					goto IL_D1;
				}
			}
			bool flag2 = evt == NetworkEntity.EntityEvent.Break || evt == NetworkEntity.EntityEvent.VisBreak;
			if (flag2)
			{
				num = 2;
				bool flag3 = evt == NetworkEntity.EntityEvent.Break;
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
			bool flag5 = evt == NetworkEntity.EntityEvent.Douse;
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

		// Token: 0x0600010E RID: 270 RVA: 0x000123C0 File Offset: 0x000105C0
		protected new void BreakOff(uint frame)
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
					this.Event(NetworkEntity.EntityEvent.Base);
				}
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00012518 File Offset: 0x00010718
		protected new void OnJointBreak(float breakForce)
		{
			bool flag = this.isTracking && this.isBlock;
			if (flag)
			{
				this.serverMachine.DamageController.ApplyJointDamage(AdNetworkBlock.jointDamageValue);
				this.serverMachine.ApplyDamage(this.blockBehaviour, MachineDamageType.JointBreak);
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00012568 File Offset: 0x00010768
		private void OnVisualBreak()
		{
			bool flag = !StatMaster.isClient && this.isBlock;
			if (flag)
			{
				this.Event(NetworkEntity.EntityEvent.VisBreak);
				this.serverMachine.ApplyDamage(this.blockBehaviour, MachineDamageType.Break);
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00003D71 File Offset: 0x00001F71
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt, byte[] data)
		{
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000125A8 File Offset: 0x000107A8
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt, byte data)
		{
			bool flag = !this.networkCheck;
			if (!flag)
			{
				if (evt <= NetworkEntity.EntityEvent.ToggleSmoke)
				{
					if (evt != NetworkEntity.EntityEvent.Ignite)
					{
						if (evt == NetworkEntity.EntityEvent.ToggleSmoke)
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
						bool flag2 = this.isBlock && this.blockBehaviour.Prefab.Type == BlockType.Rocket;
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
				else if (evt != NetworkEntity.EntityEvent.SetDamageLevel)
				{
					if (evt != NetworkEntity.EntityEvent.RSCPlay2)
					{
						switch (evt)
						{
							case NetworkEntity.EntityEvent.EmitSparks:
								{
									bool hasCogMotorDamage = this.hasCogMotorDamage;
									if (hasCogMotorDamage)
									{
										bool flag5 = this.blockBehaviour.Prefab.Type == BlockType.CircularSaw;
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
							case NetworkEntity.EntityEvent.ToggleVacuum:
								{
									bool flag6 = this.isBlock && this.blockBehaviour.Prefab.Type == BlockType.Vacuum;
									if (flag6)
									{
										(this.blockBehaviour as VacuumBlock).ToggleParticles(data == 1);
									}
									break;
								}
							case NetworkEntity.EntityEvent.PlayGrabSound:
								{
									bool flag7 = this.isBlock && this.blockBehaviour.Prefab.Type == BlockType.Grabber;
									if (flag7)
									{
										(this.blockBehaviour as GrabberBlock).joinOnTriggerBlock.PlayGrabSound(data == 1);
									}
									break;
								}
							case NetworkEntity.EntityEvent.SurfaceFragmentBreak:
								{
									bool flag8 = this.isBlock && this.blockBehaviour.Prefab.Type == BlockType.BuildSurface;
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
							float volume = (float)data / 255f;
							componentInChildren.Play2(volume);
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

		// Token: 0x06000113 RID: 275 RVA: 0x00012894 File Offset: 0x00010A94
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
					case NetworkEntity.EntityEvent.Break:
						{
							bool isBlock = this.isBlock;
							if (isBlock)
							{
								BlockType type = this.blockBehaviour.Prefab.Type;
								BlockType blockType = type;
								if (blockType <= BlockType.Balloon)
								{
									if (blockType != BlockType.Spring)
									{
										if (blockType == BlockType.Boulder)
										{
											BreakOnForceBoulder component = base.GetComponent<BreakOnForceBoulder>();
											component.Break();
											this.BreakIntoChildren(component.BrokenInstance);
											break;
										}
										if (blockType != BlockType.Balloon)
										{
											break;
										}
										(this.blockBehaviour as BalloonController).Pop();
										break;
									}
								}
								else if (blockType != BlockType.RopeWinch)
								{
									if (blockType == BlockType.BuildSurface)
									{
										(this.blockBehaviour as BuildSurface).OnRemoteBreak();
										break;
									}
									if (blockType != BlockType.SqrBalloon)
									{
										break;
									}
									(this.blockBehaviour as SqrBalloonController).Pop();
									break;
								}
								(this.blockBehaviour as SpringCode).Snap();
							}
							break;
						}
					case NetworkEntity.EntityEvent.Base:
						this.BreakOff(frame);
						break;
					case NetworkEntity.EntityEvent.VisBreak:
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
									bool flag2 = this.blockBehaviour.Prefab.Type == BlockType.Balloon;
									if (flag2)
									{
										(this.blockBehaviour as BalloonController).Snap();
									}
								}
							}
							break;
						}
					case NetworkEntity.EntityEvent.IgniteBurning:
						{
							bool flag3 = this.fireTag != null;
							if (flag3)
							{
								this.fireTag.Ignite();
							}
							break;
						}
					case NetworkEntity.EntityEvent.Freeze:
						{
							bool flag4 = this.iceTag != null;
							if (flag4)
							{
								this.iceTag.Freeze();
							}
							else
							{
								//AdCustomModuleMod.Warning("Freezing '" + Machine.GetObjectPath(base.gameObject) + "', but no IceTag found!");
							}
							break;
						}
					case NetworkEntity.EntityEvent.Douse:
						{
							bool flag5 = this.fireController != null;
							if (flag5)
							{
								this.fireController.DouseFire();
							}
							break;
						}
					case NetworkEntity.EntityEvent.Explode:
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
					case NetworkEntity.EntityEvent.SoundOnCollide:
						{
							SoundOnCollide component2 = base.GetComponent<SoundOnCollide>();
							bool flag12 = component2 != null;
							if (flag12)
							{
								component2.PlaySound();
							}
							break;
						}
					case NetworkEntity.EntityEvent.Kill:
						{
							bool isBlock4 = this.isBlock;
							if (isBlock4)
							{
								bool flag13 = this.blockBehaviour.Prefab.Type == BlockType.Brace;
								if (flag13)
								{
									(this.blockBehaviour as BraceCode).RemoveBrace();
								}
								else
								{
									//AdCustomModuleMod.Warning("Trying to remove braces on block " + this.blockBehaviour.Prefab.Type.ToString() + "!");
								}
							}
							break;
						}
					case NetworkEntity.EntityEvent.ParticleOnCollide:
						{
							ParticleOnCollide componentInChildren3 = base.GetComponentInChildren<ParticleOnCollide>();
							bool flag14 = componentInChildren3 != null;
							if (flag14)
							{
								componentInChildren3.PlayParticles();
							}
							break;
						}
					case NetworkEntity.EntityEvent.SetBloodyLevel:
						{
							bool flag15 = this.isBlock && this.blockBehaviour.Prefab.hasBVC;
							if (flag15)
							{
								this.blockBehaviour.VisualController.SetBloodyLevel(1f);
							}
							break;
						}
					case NetworkEntity.EntityEvent.RSCPlay:
						{
							RandomSoundController componentInChildren4 = base.GetComponentInChildren<RandomSoundController>();
							bool flag16 = componentInChildren4 != null;
							if (flag16)
							{
								componentInChildren4.Play();
							}
							break;
						}
					case NetworkEntity.EntityEvent.RSCPlay3:
						{
							RandomSoundController componentInChildren5 = base.GetComponentInChildren<RandomSoundController>();
							bool flag17 = componentInChildren5 != null;
							if (flag17)
							{
								componentInChildren5.Play3();
							}
							break;
						}
					case NetworkEntity.EntityEvent.RSCStop:
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

		// Token: 0x06000114 RID: 276 RVA: 0x00012D88 File Offset: 0x00010F88
		public new static int GetMaxDataSize()
		{
			return 21;
		}

		// Token: 0x0400015A RID: 346
		private static float jointDamageValue = 4f;

		// Token: 0x0400015B RID: 347
		public AdNetworkBlock baseAdNetworkBlock;

		// Token: 0x0400015C RID: 348
		private Vector3 basePos;

		// Token: 0x0400015D RID: 349
		private float BASE_THRESHOLD = 1f;

		// Token: 0x0400015E RID: 350
		private float totalDamageAdded;

		// Token: 0x0400015F RID: 351
		private bool smokeActive;

		// Token: 0x04000160 RID: 352
		private int skipFrames;

		// Token: 0x04000161 RID: 353
		private int pollFrame;

		// Token: 0x04000162 RID: 354
		private bool posActive;

		// Token: 0x04000163 RID: 355
		private bool rotActive;

		// Token: 0x04000164 RID: 356
		private Matrix4x4 transformMatrix;

		// Token: 0x04000165 RID: 357
		private ServerMachine serverMachine;

		// Token: 0x04000166 RID: 358
		private bool isClusterBase;

		// Token: 0x04000167 RID: 359
		private int baseCurrentFrame;

		// Token: 0x04000168 RID: 360
		private bool isBreakingOff;

		// Token: 0x04000169 RID: 361
		private float baseThreshold = 1.5f;

		// Token: 0x0400016A RID: 362
		private float baseSqrThreshold;

		// Token: 0x0400016B RID: 363
		private float baseDist;

		// Token: 0x0400016C RID: 364
		private bool partOfCluster;

		// Token: 0x0400016D RID: 365
		private int lastSentPos;

		// Token: 0x0400016E RID: 366
		private int lastSentRot;

		// Token: 0x0400016F RID: 367
		private bool debug = false;
	}
}

