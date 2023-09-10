using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using Modding.Modules;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;
namespace skpCustomModule
{
	// Token: 0x0200006C RID: 108
	public class AdCustomModuleMod : MonoBehaviour
	{
		// Token: 0x0600027D RID: 637 RVA: 0x00023050 File Offset: 0x00021250
		public void Awake()
		{
			AdCustomModuleMod.mod = SingleInstance<AdCustomModule>.Instance;
			AdCustomModuleMod.mod.name = "AdCustomModuleMod";
			Object.DontDestroyOnLoad(AdCustomModuleMod.mod);
			AdCustomModuleMod.mod2 = SingleInstance<AdShootingModule>.Instance;
			AdCustomModuleMod.mod2.name = "AdShootingModuleMod";
			Object.DontDestroyOnLoad(AdCustomModuleMod.mod2);
			AdCustomModuleMod.mod3 = SingleInstance<ProjectileLoader>.Instance;
			AdCustomModuleMod.mod3.name = "ProjectileLoader";
			Object.DontDestroyOnLoad(AdCustomModuleMod.mod3);
			AdCustomModuleMod.mod4 = SingleInstance<SkyBoxChanger>.Instance;
			AdCustomModuleMod.mod4.name = "SkyBoxChanger";
			Object.DontDestroyOnLoad(AdCustomModuleMod.mod4);
			AdCustomModuleMod.mod5 = SingleInstance<AdSkinLoader>.Instance;
			AdCustomModuleMod.mod5.name = "AdSkinLoader";
			Object.DontDestroyOnLoad(AdCustomModuleMod.mod5);
			CustomModules.AddBlockModule<AdBlockModule, AdBlockBehaviour>("AdBlockProp", true);
			CustomModules.AddBlockModule<AdShootingProp, AdShootingBehavour>("AdShootingProp", true);
			CustomModules.AddBlockModule<AdAnalogSteeringModule, AdAnalogSteeringModuleBehaviour>("AdAnalogSteeringProp", true);
			CustomModules.AddBlockModule<AdAnalogSpinningModule, AdAnalogSpinningModuleBehaviour>("AdAnalogSpinningProp", true);
			AdCustomModuleMod.Frame = 0U;
			AdCustomModuleMod.preFrame = 0U;
			AdCustomModuleMod.msgShooting = ModNetworking.CreateMessageType(new DataType[] { (DataType)12, (DataType)12 });
			ModNetworking.CallbacksWrapper callbacks = ModNetworking.Callbacks;
			MessageType messageType = AdCustomModuleMod.msgShooting;
			ModNetworking.CallbacksWrapper callbacksWrapper = callbacks;
			ModNetworking.CallbacksWrapper callbacksWrapper2 = callbacksWrapper;
			ModNetworking.CallbacksWrapper callbacksWrapper3 = callbacksWrapper2;
			MessageType messageType2 = messageType;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient)
				{
					AdCustomModuleMod.CompData01 = (byte[])message.GetData(0);
					this.FrameArray = (byte[])message.GetData(1);
					AdCustomModuleMod.Frame = BitConverter.ToUInt32(this.FrameArray, 0);
					if (AdCustomModuleMod.preFrame != AdCustomModuleMod.Frame)
					{
						if (AdCustomModuleMod.Frame != AdCustomModuleMod.preFrame + 1U)
						{
							uint num = AdCustomModuleMod.preFrame;
						}
						AdCustomModuleMod.mod2.PMcomponent.Updateflag = true;
						AdCustomModuleMod.mod2.PMcomponent.UpdateData01 = AdCustomModuleMod.CompData01;
						AdCustomModuleMod.mod2.PMcomponent.UpdateFrameNum = AdCustomModuleMod.Frame - AdCustomModuleMod.preFrame;
						AdCustomModuleMod.mod2.PMcomponent.UpdateFrame = AdCustomModuleMod.Frame;
						AdCustomModuleMod.preFrame = AdCustomModuleMod.Frame;
					}
				}
			};
			AdCustomModuleMod.msgPoolinfoCall = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)6 });
			ModNetworking.CallbacksWrapper callbacks2 = ModNetworking.Callbacks;
			MessageType messageType3 = AdCustomModuleMod.msgPoolinfoCall;
			callbacksWrapper = callbacks2;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType3;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					int callID = (int)message.GetData(0);
					string text = (string)message.GetData(1);
					List<int> list = new List<int>(AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Keys);
					int num = new List<string>(AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Values).IndexOf(text);
					if (num >= 0)
					{
						int keynum = list[num];
						AdCustomModuleMod.mod2.PMcomponent.Keynum = keynum;
						AdCustomModuleMod.mod2.PMcomponent.CallBlock = text;
						AdCustomModuleMod.mod2.PMcomponent.CallID = callID;
						AdCustomModuleMod.mod2.PMcomponent.ProjectileMatching = true;
					}
				}
			};
			AdCustomModuleMod.msgPoolinfoCallBack = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)6, (DataType)2 });
			ModNetworking.CallbacksWrapper callbacks3 = ModNetworking.Callbacks;
			MessageType messageType4 = AdCustomModuleMod.msgPoolinfoCallBack;
			callbacksWrapper = callbacks3;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType4;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient)
				{
					AdCustomModuleMod.msgPoolinfoCallBackChecker = true;
					int key = (int)message.GetData(0);
					string item = (string)message.GetData(1);
					if ((int)message.GetData(2) == AdCustomModuleMod.LocalPlayerNetworkID)
					{
						List<int> list = new List<int>(AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Keys);
						int index = new List<string>(AdCustomModuleMod.mod2.AdShootingModuleProjectiles.Values).IndexOf(item);
						int value = list[index];
						if (AdCustomModuleMod.mod2.ProjectilePoolMaching.ContainsKey(key))
						{
							AdCustomModuleMod.mod2.ProjectilePoolMaching[key] = value;
							return;
						}
						AdCustomModuleMod.mod2.ProjectilePoolMaching.Add(key, value);
					}
				}
			};
			AdCustomModuleMod.msgSkyBoxData = ModNetworking.CreateMessageType(new DataType[]
			{
				(DataType) 2,
				default(DataType),
				default(DataType),
				default(DataType),
                (DataType)4
			});
			ModNetworking.CallbacksWrapper callbacks4 = ModNetworking.Callbacks;
			MessageType messageType5 = AdCustomModuleMod.msgSkyBoxData;
			callbacksWrapper = callbacks4;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType5;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient)
				{
					AdCustomModuleMod.skyboxIndex = (int)message.GetData(0);
					AdCustomModuleMod.FloorDeactive = (bool)message.GetData(1);
					AdCustomModuleMod.ExpandFloor = (bool)message.GetData(2);
					AdCustomModuleMod.ExExpandFloor = (bool)message.GetData(3);
					AdCustomModuleMod.scale = (float)message.GetData(4);
					AdCustomModuleMod.skyboxchangeFlag = true;
				}
			};
			AdCustomModuleMod.msgSkyBoxChecker = ModNetworking.CreateMessageType(new DataType[1]);
			ModNetworking.CallbacksWrapper callbacks5 = ModNetworking.Callbacks;
			MessageType messageType6 = AdCustomModuleMod.msgSkyBoxChecker;
			callbacksWrapper = callbacks5;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType6;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					AdCustomModuleMod.skyboxchecker = (bool)message.GetData(0);
				}
			};
			AdCustomModuleMod.msgSkyBoxCallBackNone = ModNetworking.CreateMessageType(new DataType[1]);
			ModNetworking.CallbacksWrapper callbacks6 = ModNetworking.Callbacks;
			MessageType messageType7 = AdCustomModuleMod.msgSkyBoxCallBackNone;
			callbacksWrapper = callbacks6;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType7;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient)
				{
					AdCustomModuleMod.skyboxcheckerCallBack = (bool)message.GetData(0);
				}
			};
			AdCustomModuleMod.msgWaterData = ModNetworking.CreateMessageType(new DataType[]
			{
				default(DataType),
				default(DataType),
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4,
				(DataType)4
			});
			ModNetworking.CallbacksWrapper callbacks7 = ModNetworking.Callbacks;
			MessageType messageType8 = AdCustomModuleMod.msgWaterData;
			callbacksWrapper = callbacks7;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType8;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient)
				{
					AdCustomModuleMod.waterEnable = (bool)message.GetData(0);
					AdCustomModuleMod.waterFloatingEnable = (bool)message.GetData(1);
					for (int i = 0; i < 14; i++)
					{
						AdCustomModuleMod.waterinfo1[i] = (float)message.GetData(i + 2);
					}
					for (int j = 0; j < 3; j++)
					{
						AdCustomModuleMod.waterinfo2[j] = (float)message.GetData(j + 16);
					}
					for (int k = 0; k < 20; k++)
					{
						AdCustomModuleMod.waterinfo3[k] = (float)message.GetData(k + 19);
					}
					AdCustomModuleMod.waterchangeFlag = true;
				}
			};
			AdCustomModuleMod.msgWaterChecker = ModNetworking.CreateMessageType(new DataType[1]);
			ModNetworking.CallbacksWrapper callbacks8 = ModNetworking.Callbacks;
			MessageType messageType9 = AdCustomModuleMod.msgWaterChecker;
			callbacksWrapper = callbacks8;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType9;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					AdCustomModuleMod.waterchecker = (bool)message.GetData(0);
				}
			};
			AdCustomModuleMod.msgMarkerOptionData = ModNetworking.CreateMessageType(new DataType[1]);
			ModNetworking.CallbacksWrapper callbacks9 = ModNetworking.Callbacks;
			MessageType messageType10 = AdCustomModuleMod.msgMarkerOptionData;
			callbacksWrapper = callbacks9;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType10;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				AdCustomModuleMod.markerOptionEnable = (bool)message.GetData(0);
				AdCustomModuleMod.markerOptionchangeFlag = true;
			};
			AdCustomModuleMod.msgMarkerOptionChecker = ModNetworking.CreateMessageType(new DataType[1]);
			ModNetworking.CallbacksWrapper callbacks10 = ModNetworking.Callbacks;
			MessageType messageType11 = AdCustomModuleMod.msgMarkerOptionChecker;
			callbacksWrapper = callbacks10;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType11;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					AdCustomModuleMod.markerOptionchecker = (bool)message.GetData(0);
				}
			};
			AdCustomModuleMod.msgWaterOptionData = ModNetworking.CreateMessageType(new DataType[1]);
			ModNetworking.CallbacksWrapper callbacks11 = ModNetworking.Callbacks;
			MessageType messageType12 = AdCustomModuleMod.msgWaterOptionData;
			callbacksWrapper = callbacks11;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType12;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient)
				{
					AdCustomModuleMod.waterSplashEnable = (bool)message.GetData(0);
					AdCustomModuleMod.waterSplashchangeFlag = true;
				}
			};
			AdCustomModuleMod.msgWaterOptionChecker = ModNetworking.CreateMessageType(new DataType[1]);
			ModNetworking.CallbacksWrapper callbacks12 = ModNetworking.Callbacks;
			MessageType messageType13 = AdCustomModuleMod.msgWaterOptionChecker;
			callbacksWrapper = callbacks12;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType13;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					AdCustomModuleMod.waterSplashchecker = (bool)message.GetData(0);
				}
			};
			AdCustomModuleMod.msgWaterSplashInit = ModNetworking.CreateMessageType(new DataType[] { (DataType)6, (DataType)12 });
			ModNetworking.CallbacksWrapper callbacks13 = ModNetworking.Callbacks;
			MessageType messageType14 = AdCustomModuleMod.msgWaterSplashInit;
			callbacksWrapper = callbacks13;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType14;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient && !StatMaster.isLocalSim)
				{
					string key = (string)message.GetData(0);
					byte[] array = (byte[])message.GetData(1);
					if (AdCustomModuleMod.mod6.BlockObjectContainer.ContainsKey(key))
					{
						UWaterBehaviourScript uwaterBehaviourScript = AdCustomModuleMod.mod6.BlockObjectContainer[key];
						Vector3 networkBlockBounds = Vector3.one;
						NetworkCompression.DecompressPosition(array, 0, out networkBlockBounds);
						uwaterBehaviourScript.NetworkBlockBounds = networkBlockBounds;
					}
				}
			};
			AdCustomModuleMod.msgWaterSplash = ModNetworking.CreateMessageType(new DataType[]
			{
				(DataType) 6,
				default(DataType),
				(DataType) 4,
				(DataType) 12
			});
			ModNetworking.CallbacksWrapper callbacks14 = ModNetworking.Callbacks;
			MessageType messageType15 = AdCustomModuleMod.msgWaterSplash;
			callbacksWrapper = callbacks14;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType15;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient && !StatMaster.isLocalSim)
				{
					string key = (string)message.GetData(0);
					bool networkEffectFlag = (bool)message.GetData(1);
					float networkSpeed = (float)message.GetData(2);
					byte[] array = (byte[])message.GetData(3);
					if (AdCustomModuleMod.mod6.BlockObjectContainer.ContainsKey(key))
					{
						UWaterBehaviourScript uwaterBehaviourScript = AdCustomModuleMod.mod6.BlockObjectContainer[key];
						Vector3 networkVector = Vector3.one;
						NetworkCompression.DecompressPosition(array, 0, out networkVector);
						uwaterBehaviourScript.NetworkEffectFlag = networkEffectFlag;
						uwaterBehaviourScript.NetworkSpeed = networkSpeed;
						uwaterBehaviourScript.NetworkVector = networkVector;
					}
				}
			};
			AdCustomModuleMod.msgLockOnData = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)3 });
			ModNetworking.CallbacksWrapper callbacks15 = ModNetworking.Callbacks;
			MessageType messageType16 = AdCustomModuleMod.msgLockOnData;
			callbacksWrapper = callbacks15;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType16;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					int key = (int)message.GetData(0);
					int[] array = (int[])message.GetData(1);
					List<int> list = new List<int>();
					for (int i = 0; i < array.Length; i++)
					{
						list.Add(array[i]);
					}
					if (AdCustomModuleMod.mod2.TaregtIdListContainer.ContainsKey(key))
					{
						AdCustomModuleMod.mod2.TaregtIdListContainer[key] = list;
						return;
					}
					AdCustomModuleMod.mod2.TaregtIdListContainer.Add(key, list);
				}
			};
			AdCustomModuleMod.msgSkininfoCall = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)6, (DataType)6 });
			ModNetworking.CallbacksWrapper callbacks16 = ModNetworking.Callbacks;
			MessageType messageType17 = AdCustomModuleMod.msgSkininfoCall;
			callbacksWrapper = callbacks16;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType17;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					int callProjectilePlayerID = (int)message.GetData(0);
					string callProjectileBlock = (string)message.GetData(1);
					string callProjectileSkin = (string)message.GetData(2);
					AdCustomModuleMod.mod2.PMcomponent.CallProjectilePlayerID = callProjectilePlayerID;
					AdCustomModuleMod.mod2.PMcomponent.CallProjectileBlock = callProjectileBlock;
					AdCustomModuleMod.mod2.PMcomponent.CallProjectileSkin = callProjectileSkin;
					AdCustomModuleMod.mod2.PMcomponent.ProjectileSkinMatching = true;
				}
			};
			AdCustomModuleMod.msgSkininfoCallBack = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)2, (DataType)6, (DataType)2, (DataType)6 });
			ModNetworking.CallbacksWrapper callbacks17 = ModNetworking.Callbacks;
			MessageType messageType18 = AdCustomModuleMod.msgSkininfoCallBack;
			callbacksWrapper = callbacks17;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType18;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient)
				{
					AdCustomModuleMod.msgSkininfoCallBackChecker = true;
					int num = (int)message.GetData(0);
					int key = (int)message.GetData(1);
					string CallBackBlockName = (string)message.GetData(2);
					int key2 = (int)message.GetData(3);
					string CallBackSkinName = (string)message.GetData(4);
					if (num == AdCustomModuleMod.LocalPlayerNetworkID)
					{
						KeyMachingTable keyMachingTable = AdCustomModuleMod.mod2.ProjectileSkinTable.List.FirstOrDefault((KeyMachingTable x) => x.Name == CallBackBlockName);
						keyMachingTable.Key = key;
						keyMachingTable.List.FirstOrDefault((KeyMachingTable c) => c.Name == CallBackSkinName).Key = key2;
					}
				}
			};
			AdCustomModuleMod.msgCoreSpeed = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)4 });
			ModNetworking.CallbacksWrapper callbacks18 = ModNetworking.Callbacks;
			MessageType messageType19 = AdCustomModuleMod.msgCoreSpeed;
			callbacksWrapper = callbacks18;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType19;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient && !StatMaster.isLocalSim)
				{
					int key = (int)message.GetData(0);
					float value = (float)message.GetData(1);
					if (!AdCustomModuleMod.mod2.CoreSpeedContainer.ContainsKey(key))
					{
						AdCustomModuleMod.mod2.CoreSpeedContainer.Add(key, value);
						return;
					}
					AdCustomModuleMod.mod2.CoreSpeedContainer[key] = value;
				}
			};
			AdCustomModuleMod.msgCoreAcceleration = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)4 });
			ModNetworking.CallbacksWrapper callbacks19 = ModNetworking.Callbacks;
			MessageType messageType20 = AdCustomModuleMod.msgCoreAcceleration;
			callbacksWrapper = callbacks19;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType20;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isClient && !StatMaster.isLocalSim)
				{
					int key = (int)message.GetData(0);
					float value = (float)message.GetData(1);
					if (!AdCustomModuleMod.mod2.CoreAccelerationContainer.ContainsKey(key))
					{
						AdCustomModuleMod.mod2.CoreAccelerationContainer.Add(key, value);
						return;
					}
					AdCustomModuleMod.mod2.CoreAccelerationContainer[key] = value;
				}
			};
			AdCustomModuleMod.msgJoyStickData = ModNetworking.CreateMessageType(new DataType[] { (DataType)2, (DataType)4, (DataType)4, (DataType)4, (DataType)4, (DataType)4, (DataType)4, (DataType)4, (DataType)4 });
			ModNetworking.CallbacksWrapper callbacks20 = ModNetworking.Callbacks;
			MessageType messageType21 = AdCustomModuleMod.msgJoyStickData;
			callbacksWrapper = callbacks20;
			callbacksWrapper2 = callbacksWrapper;
			callbacksWrapper3 = callbacksWrapper2;
			messageType2 = messageType21;
			callbacksWrapper3[messageType2] += delegate(Message message)
			{
				if (StatMaster.isHosting)
				{
					JoystickAxisInfo value = default(JoystickAxisInfo);
					int key = (int)message.GetData(0);
					value.LStick_X = (float)message.GetData(1);
					value.LStick_Y = (float)message.GetData(2);
					value.RStick_X = (float)message.GetData(3);
					value.RStick_Y = (float)message.GetData(4);
					value.Pad_X = (float)message.GetData(5);
					value.Pad_Y = (float)message.GetData(6);
					value.LTriger = (float)message.GetData(7);
					value.RTriger = (float)message.GetData(8);
					if (AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer.ContainsKey(key))
					{
						AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer[key] = value;
						return;
					}
					AdCustomModuleMod.mod4.AdSaveData.JoyAxisContainer.Add(key, value);
				}
			};
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00002F76 File Offset: 0x00001176

		// Token: 0x040004FA RID: 1274
		public static AdCustomModule mod;

		// Token: 0x040004FB RID: 1275
		public static AdShootingModule mod2;

		// Token: 0x040004FC RID: 1276
		public static ProjectileLoader mod3;

		// Token: 0x040004FD RID: 1277
		public static SkyBoxChanger mod4;

		// Token: 0x040004FE RID: 1278
		public static AdSkinLoader mod5;

		// Token: 0x040004FF RID: 1279
		public static AdWaterModule mod6;

		// Token: 0x04000500 RID: 1280
		public static MessageType msgShooting;

		// Token: 0x04000501 RID: 1281
		public static MessageType msgPoolinfoCall;

		// Token: 0x04000502 RID: 1282
		public static MessageType msgPoolinfoCallBack;

		// Token: 0x04000503 RID: 1283
		public static MessageType msgSkyBoxData;

		// Token: 0x04000504 RID: 1284
		public static MessageType msgSkyBoxChecker;

		// Token: 0x04000505 RID: 1285
		public static MessageType msgSkyBoxCallBackNone;

		// Token: 0x04000506 RID: 1286
		public static MessageType msgWaterData;

		// Token: 0x04000507 RID: 1287
		public static MessageType msgWaterChecker;

		// Token: 0x04000508 RID: 1288
		public static MessageType msgMarkerOptionData;

		// Token: 0x04000509 RID: 1289
		public static MessageType msgMarkerOptionChecker;

		// Token: 0x0400050A RID: 1290
		public static MessageType msgWaterOptionData;

		// Token: 0x0400050B RID: 1291
		public static MessageType msgWaterOptionChecker;

		// Token: 0x0400050C RID: 1292
		public static MessageType msgWaterSplash;

		// Token: 0x0400050D RID: 1293
		public static MessageType msgWaterSplashInit;

		// Token: 0x0400050E RID: 1294
		public static MessageType msgLockOnData;

		// Token: 0x0400050F RID: 1295
		public static MessageType msgSkininfoCall;

		// Token: 0x04000510 RID: 1296
		public static MessageType msgSkininfoCallBack;

		// Token: 0x04000511 RID: 1297
		public static MessageType msgCoreSpeed;

		// Token: 0x04000512 RID: 1298
		public static MessageType msgCoreAcceleration;

		// Token: 0x04000513 RID: 1299
		public static MessageType msgJoyStickData;

		// Token: 0x04000514 RID: 1300
		public static byte[] CompData01;

		// Token: 0x04000515 RID: 1301
		public static byte[] CompData02;

		// Token: 0x04000516 RID: 1302
		public static byte[] CompData03;

		// Token: 0x04000517 RID: 1303
		public static byte[] CompData04;

		// Token: 0x04000518 RID: 1304
		public static byte[] CompData05;

		// Token: 0x04000519 RID: 1305
		private byte[] FrameArray;

		// Token: 0x0400051A RID: 1306
		public static byte[] HostupdateData;

		// Token: 0x0400051B RID: 1307
		public static int num;

		// Token: 0x0400051C RID: 1308
		public static int offset;

		// Token: 0x0400051D RID: 1309
		public static bool msgcatchflag;

		// Token: 0x0400051E RID: 1310
		public static uint Frame;

		// Token: 0x0400051F RID: 1311
		public static uint preFrame;

		// Token: 0x04000520 RID: 1312
		public static int[] DicKey;

		// Token: 0x04000521 RID: 1313
		public static string[] DicValue;

		// Token: 0x04000522 RID: 1314
		public static int LocalPlayerNetworkID;

		// Token: 0x04000523 RID: 1315
		public static bool msgPoolinfoCallBackChecker = true;

		// Token: 0x04000524 RID: 1316
		public static bool msgSkininfoCallBackChecker = true;

		// Token: 0x04000525 RID: 1317
		public static bool skyboxchangeFlag = false;

		// Token: 0x04000526 RID: 1318
		public static bool waterchangeFlag = false;

		// Token: 0x04000527 RID: 1319
		public static bool waterEnable = false;

		// Token: 0x04000528 RID: 1320
		public static bool waterFloatingEnable = false;

		// Token: 0x04000529 RID: 1321
		public static int skyboxIndex = 0;

		// Token: 0x0400052A RID: 1322
		public static float[] skyboxinfo = new float[5];

		// Token: 0x0400052B RID: 1323
		public static float[] skyboxinfo2 = new float[3];

		// Token: 0x0400052C RID: 1324
		public static float[] waterinfo1 = new float[14];

		// Token: 0x0400052D RID: 1325
		public static float[] waterinfo2 = new float[3];

		// Token: 0x0400052E RID: 1326
		public static float[] waterinfo3 = new float[20];

		// Token: 0x0400052F RID: 1327
		public static bool skyboxchecker = false;

		// Token: 0x04000530 RID: 1328
		public static bool skyboxcheckerCallBack = false;

		// Token: 0x04000531 RID: 1329
		public static bool waterchecker = false;

		// Token: 0x04000532 RID: 1330
		public static bool markerOptionchecker = false;

		// Token: 0x04000533 RID: 1331
		public static bool markerOptionchangeFlag = false;

		// Token: 0x04000534 RID: 1332
		public static bool markerOptionEnable = false;

		// Token: 0x04000535 RID: 1333
		public static bool waterSplashchecker = false;

		// Token: 0x04000536 RID: 1334
		public static bool waterSplashchangeFlag = false;

		// Token: 0x04000537 RID: 1335
		public static bool waterSplashEnable = false;

		// Token: 0x04000538 RID: 1336
		public static bool FloorDeactive = false;

		// Token: 0x04000539 RID: 1337
		public static bool ExpandFloor = false;

		// Token: 0x0400053A RID: 1338
		public static bool ExExpandFloor = false;

		// Token: 0x0400053B RID: 1339
		public static int[] LockOnData = new int[10];

		// Token: 0x0400053C RID: 1340
		public static bool[] LockOnDataChangeFlag = new bool[10];

		// Token: 0x0400053D RID: 1341
		public static float scale;
	}
}
