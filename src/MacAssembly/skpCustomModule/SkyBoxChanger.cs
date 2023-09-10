using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x0200006F RID: 111
	public class SkyBoxChanger : SingleInstance<SkyBoxChanger>
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600029A RID: 666 RVA: 0x00003080 File Offset: 0x00001280
		public override string Name
		{
			get
			{
				return "SkyBoxChanger";
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600029B RID: 667 RVA: 0x00003087 File Offset: 0x00001287
		// (set) Token: 0x0600029C RID: 668 RVA: 0x0000308F File Offset: 0x0000128F
		public string Language { get; set; }

		// Token: 0x0600029D RID: 669 RVA: 0x00024310 File Offset: 0x00022510
		private void Awake()
		{
			this.Init = false;
			BesiegeConfig besiegeConfig = new BesiegeConfig();
			besiegeConfig.Load(Path.Combine(Application.dataPath, "Config.xml"), 0);
			this.Language = besiegeConfig.Language;
			this.AdSaveData = base.gameObject.AddComponent<AdDataHolder>();
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00024360 File Offset: 0x00022560
		private void Update()
		{
			if (StatMaster.inMenu && StatMaster.isMP && !StatMaster.isClient && !StatMaster.isHosting)
			{
				if (Input.GetKey((KeyCode)306) && Input.GetKeyDown((KeyCode)117))
				{
					this.Keyhide2 = !this.Keyhide2;
				}
				if (int.Parse(this.maxPlayersValue[0]) <= 10)
				{
					this.maxPlayersValue[1] = "16";
				}
				else
				{
					this.maxPlayersValue[1] = "20";
				}
				OptionsMaster.maxPlayersPerHost = int.Parse(this.maxPlayersValue[1]);
				OptionsMaster.maxPlayers = int.Parse(this.maxPlayersValue[0]);
			}
			if (Input.GetKeyDown((KeyCode)9))
			{
				this.hide = !this.hide;
			}
			if (Input.GetKey((KeyCode)306) && Input.GetKeyDown((KeyCode)117))
			{
				this.Keyhide = !this.Keyhide;
			}
			if (StatMaster.isMainMenu)
			{
				if (this.Init)
				{
					this.Init = false;
					this.skyChecker = false;
					this.waterChecker = false;
					this.markerOptionChecker = false;
					this.waterSplashchecker = false;
					AdCustomModuleMod.skyboxcheckerCallBack = false;
				}
			}
			else
			{
				if (!this.Init)
				{
					this.Init = true;
					RuntimePlatform platform = Application.platform;

				}
				if (this.LightObject == null)
				{
					if (StatMaster.isMP)
					{
						this.LightObject = GameObject.Find("MULTIPLAYER LEVEL/Environments/Directional light").gameObject;
					}
					else if (!StatMaster.isClient)
					{
						if (GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere/Directional light") != null)
						{
							Debug.Log("ACM:LEVEL BARREN EXPANSE");
							this.LightObject = GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere/Directional light").gameObject;
						}
						else if (GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere/Directional light") != null)
						{
							Debug.Log("ACM:LEVEL SANDBOX");
							this.LightObject = GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere/Directional light").gameObject;
						}
						else if (GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE/Directional light") != null)
						{
							Debug.Log("ACM:LEVEL MISTY MOUNTAIN");
							this.LightObject = GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE/Directional light").gameObject;
						}
					}
					for (int j = 0; j < 5; j++)
					{
						this.FogValue_temp[j] = this.FogValue[j];
					}
				}
				if (StatMaster.isMP && this.EnvironmentsObject == null)
				{
					this.EnvironmentsObject = GameObject.Find("MULTIPLAYER LEVEL/Environments/");
					this.BarrenObject = this.EnvironmentsObject.transform.Find("Barren").gameObject;
					this.IpsilonObject = this.EnvironmentsObject.transform.Find("Ipsilon").gameObject;
					this.TolbryndObject = this.EnvironmentsObject.transform.Find("Tolbrynd").gameObject;
					this.MountainObject = this.EnvironmentsObject.transform.Find("MountainTop").gameObject;
					this.BarrenEnvObject = this.EnvironmentsObject.transform.Find("Barren/BarrenEnv").gameObject;
					this.DesertObject = this.EnvironmentsObject.transform.Find("Desert").gameObject;
				}
				if (StatMaster.isClient && StatMaster.isMP && !StatMaster.inMenu)
				{
					if (!this.skyChecker)
					{
						base.StartCoroutine(this.SkyChecker());
						this.skyChecker = true;
					}
					if (!this.waterChecker)
					{
						base.StartCoroutine(this.WaterChecker());
						this.waterChecker = true;
					}
					if (!this.markerOptionChecker)
					{
						base.StartCoroutine(this.MarkerOptionChecker());
						this.markerOptionChecker = true;
					}
					if (!this.waterSplashchecker)
					{
						base.StartCoroutine(this.WaterSplashchecker());
						this.waterSplashchecker = true;
					}
				}
			}
			if (this.changeflag)
			{
				if (!StatMaster.isMP)
				{
					if (GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE") != null)
					{
						this.TerainChecker = SkyBoxChanger.TerainType.MountainTop;
					}
					else
					{
						this.TerainChecker = SkyBoxChanger.TerainType.Barren;
					}
				}
				else if (this.IpsilonObject.activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.ipsilon;
				}
				else if (this.TolbryndObject.activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.Tolbrynd;
				}
				else if (this.MountainObject.activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.MountainTop;
				}
				else if (this.DesertObject.activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.Desert;
				}
				else if (this.BarrenObject.activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.Barren;
				}
				else
				{
					this.TerainChecker = SkyBoxChanger.TerainType.Barren;
				}
				if (this.changeflag)
				{
					for (int k = 0; k < 5; k++)
					{
						if (this.TerainChecker == SkyBoxChanger.TerainType.MountainTop)
						{
							this.FogValue[k] = Convert.ToString(this.SkySetting02Container[this.selector][k]);
							this.FogValue_temp[k] = this.FogValue[k];
						}
						else
						{
							this.FogValue[k] = Convert.ToString(this.SkySettingContainer[this.selector][k]);
							this.FogValue_temp[k] = this.FogValue[k];
						}
					}
					for (int l = 0; l < 3; l++)
					{
						this.SkyColorValue[l] = Convert.ToString(this.SkySetting03Container[this.selector][l]);
						this.SkyColorValue_temp[l] = this.SkyColorValue[l];
					}
					this.changeflag = false;
				}
				if (AdCustomModuleMod.skyboxchangeFlag)
				{
					this.FloorDeactiveSwitch = AdCustomModuleMod.FloorDeactive;
					this.ExpandFloorSwitch = AdCustomModuleMod.ExpandFloor;
					this.ExExpandFloorSwitch = AdCustomModuleMod.ExExpandFloor;
					this.scale = AdCustomModuleMod.scale;
					for (int m = 0; m < 5; m++)
					{
						this.selector = AdCustomModuleMod.skyboxIndex;
						this.FogValue[m] = Convert.ToString(AdCustomModuleMod.skyboxinfo[m]);
						this.FogValue_temp[m] = this.FogValue[m];
					}
					for (int n = 0; n < 3; n++)
					{
						this.SkyColorValue[n] = Convert.ToString(AdCustomModuleMod.skyboxinfo2[n]);
						this.SkyColorValue_temp[n] = this.SkyColorValue[n];
					}
					this.networkflag = true;
					AdCustomModuleMod.skyboxchangeFlag = false;
				}
			}
			if (AdCustomModuleMod.waterchangeFlag)
			{
				this.WaterEnable = AdCustomModuleMod.waterEnable;
				this.WaterBouncyEnable = AdCustomModuleMod.waterFloatingEnable;
				this.WaterBouncyEnableSwitch = AdCustomModuleMod.waterFloatingEnable;
				for (int num = 0; num < 14; num++)
				{
					this.WaterValue[num] = Convert.ToString(AdCustomModuleMod.waterinfo1[num]);
					this.WaterValue_temp[num] = this.WaterValue[num];
				}
				for (int num2 = 0; num2 < 3; num2++)
				{
					this.WaterFogValue[num2] = Convert.ToString(AdCustomModuleMod.waterinfo2[num2]);
					this.WaterFogValue_temp[num2] = this.WaterFogValue[num2];
				}
				for (int num3 = 0; num3 < 4; num3++)
				{
					this.WaterRefrColor[num3] = Convert.ToString(AdCustomModuleMod.waterinfo3[num3]);
					this.WaterRefrColor_temp[num3] = this.WaterRefrColor[num3];
				}
				for (int num4 = 0; num4 < 4; num4++)
				{
					this.WaterFadeColor[num4] = Convert.ToString(AdCustomModuleMod.waterinfo3[num4 + 4]);
					this.WaterFadeColor_temp[num4] = this.WaterFadeColor[num4];
				}
				for (int num5 = 0; num5 < 4; num5++)
				{
					this.WaterFogColor[num5] = Convert.ToString(AdCustomModuleMod.waterinfo3[num5 + 8]);
					this.WaterFogColor_temp[num5] = this.WaterFogColor[num5];
				}
				for (int num6 = 0; num6 < 4; num6++)
				{
					this.InWaterFarColor[num6] = Convert.ToString(AdCustomModuleMod.waterinfo3[num6 + 12]);
					this.InWaterFarColor_temp[num6] = this.InWaterFarColor[num6];
				}
				for (int num7 = 0; num7 < 4; num7++)
				{
					this.InWaterRefColor[num7] = Convert.ToString(AdCustomModuleMod.waterinfo3[num7 + 16]);
					this.InWaterRefColor_temp[num7] = this.InWaterRefColor[num7];
				}
				if (this.WaterEnable)
				{
					this.networkwaterEnable = true;
				}
				else
				{
					this.networkwaterDisable = true;
				}
				this.waterCallback = true;
				AdCustomModuleMod.waterchangeFlag = false;
			}
			if (AdCustomModuleMod.markerOptionchangeFlag)
			{
				this.markerCallback = true;
				this.toggle_TargetMarkerDisable = AdCustomModuleMod.markerOptionEnable;
			}
			if (AdCustomModuleMod.waterSplashchangeFlag)
			{
				this.waterSplashCallback = true;
				this.toggle_WaterSplashEnable = AdCustomModuleMod.waterSplashEnable;
			}
			if (AdCustomModuleMod.skyboxchecker)
			{
				if (!this.barrenChecker)
				{
					this.networkflag = true;
				}
				ModNetworking.SendToAll(AdCustomModuleMod.msgSkyBoxCallBackNone.CreateMessage(new object[] { true }));
				AdCustomModuleMod.skyboxchecker = false;
			}
			if (AdCustomModuleMod.waterchecker)
			{
				if (this.WaterEnable)
				{
					this.networkwaterEnable = true;
				}
				else
				{
					this.networkwaterDisable = true;
				}
				AdCustomModuleMod.waterchecker = false;
			}
			if (AdCustomModuleMod.markerOptionchecker)
			{
				this.networkMarkerOptionflag = true;
				AdCustomModuleMod.markerOptionchecker = false;
			}
			if (AdCustomModuleMod.waterSplashchecker)
			{
				this.networkWaterSplashOptionflag = true;
				AdCustomModuleMod.waterSplashchecker = false;
			}
			if (this.toggle_skyboxApply || this.networkflag)
			{
				selector = 0;
				changeflag = true;
				this.barrenChecker = false;
				this.FloorDeactive = this.FloorDeactiveSwitch;
				this.ExpandFloor = this.ExpandFloorSwitch;
				this.ExExpandFloor = this.ExExpandFloorSwitch;
				this.TerainObjectChecker();
				if (StatMaster.isMP && StatMaster.isHosting)
				{
					ModNetworking.SendToAll(AdCustomModuleMod.msgSkyBoxData.CreateMessage(new object[]
					{
						this.selector,
						this.FloorDeactiveSwitch,
						this.ExpandFloorSwitch,
						this.ExExpandFloorSwitch,
						this.scale
					}));
				}
				this.networkflag = false;
			}
			if (this.toggle_waterApply || this.networkwaterEnable)
			{
				GameObject gameObject = null;
				if (StatMaster.isMP)
				{
					gameObject = GameObject.Find("MULTIPLAYER LEVEL/Environments").gameObject;
				}
				else if (!StatMaster.isClient)
				{
					if (GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere") != null)
					{
						gameObject = GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere").gameObject;
					}
					else if (GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere") != null)
					{
						gameObject = GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere").gameObject;
					}
					else if (GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE") != null)
					{
						gameObject = GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE").gameObject;
					}
				}
				if (!gameObject.transform.FindChild("WaterPlane00"))
				{
					this.WaterObjectArray = new GameObject[3, 3];
					for (int num8 = 0; num8 < 3; num8++)
					{
						for (int num9 = 0; num9 < 3; num9++)
						{
							this.WaterObjectArray[num8, num9] = Object.Instantiate<GameObject>(this.WaterObject);
							this.WaterObjectArray[num8, num9].transform.SetParent(gameObject.transform);
							float num10 = -4000f + 4000f * (float)num8;
							float num11 = float.Parse(this.WaterValue[3]);
							float num12 = -4000f + 4000f * (float)num9;
							this.WaterObjectArray[num8, num9].transform.position = new Vector3(num10, num11, num12);
							this.WaterObjectArray[num8, num9].name = "WaterPlane" + num8.ToString() + num9.ToString();
							GameObject gameObject2 = this.WaterObjectArray[num8, num9].transform.FindChild("WaterProDaytime").gameObject;
						}
					}
				}
				else
				{
					for (int num13 = 0; num13 < 3; num13++)
					{
						for (int num14 = 0; num14 < 3; num14++)
						{
							float num15 = -4000f + 4000f * (float)num13;
							float num16 = float.Parse(this.WaterValue[3]);
							float num17 = -4000f + 4000f * (float)num14;
							this.WaterObjectArray[num13, num14].transform.position = new Vector3(num15, num16, num17);
						}
					}
				}
			}
			if (this.toggle_waterDisable || this.networkwaterDisable)
			{
				GameObject gameObject3 = null;
				if (StatMaster.isMP)
				{
					gameObject3 = GameObject.Find("MULTIPLAYER LEVEL/Environments").gameObject;
				}
				else if (!StatMaster.isClient)
				{
					if (GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere") != null)
					{
						gameObject3 = GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere").gameObject;
					}
					else if (GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere") != null)
					{
						gameObject3 = GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere").gameObject;
					}
					else if (GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE") != null)
					{
						gameObject3 = GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE").gameObject;
					}
				}
				for (int num28 = 0; num28 < 3; num28++)
				{
					for (int num29 = 0; num29 < 3; num29++)
					{
						Transform transform = gameObject3.transform.FindChild("WaterPlane" + num28.ToString() + num29.ToString());
						if (transform != null)
						{
							Object.Destroy(transform.gameObject);
						}
					}
				}
				this.WaterEnable = false;
				this.networkwaterDisable = false;
				
			}
			if (this.toggle_TargetMarkerDisable != this.TargetMarkerDisable_pre || this.networkMarkerOptionflag)
			{
				this.TargetMarkerDisable_pre = this.toggle_TargetMarkerDisable;
				this.networkMarkerOptionflag = false;
				if (StatMaster.isMP && StatMaster.isHosting)
				{
					ModNetworking.SendToAll(AdCustomModuleMod.msgMarkerOptionData.CreateMessage(new object[] { this.toggle_TargetMarkerDisable }));
				}
			}
			if (this.toggle_WaterSplashEnable != this.WaterSplashEnable_pre || this.networkWaterSplashOptionflag)
			{
				this.WaterSplashEnable_pre = this.toggle_WaterSplashEnable;
				this.networkWaterSplashOptionflag = false;
				if (StatMaster.isMP && StatMaster.isHosting)
				{
					ModNetworking.SendToAll(AdCustomModuleMod.msgWaterOptionData.CreateMessage(new object[] { this.toggle_WaterSplashEnable }));
				}
			}
			if (this.SkyObject != null && !this.SkyObject.activeSelf)
			{
				this.SkyObject.SetActive(true);
			}
			if (this.toggle_JoystickOptionSave)
			{
				if (!this.toggle_JoystickOptionSave_hold)
				{
					this.AdSaveData.SaveData();
				}
				this.toggle_JoystickOptionSave_hold = true;
			}
			else
			{
				this.toggle_JoystickOptionSave_hold = false;
			}
			if (this.toggle_JoystickOptionLoad)
			{
				if (!this.toggle_JoystickOptionLoad_hold)
				{
					this.AdSaveData.LoadData();
				}
				this.toggle_JoystickOptionLoad_hold = true;
				return;
			}
			this.toggle_JoystickOptionLoad_hold = false;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0002621C File Offset: 0x0002441C
		public void FixedUpdate()
		{
			if (!StatMaster.isMP)
			{
				bool isLocalSim = StatMaster.isLocalSim;
			}
			if (StatMaster.levelSimulating)
			{
				if (!this.WaterBulletBouncyCheck || !this.ModuleCollisionModeCheck)
				{
					if (StatMaster.isMP)
					{
						this.ProjectilePool = GameObject.Find("PROJECTILES/Projectile Pool");
					}
					else
					{
						this.ProjectilePool = GameObject.Find("PHYSICS GOAL");
					}
					this.ModuleCollisionModeCheck = true;
					this.WaterBulletBouncyCheck = true;
				}
			}
			else
			{
				this.ModuleCollisionModeCheck = false;
				this.WaterBulletBouncyCheck = false;
			}
			if (this.WaterEnable && this.WaterBouncyEnable && StatMaster.levelSimulating && this.WaterBulletBouncyCheck)
			{
				foreach (object obj in this.ProjectilePool.transform)
				{
					Transform transform = (Transform)obj;
					if (StatMaster.isMP)
					{
						if (transform.GetComponent<Rigidbody>() && !transform.GetComponent<WaterProjectileBehaviour>())
						{
							transform.gameObject.AddComponent<WaterProjectileBehaviour>();
						}
					}
					else if (transform.gameObject.name == "ShootingProjectileNetworked(Clone)(Clone)" && !transform.GetComponent<WaterProjectileBehaviour>())
					{
						transform.gameObject.AddComponent<WaterProjectileBehaviour>();
					}
				}
			}
			if (StatMaster.levelSimulating && this.ModuleCollisionModeCheck)
			{
				foreach (object obj2 in this.ProjectilePool.transform)
				{
					Transform transform2 = (Transform)obj2;
					if (StatMaster.isMP)
					{
						if (transform2.GetComponent<Rigidbody>() && !transform2.GetComponent<ModuleProjectileBehavior>())
						{
							transform2.gameObject.AddComponent<ModuleProjectileBehavior>();
						}
					}
					else if (transform2.gameObject.name == "ShootingProjectileNetworked(Clone)(Clone)" && !transform2.GetComponent<ModuleProjectileBehavior>())
					{
						transform2.gameObject.AddComponent<ModuleProjectileBehavior>();
					}
				}
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00003098 File Offset: 0x00001298
		private IEnumerator SkyChecker()
		{
			yield return new WaitForSeconds(1f);
			ModNetworking.SendToHost(AdCustomModuleMod.msgSkyBoxChecker.CreateMessage(new object[] { true }));
			yield return new WaitForSeconds(1f);
			while (!AdCustomModuleMod.skyboxcheckerCallBack)
			{
				yield return new WaitForSeconds(1f);
				ModNetworking.SendToHost(AdCustomModuleMod.msgSkyBoxChecker.CreateMessage(new object[] { true }));
				yield return new WaitForSeconds(1f);
			}
			yield return null;
			yield break;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x000030A0 File Offset: 0x000012A0
		private IEnumerator WaterChecker()
		{
			this.waterCallback = false;
			yield return new WaitForSeconds(1f);
			ModNetworking.SendToHost(AdCustomModuleMod.msgWaterChecker.CreateMessage(new object[] { true }));
			yield return new WaitForSeconds(1f);
			while (!this.waterCallback)
			{
				yield return new WaitForSeconds(1f);
				this.waterCallback = false;
				ModNetworking.SendToHost(AdCustomModuleMod.msgWaterChecker.CreateMessage(new object[] { true }));
				yield return new WaitForSeconds(1f);
			}
			yield return null;
			yield break;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x000030AF File Offset: 0x000012AF
		private IEnumerator MarkerOptionChecker()
		{
			this.markerCallback = false;
			yield return new WaitForSeconds(1f);
			ModNetworking.SendToHost(AdCustomModuleMod.msgMarkerOptionChecker.CreateMessage(new object[] { true }));
			yield return new WaitForSeconds(1f);
			while (!this.markerCallback)
			{
				yield return new WaitForSeconds(1f);
				ModNetworking.SendToHost(AdCustomModuleMod.msgMarkerOptionChecker.CreateMessage(new object[] { true }));
				yield return new WaitForSeconds(1f);
			}
			yield return null;
			yield break;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x000030BE File Offset: 0x000012BE
		private IEnumerator WaterSplashchecker()
		{
			this.waterSplashCallback = false;
			yield return new WaitForSeconds(1f);
			ModNetworking.SendToHost(AdCustomModuleMod.msgWaterOptionChecker.CreateMessage(new object[] { true }));
			yield return new WaitForSeconds(1f);
			while (!this.waterSplashCallback)
			{
				yield return new WaitForSeconds(1f);
				ModNetworking.SendToHost(AdCustomModuleMod.msgWaterOptionChecker.CreateMessage(new object[] { true }));
				yield return new WaitForSeconds(1f);
			}
			yield return null;
			yield break;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00026424 File Offset: 0x00024624
		private void TerainObjectChecker()
		{
			if (StatMaster.isMP)
			{
				while (GameObject.Find("Main Camera/FOG SPHERE"))
				{
					GameObject.Find("Main Camera/FOG SPHERE").SetActive(false);
				}
				while (GameObject.Find("Main Camera/Fog Volume"))
				{
					GameObject.Find("Main Camera/Fog Volume").SetActive(false);
				}
				while (GameObject.Find("Main Camera/Fog Volume Dark"))
				{
					GameObject.Find("Main Camera/Fog Volume Dark").SetActive(false);
				}
				while (GameObject.Find("Main Camera/Fog Volume Dark (1)"))
				{
					GameObject.Find("Main Camera/Fog Volume Dark (1)").SetActive(false);
				}
				if (GameObject.Find("MULTIPLAYER LEVEL/Environments/Ipsilon").activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.ipsilon;
				}
				else if (GameObject.Find("MULTIPLAYER LEVEL/Environments/Tolbrynd").activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.Tolbrynd;
				}
				else if (GameObject.Find("MULTIPLAYER LEVEL/Environments/MountainTop").activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.MountainTop;
				}
				else if (GameObject.Find("MULTIPLAYER LEVEL/Environments/Desert").activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.Desert;
				}
				else if (GameObject.Find("MULTIPLAYER LEVEL/Environments/Barren").activeSelf)
				{
					this.TerainChecker = SkyBoxChanger.TerainType.Barren;
				}
				this.EnvironmentsObject = GameObject.Find("MULTIPLAYER LEVEL/Environments/");
				switch (this.TerainChecker)
				{
				case SkyBoxChanger.TerainType.Barren:
					this.EnvironmentsObject.transform.Find("Barren/BarrenEnv").gameObject.SetActive(true);
					break;
				case SkyBoxChanger.TerainType.ipsilon:
					this.EnvironmentsObject.transform.Find("Barren").gameObject.SetActive(true);
					this.EnvironmentsObject.transform.Find("Barren/BarrenEnv").gameObject.SetActive(false);
					break;
				case SkyBoxChanger.TerainType.Tolbrynd:
					this.EnvironmentsObject.transform.Find("Barren").gameObject.SetActive(true);
					this.EnvironmentsObject.transform.Find("Barren/BarrenEnv").gameObject.SetActive(false);
					break;
				case SkyBoxChanger.TerainType.MountainTop:
					this.EnvironmentsObject.transform.Find("Barren").gameObject.SetActive(true);
					this.EnvironmentsObject.transform.Find("Barren/BarrenEnv").gameObject.SetActive(false);
					break;
				case SkyBoxChanger.TerainType.Desert:
					this.EnvironmentsObject.transform.Find("Barren").gameObject.SetActive(true);
					this.EnvironmentsObject.transform.Find("Barren/BarrenEnv").gameObject.SetActive(false);
					this.EnvironmentsObject.transform.Find("Desert/DesertAtmosphere (2)").gameObject.SetActive(false);
					break;
				}
				if (GameObject.Find("MULTIPLAYER LEVEL/Environments/Barren/AviamisAtmosphere/STAR SPHERE") != null)
				{
					GameObject gameObject = GameObject.Find("MULTIPLAYER LEVEL/Environments/Barren/AviamisAtmosphere/STAR SPHERE").gameObject;
					gameObject.transform.FindChild("StarField (1)").gameObject.SetActive(false);
					gameObject.transform.FindChild("Shooting stars").gameObject.SetActive(false);
					gameObject.SetActive(false);
				}
				if (this.SkyObject == null)
				{
					this.SkyObject = new GameObject();
					this.SkyObject.name = "SKY SPHERE";
					this.SkyObject.transform.localScale = new Vector3(4000f, 4000f, 4000f);
					this.SkyObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
					this.SkyObject.AddComponent<MeshFilter>();
					this.SkyObject.AddComponent<MeshRenderer>();
					this.SkyObject.AddComponent<mySmoothFollow>();
					mySmoothFollow component = this.SkyObject.GetComponent<mySmoothFollow>();
					component.target = Camera.main.transform;
					component.smoothAmount = 100f;
					Renderer component2 = this.SkyObject.GetComponent<Renderer>();
					Shader shader = Shader.Find("Particles/Additive (UnityEngine.Shader)");
					component2.material.shader = shader;
					this.SkyObject.transform.parent = this.EnvironmentsObject.transform;
				}
				if (this.BoundaryBottom == null)
				{
					GameObject gameObject2 = GameObject.Find("WORLD BOUNDARIES").gameObject;
					this.BoundaryBottom = Object.Instantiate<GameObject>(gameObject2.transform.Find("WorldBoundaryTop").gameObject);
					this.BoundaryBottom.transform.SetParent(gameObject2.transform);
					this.BoundaryBottom.name = "WorldBoundaryBottom";
					this.BoundaryBottom.transform.position = new Vector3(0f, -250f, 0f);
				}
				if (GameObject.Find("ICE FREEZE") != null)
				{
					GameObject.Find("ICE FREEZE").gameObject.SetActive(false);
				}
				if (this.ExpandFloor || this.ExExpandFloor)
				{
					if (this.ExpandFloor)
					{
						this.BoundarySize = 20000f;
					}
					else if (this.ExExpandFloor)
					{
						this.BoundarySize = this.scale;
					}
					StatMaster.Bounding.worldExtents = new Vector3(this.BoundarySize / 2f, this.BoundarySize, this.BoundarySize / 2f);
					GameObject gameObject3 = GameObject.Find("WORLD BOUNDARIES").gameObject;
					Transform transform = gameObject3.transform.Find("WorldBoundaryBack");
					transform.position = new Vector3(0f, -250f, -this.BoundarySize / 2f);
					BoxCollider component3 = transform.GetComponent<BoxCollider>();
					component3.size = new Vector3(this.BoundarySize, this.BoundarySize, 20f);
					component3.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
					Transform transform2 = gameObject3.transform.Find("WorldBoundaryFront");
					transform2.position = new Vector3(0f, -250f, this.BoundarySize / 2f);
					BoxCollider component4 = transform2.GetComponent<BoxCollider>();
					component4.size = new Vector3(this.BoundarySize, this.BoundarySize, 20f);
					component4.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
					Transform transform3 = gameObject3.transform.Find("WorldBoundaryLeft");
					transform3.position = new Vector3(-this.BoundarySize / 2f, -250f, 0f);
					BoxCollider component5 = transform3.GetComponent<BoxCollider>();
					component5.size = new Vector3(20f, this.BoundarySize, this.BoundarySize);
					component5.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
					Transform transform4 = gameObject3.transform.Find("WorldBoundaryRight");
					transform4.position = new Vector3(this.BoundarySize / 2f, -250f, 0f);
					BoxCollider component6 = transform4.GetComponent<BoxCollider>();
					component6.size = new Vector3(20f, this.BoundarySize, this.BoundarySize);
					component6.center = new Vector3(0f, this.BoundarySize / 2f, 0f);
					Transform transform5 = gameObject3.transform.Find("WorldBoundaryTop");
					transform5.position = new Vector3(0f, this.BoundarySize - 250f, 0f);
					BoxCollider component7 = transform5.GetComponent<BoxCollider>();
					component7.size = new Vector3(this.BoundarySize, 20f, this.BoundarySize);
					component7.center = new Vector3(0f, 0f, 10f);
					Transform transform6 = gameObject3.transform.Find("WorldBoundaryBottom");
					transform6.position = new Vector3(0f, -250f, 0f);
					BoxCollider component8 = transform6.GetComponent<BoxCollider>();
					component8.size = new Vector3(this.BoundarySize, 20f, this.BoundarySize);
					component8.center = new Vector3(0f, 0f, 10f);
					Collider[] componentsInChildren = gameObject3.GetComponentsInChildren<Collider>();
					Bounds worldBounds = default(Bounds);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Bounds bounds = componentsInChildren[i].bounds;
						if (i == 0)
						{
							worldBounds = bounds;
						}
						else
						{
							worldBounds.Encapsulate(bounds);
						}
					}
					NetworkCompression.SetWorldBounds(worldBounds);
				}
				else if (!this.ExpandFloor && !this.ExExpandFloor)
				{
					this.BoundarySize = 2000f;
					GameObject gameObject4 = GameObject.Find("WORLD BOUNDARIES").gameObject;
					Transform transform7 = gameObject4.transform.Find("WorldBoundaryBack");
					transform7.position = new Vector3(0f, -250f, -1000f);
					BoxCollider component9 = transform7.GetComponent<BoxCollider>();
					component9.size = new Vector3(2000f, 2000f, 20f);
					component9.center = new Vector3(0f, 1000f, 0f);
					Transform transform8 = gameObject4.transform.Find("WorldBoundaryFront");
					transform8.position = new Vector3(0f, -250f, 1000f);
					BoxCollider component10 = transform8.GetComponent<BoxCollider>();
					component10.size = new Vector3(2000f, 2000f, 20f);
					component10.center = new Vector3(0f, 1000f, 0f);
					Transform transform9 = gameObject4.transform.Find("WorldBoundaryLeft");
					transform9.position = new Vector3(-1000f, -250f, 0f);
					BoxCollider component11 = transform9.GetComponent<BoxCollider>();
					component11.size = new Vector3(20f, 2000f, 2000f);
					component11.center = new Vector3(0f, 1000f, 0f);
					Transform transform10 = gameObject4.transform.Find("WorldBoundaryRight");
					transform10.position = new Vector3(1000f, -250f, 0f);
					BoxCollider component12 = transform10.GetComponent<BoxCollider>();
					component12.size = new Vector3(20f, 2000f, 2000f);
					component12.center = new Vector3(0f, 1000f, 0f);
					Transform transform11 = gameObject4.transform.Find("WorldBoundaryTop");
					transform11.position = new Vector3(0f, 1750f, 0f);
					BoxCollider component13 = transform11.GetComponent<BoxCollider>();
					component13.size = new Vector3(2000f, 20f, 2000f);
					component13.center = new Vector3(0f, 0f, 10f);
					Transform transform12 = gameObject4.transform.Find("WorldBoundaryBottom");
					transform12.position = new Vector3(0f, -250f, 0f);
					BoxCollider component14 = transform12.GetComponent<BoxCollider>();
					component14.size = new Vector3(2000f, 20f, 2000f);
					component14.center = new Vector3(0f, 0f, 10f);
					Collider[] componentsInChildren2 = gameObject4.GetComponentsInChildren<Collider>();
					Bounds worldBounds2 = default(Bounds);
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						Bounds bounds2 = componentsInChildren2[j].bounds;
						if (j == 0)
						{
							worldBounds2 = bounds2;
						}
						else
						{
							worldBounds2.Encapsulate(bounds2);
						}
					}
					NetworkCompression.SetWorldBounds(worldBounds2);
				}
				if (this.FloorObject == null)
				{
					this.FloorObject = GameObject.Find("MULTIPLAYER LEVEL/FloorBig");
				}
				if (this.FloorDeactive && this.FloorObject.activeSelf)
				{
					this.FloorObject.SetActive(false);
					return;
				}
				if (!this.FloorDeactive && !this.FloorObject.activeSelf)
				{
					this.FloorObject.SetActive(true);
					return;
				}
			}
			else if (!StatMaster.isClient)
			{
				while (GameObject.Find("Main Camera/FOG SPHERE"))
				{
					GameObject.Find("Main Camera/FOG SPHERE").SetActive(false);
				}
				while (GameObject.Find("Main Camera/Fog Volume"))
				{
					GameObject.Find("Main Camera/Fog Volume").SetActive(false);
				}
				if (GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere/STAR SPHERE") != null)
				{
					this.SkyObject = GameObject.Find("LEVEL BARREN EXPANSE/AviamisAtmosphere/STAR SPHERE").gameObject;
					this.SkyObject.transform.FindChild("StarField (1)").gameObject.SetActive(false);
					this.SkyObject.transform.FindChild("Shooting stars").gameObject.SetActive(false);
					if (this.FloorObject == null)
					{
						this.FloorObject = GameObject.Find("LEVEL BARREN EXPANSE/FloorBig");
					}
					if (this.FloorDeactive && this.FloorObject.activeSelf)
					{
						this.FloorObject.SetActive(false);
						return;
					}
					if (!this.FloorObject.activeSelf)
					{
						this.FloorObject.SetActive(true);
						return;
					}
				}
				else if (GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere/STAR SPHERE") != null)
				{
					this.SkyObject = GameObject.Find("LEVEL SANDBOX/AviamisAtmosphere/STAR SPHERE").gameObject;
					this.SkyObject.transform.FindChild("StarField (1)").gameObject.SetActive(false);
					this.SkyObject.transform.FindChild("Shooting stars").gameObject.SetActive(false);
					if (this.FloorObject == null)
					{
						this.FloorObject = GameObject.Find("LEVEL SANDBOX/FloorBig");
					}
					if (this.FloorDeactive && this.FloorObject.activeSelf)
					{
						this.FloorObject.SetActive(false);
						return;
					}
					if (!this.FloorObject.activeSelf)
					{
						this.FloorObject.SetActive(true);
						return;
					}
				}
				else if (GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE") != null)
				{
					if (this.SkyObject == null)
					{
						this.SkyObject = new GameObject();
						this.SkyObject.name = "SKY SPHERE";
						this.SkyObject.transform.localScale = new Vector3(4000f, 4000f, 4000f);
						this.SkyObject.AddComponent<MeshFilter>();
						this.SkyObject.AddComponent<MeshRenderer>();
						this.SkyObject.AddComponent<mySmoothFollow>();
						Renderer component15 = this.SkyObject.GetComponent<Renderer>();
						Shader shader2 = Shader.Find("Particles/Additive (UnityEngine.Shader)");
						component15.material.shader = shader2;
						this.SkyObject.transform.parent = GameObject.Find("LEVEL MISTY MOUNTAIN/ATMOSPHERE").transform;
					}
					if (this.FloorObject == null)
					{
						this.FloorObject = GameObject.Find("LEVEL MISTY MOUNTAIN/FloorBig");
					}
					if (this.FloorDeactive && this.FloorObject.activeSelf)
					{
						this.FloorObject.SetActive(false);
						return;
					}
					if (!this.FloorObject.activeSelf)
					{
						this.FloorObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00027484 File Offset: 0x00025684
		private void OnGUI()
		{
			this.gui_ACMOptions = delegate(int p0)
			{
				if (this.Init)
				{
					GUILayout.BeginVertical("box", new GUILayoutOption[0]);
					
					//GUILayout.Label("SkyBoxName : " + this.SkyNameContainer[this.selector], new GUILayoutOption[0]);
					this.toggle_skyboxApply = GUILayout.Button("Apply", new GUILayoutOption[0]);
					this.FloorDeactiveSwitch = GUILayout.Toggle(this.FloorDeactiveSwitch, "FloorDeactive", new GUILayoutOption[0]);
					this.ExpandFloorSwitch = GUILayout.Toggle(this.ExpandFloorSwitch, "空气墙扩大10倍", new GUILayoutOption[0]);
					if (this.ExpandFloorSwitch && this.ExExpandFloorSwitch)
					{
						this.ExExpandFloorSwitch = false;
					}
					this.ExExpandFloorSwitch = GUILayout.Toggle(this.ExExpandFloorSwitch, "空气墙扩大自定义倍", new GUILayoutOption[0]);
					if (this.ExpandFloorSwitch && this.ExExpandFloorSwitch)
					{
						this.ExpandFloorSwitch = false;
					}
					this.scale = Convert.ToSingle(GUILayout.TextArea(this.scale.ToString(), new GUILayoutOption[0]));

					GUILayout.EndVertical();
					/*
					GUILayout.BeginVertical("box", new GUILayoutOption[0]);
					GUILayout.Label("Water : " + this.WaterEnable.ToString(), new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					this.toggle_waterApply = GUILayout.Button("Apply", new GUILayoutOption[0]);
					this.toggle_waterDisable = GUILayout.Button("Disable", new GUILayoutOption[0]);
					GUILayout.EndHorizontal();
					this.SliderandTextField("WaterHeight", ref this.WaterValue[3], ref this.WaterValue_temp[3], 0f, 200f, "Slider_WaterValue", 7, false, 150f);
					this.WaterBouncyEnableSwitch = GUILayout.Toggle(this.WaterBouncyEnableSwitch, "EnableFloating", new GUILayoutOption[0]);
					this.toggle_WaterSplashEnable = GUILayout.Toggle(this.toggle_WaterSplashEnable, "WaterSplashEnable", new GUILayoutOption[0]);
					this.WaterBouncyDebug = GUILayout.Toggle(this.WaterBouncyDebug, "FloatingVisible", new GUILayoutOption[0]);
					this.ToggleIndent("WaterSettings", 10f, ref this.indentflag3, delegate
					{
						GUILayout.BeginVertical("box", new GUILayoutOption[0]);
						this.SliderandTextField("WaveScale", ref this.WaterValue[0], ref this.WaterValue_temp[0], 0f, 1f, "Slider_WaveScale", 7, false, 150f);
						this.SliderandTextField("ReflectionDistort", ref this.WaterValue[1], ref this.WaterValue_temp[1], 0f, 1f, "Slider_ReflDistort", 7, false, 150f);
						this.SliderandTextField("RefractionDistort", ref this.WaterValue[2], ref this.WaterValue_temp[2], 0f, 1f, "Slider_RefrDistort", 7, false, 150f);
						this.SliderandTextField("WaterClearlity", ref this.WaterValue[4], ref this.WaterValue_temp[4], 0f, 0.1f, "Slider_WaterClearlity", 7, false, 150f);
						this.SliderandTextField("ExpDensity", ref this.WaterValue[5], ref this.WaterValue_temp[5], 0f, 2f, "Slider_ExponentialDensity", 7, false, 150f);
						this.SliderandTextField("EndClearlity", ref this.WaterValue[6], ref this.WaterValue_temp[6], 0f, 1f, "Slider_FadeEnd", 7, true, 150f);
						this.SliderandTextField("FogIntensity", ref this.WaterValue[11], ref this.WaterValue_temp[11], 0f, 0.1f, "Slider_FogIntensity", 7, false, 150f);
						this.SliderandTextField("FogExpDensity", ref this.WaterValue[12], ref this.WaterValue_temp[12], 0f, 2f, "Slider_FogExponentialDensity", 7, false, 150f);
						this.SliderandTextField("FogEndAlpha", ref this.WaterValue[13], ref this.WaterValue_temp[13], 0f, 1f, "Slider_FogFadeEnd", 7, true, 150f);
						this.SliderandTextField("WaveSpeed x1", ref this.WaterValue[7], ref this.WaterValue_temp[7], -25f, 25f, "Slider_WaveSpeed x1", 7, false, 150f);
						this.SliderandTextField("WaveSpeed y1", ref this.WaterValue[8], ref this.WaterValue_temp[8], -25f, 25f, "Slider_WaveSpeed y1", 7, false, 150f);
						this.SliderandTextField("WaveSpeed x2", ref this.WaterValue[9], ref this.WaterValue_temp[9], -25f, 25f, "Slider_WaveSpeed x2", 7, false, 150f);
						this.SliderandTextField("WaveSpeed y2", ref this.WaterValue[10], ref this.WaterValue_temp[10], -25f, 25f, "Slider_WaveSpeed y2", 7, false, 150f);
						this.SliderandTextField("UWaterFogMaxDistance", ref this.WaterFogValue[0], ref this.WaterFogValue_temp[0], 0f, 3000f, "UWaterFogMaxDistance", 7, false, 150f);
						this.SliderandTextField("UWaterExpDensity", ref this.WaterFogValue[1], ref this.WaterFogValue_temp[1], 0f, 2f, "UWaterExpDensity", 7, false, 150f);
						this.SliderandTextField("UWaterFogEndAlpha", ref this.WaterFogValue[2], ref this.WaterFogValue_temp[2], 0f, 1f, "UWaterFogEndAlpha", 7, true, 150f);
						GUILayout.EndVertical();
					});
					this.ToggleIndent("WaterColors", 10f, ref this.indentflag4, delegate
					{
						GUILayout.BeginVertical("box", new GUILayoutOption[0]);
						GUILayout.Label("WaterRefrColor", new GUILayoutOption[0]);
						this.SliderandTextField("R :", ref this.WaterRefrColor[0], ref this.WaterRefrColor_temp[0], 0f, 1f, "WaterRefrColor_R", 7, true, 150f);
						this.SliderandTextField("G :", ref this.WaterRefrColor[1], ref this.WaterRefrColor_temp[1], 0f, 1f, "WaterRefrColor_G", 7, true, 150f);
						this.SliderandTextField("B :", ref this.WaterRefrColor[2], ref this.WaterRefrColor_temp[2], 0f, 1f, "WaterRefrColor_B", 7, true, 150f);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label(" ", new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(140f)
						});
						this.WaterRefrColorTex.SetPixel(0, 0, new Color(float.Parse(this.WaterRefrColor[0]), float.Parse(this.WaterRefrColor[1]), float.Parse(this.WaterRefrColor[2])));
						this.WaterRefrColorTex.Apply();
						this.WaterRefrColorGUI.normal.background = this.WaterRefrColorTex;
						this.WaterRefrColorGUI.fixedHeight = 15f;
						this.WaterRefrColorGUI.fixedWidth = 100f;
						GUILayout.BeginHorizontal(this.WaterRefrColorGUI, new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(100f)
						});
						GUILayout.Label(" ", new GUILayoutOption[] { GUILayout.Height(15f) });
						GUILayout.EndHorizontal();
						GUILayout.EndHorizontal();
						GUILayout.Label("WaterFadeColor", new GUILayoutOption[0]);
						this.SliderandTextField("R :", ref this.WaterFadeColor[0], ref this.WaterFadeColor_temp[0], 0f, 1f, "WaterFadeColor_R", 7, true, 150f);
						this.SliderandTextField("G :", ref this.WaterFadeColor[1], ref this.WaterFadeColor_temp[1], 0f, 1f, "WaterFadeColor_G", 7, true, 150f);
						this.SliderandTextField("B :", ref this.WaterFadeColor[2], ref this.WaterFadeColor_temp[2], 0f, 1f, "WaterFadeColor_B", 7, true, 150f);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label(" ", new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(140f)
						});
						this.WaterFadeColorTex.SetPixel(0, 0, new Color(float.Parse(this.WaterFadeColor[0]), float.Parse(this.WaterFadeColor[1]), float.Parse(this.WaterFadeColor[2])));
						this.WaterFadeColorTex.Apply();
						this.WaterFadeColorGUI.normal.background = this.WaterFadeColorTex;
						this.WaterFadeColorGUI.fixedHeight = 15f;
						this.WaterFadeColorGUI.fixedWidth = 100f;
						GUILayout.BeginHorizontal(this.WaterFadeColorGUI, new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(100f)
						});
						GUILayout.Label(" ", new GUILayoutOption[] { GUILayout.Height(15f) });
						GUILayout.EndHorizontal();
						GUILayout.EndHorizontal();
						GUILayout.Label("WaterFogColor", new GUILayoutOption[0]);
						this.SliderandTextField("R :", ref this.WaterFogColor[0], ref this.WaterFogColor_temp[0], 0f, 1f, "WaterFogColor_R", 7, true, 150f);
						this.SliderandTextField("G :", ref this.WaterFogColor[1], ref this.WaterFogColor_temp[1], 0f, 1f, "WaterFogColor_G", 7, true, 150f);
						this.SliderandTextField("B :", ref this.WaterFogColor[2], ref this.WaterFogColor_temp[2], 0f, 1f, "WaterFogColor_B", 7, true, 150f);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label(" ", new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(140f)
						});
						this.WaterFogColorTex.SetPixel(0, 0, new Color(float.Parse(this.WaterFogColor[0]), float.Parse(this.WaterFogColor[1]), float.Parse(this.WaterFogColor[2])));
						this.WaterFogColorTex.Apply();
						this.WaterFogColorGUI.normal.background = this.WaterFogColorTex;
						this.WaterFogColorGUI.fixedHeight = 15f;
						this.WaterFogColorGUI.fixedWidth = 100f;
						GUILayout.BeginHorizontal(this.WaterFogColorGUI, new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(100f)
						});
						GUILayout.Label(" ", new GUILayoutOption[] { GUILayout.Height(15f) });
						GUILayout.EndHorizontal();
						GUILayout.EndHorizontal();
						GUILayout.Label("InWaterRefColor", new GUILayoutOption[0]);
						this.SliderandTextField("R :", ref this.InWaterRefColor[0], ref this.InWaterRefColor_temp[0], 0f, 1f, "InWaterRefColor_R", 7, true, 150f);
						this.SliderandTextField("G :", ref this.InWaterRefColor[1], ref this.InWaterRefColor_temp[1], 0f, 1f, "InWaterRefColor_G", 7, true, 150f);
						this.SliderandTextField("B :", ref this.InWaterRefColor[2], ref this.InWaterRefColor_temp[2], 0f, 1f, "InWaterRefColor_B", 7, true, 150f);
						this.SliderandTextField("A :", ref this.InWaterRefColor[3], ref this.InWaterRefColor_temp[3], 0f, 1f, "InWaterRefColor_A", 7, true, 150f);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label(" ", new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(140f)
						});
						this.InWaterRefColorTex.SetPixel(0, 0, new Color(float.Parse(this.InWaterRefColor[0]), float.Parse(this.InWaterRefColor[1]), float.Parse(this.InWaterRefColor[2])));
						this.InWaterRefColorTex.Apply();
						this.InWaterRefColorGUI.normal.background = this.InWaterRefColorTex;
						this.InWaterRefColorGUI.fixedHeight = 15f;
						this.InWaterRefColorGUI.fixedWidth = 100f;
						GUILayout.BeginHorizontal(this.InWaterRefColorGUI, new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(100f)
						});
						GUILayout.Label(" ", new GUILayoutOption[] { GUILayout.Height(15f) });
						GUILayout.EndHorizontal();
						GUILayout.EndHorizontal();
						GUILayout.Label("InWaterFarColor", new GUILayoutOption[0]);
						this.SliderandTextField("R :", ref this.InWaterFarColor[0], ref this.InWaterFarColor_temp[0], 0f, 1f, "InWaterFarColor_R", 7, true, 150f);
						this.SliderandTextField("G :", ref this.InWaterFarColor[1], ref this.InWaterFarColor_temp[1], 0f, 1f, "InWaterFarColor_G", 7, true, 150f);
						this.SliderandTextField("B :", ref this.InWaterFarColor[2], ref this.InWaterFarColor_temp[2], 0f, 1f, "InWaterFarColor_B", 7, true, 150f);
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label(" ", new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(140f)
						});
						this.InWaterFarColorTex.SetPixel(0, 0, new Color(float.Parse(this.InWaterFarColor[0]), float.Parse(this.InWaterFarColor[1]), float.Parse(this.InWaterFarColor[2])));
						this.InWaterFarColorTex.Apply();
						this.InWaterFarColorGUI.normal.background = this.InWaterFarColorTex;
						this.InWaterFarColorGUI.fixedHeight = 15f;
						this.InWaterFarColorGUI.fixedWidth = 100f;
						GUILayout.BeginHorizontal(this.InWaterFarColorGUI, new GUILayoutOption[]
						{
							GUILayout.Height(15f),
							GUILayout.Width(100f)
						});
						GUILayout.Label(" ", new GUILayoutOption[] { GUILayout.Height(15f) });
						GUILayout.EndHorizontal();
						GUILayout.EndHorizontal();
						GUILayout.EndVertical();
					});
					GUILayout.EndVertical();
					GUILayout.BeginVertical("box", new GUILayoutOption[0]);
					this.ModuleCollisionModeSwitch_CD = GUILayout.Toggle(this.ModuleCollisionModeSwitch_CD, "ProjectileCollisionMode_ContinuousDynamic", new GUILayoutOption[0]);
					this.toggle_TargetMarkerDisable = GUILayout.Toggle(this.toggle_TargetMarkerDisable, "TargetMarkerDisable", new GUILayoutOption[0]);
					GUILayout.EndVertical();
				}
				if (this.debug)
				{
					GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
					GUILayout.BeginVertical(new GUILayoutOption[0]);
					GUILayout.Label("maxPlayersPerHost : " + OptionsMaster.maxPlayersPerHost.ToString(), new GUILayoutOption[0]);
					GUILayout.Label("maxPlayers : " + OptionsMaster.maxPlayers.ToString(), new GUILayoutOption[0]);
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label("Data", new GUILayoutOption[0]);
					for (int i = 0; i < 4; i++)
					{
						if (AdCustomModuleMod.mod2.BeaconContainer.ContainsKey(i))
						{
							GUILayout.BeginVertical("box", new GUILayoutOption[0]);
							GUILayout.Label("ID : " + i.ToString(), new GUILayoutOption[0]);
							this.posi_x[i] = AdCustomModuleMod.mod2.BeaconContainer[i].Posi.x;
							this.posi_y[i] = AdCustomModuleMod.mod2.BeaconContainer[i].Posi.y;
							this.posi_z[i] = AdCustomModuleMod.mod2.BeaconContainer[i].Posi.z;
							GUILayout.Label("x : " + string.Format("{0,8:F2}", this.posi_x[i]), new GUILayoutOption[0]);
							GUILayout.Label("y : " + string.Format("{0,8:F2}", this.posi_y[i]), new GUILayoutOption[0]);
							GUILayout.Label("z : " + string.Format("{0,8:F2}", this.posi_z[i]), new GUILayoutOption[0]);
							GUILayout.EndVertical();
							GUILayout.FlexibleSpace();
						}
					}
					GUILayout.EndHorizontal();
					this.guiScroll = GUILayout.BeginScrollView(this.guiScroll, new GUILayoutOption[] { GUILayout.Width(140f) });
					GUILayout.BeginVertical("box", new GUILayoutOption[0]);
					GUILayout.Label("J1A0 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis0") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A1 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis1") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A2 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis2") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A3 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis3") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A4 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis4") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A5 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis5") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A6 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis6") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A7 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis7") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A8 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis8") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A9 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis9") * 10f), new GUILayoutOption[0]);
					GUILayout.Label("J1A10 : " + string.Format("{0,8:F2}", Input.GetAxisRaw("Joy1Axis10") * 10f), new GUILayoutOption[0]);
					GUILayout.EndVertical();
					GUILayout.EndScrollView();
					GUILayout.EndHorizontal();*/
				}
				GUI.DragWindow();
			};
			if (((!this.hide && StatMaster.isHosting) || (!this.hide && !StatMaster.isClient)) && !StatMaster.isMainMenu && !StatMaster.inMenu && ((!this.Keyhide && StatMaster.isHosting) || (!this.Keyhide && !StatMaster.isClient)) && !StatMaster.isMainMenu && !StatMaster.inMenu)
			{
				this.windowHeight = 300f;
				this.windowRect.height = this.windowHeight;
				//this.windowRect = GUI.Window(this.window_id, this.windowRect, this.gui_ACMOptions, "Extend Options");
				//this.windowRect3 = GUI.Window(this.window_id3, this.windowRect3, this.gui_JoystickOptions, "Joystick Options");
			}
			this.gui_NetworkOptions = delegate(int p0)
			{
				GUILayout.BeginVertical("box", new GUILayoutOption[0]);
				this.ClampTextField_Int("Number of PlayersConnection", ref this.maxPlayersValue[0], ref this.maxPlayersValue_temp[0], 0, 18, "maxPlayers", 10, true);
				GUILayout.EndVertical();
				GUI.DragWindow();
			};
			if (!this.Keyhide2 && StatMaster.inMenu && StatMaster.isMP && !StatMaster.isClient && !StatMaster.isHosting)
			{
				this.windowRect2 = GUI.Window(this.window_id2, this.windowRect2, this.gui_NetworkOptions, "Network Options");
			}
			this.gui_JoystickOptions = delegate(int p0)
			{
				GUILayout.BeginVertical("box", new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Joystick Number : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.joystickNumber, ref this.temp_string, 0f, 0f, "JoystickNumber", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("LStick Axis-X Number : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.LStick_X, ref this.temp_string, 0f, 0f, "LStickAxisXNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("LStick Axis-Y Number : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.LStick_Y, ref this.temp_string, 0f, 0f, "LStickAxisYNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("RStick Axis-X Number : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.RStick_X, ref this.temp_string, 0f, 0f, "RStickAxisXNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("RStick Axis-Y Number : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.RStick_Y, ref this.temp_string, 0f, 0f, "RStickAxisYNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Pad Axis-X Number : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.Pad_X, ref this.temp_string, 0f, 0f, "PadAxisXNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Pad Axis-Y Number : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.Pad_Y, ref this.temp_string, 0f, 0f, "PadAxisYNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("LTriger : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.LTriger, ref this.temp_string, 0f, 0f, "LTrigerNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("RTriger : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.RTriger, ref this.temp_string, 0f, 0f, "RTrigerNum", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("DeadZone : ", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				this.ClampTextField2("", ref this.AdSaveData.ACMconfig.deadZone, ref this.temp_string, 0f, 0f, "DeadZone", 10, false);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.AdSaveData.ACMconfig.DS4trigermode = GUILayout.Toggle(this.AdSaveData.ACMconfig.DS4trigermode, "TrigerMode -10 to 10", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Raw Data", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(140f) });
				GUILayout.Label("LStick Axis-X : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.LStick_X) * 100f), new GUILayoutOption[0]);
				GUILayout.Label("LStick Axis-Y : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.LStick_Y) * 100f), new GUILayoutOption[0]);
				GUILayout.Label("RStick Axis-X : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.RStick_X) * 100f), new GUILayoutOption[0]);
				GUILayout.Label("RStick Axis-Y : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.RStick_Y) * 100f), new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(140f) });
				GUILayout.Label("Pad-X : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.Pad_X) * 100f), new GUILayoutOption[0]);
				GUILayout.Label("Pad-Y : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.Pad_Y) * 100f), new GUILayoutOption[0]);
				GUILayout.Label("LTriger : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.LTriger) * 100f), new GUILayoutOption[0]);
				GUILayout.Label("RTriger : " + string.Format("{0,6:F2}", Input.GetAxisRaw("Joy" + this.AdSaveData.ACMconfig.joystickNumber + "Axis" + this.AdSaveData.ACMconfig.RTriger) * 100f), new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.toggle_JoystickOptionSave = GUILayout.Button("Save", new GUILayoutOption[0]);
				this.toggle_JoystickOptionLoad = GUILayout.Button("Load", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUI.DragWindow();
			};
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00027660 File Offset: 0x00025860
		private void ToggleIndent(string text, float w, ref bool flag, Action func)
		{
			flag = GUILayout.Toggle(flag, text, new GUILayoutOption[0]);
			if (flag)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(w) });
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				func();
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x000276C4 File Offset: 0x000258C4
		private void SliderandTextField(string Index, ref string InputString, ref string TextTemp, float Min, float Max, string ConrolName, int digit = 7, bool enableclamp = true, float sliderWidth = 150f)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Index, new GUILayoutOption[] { GUILayout.Height(20f) });
			float num = float.Parse(InputString);
			GUILayout.FlexibleSpace();
			num = GUILayout.HorizontalSlider(num, Min, Max, new GUILayoutOption[]
			{
				GUILayout.Height(20f),
				GUILayout.Width(sliderWidth)
			});
			if (GUI.GetNameOfFocusedControl() != ConrolName)
			{
				float num2;
				if (!float.TryParse(TextTemp, out num2) && TextTemp != InputString)
				{
					TextTemp = InputString;
				}
				if (InputString == TextTemp)
				{
					TextTemp = Convert.ToString(num);
				}
			}
			else if (Event.current.Equals(Event.KeyboardEvent("Return")))
			{
				GUI.FocusControl("");
			}
			GUI.SetNextControlName(ConrolName);
			TextTemp = GUILayout.TextField(TextTemp, digit, new GUILayoutOption[]
			{
				GUILayout.Height(20f),
				GUILayout.Width(60f)
			});
			float num3;
			if (float.TryParse(TextTemp, out num3) && GUI.GetNameOfFocusedControl() != ConrolName)
			{
				float num4 = float.Parse(TextTemp);
				if (num4 <= Min)
				{
					TextTemp = Convert.ToString(Min);
				}
				else if (num4 >= Max && enableclamp)
				{
					TextTemp = Convert.ToString(Max);
				}
				InputString = TextTemp;
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0002780C File Offset: 0x00025A0C
		private void ClampTextField(string Index, ref string InputString, ref string TextTemp, float Min, float Max, string ConrolName, int digit = 7, bool enableclamp = true)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Index, new GUILayoutOption[] { GUILayout.Height(20f) });
			GUILayout.FlexibleSpace();
			if (GUI.GetNameOfFocusedControl() != ConrolName)
			{
				float num;
				if (!float.TryParse(TextTemp, out num) && TextTemp != InputString)
				{
					TextTemp = InputString;
				}
			}
			else if (Event.current.Equals(Event.KeyboardEvent("Return")))
			{
				GUI.FocusControl("");
			}
			GUI.SetNextControlName(ConrolName);
			TextTemp = GUILayout.TextField(TextTemp, digit, new GUILayoutOption[]
			{
				GUILayout.Height(20f),
				GUILayout.Width(60f)
			});
			float num2;
			if (float.TryParse(TextTemp, out num2) && GUI.GetNameOfFocusedControl() != ConrolName)
			{
				float num3 = float.Parse(TextTemp);
				if (num3 <= Min && enableclamp)
				{
					TextTemp = Convert.ToString(Min);
				}
				else if (num3 >= Max && enableclamp)
				{
					TextTemp = Convert.ToString(Max);
				}
				InputString = TextTemp;
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00027918 File Offset: 0x00025B18
		private void ClampTextField2(string Index, ref string InputString, ref string TextTemp, float Min, float Max, string ConrolName, int digit = 7, bool enableclamp = true)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Index, new GUILayoutOption[] { GUILayout.Height(20f) });
			GUILayout.FlexibleSpace();
			if (GUI.GetNameOfFocusedControl() == ConrolName && Event.current.Equals(Event.KeyboardEvent("Return")))
			{
				GUI.FocusControl("");
			}
			GUI.SetNextControlName(ConrolName);
			TextTemp = GUILayout.TextField(InputString, digit, new GUILayoutOption[]
			{
				GUILayout.Height(20f),
				GUILayout.Width(60f)
			});
			if (GUI.GetNameOfFocusedControl() == ConrolName)
			{
				float num;
				if (float.TryParse(TextTemp, out num))
				{
					float num2 = float.Parse(TextTemp);
					if (num2 <= Min && enableclamp)
					{
						TextTemp = Convert.ToString(Min);
					}
					else if (num2 >= Max && enableclamp)
					{
						TextTemp = Convert.ToString(Max);
					}
					InputString = TextTemp;
				}
				else
				{
					if (TextTemp == "")
					{
						InputString = "";
					}
					TextTemp = InputString;
				}
			}
			else if (InputString == "")
			{
				InputString = "0";
			}
			float num3;
			if (float.TryParse(TextTemp, out num3) && GUI.GetNameOfFocusedControl() != ConrolName)
			{
				float num4 = float.Parse(TextTemp);
				if (num4 <= Min && enableclamp)
				{
					TextTemp = Convert.ToString(Min);
				}
				else if (num4 >= Max && enableclamp)
				{
					TextTemp = Convert.ToString(Max);
				}
				InputString = TextTemp;
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00027A8C File Offset: 0x00025C8C
		private void ClampTextField_Int(string Index, ref string InputString, ref string TextTemp, int Min, int Max, string ConrolName, int digit = 7, bool enableclamp = true)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(Index, new GUILayoutOption[] { GUILayout.Height(20f) });
			GUILayout.FlexibleSpace();
			if (GUI.GetNameOfFocusedControl() != ConrolName)
			{
				int num;
				if (!int.TryParse(TextTemp, out num) && TextTemp != InputString)
				{
					TextTemp = InputString;
				}
			}
			else if (Event.current.Equals(Event.KeyboardEvent("Return")))
			{
				GUI.FocusControl("");
			}
			GUI.SetNextControlName(ConrolName);
			TextTemp = GUILayout.TextField(TextTemp, digit, new GUILayoutOption[]
			{
				GUILayout.Height(20f),
				GUILayout.Width(60f)
			});
			int num2;
			if (int.TryParse(TextTemp, out num2) && GUI.GetNameOfFocusedControl() != ConrolName)
			{
				int num3 = int.Parse(TextTemp);
				if (num3 <= Min && enableclamp)
				{
					TextTemp = Convert.ToString(Min);
				}
				else if (num3 >= Max && enableclamp)
				{
					TextTemp = Convert.ToString(Max);
				}
				InputString = Convert.ToString(num3);
			}
			GUILayout.EndHorizontal();
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00027B9C File Offset: 0x00025D9C
		private Texture2D MakeColorField(int w, int h, Color col)
		{
			Color[] array = new Color[w * h];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = col;
			}
			Texture2D texture2D = new Texture2D(w, h);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00027BDC File Offset: 0x00025DDC
		public SkyBoxChanger()
		{
			this.SkySettingContainer = new Dictionary<int, float[]>
			{
				{
					0,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					1,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					2,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 4f }
				},
				{
					3,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					4,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					5,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					6,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					7,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					8,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				},
				{
					9,
					new float[] { 6000f, 0.0005f, 160f, 1.3f, 2.5f }
				}
			};
			this.SkySetting02Container = new Dictionary<int, float[]>
			{
				{
					0,
					new float[] { 6000f, 0.0003f, 160f, 0.03f, -150f }
				},
				{
					1,
					new float[] { 6000f, 0.0003f, 160f, 0.03f, -150f }
				},
				{
					2,
					new float[] { 6000f, 0.001f, 160f, 0.06f, -105.1f }
				},
				{
					3,
					new float[] { 6000f, 0.001f, 160f, 0.06f, -105.1f }
				},
				{
					4,
					new float[] { 6000f, 0.001f, 160f, 0.06f, -105.1f }
				},
				{
					5,
					new float[] { 6000f, 0.001f, 160f, 0.06f, -105.1f }
				},
				{
					6,
					new float[] { 6000f, 0.0003f, 160f, 0.03f, -150f }
				},
				{
					7,
					new float[] { 6000f, 0.001f, 160f, 0.06f, -105.1f }
				},
				{
					8,
					new float[] { 6000f, 0.001f, 160f, 0.06f, -105.1f }
				},
				{
					9,
					new float[] { 6000f, 0.0003f, 160f, 0.03f, -150f }
				}
			};
			this.SkySetting03Container = new Dictionary<int, float[]>
			{
				{
					0,
					new float[] { 1f, 1f, 1f }
				},
				{
					1,
					new float[] { 0.898f, 0.98f, 1f }
				},
				{
					2,
					new float[] { 0.95f, 0.8f, 0.75f }
				},
				{
					3,
					new float[] { 0.2f, 0.2f, 0.25f }
				},
				{
					4,
					new float[] { 0.898f, 0.98f, 1f }
				},
				{
					5,
					new float[] { 0.95f, 0.92f, 0.88f }
				},
				{
					6,
					new float[] { 0.898f, 0.98f, 1f }
				},
				{
					7,
					new float[] { 0.95f, 0.85f, 0.75f }
				},
				{
					8,
					new float[] { 0.3f, 0.3f, 0.33f }
				},
				{
					9,
					new float[] { 0.898f, 0.98f, 1f }
				}
			};
			this.Init = false;
			this.window_id = ModUtility.GetWindowId();
			this.window_id2 = ModUtility.GetWindowId();
			this.window_id3 = ModUtility.GetWindowId();
			this.windowRect = new Rect(20f, 240f, 400f, 600f);
			this.windowRect2 = new Rect(20f, 240f, 350f, 60f);
			this.windowRect3 = new Rect(425f, 240f, 300f, 530f);
			this.hide = false;
			this.Keyhide = true;
			this.Keyhide2 = true;
			this.Keyhide3 = true;
			this.windowHeight = 600f;
			this.posi_x = new float[10];
			this.posi_y = new float[10];
			this.posi_z = new float[10];
			this.sliderValue = 5f;
			this.sliderValue1 = 0.03f;
			this.sliderValue2 = 1.1f;
			this.sliderValue3 = 0.9f;
			this.sliderValue4 = 0.015f;
			this.sliderValue5 = 0f;
			this.sliderValue6 = 0f;
			this.indentflag = false;
			this.indentflag2 = false;
			this.indentflag3 = false;
			this.indentflag4 = false;
			this.indentflag5 = false;
			this.selector = 20;
			this.changeflag = false;
			this.networkflag = false;
			this.networkwaterEnable = false;
			this.networkwaterDisable = false;
			this.networkMarkerOptionflag = false;
			this.networkWaterSplashOptionflag = false;
			this.toggle_skyboxApply = false;
			this.toggle_waterApply = false;
			this.toggle_waterDisable = false;
			this.FogValue = new string[] { "0.0", "0.0", "0.0", "0.0", "0.0" };
			this.FogValue_temp = new string[] { "0.0", "0.0", "0.0", "0.0", "0.0" };
			this.SkyColorValue = new string[] { "0.0", "0.0", "0.0", "1.0" };
			this.SkyColorValue_temp = new string[] { "0.0", "0.0", "0.0", "1.0" };
			this.WaterValue = new string[]
			{
				"0.1", "0.5", "0.4", "5.0", "0.003", "0.6", "0.95", "21.0", "13.0", "-10.0",
				"-7.0", "0.035", "3.5", "0.5"
			};
			this.WaterValue_temp = new string[]
			{
				"0.1", "0.5", "0.4", "5.0", "0.003", "0.6", "0.95", "21.0", "13.0", "-10.0",
				"-7.0", "0.035", "3.5", "0.5"
			};
			this.WaterFogValue = new string[] { "1000.0", "0.8", "0.9" };
			this.WaterFogValue_temp = new string[] { "1000.0", "0.8", "0.9" };
			this.WaterRefrColor = new string[] { "0.76", "0.9", "1.0", "1.0" };
			this.WaterFadeColor = new string[] { "0.12", "0.31", "0.43", "1.0" };
			this.WaterFogColor = new string[] { "0.42", "0.62", "0.77", "1.0" };
			this.InWaterRefColor = new string[] { "0.88", "1.0", "1.0", "0.1" };
			this.InWaterFarColor = new string[] { "0.15", "0.6", "0.6", "1.0" };
			this.WaterRefrColor_temp = new string[] { "0.76", "0.9", "1.0", "1.0" };
			this.WaterFadeColor_temp = new string[] { "0.12", "0.31", "0.43", "1.0" };
			this.WaterFogColor_temp = new string[] { "0.42", "0.62", "0.77", "1.0" };
			this.InWaterRefColor_temp = new string[] { "0.88", "1.0", "1.0", "0.1" };
			this.InWaterFarColor_temp = new string[] { "0.15", "0.6", "0.6", "1.0" };
			this.SkyColorGUI = new GUIStyle();
			this.WaterRefrColorGUI = new GUIStyle();
			this.WaterFadeColorGUI = new GUIStyle();
			this.WaterFogColorGUI = new GUIStyle();
			this.InWaterRefColorGUI = new GUIStyle();
			this.InWaterFarColorGUI = new GUIStyle();
			this.guiScroll = Vector2.zero;
			this.SkyColorTex = new Texture2D(1, 1, (TextureFormat)5, false);
			this.WaterRefrColorTex = new Texture2D(1, 1, (TextureFormat)4, false);
			this.WaterFadeColorTex = new Texture2D(1, 1, (TextureFormat)5, false);
			this.WaterFogColorTex = new Texture2D(1, 1, (TextureFormat)5, false);
			this.InWaterRefColorTex = new Texture2D(1, 1, (TextureFormat)5, false);
			this.InWaterFarColorTex = new Texture2D(1, 1, (TextureFormat)5, false);
			this.barrenChecker = true;
			this.skyChecker = false;
			this.waterChecker = false;
			this.markerOptionChecker = false;
			this.waterSplashchecker = false;
			this.skyCallback = false;
			this.waterCallback = false;
			this.markerCallback = false;
			this.waterSplashCallback = false;
			this.WaterObjectenable = false;
			this.WaterEnable = false;
			this.WaterHeight = 5f;
			this.WaterBouncyEnable = false;
			this.WaterBouncyEnableSwitch = false;
			this.WaterBouncyDebug = false;
			this.WaterBulletBouncyCheck = false;
			this.FloorDeactiveSwitch = false;
			this.FloorDeactive = false;
			this.ExpandFloorSwitch = false;
			this.ExpandFloor = false;
			this.ExExpandFloorSwitch = false;
			this.ExExpandFloor = false;
			this.toggle_JoystickOptionSave = false;
			this.toggle_JoystickOptionSave_hold = false;
			this.toggle_JoystickOptionLoad = false;
			this.toggle_JoystickOptionLoad_hold = false;
			this.ModuleCollisionMode_CD = true;
			this.ModuleCollisionModeSwitch_CD = true;
			this.ModuleCollisionModeCheck = false;
			this.toggle_TargetMarkerDisable = false;
			this.TargetMarkerDisable_pre = false;
			this.toggle_WaterSplashEnable = false;
			this.WaterSplashEnable_pre = false;
			this.maxPlayersValue = new string[] { "10", "16" };
			this.maxPlayersValue_temp = new string[] { "10", "16" };
			this.BoundarySize = 4000f;
			this.debug = false;
		}

		// Token: 0x04000554 RID: 1364

		// Token: 0x04000557 RID: 1367
		public Dictionary<int, float[]> SkySettingContainer;

		// Token: 0x04000558 RID: 1368
		public Dictionary<int, float[]> SkySetting02Container;

		// Token: 0x04000559 RID: 1369
		public Dictionary<int, float[]> SkySetting03Container;

		// Token: 0x0400055A RID: 1370
		private bool Init;

		// Token: 0x0400055B RID: 1371

		// Token: 0x0400055C RID: 1372

		// Token: 0x0400055D RID: 1373

		// Token: 0x0400055E RID: 1374
		private int window_id;

		// Token: 0x0400055F RID: 1375
		private int window_id2;

		// Token: 0x04000560 RID: 1376
		private int window_id3;

		// Token: 0x04000561 RID: 1377
		private GUI.WindowFunction gui_ACMOptions;

		// Token: 0x04000562 RID: 1378
		private GUI.WindowFunction gui_NetworkOptions;

		// Token: 0x04000563 RID: 1379
		private GUI.WindowFunction gui_JoystickOptions;

		// Token: 0x04000564 RID: 1380
		public Rect windowRect;

		// Token: 0x04000565 RID: 1381
		public Rect windowRect2;

		// Token: 0x04000566 RID: 1382
		public Rect windowRect3;

		// Token: 0x04000567 RID: 1383
		public bool hide;

		// Token: 0x04000568 RID: 1384
		public bool Keyhide;

		// Token: 0x04000569 RID: 1385
		public bool Keyhide2;

		// Token: 0x0400056A RID: 1386
		public bool Keyhide3;

		// Token: 0x0400056B RID: 1387
		public float windowHeight;

		// Token: 0x0400056C RID: 1388
		public float[] posi_x;

		// Token: 0x0400056D RID: 1389
		public float[] posi_y;

		// Token: 0x0400056E RID: 1390
		public float[] posi_z;

		// Token: 0x0400056F RID: 1391
		public float sliderValue;

		// Token: 0x04000570 RID: 1392
		public float sliderValue1;

		// Token: 0x04000571 RID: 1393
		public float sliderValue2;

		// Token: 0x04000572 RID: 1394
		public float sliderValue3;

		// Token: 0x04000573 RID: 1395
		public float sliderValue4;

		// Token: 0x04000574 RID: 1396
		public float sliderValue5;

		// Token: 0x04000575 RID: 1397
		public float sliderValue6;

		// Token: 0x04000576 RID: 1398
		public bool indentflag;

		// Token: 0x04000577 RID: 1399
		public bool indentflag2;

		// Token: 0x04000578 RID: 1400
		public bool indentflag3;

		// Token: 0x04000579 RID: 1401
		public bool indentflag4;

		// Token: 0x0400057A RID: 1402
		public bool indentflag5;

		// Token: 0x0400057B RID: 1403
		public int selector;

		// Token: 0x0400057C RID: 1404
		public bool changeflag;

		// Token: 0x0400057D RID: 1405
		public bool networkflag;

		// Token: 0x0400057E RID: 1406
		public bool networkwaterEnable;

		// Token: 0x0400057F RID: 1407
		public bool networkwaterDisable;

		// Token: 0x04000580 RID: 1408
		public bool networkMarkerOptionflag;

		// Token: 0x04000581 RID: 1409
		public bool networkWaterSplashOptionflag;

		// Token: 0x04000582 RID: 1410
		public bool toggle_skyboxApply;

		// Token: 0x04000583 RID: 1411
		public bool toggle_waterApply;

		// Token: 0x04000584 RID: 1412
		public bool toggle_waterDisable;

		// Token: 0x04000585 RID: 1413
		public string[] FogValue;

		// Token: 0x04000586 RID: 1414
		public string[] FogValue_temp;

		// Token: 0x04000587 RID: 1415
		public string[] SkyColorValue;

		// Token: 0x04000588 RID: 1416
		public string[] SkyColorValue_temp;

		// Token: 0x04000589 RID: 1417
		public string[] WaterValue;

		// Token: 0x0400058A RID: 1418
		public string[] WaterValue_temp;

		// Token: 0x0400058B RID: 1419
		public string[] WaterFogValue;

		// Token: 0x0400058C RID: 1420
		public string[] WaterFogValue_temp;

		// Token: 0x0400058D RID: 1421
		public string[] WaterRefrColor;

		// Token: 0x0400058E RID: 1422
		public string[] WaterFadeColor;

		// Token: 0x0400058F RID: 1423
		public string[] WaterFogColor;

		// Token: 0x04000590 RID: 1424
		public string[] InWaterRefColor;

		// Token: 0x04000591 RID: 1425
		public string[] InWaterFarColor;

		// Token: 0x04000592 RID: 1426
		public string[] WaterRefrColor_temp;

		// Token: 0x04000593 RID: 1427
		public string[] WaterFadeColor_temp;

		// Token: 0x04000594 RID: 1428
		public string[] WaterFogColor_temp;

		// Token: 0x04000595 RID: 1429
		public string[] InWaterRefColor_temp;

		// Token: 0x04000596 RID: 1430
		public string[] InWaterFarColor_temp;

		// Token: 0x04000597 RID: 1431
		private GUIStyle SkyColorGUI;

		// Token: 0x04000598 RID: 1432
		private GUIStyle WaterRefrColorGUI;

		// Token: 0x04000599 RID: 1433
		private GUIStyle WaterFadeColorGUI;

		// Token: 0x0400059A RID: 1434
		private GUIStyle WaterFogColorGUI;

		// Token: 0x0400059B RID: 1435
		private GUIStyle InWaterRefColorGUI;

		// Token: 0x0400059C RID: 1436
		private GUIStyle InWaterFarColorGUI;

		// Token: 0x0400059D RID: 1437
		private Vector2 guiScroll;

		// Token: 0x0400059E RID: 1438
		private Texture2D SkyColorTex;

		// Token: 0x0400059F RID: 1439
		private Texture2D WaterRefrColorTex;

		// Token: 0x040005A0 RID: 1440
		private Texture2D WaterFadeColorTex;

		// Token: 0x040005A1 RID: 1441
		private Texture2D WaterFogColorTex;

		// Token: 0x040005A2 RID: 1442
		private Texture2D InWaterRefColorTex;

		// Token: 0x040005A3 RID: 1443
		private Texture2D InWaterFarColorTex;

		// Token: 0x040005A4 RID: 1444
		public GameObject FogObject;

		// Token: 0x040005A5 RID: 1445
		public GameObject SkyObject;

		// Token: 0x040005A6 RID: 1446
		public GameObject LightObject;

		// Token: 0x040005A7 RID: 1447
		public GameObject EnvironmentsObject;

		// Token: 0x040005A8 RID: 1448
		public GameObject BarrenObject;

		// Token: 0x040005A9 RID: 1449
		public GameObject IpsilonObject;

		// Token: 0x040005AA RID: 1450
		public GameObject TolbryndObject;

		// Token: 0x040005AB RID: 1451
		public GameObject MountainObject;

		// Token: 0x040005AC RID: 1452
		public GameObject BarrenEnvObject;

		// Token: 0x040005AD RID: 1453
		public GameObject DesertObject;

		// Token: 0x040005AE RID: 1454
		public GameObject FloorObject;

		// Token: 0x040005AF RID: 1455
		public GameObject BoundaryBottom;

		// Token: 0x040005B0 RID: 1456
		public bool barrenChecker;

		// Token: 0x040005B1 RID: 1457
		public bool skyChecker;

		// Token: 0x040005B2 RID: 1458
		public bool waterChecker;

		// Token: 0x040005B3 RID: 1459
		public bool markerOptionChecker;

		// Token: 0x040005B4 RID: 1460
		public bool waterSplashchecker;

		// Token: 0x040005B5 RID: 1461
		public bool skyCallback;

		// Token: 0x040005B6 RID: 1462
		public bool waterCallback;

		// Token: 0x040005B7 RID: 1463
		public bool markerCallback;

		// Token: 0x040005B8 RID: 1464
		public bool waterSplashCallback;

		// Token: 0x040005B9 RID: 1465
		public Renderer FogRenderer;

		// Token: 0x040005BA RID: 1466
		public Material SkyMaterial;

		// Token: 0x040005BB RID: 1467

		// Token: 0x040005BC RID: 1468
		public MeshFilter SkyMeshFilter;

		// Token: 0x040005BD RID: 1469
		public Light LightComponent;

		// Token: 0x040005BE RID: 1470
		public SkyBoxChanger.TerainType TerainChecker;

		// Token: 0x040005BF RID: 1471

		// Token: 0x040005C0 RID: 1472
		private GameObject AdProjectilePool;

		// Token: 0x040005C1 RID: 1473
		private GameObject ProjectilePool;

		// Token: 0x040005C2 RID: 1474

		// Token: 0x040005C3 RID: 1475
		public GameObject WaterObject;

		// Token: 0x040005C4 RID: 1476
		public Material InsideWater;

		// Token: 0x040005C5 RID: 1477
		public GameObject WaterObjectUsing;

		// Token: 0x040005C6 RID: 1478
		public GameObject[,] WaterObjectArray;

		// Token: 0x040005C7 RID: 1479
		public bool WaterObjectenable;

		// Token: 0x040005C8 RID: 1480
		public bool WaterEnable;

		// Token: 0x040005C9 RID: 1481
		public float WaterHeight;

		// Token: 0x040005CA RID: 1482
		public bool WaterBouncyEnable;

		// Token: 0x040005CB RID: 1483
		public bool WaterBouncyEnableSwitch;

		// Token: 0x040005CC RID: 1484
		public bool WaterBouncyDebug;

		// Token: 0x040005CD RID: 1485
		public bool WaterBulletBouncyCheck;

		// Token: 0x040005CE RID: 1486
		public bool FloorDeactiveSwitch;

		// Token: 0x040005CF RID: 1487
		public bool FloorDeactive;

		// Token: 0x040005D0 RID: 1488
		public bool ExpandFloorSwitch;

		// Token: 0x040005D1 RID: 1489
		public bool ExpandFloor;

		// Token: 0x040005D2 RID: 1490
		public bool ExExpandFloorSwitch;

		// Token: 0x040005D3 RID: 1491
		public bool ExExpandFloor;

		// Token: 0x040005D4 RID: 1492
		public bool toggle_JoystickOptionSave;

		// Token: 0x040005D5 RID: 1493
		public bool toggle_JoystickOptionSave_hold;

		// Token: 0x040005D6 RID: 1494
		public bool toggle_JoystickOptionLoad;

		// Token: 0x040005D7 RID: 1495
		public bool toggle_JoystickOptionLoad_hold;

		// Token: 0x040005D8 RID: 1496
		public bool ModuleCollisionMode_CD;

		// Token: 0x040005D9 RID: 1497
		public bool ModuleCollisionModeSwitch_CD;

		// Token: 0x040005DA RID: 1498
		public bool ModuleCollisionModeCheck;

		// Token: 0x040005DB RID: 1499
		public bool toggle_TargetMarkerDisable;

		// Token: 0x040005DC RID: 1500
		public bool TargetMarkerDisable_pre;

		// Token: 0x040005DD RID: 1501
		public bool toggle_WaterSplashEnable;

		// Token: 0x040005DE RID: 1502
		public bool WaterSplashEnable_pre;

		// Token: 0x040005DF RID: 1503
		public string[] maxPlayersValue;

		// Token: 0x040005E0 RID: 1504
		public string[] maxPlayersValue_temp;

		// Token: 0x040005E1 RID: 1505
		public float BoundarySize;

		// Token: 0x040005E2 RID: 1506
		public AdNetworkBlock[] adNetworkblocks;

		// Token: 0x040005E4 RID: 1508
		public AdDataHolder AdSaveData;

		// Token: 0x040005E5 RID: 1509
		private string temp_string;

		// Token: 0x040005E6 RID: 1510
		private bool debug;

		// Token: 0x040005E7 RID: 1511
		public float scale;

		// Token: 0x02000070 RID: 112
		public enum TerainType
		{
			// Token: 0x040005E9 RID: 1513
			Barren,
			// Token: 0x040005EA RID: 1514
			ipsilon,
			// Token: 0x040005EB RID: 1515
			Tolbrynd,
			// Token: 0x040005EC RID: 1516
			MountainTop,
			// Token: 0x040005ED RID: 1517
			Desert
		}
	}
}
