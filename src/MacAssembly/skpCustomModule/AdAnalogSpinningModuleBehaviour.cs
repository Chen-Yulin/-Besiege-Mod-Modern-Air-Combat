using System;
using System.Collections.Generic;
using Modding.Modules;
using Modding.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x0200001C RID: 28
	public class AdAnalogSpinningModuleBehaviour : BlockModuleBehaviour<AdAnalogSpinningModule>
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000AD RID: 173 RVA: 0x000024BA File Offset: 0x000006BA
		private float FlipInvert
		{
			get
			{
				return (!base.Flipped) ? 1f : (-1f);
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0001006C File Offset: 0x0000E26C
		public override void SafeAwake()
		{
			try
			{
				this.JoyMenu = base.AddMenu("joy-menu", 0, this.joyList, false);
				this.BrakeKey = base.GetKey(base.Module.BrakeKey);
				this.BackGearKey = base.GetKey(base.Module.BackGearKey);
				this.SpeedSlider = base.GetSlider(base.Module.SpeedSlider);
				this.angleSlider = base.GetSlider(base.Module.XYAxisSlider);
				this.AccelerationSlider = base.GetSlider(base.Module.AccelerationSlider);
				this.DecelerationSlider = base.GetSlider(base.Module.DecelerationSlider);
				this.BrakePowerSlider = base.GetSlider(base.Module.BrakePowerSlider);
			}
			catch (Exception ex)
			{
				Debug.Log("Could not get all mapper types for Spinning Module! Module will be disabled.");
				Debug.Log(ex.ToString());
				Object.Destroy(this);
				return;
			}
			bool flag = !base.IsStripped;
			if (flag)
			{
				this.myJoint = base.GetComponent<ConfigurableJoint>();
				this.SetConvertedAxis();
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0001018C File Offset: 0x0000E38C
		public override void OnSimulateStart()
		{
			bool simPhysics = base.SimPhysics;
			if (simPhysics)
			{
				bool hasRigidbody = base.HasRigidbody;
				if (hasRigidbody)
				{
					base.Rigidbody.maxAngularVelocity = base.Module.MaxAngularSpeed;
				}
				this.myJoint.angularXMotion = this.GetJointMotion((Modding.Serialization.Direction)0);
				this.myJoint.angularYMotion = this.GetJointMotion((Modding.Serialization.Direction)1);
				this.myJoint.angularZMotion = this.GetJointMotion((Modding.Serialization.Direction)2);
				JointDrive angularXDrive = this.myJoint.angularXDrive;
				angularXDrive.positionDamper = 100000f;
				angularXDrive.positionSpring = 50f;
				this.myJoint.angularXDrive = angularXDrive;
				JointDrive angularYZDrive = this.myJoint.angularYZDrive;
				angularYZDrive.positionDamper = 100000f;
				angularYZDrive.positionSpring = 50f;
				this.myJoint.angularYZDrive = angularYZDrive;
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

		// Token: 0x060000B0 RID: 176 RVA: 0x0001029C File Offset: 0x0000E49C
		private ConfigurableJointMotion GetJointMotion(Direction dir)
		{
			return (ConfigurableJointMotion)((dir == this.convertedAxis) ? 2 : 0);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000102BC File Offset: 0x0000E4BC
		private void SetConvertedAxis()
		{
			bool flag = (int)base.Module.Axis == 1;
			if (flag)
			{
				this.convertedAxis = (Direction)2;
			}
			else
			{
				bool flag2 = (int)base.Module.Axis == 2;
				if (flag2)
				{
					this.convertedAxis = (Direction)1;
				}
				else
				{
					this.convertedAxis = base.Module.Axis;
				}
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00010318 File Offset: 0x0000E518
		public override void OnReload()
		{
			bool hasRigidbody = base.HasRigidbody;
			if (hasRigidbody)
			{
				base.Rigidbody.maxAngularVelocity = base.Module.MaxAngularSpeed;
			}
			this.SetConvertedAxis();
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00010350 File Offset: 0x0000E550
		public override void KeyEmulationUpdate()
		{
			bool simPhysics = base.SimPhysics;
			if (simPhysics)
			{
				this.emuBrakePressed = this.BrakeKey.EmulationPressed();
				this.emuBrakeHold = this.BrakeKey.EmulationHeld(true);
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00010390 File Offset: 0x0000E590
		public override void SimulateFixedUpdateHost()
		{
			bool flag = base.IsDestroyed || this.myJoint == null;
			if (!flag)
			{
				float num = this.SpeedSlider.Value * this.WheelEquivalenceMultiplier;
				float num2 = this.AccelerationSlider.Value * 10f;
				float num3 = this.DecelerationSlider.Value * 10f;
				float num4 = this.BrakePowerSlider.Value * 10f;
				Rigidbody connectedBody = this.myJoint.connectedBody;
				bool flag2 = connectedBody != null;
				bool flag3 = (!flag2 || !connectedBody.isKinematic || base.HasRigidbody || !base.Rigidbody.isKinematic) && num != 0f;
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
					bool isPressed = this.BackGearKey.IsPressed;
					if (isPressed)
					{
						bool flag6 = !this.keyhold;
						if (flag6)
						{
							this.keyhold = true;
							bool flag7 = this.backGear == 1f;
							if (flag7)
							{
								this.backGear = -1f;
							}
							else
							{
								this.backGear = 1f;
							}
						}
					}
					else
					{
						this.keyhold = false;
					}
					bool flag8 = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer.ContainsKey(this.PlayerID);
					if (flag8)
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
										float num5 = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].Pad_X;
										float num6 = -AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].Pad_Y;
										double num7 = Math.Sqrt(Math.Pow((double)num5, 2.0) + Math.Pow((double)num6, 2.0));
										double num8 = Math.Atan2((double)num5, (double)num6);
										double d = num8 - (double)this.angleSlider.Value * 3.141592653589793 / 180.0;
										double num9 = Math.Cos(d) * num7;
										this.input = (float)num9;
									}
								}
								else
								{
									float num5 = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].RStick_X;
									float num6 = -AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].RStick_Y;
									double num7 = Math.Sqrt(Math.Pow((double)num5, 2.0) + Math.Pow((double)num6, 2.0));
									double num8 = Math.Atan2((double)num5, (double)num6);
									double d = num8 - (double)this.angleSlider.Value * 3.141592653589793 / 180.0;
									double num9 = Math.Cos(d) * num7;
									this.input = (float)num9;
								}
							}
							else
							{
								float num5 = AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].LStick_X;
								float num6 = -AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[this.PlayerID].LStick_Y;
								double num7 = Math.Sqrt(Math.Pow((double)num5, 2.0) + Math.Pow((double)num6, 2.0));
								double num8 = Math.Atan2((double)num5, (double)num6);
								double d = num8 - (double)this.angleSlider.Value * 3.141592653589793 / 180.0;
								double num9 = Math.Cos(d) * num7;
								this.input = (float)num9;
							}
						}
					}
					else
					{
						this.input = 0f;
					}
					float num10 = this.FlipInvert * this.backGear;
					float axisComponent = DirectionExtensions.GetAxisComponent(this.convertedAxis, this.myJoint.targetAngularVelocity);
					float num11 = ((this.input >= 0f) ? (Time.deltaTime * this.input * num2 * num10) : 0f);
					float num12 = ((this.input < 0f) ? (Time.deltaTime * this.input * num4) : 0f);
					float num13 = (Time.deltaTime * num3 - num12) * Mathf.Sign(axisComponent);
					bool flag9 = Mathf.Abs(axisComponent) < Mathf.Abs(num13);
					if (flag9)
					{
						num13 = axisComponent;
					}
					float num14 = axisComponent + num11 - num13;
					float num15 = Mathf.Clamp(num14, 0f - num, num);
					float num16 = Mathf.Abs(axisComponent) - Mathf.Abs(num15);
					bool flag10 = this.BrakeKey.Value == 1f;
					if (flag10)
					{
						num15 = 0f;
					}
					this.myJoint.targetAngularVelocity = num15 * DirectionExtensions.ToAxisVector(this.convertedAxis);
				}
			}
		}

		// Token: 0x0400016E RID: 366
		private MMenu JoyMenu;

		// Token: 0x0400016F RID: 367
		private List<string> joyList = new List<string> { "L-Stick", "R-Stick", "Pad", "L-Triger", "R-Triger", "LRmix-Tri" };

		// Token: 0x04000170 RID: 368
		public MKey BrakeKey;

		// Token: 0x04000171 RID: 369
		public MKey BackGearKey;

		// Token: 0x04000172 RID: 370
		public MSlider SpeedSlider;

		// Token: 0x04000173 RID: 371
		private MSlider angleSlider;

		// Token: 0x04000174 RID: 372
		public MSlider AccelerationSlider;

		// Token: 0x04000175 RID: 373
		public MSlider DecelerationSlider;

		// Token: 0x04000176 RID: 374
		public MSlider BrakePowerSlider;

		// Token: 0x04000177 RID: 375
		private float WheelEquivalenceMultiplier = 10.4f;

		// Token: 0x04000178 RID: 376
		private Direction convertedAxis;

		// Token: 0x04000179 RID: 377
		private ConfigurableJoint myJoint;

		// Token: 0x0400017A RID: 378
		private float backGear = 1f;

		// Token: 0x0400017B RID: 379
		private bool keyhold = false;

		// Token: 0x0400017C RID: 380
		private bool emuBrakePressed;

		// Token: 0x0400017D RID: 381
		private bool emuBrakeHold;

		// Token: 0x0400017E RID: 382
		private int PlayerID;

		// Token: 0x0400017F RID: 383
		private float input;
	}
}
