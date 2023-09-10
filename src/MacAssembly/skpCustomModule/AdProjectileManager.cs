using System;
using System.Collections.Generic;
using System.Linq;
using mattmc3.dotmore.Collections.Generic;
using Modding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x0200005B RID: 91
	public class AdProjectileManager : MonoBehaviour
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600020A RID: 522 RVA: 0x00002CAC File Offset: 0x00000EAC
		public int BufferLength
		{
			get
			{
				return this.networkController.FullBufferLength;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00002CB9 File Offset: 0x00000EB9
		public bool SendShort
		{
			get
			{
				return this.networkController.SendShort;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00002CC6 File Offset: 0x00000EC6
		public int ProjectileCount
		{
			get
			{
				return this.spawnedProjectiles.Count;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00002CD3 File Offset: 0x00000ED3
		public int SpawnDataLength
		{
			get
			{
				return 2 + this.spawnDataPool.Count + this.despawnDataPool.Count + this.dataPoolLength;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600020E RID: 526 RVA: 0x00002366 File Offset: 0x00000566
		public bool ExExpand
		{
			get
			{
				return AdCustomModuleMod.mod4.ExExpandFloor;
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00002CF5 File Offset: 0x00000EF5
		public void PollObjects()
		{
			this.networkController.PollObjects(true, this.networkController.objList, this.networkController.objCount);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00020DCC File Offset: 0x0001EFCC
		protected void Awake()
		{
			this.networkController = base.gameObject.AddComponent<NetworkController>();
			this.networkController.Clear();
			AdProjectileManager.Instance = this;
			this.spawnDataPool = new List<AdProjectileManager.SpawnInfo>();
			this.despawnDataPool = new List<AdProjectileManager.DespawnInfo>();
			this.projectilePool = new List<AdProjectileManager.ProjectilePool>(this.projectilePrefabs.Length);
			int capacity = this.projectilePrefabs.Length * AdProjectileManager.MAX_POOL_SIZE;
			this.spawnedProjectiles = new List<AdNetworkProjectile>(capacity);
			this.networkController.SetCapacity(capacity);
			GameObject gameObject = new GameObject("Projectile Pool");
			this.poolParent = gameObject.transform;
			this.poolParent.parent = base.transform;
			int capacity2 = this.projectilePrefabs.Length * AdProjectileManager.MAX_POOL_SIZE;
			this.lastEventFrame = new Dictionary<ushort, uint>(capacity2);
			this.eventFrameKeys = new List<ushort>(capacity2);
			for (int i = 0; i < this.projectilePrefabs.Length; i++)
			{
				this.CreatePool(i);
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00020EC4 File Offset: 0x0001F0C4
		private void Update()
		{
			bool flag = StatMaster.isMainMenu || StatMaster.inMenu;
			if (!flag)
			{
				float deltaTime = TimeSlider.Instance.deltaTime;
				bool levelSimulating = StatMaster.levelSimulating;
				if (levelSimulating)
				{
					bool isClient = StatMaster.isClient;
					if (isClient)
					{
						bool updateflag = this.Updateflag;
						if (updateflag)
						{
							this.UnpackData(this.UpdateFrame, this.UpdateFrameNum, this.UpdateData01);
							this.Updateflag = false;
						}
						bool isLocalSim = StatMaster.isLocalSim;
						if (!isLocalSim)
						{
							this.UpdateProjectiles(deltaTime);
						}
					}
				}
				else
				{
					bool simulationStartState = this.SimulationStartState;
					if (simulationStartState)
					{
					}
				}
				bool projectileMatching = this.ProjectileMatching;
				if (projectileMatching)
				{
					this.ProjectileMatching = false;
					ModNetworking.SendToAll(AdCustomModuleMod.msgPoolinfoCallBack.CreateMessage(new object[] { this.Keynum, this.CallBlock, this.CallID }));
				}
				bool projectileSkinMatching = this.ProjectileSkinMatching;
				if (projectileSkinMatching)
				{
					this.ProjectileSkinMatching = false;
					KeyMachingTable keyMachingTable = AdCustomModuleMod.mod2.ProjectileSkinTable.List.FirstOrDefault((KeyMachingTable x) => x.Name == this.CallProjectileBlock);
					KeyMachingTable keyMachingTable2 = keyMachingTable.List.FirstOrDefault((KeyMachingTable c) => c.Name == this.CallProjectileSkin);
					bool flag2 = keyMachingTable2 != null;
					if (flag2)
					{
						int key = keyMachingTable.Key;
						int key2 = keyMachingTable2.Key;
						ModNetworking.SendToAll(AdCustomModuleMod.msgSkininfoCallBack.CreateMessage(new object[] { this.CallProjectilePlayerID, key, this.CallProjectileBlock, key2, this.CallProjectileSkin }));
					}
					else
					{
						int key3 = keyMachingTable.List.Count<KeyMachingTable>();
						KeyMachingTable keyMachingTable3 = new KeyMachingTable();
						keyMachingTable3.Key = key3;
						keyMachingTable3.Name = this.CallProjectileSkin;
						keyMachingTable.List.Add(keyMachingTable3);
						keyMachingTable2 = keyMachingTable.List.FirstOrDefault((KeyMachingTable c) => c.Name == this.CallProjectileSkin);
						int key4 = keyMachingTable.Key;
						int key5 = keyMachingTable2.Key;
						ModNetworking.SendToAll(AdCustomModuleMod.msgSkininfoCallBack.CreateMessage(new object[] { this.CallProjectilePlayerID, key4, this.CallProjectileBlock, key5, this.CallProjectileSkin }));
						Debug.Log("AdSkinTable is null");
					}
				}
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00021134 File Offset: 0x0001F334
		public void LateUpdate()
		{
			bool flag = StatMaster.isMainMenu || StatMaster.inMenu;
			if (!flag)
			{
				bool flag2 = !StatMaster.networkActive;
				if (!flag2)
				{
					this.isTracking = StatMaster.isHosting || StatMaster.isLocalSim;
					bool hasPlayers = Playerlist.Players.Count > 1;
					bool flag3 = false;
					float deltaTime = TimeSlider.Instance.deltaTime;
					this.lastUpdate += deltaTime;
					float sendRate = NetworkScene.ServerSettings.sendRate;
					bool flag4 = this.lastUpdate >= sendRate;
					if (flag4)
					{
						while (this.lastUpdate >= sendRate)
						{
							this.lastUpdate -= sendRate;
						}
						bool flag5 = this.pollLevel;
						if (flag5)
						{
							bool flag6 = !this.hasPolledLevel;
							if (flag6)
							{
								this.PollObject(this.Frame, hasPlayers);
							}
							else
							{
								this.hasPolledLevel = false;
							}
						}
						flag3 = true;
					}
					else
					{
						bool flag7 = this.pollLevel && !this.hasPolledLevel && this.lastUpdate / sendRate >= 0.5f;
						if (flag7)
						{
							this.PollObject(this.Frame, hasPlayers);
							this.hasPolledLevel = true;
						}
					}
					bool flag8 = this.pollLevel && flag3;
					if (flag8)
					{
						this.Frame += 1U;
					}
					bool levelSimulating = StatMaster.levelSimulating;
					if (levelSimulating)
					{
						bool flag9 = !this.SimulationStartState;
						if (flag9)
						{
							this.SimulationStartState = true;
							this.Clear();
							this.networkController.Clear();
							this.networkController.InitSim(this.isTracking);
							this.networkController.ToggleEssentialBuffer(false);
						}
						bool menuReset = this.MenuReset;
						if (menuReset)
						{
							this.MenuReset = false;
						}
						bool isClient = StatMaster.isClient;
						if (isClient)
						{
							bool isLocalSim = StatMaster.isLocalSim;
							if (isLocalSim)
							{
							}
						}
						else
						{
							bool isHosting = StatMaster.isHosting;
							if (isHosting)
							{
								this.pollLevel = true;
							}
						}
					}
					else
					{
						bool simulationStartState = this.SimulationStartState;
						if (simulationStartState)
						{
							this.SimulationStartState = false;
							this.pollLevel = false;
							this.Frame = 0U;
							this.subFrame = 0;
							this.networkController.Clear();
						}
					}
				}
			}
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00021370 File Offset: 0x0001F570
		public void PollObject(uint currentFrame, bool hasPlayers)
		{
			this.PollObjects();
			bool flag = hasPlayers && this.PackTransformData(out this.HostupdateData01);
			if (flag)
			{
				byte[] bytes = BitConverter.GetBytes(currentFrame);
				ModNetworking.SendToAll(AdCustomModuleMod.msgShooting.CreateMessage(new object[] { this.HostupdateData01, bytes }));
			}
			else
			{
				this.ClearSpawnData();
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000213D4 File Offset: 0x0001F5D4
		public void AddAdditionalProjectile(int id, GameObject prefab)
		{
			bool flag = id != this.projectilePrefabs.Length;
			if (flag)
			{
				Debug.Log("AdProjectileAppend ID:" + id.ToString());
			}
			this.projectilePrefabs = EnumerableExtensions.Append<GameObject>(this.projectilePrefabs, prefab).ToArray<GameObject>();
			int capacity = this.projectilePrefabs.Length * AdProjectileManager.MAX_POOL_SIZE;
			NetworkEntity[] objList = this.networkController.objList;
			this.networkController.SetCapacity(capacity);
			for (int i = 0; i < objList.Length; i++)
			{
				this.networkController.objList[i] = objList[i];
			}
			this.spawnedProjectiles.Capacity = capacity;
			this.eventFrameKeys.Capacity = capacity;
			this.CreatePool(id);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00021494 File Offset: 0x0001F694
		public void ClearAdditionalProjectiles()
		{
			int num = this.projectilePrefabs.Length;
			for (int i = num; i < this.projectilePool.Count; i++)
			{
				AdProjectileManager.ProjectilePool projectilePool = this.projectilePool[i];
				while (projectilePool.Pool.Count > 0)
				{
					AdNetworkProjectile adNetworkProjectile = projectilePool.Pool.Pop();
					Object.Destroy(adNetworkProjectile.gameObject);
				}
				for (int j = 0; j < AdProjectileManager.MAX_POOL_SIZE; j++)
				{
					ushort num2 = (ushort)(i * AdProjectileManager.MAX_POOL_SIZE + j);
					this.lastEventFrame.Remove(num2);
					this.eventFrameKeys.Remove(num2);
				}
			}
			this.projectilePool = this.projectilePool.Take(num).ToList<AdProjectileManager.ProjectilePool>();
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00021568 File Offset: 0x0001F768
		private void CreatePool(int i)
		{
			AdProjectileManager.ProjectilePool projectilePool = new AdProjectileManager.ProjectilePool();
			for (int j = 0; j < AdProjectileManager.DEFAULT_POOL_SIZE; j++)
			{
				AdNetworkProjectile adNetworkProjectile = this.Create(i);
				adNetworkProjectile.transform.parent = this.poolParent;
				projectilePool.Pool.Push(adNetworkProjectile);
			}
			for (int k = 0; k < AdProjectileManager.MAX_POOL_SIZE; k++)
			{
				ushort num = (ushort)(i * AdProjectileManager.MAX_POOL_SIZE + k);
				this.lastEventFrame.Add(num, 0U);
				this.eventFrameKeys.Add(num);
			}
			this.projectilePool.Add(projectilePool);
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0002160C File Offset: 0x0001F80C
		public AdProjectileManager.ProjectilePool GetPool(int id)
		{
			return this.projectilePool[id];
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00002D1B File Offset: 0x00000F1B
		public void InitSim(bool track)
		{
			this.networkController.InitSim(track);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00002D2B File Offset: 0x00000F2B
		public void WriteBufferData(byte[] buffer, int offset)
		{
			this.networkController.WriteBufferData(true, buffer, offset);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0002162C File Offset: 0x0001F82C
		public int GetSimFrame()
		{
			return 2 + this.spawnedProjectiles.Count * 18;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00002D3D File Offset: 0x00000F3D
		public void ResetFrame()
		{
			this.networkController.ResetFrame();
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00021650 File Offset: 0x0001F850
		public int ReadBufferData(uint frame, byte[] transformData, int offset)
		{
			return this.networkController.ReadBufferData(frame, transformData, offset);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00002D4C File Offset: 0x00000F4C
		public void NewFrame(uint frame)
		{
			this.networkController.NewFrame(frame);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00021670 File Offset: 0x0001F870
		public void UpdateProjectiles(float delta)
		{
			for (int i = 0; i < this.spawnedProjectiles.Count; i++)
			{
				this.spawnedProjectiles[i].UpdateEntity(delta);
			}
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00002D5C File Offset: 0x00000F5C
		public void ClearSpawnData()
		{
			this.dataPoolLength = 0;
			this.spawnDataPool.Clear();
			this.despawnDataPool.Clear();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x000216B0 File Offset: 0x0001F8B0
		public void WriteSpawnData(byte[] data, int offset)
		{
			data[offset] = (byte)this.despawnDataPool.Count;
			offset++;
			for (int i = 0; i < this.despawnDataPool.Count; i++)
			{
				AdProjectileManager.DespawnInfo despawnInfo = this.despawnDataPool[i];
				int num = despawnInfo.Size();
				data[offset] = (byte)num;
				offset++;
				NetworkCompression.WriteUInt16(despawnInfo.networkId, data, offset);
				offset += 2;
				Buffer.BlockCopy(despawnInfo.data, 0, data, offset, despawnInfo.data.Length);
				offset += despawnInfo.data.Length;
			}
			data[offset] = (byte)this.spawnDataPool.Count;
			offset++;
			for (int j = 0; j < this.spawnDataPool.Count; j++)
			{
				AdProjectileManager.SpawnInfo spawnInfo = this.spawnDataPool[j];
				int num2 = spawnInfo.Size();
				data[offset] = (byte)num2;
				offset++;
				data[offset] = spawnInfo.projectileType;
				offset++;
				data[offset] = spawnInfo.playerId;
				offset++;
				data[offset] = spawnInfo.targetId;
				offset++;
				NetworkCompression.WriteUInt16(spawnInfo.skinId, data, offset);
				offset += 2;
				AdNetworkCompression.CompressFloat(spawnInfo.thrustdelayTime, data, offset);
				offset += 2;
				NetworkCompression.WriteUInt16(spawnInfo.networkId, data, offset);
				offset += 2;
				Buffer.BlockCopy(spawnInfo.data, 0, data, offset, spawnInfo.data.Length);
				offset += spawnInfo.data.Length;
			}
			this.ClearSpawnData();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00021838 File Offset: 0x0001FA38
		private bool IsNewData(ushort networkId, uint frame)
		{
			bool flag = frame < this.lastEventFrame[networkId];
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.lastEventFrame[networkId] = frame;
				result = true;
			}
			return result;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00021874 File Offset: 0x0001FA74
		public int ReadSpawnData(uint frame, byte[] data, int offset)
		{
			this.despawnId = 250;
			int num = offset;
			int num2 = (int)data[offset];
			offset++;
			for (int i = 0; i < num2; i++)
			{
				int num3 = (int)data[offset];
				offset++;
				ushort networkId = NetworkCompression.ReadUInt16(data, offset);
				offset += 2;
				byte[] array = new byte[num3 - 2];
				Buffer.BlockCopy(data, offset, array, 0, num3 - 2);
				offset += array.Length;
				AdNetworkProjectile projectile = null;
				bool flag = this.IsNewData(networkId, frame) && this.GetProjectile((uint)networkId, out projectile);
				if (flag)
				{
					this.despawnId = networkId;
					this.Despawn(projectile, array);
				}
			}
			int num4 = (int)data[offset];
			offset++;
			for (int j = 0; j < num4; j++)
			{
				int num5 = (int)data[offset];
				offset++;
				int projectileType = 0;
				bool flag2 = false;
				bool flag3 = AdCustomModuleMod.mod2.ProjectilePoolMaching.ContainsKey((int)data[offset]);
				if (flag3)
				{
					projectileType = AdCustomModuleMod.mod2.ProjectilePoolMaching[(int)data[offset]];
					flag2 = true;
				}
				offset++;
				ushort playerId = (ushort)data[offset];
				offset++;
				ushort targetId = (ushort)data[offset];
				offset++;
				ushort skinId = NetworkCompression.ReadUInt16(data, offset);
				offset += 2;
				float thrustdelayTime = 0f;
				AdNetworkCompression.DecompressFloat(data, offset, out thrustdelayTime);
				offset += 2;
				ushort networkId2 = NetworkCompression.ReadUInt16(data, offset);
				byte[] array2 = new byte[num5 - 7];
				Buffer.BlockCopy(data, offset, array2, 0, array2.Length);
				offset += array2.Length;
				bool flag4 = this.IsNewData(networkId2, frame) && flag2;
				if (flag4)
				{
					this.Spawn(projectileType, frame, playerId, targetId, skinId, thrustdelayTime, array2);
				}
			}
			return offset - num;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00021A1C File Offset: 0x0001FC1C
		public Transform Spawn(int projectileType, uint frame, ushort playerId, ushort targetId, ushort skinId, float thrustdelayTime, byte[] spawnInfo)
		{
			int num = 0;
			AdProjectileManager.ProjectilePool projectilePool = this.projectilePool[projectileType];
			bool isHosting = StatMaster.isHosting;
			ushort num2;
			AdNetworkProjectile adNetworkProjectile;
			if (isHosting)
			{
				while (projectilePool.ActiveCount >= AdProjectileManager.MAX_POOL_SIZE)
				{
					AdNetworkProjectile projectile = projectilePool.Active[0];
					this.Despawn(projectile);
				}
				bool flag = this.spawnedProjectiles.Count > 0;
				if (flag)
				{
					while (num < this.spawnedProjectiles.Count && (ulong)this.spawnedProjectiles[num].id == (ulong)((long)num))
					{
						num++;
					}
				}
				num2 = (ushort)num;
				ushort num3 = (ushort)num;
			}
			else
			{
				num2 = NetworkCompression.ReadUInt16(spawnInfo, 0);
				bool projectile2 = this.GetProjectile((uint)num2, out adNetworkProjectile);
				if (projectile2)
				{
					this.Despawn(adNetworkProjectile);
				}
				bool flag2 = this.spawnedProjectiles.Count > 0;
				if (flag2)
				{
					while (num < this.spawnedProjectiles.Count && this.spawnedProjectiles[num].id < (uint)num2)
					{
						num++;
					}
				}
				byte[] array = new byte[spawnInfo.Length - 2];
				Buffer.BlockCopy(spawnInfo, 2, array, 0, array.Length);
				spawnInfo = array;
			}
			adNetworkProjectile = ((projectilePool.Pool.Count != 0) ? projectilePool.Pool.Pop() : this.Create(projectileType));
			bool flag3 = adNetworkProjectile != null;
			if (flag3)
			{
				projectilePool.ActiveCount++;
				projectilePool.Active.Add(adNetworkProjectile);
				Transform transform = adNetworkProjectile.transform;
				this.spawnedProjectiles.Insert(num, adNetworkProjectile);
				transform.SetParent(base.transform, false);
				adNetworkProjectile.Spawn(frame, playerId, targetId, skinId, thrustdelayTime, spawnInfo);
				adNetworkProjectile.gameObject.SetActive(true);
				adNetworkProjectile.Init((uint)num2, this.networkController, transform, StatMaster.isHosting);
				this.networkController.Add(adNetworkProjectile);
				ServerHealth.countDirty = true;
			}
			else
			{
				Debug.Log("Don't exist AdProjecctile ID : " + projectileType.ToString());
				bool flag4 = AdCustomModuleMod.mod2.AdShootingModuleProjectiles.ContainsKey(projectileType);
				if (flag4)
				{
					Debug.Log("Don't exist AdProjecctile Name : " + AdCustomModuleMod.mod2.AdShootingModuleProjectiles[projectileType]);
				}
				else
				{
					Debug.Log("Don't exist AdProjecctile Name : null");
				}
			}
			bool isHosting2 = StatMaster.isHosting;
			if (isHosting2)
			{
				this.ClearSpawn(num2);
				AdProjectileManager.SpawnInfo spawnInfo2 = new AdProjectileManager.SpawnInfo();
				spawnInfo2.projectileType = (byte)projectileType;
				spawnInfo2.playerId = (byte)playerId;
				spawnInfo2.targetId = (byte)targetId;
				spawnInfo2.skinId = skinId;
				spawnInfo2.networkId = num2;
				spawnInfo2.thrustdelayTime = thrustdelayTime;
				spawnInfo2.data = spawnInfo;
				this.spawnDataPool.Add(spawnInfo2);
				this.dataPoolLength += spawnInfo2.Size();
			}
			return adNetworkProjectile.transform;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00021D10 File Offset: 0x0001FF10
		public void Clear()
		{
			this.isClearing = true;
			while (this.spawnedProjectiles.Count > 0)
			{
				this.Despawn(this.spawnedProjectiles[0]);
			}
			for (int i = 0; i < this.projectilePrefabs.Length; i++)
			{
				this.projectilePool[i].ActiveCount = 0;
			}
			for (int j = 0; j < this.eventFrameKeys.Count; j++)
			{
				this.lastEventFrame[this.eventFrameKeys[j]] = 0U;
			}
			this.isClearing = false;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00021DB8 File Offset: 0x0001FFB8
		public void DespawnParentedProjectiles(Transform objTransform)
		{
			AdProjectileManager.ProjectilePool projectilePool = this.projectilePool[1];
			List<AdNetworkProjectile> list = new List<AdNetworkProjectile>(projectilePool.Active);
			list.AddRange(this.projectilePool[2].Active);
			list.AddRange(this.projectilePool[3].Active);
			for (int i = Enum.GetValues(typeof(NetworkProjectileType)).Length; i < this.projectilePrefabs.Length; i++)
			{
				list.AddRange(this.projectilePool[i].Active);
			}
			foreach (AdNetworkProjectile adNetworkProjectile in list)
			{
				bool flag = !(adNetworkProjectile == null) && adNetworkProjectile.transform.IsChildOf(objTransform);
				if (flag)
				{
					this.Despawn(adNetworkProjectile);
				}
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00021EC0 File Offset: 0x000200C0
		public void Despawn(Transform projectileTransform)
		{
			AdNetworkProjectile component = projectileTransform.GetComponent<AdNetworkProjectile>();
			bool flag = component != null;
			if (flag)
			{
				this.Despawn(component, null);
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00002D7E File Offset: 0x00000F7E
		public void Despawn(AdNetworkProjectile projectile)
		{
			this.Despawn(projectile, null);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00021EEC File Offset: 0x000200EC
		public void Despawn(AdNetworkProjectile projectile, byte[] despawnInfo)
		{
			bool flag = projectile == null;
			if (!flag)
			{
				AdProjectileManager.ProjectilePool projectilePool = this.projectilePool[projectile.projectileInfo.AdProjectileType];
				bool flag2 = projectilePool.Active.Contains(projectile);
				if (flag2)
				{
					bool flag3 = !projectile.projectileInfo.noRigidbody;
					if (flag3)
					{
						projectile.projectileInfo.Rigidbody.interpolation = 0;
					}
					Transform transform = projectile.transform;
					transform.SetParent(this.poolParent, false);
					projectile.Despawn(despawnInfo);
					projectilePool.ActiveCount--;
					projectilePool.Active.Remove(projectile);
					this.spawnedProjectiles.Remove(projectile);
					this.networkController.Remove(projectile);
					ServerHealth.countDirty = true;
					bool flag4 = StatMaster.isHosting && !this.isClearing;
					if (flag4)
					{
						this.ClearDespawn((ushort)projectile.id);
						AdProjectileManager.DespawnInfo despawnInfo2 = new AdProjectileManager.DespawnInfo();
						despawnInfo2.networkId = (ushort)projectile.id;
						despawnInfo2.data = ((despawnInfo == null) ? new byte[0] : despawnInfo);
						this.despawnDataPool.Add(despawnInfo2);
						this.dataPoolLength += despawnInfo2.Size();
					}
					else
					{
						bool flag5 = !this.isClearing;
						if (flag5)
						{
							bool flag6 = (projectile.projectileScript.canExplode && StatMaster.GodTools.ExplodingCannonballs) || projectile.projectileScript.alwaysExplodes;
							if (flag6)
							{
								int num = 0;
								Vector3 zero = Vector3.zero;
								Quaternion identity = Quaternion.identity;
								try
								{
									bool exExpand = this.ExExpand;
									if (exExpand)
									{
										AdNetworkCompression.DecompressPosition(despawnInfo, num, out zero);
										num += 12;
									}
									else
									{
										NetworkCompression.DecompressPosition(despawnInfo, num, out zero);
										num += 6;
									}
									NetworkCompression.DecompressRotation(despawnInfo, num, out identity);
									this.Explode(projectile, zero, identity);
								}
								catch
								{
								}
							}
						}
					}
					projectile.gameObject.SetActive(false);
					bool flag7 = !projectile.HasChangedState;
					if (flag7)
					{
						projectile.ReturnToPool();
						projectilePool.Pool.Push(projectile);
					}
					else
					{
						Object.Destroy(projectile.gameObject);
					}
				}
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00022128 File Offset: 0x00020328
		public void Explode(AdNetworkProjectile projectile, Vector3 explosionPos, Quaternion explosionRot)
		{
			int index = AdCustomModuleMod.mod2.ExplosionEffectCountContainer[projectile.BlockName];
			Transform transform = AdCustomModuleMod.mod2.ExplosionEffectContainer[projectile.BlockName][index];
			bool flag = transform == null;
			if (flag)
			{
				transform = Object.Instantiate<GameObject>(AdCustomModuleMod.mod2.ExplosionContainer[projectile.BlockName]).transform;
				transform.name = "ExplosionEffect";
				transform.SetParent(AdCustomModuleMod.mod2.PMEffectPool.transform);
				AdCustomModuleMod.mod2.ExplosionEffectContainer[projectile.BlockName][index] = transform;
			}
			transform.gameObject.SetActive(true);
			transform.SetParent(this.GetPhysGoal());
			transform.position = explosionPos;
			bool useBooster = projectile.projectileScript.useBooster;
			if (useBooster)
			{
				transform.rotation = Quaternion.identity;
			}
			else
			{
				transform.rotation = explosionRot;
			}
			bool flag2 = AdCustomModuleMod.mod2.ExplosionEffectCountContainer[projectile.BlockName] < AdCustomModuleMod.mod2.MAX_Effect_POOL_SIZE;
			if (flag2)
			{
				Dictionary<string, int> explosionEffectCountContainer = AdCustomModuleMod.mod2.ExplosionEffectCountContainer;
				string blockName = projectile.BlockName;
				int num = explosionEffectCountContainer[blockName];
				explosionEffectCountContainer[blockName] = num + 1;
			}
			else
			{
				AdCustomModuleMod.mod2.ExplosionEffectCountContainer[projectile.BlockName] = 0;
			}
			bool useTrailEffect = projectile.projectileScript.useTrailEffect;
			bool flag3 = useTrailEffect;
			if (flag3)
			{
				Transform transform2 = projectile.transform.FindChild("Gyro").transform.FindChild("TrailEffect");
				TrailResset component = transform2.GetComponent<TrailResset>();
				component.NetworkTrailstop = true;
				Vector3 position = component.Normalizedposition + explosionPos - component.preposition;
				transform2.SetParent(this.GetPhysGoal());
				transform2.position = position;
				transform2.rotation = explosionRot;
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00015BE4 File Offset: 0x00013DE4
		private Transform GetPhysGoal()
		{
			return ReferenceMaster.physicsGoalInstance;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00022310 File Offset: 0x00020510
		private void ClearSpawn(ushort networkId)
		{
			List<AdProjectileManager.SpawnInfo> list = new List<AdProjectileManager.SpawnInfo>(this.spawnDataPool);
			for (int i = 0; i < list.Count; i++)
			{
				AdProjectileManager.SpawnInfo spawnInfo = list[i];
				bool flag = spawnInfo.networkId == networkId;
				if (flag)
				{
					this.dataPoolLength -= spawnInfo.Size();
					this.spawnDataPool.Remove(spawnInfo);
				}
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0002237C File Offset: 0x0002057C
		private void ClearDespawn(ushort networkId)
		{
			List<AdProjectileManager.DespawnInfo> list = new List<AdProjectileManager.DespawnInfo>(this.despawnDataPool);
			for (int i = 0; i < list.Count; i++)
			{
				AdProjectileManager.DespawnInfo despawnInfo = list[i];
				bool flag = despawnInfo.networkId == networkId;
				if (flag)
				{
					this.dataPoolLength -= despawnInfo.Size();
					this.despawnDataPool.Remove(despawnInfo);
				}
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x000223E8 File Offset: 0x000205E8
		private AdNetworkProjectile Create(int projectileType)
		{
			AdNetworkProjectile adNetworkProjectile = null;
			bool flag = this.projectilePrefabs[projectileType] != null;
			if (flag)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.projectilePrefabs[projectileType]);
				adNetworkProjectile = gameObject.GetComponent<AdNetworkProjectile>();
				adNetworkProjectile.UpdateTransforms();
			}
			return adNetworkProjectile;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00022430 File Offset: 0x00020630
		private bool GetProjectile(uint networkId, out AdNetworkProjectile projectile)
		{
			for (int i = 0; i < this.spawnedProjectiles.Count; i++)
			{
				AdNetworkProjectile adNetworkProjectile = this.spawnedProjectiles[i];
				bool flag = adNetworkProjectile.id == networkId;
				if (flag)
				{
					projectile = adNetworkProjectile;
					return true;
				}
			}
			projectile = null;
			return false;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00022488 File Offset: 0x00020688
		public bool PackTransformData(out byte[] updateData)
		{
			bool flag = StatMaster.levelSimulating && !StatMaster.isLocalSim;
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				updateData = null;
				result = false;
			}
			else
			{
				int spawnDataLength = this.SpawnDataLength;
				updateData = new byte[1 + (flag ? (spawnDataLength + this.BufferLength) : 0)];
				int num = 0;
				updateData[num] = (byte)(flag ? 1 : 0);
				num++;
				bool flag3 = flag;
				if (flag3)
				{
					this.WriteSpawnData(updateData, num);
					num += spawnDataLength;
					this.WriteBufferData(updateData, num);
					num += this.BufferLength;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0002251C File Offset: 0x0002071C
		public void UnpackData(uint frame, uint frameNum, byte[] data01)
		{
			uint num = frameNum;
			bool flag = num > 1U;
			if (flag)
			{
				num = 1U;
			}
			int num2 = 0;
			bool flag2 = data01[num2] == 1;
			num2++;
			int num3 = this.ReadSpawnData(frame, data01, num2);
			num2 += num3;
			int num4 = this.ReadBufferData(frame, data01, num2);
			num2 += num4;
			bool flag3 = num == 0U;
			if (flag3)
			{
				this.NewFrame(frame);
			}
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0002257C File Offset: 0x0002077C
		public bool IsFPSFrame(uint frame)
		{
			return frame % 5U == 0U;
		}

		// Token: 0x0400043E RID: 1086
		public GameObject[] projectilePrefabs = new GameObject[0];

		// Token: 0x0400043F RID: 1087
		public static AdProjectileManager Instance;

		// Token: 0x04000440 RID: 1088
		private static int DEFAULT_POOL_SIZE = 50;

		// Token: 0x04000441 RID: 1089
		private static int MAX_POOL_SIZE = 200;

		// Token: 0x04000442 RID: 1090
		private NetworkController networkController;

		// Token: 0x04000443 RID: 1091
		private List<AdProjectileManager.SpawnInfo> spawnDataPool;

		// Token: 0x04000444 RID: 1092
		private List<AdProjectileManager.DespawnInfo> despawnDataPool;

		// Token: 0x04000445 RID: 1093
		private List<int> despawnID;

		// Token: 0x04000446 RID: 1094
		private Dictionary<ushort, uint> lastEventFrame;

		// Token: 0x04000447 RID: 1095
		private int dataPoolLength;

		// Token: 0x04000448 RID: 1096
		private List<AdProjectileManager.ProjectilePool> projectilePool;

		// Token: 0x04000449 RID: 1097
		private List<AdNetworkProjectile> spawnedProjectiles;

		// Token: 0x0400044A RID: 1098
		private Transform poolParent;

		// Token: 0x0400044B RID: 1099
		private bool isClearing;

		// Token: 0x0400044C RID: 1100
		private List<ushort> eventFrameKeys;

		// Token: 0x0400044D RID: 1101
		public uint Frame = 0U;

		// Token: 0x0400044E RID: 1102
		public int subFrame = 0;

		// Token: 0x0400044F RID: 1103
		public float lastUpdate = 0f;

		// Token: 0x04000450 RID: 1104
		public bool pollLevel = false;

		// Token: 0x04000451 RID: 1105
		public bool hasPolledLevel = true;

		// Token: 0x04000452 RID: 1106
		public bool SimulationStartState = true;

		// Token: 0x04000453 RID: 1107
		public bool MenuReset = false;

		// Token: 0x04000454 RID: 1108
		public byte[] HostupdateData01 = new byte[0];

		// Token: 0x04000455 RID: 1109
		public byte[] HostupdateData02 = new byte[0];

		// Token: 0x04000456 RID: 1110
		public byte[] HostupdateData03 = new byte[0];

		// Token: 0x04000457 RID: 1111
		public byte[] HostupdateData04 = new byte[0];

		// Token: 0x04000458 RID: 1112
		public byte[] HostupdateData05 = new byte[0];

		// Token: 0x04000459 RID: 1113
		public bool isTracking;

		// Token: 0x0400045A RID: 1114
		public ushort IDcounter = 0;

		// Token: 0x0400045B RID: 1115
		private ushort despawncount = 0;

		// Token: 0x0400045C RID: 1116
		public bool Updateflag = false;

		// Token: 0x0400045D RID: 1117
		public byte[] UpdateData01 = new byte[0];

		// Token: 0x0400045E RID: 1118
		public byte[] UpdateData02 = new byte[0];

		// Token: 0x0400045F RID: 1119
		public byte[] UpdateData03 = new byte[0];

		// Token: 0x04000460 RID: 1120
		public byte[] UpdateData04 = new byte[0];

		// Token: 0x04000461 RID: 1121
		public byte[] UpdateData05 = new byte[0];

		// Token: 0x04000462 RID: 1122
		public uint UpdateFrameNum;

		// Token: 0x04000463 RID: 1123
		public uint UpdateFrame;

		// Token: 0x04000464 RID: 1124
		public bool ProjectileMatching = false;

		// Token: 0x04000465 RID: 1125
		public int Keynum;

		// Token: 0x04000466 RID: 1126
		public int CallID;

		// Token: 0x04000467 RID: 1127
		public string CallBlock;

		// Token: 0x04000468 RID: 1128
		public int ProjectilePoolCounter = 0;

		// Token: 0x04000469 RID: 1129
		public bool ProjectileSkinMatching = false;

		// Token: 0x0400046A RID: 1130
		public int CallProjectilePlayerID;

		// Token: 0x0400046B RID: 1131
		public string CallProjectileBlock;

		// Token: 0x0400046C RID: 1132
		public string CallProjectileSkin;

		// Token: 0x0400046D RID: 1133
		public ushort despawnId;

		// Token: 0x0400046E RID: 1134
		private float float_to_ushort = 100f;

		// Token: 0x0400046F RID: 1135
		private float ushort_to_float = 0.01f;

		// Token: 0x04000470 RID: 1136
		protected Dictionary<uint, NetworkEntity> networkObjects = new Dictionary<uint, NetworkEntity>();

		// Token: 0x0200005C RID: 92
		public class ProjectilePool
		{
			// Token: 0x06000237 RID: 567 RVA: 0x00002DC3 File Offset: 0x00000FC3
			public ProjectilePool()
			{
				this.Pool = new Stack<AdNetworkProjectile>();
				this.Active = new List<AdNetworkProjectile>();
				this.ActiveCount = 0;
			}

			// Token: 0x04000471 RID: 1137
			public Stack<AdNetworkProjectile> Pool;

			// Token: 0x04000472 RID: 1138
			public List<AdNetworkProjectile> Active;

			// Token: 0x04000473 RID: 1139
			public int ActiveCount;
		}

		// Token: 0x0200005D RID: 93
		private class SpawnInfo
		{
			// Token: 0x06000238 RID: 568 RVA: 0x000226AC File Offset: 0x000208AC
			public int Size()
			{
				return 9 + this.data.Length;
			}

			// Token: 0x04000474 RID: 1140
			public ushort networkId;

			// Token: 0x04000475 RID: 1141
			public float thrustdelayTime;

			// Token: 0x04000476 RID: 1142
			public byte projectileType;

			// Token: 0x04000477 RID: 1143
			public byte playerId;

			// Token: 0x04000478 RID: 1144
			public byte targetId;

			// Token: 0x04000479 RID: 1145
			public ushort skinId;

			// Token: 0x0400047A RID: 1146
			public byte[] data;
		}

		// Token: 0x0200005E RID: 94
		private class DespawnInfo
		{
			// Token: 0x0600023A RID: 570 RVA: 0x000226CC File Offset: 0x000208CC
			public int Size()
			{
				return 2 + this.data.Length;
			}

			// Token: 0x0400047B RID: 1147
			public ushort networkId;

			// Token: 0x0400047C RID: 1148
			public byte[] data;
		}
	}
}
