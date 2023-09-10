using System;
using System.Linq;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000059 RID: 89
	public class AdNetworkProjectile : NetworkBlock
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001EC RID: 492 RVA: 0x00002366 File Offset: 0x00000566
		public bool ExExpand
		{
			get
			{
				return AdCustomModuleMod.mod4.ExExpandFloor;
			}
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0001F838 File Offset: 0x0001DA38
		protected override void AwakeBase()
		{
			bool isAwake = this.isAwake;
			if (!isAwake)
			{
				base.AwakeBase();
				this.hasGyro = false;
				this.projectileScript = base.GetComponent<AdProjectileScript>();
				this.hasProjectileScript = this.projectileScript != null;
				bool flag = this.hasProjectileScript;
				if (flag)
				{
					this.hasGyro = this.projectileScript.gyro != null;
					bool flag2 = this.hasGyro;
					if (flag2)
					{
						this.trackTransform = this.projectileScript.gyro;
					}
				}
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000B990 File Offset: 0x00009B90
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

		// Token: 0x060001EF RID: 495 RVA: 0x0000BA14 File Offset: 0x00009C14
		public override void UpdateTransforms()
		{
			bool flag = !this.isBlock || !this.blockBehaviour.isSimulating;
			if (flag)
			{
				base.UpdateTransforms();
			}
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0001F8C0 File Offset: 0x0001DAC0
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
				this.baseAdNetworkBlock = this.baseBlock.GetComponent<AdNetworkProjectile>();
			}
			this.Init(blockIdentifier, controller, track);
			this.ResetTransformData();
			bool flag2 = track && this.pollTransform;
			if (flag2)
			{
				this.pollFrame = (int)((ulong)blockIdentifier % (ulong)((long)(NetworkScene.ServerSettings.skipChildCount + 1)));
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0001F97C File Offset: 0x0001DB7C
		public override void BreakIntoChildren(Transform breakInstance)
		{
			AdNetworkBlock component = breakInstance.GetComponent<AdNetworkBlock>();
			this.childManager.InitBlockChildren(component);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0001F9A0 File Offset: 0x0001DBA0
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

		// Token: 0x060001F3 RID: 499 RVA: 0x0001FA14 File Offset: 0x0001DC14
		private bool IsPlayerProjectile(NetworkProjectileType type)
		{
			int length = Enum.GetValues(typeof(NetworkProjectileType)).Length;
			return type == null || (int)type == 3 || (int)type >= length;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0001FA4C File Offset: 0x0001DC4C
		protected virtual void SetParentMachine(ushort playerId)
		{
			ServerMachine parentMachine = null;
			bool machine = NetworkScene.Instance.GetMachine(playerId, out parentMachine);
			if (machine)
			{
				this.projectileInfo.SetParentMachine(parentMachine);
			}
			else
			{
				this.projectileInfo.ResetParentMachine();
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0001FA90 File Offset: 0x0001DC90
		public virtual void Spawn(uint frame, ushort playerId, ushort targetId, ushort skinId, float thrustdelayTime, byte[] spawnInfo)
		{
			int num = 0;
			this.playerId = playerId;
			this.targetId = targetId;
			this.skinId = skinId;
			this.projectileInfo = base.GetComponent<ProjectileInfo>();
			this.SetParentMachine(playerId);
			Vector3 zero = Vector3.zero;
			bool exExpand = this.ExExpand;
			if (exExpand)
			{
				AdNetworkCompression.DecompressPosition(spawnInfo, num, out zero);
				num += 12;
			}
			else
			{
				NetworkCompression.DecompressPosition(spawnInfo, num, out zero);
				num += 6;
			}
			base.transform.position = zero;
			Quaternion identity = Quaternion.identity;
			NetworkCompression.DecompressRotation(spawnInfo, num, out identity);
			base.transform.rotation = identity;
			bool noRigidbody = this.projectileInfo.noRigidbody;
			if (noRigidbody)
			{
				this.projectileInfo.Rigidbody = base.gameObject.AddComponent<Rigidbody>();
				this.projectileInfo.noRigidbody = false;
			}
			Rigidbody rigidbody = this.projectileInfo.Rigidbody;
			bool isHosting = StatMaster.isHosting;
			if (isHosting)
			{
				rigidbody.isKinematic = this.bodyKinematic;
				rigidbody.mass = this.bodyMass;
				rigidbody.drag = this.bodyDrag;
				rigidbody.angularDrag = this.bodyAngularDrag;
				rigidbody.interpolation = this.bodyInterpolation;
				rigidbody.collisionDetectionMode = this.bodyCollisionMode;
			}
			else
			{
				rigidbody.isKinematic = true;
				base.gameObject.SetActive(true);
			}
			this.turnOffOnDisable = false;
			bool flag = this.hasProjectileScript;
			if (flag)
			{
				this.projectileScript.ownerID = (int)playerId;
				this.projectileScript.TargetId = (int)targetId;
				this.projectileScript.skinID = (int)skinId;
				this.projectileScript.DelayBoostertime = thrustdelayTime;
				KeyMachingTable keyMachingTable = AdCustomModuleMod.mod2.ProjectileSkinTable.List.FirstOrDefault((KeyMachingTable x) => x.Name == this.BlockName);
				KeyMachingTable keyMachingTable2 = keyMachingTable.List.FirstOrDefault((KeyMachingTable c) => c.Key == (int)skinId);
				bool flag2 = keyMachingTable2 == null;
				if (flag2)
				{
					this.projectileScript.SkinName = "DEFAULT";
				}
				else
				{
					this.projectileScript.SkinName = keyMachingTable2.Name;
				}
				this.projectileScript.enabled = true;
				Vector3 zero2 = Vector3.zero;
				bool exExpand2 = this.ExExpand;
				if (exExpand2)
				{
					bool flag3 = spawnInfo.Length != 19;
					if (flag3)
					{
						num += 7;
						NetworkCompression.DecompressVector(spawnInfo, num, 0f, 100f, out zero2);
						this.projectileScript.SetScale(zero2);
					}
				}
				else
				{
					bool flag4 = spawnInfo.Length != 13;
					if (flag4)
					{
						num += 7;
						NetworkCompression.DecompressVector(spawnInfo, num, 0f, 100f, out zero2);
						this.projectileScript.SetScale(zero2);
					}
				}
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00002191 File Offset: 0x00000391
		public virtual void Despawn(byte[] despawnInfo)
		{
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0001FD5C File Offset: 0x0001DF5C
		public virtual void ReturnToPool()
		{
			this.myTransform.localRotation = Quaternion.identity;
			this.myTransform.localScale = Vector3.one;
			bool flag = this.hasGyro && !this.hasChangedState;
			if (flag)
			{
				Transform transform = this.projectileScript.gyro.transform;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0001FDDC File Offset: 0x0001DFDC
		public override int PollObject(bool fullUpdate, byte[] data, int offset)
		{
			int num = offset;
			offset++;
			int eventCount = this.sendEntity.eventCount;
			int num2 = 6;
			this.position = this.trackTransform.position;
			Vector3 vector = NetworkEntity.ClampPosition(this.position);
			this.rotation = this.trackTransform.rotation;
			Quaternion rotation = this.rotation;
			int num3 = 0;
			bool flag = false;
			bool flag2 = !this.posTracker.WithinThreshold(vector);
			if (flag2)
			{
				bool exExpand = this.ExExpand;
				if (exExpand)
				{
					bool flag3 = eventCount > 0;
					if (flag3)
					{
						Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount);
						this.sendEntity.eventCount = 0;
						offset += eventCount;
						num3 |= eventCount + num2 << 3;
					}
					else
					{
						num3 |= num2 << 3;
					}
					flag = true;
					AdNetworkCompression.CompressPosition(vector, data, offset);
					offset += 12;
				}
				else
				{
					bool flag4 = eventCount > 0;
					if (flag4)
					{
						Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount);
						this.sendEntity.eventCount = 0;
						offset += eventCount;
						num3 |= eventCount << 3;
					}
					flag = true;
					NetworkCompression.CompressPosition(vector, data, offset);
					offset += 6;
				}
				num3 |= 1;
				this.posTracker.Store(vector);
				this.hasChangedPos = true;
			}
			bool flag5 = !flag && eventCount > 0;
			if (flag5)
			{
				Buffer.BlockCopy(this.sendEntity.EventList, 0, data, offset, eventCount);
				this.sendEntity.eventCount = 0;
				offset += eventCount;
				num3 |= eventCount << 3;
			}
			bool flag6 = !this.rotTracker.WithinThreshold(rotation);
			if (flag6)
			{
				NetworkCompression.CompressRotation(rotation, data, offset);
				offset += 7;
				num3 |= 4;
				this.rotTracker.Store(rotation);
				this.hasChangedRot = true;
			}
			data[num] = (byte)num3;
			bool turningOff = this.turningOff;
			if (turningOff)
			{
				this.pollTransform = false;
				this.turningOff = false;
			}
			return offset - num;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0001FFD4 File Offset: 0x0001E1D4
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
					this.posTracker.Set(this.posHolder);
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
					this.rotTracker.Set(this.rotHolder);
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

		// Token: 0x060001FA RID: 506 RVA: 0x0002028C File Offset: 0x0001E48C
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

		// Token: 0x060001FB RID: 507 RVA: 0x000202C4 File Offset: 0x0001E4C4
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

		// Token: 0x060001FC RID: 508 RVA: 0x000202FC File Offset: 0x0001E4FC
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

		// Token: 0x060001FD RID: 509 RVA: 0x0002034C File Offset: 0x0001E54C
		public override void NewFrame(uint frame)
		{
			bool flag = this.isBaseBlock || !this.SkipFrame();
			if (flag)
			{
				base.NewFrame(frame);
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000D810 File Offset: 0x0000BA10
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
				this.sendEntity.AddEvent((byte)evt);
				for (int i = 0; i < eventData.Length; i++)
				{
					this.sendEntity.AddEvent(eventData[i]);
				}
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000D8A0 File Offset: 0x0000BAA0
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

		// Token: 0x06000200 RID: 512 RVA: 0x0002037C File Offset: 0x0001E57C
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

		// Token: 0x06000201 RID: 513 RVA: 0x000204D8 File Offset: 0x0001E6D8
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

		// Token: 0x06000202 RID: 514 RVA: 0x00002191 File Offset: 0x00000391
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt, byte[] data)
		{
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00020630 File Offset: 0x0001E830
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt, byte data)
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
					bool flag = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 59;
					if (flag)
					{
						bool activeInHierarchy = base.gameObject.activeInHierarchy;
						if (activeInHierarchy)
						{
							(this.blockBehaviour as TimedRocket).Fire(0f);
						}
					}
					else
					{
						bool flag2 = this.fireController != null;
						if (flag2)
						{
							float fireDuration = (float)data / 255f * this.fireController.randomAmount * 2f - this.fireController.randomAmount;
							this.fireController.SetFireDuration(fireDuration);
						}
						bool flag3 = this.fireTag != null;
						if (flag3)
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
							bool flag4 = (int)this.blockBehaviour.Prefab.Type == 17;
							if (flag4)
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
						bool flag5 = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 62;
						if (flag5)
						{
							(this.blockBehaviour as VacuumBlock).ToggleParticles(data == 1);
						}
						break;
					}
					case 33:
					{
						bool flag6 = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 27;
						if (flag6)
						{
							(this.blockBehaviour as GrabberBlock).joinOnTriggerBlock.PlayGrabSound(data == 1);
						}
						break;
					}
					case 34:
					{
						bool flag7 = this.isBlock && (int)this.blockBehaviour.Prefab.Type == 73;
						if (flag7)
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
					bool flag8 = componentInChildren != null;
					if (flag8)
					{
						float num = (float)data / 255f;
						componentInChildren.Play2(num);
					}
				}
			}
			else
			{
				bool flag9 = this.isBlock && this.blockBehaviour.Prefab.hasBVC;
				if (flag9)
				{
					float damageLevel = (float)data / 255f;
					this.blockBehaviour.VisualController.SetDamageLevel(damageLevel);
				}
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00020908 File Offset: 0x0001EB08
		public override void SetEvent(uint frame, NetworkEntity.EntityEvent evt)
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
					if (blockType <= (BlockType)36)
					{
						if (blockType != (BlockType)9)
						{
							if (blockType != (BlockType)36)
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
						if (blockType != (BlockType)45)
						{
							if (blockType != (BlockType)73)
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
						bool flag = (int)this.blockBehaviour.Prefab.Type == 43;
						if (flag)
						{
							(this.blockBehaviour as BalloonController).Snap();
						}
					}
				}
				break;
			}
			case (EntityEvent)5:
			{
				bool flag2 = this.fireTag != null;
				if (flag2)
				{
					this.fireTag.Ignite();
				}
				break;
			}
			case (EntityEvent)6:
			{
				bool flag3 = this.iceTag != null;
				if (flag3)
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
				bool flag4 = this.fireController != null;
				if (flag4)
				{
					this.fireController.DouseFire();
				}
				break;
			}
			case (EntityEvent)8:
			{
				bool flag5 = !base.gameObject.activeInHierarchy;
				if (!flag5)
				{
					bool isBlock3 = this.isBlock;
					if (isBlock3)
					{
						TimedRocket timedRocket = this.blockBehaviour as TimedRocket;
						bool flag6 = timedRocket != null;
						if (flag6)
						{
							timedRocket.OnExplode();
						}
						ControllableBomb controllableBomb = this.blockBehaviour as ControllableBomb;
						bool flag7 = controllableBomb != null;
						if (flag7)
						{
							controllableBomb.ExplodeMessage();
						}
						ExplodeOnCollideBlock explodeOnCollideBlock = this.blockBehaviour as ExplodeOnCollideBlock;
						bool flag8 = explodeOnCollideBlock != null;
						if (flag8)
						{
							explodeOnCollideBlock.Explodey();
						}
					}
					else
					{
						ExplodeOnCollide componentInChildren = base.GetComponentInChildren<ExplodeOnCollide>();
						bool flag9 = componentInChildren != null;
						if (flag9)
						{
							componentInChildren.Explodey();
						}
						SmallExplosion componentInChildren2 = base.GetComponentInChildren<SmallExplosion>();
						bool flag10 = componentInChildren2 != null;
						if (flag10)
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
				bool flag11 = component2 != null;
				if (flag11)
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
					bool flag12 = (int)this.blockBehaviour.Prefab.Type == 7;
					if (flag12)
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
				bool flag13 = componentInChildren3 != null;
				if (flag13)
				{
					componentInChildren3.PlayParticles();
				}
				break;
			}
			case (EntityEvent)20:
			{
				bool flag14 = this.isBlock && this.blockBehaviour.Prefab.hasBVC;
				if (flag14)
				{
					this.blockBehaviour.VisualController.SetBloodyLevel(1f);
				}
				break;
			}
			case (EntityEvent)23:
			{
				RandomSoundController componentInChildren4 = base.GetComponentInChildren<RandomSoundController>();
				bool flag15 = componentInChildren4 != null;
				if (flag15)
				{
					componentInChildren4.Play();
				}
				break;
			}
			case (EntityEvent)25:
			{
				RandomSoundController componentInChildren5 = base.GetComponentInChildren<RandomSoundController>();
				bool flag16 = componentInChildren5 != null;
				if (flag16)
				{
					componentInChildren5.Play3();
				}
				break;
			}
			case (EntityEvent)26:
			{
				RandomSoundController componentInChildren6 = base.GetComponentInChildren<RandomSoundController>();
				bool flag17 = componentInChildren6 != null;
				if (flag17)
				{
					componentInChildren6.Stop();
				}
				break;
			}
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000E460 File Offset: 0x0000C660
		public static int GetMaxDataSize()
		{
			return 21;
		}

		// Token: 0x04000419 RID: 1049
		public ushort playerId;

		// Token: 0x0400041A RID: 1050
		public ushort targetId;

		// Token: 0x0400041B RID: 1051
		public ushort skinId;

		// Token: 0x0400041C RID: 1052
		public float thrustdelayTime;

		// Token: 0x0400041D RID: 1053
		public string BlockName;

		// Token: 0x0400041E RID: 1054
		public bool hasProjectileScript;

		// Token: 0x0400041F RID: 1055
		public AdProjectileScript projectileScript;

		// Token: 0x04000420 RID: 1056
		public AdNetworkProjectile baseAdNetworkBlock;

		// Token: 0x04000421 RID: 1057
		public ProjectileInfo projectileInfo;

		// Token: 0x04000422 RID: 1058
		public float bodyMass;

		// Token: 0x04000423 RID: 1059
		public float bodyDrag;

		// Token: 0x04000424 RID: 1060
		public float bodyAngularDrag;

		// Token: 0x04000425 RID: 1061
		public RigidbodyInterpolation bodyInterpolation;

		// Token: 0x04000426 RID: 1062
		public bool bodyKinematic;

		// Token: 0x04000427 RID: 1063
		public CollisionDetectionMode bodyCollisionMode;

		// Token: 0x04000428 RID: 1064
		private bool hasGyro;

		// Token: 0x04000429 RID: 1065
		private Vector3 basePos;

		// Token: 0x0400042A RID: 1066
		private float BASE_THRESHOLD = 1f;

		// Token: 0x0400042B RID: 1067
		private float totalDamageAdded;

		// Token: 0x0400042C RID: 1068
		private bool smokeActive;

		// Token: 0x0400042D RID: 1069
		private int skipFrames;

		// Token: 0x0400042E RID: 1070
		private int pollFrame;

		// Token: 0x0400042F RID: 1071
		private bool posActive;

		// Token: 0x04000430 RID: 1072
		private bool rotActive;

		// Token: 0x04000431 RID: 1073
		private Matrix4x4 transformMatrix;

		// Token: 0x04000432 RID: 1074
		private ServerMachine serverMachine;

		// Token: 0x04000433 RID: 1075
		private bool isClusterBase;

		// Token: 0x04000434 RID: 1076
		private int baseCurrentFrame;

		// Token: 0x04000435 RID: 1077
		private bool isBreakingOff;

		// Token: 0x04000436 RID: 1078
		private float baseThreshold = 1.5f;

		// Token: 0x04000437 RID: 1079
		private float baseSqrThreshold;

		// Token: 0x04000438 RID: 1080
		private float baseDist;

		// Token: 0x04000439 RID: 1081
		private bool partOfCluster;

		// Token: 0x0400043A RID: 1082
		private int lastSentPos;

		// Token: 0x0400043B RID: 1083
		private int lastSentRot;
	}
}
