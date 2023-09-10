using System;
using System.Collections.Generic;
using Modding.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x0200000C RID: 12
	public class UWaterVolumeBehaviour : MonoBehaviour
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002267 File Offset: 0x00000467
		private bool Wenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterEnable;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002273 File Offset: 0x00000473
		private bool WBenable
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterBouncyEnable;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000033 RID: 51 RVA: 0x0000228B File Offset: 0x0000048B
		private float WaterHeight
		{
			get
			{
				return AdCustomModuleMod.mod4.WaterHeight;
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00009098 File Offset: 0x00007298
		public void Awake()
		{
			bool flag = base.transform.GetComponent<Rigidbody>();
			if (flag)
			{
				this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
				bool flag2 = this.ObjectBehavior == null;
				if (flag2)
				{
					this.ObjectBehavior = base.gameObject.GetComponent<BlockBehaviour>();
				}
				bool flag3 = UWaterVolumeBehaviour.FlatYFloatList.Contains((BlockType)this.ObjectBehavior.BlockID);
				if (flag3)
				{
					this.isFlatYFloat = true;
				}
				bool flag4 = UWaterVolumeBehaviour.block_float.ContainsKey((BlockType)this.ObjectBehavior.BlockID);
				if (flag4)
				{
					this.Maxfloating = UWaterVolumeBehaviour.block_float[(BlockType)this.ObjectBehavior.BlockID];
				}
				else
				{
					this.Maxfloating = 30f;
				}
			}
			this.ScanAxis3 = new Quaternion[4];
			for (int i = 0; i < 4; i++)
			{
				this.ScanAxis3[i] = Quaternion.Euler(0f, (float)(90 * i), 0f);
			}
			this.ScanAxis2 = new Quaternion[12];
			for (int j = 0; j < 12; j++)
			{
				this.ScanAxis2[j] = Quaternion.Euler(0f, (float)(30 * j), 0f);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002297 File Offset: 0x00000497
		public void Start()
		{
			this.RaycastInterval = 0.3f;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000091E8 File Offset: 0x000073E8
		public void Update()
		{
			bool flag = !StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim || (StatMaster.isClient && StatMaster.InLocalPlayMode);
			bool flag2 = this.Wenable && this.WBenable && (StatMaster.levelSimulating && flag);
			if (flag2)
			{
				bool flag3 = this.RaycastInterval < 0f;
				if (flag3)
				{
					this.RaycastInterval = this.RaycastIntervalSetting;
					bool flag4 = this.BlockRigidbody == null;
					if (flag4)
					{
						this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
					}
					bool flag5 = this.BlockRigidbody != null;
					if (flag5)
					{
						this.CenterOfComponent = this.BlockRigidbody.worldCenterOfMass;
						Vector3 forward = base.transform.forward;
						bool flag6 = forward != new Vector3(0f, 1f, 0f);
						if (flag6)
						{
							this.ScanFoward = new Vector3(forward.x, 0f, forward.z).normalized;
						}
						bool flag7 = this.top_span <= 0;
						if (flag7)
						{
							Vector3 vector = this.CenterOfComponent;
							RaycastHit[] array = new RaycastHit[4];
							float num = 0f;
							float num2 = 30f;
							float num3 = this.CenterOfComponent.y - 1f;
							bool flag8 = num3 < this.WaterHeight;
							if (flag8)
							{
								for (int i = 0; i < 4; i++)
								{
									bool flag9 = true;
									Vector3 vector2 = this.ScanAxis3[i] * this.ScanFoward * 0.3f;
									bool flag10 = Physics.Raycast(vector + vector2, Vector3.up, out array[i], num2, this.LayerMask, (QueryTriggerInteraction)1);
									while (flag9)
									{
										bool flag11 = flag10;
										if (flag11)
										{
											bool flag12 = array[i].rigidbody;
											if (flag12)
											{
												bool flag13 = array[i].rigidbody == this.HostRigidbody;
												if (flag13)
												{
													vector += Vector3.up;
													flag10 = Physics.Raycast(vector + vector2, Vector3.up, out array[i], num2, this.LayerMask, (QueryTriggerInteraction)1);
												}
												else
												{
													flag9 = false;
												}
											}
											else
											{
												flag9 = false;
											}
										}
										else
										{
											flag9 = false;
										}
									}
									bool flag14 = flag10;
									if (flag14)
									{
										num += 1f;
									}
								}
								bool flag15 = num > 1f;
								if (flag15)
								{
									this.volumelength = Math.Max(Math.Max(array[0].distance, array[1].distance), Math.Max(array[2].distance, array[3].distance));
								}
								else
								{
									this.volumelength = 0f;
								}
							}
							this.top_span = 1;
						}
						else
						{
							this.top_span--;
						}
						bool flag16 = this.volume_span <= 0;
						if (flag16)
						{
							Vector3 vector3 = this.CenterOfComponent + new Vector3(0f, this.volumelength / 2f, 0f);
							float num4 = 30f;
							float num5 = this.CenterOfComponent.y - 5f;
							bool flag17 = num5 < this.WaterHeight && (double)this.volumelength > 0.5;
							if (flag17)
							{
								switch (this.anglecount)
								{
								case 0:
									this.VolumeHitcount = 0;
									for (int j = 0; j < 4; j++)
									{
										bool flag18 = false;
										Vector3 vector4 = this.ScanAxis2[j] * this.ScanFoward + Vector3.up * 1.2f;
										RaycastHit[] array2 = Physics.RaycastAll(vector3, vector4, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int k = 0; k < array2.Length; k++)
										{
											bool flag19 = array2[k].rigidbody;
											if (flag19)
											{
												bool flag20 = array2[k].rigidbody != this.HostRigidbody;
												if (flag20)
												{
													flag18 = true;
												}
											}
										}
										bool flag21 = flag18;
										if (flag21)
										{
											this.VolumeHitcount++;
										}
									}
									break;
								case 1:
									for (int l = 4; l < 8; l++)
									{
										bool flag22 = false;
										Vector3 vector5 = this.ScanAxis2[l] * this.ScanFoward + Vector3.up * 1.2f;
										RaycastHit[] array3 = Physics.RaycastAll(vector3, vector5, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int m = 0; m < array3.Length; m++)
										{
											bool flag23 = array3[m].rigidbody;
											if (flag23)
											{
												bool flag24 = array3[m].rigidbody != this.HostRigidbody;
												if (flag24)
												{
													flag22 = true;
												}
											}
										}
										bool flag25 = flag22;
										if (flag25)
										{
											this.VolumeHitcount++;
										}
									}
									break;
								case 2:
								{
									for (int n = 8; n < 12; n++)
									{
										bool flag26 = false;
										Vector3 vector6 = this.ScanAxis2[n] * this.ScanFoward + Vector3.up * 1.2f;
										RaycastHit[] array4 = Physics.RaycastAll(vector3, vector6, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int num6 = 0; num6 < array4.Length; num6++)
										{
											bool flag27 = array4[num6].rigidbody;
											if (flag27)
											{
												bool flag28 = array4[num6].rigidbody != this.HostRigidbody;
												if (flag28)
												{
													flag26 = true;
												}
											}
										}
										bool flag29 = flag26;
										if (flag29)
										{
											this.VolumeHitcount++;
										}
									}
									bool flag30 = this.VolumeHitcount >= 9;
									if (flag30)
									{
										this.hasvolume[0] = true;
									}
									else
									{
										this.hasvolume[0] = false;
									}
									break;
								}
								case 3:
									this.VolumeHitcount = 0;
									for (int num7 = 0; num7 < 4; num7++)
									{
										bool flag31 = false;
										Vector3 vector7 = this.ScanAxis2[num7] * this.ScanFoward;
										RaycastHit[] array5 = Physics.RaycastAll(vector3, vector7, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int num8 = 0; num8 < array5.Length; num8++)
										{
											bool flag32 = array5[num8].rigidbody;
											if (flag32)
											{
												flag31 = true;
											}
										}
										bool flag33 = flag31;
										if (flag33)
										{
											this.VolumeHitcount++;
										}
									}
									break;
								case 4:
									for (int num9 = 4; num9 < 8; num9++)
									{
										bool flag34 = false;
										Vector3 vector8 = this.ScanAxis2[num9] * this.ScanFoward;
										RaycastHit[] array6 = Physics.RaycastAll(vector3, vector8, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int num10 = 0; num10 < array6.Length; num10++)
										{
											bool flag35 = array6[num10].rigidbody;
											if (flag35)
											{
												flag34 = true;
											}
										}
										bool flag36 = flag34;
										if (flag36)
										{
											this.VolumeHitcount++;
										}
									}
									break;
								case 5:
								{
									for (int num11 = 8; num11 < 12; num11++)
									{
										bool flag37 = false;
										Vector3 vector9 = this.ScanAxis2[num11] * this.ScanFoward;
										RaycastHit[] array7 = Physics.RaycastAll(vector3, vector9, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int num12 = 0; num12 < array7.Length; num12++)
										{
											bool flag38 = array7[num12].rigidbody;
											if (flag38)
											{
												flag37 = true;
											}
										}
										bool flag39 = flag37;
										if (flag39)
										{
											this.VolumeHitcount++;
										}
									}
									bool flag40 = this.VolumeHitcount >= 9;
									if (flag40)
									{
										this.hasvolume[1] = true;
									}
									else
									{
										this.hasvolume[1] = false;
									}
									break;
								}
								case 6:
									this.VolumeHitcount = 0;
									for (int num13 = 0; num13 < 4; num13++)
									{
										bool flag41 = false;
										Vector3 vector10 = this.ScanAxis2[num13] * this.ScanFoward - Vector3.up * 1.2f;
										RaycastHit[] array8 = Physics.RaycastAll(vector3, vector10, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int num14 = 0; num14 < array8.Length; num14++)
										{
											bool flag42 = array8[num14].rigidbody;
											if (flag42)
											{
												flag41 = true;
											}
										}
										bool flag43 = flag41;
										if (flag43)
										{
											this.VolumeHitcount++;
										}
									}
									break;
								case 7:
									for (int num15 = 4; num15 < 8; num15++)
									{
										bool flag44 = false;
										Vector3 vector11 = this.ScanAxis2[num15] * this.ScanFoward - Vector3.up * 1.2f;
										RaycastHit[] array9 = Physics.RaycastAll(vector3, vector11, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int num16 = 0; num16 < array9.Length; num16++)
										{
											bool flag45 = array9[num16].rigidbody;
											if (flag45)
											{
												flag44 = true;
											}
										}
										bool flag46 = flag44;
										if (flag46)
										{
											this.VolumeHitcount++;
										}
									}
									break;
								case 8:
								{
									for (int num17 = 8; num17 < 12; num17++)
									{
										bool flag47 = false;
										Vector3 vector12 = this.ScanAxis2[num17] * this.ScanFoward - Vector3.up * 1.2f;
										RaycastHit[] array10 = Physics.RaycastAll(vector3, vector12, num4, this.LayerMask, (QueryTriggerInteraction)1);
										for (int num18 = 0; num18 < array10.Length; num18++)
										{
											bool flag48 = array10[num18].rigidbody;
											if (flag48)
											{
												flag47 = true;
											}
										}
										bool flag49 = flag47;
										if (flag49)
										{
											this.VolumeHitcount++;
										}
									}
									bool flag50 = this.VolumeHitcount >= 9;
									if (flag50)
									{
										this.hasvolume[2] = true;
									}
									else
									{
										this.hasvolume[2] = false;
									}
									break;
								}
								}
							}
							this.anglecount++;
							bool flag51 = this.anglecount > 10;
							if (flag51)
							{
								this.anglecount = 0;
							}
							this.volume_span = 1;
						}
						else
						{
							this.volume_span--;
						}
					}
				}
				else
				{
					this.RaycastInterval -= Time.deltaTime;
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00009D5C File Offset: 0x00007F5C
		public void FixedUpdate()
		{
			bool flag = !StatMaster.isMP || StatMaster.isHosting || StatMaster.isLocalSim || (StatMaster.isClient && StatMaster.InLocalPlayMode);
			bool flag2 = this.Wenable && this.WBenable && (StatMaster.levelSimulating && flag);
			if (flag2)
			{
				bool flag3 = this.BlockRigidbody == null;
				if (flag3)
				{
					this.BlockRigidbody = base.transform.GetComponent<Rigidbody>();
				}
				bool flag4 = this.BlockRigidbody != null;
				if (flag4)
				{
					this.CenterOfComponent = this.BlockRigidbody.worldCenterOfMass;
					bool flag5 = this.hasvolume[0] && this.hasvolume[1] && this.hasvolume[2] && (double)this.volumelength > 0.5;
					if (flag5)
					{
						this.hasvolumess = 60;
					}
					else
					{
						this.hasvolumess--;
						this.volumelength = 0f;
					}
					bool flag6 = this.hasvolumess > 0;
					if (flag6)
					{
						bool flag7 = this.volumefloatinpow < this.Maxfloating;
						if (flag7)
						{
							this.volumefloatinpow += 0.5f;
						}
						else
						{
							this.volumefloatinpow = this.Maxfloating;
						}
					}
					else
					{
						this.hasvolumess = 0;
						bool flag8 = this.volumefloatinpow > 0f;
						if (flag8)
						{
							this.volumefloatinpow -= 0.2f;
						}
						else
						{
							this.volumefloatinpow = 0f;
						}
					}
					float num = this.CenterOfComponent.y - 0.5f;
					bool flag9 = num < this.WaterHeight;
					if (flag9)
					{
						bool flag10 = this.volumelengthReg < this.volumelength;
						if (flag10)
						{
							this.volumelengthReg += 0.02f;
						}
						else
						{
							bool flag11 = this.volumelengthReg > this.volumelength;
							if (flag11)
							{
								this.volumelengthReg -= 0.02f;
								bool flag12 = this.volumelengthReg <= 0f;
								if (flag12)
								{
									this.volumelengthReg = 0f;
								}
							}
						}
						bool flag13 = this.isFlatYFloat;
						if (flag13)
						{
							float num2 = Vector3.Angle(Vector3.up, base.transform.forward);
							bool flag14 = num2 > 90f;
							if (flag14)
							{
								num2 = 180f - num2;
							}
							float num3 = Vector3.Angle(Vector3.up, base.transform.right);
							bool flag15 = num3 > 90f;
							if (flag15)
							{
								num3 = 180f - num3;
							}
							float val = this.WaterHeight - num;
							this.BlockRigidbody.AddForce(Vector3.up * this.volumefloatinpow * (0.5f + 0.5f * (num2 / 90f) * (num3 / 90f)) * Math.Min(this.volumelengthReg, val));
						}
						else
						{
							float val2 = this.WaterHeight - num;
							this.BlockRigidbody.AddForce(Vector3.up * this.volumefloatinpow * Math.Min(this.volumelengthReg, val2));
						}
						bool flag16 = this.volumefloatinpow > 0f;
						if (flag16)
						{
							bool waterBouncyDebug = AdCustomModuleMod.mod4.WaterBouncyDebug;
							if (waterBouncyDebug)
							{
								bool flag17 = !base.transform.GetComponent<LineRenderer>();
								if (flag17)
								{
									this.lineRenderer = base.gameObject.AddComponent<LineRenderer>();
									Material material = new Material(Shader.Find("Unlit/Color"));
									material.SetVector("_Color", Color.green);
									this.lineRenderer.material = material;
									this.debugline = true;
								}
								this.lineRenderer.SetVertexCount(2);
								this.lineRenderer.SetColors(Color.green, Color.green);
								this.lineRenderer.SetWidth(0.2f, 0.2f);
								bool flag18 = this.isFlatYFloat;
								float num6;
								if (flag18)
								{
									float num4 = Vector3.Angle(Vector3.up, base.transform.forward);
									bool flag19 = num4 > 90f;
									if (flag19)
									{
										num4 = 180f - num4;
									}
									float num5 = Vector3.Angle(Vector3.up, base.transform.right);
									bool flag20 = num5 > 90f;
									if (flag20)
									{
										num5 = 180f - num5;
									}
									float val3 = this.WaterHeight - num;
									num6 = this.volumefloatinpow / this.Maxfloating * (float)Math.Cos(3.141592653589793 * (double)(90f - num4) / 180.0) * (float)Math.Cos(3.141592653589793 * (double)(90f - num5) / 180.0) * Math.Min(this.volumelengthReg, val3);
								}
								else
								{
									float val4 = this.WaterHeight - num;
									num6 = this.volumefloatinpow / this.Maxfloating * Math.Min(this.volumelengthReg, val4);
								}
								Vector3[] positions = new Vector3[]
								{
									this.CenterOfComponent - Vector3.up * num6,
									this.CenterOfComponent
								};
								this.lineRenderer.SetPositions(positions);
							}
							else
							{
								bool flag21 = this.debugline;
								if (flag21)
								{
									this.lineRenderer = base.transform.GetComponent<LineRenderer>();
									Object.Destroy(this.lineRenderer);
									this.debugline = false;
								}
							}
						}
						else
						{
							bool flag22 = this.debugline;
							if (flag22)
							{
								bool flag23 = base.transform.GetComponent<LineRenderer>() && this.debugline;
								if (flag23)
								{
									this.lineRenderer = base.transform.GetComponent<LineRenderer>();
									Object.Destroy(this.lineRenderer);
									this.debugline = false;
								}
							}
						}
					}
					else
					{
						this.volumelengthReg = 0f;
						bool flag24 = base.transform.GetComponent<LineRenderer>();
						if (flag24)
						{
							this.lineRenderer = base.transform.GetComponent<LineRenderer>();
							Object.Destroy(this.lineRenderer);
						}
					}
				}
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000A49C File Offset: 0x0000869C
		// Note: this type is marked as 'beforefieldinit'.
		static UWaterVolumeBehaviour()
		{
			Dictionary<BlockType, float> dictionary = new Dictionary<BlockType, float>();
			dictionary[(BlockType)3] = 30f;
			dictionary[(BlockType)25] = 10f;
			dictionary[(BlockType)34] = 50f;
			UWaterVolumeBehaviour.block_float = dictionary;
		}

		// Token: 0x040000B6 RID: 182
		private static readonly List<BlockType> FlatYFloatList = new List<BlockType> { (BlockType)3, (BlockType)25, (BlockType)34 };

		// Token: 0x040000B7 RID: 183
		private static readonly Dictionary<BlockType, float> block_float;

		// Token: 0x040000B8 RID: 184
		public BlockBehaviour ObjectBehavior;

		// Token: 0x040000B9 RID: 185
		public Rigidbody BlockRigidbody;

		// Token: 0x040000BA RID: 186
		public Rigidbody HostRigidbody;

		// Token: 0x040000BB RID: 187
		public Vector3 CenterOfComponent;

		// Token: 0x040000BC RID: 188
		private Vector3 ScanFoward;

		// Token: 0x040000BD RID: 189
		private int LayerMask = 33574913;

		// Token: 0x040000BE RID: 190
		private int volume_span = 0;

		// Token: 0x040000BF RID: 191
		private bool[] hasvolume = new bool[3];

		// Token: 0x040000C0 RID: 192
		private int hasvolumess = 0;

		// Token: 0x040000C1 RID: 193
		private float Maxfloating = 0f;

		// Token: 0x040000C2 RID: 194
		private int anglecount = 0;

		// Token: 0x040000C3 RID: 195
		private int top_span = 0;

		// Token: 0x040000C4 RID: 196
		private bool isFlatXBlock = false;

		// Token: 0x040000C5 RID: 197
		private float volumefloatinpow = 0f;

		// Token: 0x040000C6 RID: 198
		private float volumelength = 0f;

		// Token: 0x040000C7 RID: 199
		private float volumelengthReg = 0f;

		// Token: 0x040000C8 RID: 200
		private LineRenderer lineRenderer;

		// Token: 0x040000C9 RID: 201
		public bool isFlatYFloat = false;

		// Token: 0x040000CA RID: 202
		public int OwnerID;

		// Token: 0x040000CB RID: 203
		public Player PlayerData;

		// Token: 0x040000CC RID: 204
		public bool Localflag = false;

		// Token: 0x040000CD RID: 205
		private bool debugline = false;

		// Token: 0x040000CE RID: 206
		public float slaveblockfloating = 0f;

		// Token: 0x040000CF RID: 207
		public bool slaveflag = false;

		// Token: 0x040000D0 RID: 208
		public float drag = 0.08f;

		// Token: 0x040000D1 RID: 209
		public float floating = 1.5f;

		// Token: 0x040000D2 RID: 210
		private float RaycastIntervalSetting = 0.05f;

		// Token: 0x040000D3 RID: 211
		private float RaycastInterval;

		// Token: 0x040000D4 RID: 212
		private int VolumeHitcount = 0;

		// Token: 0x040000D5 RID: 213
		private Vector3[] ScanAxis = new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, -1f, 0f)
		};

		// Token: 0x040000D6 RID: 214
		private Quaternion[] ScanAxis2;

		// Token: 0x040000D7 RID: 215
		private Quaternion[] ScanAxis3;
	}
}
