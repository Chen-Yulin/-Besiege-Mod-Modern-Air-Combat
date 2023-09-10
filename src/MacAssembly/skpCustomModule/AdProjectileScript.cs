using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace skpCustomModule
{
	// Token: 0x02000048 RID: 72
	public class AdProjectileScript : MonoBehaviour
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00002366 File Offset: 0x00000566
		public bool ExExpand
		{
			get
			{
				return AdCustomModuleMod.mod4.ExExpandFloor;
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00017658 File Offset: 0x00015858
		public void Awake()
		{
			this.myTransform = base.transform;
			this.myRigidbody = base.gameObject.GetComponent<Rigidbody>();
			this.gyro = base.transform.Find("Gyro").transform;
			this.noRigidbody = this.myRigidbody == null;
			this.enableCollision = false;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000176B8 File Offset: 0x000158B8
		public void Start()
		{
			this.worldUp = Vector3.up;
			bool flag = this.useProjectileSound;
			if (flag)
			{
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x000176E0 File Offset: 0x000158E0
		public void OnEnable()
		{
			bool flag = !StatMaster.levelSimulating;
			if (!flag)
			{
				this.DelayBoosterTimeEnd = false;
				this.TargetMissing = false;
				this.colliderTime = 0f;
				this.enableCollision = false;
				this.existenceTime = 0f;
				this.bombtriger = false;
				bool flag2 = this.useBeacon;
				if (flag2)
				{
					this.myRigidbody.maxAngularVelocity = 28f;
					bool flag3 = this.BlockTag == 0;
					if (flag3)
					{
						AdCustomModuleMod.mod2.ProjectilePoolCounter++;
						this.BlockTag = AdCustomModuleMod.mod2.ProjectilePoolCounter;
					}
				}
				this.useDelayBoosterTime = false;
				this.myTransform = base.transform;
				this.myRigidbody = base.gameObject.GetComponent<Rigidbody>();
				this.hasAttached = false;
				this.PrePosi = default(Vector3);
				this.gyro = base.transform.FindChild("Gyro").transform;
				this.gyro.localRotation = Quaternion.identity;
				bool flag4 = !(this.SkinName == this.preSkinName) || (OptionsMaster.skinsEnabled && !this.skinsEnabled);
				if (flag4)
				{
					this.skinsEnabled = OptionsMaster.skinsEnabled;
					this.preSkinName = this.SkinName;
					this.subMeshchecker = false;
					this.subTexchecker = false;
					bool flag5 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey(this.SkinName);
					if (flag5)
					{
						bool flag6 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer[this.SkinName].ProjectileSkinContainer.ContainsKey(this.BlockName);
						if (flag6)
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData = AdCustomModuleMod.mod2.ProjectileSkinPackContainer[this.SkinName].ProjectileSkinContainer[this.BlockName];
							this.projectileSkin = adSkinData;
						}
					}
					else
					{
						Debug.LogWarning("Don't exsit AdProjectileSkin : " + this.SkinName);
						bool flag7 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey("DEFAULT");
						if (flag7)
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData2 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer["DEFAULT"].ProjectileSkinContainer[this.BlockName];
							this.projectileSkin = adSkinData2;
						}
						else
						{
							Debug.LogWarning("Don't set : " + this.SkinName);
						}
					}
				}
				else
				{
					bool flag8 = !OptionsMaster.skinsEnabled && this.skinsEnabled;
					if (flag8)
					{
						this.subMeshchecker = false;
						this.subTexchecker = false;
						this.skinsEnabled = OptionsMaster.skinsEnabled;
						Debug.LogWarning("Don't exsit AdProjectileSkin : " + this.SkinName);
						bool flag9 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey("DEFAULT");
						if (flag9)
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData3 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer["DEFAULT"].ProjectileSkinContainer[this.BlockName];
							this.projectileSkin = adSkinData3;
						}
						else
						{
							Debug.LogWarning("Don't set : " + this.SkinName);
						}
					}
				}
				bool flag10 = this.SkinName == "DEFAULT" || !OptionsMaster.skinsEnabled;
				if (flag10)
				{
					bool flag11 = !this.subMeshchecker && !this.subTexchecker;
					if (flag11)
					{
						GameObject gameObject = base.transform.FindChild("Gyro").FindChild("Vis").gameObject;
						this.projectileSkin.mesh.ApplyToObject(gameObject);
						this.projectileSkin.texture.ApplyToObject(gameObject);
						this.subMeshchecker = true;
						this.subTexchecker = true;
					}
				}
				else
				{
					bool flag12 = this.projectileSkin.mesh.Loaded && !this.subMeshchecker;
					if (flag12)
					{
						this.subMeshchecker = true;
						bool flag13 = !this.projectileSkin.mesh.HasError;
						if (flag13)
						{
							GameObject gameObject2 = base.transform.FindChild("Gyro").FindChild("Vis").gameObject;
							this.projectileSkin.mesh.ApplyToObject(gameObject2);
						}
					}
					bool flag14 = this.projectileSkin.texture.Loaded && !this.subTexchecker;
					if (flag14)
					{
						this.subTexchecker = true;
						bool flag15 = !this.projectileSkin.texture.HasError;
						if (flag15)
						{
							GameObject gameObject3 = base.transform.FindChild("Gyro").FindChild("Vis").gameObject;
							this.projectileSkin.texture.ApplyToObject(gameObject3);
						}
					}
				}
				bool flag16 = StatMaster.isClient && StatMaster.Mode.LevelEditor.clientGlobalSim;
				if (!flag16)
				{
					bool flag17 = (!StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim) && !this.noRigidbody;
					if (flag17)
					{
						this.myRigidbody.interpolation = (RigidbodyInterpolation)1;
					}
					this.deflected = false;
					bool flag18 = this.enableRotation;
					if (flag18)
					{
						this.rotation.dps = this.rotation.degreesPerSecond * Random.Range(1f - this.rotation.randomRotationVariation, 1f + this.rotation.randomRotationVariation);
					}
					bool flag19 = this.randomSoundController == null;
					if (flag19)
					{
						this.randomSoundController = base.gameObject.GetComponent<RandomSoundController>();
					}
					bool flag20 = this.col == null;
					if (flag20)
					{
						this.col = this.cols[0];
					}
					else
					{
						bool flag21 = this.cols == null;
						if (flag21)
						{
							this.cols = new Collider[] { this.col };
						}
					}
					this.torqueArray = new Vector3[20];
				}
			}
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00017C98 File Offset: 0x00015E98
		public void OnDisable()
		{
			bool flag = this.useBeacon;
			if (flag)
			{
				this.TargetMissing = false;
				bool flag2 = this.TargetId != this.ownerID;
				if (flag2)
				{
					bool flag3 = AdCustomModuleMod.mod2.MissileToTargetListContainer[this.TargetId].Contains(this.BlockTag);
					if (flag3)
					{
						AdCustomModuleMod.mod2.MissileToTargetListContainer[this.TargetId].Remove(this.BlockTag);
					}
				}
			}
			bool flag4 = this.useDelayBoosterTime;
			if (flag4)
			{
				Object.Destroy(this.bulleteffectPrefab);
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00017D30 File Offset: 0x00015F30
		public void Update()
		{
			this.AlwaysUpdate();
			bool flag = !StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim;
			bool flag2 = !StatMaster.levelSimulating || !flag;
			if (!flag2)
			{
				bool flag3 = this.colliderTime < this.colliderEnableTime;
				if (flag3)
				{
					this.colliderTime += Time.deltaTime;
				}
				else
				{
					bool flag4 = !this.enableCollision;
					if (flag4)
					{
						this.enableCollision = true;
					}
				}
				bool flag5 = this.Timefuse != 0f;
				if (flag5)
				{
					bool flag6 = this.existenceTime < this.Timefuse;
					if (flag6)
					{
						this.existenceTime += Time.deltaTime;
					}
					else
					{
						bool flag7 = !this.bombtriger;
						if (flag7)
						{
							this.bombtriger = true;
							base.StartCoroutine(this.TimefuseBomb());
						}
					}
				}
				Rigidbody rigidbody = this.myRigidbody;
				bool flag8 = !this.noRigidbody;
				if (flag8)
				{
					bool flag9 = Time.timeScale <= 0.6f;
					if (flag9)
					{
						bool flag10 = rigidbody.interpolation == 0;
						if (flag10)
						{
							rigidbody.interpolation = (RigidbodyInterpolation)1;
						}
					}
					else
					{
						bool flag11 = (int)rigidbody.interpolation == 1;
						if (flag11)
						{
							rigidbody.interpolation = 0;
						}
					}
				}
				bool flag12 = this.hasAttached;
				if (flag12)
				{
					bool flag13 = !(this.firecontrol != null) || !this.firecontrol.onFire;
					if (!flag13)
					{
						bool flag14 = this.fTime <= 0f;
						if (flag14)
						{
							bool flag15 = this.firecontrol.fireTagCode;
							if (flag15)
							{
								this.firecontrol.fireTagCode.burning = false;
								this.firecontrol.onFire = false;
							}
						}
						else
						{
							this.fTime -= Time.deltaTime;
						}
					}
				}
				else
				{
					bool flag16 = StatMaster.isMP && this.triggerProjectile && AdProjectileScript.onUpdateProjectile != null;
					if (flag16)
					{
						bool flag17 = !this.noRigidbody;
						if (flag17)
						{
							this.projectilePosition = rigidbody.position;
						}
						AdProjectileScript.onUpdateProjectile(this);
					}
				}
			}
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00017F6C File Offset: 0x0001616C
		public void AlwaysUpdate()
		{
			bool flag = !StatMaster.levelSimulating;
			if (!flag)
			{
				bool flag2 = StatMaster.isMP && this.useBeacon;
				if (flag2)
				{
					bool flag3 = AdCustomModuleMod.mod2.MissilePositionContainer.ContainsKey(this.BlockTag);
					if (flag3)
					{
						AdCustomModuleMod.mod2.MissilePositionContainer[this.BlockTag] = this.gyro;
					}
					else
					{
						AdCustomModuleMod.mod2.MissilePositionContainer.Add(this.BlockTag, this.gyro);
					}
					bool flag4 = this.TargetId != this.ownerID && !this.TargetMissing;
					if (flag4)
					{
						bool flag5 = !AdCustomModuleMod.mod2.MissileToTargetListContainer[this.TargetId].Contains(this.BlockTag);
						if (flag5)
						{
							AdCustomModuleMod.mod2.MissileToTargetListContainer[this.TargetId].Add(this.BlockTag);
						}
					}
					bool flag6 = AdCustomModuleMod.mod2.PlayerRespawnFlagContainer.ContainsKey(this.TargetId);
					if (flag6)
					{
						bool flag7 = this.useBeacon;
						if (flag7)
						{
							bool flag8 = AdCustomModuleMod.mod2.PlayerRespawnFlagContainer[this.TargetId];
							if (flag8)
							{
								bool flag9 = this.TargetId != this.ownerID;
								if (flag9)
								{
									bool flag10 = AdCustomModuleMod.mod2.MissileToTargetListContainer[this.TargetId].Contains(this.BlockTag);
									if (flag10)
									{
										AdCustomModuleMod.mod2.MissileToTargetListContainer[this.TargetId].Remove(this.BlockTag);
									}
								}
								this.TargetMissing = true;
							}
						}
					}
				}
				bool flag11 = this.DelayBoostertime <= 0f && !this.DelayBoosterTimeEnd;
				if (flag11)
				{
					bool flag12 = this.useTrailEffect;
					if (flag12)
					{
						bool flag13 = this.gyro.FindChild("TrailEffect") == null;
						if (flag13)
						{
							int num = AdCustomModuleMod.mod2.TrailEffectCount;
							int max_Effect_POOL_SIZE = AdCustomModuleMod.mod2.MAX_Effect_POOL_SIZE;
							bool flag14 = false;
							for (int i = num; i < max_Effect_POOL_SIZE; i++)
							{
								bool flag15 = AdCustomModuleMod.mod2.TrailEffectContainer[this.BlockName][i] == null;
								if (flag15)
								{
									this.trailPrefab = Object.Instantiate<GameObject>(AdCustomModuleMod.mod2.TrailContainer[this.BlockName]);
									this.trailPrefab.name = "TrailEffect";
									AdCustomModuleMod.mod2.TrailEffectContainer[this.BlockName][i] = this.trailPrefab.transform;
									AdCustomModuleMod.mod2.TrailEffectCount = i + 1;
									flag14 = true;
									break;
								}
								bool flag16 = !AdCustomModuleMod.mod2.TrailEffectContainer[this.BlockName][i].gameObject.activeSelf;
								if (flag16)
								{
									this.trailPrefab = AdCustomModuleMod.mod2.TrailEffectContainer[this.BlockName][i].gameObject;
									AdCustomModuleMod.mod2.TrailEffectCount = i + 1;
									flag14 = true;
									break;
								}
								bool flag17 = i == max_Effect_POOL_SIZE;
								if (flag17)
								{
									num = 0;
								}
								else
								{
									bool flag18 = i == num;
									if (flag18)
									{
										i = max_Effect_POOL_SIZE;
									}
								}
							}
							bool flag19 = !flag14;
							if (flag19)
							{
								this.trailPrefab = AdCustomModuleMod.mod2.TrailEffectContainer[this.BlockName][num].gameObject;
								AdCustomModuleMod.mod2.TrailEffectCount = num + 1;
							}
							this.trailPrefab.transform.gameObject.SetActive(true);
							this.trailPrefab.transform.SetParent(this.gyro);
							this.trailPrefab.transform.position = this.gyro.position;
							this.trailPrefab.transform.rotation = this.gyro.rotation;
							this.TrailResetcomp = this.trailPrefab.transform.GetComponent<TrailResset>();
							this.TrailResetcomp.ResetTrail();
							bool flag20 = AdCustomModuleMod.mod2.TrailEffectCount < AdCustomModuleMod.mod2.MAX_Effect_POOL_SIZE;
							if (!flag20)
							{
								AdCustomModuleMod.mod2.TrailEffectCount = 0;
							}
						}
					}
					bool flag21 = this.useProjectileSound;
					if (flag21)
					{
						this.SFX = base.transform.GetComponent<RandomSoundPitch>();
						this.sound = AdCustomModuleMod.mod2.ProjectileSoundContainer[this.BlockName];
						base.StartCoroutine(this.ProjectileSFX());
					}
					bool flag22 = this.useBulletEffect;
					if (flag22)
					{
						bool flag23 = this.gyro.FindChild("BulletEffectPrefab") == null;
						if (flag23)
						{
							this.bulleteffectPrefab = Object.Instantiate(AdCustomModuleMod.mod2.BulletEffectContainer[this.BlockName], this.gyro) as GameObject;
							this.bulleteffectPrefab.name = "BulletEffectPrefab";
							this.bulleteffectPrefab.transform.position = this.gyro.position;
							this.bulleteffectPrefab.transform.rotation = this.gyro.rotation;
						}
					}
					this.DelayBoosterTimeEnd = true;
				}
				else
				{
					bool flag24 = !this.useDelayBoosterTime && !this.DelayBoosterTimeEnd;
					if (flag24)
					{
						this.useDelayBoosterTime = true;
					}
					this.DelayBoostertime -= Time.deltaTime;
				}
				bool flag25 = !(this.SkinName == this.preSkinName) || (OptionsMaster.skinsEnabled && !this.skinsEnabled);
				if (flag25)
				{
					this.skinsEnabled = OptionsMaster.skinsEnabled;
					this.preSkinName = this.SkinName;
					this.subMeshchecker = false;
					this.subTexchecker = false;
					bool flag26 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey(this.SkinName);
					if (flag26)
					{
						bool flag27 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer[this.SkinName].ProjectileSkinContainer.ContainsKey(this.BlockName);
						if (flag27)
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData = AdCustomModuleMod.mod2.ProjectileSkinPackContainer[this.SkinName].ProjectileSkinContainer[this.BlockName];
							this.projectileSkin = adSkinData;
						}
					}
					else
					{
						Debug.LogWarning("Don't exsit AdProjectileSkin : " + this.SkinName);
						bool flag28 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey("DEFAULT");
						if (flag28)
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData2 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer["DEFAULT"].ProjectileSkinContainer[this.BlockName];
							this.projectileSkin = adSkinData2;
						}
						else
						{
							Debug.LogWarning("Don't set : " + this.SkinName);
						}
					}
				}
				else
				{
					bool flag29 = !OptionsMaster.skinsEnabled && this.skinsEnabled;
					if (flag29)
					{
						this.subMeshchecker = false;
						this.subTexchecker = false;
						this.skinsEnabled = OptionsMaster.skinsEnabled;
						Debug.LogWarning("Don't exsit AdProjectileSkin : " + this.SkinName);
						bool flag30 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer.ContainsKey("DEFAULT");
						if (flag30)
						{
							AdSkinLoader.AdSkinDataPack.AdSkinData adSkinData3 = AdCustomModuleMod.mod2.ProjectileSkinPackContainer["DEFAULT"].ProjectileSkinContainer[this.BlockName];
							this.projectileSkin = adSkinData3;
						}
						else
						{
							Debug.LogWarning("Don't set : " + this.SkinName);
						}
					}
					else
					{
						bool flag31 = this.SkinName == "DEFAULT" || !OptionsMaster.skinsEnabled;
						if (flag31)
						{
							bool flag32 = !this.subMeshchecker && !this.subTexchecker;
							if (flag32)
							{
								GameObject gameObject = base.transform.FindChild("Gyro").FindChild("Vis").gameObject;
								this.projectileSkin.mesh.ApplyToObject(gameObject);
								this.projectileSkin.texture.ApplyToObject(gameObject);
								this.subMeshchecker = true;
								this.subTexchecker = true;
							}
						}
						else
						{
							bool flag33 = this.projectileSkin.mesh.Loaded && !this.subMeshchecker;
							if (flag33)
							{
								this.subMeshchecker = true;
								bool flag34 = !this.projectileSkin.mesh.HasError;
								if (flag34)
								{
									GameObject gameObject2 = base.transform.FindChild("Gyro").FindChild("Vis").gameObject;
									this.projectileSkin.mesh.ApplyToObject(gameObject2);
								}
							}
							bool flag35 = this.projectileSkin.texture.Loaded && !this.subTexchecker;
							if (flag35)
							{
								this.subTexchecker = true;
								bool flag36 = !this.projectileSkin.texture.HasError;
								if (flag36)
								{
									GameObject gameObject3 = base.transform.FindChild("Gyro").FindChild("Vis").gameObject;
									this.projectileSkin.texture.ApplyToObject(gameObject3);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0001889C File Offset: 0x00016A9C
		public virtual void FixedUpdate()
		{
			bool flag = (!StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim) && StatMaster.levelSimulating && (!this.hasAttached || this.deflected);
			if (flag)
			{
				Rigidbody rigidbody = this.myRigidbody;
				bool flag2 = this.noRigidbody || this.myTransform == null;
				if (flag2)
				{
					string str = "Body or transform null in ProjectileScript.FixedUpdate! MyBody: ";
					Rigidbody rigidbody2 = rigidbody;
					string str2 = ((rigidbody2 != null) ? rigidbody2.ToString() : null);
					string str3 = " myTransform: ";
					Transform transform = this.myTransform;
					Debug.LogError(str + str2 + str3 + ((transform != null) ? transform.ToString() : null));
				}
				else
				{
					bool flag3 = !this.enableRotation && !this.useBooster;
					if (flag3)
					{
						rigidbody.AddTorque(Vector3.Cross(this.myTransform.forward, rigidbody.velocity) * this.lookAtTorquePower);
					}
					else
					{
						bool flag4 = !this.useBooster;
						if (flag4)
						{
							rigidbody.AddTorque(this.rotation.dps * Time.fixedDeltaTime * this.myTransform.TransformDirection(this.rotation.rotateAxis) * rigidbody.mass * 60f);
						}
						else
						{
							bool flag5 = this.useBooster && this.DelayBoostertime <= 0f;
							if (flag5)
							{
								bool flag6 = this.useBeacon && this.existenceTime > this.TrailDelayTime;
								if (flag6)
								{
									bool flag7 = this.debugflag;
									if (flag7)
									{
										bool flag8 = base.transform.GetComponent<LineRenderer>() == null;
										if (flag8)
										{
											base.gameObject.AddComponent<LineRenderer>();
										}
										this.LineRcomponent = base.transform.GetComponent<LineRenderer>();
										this.LineRcomponent.enabled = true;
										this.LineRcomponent.SetVertexCount(2);
									}
									bool flag9 = this.TargetId == this.ownerID;
									if (flag9)
									{
										rigidbody.AddTorque(Vector3.Cross(this.projectileForward, this.myTransform.forward) * -this.lookAtTorquePower * 5f);
									}
									else
									{
										bool flag10 = this.debugflag;
										if (flag10)
										{
											this.LineRcomponent.SetPosition(0, AdCustomModuleMod.mod2.BeaconContainer[this.TargetId].Posi);
											this.LineRcomponent.SetPosition(1, this.myTransform.position);
										}
										string guidtype = this.Guidtype;
										string text = guidtype;
										Vector3 vector;
										Vector3 vector2;
										float num;
										float val;
										float num2;
										Vector3 vector3;
										Vector3 vector4;
										if (text != null)
										{
											if (text == "ITANO")
											{
												vector = default(Vector3);
												bool flag11 = this.PrePosi != Vector3.zero;
												if (flag11)
												{
													vector = (AdCustomModuleMod.mod2.BeaconContainer[this.TargetId].Posi - this.PrePosi) * 2f;
												}
												vector2 = AdCustomModuleMod.mod2.BeaconContainer[this.TargetId].Posi + vector - this.myTransform.position;
												this.PrePosi = AdCustomModuleMod.mod2.BeaconContainer[this.TargetId].Posi;
												num = this.Mratio * 500f;
												val = this.Mratio * 150f;
												num2 = this.Mratio * 200f;
												bool flag12 = (double)Random.value > 0.93 && this.superiortime <= 0 && vector2.sqrMagnitude > 4000f && vector2.sqrMagnitude < 10000f;
												if (flag12)
												{
													this.superiortime = 3;
												}
												bool flag13 = this.superiortime > 0;
												if (flag13)
												{
													num = this.Mratio * 1000f;
													val = this.Mratio * 500f;
													this.superiortime--;
												}
												vector3 = Vector3.Cross(vector2.normalized, this.myTransform.forward) * -num;
												bool flag14 = (double)Random.value > 0.97 && this.inferiortime <= 0 && vector2.sqrMagnitude > 4000f;
												if (flag14)
												{
													this.inferiortime = 4;
												}
												bool flag15 = this.inferiortime > 0;
												if (flag15)
												{
													vector3 = Vector3.zero;
													this.inferiortime--;
												}
												for (int i = 0; i < 14; i++)
												{
													this.torqueArray[i] = this.torqueArray[i + 1];
												}
												this.torqueArray[0] = vector3;
												vector4 = default(Vector3);
												for (int j = 0; j < 15; j++)
												{
													vector4 += this.torqueArray[j];
												}
												bool flag16 = (double)Random.value > 0.92 && this.yuragi <= 0;
												if (flag16)
												{
													this.randtorq.x = Random.Range(-num2, num2);
													this.randtorq.y = Random.Range(-num2, num2);
													this.randtorq.z = Random.Range(-num2, num2);
													this.yuragi = 8;
												}
												else
												{
													bool flag17 = this.yuragi > 0;
													if (flag17)
													{
														this.yuragi--;
													}
													else
													{
														this.randtorq = default(Vector3);
													}
												}
												bool flag18 = vector2.sqrMagnitude < 5000f;
												if (flag18)
												{
													vector4 = vector4.normalized * Math.Min(vector4.magnitude, val);
												}
												else
												{
													vector4 = vector4.normalized * Math.Min(vector4.magnitude, val) + this.randtorq;
												}
												bool flag19 = !this.TargetMissing;
												if (flag19)
												{
													rigidbody.AddTorque(vector4);
												}
												rigidbody.velocity -= rigidbody.velocity * 0.0001f * Vector3.Angle(this.myTransform.forward, vector4);
												goto IL_901;
											}
										}
										vector = default(Vector3);
										bool flag20 = this.PrePosi != Vector3.zero;
										if (flag20)
										{
											vector = (AdCustomModuleMod.mod2.BeaconContainer[this.TargetId].Posi - this.PrePosi) * 2f;
										}
										vector2 = AdCustomModuleMod.mod2.BeaconContainer[this.TargetId].Posi + vector - this.myTransform.position;
										num = this.Mratio * 500f;
										val = this.Mratio * 150f;
										num2 = this.Mratio * 100f;
										this.PrePosi = AdCustomModuleMod.mod2.BeaconContainer[this.TargetId].Posi;
										vector3 = Vector3.Cross(vector2.normalized, this.myTransform.forward) * -num;
										for (int k = 0; k < 14; k++)
										{
											this.torqueArray[k] = this.torqueArray[k + 1];
										}
										this.torqueArray[0] = vector3;
										vector4 = default(Vector3);
										for (int l = 0; l < 15; l++)
										{
											vector4 += this.torqueArray[l];
										}
										bool flag21 = (double)Random.value > 0.9 && this.yuragi <= 0;
										if (flag21)
										{
											this.randtorq.x = Random.Range(-num2, num2);
											this.randtorq.y = Random.Range(-num2, num2);
											this.randtorq.z = Random.Range(-num2, num2);
											this.yuragi = 5;
										}
										else
										{
											bool flag22 = this.yuragi > 0;
											if (flag22)
											{
												this.yuragi--;
											}
											else
											{
												this.randtorq = default(Vector3);
											}
										}
										bool flag23 = vector2.sqrMagnitude < 5000f;
										if (flag23)
										{
											vector4 = vector4.normalized * Math.Min(vector4.magnitude, val);
										}
										else
										{
											vector4 = vector4.normalized * Math.Min(vector4.magnitude, val) + this.randtorq;
										}
										bool flag24 = !this.TargetMissing;
										if (flag24)
										{
											rigidbody.AddTorque(vector4);
										}
										rigidbody.velocity -= rigidbody.velocity * 0.0001f * Vector3.Angle(this.myTransform.forward, vector4);
										IL_901:;
									}
									rigidbody.AddForce(this.myTransform.forward * 100f * this.boosterPower);
								}
								else
								{
									rigidbody.AddTorque(Vector3.Cross(this.projectileForward, this.myTransform.forward) * -this.lookAtTorquePower * 5f);
									rigidbody.AddForce(this.myTransform.forward * 100f * this.boosterPower);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00002A37 File Offset: 0x00000C37
		public IEnumerator ProjectileSFX()
		{
			yield return new WaitForFixedUpdate();
			this.SFX.Play(this.sound, 0.5f);
			yield break;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00019234 File Offset: 0x00017434
		public void CreateWingDrag()
		{
			bool flag = this.useBooster && (StatMaster.isHosting || !StatMaster.isMP);
			if (flag)
			{
				Transform transform = new GameObject("Drag").transform;
				this.myTransform = base.transform;
				this.myRigidbody = base.gameObject.GetComponent<Rigidbody>();
				transform.gameObject.AddComponent<Rigidbody>();
				transform.gameObject.AddComponent<FixedJoint>();
				transform.SetParent(this.GetPhysGoal());
				transform.position = this.myTransform.position;
				transform.rotation = this.myTransform.rotation;
				FixedJoint component = transform.GetComponent<FixedJoint>();
				component.connectedBody = this.myRigidbody;
				component.breakForce = float.PositiveInfinity;
				component.breakTorque = float.PositiveInfinity;
				transform.position -= this.projectileForward * 0.5f;
				Rigidbody component2 = transform.GetComponent<Rigidbody>();
				component2.drag = 0.2f;
				component2.angularDrag = 0.05f;
				transform.gameObject.AddComponent<OnJointBrakeBehavour>();
				OnJointBrakeBehavour component3 = transform.GetComponent<OnJointBrakeBehavour>();
				component3.jointTransform = this.myTransform;
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0001936C File Offset: 0x0001756C
		protected bool IgnoreOwnerCollision(Collider other)
		{
			bool flag = other.attachedRigidbody == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				BasicInfo component = other.attachedRigidbody.GetComponent<BasicInfo>();
				bool flag2 = component == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = (int)component.infoType != 1 || !component.HasParentMachine;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.ownerID == (int)component.ParentMachine.PlayerID;
						bool flag5 = (int)(component as BlockBehaviour).Prefab.Type == 61;
						bool flag6 = false;
						bool flag7 = flag4 && flag5 && flag6;
						if (flag7)
						{
							result = true;
						}
						else
						{
							int num = 0;
							bool flag8 = flag4 && num == (component as BlockBehaviour).BlockID;
							result = flag8;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00019440 File Offset: 0x00017640
		public void Explode(Vector3 explosionPos, Quaternion explosionRot)
		{
			int index = AdCustomModuleMod.mod2.ExplosionEffectCountContainer[this.BlockName];
			Transform transform = AdCustomModuleMod.mod2.ExplosionEffectContainer[this.BlockName][index];
			bool flag = transform == null;
			if (flag)
			{
				transform = Object.Instantiate<GameObject>(AdCustomModuleMod.mod2.ExplosionContainer[this.BlockName]).transform;
				transform.name = "ExplosionEffect";
				transform.SetParent(AdCustomModuleMod.mod2.PMEffectPool.transform);
				AdCustomModuleMod.mod2.ExplosionEffectContainer[this.BlockName][index] = transform;
			}
			transform.gameObject.SetActive(true);
			transform.position = explosionPos;
			bool flag2 = this.useExplodeEffectRotation;
			if (flag2)
			{
				transform.rotation = explosionRot;
			}
			else
			{
				transform.rotation = Quaternion.identity;
			}
			bool flag3 = AdCustomModuleMod.mod2.ExplosionEffectCountContainer[this.BlockName] < AdCustomModuleMod.mod2.MAX_Effect_POOL_SIZE;
			if (flag3)
			{
				Dictionary<string, int> explosionEffectCountContainer = AdCustomModuleMod.mod2.ExplosionEffectCountContainer;
				string blockName = this.BlockName;
				int num = explosionEffectCountContainer[blockName];
				explosionEffectCountContainer[blockName] = num + 1;
			}
			else
			{
				AdCustomModuleMod.mod2.ExplosionEffectCountContainer[this.BlockName] = 0;
			}
			bool flag4 = !this.alwaysExplodes;
			if (flag4)
			{
				StatMaster.GodTools.HasBeenUsed = true;
			}
			bool flag5 = StatMaster.isMP && !StatMaster.isLocalSim;
			if (flag5)
			{
				bool isHosting = StatMaster.isHosting;
				if (isHosting)
				{
					bool exExpand = this.ExExpand;
					byte[] array;
					if (exExpand)
					{
						array = new byte[19];
						AdNetworkCompression.CompressPosition(explosionPos, array, 0);
						NetworkCompression.CompressRotation(explosionRot, array, 12);
					}
					else
					{
						array = new byte[13];
						NetworkCompression.CompressPosition(explosionPos, array, 0);
						NetworkCompression.CompressRotation(explosionRot, array, 6);
					}
					AdProjectileManager.Instance.Despawn(base.GetComponent<AdNetworkProjectile>(), array);
				}
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00019640 File Offset: 0x00017840
		public void TrailPurge()
		{
			bool flag = this.useTrailEffect;
			if (flag)
			{
				this.Trailtransform = this.trailPrefab.transform;
				this.Trailtransform.SetParent(this.myTransform.parent);
				this.TrailResetcomp.NetworkTrailstop = true;
				this.Trailtransform.position = this.gyro.position;
				this.Trailtransform.rotation = this.gyro.rotation;
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000196BC File Offset: 0x000178BC
		public void OnCollisionEnter(Collision collision)
		{
			bool flag = this.ValidCollisionOrTrigger(collision.collider);
			if (flag)
			{
				base.StartCoroutine(this.HandleCollisionOrTrigger(collision.collider, collision.contacts[0].normal));
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00019700 File Offset: 0x00017900
		public virtual void OnTriggerEnter(Collider other)
		{
			bool flag = this.ValidCollisionOrTrigger(other);
			if (flag)
			{
				base.StartCoroutine(this.HandleCollisionOrTrigger(other, other.transform.position - base.transform.position));
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00019744 File Offset: 0x00017944
		private bool ValidCollisionOrTrigger(Collider other)
		{
			bool flag = (StatMaster.isMP && !StatMaster.isHosting && !StatMaster.isLocalSim) || other.isTrigger || this.hasAttached || !StatMaster.levelSimulating;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.IgnoreOwnerCollision(other);
				if (flag2)
				{
					result = false;
				}
				else
				{
					AdProjectileScript componentInParent = other.GetComponentInParent<AdProjectileScript>();
					bool flag3 = componentInParent != null && componentInParent.projectileInfo.AdProjectileType == this.projectileInfo.AdProjectileType && this.ownerID == componentInParent.ownerID;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = !this.enableCollision;
						result = !flag4;
					}
				}
			}
			return result;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00002A46 File Offset: 0x00000C46
		public IEnumerator HandleCollisionOrTrigger(Collider other, Vector3 collisionNormal)
		{
			this.hasAttached = true;
			bool flag = this.canAttach;
			if (flag)
			{
				this.attachedRigidbody = other.attachedRigidbody;
			}
			bool parentSet = false;
			bool flag2 = this.attachedRigidbody != null;
			if (flag2)
			{
				this.ForceFromHit(this.attachedRigidbody);
				BasicInfo bInfo = this.attachedRigidbody.GetComponent<BasicInfo>();
				bool flag3 = bInfo is BlockBehaviour;
				if (flag3)
				{
					BlockBehaviour block = bInfo as BlockBehaviour;
					bool flag4 = this.freezing && !this.deflectable;
					if (flag4)
					{
						this.FreezeAttack(block);
					}
					bool flag5 = block.isSimulating && block.Prefab.hasHealthBar;
					if (flag5)
					{
						bool gotChildBlocks = block.gotChildBlocks;
						if (gotChildBlocks)
						{
							BlockBehaviour childBlock = block.GetChildBlockFromCollider(other);
							bool flag6 = childBlock != null;
							if (flag6)
							{
								bool hasHealthBar = childBlock.Prefab.hasHealthBar;
								if (hasHealthBar)
								{
									childBlock.BlockHealth.DamageBlock(this.blockDamageAmount);
								}
								bool flag7 = this.canAttach;
								if (flag7)
								{
									base.transform.SetParent(childBlock.transform, true);
									parentSet = true;
								}
							}
							childBlock = null;
						}
						else
						{
							block.BlockHealth.DamageBlock(this.blockDamageAmount);
						}
					}
					else
					{
						bool flag8 = this.deflectable;
						if (flag8)
						{
							this.Deflect();
							yield break;
						}
					}
					block = null;
				}
				else
				{
					bool flag9 = bInfo is AIGenericEntity;
					if (flag9)
					{
						KillingHandler KH2 = (bInfo as AIGenericEntity).aiEntity.my.killingHandler;
						bool flag10 = KH2 != null && !KH2.my.AiCode.isDead;
						if (flag10)
						{
							bool flag11 = this.deflectable && KH2.damageAmount.projectileDeflection > Random.value;
							if (flag11)
							{
								this.Deflect();
								yield break;
							}
							KH2.TakeDamage(this.attackDamage, this.injuryType);
							bool flag12 = this.canAttach;
							if (flag12)
							{
								this.myRigidbody.interpolation = 0;
								base.transform.SetParent(KH2.my.AiCode.my.VisObject, true);
								parentSet = true;
							}
						}
						KH2 = null;
					}
					else
					{
						bool flag13 = bInfo is EnemyAISimple;
						if (flag13)
						{
							EnemyAISimple simpleAi = bInfo as EnemyAISimple;
							bool flag14 = !simpleAi.isDead;
							if (flag14)
							{
								bool flag15 = this.deflectable && simpleAi.projectileDeflection > Random.value;
								if (flag15)
								{
									this.Deflect();
									yield break;
								}
								simpleAi.TakeDamage(this.attackDamage, InjuryType.Sharp);
								bool flag16 = this.canAttach;
								if (flag16)
								{
									this.myRigidbody.interpolation = 0;
									base.transform.SetParent(simpleAi.visObject, true);
									parentSet = true;
								}
							}
							simpleAi = null;
						}
						else
						{
							bool flag17 = !StatMaster.isMP;
							if (flag17)
							{
								KillingHandler KH3 = this.attachedRigidbody.GetComponent<KillingHandler>();
								bool flag18 = KH3 != null && !KH3.my.AiCode.isDead;
								if (flag18)
								{
									bool flag19 = this.deflectable && KH3.damageAmount.projectileDeflection > Random.value;
									if (flag19)
									{
										this.Deflect();
										yield break;
									}
									KH3.TakeDamage(this.attackDamage, this.injuryType);
									bool flag20 = this.canAttach;
									if (flag20)
									{
										this.myRigidbody.interpolation = 0;
										base.transform.SetParent(KH3.my.AiCode.my.VisObject, true);
										parentSet = true;
									}
								}
								KH3 = null;
							}
						}
					}
				}
				bool flag21 = !parentSet;
				if (flag21)
				{
					int mask = 32;
					IExplosionEffect[] explosionAffected = (IExplosionEffect[])ReferenceMaster.GetInterfaces<IExplosionEffect>(this.attachedRigidbody.gameObject);
					IExplosionEffect[] array = explosionAffected;
					foreach (IExplosionEffect e in array)
					{
						bool activeInHierarchy = this.attachedRigidbody.gameObject.activeInHierarchy;
						if (activeInHierarchy)
						{
							IExplosionEffect explosionEffect = e;
							if (explosionEffect != null)
							{
								explosionEffect.OnExplode(0f, 0f, 0f, Vector3.zero, 0f, mask);
							}
						}
						//e = null;
					}
					IExplosionEffect[] array2 = null;
					bool flag22 = this.canAttach;
					if (flag22)
					{
						this.myRigidbody.interpolation = 0;
						base.transform.parent = this.attachedRigidbody.transform;
					}
					explosionAffected = null;
					array = null;
				}
				this.gyro.localRotation = base.transform.localRotation;
				base.transform.localRotation = Quaternion.identity;
				Transform transform = base.transform;
				Vector3 localScale = base.transform.localScale;
				float x = localScale.x;
				Vector3 lossyScale = base.transform.lossyScale;
				float x2 = x / lossyScale.x;
				Vector3 localScale2 = base.transform.localScale;
				float y = localScale2.y;
				Vector3 lossyScale2 = base.transform.lossyScale;
				float y2 = y / lossyScale2.y;
				Vector3 localScale3 = base.transform.localScale;
				float z = localScale3.z;
				Vector3 lossyScale3 = base.transform.lossyScale;
				transform.localScale = new Vector3(x2, y2, z / lossyScale3.z);
				bInfo = null;
				transform = null;
				localScale = default(Vector3);
				lossyScale = default(Vector3);
				localScale2 = default(Vector3);
				lossyScale2 = default(Vector3);
				localScale3 = default(Vector3);
				lossyScale3 = default(Vector3);
			}
			bool flag23 = this.canAttach;
			if (flag23)
			{
				this.myRigidbody.isKinematic = true;
			}
			bool flag24 = this.useTrailEffect;
			if (flag24)
			{
				this.TrailPurge();
			}
			bool flag25 = this.useDelayBoosterTime;
			if (flag25)
			{
				Object.Destroy(this.bulleteffectPrefab);
			}
			bool flag26 = (this.canExplode && StatMaster.GodTools.ExplodingCannonballs) || this.alwaysExplodes;
			if (flag26)
			{
				bool flag27 = !this.useBooster;
				if (flag27)
				{
					yield return new WaitForSeconds(Random.Range(this.fuseDelay, this.fuseDelay + this.Randomfuse));
				}
				bool flag28 = this.useTrailEffect;
				if (flag28)
				{
					this.Trailtransform.position = base.transform.position;
					this.Trailtransform.rotation = base.transform.rotation;
				}
				this.Explode(base.transform.position, Quaternion.FromToRotation(Vector3.forward, collisionNormal));
			}
			else
			{
				bool flag29 = this.despawnOnCollision;
				if (flag29)
				{
					bool flag30 = StatMaster.isMP && !StatMaster.isLocalSim;
					if (flag30)
					{
						bool isHosting = StatMaster.isHosting;
						if (isHosting)
						{
							AdProjectileManager.Instance.Despawn(base.GetComponent<AdNetworkProjectile>(), null);
						}
					}
					else
					{
						Object.Destroy(base.gameObject);
					}
				}
			}
			bool flag31 = base.gameObject.activeInHierarchy && this.useKillTimer;
			if (flag31)
			{
				base.StartCoroutine(this.DespawnTimer());
			}
			else
			{
				this.DisableProjectile();
			}
			yield break;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00002A63 File Offset: 0x00000C63
		public IEnumerator TimefuseBomb()
		{
			bool flag = this.useTrailEffect;
			if (flag)
			{
				this.TrailPurge();
			}
			bool flag2 = this.useDelayBoosterTime;
			if (flag2)
			{
				Object.Destroy(this.bulleteffectPrefab);
			}
			bool flag3 = (this.canExplode && StatMaster.GodTools.ExplodingCannonballs) || this.alwaysExplodes;
			if (flag3)
			{
				bool flag4 = !this.useBooster;
				if (flag4)
				{
					yield return new WaitForSeconds(Random.Range(0f, 0.05f));
				}
				bool flag5 = this.useTrailEffect;
				if (flag5)
				{
					this.Trailtransform.position = base.transform.position;
					this.Trailtransform.rotation = base.transform.rotation;
				}
				this.Explode(base.transform.position, base.transform.rotation);
			}
			else
			{
				bool flag6 = this.despawnOnCollision;
				if (flag6)
				{
					bool flag7 = StatMaster.isMP && !StatMaster.isLocalSim;
					if (flag7)
					{
						bool isHosting = StatMaster.isHosting;
						if (isHosting)
						{
							AdProjectileManager.Instance.Despawn(base.GetComponent<AdNetworkProjectile>(), null);
						}
					}
					else
					{
						Object.Destroy(base.gameObject);
					}
				}
			}
			bool flag8 = base.gameObject.activeInHierarchy && this.useKillTimer;
			if (flag8)
			{
				base.StartCoroutine(this.DespawnTimer());
			}
			else
			{
				this.DisableProjectile();
			}
			yield break;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x000197F8 File Offset: 0x000179F8
		protected void Deflect()
		{
			Rigidbody rigidbody = this.myRigidbody;
			Vector3 velocity = rigidbody.velocity;
			Vector3 vector = (rigidbody.velocity = (this.worldUp + velocity.normalized * -0.25f) * 10f);
			this.deflected = true;
			this.hasAttached = false;
			bool flag = base.gameObject.activeInHierarchy && this.useKillTimer;
			if (flag)
			{
				base.StartCoroutine(this.DespawnTimer());
			}
			else
			{
				this.DisableProjectile();
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0001988C File Offset: 0x00017A8C
		private void FreezeAttack(BlockBehaviour hitBlock)
		{
			bool gotChildBlocks = hitBlock.gotChildBlocks;
			if (gotChildBlocks)
			{
				hitBlock.CreateSimLists();
				foreach (BlockBehaviour blockBehaviour in hitBlock.parentedColliders.Keys)
				{
					bool canFreeze = blockBehaviour.Prefab.canFreeze;
					if (canFreeze)
					{
						blockBehaviour.iceTag.Freeze();
					}
				}
			}
			bool canFreeze2 = hitBlock.Prefab.canFreeze;
			if (canFreeze2)
			{
				hitBlock.iceTag.Freeze();
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00019934 File Offset: 0x00017B34
		private void DisableProjectile()
		{
			bool flag = this.canAttach && this.disableCollider;
			if (flag)
			{
				Collider[] array = this.cols;
				foreach (Collider collider in array)
				{
					collider.enabled = false;
				}
			}
			this.deflected = false;
			this.ClearFire();
			base.enabled = false;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00002A72 File Offset: 0x00000C72
		private IEnumerator DespawnTimer()
		{
			bool flag = this.canAttach && this.disableCollider;
			if (flag)
			{
				Collider[] array = this.cols;
				foreach (Collider c in array)
				{
					c.enabled = false;
					//c = null;
				}
				Collider[] array2 = null;
				array = null;
			}
			yield return new WaitForSeconds(this.killTimer);
			bool flag2 = StatMaster.isHosting && !StatMaster.isLocalSim;
			if (flag2)
			{
				AdNetworkProjectile netProjectile = base.GetComponent<AdNetworkProjectile>();
				AdProjectileManager.Instance.Despawn(netProjectile);
				netProjectile = null;
			}
			else
			{
				base.gameObject.SetActive(false);
				this.myRigidbody.isKinematic = true;
			}
			this.deflected = false;
			this.ClearFire();
			this.hasAttached = false;
			yield break;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00019998 File Offset: 0x00017B98
		private void ClearFire()
		{
			bool flag = this.firecontrol != null && this.firecontrol.onFire && this.firecontrol.gameObject.activeInHierarchy;
			if (flag)
			{
				this.firecontrol.ImmediateStop();
				this.firecontrol.currentFireDuration = 0f;
				bool hasStartingAtributes = this.firecontrol.hasStartingAtributes;
				if (hasStartingAtributes)
				{
					this.firecontrol.SetBurnedLevel(0f);
				}
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00002A81 File Offset: 0x00000C81
		protected void ForceFromHit(Rigidbody target)
		{
			target.AddForce(this.myRigidbody.velocity * this.impactForceMultiplier);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00019A14 File Offset: 0x00017C14
		private void OnDestroy()
		{
			bool flag = StatMaster.isHosting && !StatMaster.isLocalSim;
			if (flag)
			{
				AdNetworkProjectile component = base.GetComponent<AdNetworkProjectile>();
				AdProjectileManager.Instance.Despawn(component);
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00002AA1 File Offset: 0x00000CA1
		public void SetScale(Vector3 scale)
		{
			this.gyro.localScale = scale;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00015BE4 File Offset: 0x00013DE4
		private Transform GetPhysGoal()
		{
			return ReferenceMaster.physicsGoalInstance;
		}

		// Token: 0x040002DF RID: 735
		public static Action<AdProjectileScript> onUpdateProjectile;

		// Token: 0x040002E0 RID: 736
		public bool debugflag = false;

		// Token: 0x040002E1 RID: 737
		public LineRenderer LineRcomponent;

		// Token: 0x040002E2 RID: 738
		[HideInInspector]
		public float attackDamage = 100f;

		// Token: 0x040002E3 RID: 739
		public AdProjectileScript.Rotation rotation = new AdProjectileScript.Rotation();

		// Token: 0x040002E4 RID: 740
		public bool deflectable = false;

		// Token: 0x040002E5 RID: 741
		public bool freezing;

		// Token: 0x040002E6 RID: 742
		public bool canAttach = true;

		// Token: 0x040002E7 RID: 743
		public bool canExplode;

		// Token: 0x040002E8 RID: 744
		public string BlockName;

		// Token: 0x040002E9 RID: 745
		public string SkinName = "DEFAULT";

		// Token: 0x040002EA RID: 746
		public int skinID;

		// Token: 0x040002EB RID: 747
		public string preSkinName = "DEFAULT";

		// Token: 0x040002EC RID: 748
		public bool skinLoading = false;

		// Token: 0x040002ED RID: 749
		public bool subMeshchecker = true;

		// Token: 0x040002EE RID: 750
		public bool subTexchecker = false;

		// Token: 0x040002EF RID: 751
		private AdSkinLoader.AdSkinDataPack.AdSkinData projectileSkin;

		// Token: 0x040002F0 RID: 752
		public bool skinsEnabled = OptionsMaster.skinsEnabled;

		// Token: 0x040002F1 RID: 753
		public bool alwaysExplodes;

		// Token: 0x040002F2 RID: 754
		public bool useTrailEffect = false;

		// Token: 0x040002F3 RID: 755
		public bool useBulletEffect = false;

		// Token: 0x040002F4 RID: 756
		public bool useBooster = false;

		// Token: 0x040002F5 RID: 757
		public bool useExplodeEffectRotation = false;

		// Token: 0x040002F6 RID: 758
		public bool useProjectileSound = false;

		// Token: 0x040002F7 RID: 759
		public bool useBeacon = false;

		// Token: 0x040002F8 RID: 760
		public bool useFreezer = false;

		// Token: 0x040002F9 RID: 761
		public string Guidtype;

		// Token: 0x040002FA RID: 762
		public bool useDelayBoosterTime = false;

		// Token: 0x040002FB RID: 763
		public bool DelayBoosterTimeEnd = false;

		// Token: 0x040002FC RID: 764
		public float DelayBoostertime = 0f;

		// Token: 0x040002FD RID: 765
		public bool despawnOnCollision;

		// Token: 0x040002FE RID: 766
		public float killTimer = 1f;

		// Token: 0x040002FF RID: 767
		public bool useKillTimer = true;

		// Token: 0x04000300 RID: 768
		public float blockDamageAmount = 1f;

		// Token: 0x04000301 RID: 769
		public InjuryType injuryType = (InjuryType)1;

		// Token: 0x04000302 RID: 770
		public bool triggerProjectile;

		// Token: 0x04000303 RID: 771
		public ProjectileInfo projectileInfo;

		// Token: 0x04000304 RID: 772
		public GameObject explosionPrefab;

		// Token: 0x04000305 RID: 773
		public GameObject trailPrefab;

		// Token: 0x04000306 RID: 774
		public GameObject bulleteffectPrefab;

		// Token: 0x04000307 RID: 775
		public RandomSoundController randomSoundController;

		// Token: 0x04000308 RID: 776
		public FireController firecontrol;

		// Token: 0x04000309 RID: 777
		public float firetime = 2f;

		// Token: 0x0400030A RID: 778
		public float impactForceMultiplier = 10f;

		// Token: 0x0400030B RID: 779
		public Collider col;

		// Token: 0x0400030C RID: 780
		public Collider[] cols;

		// Token: 0x0400030D RID: 781
		public float colliderEnableTime = 0.02f;

		// Token: 0x0400030E RID: 782
		public bool disableCollider = true;

		// Token: 0x0400030F RID: 783
		public bool enableCollision = false;

		// Token: 0x04000310 RID: 784
		public MeshRenderer visObj;

		// Token: 0x04000311 RID: 785
		public bool hasAttached;

		// Token: 0x04000312 RID: 786
		public float lookAtTorquePower = 10f;

		// Token: 0x04000313 RID: 787
		public bool enableRotation;

		// Token: 0x04000314 RID: 788
		public float boosterPower;

		// Token: 0x04000315 RID: 789
		public int ownerID;

		// Token: 0x04000316 RID: 790
		public Vector3 projectilePosition;

		// Token: 0x04000317 RID: 791
		public Vector3 projectileForward;

		// Token: 0x04000318 RID: 792
		public Transform gyro;

		// Token: 0x04000319 RID: 793
		private Rigidbody attachedRigidbody;

		// Token: 0x0400031A RID: 794
		protected Rigidbody myRigidbody;

		// Token: 0x0400031B RID: 795
		protected Transform myTransform;

		// Token: 0x0400031C RID: 796
		public bool noRigidbody;

		// Token: 0x0400031D RID: 797
		private Vector3 worldUp;

		// Token: 0x0400031E RID: 798
		protected bool deflected;

		// Token: 0x0400031F RID: 799
		private float fTime = 2f;

		// Token: 0x04000320 RID: 800
		private float colliderTime;

		// Token: 0x04000321 RID: 801
		public Transform Trailtransform;

		// Token: 0x04000322 RID: 802
		public TrailResset TrailResetcomp;

		// Token: 0x04000323 RID: 803
		private RandomSoundPitch SFX;

		// Token: 0x04000324 RID: 804
		private AudioClip sound = new AudioClip();

		// Token: 0x04000325 RID: 805
		public float existenceTime;

		// Token: 0x04000326 RID: 806
		public float Timefuse = 0f;

		// Token: 0x04000327 RID: 807
		public float fuseDelay = 0f;

		// Token: 0x04000328 RID: 808
		public float Randomfuse = 0.05f;

		// Token: 0x04000329 RID: 809
		private bool bombtriger;

		// Token: 0x0400032A RID: 810
		public float Mratio = 0.5f;

		// Token: 0x0400032B RID: 811
		private Vector3[] torqueArray = new Vector3[20];

		// Token: 0x0400032C RID: 812
		private Vector3 PrePosi = default(Vector3);

		// Token: 0x0400032D RID: 813
		private Vector3 randtorq = default(Vector3);

		// Token: 0x0400032E RID: 814
		private int yuragi = 0;

		// Token: 0x0400032F RID: 815
		private int inferiortime = 0;

		// Token: 0x04000330 RID: 816
		private int superiortime = 0;

		// Token: 0x04000331 RID: 817
		public float TrailDelayTime = 0.2f;

		// Token: 0x04000332 RID: 818
		public int TargetId;

		// Token: 0x04000333 RID: 819
		public int BlockTag = 0;

		// Token: 0x04000334 RID: 820
		public bool TargetMissing = false;

		// Token: 0x04000335 RID: 821
		private List<Transform> ExplosionEffectList;

		// Token: 0x04000336 RID: 822
		private int ExplosionEffectnum = 10;

		// Token: 0x04000337 RID: 823
		private int ExplosionEffectCount = 0;

		// Token: 0x02000049 RID: 73
		[Serializable]
		public class Rotation
		{
			// Token: 0x04000338 RID: 824
			[HideInInspector]
			public float dps;

			// Token: 0x04000339 RID: 825
			public Vector3 rotateAxis = -Vector3.right;

			// Token: 0x0400033A RID: 826
			public float degreesPerSecond = -270f;

			// Token: 0x0400033B RID: 827
			public float randomRotationVariation = 0.1f;
		}

		// Token: 0x0200004A RID: 74
		[Serializable]
		public class Range
		{
			// Token: 0x06000193 RID: 403 RVA: 0x00019C14 File Offset: 0x00017E14
			public void Init()
			{
				this.physG = Physics.gravity.magnitude;
			}

			// Token: 0x0400033C RID: 828
			public Transform projectileSpawnPos;

			// Token: 0x0400033D RID: 829
			public GameObject projectile;

			// Token: 0x0400033E RID: 830
			public NetworkProjectileType networkProjectileType;

			// Token: 0x0400033F RID: 831
			public float shootingForce = 20f;

			// Token: 0x04000340 RID: 832
			public float shootingAngle = 35f;

			// Token: 0x04000341 RID: 833
			public float randomAimAmount = 0.01f;

			// Token: 0x04000342 RID: 834
			public bool prediction;

			// Token: 0x04000343 RID: 835
			public float predictionScalar = 0.5f;

			// Token: 0x04000344 RID: 836
			public float extraProjectiles = 4f;

			// Token: 0x04000345 RID: 837
			[HideInInspector]
			public float physG;

			// Token: 0x04000346 RID: 838
			[HideInInspector]
			public Vector3 projectileScale;

			// Token: 0x04000347 RID: 839
			[HideInInspector]
			public int Amount = 4;
		}

		// Token: 0x0200004B RID: 75
		[Serializable]
		public class Projectile
		{
			// Token: 0x06000195 RID: 405 RVA: 0x00002AE0 File Offset: 0x00000CE0
			public Projectile(GameObject p)
			{
				this.NewProjectile(p);
			}

			// Token: 0x06000196 RID: 406 RVA: 0x00019C8C File Offset: 0x00017E8C
			public void NewProjectile(GameObject p)
			{
				this.gameObject = p;
				this.rigidbody = this.gameObject.GetComponent<Rigidbody>();
				this.projectileScript = this.gameObject.GetComponent<AdProjectileScript>();
				this.transform = this.gameObject.transform;
				this.collider = this.gameObject.GetComponent<Collider>();
				this.gyro = this.transform.FindChild("Gyro").transform;
			}

			// Token: 0x04000348 RID: 840
			public Rigidbody rigidbody;

			// Token: 0x04000349 RID: 841
			public AdProjectileScript projectileScript;

			// Token: 0x0400034A RID: 842
			public Transform transform;

			// Token: 0x0400034B RID: 843
			public GameObject gameObject;

			// Token: 0x0400034C RID: 844
			public Collider collider;

			// Token: 0x0400034D RID: 845
			public Transform gyro;
		}
	}
}
