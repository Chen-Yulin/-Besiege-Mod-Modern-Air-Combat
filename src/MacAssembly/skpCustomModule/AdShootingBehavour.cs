using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modding;
using Modding.Common;
using Modding.Modules;
using Modding.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace skpCustomModule
{
	// Token: 0x02000033 RID: 51
	public class AdShootingBehavour : BlockModuleBehaviour<AdShootingProp>
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00002715 File Offset: 0x00000915
		private int FlipInvert
		{
			get
			{
				return (!base.Flipped) ? 1 : (-1);
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00002366 File Offset: 0x00000566
		public bool ExExpand
		{
			get
			{
				return AdCustomModuleMod.mod4.ExExpandFloor;
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00012880 File Offset: 0x00010A80
		public override void SafeAwake()
		{
			base.SafeAwake();
			this.BlockName = base.transform.name.Replace("(Clone)", "");
			try
			{
				this.PowerSlider = base.GetSlider(base.Module.PowerSlider);
				this.RateOfFire = base.GetSlider(base.Module.RateOfFireSlider);
				this.HoldToShootToggle = base.GetToggle(base.Module.HoldToShootToggle);
				this.FireKey = base.GetKey(base.Module.FireKey);
				bool useTimefuse = base.Module.useTimefuse;
				if (useTimefuse)
				{
					this.Timefuse = base.GetSlider(base.Module.TimefuseSlider);
				}
				bool useDelayTimer = base.Module.useDelayTimer;
				if (useDelayTimer)
				{
					this.DelayTime = base.GetSlider(base.Module.DelayTimerSlider);
				}
				bool useThrustDelayTimer = base.Module.useThrustDelayTimer;
				if (useThrustDelayTimer)
				{
					this.ThrustDelayTime = base.GetSlider(base.Module.ThrustDelayTimerSlider);
				}
			}
			catch
			{
				Debug.Log("Error BlockName : " + this.BlockName);
				Object.Destroy(this);
				return;
			}
			this.AmmoLeft = base.Module.DefaultAmmo;
			bool isChaff = base.Module.isChaff;
			if (isChaff)
			{
				this.GetChaffPrefab();
			}
			else
			{
				this.GetExplosionPrefab();
			}
			this.GetShotFlashPrefab();
			this.GetTrailPrefab();
			this.GetBulletEffectPrefab();
			this.GetAdProjectilePrefab();
			this.useProjManager = StatMaster.isHosting && !base.Machine.InternalObject.LocalSim && StatMaster.isMP;
			bool flag = !StatMaster.isMP;
			if (flag)
			{
				this.OwnerId = 0;
			}
			else
			{
				this.OwnerId = (int)base.Machine.InternalObject.PlayerID;
				try
				{
					AdCustomModuleMod.LocalPlayerNetworkID = (int)Player.GetLocalPlayer().NetworkId;
				}
				catch
				{
				}
			}
			this.PostRegisterPrefabs(this.Prefab);
			this.projectileStartPosition = base.Module.ProjectileStart;
			this.PurgeVector = base.Module.PurgeVector;
			this.ShotFlashPosition = base.Module.ShotFlashPosition;
			this.Flipflag = false;
			bool flag2 = !AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey("DEFAULT");
			if (flag2)
			{
				AdSkinLoader.AdSkinDataPack value = new AdSkinLoader.AdSkinDataPack();
				AdCustomModuleMod.mod2.ProjectileSkinPackContainer.Add("DEFAULT", value);
			}
			bool flag3 = !AdCustomModuleMod.mod2.ProjectileSkinPackContainer["DEFAULT"].ProjectileSkinContainer.ContainsKey(this.BlockName);
			if (flag3)
			{
				AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData = new AdSkinLoader.AdSkinDataPack.AdSkinData();
				ModMesh modMesh = (ModMesh)base.GetResource(base.Module.Shootingstateinfo.Mesh);
				ModTexture modTexture = (ModTexture)base.GetResource(base.Module.Shootingstateinfo.Texture);
				adSkinData.AdDefualtSkinSet(modMesh, modTexture);
				AdCustomModuleMod.mod2.ProjectileSkinPackContainer["DEFAULT"].ProjectileSkinContainer.Add(this.BlockName, adSkinData);
			}
			bool flag4 = !StatMaster.levelSimulating;
			if (flag4)
			{
				bool showPlaceholderProjectile = base.Module.ShowPlaceholderProjectile;
				if (showPlaceholderProjectile)
				{
					this.projectileStart = Object.Instantiate<GameObject>(AdCustomModuleMod.mod3.ShootingDirectionVisual).transform;
					this.projectileStart.parent = base.transform;
					this.projectileStartPosition.SetOnTransform(this.projectileStart);
					this.projectileStart.FindChild("Vis").gameObject.SetActive(false);
					this.projectilePlaceholder = this.GetAdProjectilePrefabPlaceholder(this.Prefab);
					this.projectilePlaceholder.transform.parent = this.projectileStart;
					this.projectilePlaceholder.transform.localPosition = Vector3.zero;
					this.projectilePlaceholder.transform.localRotation = Quaternion.identity;
					this.projectileStart.gameObject.SetActive(true);
					this.projectilePlaceholder.SetActive(true);
				}
				else
				{
					this.projectileStart = Object.Instantiate<GameObject>(AdCustomModuleMod.mod3.ShootingDirectionVisual).transform;
					this.projectileStart.parent = base.transform;
					this.projectileStartPosition.SetOnTransform(this.projectileStart);
					this.projectileStart.FindChild("Vis").gameObject.SetActive(false);
					this.projectilePlaceholder = this.GetAdProjectilePrefabPlaceholder(this.Prefab);
					this.projectilePlaceholder.transform.parent = this.projectileStart;
					this.projectilePlaceholder.transform.localPosition = Vector3.zero;
					this.projectilePlaceholder.transform.localRotation = Quaternion.identity;
				}
			}
			else
			{
				this.ShotFlashList = new List<Transform>();
				for (int i = 0; i < this.ShotFlashnum + 1; i++)
				{
					Transform transform = Object.Instantiate<GameObject>(AdCustomModuleMod.mod2.ShotFlashContainer[this.BlockName]).transform;
					transform.name = "ShotFlash";
					transform.SetParent(base.transform);
					this.ShotFlashList.Add(transform);
					transform.gameObject.SetActive(false);
				}
				bool flag5 = this.projectileStart == null;
				if (flag5)
				{
					this.projectileStart = Object.Instantiate<GameObject>(AdCustomModuleMod.mod3.ShootingDirectionVisual).transform;
					this.projectileStart.parent = base.transform;
					this.projectileStartPosition.SetOnTransform(this.projectileStart);
				}
				bool showPlaceholderProjectile2 = base.Module.ShowPlaceholderProjectile;
				if (showPlaceholderProjectile2)
				{
					bool flag6 = this.projectilePlaceholder == null;
					if (flag6)
					{
						this.projectileStart.FindChild("Vis").gameObject.SetActive(false);
						this.projectilePlaceholder = this.GetAdProjectilePrefabPlaceholder(this.Prefab);
						this.projectilePlaceholder.transform.parent = this.projectileStart;
						this.projectilePlaceholder.transform.localPosition = Vector3.zero;
						this.projectilePlaceholder.transform.localRotation = Quaternion.identity;
						this.projectileStart.gameObject.SetActive(true);
						this.projectilePlaceholder.SetActive(true);
					}
				}
				else
				{
					bool flag7 = this.projectilePlaceholder == null;
					if (flag7)
					{
						this.projectileStart.FindChild("Vis").gameObject.SetActive(false);
						this.projectilePlaceholder = this.GetAdProjectilePrefabPlaceholder(this.Prefab);
						this.projectilePlaceholder.transform.parent = this.projectileStart;
						this.projectilePlaceholder.transform.localPosition = Vector3.zero;
						this.projectilePlaceholder.transform.localRotation = Quaternion.identity;
					}
				}
				bool flag8 = base.Module.Sounds != null && base.Module.Sounds.Length != 0;
				if (flag8)
				{
					Transform transform2 = Object.Instantiate<GameObject>(AdCustomModuleMod.mod3.ShootingSoundControl).transform;
					transform2.parent = base.transform;
					transform2.gameObject.SetActive(true);
					transform2.localPosition = Vector3.zero;
					this.soundController = transform2.GetComponent<RandomSoundController>();
					List<AudioClip> list = new List<AudioClip>();
					object[] sounds = base.Module.Sounds;
					foreach (object obj in sounds)
					{
						bool flag9 = obj is ResourceReference;
						if (flag9)
						{
							ModAudioClip modAudioClip = (ModAudioClip)base.GetResource((ResourceReference)obj);
							list.Add(modAudioClip);
						}
						else
						{
							Debug.Log("Unknown sound type!");
						}
					}
					this.soundController.audioclips2 = list.ToArray();
				}
				this.projectileStart.FindChild("Vis").gameObject.SetActive(base.Module.ShowPlaceholderProjectile);
				this.spawnedProjectiles = new List<Transform>();
				bool flag10 = base.SimPhysics && !this.useProjManager;
				if (flag10)
				{
					this.projectilePrefabScript = this.Prefab.GetComponent<AdProjectileScript>();
					this.projectileArray = new AdProjectileScript.Projectile[base.Module.PoolSize];
					for (int k = 0; k < this.projectileArray.Length; k++)
					{
						GameObject gameObject = (GameObject)Object.Instantiate(this.Prefab, this.projectileStart.position, this.projectileStart.rotation);
						this.spawnedProjectiles.Add(gameObject.transform);
						AdProjectileScript.Projectile projectile = new AdProjectileScript.Projectile(gameObject);
						this.projectileArray[k] = projectile;
						projectile.transform.parent = this.GetPhysGoal();
						projectile.NewProjectile(gameObject);
						projectile.projectileScript.attackDamage = base.Module.Shootingstateinfo.EntityDamage;
						projectile.projectileScript.blockDamageAmount = base.Module.Shootingstateinfo.BlockDamage;
						projectile.projectileScript.hasAttached = !this.projectilePrefabScript.useKillTimer;
						projectile.projectileScript.disableCollider = false;
						projectile.gameObject.SetActive(false);
					}
				}
				this.timeDiff = 0f;
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00002191 File Offset: 0x00000391
		public void OnEnable()
		{
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00013220 File Offset: 0x00011420
		public GameObject GetAdProjectilePrefab()
		{
			bool flag = this.Prefab != null;
			GameObject prefab;
			if (flag)
			{
				prefab = this.Prefab;
			}
			else
			{
				this.Prefab = Object.Instantiate<GameObject>(AdCustomModuleMod.mod3.ProjectileTemplate);
				ProjectileInfo component = this.Prefab.GetComponent<ProjectileInfo>();
				AdProjectileScript component2 = this.Prefab.GetComponent<AdProjectileScript>();
				AdNetworkProjectile component3 = this.Prefab.GetComponent<AdNetworkProjectile>();
				Rigidbody component4 = this.Prefab.GetComponent<Rigidbody>();
				Transform transform = this.Prefab.transform.FindChild("Gyro").transform;
				Transform transform2 = transform.FindChild("Vis").transform;
				Transform transform3 = transform.FindChild("Colliders").transform;
				WaterProjectileBehaviour component5 = this.Prefab.GetComponent<WaterProjectileBehaviour>();
				component5.thisAdprojectile = true;
				component5.floating = base.Module.Shootingstateinfo.Buoyancy;
				MeshReference mesh = base.Module.Shootingstateinfo.Mesh;
				ModMesh modMesh = (ModMesh)base.GetResource(base.Module.Shootingstateinfo.Mesh);
				ModTexture modTexture = (ModTexture)base.GetResource(base.Module.Shootingstateinfo.Texture);
				transform2.gameObject.GetComponent<MeshFilter>().mesh = modMesh;
				MeshRenderer component6 = transform2.gameObject.GetComponent<MeshRenderer>();
				component6.material.mainTexture = modTexture;
				transform2.localPosition = mesh.Position;
				transform2.localRotation = Quaternion.Euler(mesh.Rotation);
				transform2.localScale = mesh.Scale;
				component4.useGravity = !base.Module.Shootingstateinfo.IgnoreGravity;
				component4.mass = (component3.bodyMass = base.Module.Shootingstateinfo.Mass);
				component4.drag = (component3.bodyDrag = base.Module.Shootingstateinfo.Drag);
				component4.angularDrag = (component3.bodyAngularDrag = base.Module.Shootingstateinfo.AngularDrag);
				transform3.localPosition = Vector3.zero;
				transform3.localRotation = Quaternion.identity;
				transform3.localScale = Vector3.one;
				List<Collider> list = new List<Collider>();
				ModCollider[] colliders = base.Module.Shootingstateinfo.Colliders;
				foreach (ModCollider modCollider in colliders)
				{
					Collider item = modCollider.CreateCollider(transform3);
					list.Add(item);
				}
				switch (base.Module.Shootingstateinfo.CollisionTypeS)
				{
				case CollisionType.Discrete:
					component4.collisionDetectionMode = (component3.bodyCollisionMode = 0);
					break;
				case CollisionType.Continuous:
					component4.collisionDetectionMode = (component3.bodyCollisionMode = (CollisionDetectionMode)1);
					break;
				case CollisionType.ContinuousDynamic:
					component4.collisionDetectionMode = (component3.bodyCollisionMode = (CollisionDetectionMode)2);
					break;
				}
				float frictionStr = base.Module.Shootingstateinfo.FrictionStr;
				CombineType friCombType = base.Module.Shootingstateinfo.FriCombType;
				float bounceStr = base.Module.Shootingstateinfo.BounceStr;
				CombineType bounceComType = base.Module.Shootingstateinfo.BounceComType;
				float num = frictionStr;
				float bounciness = bounceStr;
				this.PhysMaterial = new PhysicMaterial();
				this.PhysMaterial.staticFriction = num;
				this.PhysMaterial.dynamicFriction = num;
				switch (friCombType)
				{
				case CombineType.Average:
					this.PhysMaterial.frictionCombine = 0;
					break;
				case CombineType.Minimum:
					this.PhysMaterial.frictionCombine = (PhysicMaterialCombine)2;
					break;
				case CombineType.Maximum:
					this.PhysMaterial.frictionCombine = (PhysicMaterialCombine)3;
					break;
				case CombineType.Multiply:
					this.PhysMaterial.frictionCombine = (PhysicMaterialCombine)1;
					break;
				}
				this.PhysMaterial.bounciness = bounciness;
				switch (bounceComType)
				{
				case CombineType.Average:
					this.PhysMaterial.bounceCombine = 0;
					break;
				case CombineType.Minimum:
					this.PhysMaterial.bounceCombine = (PhysicMaterialCombine)2;
					break;
				case CombineType.Maximum:
					this.PhysMaterial.bounceCombine = (PhysicMaterialCombine)3;
					break;
				case CombineType.Multiply:
					this.PhysMaterial.bounceCombine = (PhysicMaterialCombine)1;
					break;
				}
				foreach (Collider collider in list)
				{
					collider.material = this.PhysMaterial;
				}
				bool flag2 = base.Module.TrailEffect != null;
				if (flag2)
				{
					component2.useTrailEffect = true;
				}
				bool flag3 = base.Module.BulletEffect != null;
				if (flag3)
				{
					component2.useBulletEffect = true;
				}
				bool useBooster = base.Module.useBooster;
				if (useBooster)
				{
					component2.useBooster = true;
					this.Prefab.AddComponent<AdActiveChaffExplosionComponent>();
				}
				bool useExplodeRotation = base.Module.useExplodeRotation;
				if (useExplodeRotation)
				{
					component2.useExplodeEffectRotation = true;
				}
				bool useBeacon = base.Module.useBeacon;
				if (useBeacon)
				{
					component2.useBeacon = true;
					component2.Mratio = base.Module.GuidRatio;
					component2.Guidtype = base.Module.Guidtype;
				}
				bool useFreezingAttack = base.Module.useFreezingAttack;
				if (useFreezingAttack)
				{
					component2.freezing = true;
				}
				bool flag4 = base.Module.ProjectileSounds != null && base.Module.ProjectileSounds.Length != 0;
				if (flag4)
				{
					component2.useProjectileSound = true;
					this.Prefab.AddComponent<AudioSource>();
					this.Prefab.AddComponent<RandomSoundPitch>();
					AudioSource component7 = this.Prefab.transform.GetComponent<AudioSource>();
					component7.spatialBlend = 1f;
					component7.minDistance = 10f;
					bool flag5 = !AdCustomModuleMod.mod2.ProjectileSoundContainer.ContainsKey(this.BlockName);
					if (flag5)
					{
						object[] projectileSounds = base.Module.ProjectileSounds;
						foreach (object obj in projectileSounds)
						{
							bool flag6 = obj is ResourceReference;
							if (flag6)
							{
								ModAudioClip value = (ModAudioClip)base.GetResource((ResourceReference)obj);
								AdCustomModuleMod.mod2.ProjectileSoundContainer.Add(this.BlockName, value);
							}
							else
							{
								Debug.Log("Unknown sound type!");
							}
						}
					}
				}
				else
				{
					component2.useProjectileSound = false;
				}
				component2.Randomfuse = base.Module.RandomFuseInterval;
				component2.fuseDelay = base.Module.FuseDelayTime;
				component2.cols = list.ToArray();
				component2.canExplode = base.Module.SupportsExplosionGodTool;
				component2.alwaysExplodes = base.Module.ProjectilesExplode;
				component2.despawnOnCollision = base.Module.ProjectilesDespawnImmediately;
				component2.attackDamage = base.Module.Shootingstateinfo.EntityDamage;
				component2.blockDamageAmount = base.Module.Shootingstateinfo.BlockDamage;
				component2.canAttach = base.Module.Shootingstateinfo.Attaches;
				component2.projectileInfo = component;
				component2.disableCollider = false;
				component2.BlockName = this.BlockName;
				component3.BlockName = this.BlockName;
				prefab = this.Prefab;
			}
			return prefab;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0001397C File Offset: 0x00011B7C
		public GameObject GetAdProjectilePrefabPlaceholder(GameObject PrefabPlaceholder)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabPlaceholder);
			Object.Destroy(gameObject.GetComponent<AdNetworkProjectile>());
			Object.Destroy(gameObject.GetComponent<AdProjectileScript>());
			Object.Destroy(gameObject.GetComponent<WaterProjectileBehaviour>());
			Object.Destroy(gameObject.GetComponent<ProjectileInfo>());
			Object.Destroy(gameObject.GetComponent<Rigidbody>());
			bool flag = !base.Module.PlaceholderProjectileUseCollider;
			if (flag)
			{
				Transform transform = gameObject.transform.FindChild("Gyro").FindChild("Colliders");
				transform.gameObject.SetActive(false);
			}
			bool useBooster = base.Module.useBooster;
			if (useBooster)
			{
			}
			bool flag2 = base.Module.ProjectileSounds != null && base.Module.ProjectileSounds.Length != 0;
			if (flag2)
			{
				Object.Destroy(gameObject.GetComponent<AudioSource>());
				Object.Destroy(gameObject.GetComponent<RandomSoundPitch>());
			}
			return gameObject;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00013A60 File Offset: 0x00011C60
		public void PostRegisterPrefabs(GameObject Prefab)
		{
			bool flag = !AdCustomModuleMod.mod2.AdShootingModuleProjectiles.ContainsValue(this.BlockName);
			int num;
			if (flag)
			{
				num = AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Count;
				base.Module.ProjectileId = num;
				this.ProjectileId = num;
				AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Add(base.Module.ProjectileId, this.BlockName);
				ProjectileInfo component = Prefab.GetComponent<ProjectileInfo>();
				component.AdProjectileType = num;
				AdProjectileManager instance = AdProjectileManager.Instance;
				instance.AddAdditionalProjectile(base.Module.ProjectileId, Prefab);
				bool isMP = StatMaster.isMP;
				if (isMP)
				{
					bool isClient = StatMaster.isClient;
					if (isClient)
					{
						this.msgSending = true;
						base.StartCoroutine(this.PostRegisterPrefabs2());
					}
					else
					{
						bool isHosting = StatMaster.isHosting;
						if (isHosting)
						{
							AdCustomModuleMod.mod2.ProjectilePoolMaching.Add(num, num);
						}
					}
				}
			}
			else
			{
				List<int> list = new List<int>(AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Keys);
				List<string> list2 = new List<string>(AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Values);
				int index = list2.IndexOf(this.BlockName);
				int num2 = list[index];
				num = num2;
				base.Module.ProjectileId = num;
				this.ProjectileId = num;
				ProjectileInfo component2 = Prefab.GetComponent<ProjectileInfo>();
				component2.AdProjectileType = num;
			}
			KeyMachingTable keyMachingTable = AdCustomModuleMod.mod2.ProjectileSkinTable.List.FirstOrDefault((KeyMachingTable x) => x.Name == this.BlockName);
			bool flag2 = keyMachingTable == null;
			if (flag2)
			{
				KeyMachingTable keyMachingTable2 = new KeyMachingTable();
				keyMachingTable2.Key = num;
				keyMachingTable2.Name = this.BlockName;
				AdCustomModuleMod.mod2.ProjectileSkinTable.List.Add(keyMachingTable2);
			}
			keyMachingTable = AdCustomModuleMod.mod2.ProjectileSkinTable.List.FirstOrDefault((KeyMachingTable x) => x.Name == this.BlockName);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00002723 File Offset: 0x00000923
		private IEnumerator PostRegisterPrefabs2()
		{
			yield return new WaitForSeconds(0.2f);
			ModNetworking.SendToHost(AdCustomModuleMod.msgPoolinfoCall.CreateMessage(new object[]
			{
				AdCustomModuleMod.LocalPlayerNetworkID,
				this.BlockName
			}));
			yield return new WaitForSeconds(0.5f);
			this.msgSending = false;
			yield return null;
			yield break;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00002732 File Offset: 0x00000932
		private IEnumerator PostRegisterPrefabs3()
		{
			yield return new WaitForSeconds(0.2f);
			ModNetworking.SendToHost(AdCustomModuleMod.msgSkininfoCall.CreateMessage(new object[]
			{
				AdCustomModuleMod.LocalPlayerNetworkID,
				this.BlockName,
				this.SkinName
			}));
			yield return new WaitForSeconds(0.5f);
			yield return null;
			yield break;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00013C40 File Offset: 0x00011E40
		public void GetExplosionPrefab()
		{
			bool flag = base.Module.ExplodeEffect != null;
			if (flag)
			{
				bool flag2 = !AdCustomModuleMod.mod2.ExplosionContainer.ContainsKey(this.BlockName);
				if (flag2)
				{
					bool useDefaultAsset = base.Module.useDefaultAsset;
					AssetBundle assetBundle;
					if (useDefaultAsset)
					{
						//assetBundle = AdCustomModuleMod.mod2.modAssetBundle;
					}
					else
					{
						string name = base.Module.AssetBundleName.Name;
						RuntimePlatform platform = Application.platform;
						RuntimePlatform runtimePlatform = platform;
						if ((int)runtimePlatform != 1)
						{
							if ((int)runtimePlatform != 2)
							{
								if ((int)runtimePlatform != 13)
								{
									assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
								}
								else
								{
									ResourceReference resourceReference = new ResourceReference();
									resourceReference.Name = name + "Mac";
									try
									{
										assetBundle = (ModAssetBundle)base.GetResource(resourceReference);
									}
									catch
									{
										assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
										ModConsole.Log("ACM : Don't detect asset for Linux!");
									}
								}
							}
							else
							{
								assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							}
						}
						else
						{
							ResourceReference resourceReference2 = new ResourceReference();
							resourceReference2.Name = name + "Mac";
							try
							{
								assetBundle = (ModAssetBundle)base.GetResource(resourceReference2);
							}
							catch
							{
								assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
								ModConsole.Log("ACM : Don't detect asset for OSX!");
							}
						}
					}
					/*
					GameObject gameObject = assetBundle.LoadAsset<GameObject>(base.Module.ExplodeEffect);
					gameObject = Object.Instantiate<GameObject>(gameObject);
					gameObject.SetActive(false);
					gameObject.transform.SetParent(AdCustomModuleMod.mod2.PMcomponent.transform);
					gameObject.AddComponent<AdExplosionEffect>();
					gameObject.AddComponent<RandomSeed>();*/
					AdExplosionEffect component = gameObject.transform.GetComponent<AdExplosionEffect>();
					component.radius = base.Module.ExplodeRadius;
					component.power = base.Module.ExplodePower;
					component.upPower = base.Module.ExplodeUpPower;
					component.BlockName = this.BlockName;
					gameObject.AddComponent<AudioSource>();
					gameObject.AddComponent<RandomSoundPitch>();
					AudioSource component2 = gameObject.transform.GetComponent<AudioSource>();
					component2.spatialBlend = 1f;
					component2.minDistance = 10f;
					bool flag3 = base.Module.HitSounds != null && base.Module.HitSounds.Length != 0;
					if (flag3)
					{
						bool flag4 = !AdCustomModuleMod.mod2.ExplodeSoundContainer.ContainsKey(this.BlockName);
						if (flag4)
						{
							object[] hitSounds = base.Module.HitSounds;
							foreach (object obj in hitSounds)
							{
								bool flag5 = obj is ResourceReference;
								if (flag5)
								{
									ModAudioClip value = (ModAudioClip)base.GetResource((ResourceReference)obj);
									AdCustomModuleMod.mod2.ExplodeSoundContainer.Add(this.BlockName, value);
								}
								else
								{
									Debug.Log("Unknown sound type!");
								}
							}
						}
					}
					AdCustomModuleMod.mod2.ExplosionContainer.Add(this.BlockName, gameObject);
					List<Transform> list = new List<Transform>();
					for (int j = 0; j < AdCustomModuleMod.mod2.MAX_Effect_POOL_SIZE + 1; j++)
					{
						Transform transform = Object.Instantiate<GameObject>(AdCustomModuleMod.mod2.ExplosionContainer[this.BlockName]).transform;
						transform.name = "ExplosionEffect";
						transform.SetParent(AdCustomModuleMod.mod2.PMEffectPool.transform);
						list.Add(transform);
						transform.gameObject.SetActive(false);
					}
					AdCustomModuleMod.mod2.ExplosionEffectContainer.Add(this.BlockName, list);
					AdCustomModuleMod.mod2.ExplosionEffectCountContainer.Add(this.BlockName, 0);
				}
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00014064 File Offset: 0x00012264
		public void GetShotFlashPrefab()
		{
			bool flag = base.Module.ShotFlashEffect != null;
			if (flag)
			{
				bool flag2 = !AdCustomModuleMod.mod2.ShotFlashContainer.ContainsKey(this.BlockName);
				if (flag2)
				{
					bool useDefaultAsset = base.Module.useDefaultAsset;
					AssetBundle assetBundle;
					if (useDefaultAsset)
					{
						//assetBundle = AdCustomModuleMod.mod2.modAssetBundle;
					}
					else
					{
						string name = base.Module.AssetBundleName.Name;
						RuntimePlatform platform = Application.platform;
						RuntimePlatform runtimePlatform = platform;
						if ((int)runtimePlatform != 1)
						{
							if ((int)runtimePlatform != 2)
							{
								if ((int)runtimePlatform != 13)
								{
									assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
								}
								else
								{
									ResourceReference resourceReference = new ResourceReference();
									resourceReference.Name = name + "Mac";
									try
									{
										assetBundle = (ModAssetBundle)base.GetResource(resourceReference);
									}
									catch
									{
										assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
										ModConsole.Log("ACM : Don't detect asset for Linux!");
									}
								}
							}
							else
							{
								assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							}
						}
						else
						{
							ResourceReference resourceReference2 = new ResourceReference();
							resourceReference2.Name = name + "Mac";
							try
							{
								assetBundle = (ModAssetBundle)base.GetResource(resourceReference2);
							}
							catch
							{
								assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
								ModConsole.Log("ACM : Don't detect asset for OSX!");
							}
						}
					}
					//GameObject gameObject = assetBundle.LoadAsset<GameObject>(base.Module.ShotFlashEffect);
					gameObject.AddComponent<RandomSeed>();
					AdCustomModuleMod.mod2.ShotFlashContainer.Add(this.BlockName, gameObject);
				}
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00014258 File Offset: 0x00012458
		public void GetTrailPrefab()
		{
			bool flag = base.Module.TrailEffect != null;
			if (flag)
			{
				bool flag2 = !AdCustomModuleMod.mod2.TrailContainer.ContainsKey(this.BlockName);
				if (flag2)
				{
					string name = base.Module.AssetBundleName.Name;
					RuntimePlatform platform = Application.platform;
					RuntimePlatform runtimePlatform = platform;
					AssetBundle assetBundle;
					if ((int)runtimePlatform != 1)
					{
						if ((int)runtimePlatform != 2)
						{
							if ((int)runtimePlatform != 13)
							{
								assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							}
							else
							{
								ResourceReference resourceReference = new ResourceReference();
								resourceReference.Name = name + "Mac";
								try
								{
									assetBundle = (ModAssetBundle)base.GetResource(resourceReference);
								}
								catch
								{
									assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
									ModConsole.Log("ACM : Don't detect asset for OSX!");
								}
							}
						}
						else
						{
							assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
						}
					}
					else
					{
						ResourceReference resourceReference2 = new ResourceReference();
						resourceReference2.Name = name + "Mac";
						try
						{
							assetBundle = (ModAssetBundle)base.GetResource(resourceReference2);
						}
						catch
						{
							assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							ModConsole.Log("ACM : Don't detect asset for OSX!");
						}
					}
					GameObject gameObject = assetBundle.LoadAsset<GameObject>(base.Module.TrailEffect);
					gameObject.AddComponent<TrailResset>();
					gameObject.AddComponent<RandomSeed>();
					AdCustomModuleMod.mod2.TrailContainer.Add(this.BlockName, gameObject);
					List<Transform> list = new List<Transform>();
					for (int i = 0; i < AdCustomModuleMod.mod2.MAX_Effect_POOL_SIZE + 1; i++)
					{
						Transform transform = Object.Instantiate<GameObject>(AdCustomModuleMod.mod2.TrailContainer[this.BlockName]).transform;
						transform.name = "TrailEffect";
						transform.SetParent(AdCustomModuleMod.mod2.PMEffectPool.transform);
						list.Add(transform);
						transform.gameObject.SetActive(false);
					}
					AdCustomModuleMod.mod2.TrailEffectContainer.Add(this.BlockName, list);
				}
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000144C8 File Offset: 0x000126C8
		public void GetBulletEffectPrefab()
		{
			bool flag = base.Module.BulletEffect != null;
			if (flag)
			{
				bool flag2 = !AdCustomModuleMod.mod2.BulletEffectContainer.ContainsKey(this.BlockName);
				if (flag2)
				{
					string name = base.Module.AssetBundleName.Name;
					RuntimePlatform platform = Application.platform;
					RuntimePlatform runtimePlatform = platform;
					AssetBundle assetBundle;
					if ((int)runtimePlatform != 1)
					{
						if ((int)runtimePlatform != 2)
						{
							if ((int)runtimePlatform != 13)
							{
								assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							}
							else
							{
								ResourceReference resourceReference = new ResourceReference();
								resourceReference.Name = name + "Mac";
								try
								{
									assetBundle = (ModAssetBundle)base.GetResource(resourceReference);
								}
								catch
								{
									assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
									ModConsole.Log("ACM : Don't detect asset for OSX!");
								}
							}
						}
						else
						{
							assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
						}
					}
					else
					{
						ResourceReference resourceReference2 = new ResourceReference();
						resourceReference2.Name = name + "Mac";
						try
						{
							assetBundle = (ModAssetBundle)base.GetResource(resourceReference2);
						}
						catch
						{
							assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							ModConsole.Log("ACM : Don't detect asset for OSX!");
						}
					}
					GameObject gameObject = assetBundle.LoadAsset<GameObject>(base.Module.BulletEffect);
					gameObject.AddComponent<TrailResset>();
					gameObject.AddComponent<RandomSeed>();
					AdCustomModuleMod.mod2.BulletEffectContainer.Add(this.BlockName, gameObject);
				}
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00014698 File Offset: 0x00012898
		public void GetChaffPrefab()
		{
			bool flag = base.Module.ChaffEffect != null;
			if (flag)
			{
				bool flag2 = !AdCustomModuleMod.mod2.ExplosionContainer.ContainsKey(this.BlockName);
				if (flag2)
				{
					string name = base.Module.AssetBundleName.Name;
					RuntimePlatform platform = Application.platform;
					RuntimePlatform runtimePlatform = platform;
					AssetBundle assetBundle;
					if ((int)runtimePlatform != 1)
					{
						if ((int)runtimePlatform != 2)
						{
							if ((int)runtimePlatform != 13)
							{
								assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							}
							else
							{
								ResourceReference resourceReference = new ResourceReference();
								resourceReference.Name = name + "Mac";
								try
								{
									assetBundle = (ModAssetBundle)base.GetResource(resourceReference);
								}
								catch
								{
									assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
									ModConsole.Log("ACM : Don't detect asset for OSX!");
								}
							}
						}
						else
						{
							assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
						}
					}
					else
					{
						ResourceReference resourceReference2 = new ResourceReference();
						resourceReference2.Name = name + "Mac";
						try
						{
							assetBundle = (ModAssetBundle)base.GetResource(resourceReference2);
						}
						catch
						{
							assetBundle = (ModAssetBundle)base.GetResource(base.Module.AssetBundleName);
							ModConsole.Log("ACM : Don't detect asset for OSX!");
						}
					}
					GameObject gameObject = assetBundle.LoadAsset<GameObject>(base.Module.ChaffEffect);
					gameObject.AddComponent<AdChaffEffect>();
					gameObject.AddComponent<RandomSeed>();
					AdChaffEffect component = gameObject.GetComponent<AdChaffEffect>();
					component.BlockName = this.BlockName;
					component.radius = base.Module.ExplodeRadius;
					gameObject.AddComponent<AudioSource>();
					gameObject.AddComponent<RandomSoundPitch>();
					AudioSource component2 = gameObject.transform.GetComponent<AudioSource>();
					component2.spatialBlend = 1f;
					component2.minDistance = 10f;
					bool flag3 = base.Module.HitSounds != null && base.Module.HitSounds.Length != 0;
					if (flag3)
					{
						bool flag4 = !AdCustomModuleMod.mod2.ExplodeSoundContainer.ContainsKey(this.BlockName);
						if (flag4)
						{
							object[] hitSounds = base.Module.HitSounds;
							foreach (object obj in hitSounds)
							{
								bool flag5 = obj is ResourceReference;
								if (flag5)
								{
									ModAudioClip value = (ModAudioClip)base.GetResource((ResourceReference)obj);
									AdCustomModuleMod.mod2.ExplodeSoundContainer.Add(this.BlockName, value);
								}
								else
								{
									Debug.Log("Unknown sound type!");
								}
							}
						}
					}
					AdCustomModuleMod.mod2.ExplosionContainer.Add(this.BlockName, gameObject);
					List<Transform> list = new List<Transform>();
					for (int j = 0; j < AdCustomModuleMod.mod2.MAX_Effect_POOL_SIZE + 1; j++)
					{
						Transform transform = Object.Instantiate<GameObject>(AdCustomModuleMod.mod2.ExplosionContainer[this.BlockName]).transform;
						transform.name = "ExplosionEffect";
						transform.SetParent(AdCustomModuleMod.mod2.PMEffectPool.transform);
						list.Add(transform);
						transform.gameObject.SetActive(false);
					}
					AdCustomModuleMod.mod2.ExplosionEffectContainer.Add(this.BlockName, list);
					AdCustomModuleMod.mod2.ExplosionEffectCountContainer.Add(this.BlockName, 0);
				}
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00014A44 File Offset: 0x00012C44
		public void Update()
		{
			bool isMP = StatMaster.isMP;
			bool flag = !isMP || base.Machine.Player.InternalObject.PlayMode == BesiegePlayMode.GlobalSimulation;
			bool flag2 = StatMaster.levelSimulating && flag;
			if (flag2)
			{
				bool isClient = StatMaster.isClient;
				if (isClient)
				{
					bool flag3 = !this.msgSending && !AdCustomModuleMod.mod2.ProjectilePoolMaching.ContainsValue(base.Module.ProjectileId);
					if (flag3)
					{
						base.StartCoroutine(this.PostRegisterPrefabs2());
						this.msgSending = true;
					}
				}
				bool isHosting = StatMaster.isHosting;
				if (isHosting)
				{
					bool flag4 = !this.msgSending && !AdCustomModuleMod.mod2.ProjectilePoolMaching.ContainsValue(base.Module.ProjectileId);
					if (flag4)
					{
						AdCustomModuleMod.mod2.ProjectilePoolMaching.Add(base.Module.ProjectileId, base.Module.ProjectileId);
						this.msgSending = true;
					}
				}
				bool flag5 = !this.SimulationStart;
				if (flag5)
				{
					this.SimulationStart = true;
					bool flag6 = StatMaster.isMP && base.Module.useBeacon;
					if (flag6)
					{
						bool flag7 = base.Machine.Player.NetworkId == Player.GetLocalPlayer().NetworkId;
						if (flag7)
						{
							AdCustomModuleMod.mod2.MissileNum++;
						}
					}
				}
			}
			else
			{
				bool simulationStart = this.SimulationStart;
				if (simulationStart)
				{
					this.SimulationStart = false;
				}
			}
			bool flag8 = this.FlipInvert == 1;
			if (flag8)
			{
				bool flipflag = this.Flipflag;
				if (flipflag)
				{
					this.projectileStartPosition = base.Module.ProjectileStart;
					this.PurgeVector = base.Module.PurgeVector;
					this.ShotFlashPosition = base.Module.ShotFlashPosition;
					this.projectileStartPosition.SetOnTransform(this.projectileStart);
					this.Flipflag = false;
				}
			}
			else
			{
				bool flag9 = !this.Flipflag;
				if (flag9)
				{
					this.projectileStartPosition = base.Module.ProjectileStart;
					this.PurgeVector = base.Module.PurgeVector;
					this.ShotFlashPosition = base.Module.ShotFlashPosition;
					this.projectileStartPosition.FlipTransform();
					this.projectileStartPosition.SetOnTransform(this.projectileStart);
					this.PurgeVector.x = -1f * this.PurgeVector.x;
					this.ShotFlashPosition.FlipTransform();
					this.Flipflag = true;
				}
			}
			bool flag10 = !this.BVCcomponent;
			if (flag10)
			{
				this.BVCcomponent = base.GetComponent<BlockVisualController>();
			}
			bool flag11 = this.BVCcomponent && this.projectilePlaceholder;
			if (flag11)
			{
				bool skinsEnabled = OptionsMaster.skinsEnabled;
				if (skinsEnabled)
				{
					this.SkinName = this.BVCcomponent.selectedSkin.pack.name;
				}
				else
				{
					this.SkinName = "DEFAULT";
				}
				bool flag12 = !(this.SkinName == this.preSkinName) || this.skinLoading;
				if (flag12)
				{
					this.preSkinName = this.SkinName;
					this.skinLoading = true;
					this.subMeshchecker = false;
					this.subTexchecker = false;
					this.msgSending2 = false;
					bool flag13 = !AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey(this.SkinName);
					if (flag13)
					{
						AdSkinLoader.AdSkinDataPack value = new AdSkinLoader.AdSkinDataPack();
						AdCustomModuleMod.mod2.ProjectileSkinPackContainer.Add(this.SkinName, value);
					}
					else
					{
						bool flag14 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer[this.SkinName].ProjectileSkinContainer.ContainsKey(this.BlockName);
						if (flag14)
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData = AdCustomModuleMod.mod2.ProjectileSkinPackContainer[this.SkinName].ProjectileSkinContainer[this.BlockName];
							this.projectileSkin = adSkinData;
						}
						else
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData2 = new AdSkinLoader.AdSkinDataPack.AdSkinData();
							bool flag15 = this.SkinName == "DEFAULT";
							if (flag15)
							{
								ModMesh modMesh = (ModMesh)base.GetResource(base.Module.Shootingstateinfo.Mesh);
								ModTexture modTexture = (ModTexture)base.GetResource(base.Module.Shootingstateinfo.Texture);
								adSkinData2.AdDefualtSkinSet(modMesh, modTexture);
								this.projectileSkin = adSkinData2;
							}
							else
							{
								string objpath = string.Concat(new string[]
								{
									this.BVCcomponent.selectedSkin.pack.path,
									"/",
									this.BlockName,
									"/projectile/",
									this.BlockName,
									".obj"
								});
								string texpath = string.Concat(new string[]
								{
									this.BVCcomponent.selectedSkin.pack.path,
									"/",
									this.BlockName,
									"/projectile/",
									this.BlockName,
									".png"
								});
								adSkinData2.AdSkinSet(this.BlockName, objpath, texpath);
								this.projectileSkin = adSkinData2;
							}
							AdCustomModuleMod.mod2.ProjectileSkinPackContainer[this.SkinName].ProjectileSkinContainer.Add(this.BlockName, adSkinData2);
						}
						this.skinLoading = false;
					}
					KeyMachingTable keyMachingTable = AdCustomModuleMod.mod2.ProjectileSkinTable.List.FirstOrDefault((KeyMachingTable x) => x.Name == this.BlockName);
					KeyMachingTable keyMachingTable2 = keyMachingTable.List.FirstOrDefault((KeyMachingTable c) => c.Name == this.SkinName);
					bool flag16 = keyMachingTable2 == null;
					if (flag16)
					{
						int key = keyMachingTable.List.Count<KeyMachingTable>();
						KeyMachingTable keyMachingTable3 = new KeyMachingTable();
						keyMachingTable3.Key = key;
						keyMachingTable3.Name = this.SkinName;
						keyMachingTable.List.Add(keyMachingTable3);
						this.skinID = key;
					}
					else
					{
						this.skinID = keyMachingTable2.Key;
					}
				}
				else
				{
					bool flag17 = this.SkinName == "DEFAULT";
					if (flag17)
					{
						bool flag18 = !this.subMeshchecker && !this.subTexchecker;
						if (flag18)
						{
							GameObject gameObject = this.projectilePlaceholder.transform.FindChild("Gyro").FindChild("Vis").gameObject;
							this.projectileSkin.mesh.ApplyToObject(gameObject);
							this.projectileSkin.texture.ApplyToObject(gameObject);
							this.subMeshchecker = true;
							this.subTexchecker = true;
						}
					}
					else
					{
						bool flag19 = this.projectileSkin.mesh.Loaded && !this.subMeshchecker;
						if (flag19)
						{
							this.subMeshchecker = true;
							bool flag20 = !this.projectileSkin.mesh.HasError;
							if (flag20)
							{
								GameObject gameObject2 = this.projectilePlaceholder.transform.FindChild("Gyro").FindChild("Vis").gameObject;
								this.projectileSkin.mesh.ApplyToObject(gameObject2);
							}
						}
						bool flag21 = this.projectileSkin.texture.Loaded && !this.subTexchecker;
						if (flag21)
						{
							this.subTexchecker = true;
							bool flag22 = !this.projectileSkin.texture.HasError;
							if (flag22)
							{
								GameObject gameObject3 = this.projectilePlaceholder.transform.FindChild("Gyro").FindChild("Vis").gameObject;
								this.projectileSkin.texture.ApplyToObject(gameObject3);
							}
						}
					}
				}
				bool flag23 = !this.msgSending2;
				if (flag23)
				{
					base.StartCoroutine(this.PostRegisterPrefabs3());
					this.msgSending2 = true;
				}
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00002191 File Offset: 0x00000391
		public override void SimulateUpdateHost()
		{
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00002191 File Offset: 0x00000391
		public override void SimulateFixedUpdateClient()
		{
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0001522C File Offset: 0x0001342C
		public override void SimulateUpdateAlways()
		{
			bool flag = (this.FireKey.IsPressed || this.FireKey.EmulationPressed()) && !this.HoldToShootToggle.IsActive;
			if (flag)
			{
				this.autoFire = !this.autoFire;
			}
			bool flag2 = this.timeDiff <= 0f;
			if (flag2)
			{
				bool showPlaceholderProjectile = base.Module.ShowPlaceholderProjectile;
				if (showPlaceholderProjectile)
				{
					bool flag3 = !this.projectilePlaceholder.activeSelf;
					if (flag3)
					{
						this.projectilePlaceholder.SetActive(true);
					}
				}
				bool flag4 = this.AmmoLeft > 0 || base.Machine.InfiniteAmmo;
				if (flag4)
				{
					bool flag5 = ((this.FireKey.IsHeld || this.FireKey.EmulationHeld(false)) && this.HoldToShootToggle.IsActive) || this.autoFire;
					if (flag5)
					{
						bool useBurstShot = base.Module.useBurstShot;
						if (useBurstShot)
						{
							this.BurstShotCount = (float)base.Module.BurstShotNum;
							this.timeBurst = 0f;
							this.BurstShoting = true;
						}
						base.StartCoroutine(this.Fire());
					}
				}
			}
			else
			{
				bool flag6 = !this.BurstShoting;
				if (flag6)
				{
					this.timeDiff -= Time.deltaTime;
				}
				bool flag7 = this.BurstShotCount > 0f;
				if (flag7)
				{
					bool flag8 = this.timeBurst > 0f;
					if (flag8)
					{
						this.timeBurst -= Time.deltaTime;
					}
					else
					{
						base.StartCoroutine(this.Fire());
					}
				}
				else
				{
					this.BurstShoting = false;
				}
			}
			bool useThrustDelayTimer = base.Module.useThrustDelayTimer;
			if (useThrustDelayTimer)
			{
				this.ThrustDelayTimeValue = this.ThrustDelayTime.Value;
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00015404 File Offset: 0x00013604
		public override void OnReloadAmmo(ref int units, AmmoType type, bool setAmmo, bool eachBlock)
		{
			bool flag = type != null && type != base.Module.AmmoType;
			if (!flag)
			{
				if (setAmmo)
				{
					bool flag2 = eachBlock || units < base.Module.DefaultAmmo;
					if (flag2)
					{
						this.AmmoLeft = units;
						units = 0;
					}
					else
					{
						units -= base.Module.DefaultAmmo;
						this.AmmoLeft = base.Module.DefaultAmmo;
					}
				}
				else
				{
					bool flag3 = eachBlock || units <= base.Module.DefaultAmmo - this.AmmoLeft;
					if (flag3)
					{
						this.AmmoLeft += units;
						units = 0;
					}
					else
					{
						units -= base.Module.DefaultAmmo - this.AmmoLeft;
						this.AmmoLeft = base.Module.DefaultAmmo;
					}
				}
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00002741 File Offset: 0x00000941
		private IEnumerator Fire()
		{
			bool flag2 = ((this.firing || this.timeDiff > 0f) && !this.BurstShoting) || base.Machine.InternalObject.ghostMode;
			if (flag2)
			{
				yield break;
			}
			bool burstShoting = this.BurstShoting;
			if (burstShoting)
			{
				bool flag3 = this.timeBurst > 0f;
				if (flag3)
				{
					yield break;
				}
				this.BurstShotCount -= 1f;
				this.timeBurst = 1f / base.Module.RateOfBurst;
			}
			this.timeDiff = 1f / this.RateOfFire.Value;
			bool infiniteAmmo = base.Machine.InfiniteAmmo;
			bool flag4 = infiniteAmmo && !StatMaster.GodTools.HasBeenUsed;
			if (flag4)
			{
				StatMaster.GodTools.HasBeenUsed = true;
			}
			bool flag5 = (this.AmmoLeft <= 0 && !infiniteAmmo) || StatMaster.Rules.DisableProjectiles;
			if (flag5)
			{
				yield break;
			}
			this.firing = true;
			bool waited = false;
			bool simPhysics = base.SimPhysics;
			if (simPhysics)
			{
				while (!base.Machine.InternalObject.isReady || Time.timeScale == 0f)
				{
					waited = true;
					yield return null;
				}
				bool flag6 = waited;
				if (flag6)
				{
					yield return new WaitForFixedUpdate();
				}
			}
			this.AmmoLeft--;
			bool flag7 = !base.Module.useDelay;
			if (flag7)
			{
				bool useDelayTimer = base.Module.useDelayTimer;
				if (useDelayTimer)
				{
					yield return new WaitForSeconds(Random.Range(0f, base.Module.RandomInterval) + this.DelayTime.Value);
				}
				else
				{
					yield return new WaitForSeconds(Random.Range(0f, base.Module.RandomInterval));
				}
			}
			else
			{
				bool useDelayTimer2 = base.Module.useDelayTimer;
				if (useDelayTimer2)
				{
					yield return new WaitForSeconds(Random.Range(0f, base.Module.RandomInterval) + base.Module.DelayTime + this.DelayTime.Value);
				}
				else
				{
					yield return new WaitForSeconds(Random.Range(0f, base.Module.RandomInterval) + base.Module.DelayTime);
				}
			}
			yield return new WaitForSeconds(Random.Range(0f, base.Module.RandomInterval));
			bool flag8 = this.projectilePlaceholder;
			if (flag8)
			{
				this.projectilePlaceholder.SetActive(false);
			}
			bool simPhysics2 = base.SimPhysics;
			if (simPhysics2)
			{
				bool useJamReducer = base.Module.useJamReducer;
				Vector3 prpStartdelta;
				if (useJamReducer)
				{
					prpStartdelta = this.projectileStart.position + this.projectileStart.forward * Vector3.Dot(base.Rigidbody.velocity, this.projectileStart.forward) * 0.02f;
				}
				else
				{
					prpStartdelta = this.projectileStart.position;
				}
				int nearID = this.OwnerId;
				double[] distance = new double[2];
				bool useBeacon = base.Module.useBeacon;
				if (useBeacon)
				{
					bool flag = false;
					bool flag9 = AdCustomModuleMod.mod2.TaregtIdListContainer.ContainsKey(this.OwnerId);
					if (flag9)
					{
						foreach (int target in AdCustomModuleMod.mod2.TaregtIdListContainer[this.OwnerId])
						{
							bool flag10 = AdCustomModuleMod.mod2.BeaconContainer.ContainsKey(target);
							if (flag10)
							{
								Vector3 vec = AdCustomModuleMod.mod2.BeaconContainer[target].Posi;
								distance[0] = (double)(vec - base.transform.position).magnitude;
								bool flag11 = flag;
								if (flag11)
								{
									bool flag12 = distance[0] < distance[1];
									if (flag12)
									{
										distance[1] = distance[0];
										nearID = target;
									}
								}
								else
								{
									distance[1] = distance[0];
									nearID = target;
									flag = true;
								}
								vec = default(Vector3);
							}
						}
						List<int>.Enumerator enumerator = default(List<int>.Enumerator);
					}
				}
				AdProjectileScript.Projectile currentProjectile = ((!this.useProjManager && !this.projectilePrefabScript.useKillTimer) ? this.GetProjectileReverse(prpStartdelta) : this.GetProjectile(prpStartdelta, nearID, this.ThrustDelayTimeValue));
				bool flag13 = currentProjectile != null;
				if (flag13)
				{
					Rigidbody projectileBody = currentProjectile.rigidbody;
					AdProjectileScript projectileScript = currentProjectile.projectileScript;
					projectileBody.isKinematic = false;
					projectileScript.ownerID = this.OwnerId;
					projectileScript.TargetId = nearID;
					projectileScript.skinID = this.skinID;
					projectileScript.SkinName = this.SkinName;
					projectileScript.enabled = true;
					bool useTimefuse = base.Module.useTimefuse;
					if (useTimefuse)
					{
						projectileScript.Timefuse = this.Timefuse.Value + Random.Range(0f, base.Module.RandomFuseInterval);
					}
					projectileScript.DelayBoostertime = this.ThrustDelayTimeValue;
					bool useBooster = base.Module.useBooster;
					if (useBooster)
					{
						projectileBody.velocity = base.Rigidbody.velocity;
						Vector3 randomDir = this.projectileStart.forward + Random.insideUnitSphere * base.Module.RandomDiffusion;
						currentProjectile.transform.rotation = this.projectileStart.rotation * Quaternion.FromToRotation(this.projectileStart.forward, randomDir);
						projectileScript.projectileForward = currentProjectile.transform.forward;
						projectileBody.AddRelativeForce(100f * base.Module.PurgePower * this.PurgeVector);
						projectileScript.boosterPower = this.PowerSlider.Value;
						this.GetDragPrefab2();
						Transform DragTransform2 = Object.Instantiate<GameObject>(this.DragPrefab2).transform;
						DragTransform2.SetParent(this.GetPhysGoal());
						DragTransform2.position = prpStartdelta;
						DragTransform2.rotation = currentProjectile.transform.rotation;
						DragTransform2.Translate(new Vector3(0f, 0f, 0.5f), (Space)1);
						FixedJoint jointcomp3 = DragTransform2.GetComponent<FixedJoint>();
						jointcomp3.connectedBody = projectileBody;
						jointcomp3.breakForce = float.PositiveInfinity;
						jointcomp3.breakTorque = float.PositiveInfinity;
						OnJointBrakeBehavour jointcomp4 = DragTransform2.GetComponent<OnJointBrakeBehavour>();
						jointcomp4.jointTransform = projectileBody.transform;
						bool useThrustDelayTimer = base.Module.useThrustDelayTimer;
						if (useThrustDelayTimer)
						{
							WingDragBehavour2 wingcomp = DragTransform2.GetComponent<WingDragBehavour2>();
							wingcomp.DelayTime = this.ThrustDelayTime.Value;
							wingcomp = null;
						}
						DragTransform2.gameObject.SetActive(true);
						Transform DragTransform3 = Object.Instantiate<GameObject>(this.DragPrefab2).transform;
						DragTransform3.SetParent(this.GetPhysGoal());
						DragTransform3.position = prpStartdelta;
						DragTransform3.rotation = currentProjectile.transform.rotation;
						DragTransform3.Translate(new Vector3(0f, 0f, -0.6f), (Space)1);
						FixedJoint jointcomp5 = DragTransform3.GetComponent<FixedJoint>();
						jointcomp5.connectedBody = projectileBody;
						jointcomp5.breakForce = float.PositiveInfinity;
						jointcomp5.breakTorque = float.PositiveInfinity;
						OnJointBrakeBehavour jointcomp6 = DragTransform3.GetComponent<OnJointBrakeBehavour>();
						jointcomp6.jointTransform = projectileBody.transform;
						bool useThrustDelayTimer2 = base.Module.useThrustDelayTimer;
						if (useThrustDelayTimer2)
						{
							WingDragBehavour2 wingcomp2 = DragTransform3.GetComponent<WingDragBehavour2>();
							wingcomp2.DelayTime = this.ThrustDelayTime.Value;
							wingcomp2 = null;
						}
						DragTransform3.gameObject.SetActive(true);
						randomDir = default(Vector3);
						DragTransform2 = null;
						jointcomp3 = null;
						jointcomp4 = null;
						DragTransform3 = null;
						jointcomp5 = null;
						jointcomp6 = null;
					}
					else
					{
						Vector3 randomDir2 = this.projectileStart.forward + Random.insideUnitSphere * base.Module.RandomDiffusion;
						projectileBody.velocity = base.Rigidbody.velocity;
						projectileBody.AddForce(100f * this.PowerSlider.Value * randomDir2);
						Vector3 recoilForce = 100f * this.PowerSlider.Value * -randomDir2 * base.Module.RecoilMultiplier;
						base.Rigidbody.AddForce(recoilForce);
						randomDir2 = default(Vector3);
						recoilForce = default(Vector3);
					}
					projectileBody = null;
					projectileScript = null;
				}
				prpStartdelta = default(Vector3);
				distance = null;
				currentProjectile = null;
			}
			this.firing = false;
			Transform effect = this.ShotFlashList[this.ShotFlashCount];
			effect.gameObject.SetActive(true);
			this.ShotFlashPosition.SetOnTransform(effect);
			bool flag14 = this.ShotFlashCount < this.ShotFlashnum;
			if (flag14)
			{
				this.ShotFlashCount++;
			}
			else
			{
				this.ShotFlashCount = 0;
			}
			bool flag15 = this.soundController;
			if (flag15)
			{
				this.soundController.Play2(0.15f);
			}
			yield break;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000154EC File Offset: 0x000136EC
		private AdProjectileScript.Projectile GetProjectile(Vector3 prpStartdelta, int TargetId, float thrustdelayTime)
		{
			bool flag = this.useProjManager;
			AdProjectileScript.Projectile result;
			if (flag)
			{
				int num = 0;
				bool exExpand = this.ExExpand;
				byte[] array;
				if (exExpand)
				{
					array = new byte[19];
					AdNetworkCompression.CompressPosition(prpStartdelta, array, num);
					num += 12;
				}
				else
				{
					array = new byte[13];
					NetworkCompression.CompressPosition(prpStartdelta, array, num);
					num += 6;
				}
				NetworkCompression.CompressRotation(this.projectileStart.rotation, array, num);
				NetworkAddPiece instance = NetworkAddPiece.Instance;
				Transform transform = AdProjectileManager.Instance.Spawn(base.Module.ProjectileId, instance.frame, (ushort)this.OwnerId, (ushort)TargetId, (ushort)this.skinID, thrustdelayTime, array);
				this.spawnedProjectiles.Add(transform);
				AdProjectileScript.Projectile projectile = new AdProjectileScript.Projectile(transform.gameObject);
				projectile.projectileScript.ownerID = this.OwnerId;
				projectile.projectileScript.SkinName = this.SkinName;
				projectile.projectileScript.skinID = this.skinID;
				bool useTrailEffect = projectile.projectileScript.useTrailEffect;
				if (useTrailEffect)
				{
					bool flag2 = projectile.projectileScript.TrailResetcomp;
					if (flag2)
					{
						projectile.projectileScript.TrailResetcomp.ResetTrail();
					}
				}
				result = projectile;
			}
			else
			{
				AdProjectileScript.Projectile projectile;
				for (int i = this.nextProjectile; i < this.projectileArray.Length; i++)
				{
					projectile = this.projectileArray[i];
					bool flag3 = !projectile.gameObject;
					if (flag3)
					{
						GameObject gameObject = Object.Instantiate(this.Prefab, prpStartdelta, this.projectileStart.rotation) as GameObject;
						this.spawnedProjectiles.Add(gameObject.transform);
						projectile.NewProjectile(gameObject);
						projectile.transform.parent = this.GetPhysGoal();
						projectile.projectileScript.ownerID = this.OwnerId;
						projectile.projectileScript.SkinName = this.SkinName;
						projectile.projectileScript.skinID = this.skinID;
						projectile.gameObject.SetActive(false);
					}
					bool flag4 = !projectile.gameObject.activeInHierarchy;
					if (flag4)
					{
						this.InitProjectile(projectile, prpStartdelta);
						this.nextProjectile = i + 1;
						bool flag5 = this.nextProjectile >= this.projectileArray.Length;
						if (flag5)
						{
							this.nextProjectile = 0;
						}
						return projectile;
					}
					bool flag6 = i >= this.projectileArray.Length;
					if (flag6)
					{
						i = 0;
					}
					bool flag7 = i == this.nextProjectile;
					if (flag7)
					{
						i = this.projectileArray.Length;
					}
				}
				projectile = this.projectileArray[this.nextProjectile];
				this.InitProjectile(projectile, prpStartdelta);
				this.nextProjectile++;
				bool flag8 = this.nextProjectile >= this.projectileArray.Length;
				if (flag8)
				{
					this.nextProjectile = 0;
				}
				result = projectile;
			}
			return result;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000157D4 File Offset: 0x000139D4
		private AdProjectileScript.Projectile GetProjectileReverse(Vector3 prpStartdelta)
		{
			AdProjectileScript.Projectile projectile;
			for (int i = this.nextProjectile; i < this.projectileArray.Length; i++)
			{
				projectile = this.projectileArray[i];
				bool flag = !projectile.gameObject;
				if (flag)
				{
					GameObject p = Object.Instantiate(this.Prefab, prpStartdelta, this.projectileStart.rotation) as GameObject;
					this.spawnedProjectiles.Add(projectile.transform);
					projectile.NewProjectile(p);
					projectile.transform.parent = this.GetPhysGoal();
					projectile.projectileScript.ownerID = this.OwnerId;
					projectile.projectileScript.SkinName = this.SkinName;
					projectile.projectileScript.skinID = this.skinID;
				}
				bool flag2 = projectile.projectileScript.hasAttached || !projectile.projectileScript.canAttach;
				if (flag2)
				{
					projectile.rigidbody.interpolation = 0;
					this.InitProjectile(projectile, prpStartdelta);
					projectile.rigidbody.interpolation = (RigidbodyInterpolation)1;
					this.nextProjectile = i + 1;
					bool flag3 = this.nextProjectile >= this.projectileArray.Length;
					if (flag3)
					{
						this.nextProjectile = 0;
					}
					return projectile;
				}
			}
			projectile = this.projectileArray[this.nextProjectile];
			this.InitProjectile(projectile, prpStartdelta);
			this.nextProjectile++;
			bool flag4 = this.nextProjectile >= this.projectileArray.Length;
			if (flag4)
			{
				this.nextProjectile = 0;
			}
			return projectile;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00015968 File Offset: 0x00013B68
		private GameObject GetDragPrefab()
		{
			bool flag = this.DragPrefab != null;
			GameObject dragPrefab;
			if (flag)
			{
				dragPrefab = this.DragPrefab;
			}
			else
			{
				this.DragPrefab = new GameObject("Drag");
				this.DragPrefab.SetActive(false);
				Transform transform = this.DragPrefab.transform;
				transform.gameObject.AddComponent<Rigidbody>();
				Rigidbody component = transform.GetComponent<Rigidbody>();
				component.drag = 5f;
				component.angularDrag = 5f;
				component.mass = 0f;
				transform.gameObject.AddComponent<FixedJoint>();
				transform.gameObject.AddComponent<WingDragBehavour>();
				transform.gameObject.AddComponent<OnJointBrakeBehavour>();
				dragPrefab = this.DragPrefab;
			}
			return dragPrefab;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00015A24 File Offset: 0x00013C24
		private GameObject GetDragPrefab2()
		{
			bool flag = this.DragPrefab2 != null;
			GameObject dragPrefab;
			if (flag)
			{
				dragPrefab = this.DragPrefab2;
			}
			else
			{
				this.DragPrefab2 = new GameObject("DragC");
				this.DragPrefab2.SetActive(false);
				Transform transform = this.DragPrefab2.transform;
				transform.gameObject.AddComponent<Rigidbody>();
				Rigidbody component = transform.GetComponent<Rigidbody>();
				component.drag = 5f;
				component.angularDrag = 5f;
				component.mass = 0f;
				transform.gameObject.AddComponent<FixedJoint>();
				transform.gameObject.AddComponent<WingDragBehavour2>();
				transform.gameObject.AddComponent<OnJointBrakeBehavour>();
				dragPrefab = this.DragPrefab2;
			}
			return dragPrefab;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00015AE0 File Offset: 0x00013CE0
		private void InitProjectile(AdProjectileScript.Projectile proj, Vector3 prpStartdelta)
		{
			proj.transform.position = prpStartdelta;
			proj.transform.rotation = this.projectileStart.rotation;
			proj.transform.localScale = Vector3.one;
			bool flag = !proj.gameObject.activeInHierarchy;
			if (flag)
			{
				proj.gameObject.SetActive(true);
			}
			else
			{
				proj.projectileScript.OnEnable();
			}
			proj.projectileScript.ownerID = this.OwnerId;
			proj.projectileScript.SkinName = this.SkinName;
			proj.projectileScript.skinID = this.skinID;
			bool flag2 = proj.gyro;
			if (flag2)
			{
				proj.gyro.localRotation = Quaternion.identity;
			}
			bool useTrailEffect = proj.projectileScript.useTrailEffect;
			if (useTrailEffect)
			{
				bool flag3 = proj.projectileScript.TrailResetcomp;
				if (flag3)
				{
					proj.projectileScript.TrailResetcomp.ResetTrail();
				}
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00015BE4 File Offset: 0x00013DE4
		private Transform GetPhysGoal()
		{
			return ReferenceMaster.physicsGoalInstance;
		}

		// Token: 0x0400023A RID: 570
		public MSlider PowerSlider;

		// Token: 0x0400023B RID: 571
		public MSlider RateOfFire;

		// Token: 0x0400023C RID: 572
		public MKey FireKey;

		// Token: 0x0400023D RID: 573
		public MToggle HoldToShootToggle;

		// Token: 0x0400023E RID: 574
		public MSlider Timefuse;

		// Token: 0x0400023F RID: 575
		public MSlider DelayTime;

		// Token: 0x04000240 RID: 576
		public MSlider ThrustDelayTime;

		// Token: 0x04000241 RID: 577
		public float ThrustDelayTimeValue = 0f;

		// Token: 0x04000242 RID: 578
		public int AmmoLeft;

		// Token: 0x04000243 RID: 579
		public int OwnerId = 0;

		// Token: 0x04000244 RID: 580
		public string BlockName;

		// Token: 0x04000245 RID: 581
		private static int MAX_POOL_SIZE = 400;

		// Token: 0x04000246 RID: 582
		private GameObject Prefab;

		// Token: 0x04000247 RID: 583
		private AdProjectileScript.Projectile[] projectileArray;

		// Token: 0x04000248 RID: 584
		private List<Transform> spawnedProjectiles;

		// Token: 0x04000249 RID: 585
		private AdProjectileScript projectilePrefabScript;

		// Token: 0x0400024A RID: 586
		private RandomSoundController soundController;

		// Token: 0x0400024B RID: 587
		public Transform projectileStart;

		// Token: 0x0400024C RID: 588
		public Transform ShotFlashStart;

		// Token: 0x0400024D RID: 589
		private PhysicMaterial PhysMaterial;

		// Token: 0x0400024E RID: 590
		private List<Transform> ShotFlashList;

		// Token: 0x0400024F RID: 591
		private int ShotFlashnum = 20;

		// Token: 0x04000250 RID: 592
		private int ShotFlashCount = 0;

		// Token: 0x04000251 RID: 593
		private bool useProjManager;

		// Token: 0x04000252 RID: 594
		private int nextProjectile;

		// Token: 0x04000253 RID: 595
		internal int ProjectileId;

		// Token: 0x04000254 RID: 596
		public GameObject projectilePlaceholder;

		// Token: 0x04000255 RID: 597
		private float timeDiff;

		// Token: 0x04000256 RID: 598
		private bool autoFire;

		// Token: 0x04000257 RID: 599
		private bool firing;

		// Token: 0x04000258 RID: 600
		private bool msgSending = false;

		// Token: 0x04000259 RID: 601
		private bool msgSending2 = false;

		// Token: 0x0400025A RID: 602
		private GameObject DragPrefab;

		// Token: 0x0400025B RID: 603
		private GameObject DragPrefab2;

		// Token: 0x0400025C RID: 604
		public bool BurstShoting = false;

		// Token: 0x0400025D RID: 605
		public float BurstShotCount = 0f;

		// Token: 0x0400025E RID: 606
		public float timeBurst = 0f;

		// Token: 0x0400025F RID: 607
		private BlockVisualController BVCcomponent;

		// Token: 0x04000260 RID: 608
		public int skinID;

		// Token: 0x04000261 RID: 609
		private string SkinName;

		// Token: 0x04000262 RID: 610
		private string preSkinName;

		// Token: 0x04000263 RID: 611
		private bool skinLoading = false;

		// Token: 0x04000264 RID: 612
		private bool subMeshchecker = false;

		// Token: 0x04000265 RID: 613
		private bool subTexchecker = false;

		// Token: 0x04000266 RID: 614
		public AdSkinLoader.AdSkinDataPack.AdSkinData projectileSkin;

		// Token: 0x04000267 RID: 615
		private bool Flipflag = false;

		// Token: 0x04000268 RID: 616
		private Vector3 PurgeVector;

		// Token: 0x04000269 RID: 617
		private AdTransformValues projectileStartPosition = new AdTransformValues();

		// Token: 0x0400026A RID: 618
		private AdTransformValues ShotFlashPosition = new AdTransformValues();

		// Token: 0x0400026B RID: 619
		private bool SimulationStart = false;

		// Token: 0x02000034 RID: 52
		public class ModdedProjectileColliderComponent : MonoBehaviour
		{
			// Token: 0x0600011D RID: 285 RVA: 0x00002782 File Offset: 0x00000982
			public void OnCollisionEnter(Collision col)
			{
				this.projectileScript.OnCollisionEnter(col);
			}

			// Token: 0x0400026C RID: 620
			public AdProjectileScript projectileScript;
		}

		// Token: 0x02000035 RID: 53
		public class AdProjectileColliderComponent : MonoBehaviour
		{
			// Token: 0x0600011F RID: 287 RVA: 0x0000279B File Offset: 0x0000099B
			public void OnCollisionEnter(Collision col)
			{
				this.projectileScript.OnCollisionEnter(col);
			}

			// Token: 0x0400026D RID: 621
			public AdProjectileScript projectileScript;
		}
	}
}
