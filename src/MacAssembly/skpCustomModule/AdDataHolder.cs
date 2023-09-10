using System;
using System.Collections.Generic;
using Modding;
using Modding.Common;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000005 RID: 5
	public class AdDataHolder : MonoBehaviour
	{
		// Token: 0x06000014 RID: 20 RVA: 0x000021AB File Offset: 0x000003AB
		private void Awake()
		{
			this.ACMconfig = this.Loadingdata();
			this.Init = true;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000021C1 File Offset: 0x000003C1
		public void LoadData()
		{
			this.ACMconfig = this.Loadingdata();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00004BC4 File Offset: 0x00002DC4
		public void Update()
		{
			float num = float.Parse(this.ACMconfig.deadZone);
			this.JoyAxisData.LStick_X = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.LStick_X) * this.scale;
			this.JoyAxisData.LStick_X = ((Mathf.Abs(this.JoyAxisData.LStick_X) - num > 0f) ? ((this.JoyAxisData.LStick_X - Mathf.Sign(this.JoyAxisData.LStick_X) * num) / (10f - num) * 10f) : 0f);
			this.JoyAxisData.LStick_Y = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.LStick_Y) * this.scale;
			this.JoyAxisData.LStick_Y = ((Mathf.Abs(this.JoyAxisData.LStick_Y) - num > 0f) ? ((this.JoyAxisData.LStick_Y - Mathf.Sign(this.JoyAxisData.LStick_Y) * num) / (10f - num) * 10f) : 0f);
			this.JoyAxisData.RStick_X = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.RStick_X) * this.scale;
			this.JoyAxisData.RStick_Y = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.RStick_Y) * this.scale;
			this.JoyAxisData.Pad_X = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.Pad_X) * this.scale;
			this.JoyAxisData.Pad_Y = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.Pad_Y) * this.scale;
			this.JoyAxisData.LTriger = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.LTriger) * this.scale;
			bool ds4trigermode = this.ACMconfig.DS4trigermode;
			if (ds4trigermode)
			{
				this.JoyAxisData.LTriger = (this.JoyAxisData.LTriger + this.scale / 10f) / 2f;
			}
			this.JoyAxisData.RTriger = Input.GetAxisRaw("Joy" + this.ACMconfig.joystickNumber + "Axis" + this.ACMconfig.RTriger) * this.scale;
			bool ds4trigermode2 = this.ACMconfig.DS4trigermode;
			if (ds4trigermode2)
			{
				this.JoyAxisData.RTriger = (this.JoyAxisData.RTriger + this.scale / 10f) / 2f;
			}
			bool levelSimulating = StatMaster.levelSimulating;
			if (levelSimulating)
			{
				bool isMP = StatMaster.isMP;
				if (isMP)
				{
					this.NetworkID = (int)Player.GetLocalPlayer().NetworkId;
					bool flag = this.JoyAxisContainer.ContainsKey(this.NetworkID);
					if (flag)
					{
						this.JoyAxisContainer[this.NetworkID] = this.JoyAxisData;
					}
					else
					{
						this.JoyAxisContainer.Add(this.NetworkID, this.JoyAxisData);
					}
					bool isClient = StatMaster.isClient;
					if (isClient)
					{
						float deltaTime = TimeSlider.Instance.deltaTime;
						float sendRate = NetworkScene.ServerSettings.sendRate;
						this.lastUpdate += deltaTime;
						bool flag2 = this.lastUpdate >= sendRate;
						if (flag2)
						{
							while (this.lastUpdate >= sendRate)
							{
								this.lastUpdate -= sendRate;
							}
							ModNetworking.SendToHost(AdCustomModuleMod.msgJoyStickData.CreateMessage(new object[]
							{
								this.NetworkID,
								this.JoyAxisData.LStick_X,
								this.JoyAxisData.LStick_Y,
								this.JoyAxisData.RStick_X,
								this.JoyAxisData.RStick_Y,
								this.JoyAxisData.Pad_X,
								this.JoyAxisData.Pad_Y,
								this.JoyAxisData.LTriger,
								this.JoyAxisData.RTriger
							}));
						}
					}
				}
				else
				{
					this.NetworkID = 0;
					bool flag3 = this.JoyAxisContainer.ContainsKey(this.NetworkID);
					if (flag3)
					{
						this.JoyAxisContainer[this.NetworkID] = this.JoyAxisData;
					}
					else
					{
						this.JoyAxisContainer.Add(this.NetworkID, this.JoyAxisData);
					}
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000050EC File Offset: 0x000032EC
		public AdData Loadingdata()
		{
			AdData adData = new AdData();
			AdData result;
			try
			{
				
				bool flag =Modding.ModIO.ExistsFile("Config.xml", true);
				if (flag)
				{
					adData = Modding.ModIO.DeserializeXml<AdData>("Config.xml", true, false);
					result = adData;
				}
				else
				{
					Debug.Log("ACM : Config.xml can not found.new Create.");
					this.SaveData();
					result = adData;
				}
			}
			catch (Exception ex)
			{
				Debug.Log("ACM : Config.xml is error2");
				Debug.Log(ex.Message);
				this.SaveData();
				result = adData;
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000021D0 File Offset: 0x000003D0
		public void SaveData()
		{
            Modding.ModIO.SerializeXml<AdData>(this.ACMconfig ?? new AdData(), "Config.xml", true);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00005170 File Offset: 0x00003370
		public double baseN(int i, int k, double t, int[] nv)
		{
			double num = 0.0;
			double num2 = 0.0;
			bool flag = k == 1;
			double result;
			if (flag)
			{
				bool flag2 = t >= (double)nv[i] && t < (double)nv[i + 1];
				if (flag2)
				{
					result = 1.0;
				}
				else
				{
					result = 0.0;
				}
			}
			else
			{
				bool flag3 = nv[i + k] - nv[i + 1] != 0;
				if (flag3)
				{
					num = ((double)nv[i + k] - t) / (double)(nv[i + k] - nv[i + 1]) * this.baseN(i + 1, k - 1, t, nv);
				}
				bool flag4 = nv[i + k - 1] - nv[i] != 0;
				if (flag4)
				{
					num2 = (t - (double)nv[i]) / (double)(nv[i + k - 1] - nv[i]) * this.baseN(i, k - 1, t, nv);
				}
				result = num + num2;
			}
			return result;
		}

		// Token: 0x04000039 RID: 57
		public AdData ACMconfig;

		// Token: 0x0400003A RID: 58
		public JoystickAxisInfo JoyAxisData;

		// Token: 0x0400003B RID: 59
		public Dictionary<int, JoystickAxisInfo> JoyAxisContainer = new Dictionary<int, JoystickAxisInfo>();

		// Token: 0x0400003C RID: 60
		private float lastUpdate = 0f;

		// Token: 0x0400003D RID: 61
		private float scale = 100f;

		// Token: 0x0400003E RID: 62
		private int NetworkID ;

		// Token: 0x0400003F RID: 63
		public bool Init = false;
	}
}
