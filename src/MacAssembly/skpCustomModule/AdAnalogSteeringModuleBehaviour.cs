using System;
using System.Collections.Generic;
using Modding.Modules;
using Modding.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace skpCustomModule
{
	// Token: 0x0200001D RID: 29
	public class AdAnalogSteeringModuleBehaviour : BlockModuleBehaviour<AdAnalogSteeringModule>
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000024BA File Offset: 0x000006BA
		private float FlipInvert
		{
			get
			{
				return (!base.Flipped) ? 1f : (-1f);
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00010A54 File Offset: 0x0000EC54
		public override void OnReload()
		{
			bool hasRigidbody = base.HasRigidbody;
			if (hasRigidbody)
			{
				base.Rigidbody.maxAngularVelocity = base.Module.MaxAngularSpeed;
			}
			this.limits.MaxValue = base.Module.LimitsHighestAngle;
			this.limits.iconInfo = base.Module.LimitsDisplay.ToFauxTransform();
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00010AB8 File Offset: 0x0000ECB8
		public override void SafeAwake()
		{
			bool flag = base.IsSimulating && !base.SimPhysics;
			if (!flag)
			{
				try
				{
					this.JoyMenu = base.AddMenu("joy-menu", 0, this.joyList, false);
					this.modeMenu = base.AddMenu("mode-menu", 0, this.modeList, false);
					this.speedSlider = base.GetSlider(base.Module.SpeedSlider);
					this.angleSlider = base.GetSlider(base.Module.XYAxisSlider);
					this.limits = base.AddLimits("Limits", "steering-limits", base.Module.LimitsDefaultMin, base.Module.LimitsDefaultMax, base.Module.LimitsHighestAngle, base.Module.LimitsDisplay.ToFauxTransform());
				}
				catch (Exception ex)
				{
					Debug.Log("Could not get all mapper types for AdAnalogSteering Module! Module will be disabled.");
					Debug.Log(ex.ToString());
					Object.Destroy(this);
					return;
				}
				bool flag2 = !base.IsStripped;
				if (flag2)
				{
					this.myJoint = base.GetComponent<ConfigurableJoint>();
					switch ((int)base.Module.Axis)
					{
					case 0:
						this.myJoint.angularXMotion = (ConfigurableJointMotion)2;
						this.myJoint.angularYMotion = 0;
						this.myJoint.angularZMotion = 0;
						this.axis = new Vector3(1f, 0f, 0f);
						break;
					case 1:
						this.myJoint.angularXMotion = 0;
						this.myJoint.angularYMotion = (ConfigurableJointMotion)2;
						this.myJoint.angularZMotion = 0;
						this.axis = new Vector3(0f, 1f, 0f);
						break;
					case 2:
						this.myJoint.angularXMotion = 0;
						this.myJoint.angularYMotion = 0;
						this.myJoint.angularZMotion = (ConfigurableJointMotion)2;
						this.axis = new Vector3(0f, 0f, 1f);
						break;
					}
				}
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00010CE8 File Offset: 0x0000EEE8
		public override void OnSimulateStart()
		{
			bool flag = base.IsSimulating && base.SimPhysics;
			if (flag)
			{
				bool hasRigidbody = base.HasRigidbody;
				if (hasRigidbody)
				{
					base.Rigidbody.maxAngularVelocity = base.Module.MaxAngularSpeed;
				}
				JointDrive angularYZDrive = this.myJoint.angularYZDrive;
				JointDrive angularXDrive = this.myJoint.angularXDrive;
				float num = (angularYZDrive.positionDamper = (angularXDrive.positionDamper = 50f));
				num = (angularYZDrive.positionSpring = (angularXDrive.positionSpring = 100000f));
				this.myJoint.angularYZDrive = angularYZDrive;
				this.myJoint.angularXDrive = angularXDrive;
				this.myJoint.targetAngularVelocity = this.axis * 10f;
				bool isMP = StatMaster.isMP;
				if (isMP)
				{
					this.PlayerID = (int)base.Machine.Player.NetworkId;
				}
				else
				{
					this.PlayerID = 0;
				}
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00010DF4 File Offset: 0x0000EFF4
		public void Update()
		{
			bool isSimulating = base.IsSimulating;
			if (!isSimulating)
			{
				bool isActive = this.limits.UseLimitsToggle.IsActive;
				if (isActive)
				{
					bool flag = !this.modeMenu.DisplayInMapper;
					if (flag)
					{
						this.modeMenu.DisplayInMapper = true;
					}
				}
				else
				{
					bool displayInMapper = this.modeMenu.DisplayInMapper;
					if (displayInMapper)
					{
						this.modeMenu.DisplayInMapper = false;
					}
				}
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00010E68 File Offset: 0x0000F068
		public override void SimulateFixedUpdateHost()
		{
			bool flag = !this.myJoint;
			if (!flag)
			{
				float value = this.speedSlider.Value;
				Rigidbody connectedBody = this.myJoint.connectedBody;
				bool flag2 = connectedBody != null;
				bool flag3 = (!flag2 || !connectedBody.isKinematic || base.HasRigidbody || !base.Rigidbody.isKinematic) && value != 0f;
				if (flag3)
				{
					bool flag4 = base.HasRigidbody && base.Rigidbody.IsSleeping();
					if (flag4)
					{
						base.Rigidbody.WakeUp();
					}
					bool flag5 = flag2 && connectedBody.IsSleeping();
					if (flag5)
					{
						connectedBody.WakeUp();
					}
					bool flag6 = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer.ContainsKey(this.PlayerID);
					if (flag6)
					{
						string selection = this.JoyMenu.Selection;
						string text = selection;
						if (text != null)
						{
							if (!(text == "L-Stick"))
							{
								if (!(text == "R-Stick"))
								{
									if (!(text == "Pad"))
									{
										if (!(text == "L-Triger"))
										{
											if (!(text == "R-Triger"))
											{
												if (text == "LRmix-Tri")
												{
													this.input = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].LTriger;
													this.input -= AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].RTriger;
												}
											}
											else
											{
												this.input = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].RTriger;
											}
										}
										else
										{
											this.input = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].LTriger;
										}
									}
									else
									{
										float num = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].Pad_X;
										float num2 = -AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].Pad_Y;
										double num3 = Math.Sqrt(Math.Pow((double)num, 2.0) + Math.Pow((double)num2, 2.0));
										double num4 = Math.Atan2((double)num, (double)num2);
										double d = num4 - (double)this.angleSlider.Value * 3.141592653589793 / 180.0;
										double num5 = Math.Cos(d) * num3;
										this.input = (float)num5;
									}
								}
								else
								{
									float num = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].RStick_X;
									float num2 = -AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].RStick_Y;
									double num3 = Math.Sqrt(Math.Pow((double)num, 2.0) + Math.Pow((double)num2, 2.0));
									double num4 = Math.Atan2((double)num, (double)num2);
									double d = num4 - (double)this.angleSlider.Value * 3.141592653589793 / 180.0;
									double num5 = Math.Cos(d) * num3;
									this.input = (float)num5;
								}
							}
							else
							{
								float num = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].LStick_X;
								float num2 = -AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].LStick_Y;
								double num3 = Math.Sqrt(Math.Pow((double)num, 2.0) + Math.Pow((double)num2, 2.0));
								double num4 = Math.Atan2((double)num, (double)num2);
								double d = num4 - (double)this.angleSlider.Value * 3.141592653589793 / 180.0;
								double num5 = Math.Cos(d) * num3;
								this.input = (float)num5;
							}
						}
					}
					else
					{
						this.input = 0f;
					}
					bool isActive = this.limits.UseLimitsToggle.IsActive;
					if (isActive)
					{
						bool flag7 = this.modeMenu.Selection == "Trace";
						if (flag7)
						{
							float num6 = ((this.input > 0f) ? (this.input * this.limits.Max / 10f) : (this.input * this.limits.Min / 10f));
							float num7 = Time.deltaTime * 100f * value;
							bool flag8 = num6 - this.angleToBe > 0f;
							if (flag8)
							{
								this.angleToBe = ((num6 - this.angleToBe > num7) ? (this.angleToBe + num7) : num6);
							}
							else
							{
								this.angleToBe = ((this.angleToBe - num6 > num7) ? (this.angleToBe - num7) : num6);
							}
							float num8 = 0f - this.limits.Min;
							float max = this.limits.Max;
							this.angleToBe = ((this.angleToBe < num8) ? num8 : ((this.angleToBe <= max) ? this.angleToBe : max));
						}
						else
						{
							float num9 = this.input * 5f;
							float num10 = Time.deltaTime * num9 * value;
							this.angleToBe += num10;
							float num11 = 0f - this.limits.Min;
							float max2 = this.limits.Max;
							this.angleToBe = ((this.angleToBe < num11) ? num11 : ((this.angleToBe <= max2) ? this.angleToBe : max2));
						}
					}
					else
					{
						float num12 = this.input * 5f;
						float num13 = Time.deltaTime * num12 * value;
						this.angleToBe += num13;
					}
					this.jointEulerRotation.x = this.axis.x * this.angleToBe * this.FlipInvert;
					this.jointEulerRotation.y = this.axis.y * this.angleToBe * this.FlipInvert;
					this.jointEulerRotation.z = this.axis.z * this.angleToBe * this.FlipInvert;
					this.myJoint.targetRotation = Quaternion.Euler(this.jointEulerRotation);
				}
			}
		}

		// Token: 0x04000180 RID: 384
		private MMenu JoyMenu;

		// Token: 0x04000181 RID: 385
		private MMenu modeMenu;

		// Token: 0x04000182 RID: 386
		private List<string> joyList = new List<string> { "L-Stick", "R-Stick", "Pad", "L-Triger", "R-Triger", "LRmix-Tri" };

		// Token: 0x04000183 RID: 387
		private List<string> modeList = new List<string> { "Trace", "Move" };

		// Token: 0x04000184 RID: 388
		private MSlider speedSlider;

		// Token: 0x04000185 RID: 389
		private MSlider angleSlider;

		// Token: 0x04000186 RID: 390
		private MLimits limits;

		// Token: 0x04000187 RID: 391
		private ConfigurableJoint myJoint;

		// Token: 0x04000188 RID: 392
		private Vector3 axis;

		// Token: 0x04000189 RID: 393
		private int PlayerID;

		// Token: 0x0400018A RID: 394
		private float input;

		// Token: 0x0400018B RID: 395
		private float angleToBe = 0f;

		// Token: 0x0400018C RID: 396
		private Vector3 jointEulerRotation = Vector3.zero;
	}
}
