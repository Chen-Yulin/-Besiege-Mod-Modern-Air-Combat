using System;
using System.Collections.Generic;
using System.Text;
using Modding.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x0200000B RID: 11
	public class UWaterBehaviourScript : AddScriptBase
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002267 File Offset: 0x00000467

		public bool Wenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterEnable;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002273 File Offset: 0x00000473
		public bool WBenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterBouncyEnable;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000227F File Offset: 0x0000047F
		public bool WSenable
		{
			get
			{
				return AdCustomModuleMod.mod4.toggle_WaterSplashEnable;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000228B File Offset: 0x0000048B
		public float WaterHeight
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterHeight;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00005744 File Offset: 0x00003944
		public override void SafeAwake()
		{
			bool flag = !StatMaster.levelSimulating;
			if (flag)
			{
			}
			bool flag2 = base.transform.GetComponent<Rigidbody>();
			if (flag2)
			{
				this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
				this.origin_angularDrag = this.BlockRigidbody.angularDrag;
				this.origin_MaxAngularDrag = this.BlockRigidbody.maxAngularVelocity;
			}
			this.BlockTransform = base.transform;
			bool flag3 = base.transform.GetComponent<FireTag>();
			if (flag3)
			{
				this.FireTagComp = base.transform.GetComponent<FireTag>();
			}
			bool flag4 = UWaterBehaviourScript.block_buoyancy.ContainsKey((BlockType)this.ObjectBehavior.BlockID);
			if (flag4)
			{
				this.floating = UWaterBehaviourScript.block_buoyancy[(BlockType)this.ObjectBehavior.BlockID];
			}
			else
			{
				this.floating = 1f;
			}
			bool flag5 = UWaterBehaviourScript.FlatXBlockList.Contains((BlockType)this.ObjectBehavior.BlockID);
			if (flag5)
			{
				this.isFlatXBlock = true;
			}
			bool flag6 = UWaterBehaviourScript.FlatYBlockList.Contains((BlockType)this.ObjectBehavior.BlockID);
			if (flag6)
			{
				this.isFlatYBlock = true;
			}
			bool flag7 = base.transform.Find("WaterSplashEffect");
			if (flag7)
			{
				this.WaterSplashEffectPrefab = base.transform.Find("WaterSplashEffect").gameObject;
			}
			else
			{
				this.WaterSplashEffectPrefab = Object.Instantiate<GameObject>(AdCustomModuleMod.mod6.BlockWaterSplashEffect);
				this.WaterSplashEffectPrefab.name = "WaterSplashEffect";
			}
			this.WaterSplashEffectPrefab.SetActive(true);
			this.WaterSplashEffectPrefab.transform.SetParent(base.transform);
			this.WaterSplashEffectPrefab.transform.position = base.transform.position;
			this.WaterSplashEffectTransform = this.WaterSplashEffectPrefab.transform;
			this.WaterSplashEffect_PS = this.WaterSplashEffectPrefab.transform.GetComponentInChildren<ParticleSystem>();
			this.WaterSplashEffectArray_PS = this.WaterSplashEffect_PS.transform.GetComponentsInChildren<ParticleSystem>();
			bool flag8 = base.transform.Find("WaterWaveEffect");
			if (flag8)
			{
				this.WaterWaveEffectPrefab = base.transform.Find("WaterWaveEffect").gameObject;
			}
			else
			{
				this.WaterWaveEffectPrefab = Object.Instantiate<GameObject>(AdCustomModuleMod.mod6.BlockWaterWaveEffect);
				this.WaterWaveEffectPrefab.name = "WaterWaveEffect";
			}
			this.WaterWaveEffectPrefab.SetActive(true);
			this.WaterWaveEffectPrefab.transform.SetParent(base.transform);
			this.WaterWaveEffectPrefab.transform.position = base.transform.position;
			this.WaterWaveEffectTransform = this.WaterWaveEffectPrefab.transform;
			this.WaterWaveEffect_PS = this.WaterWaveEffectPrefab.transform.GetComponentInChildren<ParticleSystem>();
			this.WaterWaveEffectArray_PS = this.WaterWaveEffect_PS.transform.GetComponentsInChildren<ParticleSystem>();
			BlockType blockID = (BlockType)this.ObjectBehavior.BlockID;
			BlockType blockType = blockID;
			if ((int)blockType != 3)
			{
				if ((int)blockType != 25)
				{
					if ((int)blockType == 63)
					{
						this.WaterBouyancyPrefab = this.GetWaterBouyancyPrefab();
						this.waterBouyancyobject = new GameObject[2];
						bool flag9 = !base.transform.FindChild("WaterBouncyPrefab_2");
						if (flag9)
						{
							GameObject gameObject = Object.Instantiate<GameObject>(this.WaterBouyancyPrefab);
							gameObject.transform.SetParent(base.transform);
							gameObject.name = "WaterBouncyPrefab_2";
							Transform transform = gameObject.transform;
							transform.position = base.transform.position + base.transform.forward * 0.5f;
							transform.rotation = base.transform.rotation;
							FixedJoint component = transform.GetComponent<FixedJoint>();
							component.connectedBody = this.BlockRigidbody;
							component.breakForce = float.PositiveInfinity;
							component.breakTorque = float.PositiveInfinity;
							WaterBouncyBehaviour component2 = gameObject.GetComponent<WaterBouncyBehaviour>();
							component2.HostRigidbody = this.BlockRigidbody;
							component2.ObjectBehavior = this.ObjectBehavior;
							component2.OwnerID = this.OwnerID;
							transform.gameObject.SetActive(false);
							this.waterBouyancyobject[0] = gameObject;
						}
						else
						{
							this.waterBouyancyobject[0] = base.transform.FindChild("WaterBouncyPrefab_2").gameObject;
						}
						bool flag10 = !base.transform.FindChild("WaterBouncyPrefab_3");
						if (flag10)
						{
							GameObject gameObject2 = Object.Instantiate<GameObject>(this.WaterBouyancyPrefab);
							gameObject2.transform.SetParent(base.transform);
							gameObject2.name = "WaterBouncyPrefab_3";
							Transform transform2 = gameObject2.transform;
							transform2.position = base.transform.position + base.transform.forward * 2.5f;
							transform2.rotation = base.transform.rotation;
							FixedJoint component3 = transform2.GetComponent<FixedJoint>();
							component3.connectedBody = this.BlockRigidbody;
							component3.breakForce = float.PositiveInfinity;
							component3.breakTorque = float.PositiveInfinity;
							WaterBouncyBehaviour component4 = gameObject2.GetComponent<WaterBouncyBehaviour>();
							component4.HostRigidbody = this.BlockRigidbody;
							component4.ObjectBehavior = this.ObjectBehavior;
							component4.OwnerID = this.OwnerID;
							transform2.gameObject.SetActive(false);
							this.waterBouyancyobject[1] = gameObject2;
						}
						else
						{
							this.waterBouyancyobject[1] = base.transform.FindChild("WaterBouncyPrefab_3").gameObject;
						}
						Object.Destroy(this.WaterBouyancyPrefab);
					}
				}
				else
				{
					this.WaterVolumePrefab = this.GetWaterVolumePrefab();
					this.waterVolumeobject = new GameObject[4];
					bool flag11 = !base.transform.FindChild("WaterVolumePrefab_2");
					if (flag11)
					{
						GameObject gameObject3 = Object.Instantiate<GameObject>(this.WaterVolumePrefab);
						gameObject3.transform.SetParent(base.transform);
						gameObject3.name = "WaterVolumePrefab_2";
						Transform transform3 = gameObject3.transform;
						transform3.position = base.transform.position + base.transform.forward * 1.8f + base.transform.right * 0.2f;
						transform3.rotation = base.transform.rotation;
						FixedJoint component5 = transform3.GetComponent<FixedJoint>();
						component5.connectedBody = this.BlockRigidbody;
						component5.breakForce = float.PositiveInfinity;
						component5.breakTorque = float.PositiveInfinity;
						UWaterVolumeBehaviour component6 = gameObject3.GetComponent<UWaterVolumeBehaviour>();
						component6.HostRigidbody = this.BlockRigidbody;
						component6.ObjectBehavior = this.ObjectBehavior;
						component6.OwnerID = this.OwnerID;
						transform3.gameObject.SetActive(false);
						this.waterVolumeobject[0] = gameObject3;
					}
					else
					{
						this.waterVolumeobject[0] = base.transform.FindChild("WaterVolumePrefab_2").gameObject;
					}
					bool flag12 = !base.transform.FindChild("WaterVolumePrefab_3");
					if (flag12)
					{
						GameObject gameObject4 = Object.Instantiate<GameObject>(this.WaterVolumePrefab);
						gameObject4.transform.SetParent(base.transform);
						gameObject4.name = "WaterVolumePrefab_3";
						Transform transform4 = gameObject4.transform;
						transform4.position = base.transform.position + base.transform.forward * 4.8f + base.transform.right * 0.2f;
						transform4.rotation = base.transform.rotation;
						FixedJoint component7 = transform4.GetComponent<FixedJoint>();
						component7.connectedBody = this.BlockRigidbody;
						component7.breakForce = float.PositiveInfinity;
						component7.breakTorque = float.PositiveInfinity;
						UWaterVolumeBehaviour component8 = gameObject4.GetComponent<UWaterVolumeBehaviour>();
						component8.HostRigidbody = this.BlockRigidbody;
						component8.ObjectBehavior = this.ObjectBehavior;
						component8.OwnerID = this.OwnerID;
						transform4.gameObject.SetActive(false);
						this.waterVolumeobject[1] = gameObject4;
					}
					else
					{
						this.waterVolumeobject[1] = base.transform.FindChild("WaterVolumePrefab_3").gameObject;
					}
					bool flag13 = !base.transform.FindChild("WaterVolumePrefab_4");
					if (flag13)
					{
						GameObject gameObject5 = Object.Instantiate<GameObject>(this.WaterVolumePrefab);
						gameObject5.transform.SetParent(base.transform);
						gameObject5.name = "WaterVolumePrefab_4";
						Transform transform5 = gameObject5.transform;
						transform5.position = base.transform.position + base.transform.forward * 1.8f + base.transform.right * 1.2f;
						transform5.rotation = base.transform.rotation;
						FixedJoint component9 = transform5.GetComponent<FixedJoint>();
						component9.connectedBody = this.BlockRigidbody;
						component9.breakForce = float.PositiveInfinity;
						component9.breakTorque = float.PositiveInfinity;
						UWaterVolumeBehaviour component10 = gameObject5.GetComponent<UWaterVolumeBehaviour>();
						component10.HostRigidbody = this.BlockRigidbody;
						component10.ObjectBehavior = this.ObjectBehavior;
						component10.OwnerID = this.OwnerID;
						transform5.gameObject.SetActive(false);
						this.waterVolumeobject[2] = gameObject5;
					}
					else
					{
						this.waterVolumeobject[2] = base.transform.FindChild("WaterVolumePrefab_4").gameObject;
					}
					bool flag14 = !base.transform.FindChild("WaterVolumePrefab_5");
					if (flag14)
					{
						GameObject gameObject6 = Object.Instantiate<GameObject>(this.WaterVolumePrefab);
						gameObject6.transform.SetParent(base.transform);
						gameObject6.name = "WaterVolumePrefab_5";
						Transform transform6 = gameObject6.transform;
						transform6.position = base.transform.position + base.transform.forward * 4.8f + base.transform.right * 1.2f;
						transform6.rotation = base.transform.rotation;
						FixedJoint component11 = transform6.GetComponent<FixedJoint>();
						component11.connectedBody = this.BlockRigidbody;
						component11.breakForce = float.PositiveInfinity;
						component11.breakTorque = float.PositiveInfinity;
						UWaterVolumeBehaviour component12 = gameObject6.GetComponent<UWaterVolumeBehaviour>();
						component12.HostRigidbody = this.BlockRigidbody;
						component12.ObjectBehavior = this.ObjectBehavior;
						component12.OwnerID = this.OwnerID;
						transform6.gameObject.SetActive(false);
						this.waterVolumeobject[3] = gameObject6;
					}
					else
					{
						this.waterVolumeobject[3] = base.transform.FindChild("WaterVolumePrefab_5").gameObject;
					}
					this.WaterBouyancyPrefab = this.GetWaterBouyancyPrefab();
					this.waterBouyancyobject = new GameObject[4];
					bool flag15 = !base.transform.FindChild("WaterBouncyPrefab_2");
					if (flag15)
					{
						GameObject gameObject7 = Object.Instantiate<GameObject>(this.WaterBouyancyPrefab);
						gameObject7.transform.SetParent(base.transform);
						gameObject7.name = "WaterBouncyPrefab_2";
						Transform transform7 = gameObject7.transform;
						transform7.position = base.transform.position + base.transform.forward * 1.8f + base.transform.right * 0.2f;
						transform7.rotation = base.transform.rotation;
						FixedJoint component13 = transform7.GetComponent<FixedJoint>();
						component13.connectedBody = this.BlockRigidbody;
						component13.breakForce = float.PositiveInfinity;
						component13.breakTorque = float.PositiveInfinity;
						WaterBouncyBehaviour component14 = gameObject7.GetComponent<WaterBouncyBehaviour>();
						component14.HostRigidbody = this.BlockRigidbody;
						component14.ObjectBehavior = this.ObjectBehavior;
						component14.OwnerID = this.OwnerID;
						transform7.gameObject.SetActive(false);
						this.waterBouyancyobject[0] = gameObject7;
					}
					else
					{
						this.waterBouyancyobject[0] = base.transform.FindChild("WaterBouncyPrefab_2").gameObject;
					}
					bool flag16 = !base.transform.FindChild("WaterBouncyPrefab_3");
					if (flag16)
					{
						GameObject gameObject8 = Object.Instantiate<GameObject>(this.WaterBouyancyPrefab);
						gameObject8.transform.SetParent(base.transform);
						gameObject8.name = "WaterBouncyPrefab_3";
						Transform transform8 = gameObject8.transform;
						transform8.position = base.transform.position + base.transform.forward * 4.8f + base.transform.right * 0.2f;
						transform8.rotation = base.transform.rotation;
						FixedJoint component15 = transform8.GetComponent<FixedJoint>();
						component15.connectedBody = this.BlockRigidbody;
						component15.breakForce = float.PositiveInfinity;
						component15.breakTorque = float.PositiveInfinity;
						WaterBouncyBehaviour component16 = gameObject8.GetComponent<WaterBouncyBehaviour>();
						component16.HostRigidbody = this.BlockRigidbody;
						component16.ObjectBehavior = this.ObjectBehavior;
						component16.OwnerID = this.OwnerID;
						transform8.gameObject.SetActive(false);
						this.waterBouyancyobject[1] = gameObject8;
					}
					else
					{
						this.waterBouyancyobject[1] = base.transform.FindChild("WaterBouncyPrefab_3").gameObject;
					}
					bool flag17 = !base.transform.FindChild("WaterBouncyPrefab_4");
					if (flag17)
					{
						GameObject gameObject9 = Object.Instantiate<GameObject>(this.WaterBouyancyPrefab);
						gameObject9.transform.SetParent(base.transform);
						gameObject9.name = "WaterBouncyPrefab_4";
						Transform transform9 = gameObject9.transform;
						transform9.position = base.transform.position + base.transform.forward * 1.8f + base.transform.right * 1.2f;
						transform9.rotation = base.transform.rotation;
						FixedJoint component17 = transform9.GetComponent<FixedJoint>();
						component17.connectedBody = this.BlockRigidbody;
						component17.breakForce = float.PositiveInfinity;
						component17.breakTorque = float.PositiveInfinity;
						WaterBouncyBehaviour component18 = gameObject9.GetComponent<WaterBouncyBehaviour>();
						component18.HostRigidbody = this.BlockRigidbody;
						component18.ObjectBehavior = this.ObjectBehavior;
						component18.OwnerID = this.OwnerID;
						transform9.gameObject.SetActive(false);
						this.waterBouyancyobject[2] = gameObject9;
					}
					else
					{
						this.waterBouyancyobject[2] = base.transform.FindChild("WaterBouncyPrefab_4").gameObject;
					}
					bool flag18 = !base.transform.FindChild("WaterBouncyPrefab_5");
					if (flag18)
					{
						GameObject gameObject10 = Object.Instantiate<GameObject>(this.WaterBouyancyPrefab);
						gameObject10.transform.SetParent(base.transform);
						gameObject10.name = "WaterBouncyPrefab_5";
						Transform transform10 = gameObject10.transform;
						transform10.position = base.transform.position + base.transform.forward * 4.8f + base.transform.right * 1.2f;
						transform10.rotation = base.transform.rotation;
						FixedJoint component19 = transform10.GetComponent<FixedJoint>();
						component19.connectedBody = this.BlockRigidbody;
						component19.breakForce = float.PositiveInfinity;
						component19.breakTorque = float.PositiveInfinity;
						WaterBouncyBehaviour component20 = gameObject10.GetComponent<WaterBouncyBehaviour>();
						component20.HostRigidbody = this.BlockRigidbody;
						component20.ObjectBehavior = this.ObjectBehavior;
						component20.OwnerID = this.OwnerID;
						transform10.gameObject.SetActive(false);
						this.waterBouyancyobject[3] = gameObject10;
					}
					else
					{
						this.waterBouyancyobject[3] = base.transform.FindChild("WaterBouncyPrefab_5").gameObject;
					}
					Object.Destroy(this.WaterVolumePrefab);
					Object.Destroy(this.WaterBouyancyPrefab);
				}
			}
			else
			{
				this.WaterVolumePrefab = this.GetWaterVolumePrefab();
				this.waterVolumeobject = new GameObject[2];
				bool flag19 = !base.transform.FindChild("WaterVolumePrefab_2");
				if (flag19)
				{
					GameObject gameObject11 = Object.Instantiate<GameObject>(this.WaterVolumePrefab);
					gameObject11.transform.SetParent(base.transform);
					gameObject11.name = "WaterVolumePrefab_2";
					Transform transform11 = gameObject11.transform;
					transform11.position = base.transform.position + base.transform.forward;
					transform11.rotation = base.transform.rotation;
					FixedJoint component21 = transform11.GetComponent<FixedJoint>();
					component21.connectedBody = this.BlockRigidbody;
					component21.breakForce = float.PositiveInfinity;
					component21.breakTorque = float.PositiveInfinity;
					UWaterVolumeBehaviour component22 = gameObject11.GetComponent<UWaterVolumeBehaviour>();
					component22.HostRigidbody = this.BlockRigidbody;
					component22.ObjectBehavior = this.ObjectBehavior;
					component22.OwnerID = this.OwnerID;
					transform11.gameObject.SetActive(false);
					this.waterVolumeobject[0] = gameObject11;
				}
				else
				{
					this.waterVolumeobject[0] = base.transform.FindChild("WaterVolumePrefab_2").gameObject;
				}
				bool flag20 = !base.transform.FindChild("WaterVolumePrefab_3");
				if (flag20)
				{
					GameObject gameObject12 = Object.Instantiate<GameObject>(this.WaterVolumePrefab);
					gameObject12.transform.SetParent(base.transform);
					gameObject12.name = "WaterVolumePrefab_3";
					Transform transform12 = gameObject12.transform;
					transform12.position = base.transform.position + base.transform.forward * 3f;
					transform12.rotation = base.transform.rotation;
					FixedJoint component23 = transform12.GetComponent<FixedJoint>();
					component23.connectedBody = this.BlockRigidbody;
					component23.breakForce = float.PositiveInfinity;
					component23.breakTorque = float.PositiveInfinity;
					UWaterVolumeBehaviour component24 = gameObject12.GetComponent<UWaterVolumeBehaviour>();
					component24.HostRigidbody = this.BlockRigidbody;
					component24.ObjectBehavior = this.ObjectBehavior;
					component24.OwnerID = this.OwnerID;
					transform12.gameObject.SetActive(false);
					this.waterVolumeobject[1] = gameObject12;
				}
				else
				{
					this.waterVolumeobject[1] = base.transform.FindChild("WaterVolumePrefab_3").gameObject;
				}
				Object.Destroy(this.WaterVolumePrefab);
			}
			bool flag21 = !base.transform.GetComponent<UWaterVolumeBehaviour>();
			if (flag21)
			{
				base.gameObject.AddComponent<UWaterVolumeBehaviour>();
			}
			for (int i = 0; i < 10; i++)
			{
				this.velocityArray[i] = Vector3.zero;
				this.IntegrationTimeArray[i] = 0f;
			}
			BlockType blockID2 = (BlockType)this.ObjectBehavior.BlockID;
			BlockType blockType2 = blockID2;
			if (blockType2 <= (BlockType)9)
			{
				if (blockType2 == (BlockType)7)
				{
					this.EffectActiveFlag = false;
					goto IL_13E5;
				}
				if (blockType2 == (BlockType)9)
				{
					this.EffectActiveFlag = false;
					goto IL_13E5;
				}
			}
			else
			{
				if (blockType2 == (BlockType)14)
				{
					this.velocityObjectTransform = base.transform.FindChild("Rot/Vis");
					this.EffectActiveFlag = true;
					goto IL_13E5;
				}
				if (blockType2 == (BlockType)45)
				{
					this.EffectActiveFlag = false;
					goto IL_13E5;
				}
				if (blockType2 == (BlockType)50)
				{
					this.velocityObjectTransform = base.transform.FindChild("rot/Vis");
					this.EffectActiveFlag = true;
					goto IL_13E5;
				}
			}
			this.velocityObjectTransform = base.transform.FindChild("Vis");
			this.EffectActiveFlag = true;
			IL_13E5:
			bool flag22 = this.ObjectBehavior.myBounds;
			if (flag22)
			{
				this.CenterOfBunds = this.ObjectBehavior.myBounds.localBounds.center;
			}
			bool flag23 = this.ObjectBehavior.Guid.ToString().Contains("00000000");
			if (flag23)
			{
				bool flag24 = base.transform.Find("GuidObject");
				if (flag24)
				{
					this.GuidObject = base.transform.Find("GuidObject").gameObject;
					GameObject gameObject13 = this.GuidObject.transform.GetChild(0).gameObject;
					this.GuidString = gameObject13.name;
					this.GuidObject.SetActive(false);
				}
			}
			else
			{
				this.GuidObject = new GameObject("GuidObject");
				this.GuidObject.transform.SetParent(base.transform);
				GameObject gameObject14 = new GameObject();
				gameObject14.transform.SetParent(this.GuidObject.transform);
				this.GuidString = this.ObjectBehavior.Guid.ToString();
				gameObject14.name = this.GuidString;
			}
			bool flag25 = !AdCustomModuleMod.mod6.BlockObjectContainer.ContainsKey(this.GuidString);
			if (flag25)
			{
				AdCustomModuleMod.mod6.BlockObjectContainer.Add(this.GuidString, base.transform.GetComponent<UWaterBehaviourScript>());
			}
			else
			{
				AdCustomModuleMod.mod6.BlockObjectContainer[this.GuidString] = base.transform.GetComponent<UWaterBehaviourScript>();
			}
			byte[] bytes = Encoding.ASCII.GetBytes(this.GuidString);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00006CE8 File Offset: 0x00004EE8
		public void FixedUpdate()
		{
			bool flag = !this.Init_angular && this.ObjectBehavior.isSimulating;
			if (flag)
			{
				bool flag2 = this.BlockRigidbody;
				if (flag2)
				{
					this.origin_angularDrag = this.BlockRigidbody.angularDrag;
					this.Init_angular = true;
				}
			}
			bool flag3 = !StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim || (StatMaster.isClient && StatMaster.InLocalPlayMode);
			bool flag4 = !StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim || StatMaster.isClient;
			bool flag5 = this.Wenable && this.WBenable && this.EffectActiveFlag && this.ObjectBehavior.isSimulating;
			if (flag5)
			{
				bool flag6 = !this.BlockRigidbody;
				if (flag6)
				{
					bool flag7 = base.transform.GetComponent<Rigidbody>();
					if (flag7)
					{
						this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
					}
				}
				bool simPhysics = this.ObjectBehavior.SimPhysics;
				if (simPhysics)
				{
					this.CenterOfBlock = base.transform.TransformPoint(this.CenterOfBunds);
					float num = this.CenterOfBlock.y - 0.25f;
					for (int i = 0; i < 9; i++)
					{
						this.velocityArray[9 - i] = this.velocityArray[8 - i];
						this.IntegrationTimeArray[9 - i] = this.IntegrationTimeArray[8 - i];
					}
					this.velocityArray[0] = this.velocityObjectTransform.position;
					this.IntegrationTimeArray[0] = Time.fixedDeltaTime;
					bool flag8 = (num < this.WaterHeight + 0.5f) & (num > this.WaterHeight - 1f);
					bool flag9 = flag8 && !this.drafting && this.WSenable;
					if (flag9)
					{
						this.lastUpdate += Time.fixedDeltaTime;
						bool flag10 = this.lastUpdate >= this.updateRate;
						if (flag10)
						{
							while (this.lastUpdate >= this.updateRate)
							{
								this.lastUpdate -= this.updateRate;
							}
							float num2 = 0f;
							float num3 = 0f;
							Vector3 vector = Vector3.zero;
							for (int j = 0; j < 4; j++)
							{
								vector += this.velocityArray[j + 1] - this.velocityArray[j];
								num3 += (this.velocityArray[j + 1] - this.velocityArray[j]).magnitude;
								num2 += this.IntegrationTimeArray[j];
							}
							this.BlockVelocity = num3 / num2 * 60f * 60f / 1000f;
							Vector3 vector2 = -vector / 5f;
							vector2.y = 0f;
							this.BlockVector = vector2.normalized;
							bool flag11 = this.BlockVelocity > 10f;
							if (flag11)
							{
								bool flag12 = !this.EffectOnWaterSurfaceFlag;
								if (flag12)
								{
									bool flag13 = !this.WaterSplashEffectPrefab.activeSelf;
									if (flag13)
									{
										this.WaterSplashEffectPrefab.SetActive(true);
									}
									bool flag14 = !this.WaterWaveEffectPrefab.activeSelf;
									if (flag14)
									{
										this.WaterWaveEffectPrefab.SetActive(true);
									}
									this.EffectOnWaterSurfaceFlag = true;
									this.WaterSplashEffect_PS.Play();
									this.WaterWaveEffect_PS.Play();
								}
							}
							else
							{
								bool effectOnWaterSurfaceFlag = this.EffectOnWaterSurfaceFlag;
								if (effectOnWaterSurfaceFlag)
								{
									this.EffectOnWaterSurfaceFlag = false;
									this.WaterSplashEffect_PS.Stop();
									this.WaterWaveEffect_PS.Stop();
								}
								bool flag15 = !this.WaterSplashEffect_PS.IsAlive();
								if (flag15)
								{
									this.WaterSplashEffectPrefab.SetActive(false);
								}
								bool flag16 = !this.WaterWaveEffect_PS.IsAlive();
								if (flag16)
								{
									this.WaterWaveEffectPrefab.SetActive(false);
								}
							}
							bool flag17 = this.WaterSplashEffectArray_PS.Length != 0;
							if (flag17)
							{
								float num4 = this.BlockVelocity / 2.5f;
								float num5 = this.BlockVelocity / 5f;
								float blockVelocity = this.BlockVelocity;
								float num6 = ((num4 < 30f) ? ((num4 > 5f) ? (-num4) : (-5f)) : (-30f));
								float startSpeed = ((num5 < 25f) ? ((num5 > 2f) ? num5 : 2f) : 25f);
								float num7 = ((blockVelocity < 180f) ? ((blockVelocity > 1f) ? blockVelocity : 1f) : 180f);
								bool flag18 = this.BlockVelocity > 100f;
								if (flag18)
								{
								}
								foreach (ParticleSystem particleSystem in this.WaterSplashEffectArray_PS)
								{
									particleSystem.startSpeed = startSpeed;
									this.em = particleSystem.emission;
									this.em.rate = num7;
									ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[]
									{
										new ParticleSystem.Burst(0f, 3)
									};
									this.em.SetBursts(bursts);
									this.fo = particleSystem.forceOverLifetime;
									this.fo.y = new ParticleSystem.MinMaxCurve(num6);
								}
							}
							bool flag19 = this.WaterWaveEffectArray_PS.Length != 0;
							if (flag19)
							{
								float num8 = this.BlockVelocity / 15f;
								float blockVelocity2 = this.BlockVelocity;
								float startSpeed2 = ((num8 < 6f) ? ((num8 > 1.5f) ? num8 : 1.5f) : 6f);
								float num9 = ((blockVelocity2 < 80f) ? ((blockVelocity2 > 15f) ? blockVelocity2 : 15f) : 80f);
								int num10 = 0;
								foreach (ParticleSystem particleSystem2 in this.WaterWaveEffectArray_PS)
								{
									particleSystem2.startSpeed = startSpeed2;
									this.em = particleSystem2.emission;
									this.em.rate = num9;
									num10++;
								}
							}
						}
						bool transformRight = this.TransformRight;
						if (transformRight)
						{
							bool flag20 = this.WaterSplashEffectArray_PS.Length != 0;
							if (flag20)
							{
								int num11 = 0;
								foreach (ParticleSystem particleSystem3 in this.WaterSplashEffectArray_PS)
								{
									bool flag21 = num11 == 0;
									if (flag21)
									{
										num11++;
									}
									else
									{
										particleSystem3.transform.localPosition = new Vector3(-1f, 0f, 1f);
										particleSystem3.transform.localRotation = Quaternion.Euler(40f, -60f, 0f);
									}
								}
							}
							bool flag22 = this.WaterWaveEffectArray_PS.Length != 0;
							if (flag22)
							{
								int num12 = 0;
								foreach (ParticleSystem particleSystem4 in this.WaterWaveEffectArray_PS)
								{
									bool flag23 = num12 == 0;
									if (flag23)
									{
										num12++;
									}
									else
									{
										particleSystem4.transform.localPosition = new Vector3(-1f, 0f, 1f);
										particleSystem4.transform.localRotation = Quaternion.Euler(90f, -60f, 0f);
									}
								}
							}
							this.TransformRight = false;
						}
						else
						{
							bool flag24 = this.WaterSplashEffectArray_PS.Length != 0;
							if (flag24)
							{
								int num13 = 0;
								foreach (ParticleSystem particleSystem5 in this.WaterSplashEffectArray_PS)
								{
									bool flag25 = num13 == 0;
									if (flag25)
									{
										num13++;
									}
									else
									{
										particleSystem5.transform.localPosition = new Vector3(1f, 0f, 1f);
										particleSystem5.transform.localRotation = Quaternion.Euler(40f, 60f, 0f);
									}
								}
							}
							bool flag26 = this.WaterWaveEffectArray_PS.Length != 0;
							if (flag26)
							{
								int num15 = 0;
								foreach (ParticleSystem particleSystem6 in this.WaterWaveEffectArray_PS)
								{
									bool flag27 = num15 == 0;
									if (flag27)
									{
										num15++;
									}
									else
									{
										particleSystem6.transform.localPosition = new Vector3(1f, 0f, 1f);
										particleSystem6.transform.localRotation = Quaternion.Euler(90f, 60f, 0f);
									}
								}
							}
							this.TransformRight = true;
						}
						Vector3 centerOfBlock = this.CenterOfBlock;
						centerOfBlock.y = this.WaterHeight;
						this.WaterSplashEffectTransform.rotation = Quaternion.Euler(Vector3.zero);
						this.WaterSplashEffectTransform.position = centerOfBlock + this.BlockVector;
						this.WaterWaveEffectTransform.rotation = Quaternion.Euler(Vector3.zero);
						this.WaterWaveEffectTransform.position = centerOfBlock + this.BlockVector;
						bool flag28 = this.BlockVector != Vector3.zero;
						if (flag28)
						{
							this.WaterSplashEffectTransform.forward = this.BlockVector;
							this.WaterWaveEffectTransform.forward = this.BlockVector;
						}
					}
					else
					{
						bool flag29 = this.lastUpdate <= this.updateRate;
						if (flag29)
						{
							this.lastUpdate += Time.fixedDeltaTime / 5f;
						}
						bool effectOnWaterSurfaceFlag2 = this.EffectOnWaterSurfaceFlag;
						if (effectOnWaterSurfaceFlag2)
						{
							this.EffectOnWaterSurfaceFlag = false;
							this.WaterSplashEffect_PS.Stop();
							this.WaterWaveEffect_PS.Stop();
						}
						bool flag30 = !this.WaterSplashEffect_PS.IsAlive();
						if (flag30)
						{
							this.WaterSplashEffectPrefab.SetActive(false);
						}
						bool flag31 = !this.WaterWaveEffect_PS.IsAlive();
						if (flag31)
						{
							this.WaterWaveEffectPrefab.SetActive(false);
						}
					}
				}
				else
				{
					this.CenterOfBlock = base.transform.TransformPoint(this.NetworkBlockBounds);
					float num17 = this.CenterOfBlock.y - 0.25f;
					bool flag32 = (num17 < this.WaterHeight + 0.5f) & (num17 > this.WaterHeight - 1f);
					bool wsenable = this.WSenable;
					if (wsenable)
					{
						bool flag33 = this.NetworkSpeed > 10f;
						if (flag33)
						{
							bool flag34 = !this.EffectOnWaterSurfaceFlag && this.NetworkEffectFlag;
							if (flag34)
							{
								bool flag35 = !this.WaterSplashEffectPrefab.activeSelf;
								if (flag35)
								{
									this.WaterSplashEffectPrefab.SetActive(true);
								}
								bool flag36 = !this.WaterWaveEffectPrefab.activeSelf;
								if (flag36)
								{
									this.WaterWaveEffectPrefab.SetActive(true);
								}
								this.EffectOnWaterSurfaceFlag = true;
								this.WaterSplashEffect_PS.Play();
								this.WaterWaveEffect_PS.Play();
							}
						}
						else
						{
							bool flag37 = this.EffectOnWaterSurfaceFlag && !this.NetworkEffectFlag;
							if (flag37)
							{
								this.EffectOnWaterSurfaceFlag = false;
								this.WaterSplashEffect_PS.Stop();
								this.WaterWaveEffect_PS.Stop();
							}
							bool flag38 = !this.WaterSplashEffect_PS.IsAlive();
							if (flag38)
							{
								this.WaterSplashEffectPrefab.SetActive(false);
							}
							bool flag39 = !this.WaterWaveEffect_PS.IsAlive();
							if (flag39)
							{
								this.WaterWaveEffectPrefab.SetActive(false);
							}
						}
						bool flag40 = this.WaterSplashEffectArray_PS.Length != 0;
						if (flag40)
						{
							float num18 = this.NetworkSpeed / 2.5f;
							float num19 = this.NetworkSpeed / 5f;
							float num20 = this.NetworkSpeed / 2f;
							float num21 = ((num18 < 30f) ? ((num18 > 5f) ? (-num18) : (-5f)) : (-30f));
							float startSpeed3 = ((num19 < 25f) ? ((num19 > 2f) ? num19 : 2f) : 25f);
							float num22 = ((num20 < 60f) ? ((num20 > 1f) ? num20 : 1f) : 60f);
							bool flag41 = this.NetworkSpeed > 100f;
							if (flag41)
							{
							}
							int num23 = 0;
							foreach (ParticleSystem particleSystem7 in this.WaterSplashEffectArray_PS)
							{
								this.WaterSplashEffectArray_PS[num23].startSpeed = startSpeed3;
								this.em = this.WaterSplashEffectArray_PS[num23].emission;
								this.em.rate = num22;
								ParticleSystem.Burst[] bursts2 = new ParticleSystem.Burst[]
								{
									new ParticleSystem.Burst(0f, 2)
								};
								this.em.SetBursts(bursts2);
								this.fo = this.WaterSplashEffectArray_PS[num23].forceOverLifetime;
								this.fo.y = new ParticleSystem.MinMaxCurve(num21);
								num23++;
							}
						}
						bool flag42 = this.WaterWaveEffectArray_PS.Length != 0;
						if (flag42)
						{
							float num25 = this.NetworkSpeed / 15f;
							float num26 = this.NetworkSpeed / 2f;
							float startSpeed4 = ((num25 < 6f) ? ((num25 > 1.5f) ? num25 : 1.5f) : 6f);
							float num27 = ((num26 < 30f) ? ((num26 > 15f) ? num26 : 15f) : 30f);
							int num28 = 0;
							foreach (ParticleSystem particleSystem8 in this.WaterWaveEffectArray_PS)
							{
								this.WaterWaveEffectArray_PS[num28].startSpeed = startSpeed4;
								this.em = this.WaterWaveEffectArray_PS[num28].emission;
								this.em.rate = num27;
								num28++;
							}
						}
						Vector3 centerOfBlock2 = this.CenterOfBlock;
						centerOfBlock2.y = this.WaterHeight;
						this.WaterSplashEffectTransform.rotation = Quaternion.Euler(Vector3.zero);
						this.WaterSplashEffectTransform.position = centerOfBlock2 + this.NetworkVector;
						this.WaterWaveEffectTransform.rotation = Quaternion.Euler(Vector3.zero);
						this.WaterWaveEffectTransform.position = centerOfBlock2 + this.NetworkVector;
						bool flag43 = this.NetworkVector != Vector3.zero;
						if (flag43)
						{
							this.WaterSplashEffectTransform.forward = this.NetworkVector;
							this.WaterWaveEffectTransform.forward = this.NetworkVector;
						}
					}
					else
					{
						bool flag44 = this.EffectOnWaterSurfaceFlag && !this.NetworkEffectFlag;
						if (flag44)
						{
							this.EffectOnWaterSurfaceFlag = false;
							this.WaterSplashEffect_PS.Stop();
							this.WaterWaveEffect_PS.Stop();
						}
						bool flag45 = !this.WaterSplashEffect_PS.IsAlive();
						if (flag45)
						{
							this.WaterSplashEffectPrefab.SetActive(false);
						}
						bool flag46 = !this.WaterWaveEffect_PS.IsAlive();
						if (flag46)
						{
							this.WaterWaveEffectPrefab.SetActive(false);
						}
					}
				}
			}
			else
			{
				bool activeSelf = this.WaterWaveEffectPrefab.activeSelf;
				if (activeSelf)
				{
					this.WaterSplashEffectPrefab.SetActive(false);
					this.WaterWaveEffectPrefab.SetActive(false);
				}
			}
			bool flag47 = this.Wenable && this.WBenable && this.ObjectBehavior.isSimulating && flag3;
			if (flag47)
			{
				bool flag48 = !this.BlockRigidbody;
				if (flag48)
				{
					bool flag49 = base.transform.GetComponent<Rigidbody>();
					if (flag49)
					{
						this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
					}
				}
				bool flag50 = this.BlockRigidbody;
				if (flag50)
				{
					bool flag51 = this.slaveflag;
					if (flag51)
					{
						this.slaveflag = false;
						Object.Destroy(this.WaterDragPrefab);
						Object.Destroy(this.WaterBouncySlavePrefab);
					}
					else
					{
						this.CenterOfMass = this.BlockRigidbody.worldCenterOfMass;
						bool flag52 = !this.UWaterVolumecomponent;
						if (flag52)
						{
							this.UWaterVolumecomponent = base.gameObject.GetComponent<UWaterVolumeBehaviour>();
						}
						this.UWaterVolumecomponent.HostRigidbody = this.BlockRigidbody;
						bool flag53 = this.draft_span <= 0;
						if (flag53)
						{
							Vector3 vector3 = this.CenterOfMass;
							Vector3 normalized = this.BlockRigidbody.velocity.normalized;
							float num30 = 3f;
							bool flag54 = true;
							RaycastHit raycastHit;
							bool flag55 = Physics.Raycast(vector3, normalized, out raycastHit, num30);
							while (flag54)
							{
								bool flag56 = flag55;
								if (flag56)
								{
									bool flag57 = raycastHit.rigidbody;
									if (flag57)
									{
										bool flag58 = raycastHit.rigidbody == this.BlockRigidbody;
										if (flag58)
										{
											vector3 += normalized;
											flag55 = Physics.Raycast(vector3, normalized, out raycastHit, num30);
										}
										else
										{
											flag54 = false;
										}
									}
									else
									{
										flag54 = false;
									}
								}
								else
								{
									flag54 = false;
								}
							}
							bool flag59 = flag55;
							if (flag59)
							{
								bool flag60 = raycastHit.rigidbody;
								if (flag60)
								{
									this.drafting = true;
								}
								else
								{
									this.drafting = false;
								}
							}
							else
							{
								this.drafting = false;
							}
						}
						float num31 = this.CenterOfMass.y - 0.25f;
						float num32 = this.CenterOfMass.y - 0.5f;
						float num33 = this.CenterOfMass.y - 2f;
						float num34 = (this.floating + this.slaveblockfloating) * 50f;
						bool flag61 = this.FireTagComp != null;
						if (flag61)
						{
							bool flag62 = this.FireTagComp.burning && !this.burningDisableFloat;
							if (flag62)
							{
								this.burningDisableFloat = this.FireTagComp.burning;
							}
						}
						bool flag63 = num31 < this.WaterHeight;
						if (flag63)
						{
							float magnitude = this.BlockRigidbody.velocity.magnitude;
							Vector3 vector4 = -this.BlockRigidbody.velocity.normalized;
							bool flag64 = !this.drafting;
							if (flag64)
							{
								vector4 *= magnitude;
								vector4 *= this.drag;
							}
							else
							{
								vector4 *= 0f;
							}
							float num35 = Mathf.Clamp01(this.WaterHeight - num31);
							vector4 *= num35;
							this.BlockRigidbody.velocity += vector4;
							bool flag65 = !this.burningDisableFloat;
							if (flag65)
							{
								this.BlockRigidbody.AddForce(Vector3.up * num34 * num35);
							}
							bool flag66 = this.isFlatXBlock;
							if (flag66)
							{
								Vector3 vector5 = Vector3.Project(this.BlockRigidbody.velocity, base.transform.forward);
								float num36 = vector5.magnitude * 3.5f + vector5.sqrMagnitude * 0.05f;
								this.BlockRigidbody.AddForce(-vector5.normalized * num36);
							}
							bool flag67 = this.isFlatYBlock;
							if (flag67)
							{
								Vector3 vector6 = Vector3.Project(this.BlockRigidbody.velocity, base.transform.up);
								float num37 = vector6.magnitude * 3.5f + vector6.sqrMagnitude * 0.05f;
								this.BlockRigidbody.AddForce(-vector6.normalized * num37);
							}
							bool flag68 = this.FireTagComp != null;
							if (flag68)
							{
								bool flag69 = !this.FireTagComp.hasBeenBurned;
								if (flag69)
								{
									this.FireTagComp.hasBeenBurned = true;
									this.ignaiteOnec = this.FireTagComp.igniteOnce;
									this.FireTagComp.igniteOnce = true;
									this.FireTag = true;
								}
							}
							bool waterBouncyDebug = AdCustomModuleMod.mod4.WaterBouncyDebug;
							if (waterBouncyDebug)
							{
								this.debugline = true;
								bool flag70 = !base.transform.FindChild("lineRender");
								if (flag70)
								{
									GameObject gameObject = new GameObject();
									gameObject.name = "lineRender";
									gameObject.transform.SetParent(base.transform);
									gameObject.transform.position = base.transform.position;
									this.lineRenderer = gameObject.AddComponent<LineRenderer>();
									Material material = new Material(Shader.Find("Unlit/Color"));
									material.SetVector("_Color", Color.red);
									this.lineRenderer.material = material;
								}
								else
								{
									GameObject gameObject2 = base.transform.FindChild("lineRender").gameObject;
									bool flag71 = !gameObject2.GetComponent<LineRenderer>();
									if (flag71)
									{
										this.lineRenderer = gameObject2.AddComponent<LineRenderer>();
										Material material2 = new Material(Shader.Find("Unlit/Color"));
										material2.SetVector("_Color", Color.red);
										this.lineRenderer.material = material2;
									}
								}
								this.lineRenderer.SetVertexCount(2);
								this.lineRenderer.SetWidth(0.2f, 0.2f);
								float num38 = num34 * num35 / 50f;
								Vector3[] positions = new Vector3[]
								{
									this.CenterOfMass - Vector3.up * num38,
									this.CenterOfMass
								};
								this.lineRenderer.SetPositions(positions);
							}
							else
							{
								bool flag72 = this.debugline;
								if (flag72)
								{
									GameObject gameObject3 = base.transform.FindChild("lineRender").gameObject;
									bool flag73 = gameObject3.GetComponent<LineRenderer>();
									if (flag73)
									{
										this.lineRenderer = gameObject3.GetComponent<LineRenderer>();
										Object.Destroy(this.lineRenderer);
									}
									this.debugline = false;
								}
							}
						}
						else
						{
							bool flag74 = this.debugline;
							if (flag74)
							{
								GameObject gameObject4 = base.transform.FindChild("lineRender").gameObject;
								bool flag75 = gameObject4.GetComponent<LineRenderer>();
								if (flag75)
								{
									this.lineRenderer = gameObject4.GetComponent<LineRenderer>();
									Object.Destroy(this.lineRenderer);
								}
								this.debugline = false;
							}
							bool flag76 = this.FireTagComp != null;
							if (flag76)
							{
								bool fireTag = this.FireTag;
								if (fireTag)
								{
									this.FireTagComp.hasBeenBurned = false;
									this.FireTagComp.igniteOnce = this.ignaiteOnec;
									this.FireTag = false;
								}
							}
						}
						bool flag77 = num32 < this.WaterHeight;
						if (flag77)
						{
							float num39 = Mathf.Clamp01(this.WaterHeight - num32);
							this.BlockRigidbody.angularDrag = 3f * num39;
						}
						else
						{
							this.BlockRigidbody.angularDrag = this.origin_angularDrag;
						}
						bool flag78 = num33 < this.WaterHeight;
						if (flag78)
						{
							bool flag79 = !this.waterVolumeEnabel;
							if (flag79)
							{
								this.waterVolumeEnabel = true;
								BlockType blockID = (BlockType)this.ObjectBehavior.BlockID;
								BlockType blockType = blockID;
								if (blockType != (BlockType)3)
								{
									if (blockType != (BlockType)25)
									{
										if ((int)blockType == 63)
										{
											this.waterBouyancyobject[0].SetActive(true);
											this.waterBouyancyobject[1].SetActive(true);
										}
									}
									else
									{
										this.waterVolumeobject[0].SetActive(true);
										this.waterVolumeobject[1].SetActive(true);
										this.waterVolumeobject[2].SetActive(true);
										this.waterVolumeobject[3].SetActive(true);
										this.waterBouyancyobject[0].SetActive(true);
										this.waterBouyancyobject[1].SetActive(true);
										this.waterBouyancyobject[2].SetActive(true);
										this.waterBouyancyobject[3].SetActive(true);
									}
								}
								else
								{
									this.waterVolumeobject[0].SetActive(true);
									this.waterVolumeobject[1].SetActive(true);
								}
							}
						}
						else
						{
							bool flag80 = this.waterVolumeEnabel;
							if (flag80)
							{
								this.waterVolumeEnabel = false;
								BlockType blockID2 = (BlockType)this.ObjectBehavior.BlockID;
								BlockType blockType2 = blockID2;
								if ((int)blockType2 != 3)
								{
									if ((int)blockType2 != 25)
									{
										if ((int)blockType2 == 63)
										{
											this.waterBouyancyobject[0].SetActive(false);
											this.waterBouyancyobject[1].SetActive(false);
										}
									}
									else
									{
										this.waterVolumeobject[0].SetActive(false);
										this.waterVolumeobject[1].SetActive(false);
										this.waterVolumeobject[2].SetActive(false);
										this.waterVolumeobject[3].SetActive(false);
										this.waterBouyancyobject[0].SetActive(false);
										this.waterBouyancyobject[1].SetActive(false);
										this.waterBouyancyobject[2].SetActive(false);
										this.waterBouyancyobject[3].SetActive(false);
									}
								}
								else
								{
									this.waterVolumeobject[0].SetActive(false);
									this.waterVolumeobject[1].SetActive(false);
								}
							}
						}
					}
				}
				else
				{
					bool flag81 = !this.slaveflag;
					if (flag81)
					{
						this.slaveflag = true;
						this.masterObject = base.transform.parent.parent.gameObject;
						Rigidbody component = this.masterObject.GetComponent<Rigidbody>();
						this.WaterDragPrefab = this.GetWaterDragPrefab();
						Transform transform = this.WaterDragPrefab.transform;
						transform.SetParent(base.transform);
						transform.position = base.transform.position;
						transform.rotation = base.transform.rotation;
						FixedJoint component2 = transform.GetComponent<FixedJoint>();
						component2.connectedBody = component;
						component2.breakForce = float.PositiveInfinity;
						component2.breakTorque = float.PositiveInfinity;
						WaterDragBehaviour component3 = transform.GetComponent<WaterDragBehaviour>();
						component3.isFlatXBlock = this.isFlatXBlock;
						component3.isFlatYBlock = this.isFlatYBlock;
						transform.gameObject.SetActive(true);
						this.WaterBouncySlavePrefab = this.GetWaterBouyancyPrefab();
						Vector3 localPosition;
						localPosition = new Vector3(0f, 0f, 0f);
						bool flag82 = UWaterBehaviourScript.block_float_posi.ContainsKey((BlockType)this.ObjectBehavior.BlockID);
						if (flag82)
						{
							localPosition = UWaterBehaviourScript.block_float_posi[(BlockType)this.ObjectBehavior.BlockID];
						}
						Transform transform2 = this.WaterBouncySlavePrefab.transform;
						transform2.SetParent(base.transform);
						transform2.position = base.transform.position;
						transform2.localPosition = localPosition;
						transform2.rotation = base.transform.rotation;
						FixedJoint component4 = transform2.GetComponent<FixedJoint>();
						component4.connectedBody = component;
						component4.breakForce = float.PositiveInfinity;
						component4.breakTorque = float.PositiveInfinity;
						WaterBouncyBehaviour component5 = transform2.GetComponent<WaterBouncyBehaviour>();
						component5.ObjectBehavior = this.ObjectBehavior;
						transform2.gameObject.SetActive(true);
					}
				}
			}
			else
			{
				bool flag83 = this.waterVolumeEnabel;
				if (flag83)
				{
					this.waterVolumeEnabel = false;
					BlockType blockID3 = (BlockType)this.ObjectBehavior.BlockID;
					BlockType blockType3 = blockID3;
					if ((int)blockType3 != 3)
					{
						if ((int)blockType3 != 25)
						{
							if ((int)blockType3 == 63)
							{
								this.waterBouyancyobject[0].SetActive(false);
								this.waterBouyancyobject[1].SetActive(false);
							}
						}
						else
						{
							this.waterVolumeobject[0].SetActive(false);
							this.waterVolumeobject[1].SetActive(false);
							this.waterVolumeobject[2].SetActive(false);
							this.waterVolumeobject[3].SetActive(false);
							this.waterBouyancyobject[0].SetActive(false);
							this.waterBouyancyobject[1].SetActive(false);
							this.waterBouyancyobject[2].SetActive(false);
							this.waterBouyancyobject[3].SetActive(false);
						}
					}
					else
					{
						this.waterVolumeobject[0].SetActive(false);
						this.waterVolumeobject[1].SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00008908 File Offset: 0x00006B08
		private GameObject GetWaterDragPrefab()
		{
			GameObject gameObject = new GameObject("WaterDragPrefab");
			gameObject.SetActive(false);
			Transform transform = gameObject.transform;
			transform.gameObject.AddComponent<Rigidbody>();
			Rigidbody component = transform.GetComponent<Rigidbody>();
			component.drag = 0f;
			component.angularDrag = 0f;
			component.maxAngularVelocity = 100f;
			component.mass = 0f;
			transform.gameObject.AddComponent<FixedJoint>();
			transform.gameObject.AddComponent<WaterDragBehaviour>();
			return gameObject;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00008990 File Offset: 0x00006B90
		private GameObject GetWaterVolumePrefab()
		{
			GameObject gameObject = new GameObject("WaterVolumePrefab");
			gameObject.SetActive(false);
			Transform transform = gameObject.transform;
			transform.gameObject.AddComponent<Rigidbody>();
			Rigidbody component = transform.GetComponent<Rigidbody>();
			component.drag = 0f;
			component.angularDrag = 0f;
			component.maxAngularVelocity = 100f;
			component.mass = 0f;
			transform.gameObject.AddComponent<FixedJoint>();
			transform.gameObject.AddComponent<UWaterVolumeBehaviour>();
			return gameObject;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00008A18 File Offset: 0x00006C18
		private GameObject GetWaterBouyancyPrefab()
		{
			GameObject gameObject = new GameObject("WaterBouncyPrefab");
			gameObject.SetActive(false);
			Transform transform = gameObject.transform;
			transform.gameObject.AddComponent<Rigidbody>();
			Rigidbody component = transform.GetComponent<Rigidbody>();
			component.drag = 0f;
			component.angularDrag = 0f;
			component.maxAngularVelocity = 100f;
			component.mass = 0f;
			transform.gameObject.AddComponent<FixedJoint>();
			transform.gameObject.AddComponent<WaterBouncyBehaviour>();
			return gameObject;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00008BCC File Offset: 0x00006DCC
		// Note: this type is marked as 'beforefieldinit'.
		static UWaterBehaviourScript()
		{
			Dictionary<BlockType, float> dictionary = new Dictionary<BlockType, float>();
			dictionary[(BlockType)43] = 6f;
			dictionary[(BlockType)63] = 1.75f;
			dictionary[(BlockType)1] = 1.75f;
			dictionary[(BlockType)15] = 1.5f;
			dictionary[(BlockType)2] = 1.5f;
			dictionary[(BlockType)40] = 1.5f;
			dictionary[(BlockType)46] = 1.5f;
			dictionary[(BlockType)60] = 1.5f;
			dictionary[(BlockType)39] = 1.25f;
			dictionary[(BlockType)38] = 1.25f;
			dictionary[(BlockType)51] = 1.25f;
			dictionary[(BlockType)10] = 1f;
			dictionary[(BlockType)41] = 1f;
			dictionary[(BlockType)26] = 1.25f;
			dictionary[(BlockType)55] = 1f;
			dictionary[(BlockType)25] = 0.3f;
			dictionary[(BlockType)34] = 1f;
			dictionary[(BlockType)14] = 0f;
			dictionary[(BlockType)13] = 0f;
			dictionary[(BlockType)28] = 0f;
			dictionary[(BlockType)19] = 0f;
			dictionary[(BlockType)5] = 0f;
			dictionary[(BlockType)22] = 0f;
			dictionary[(BlockType)16] = 0f;
			dictionary[(BlockType)18] = 0f;
			dictionary[(BlockType)44] = 0f;
			dictionary[(BlockType)18] = 0f;
			dictionary[(BlockType)4] = 0f;
			dictionary[(BlockType)27] = 0f;
			dictionary[(BlockType)9] = 0f;
			dictionary[(BlockType)20] = 0f;
			dictionary[(BlockType)3] = 0f;
			dictionary[(BlockType)17] = 0f;
			dictionary[(BlockType)48] = 0f;
			dictionary[(BlockType)11] = 0f;
			dictionary[(BlockType)53] = 0f;
			dictionary[(BlockType)62] = 0f;
			dictionary[(BlockType)56] = 0f;
			dictionary[(BlockType)47] = 0f;
			dictionary[(BlockType)23] = 0f;
			dictionary[(BlockType)54] = 0f;
			dictionary[(BlockType)31] = 0f;
			dictionary[(BlockType)36] = 0f;
			dictionary[(BlockType)35] = 0f;
			dictionary[(BlockType)32] = 0f;
			dictionary[(BlockType)24] = 0f;
			dictionary[(BlockType)29] = 0f;
			dictionary[(BlockType)33] = 0f;
			dictionary[(BlockType)37] = 0f;
			dictionary[(BlockType)30] = 0f;
			dictionary[(BlockType)6] = 0f;
			dictionary[(BlockType)65] = 0f;
			dictionary[(BlockType)66] = 0f;
			dictionary[(BlockType)67] = 0f;
			dictionary[(BlockType)68] = 0f;
			dictionary[(BlockType)69] = 0f;
			dictionary[(BlockType)70] = 0f;
			dictionary[(BlockType)7] = 0f;
			UWaterBehaviourScript.block_buoyancy = dictionary;
			Dictionary<BlockType, float> dictionary2 = new Dictionary<BlockType, float>();
			dictionary2[(BlockType)41] = 0.008f;
			dictionary2[(BlockType)59] = 0.008f;
			dictionary2[(BlockType)56] = 0.008f;
			dictionary2[(BlockType)11] = 0.008f;
			dictionary2[(BlockType)53] = 0.008f;
			dictionary2[(BlockType)43] = 0.005f;
			dictionary2[(BlockType)23] = 0.005f;
			dictionary2[(BlockType)31] = 0.005f;
			dictionary2[(BlockType)54] = 0.005f;
			dictionary2[(BlockType)20] = 0.005f;
			dictionary2[(BlockType)36] = 0.005f;
			dictionary2[(BlockType)47] = 0.005f;
			dictionary2[(BlockType)7] = 0f;
			UWaterBehaviourScript.block_drag = dictionary2;
			Dictionary<BlockType, float> dictionary3 = new Dictionary<BlockType, float>();
			dictionary3[(BlockType)3] = 30f;
			dictionary3[(BlockType)25] = 30f;
			dictionary3[(BlockType)34] = 30f;
			UWaterBehaviourScript.block_float = dictionary3;
			Dictionary<BlockType, Vector3> dictionary4 = new Dictionary<BlockType, Vector3>();
			dictionary4[(BlockType)10] = new Vector3(0f, -0.7f, 0.1f);
			UWaterBehaviourScript.block_float_posi = dictionary4;
			UWaterBehaviourScript.FlatXBlockList = new List<BlockType> { (BlockType)10, (BlockType)32, (BlockType)24, (BlockType)29 };
			UWaterBehaviourScript.FlatYBlockList = new List<BlockType> { (BlockType)3, (BlockType)25, (BlockType)34 };
			UWaterBehaviourScript.FlatYFloatList = new List<BlockType> { (BlockType)3, (BlockType)25, (BlockType)34 };
		}

		// Token: 0x0400006C RID: 108
		private static readonly Dictionary<BlockType, float> block_buoyancy;

		// Token: 0x0400006D RID: 109
		private static readonly Dictionary<BlockType, float> block_drag;

		// Token: 0x0400006E RID: 110
		private static readonly Dictionary<BlockType, float> block_float;

		// Token: 0x0400006F RID: 111
		private static readonly Dictionary<BlockType, Vector3> block_float_posi;

		// Token: 0x04000070 RID: 112
		private static readonly List<BlockType> FlatXBlockList;

		// Token: 0x04000071 RID: 113
		private static readonly List<BlockType> FlatYBlockList;

		// Token: 0x04000072 RID: 114
		private static readonly List<BlockType> FlatYFloatList;

		// Token: 0x04000073 RID: 115
		private Collider[] BlockColliders;

		// Token: 0x04000074 RID: 116
		private List<Collider> Colliderlist = new List<Collider>();

		// Token: 0x04000075 RID: 117
		private Renderer[] BlockRenderer;

		// Token: 0x04000076 RID: 118
		private Rigidbody BlockRigidbody;

		// Token: 0x04000077 RID: 119
		private Transform BlockTransform;

		// Token: 0x04000078 RID: 120
		private GameObject WaterDragPrefab;

		// Token: 0x04000079 RID: 121
		private GameObject WaterVolumePrefab;

		// Token: 0x0400007A RID: 122
		private GameObject WaterBouyancyPrefab;

		// Token: 0x0400007B RID: 123
		private GameObject WaterBouncySlavePrefab;

		// Token: 0x0400007C RID: 124
		private LineRenderer lineRenderer;

		// Token: 0x0400007D RID: 125
		private GameObject WaterSplashEffectPrefab;

		// Token: 0x0400007E RID: 126
		private Transform WaterSplashEffectTransform;

		// Token: 0x0400007F RID: 127
		private GameObject WaterWaveEffectPrefab;

		// Token: 0x04000080 RID: 128
		private Transform WaterWaveEffectTransform;

		// Token: 0x04000081 RID: 129
		private ParticleSystem WaterSplashEffect_PS;

		// Token: 0x04000082 RID: 130
		private ParticleSystem[] WaterSplashEffectArray_PS;

		// Token: 0x04000083 RID: 131
		private ParticleSystem WaterWaveEffect_PS;

		// Token: 0x04000084 RID: 132
		private ParticleSystem[] WaterWaveEffectArray_PS;

		// Token: 0x04000085 RID: 133
		private ParticleSystem.EmissionModule em;

		// Token: 0x04000086 RID: 134
		private ParticleSystem.ForceOverLifetimeModule fo;

		// Token: 0x04000087 RID: 135
		private Transform velocityObjectTransform;

		// Token: 0x04000088 RID: 136
		private bool EffectOnWaterSurfaceFlag = false;

		// Token: 0x04000089 RID: 137
		private bool EffectActiveFlag = false;

		// Token: 0x0400008A RID: 138
		private GameObject GuidObject;

		// Token: 0x0400008B RID: 139
		private string GuidString;

		// Token: 0x0400008C RID: 140
		private Vector3 CenterOfMass;

		// Token: 0x0400008D RID: 141
		private Vector3 CenterOfBlock;

		// Token: 0x0400008E RID: 142
		private Vector3 CenterOfBunds;

		// Token: 0x0400008F RID: 143
		private GameObject masterObject;

		// Token: 0x04000090 RID: 144
		private int draft_span = 0;

		// Token: 0x04000091 RID: 145
		private bool drafting = false;

		// Token: 0x04000092 RID: 146
		private int volume_span = 0;

		// Token: 0x04000093 RID: 147
		private bool[] hasvolume = new bool[4];

		// Token: 0x04000094 RID: 148
		private int hasvolumess = 0;

		// Token: 0x04000095 RID: 149
		private float Maxfloating = 0f;

		// Token: 0x04000096 RID: 150
		private int anglecount = 0;

		// Token: 0x04000097 RID: 151
		private float origin_angularDrag;

		// Token: 0x04000098 RID: 152
		private bool Init_angular = false;

		// Token: 0x04000099 RID: 153
		private float origin_MaxAngularDrag;

		// Token: 0x0400009A RID: 154
		private bool isFlatXBlock = false;

		// Token: 0x0400009B RID: 155
		private bool isFlatYBlock = false;

		// Token: 0x0400009C RID: 156
		private bool debugline = false;

		// Token: 0x0400009D RID: 157
		private Player PlayerData;

		// Token: 0x0400009E RID: 158
		private bool burningDisableFloat = false;

		// Token: 0x0400009F RID: 159
		private bool FireTag = false;

		// Token: 0x040000A0 RID: 160
		private bool ignaiteOnec;

		// Token: 0x040000A1 RID: 161
		private FireTag FireTagComp;

		// Token: 0x040000A2 RID: 162
		private Vector3[] velocityArray = new Vector3[10];

		// Token: 0x040000A3 RID: 163
		private float[] IntegrationTimeArray = new float[10];

		// Token: 0x040000A4 RID: 164
		private float BlockVelocity = 0f;

		// Token: 0x040000A5 RID: 165
		private Vector3 BlockVector = Vector3.zero;

		// Token: 0x040000A6 RID: 166
		private float lastUpdate;

		// Token: 0x040000A7 RID: 167
		private bool Init = false;

		// Token: 0x040000A8 RID: 168
		private float updateRate = 0.1f;

		// Token: 0x040000A9 RID: 169
		private bool TransformRight = true;

		// Token: 0x040000AA RID: 170
		public UWaterVolumeBehaviour UWaterVolumecomponent;

		// Token: 0x040000AB RID: 171
		public float slaveblockfloating = 0f;

		// Token: 0x040000AC RID: 172
		public bool slaveflag = false;

		// Token: 0x040000AD RID: 173
		public float drag = 0.08f;

		// Token: 0x040000AE RID: 174
		public float floating = 1.5f;

		// Token: 0x040000AF RID: 175
		private GameObject[] waterVolumeobject;

		// Token: 0x040000B0 RID: 176
		private GameObject[] waterBouyancyobject;

		// Token: 0x040000B1 RID: 177
		private bool waterVolumeEnabel = false;

		// Token: 0x040000B2 RID: 178
		public bool NetworkEffectFlag;

		// Token: 0x040000B3 RID: 179
		public Vector3 NetworkVector = Vector3.zero;

		// Token: 0x040000B4 RID: 180
		public Vector3 NetworkBlockBounds = Vector3.zero;

		// Token: 0x040000B5 RID: 181
		public float NetworkSpeed = 0f;
	}
}
